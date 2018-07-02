using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using RAGE.Analytics;
using System;
using AssetPackage;
using System.Linq;

namespace uAdventure.Runner
{
    [Serializable]
    public class GameState
    {
        //###########################################################################
        //########################### GAME STATE HANDLING ###########################
        //###########################################################################

        /// This class is used for serialization purposes only
        [Serializable]
        protected class ContextList
        {
            [SerializeField]
            protected List<ElementReference> elementReferences;
            public List<ElementReference> ElementReferences { get { return elementReferences; } set { elementReferences = value; } }
        }

        private readonly AdventureData data;
        [SerializeField]
        private ElementReference playerContext;

        [SerializeField]
        private int currentChapter = 0;
        [SerializeField]
        private string currentTarget = "";
        [SerializeField]
        private string lastTarget = "";
        [SerializeField]
        private List<string> removedElements;
        [SerializeField]
        private List<string> inventoryItems;

        private Dictionary<string, List<ElementReference>> elementContexts;
        private Dictionary<string, int> varFlags;

        [SerializeField]
        private List<string> varFlagKeys;
        [SerializeField]
        private List<int> varFlagValues;

        [SerializeField]
        private List<string> elementContextsKeys;
        [SerializeField]
        private List<ContextList> elementContextsValues;

        private Stack<List<KeyValuePair<string, int>>> varFlagChangeAmbits;

        public AdventureData Data
        {
            get { return data; }
        }

        public string CurrentTarget
        {
            get
            {
                return currentTarget;
            }
            set
            {
                lastTarget = currentTarget;
                currentTarget = value;
            }
        }


        public ElementReference PlayerContext
        {
            get
            {
                if (playerContext == null)
                {
                    InitPlayerContext();
                }

                return playerContext;
            }
        }

        public Player Player
        {
            get
            {
                return data.getChapters()[currentChapter].getPlayer();
            }
        }

        public bool IsFirstPerson
        {
            get
            {
                return data.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON;
            }
        }

        public GameState(AdventureData data)
        {
            this.removedElements = new List<string>();
            this.inventoryItems = new List<string>();
            this.data = data;
            varFlags = new Dictionary<string, int>();
            elementContexts = new Dictionary<string, List<ElementReference>>();
            varFlagChangeAmbits = new Stack<List<KeyValuePair<string, int>>>();
        }
        
        public void OnGameSuspend()
        {
            if (data.isSaveOnSuspend())
            {
                SerializeTo("suspend_state");
            }
        }

        public void OnGameResume()
        {
            if (data.isSaveOnSuspend())
            {
                RestoreFrom("suspend_state");
            }
        }

        private void InitPlayerContext()
        {
            var scene = GetChapterTarget(CurrentTarget) as Scene;
            if (scene == null)
            {
                playerContext = new ElementReference("Player", 0, 0, -1);
            }
            else
            {
                var trajectory = scene.getTrajectory();

                var playerPosition = new Vector2(scene.getPositionX(), scene.getPositionY());
                var scale = scene.getPlayerScale();
                if (trajectory != null)
                {
                    Trajectory.Node pos = scene.getTrajectory().getInitial();
                    playerPosition = new Vector2(pos.getX(), pos.getY());
                    scale = pos.getScale();
                }

                playerContext = new ElementReference("Player", (int)playerPosition.x, (int)playerPosition.y, scene.getPlayerLayer());
                playerContext.setScale(scale);
            }
        }

        public int CheckFlag(string flag)
        {
            int ret = FlagCondition.FLAG_INACTIVE;
            if(varFlags.ContainsKey("flag_" + flag))
            {
                ret = varFlags["flag_" + flag];
            }
            return ret;
        }

        public void SetFlag(string name, int state)
        {
            varFlags["flag_" + name] = state;
            bool bstate = state == FlagCondition.FLAG_ACTIVE;
            int intVal = bstate ? 1 : 0;

            // TODO check this after this: https://github.com/e-ucm/unity-tracker/issues/29
            if (varFlagChangeAmbits.Count > 0)
            {
                varFlagChangeAmbits.Peek().Add(new KeyValuePair<string, int>(name, intVal));
            }
            else
            {
                TrackerAsset.Instance.setVar(name, intVal);
            }
            TimerController.Instance.CheckTimers();
            CompletablesController.Instance.ConditionChanged();
            Game.Instance.reRenderScene();
        }

        public int GetVariable(string var)
        {
            int ret = 0;

            if (varFlags.ContainsKey("var_" + var))
            {
                ret = varFlags["var_" + var];
            }
            return ret;
        }

        public void SetVariable(string name, int value)
        {
            varFlags["var_" + name] = value;

            if (varFlagChangeAmbits.Count > 0)
            {
                varFlagChangeAmbits.Peek().Add(new KeyValuePair<string, int>(name, value));
            }
            else
            {
                TrackerAsset.Instance.setVar(name, value);
            }

            CompletablesController.Instance.ConditionChanged();

            Game.Instance.reRenderScene();
        }

        public List<ElementReference> GetElementReferences(string sceneId)
        {
            if (!elementContexts.ContainsKey(sceneId))
            {
                var scene = data.getChapters()[currentChapter].getScene(sceneId);
                var elementReferences = new List<ElementReference>();
                elementReferences.AddRange(scene.getItemReferences()
                    .ConvertAll(r => r.Clone() as ElementReference));
                elementReferences.AddRange(scene.getAtrezzoReferences()
                    .ConvertAll(r => r.Clone() as ElementReference));
                elementReferences.AddRange(scene.getCharacterReferences()
                    .ConvertAll(r => r.Clone() as ElementReference));
                elementContexts[sceneId] = elementReferences;
            }

            return elementContexts[sceneId];
        }

        public int CheckGlobalState(string globalState)
        {
            int ret = GlobalStateCondition.GS_SATISFIED;
            GlobalState gs = data.getChapters()[currentChapter].getGlobalState(globalState);
            if (gs != null)
            {
                ret = ConditionChecker.check(gs);
            }
            return ret;
        }

        public Macro GetMacro(string id)
        {
            return data.getChapters()[currentChapter].getMacro(id);
        }

        public Conversation GetConversation(string id)
        {
            return data.getChapters()[currentChapter].getConversation(id);
        }

        public List<Timer> GetTimers()
        {
            return data.getChapters()[currentChapter].getTimers();
        }

        public void Move(string id, Vector2 position, int time = 0)
        {
            GameObject go = GameObject.Find(id);
            Movable m = go.GetComponent<Movable>();
            if (m != null)
            {
                m.setPosition(position);
            }
        }

        public NPC GetCharacter(string name)
        {
            return data.getChapters()[currentChapter].getCharacter(name);
        }

        public Item GetObject(string name)
        {
            return data.getChapters()[currentChapter].getItem(name);
        }

        public Atrezzo GetAtrezzo(string name)
        {
            return data.getChapters()[currentChapter].getAtrezzo(name);
        }

        public List<T> GetObjects<T>()
        {
            return data.getChapters()[currentChapter].getObjects<T>();
        }

        public T FindElement<T>(string id)
        {
            if(typeof(T) == typeof(Element))
            {
                Element r = ((Element) GetCharacter(id) ?? GetObject(id)) ?? GetAtrezzo(id);
                if (r != null)
                {
                    return (T)(r as object);
                }
            }

            var allObjects = data.getChapters()[currentChapter].getObjects<T>();

            return allObjects.Where(o => o is HasId)
                .FirstOrDefault(e => (e as HasId).getId() == id);
        }
        

        public bool IsCutscene(string sceneId)
        {
            return data.getChapters()[currentChapter].getObjects<Cutscene>().Exists(s => s.getId() == sceneId);
        }

        public IChapterTarget InitialChapterTarget
        {
            get
            {
                return data.getChapters()[currentChapter].getInitialChapterTarget();
            }
        }

        public IChapterTarget PreviousChapterTarget
        {
            get
            {
                return GetChapterTarget(lastTarget);
            }
        }

        public GeneralScene GetLastScene()
        {
            var scenes = data.getChapters()[currentChapter].getGeneralScenes();
            return scenes.Where(scene => scene is Slidescene)
                .FirstOrDefault(scene => ((Slidescene)scene).getNext() == Cutscene.ENDCHAPTER);
        }

        public List<Completable> GetCompletables()
        {
            return data.getChapters()[currentChapter].getCompletables();
        }

        public IChapterTarget GetChapterTarget(string runnerTargetId)
        {
            return data.getChapters()[currentChapter].getObjects<IChapterTarget>()
                .Find(o => o.getId() == runnerTargetId);
        }

        public List<string> GetRemovedElements()
        {
            return removedElements;
        }

        public void AddRemovedElement(string elementId)
        {
            removedElements.Add(elementId);
        }

        public bool RemoveRemovedElement(string elementId)
        {
            return removedElements.Remove(elementId);
        }

        // Inventory objects

        public List<string> GetInventoryItems()
        {
            return inventoryItems;
        }

        public void AddInventoryItem(string elementId)
        {
            inventoryItems.Add(elementId);
        }

        public bool RemoveInventoryItem(string elementId)
        {
            return inventoryItems.Remove(elementId);
        }

        // Var flag change ambits
        public void BeginChangeAmbit()
        {
            this.varFlagChangeAmbits.Push(new List<KeyValuePair<string, int>>());
        }

        public List<KeyValuePair<string, int>> EndChangeAmbit(bool appendToParent = false)
        {
            var currentAmbit = this.varFlagChangeAmbits.Pop();

            if (appendToParent)
            {
                if(varFlagChangeAmbits.Count > 0)
                {
                    foreach(var varChange in currentAmbit)
                        varFlagChangeAmbits.Peek().Add(varChange);
                }
                else
                {
                    foreach (var varChange in currentAmbit)
                        TrackerAsset.Instance.setVar(varChange.Key, varChange.Value);
                }
            }

            return currentAmbit;
        }

        public void EndChangeAmbitAsExtensions()
        {
            var currentChanges = EndChangeAmbit(false);
            foreach(var varChange in currentChanges)
            {
                TrackerAsset.Instance.setVar(varChange.Key, varChange.Value);
            }
        }

        public void SerializeTo(string field)
        {
            varFlagKeys = varFlags.Keys.ToList();
            varFlagValues = varFlags.Values.ToList();
            elementContextsKeys = elementContexts.Keys.ToList();
            elementContextsValues = elementContexts.Values.ToList()
                .ConvertAll(l => new ContextList() { ElementReferences = l });

            var json = JsonUtility.ToJson(this);
            Debug.Log(json);
            PlayerPrefs.SetString(field, JsonUtility.ToJson(this));
        }

        public void RestoreFrom(string field)
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(field), this);
            varFlags.Clear();
            for(int i = 0, totalVars = varFlagKeys.Count; i < totalVars; i++)
            {
                varFlags[varFlagKeys[i]] = varFlagValues[i];
            }
            elementContexts.Clear();
            for (int i = 0, totalVars = elementContextsKeys.Count; i < totalVars; i++)
            {
                elementContexts[elementContextsKeys[i]] = elementContextsValues[i].ElementReferences;
            }
        }
    }
}
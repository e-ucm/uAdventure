using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using System.Linq;
using UniRx;
using TinCan;
using Xasu;
using Xasu.HighLevel;
using Xasu.Util;

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

        public delegate void ConditionChangedDelegate(string condition, int value);

        public ConditionChangedDelegate OnConditionChanged;

        private List<string> runningSystems;
        private Dictionary<string, Memory> memories;
        private Dictionary<string, List<ElementReference>> elementContexts;
        private Dictionary<string, int> varFlags;
        private Stack<List<KeyValuePair<string, int>>> varFlagChangeAmbits;
        private Stack<StatementPromise> ambitTraces;

        #region SerializationFields
        [SerializeField]
        private List<string> varFlagKeys;
        [SerializeField]
        private List<int> varFlagValues;
        [SerializeField]
        private List<string> elementContextsKeys;
        [SerializeField]
        private List<ContextList> elementContextsValues;
        [SerializeField]
        private List<string> memoryKeys;
        [SerializeField]
        private List<Memory> memoryValues;
        #endregion

        public AdventureData Data
        {
            get { return data; }
        }

        public int CurrentChapter
        {
            get
            {
                return currentChapter;
            }
            set
            {
                currentChapter = value;
            }
        }

        public string CurrentTarget
        {
            get
            {
                return currentTarget;
            }
            set
            {
                if(!currentTarget.StartsWith("Simva."))
                {
                    lastTarget = currentTarget;
                }
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
                var currentScene = GetChapterTarget(CurrentTarget) as Scene;
                var hasHiddenPlayer = currentScene != null && currentScene.getPlayerLayer() == -2;

                return data.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON || hasHiddenPlayer;
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
            ambitTraces = new Stack<StatementPromise>();
            memories = new Dictionary<string, Memory>();
            currentChapter = 0;
            playerContext = null;
            currentTarget = data.getChapters()[currentChapter].getInitialChapterTarget().getId();
            lastTarget = null;
        }
        
        public void OnGameSuspend()
        {
            if (data.isSaveOnSuspend())
            {
                SerializeTo("save");
            }
        }

        public void OnGameResume()
        {
            if (data.isSaveOnSuspend())
            {
                RestoreFrom("save");
            }
        }

        private string GetGameRelativeUri(string name)
        {
            return data.getApplicationIdentifier() + "://" + name;
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

                playerContext = new ElementReference("Player", (int)playerPosition.x, (int)playerPosition.y, scene.getPlayerLayer())
                {
                    Scale = scale
                };
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
            var nameUri = GetGameRelativeUri(name);

            // TODO check this after this: https://github.com/e-ucm/unity-tracker/issues/29
            if (varFlagChangeAmbits.Count > 0)
            {
                varFlagChangeAmbits.Peek().Add(new KeyValuePair<string, int>(nameUri, intVal));
            }
            else
            {
                ExtensionsPool.AddResultExtension(nameUri, intVal);
            }
            TimerController.Instance.CheckTimers();
            if (OnConditionChanged != null)
            {
                OnConditionChanged(name, state);
            }
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
            var nameUri = GetGameRelativeUri(name);

            if (varFlagChangeAmbits.Count > 0)
            {
                varFlagChangeAmbits.Peek().Add(new KeyValuePair<string, int>(nameUri, value));
            }
            else
            {
                ExtensionsPool.AddResultExtension(nameUri, value);
            }

            if (OnConditionChanged != null)
            {
                OnConditionChanged(name, value);
            }
        }

        public Memory GetMemory(string id)
        {
            Memory ret = null;

            if (memories.ContainsKey(id))
            {
                ret = memories[id];
            }
            return ret;
        }

        public void SetMemory(string id, Memory memory)
        {
            memories[id] = memory;
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

        public void Move(string id, Vector2 position, int speed = 0, EffectHolderNode observer = null)
        {
            GameObject go = GameObject.Find(id);
            Mover m = go.GetComponent<Mover>();
            if (m != null)
            {
                if (speed == 0)
                {
                    m.MoveInstant(position);
                }
                else
                {
                    Action<object> endMovement = data =>
                    {
                        EffectHolderNode tmp = (EffectHolderNode)data;
                        tmp.doPulse();
                        if (!Game.Instance.IsRunningInBackground(tmp))
                        {
                            Game.Instance.ContinueEffectExecution();
                        }
                    };
                    m.MoveFreely(position, observer, data => endMovement(data), data => endMovement(data));
                }
            }else
            {
                ScenePositioner r = go.GetComponent<ScenePositioner>();
                r.Position = position;
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
            if (OnConditionChanged != null)
            {
                OnConditionChanged(null, 0);
            }
        }

        public bool RemoveRemovedElement(string elementId)
        {
            var removed = removedElements.Remove(elementId);
            if (OnConditionChanged != null)
            {
                OnConditionChanged(null, 0);
            }
            return removed;
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

        internal int ChangeAmbitCount { get { return varFlagChangeAmbits.Count; } }

        // Var flag change ambits
        public void BeginChangeAmbit(StatementPromise trace)
        {
            this.ambitTraces.Push(trace);
            this.varFlagChangeAmbits.Push(new List<KeyValuePair<string, int>>());
            Debug.Log("Opened change ambit " + varFlagChangeAmbits.Count);
        }

        public List<KeyValuePair<string, int>> EndChangeAmbit(bool appendToParent = false)
        {
            Debug.Log("Closed change ambit " + varFlagChangeAmbits.Count);
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
                    {
                        ExtensionsPool.AddResultExtension(varChange.Key, varChange.Value);
                    }
                }
            }

            return currentAmbit;
        }

        public void EndChangeAmbitAsExtensions(StatementPromise trace)
        {
            var currentAmbitTrace = this.ambitTraces.Pop();
            var currentChanges = EndChangeAmbit(false);
            if(currentAmbitTrace != trace)
            {
                Debug.LogError("Closed trace ambit is not the topmost trace!!");
            }

            foreach(var varChange in currentChanges)
            {
                ExtensionsPool.AddResultExtension(varChange.Key, varChange.Value);
            }
            if(trace != null)
            {
                trace.Statement.SetPoolExtensions();
            }
        }

        public void Restart()
        {
            currentChapter = 0;
            playerContext = null;
            currentTarget = data.getChapters()[currentChapter].getInitialChapterTarget().getId();
            lastTarget = null;
            removedElements.Clear();
            inventoryItems.Clear();
            elementContexts.Clear();
            varFlags.Clear();
            varFlagChangeAmbits.Clear();
            memories.Clear();
            
            InventoryManager.Instance.Restore();
        }

        public void SerializeTo(string field)
        {
            varFlagKeys = varFlags.Keys.ToList();
            varFlagValues = varFlags.Values.ToList();
            elementContextsKeys = elementContexts.Keys.ToList();
            elementContextsValues = elementContexts.Values.ToList()
                .ConvertAll(l => new ContextList() { ElementReferences = l });
            memoryKeys = memories.Keys.ToList();
            memoryValues = memories.Values.ToList();

            var json = JsonUtility.ToJson(this);
            Debug.Log(json);
            PlayerPrefs.SetString(field, JsonUtility.ToJson(this));
            PlayerPrefs.Save();
        }

        public void RestoreFrom(string field)
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(field)))
            {
                Debug.LogWarning("Couldn't restore state: " + field);
                return;
            }

            try
            {
                Debug.Log("Restoring savestate: " + PlayerPrefs.GetString(field));
                JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(field), this);
                varFlags.Clear();
                for (int i = 0, totalVars = varFlagKeys.Count; i < totalVars; i++)
                {
                    varFlags[varFlagKeys[i]] = varFlagValues[i];
                }
                elementContexts.Clear();
                for (int i = 0, totalVars = elementContextsKeys.Count; i < totalVars; i++)
                {
                    elementContexts[elementContextsKeys[i]] = elementContextsValues[i].ElementReferences;
                }
                memories.Clear();
                if (memoryKeys == null || memoryValues == null)
                {
                    Debug.LogError("Memory restoration failed! (memorykeys " + ((object)memoryKeys ?? "null").ToString() + ", memoryvalues " + ((object)memoryValues ?? "null").ToString());
                }
                else if (memoryKeys.Count != memoryValues.Count)
                {
                    Debug.LogError("Memory restoration failed! (Different keys and values: " + memoryKeys.Count + " != " + memoryValues.Count);
                }
                for (int i = 0, totalMemories = memoryKeys.Count; i < totalMemories; i++)
                {
                    memories[memoryKeys[i]] = memoryValues[i];
                }

                InventoryManager.Instance.Restore();
            }
            catch(Exception ex)
            {
                Debug.Log("Unable to restore save state! Maybe you're trying to restore a save from an older version? (" + ex.Message + ", " + ex.StackTrace + ")");
                this.Restart();
            }
            
        }
    }
}
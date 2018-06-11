using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using RAGE.Analytics;
using System;
using AssetPackage;

namespace uAdventure.Runner
{
    public class GameState
    {
        //###########################################################################
        //########################### GAME STATE HANDLING ###########################
        //###########################################################################

        AdventureData data;
        private ElementReference playerContext;
        int current_chapter = 0;
        string current_target = "";
        string last_target = "";
        private List<string> removedElements;
        private List<string> inventoryItems;
        private Dictionary<string, int> varFlagValues;
        private Stack<List<KeyValuePair<string, int>>> varFlagChangeAmbits;

        public AdventureData Data
        {
            get { return data; }
        }

        public string CurrentTarget
        {
            get {
                return current_target;
            }
            set {
                last_target = current_target;
                current_target = value;
                // TODO use PlayerPrefs in settings
                // PlayerPrefs.SetString("target", current_target);
                // PlayerPrefs.Save();
            }
        }

        public GameState(AdventureData data)
        {
            this.removedElements = new List<string>();
            this.inventoryItems = new List<string>();
            this.data = data;
            varFlagValues = new Dictionary<string, int>();
            varFlagChangeAmbits = new Stack<List<KeyValuePair<string, int>>>();
        }

        public int checkFlag(string flag)
        {
            int ret = FlagCondition.FLAG_INACTIVE;
            // TODO use PlayerPrefs in adventure settings
            /*if (PlayerPrefs.HasKey("flag_"+flag))
            {
                ret = PlayerPrefs.GetInt("flag_" + flag);
            }*/

            if(varFlagValues.ContainsKey("flag_" + flag))
            {
                ret = varFlagValues["flag_" + flag];
            }
            return ret;
        }

        public void setFlag(string name, int state)
        {
            // TODO use PlayerPrefs in adventure settings
            // PlayerPrefs.SetInt("flag_" + name, state);
            // PlayerPrefs.Save();

            varFlagValues["flag_" + name] = state;
            //Debug.Log ("Flag '" + name + " puesta a " + state);
            bool bstate = state == FlagCondition.FLAG_ACTIVE;
            int intVal = bstate ? 1 : 0;

            // TODO check this after this: https://github.com/e-ucm/unity-tracker/issues/29
            if (varFlagChangeAmbits.Count > 0) varFlagChangeAmbits.Peek().Add(new KeyValuePair<string, int>(name, intVal));
            else TrackerAsset.Instance.setVar(name, intVal);
            TimerController.Instance.CheckTimers();
            CompletablesController.Instance.ConditionChanged();
            Game.Instance.reRenderScene();
        }

        public int getVariable(string var)
        {
            int ret = 0;
            // TODO use PlayerPrefs in adventure settings
            /*if (PlayerPrefs.HasKey("var_" + var))
            {
                ret = PlayerPrefs.GetInt("var_" + var);
            }*/

            if (varFlagValues.ContainsKey("var_" + var))
            {
                ret = varFlagValues["var_" + var];
            }
            return ret;
        }

        public void setVariable(string name, int value)
        {
            // TODO use PlayerPrefs in adventure settings
            /*PlayerPrefs.SetInt("var_" + name, value);
            PlayerPrefs.Save();*/
            varFlagValues["var_" + name] = value;

            if (varFlagChangeAmbits.Count > 0) varFlagChangeAmbits.Peek().Add(new KeyValuePair<string, int>(name, value));
            else TrackerAsset.Instance.setVar(name, value);

            CompletablesController.Instance.ConditionChanged();

            Game.Instance.reRenderScene();
        }

        public int checkGlobalState(string global_state)
        {
            int ret = GlobalStateCondition.GS_SATISFIED;
            GlobalState gs = data.getChapters()[current_chapter].getGlobalState(global_state);
            if (gs != null)
            {
                ret = ConditionChecker.check(gs);
            }
            return ret;
        }

        public Macro getMacro(string id)
        {
            return data.getChapters()[current_chapter].getMacro(id);
        }

        public Conversation getConversation(string id)
        {
            return data.getChapters()[current_chapter].getConversation(id);
        }

        public List<Timer> getTimers()
        {
            return data.getChapters()[current_chapter].getTimers();
        }


        public ElementReference PlayerContext
        {
            get
            {
                if (playerContext == null)
                {
                    var scene = getChapterTarget(CurrentTarget) as Scene;
                    if (scene == null)
                    {
                        playerContext = new ElementReference("Player", 0, 0, -1);
                        return playerContext;
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

                return playerContext;
            }
        }

        public Player Player
        {
            get
            {
                return data.getChapters()[current_chapter].getPlayer();
            }
        }

        public bool IsFirstPerson
        {
            get
            {
                return data.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON;
            }
        }

        public void Move(string id, Vector2 position, int time = 0)
        {
            GameObject go = GameObject.Find(id);
            Movable m = go.GetComponent<Movable>();
            if (m != null)
                m.setPosition(position);
        }

        public NPC getCharacter(string name)
        {
            return data.getChapters()[current_chapter].getCharacter(name);
        }

        public Item getObject(string name)
        {
            return data.getChapters()[current_chapter].getItem(name);
        }

        public Atrezzo getAtrezzo(string name)
        {
            Chapter c = data.getChapters()[current_chapter];
            return c.getAtrezzo(name);
        }

        public List<T> GetObjects<T>()
        {
            return data.getChapters()[current_chapter].getObjects<T>();
        }

        public T FindElement<T>(string id)
        {
            if(typeof(T) == typeof(Element))
            {
                Element r = getCharacter(id);
                if (r != null)
                    return (T) (r as object);
                r = getObject(id);
                if (r != null)
                    return (T)(r as object);
                r = getAtrezzo(id);
                if (r != null)
                    return (T)(r as object);
            }

            return data.getChapters()[current_chapter].getObjects<T>().Find(e =>
            {
                var idMethod = e.GetType().GetMethod("getId");
                return idMethod != null && ((string)idMethod.Invoke(e, null)) == id;
            });
        }
        

        public bool isCutscene(string scene_id)
        {
            return data.getChapters()[current_chapter].getObjects<Cutscene>().Exists(s => s.getId() == scene_id);
        }

        public IChapterTarget InitialChapterTarget
        {
            get
            {
                return data.getChapters()[current_chapter].getInitialChapterTarget();
            }
        }

        public IChapterTarget PreviousChapterTarget
        {
            get
            {
                return getChapterTarget(last_target);
            }
        }

        public GeneralScene getLastScene()
        {
            GeneralScene ret = null;

            foreach (GeneralScene scene in data.getChapters()[current_chapter].getGeneralScenes())
            {
                if (scene.getType() == GeneralScene.GeneralSceneSceneType.SLIDESCENE && ((Slidescene)scene).getNext() == Slidescene.ENDCHAPTER)
                {
                    ret = scene;
                    break;
                }
            }

            return ret;
        }

        public List<Completable> getCompletables()
        {
            return data.getChapters()[current_chapter].getCompletables();
        }

        public IChapterTarget getChapterTarget(string runnerTargetId)
        {
            return data.getChapters()[current_chapter].getObjects<IChapterTarget>().Find(o => o.getId() == runnerTargetId);
        }

        //##########################################################################
        //##########################################################################
        //##########################################################################

        // Removed objects

        public List<string> getRemovedElements()
        {
            return removedElements;
        }

        public void addRemovedElement(string elementId)
        {
            removedElements.Add(elementId);
        }

        public bool removeRemovedElement(string elementId)
        {
            return removedElements.Remove(elementId);
        }

        // Inventory objects

        public List<string> getInventoryItems()
        {
            return inventoryItems;
        }

        public void addInventoryItem(string elementId)
        {
            inventoryItems.Add(elementId);
        }

        public bool removeInventoryItem(string elementId)
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
    }
}
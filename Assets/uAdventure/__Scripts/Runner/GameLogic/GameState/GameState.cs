﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using RAGE.Analytics;
using System;

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
                PlayerPrefs.SetString("target", current_target);
                PlayerPrefs.Save();
            }
        }

        public GameState(AdventureData data)
        {
            this.removedElements = new List<string>();
            this.inventoryItems = new List<string>();
            this.data = data;
        }

        public int checkFlag(string flag)
        {
            int ret = FlagCondition.FLAG_INACTIVE;
            if (PlayerPrefs.HasKey("flag_"+flag))
            {
                ret = PlayerPrefs.GetInt("flag_" + flag);
            }
            return ret;
        }

        public void setFlag(string name, int state)
        {
            PlayerPrefs.SetInt("flag_" + name, state);
            PlayerPrefs.Save();

            //Debug.Log ("Flag '" + name + " puesta a " + state);
            bool bstate = state == FlagCondition.FLAG_ACTIVE;

            // TODO check this after this: https://github.com/e-ucm/unity-tracker/issues/29
            Tracker.T.setVar(name, bstate ? 1 : 0);
            TimerController.Instance.CheckTimers();
            CompletableController.Instance.conditionChanged();
            Game.Instance.reRenderScene();
        }

        public int getVariable(string var)
        {
            int ret = 0;
            if (PlayerPrefs.HasKey("var_" + var))
            {
                ret = PlayerPrefs.GetInt("var_" + var);
            }
            return ret;
        }

        public void setVariable(string name, int value)
        {
            PlayerPrefs.SetInt("var_" + name, value);
            PlayerPrefs.Save();

            Tracker.T.setVar(name, value);
            CompletableController.Instance.conditionChanged();

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
                        return null;
                    
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
                m.Traslate(position);
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

    }
}
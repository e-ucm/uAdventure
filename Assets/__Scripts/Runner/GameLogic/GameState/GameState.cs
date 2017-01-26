using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using uAdventure.RageTracker;
using System;

namespace uAdventure.Runner
{
    public class GameState
    {
        //###########################################################################
        //########################### GAME STATE HANDLING ###########################
        //###########################################################################

        AdventureData data;
        private Dictionary<string, int> flags = new Dictionary<string, int>();
        private Dictionary<string, int> variables = new Dictionary<string, int>();
        int current_chapter = 0;
        string current_target = "";

        public AdventureData Data
        {
            get { return data; }
        }

        public string CurrentTarget
        {
            get { return current_target; }
            set { current_target = value; }
        }

        public GameState(AdventureData data)
        {
            this.data = data;
            flags = new Dictionary<string, int>();
            variables = new Dictionary<string, int>();
        }

        public int checkFlag(string flag)
        {
            int ret = FlagCondition.FLAG_INACTIVE;
            if (flags.ContainsKey(flag))
            {
                ret = flags[flag];
            }
            return ret;
        }

        public void setFlag(string name, int state)
        {

            if (flags.ContainsKey(name))
            {
                flags[name] = state;
            }
            else
            {
                flags.Add(name, state);
            }

            //Debug.Log ("Flag '" + name + " puesta a " + state);
            bool bstate = state == FlagCondition.FLAG_ACTIVE;
            Tracker.T.setExtension(name, bstate);
            TimerController.Instance.checkTimers();
            Game.Instance.reRenderScene();
        }

        public int getVariable(string var)
        {
            int ret = 0;
            if (variables.ContainsKey(var))
            {
                ret = variables[var];
            }
            return ret;
        }

        public void setVariable(string name, int value)
        {
            if (variables.ContainsKey(name))
            {
                variables[name] = value;
            }
            else
            {
                variables.Add(name, value);
            }

            Tracker.T.setExtension(name, value);



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

        public Player getPlayer()
        {
            return data.getChapters()[current_chapter].getPlayer();
        }

        public bool isFirstPerson()
        {
            return data.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON;
        }

        public void Move(string id, Vector2 position, int time = 0)
        {
            GameObject go = GameObject.Find(id);
            Movable m = go.GetComponent<Movable>();
            if (m != null)
                m.Move(position);
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
        
        /*private GeneralScene getInitialScene()
        { 

            return data.getChapters()[current_chapter].getInitialGeneralScene();
        }*/

        public IChapterTarget getInitialChapterTarget()
        {
            return data.getChapters()[current_chapter].getInitialChapterTarget();
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
            return null;
        }

        public IChapterTarget getChapterTarget(string runnerTargetId)
        {
            return data.getChapters()[current_chapter].getObjects<IChapterTarget>().Find(o => o.getId() == runnerTargetId);
        }

        //##########################################################################
        //##########################################################################
        //##########################################################################

    }
}
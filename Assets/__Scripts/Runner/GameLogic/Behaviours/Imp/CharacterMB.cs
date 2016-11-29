using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using uAdventure.RageTracker;

namespace uAdventure.Runner
{
    public class CharacterMB : Representable, Interactuable
    {
        /*  ## ANIMATION METHOD ##
         * 
         * private Animation current_anim;
         * 
         * Texture2D tmp = current_anim.getFrame(0).getImage(false,false,0).texture;
            update_ratio = current_anim.getFrame(0).getTime();//Duration/1000f;
         * 
         * return (current_anim.getFrame(current_frame).getImage(false,false,0).texture.height) * context.getScale();
         * 
         * current_frame = (current_frame + 1) % current_anim.getFrames().Count;
            Texture2D tmp = current_anim.getFrame(current_frame).getImage(false,false,0).texture;
            update_ratio = current_anim.getFrame(current_frame).getTime();
         * 
         */

        protected override void Start()
        {
            this.gameObject.name = base.Element.getId();
            base.Start();
            base.setAnimation(NPC.RESOURCE_TYPE_STAND_UP);
        }

        protected override void Update()
        {
            base.Update();
        }

        bool interactable = false;
        public bool canBeInteracted()
        {
            return interactable;
        }

        public void setInteractuable(bool state)
        {
            this.interactable = state;
        }

        public InteractuableResult Interacted(RaycastHit hit = default(RaycastHit))
        {
            InteractuableResult res = InteractuableResult.IGNORES;

            if (interactable)
            {
                List<Action> available = new List<Action>();

                foreach (Action a in base.Element.getActions())
                {
                    if (ConditionChecker.check(a.getConditions()))
                    {
                        bool addaction = true;
                        foreach (Action a2 in available)
                        {
                            if ((a.getType() == Action.CUSTOM || a.getType() == Action.CUSTOM_INTERACT) && (a2.getType() == Action.CUSTOM || a2.getType() == Action.CUSTOM_INTERACT))
                            {
                                if (((CustomAction)a).getName() == ((CustomAction)a2).getName())
                                {
                                    addaction = false;
                                    break;
                                }
                            }
                            else if (a.getType() == a2.getType())
                            {
                                addaction = false;
                                break;
                            }
                        }

                        if (addaction)
                            available.Add(a);
                    }
                }

                if (available.Count > 0)
                {
                    Game.Instance.showActions(available, Input.mousePosition);
                    res = InteractuableResult.DOES_SOMETHING;
                }

                Tracker.T.trackedGameObject.Interacted(((NPC)Element).getId(), GameObjectTracker.TrackedGameObject.Npc);
                Tracker.T.RequestFlush();
            }

            return res;
        }


    }
}
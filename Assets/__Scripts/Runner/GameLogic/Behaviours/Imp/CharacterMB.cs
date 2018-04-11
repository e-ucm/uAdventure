using UnityEngine;
using uAdventure.Core;
using RAGE.Analytics;
using RAGE.Analytics.Formats;
using UnityEngine.EventSystems;
using System.Linq;

namespace uAdventure.Runner
{
    public class CharacterMB : Representable, Interactuable, IPointerClickHandler, IDropHandler, IActionReceiver
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM, Action.TALK_TO, Action.EXAMINE };

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

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            InteractuableResult res = InteractuableResult.IGNORES;

            if (interactable)
            {
                var availableActions = Element.getActions().Valid(restrictedActions).ToList();

                ActionsUtil.AddExamineIfNotExists(Element, availableActions);

                //if there is an action, we show them
                if (availableActions.Count > 0)
                {
                    Game.Instance.showActions(availableActions, Input.mousePosition, this);
                    res = InteractuableResult.DOES_SOMETHING;
                }

                Tracker.T.trackedGameObject.Interacted(((NPC)Element).getId(), GameObjectTracker.TrackedGameObject.Npc);
                Tracker.T.RequestFlush();
            }

            return res;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            uAdventureInputModule.DropTargetSelected(eventData);
        }

        public void ActionSelected(Action action)
        {
            if(Game.Instance.GameState.IsFirstPerson|| !action.isNeedsGoTo())
                Game.Instance.Execute(new EffectHolder(action.Effects));
            else
            {
                var sceneMB = FindObjectOfType<SceneMB>();
                var scene = sceneMB.sceneData as Scene;
                var topLeft = new Vector2(Context.getX() - Texture.width / 2f, Context.getY() - Texture.height);
                Rectangle area = null;
                if (scene != null && scene.getTrajectory() == null)
                {
                    // If no trajectory I have to move the area to the trajectory for it to be connected
                    area = new InfluenceArea((int)topLeft.x, (int)topLeft.y, Texture.width, Texture.height);
                    area = area.MoveAreaToTrajectory(sceneMB.Trajectory);
                }
                else
                {
                    area = Context.getInfluenceArea().MoveArea(topLeft);
                }
                PlayerMB.Instance.Do(action, area);
            }
        }
    }
}
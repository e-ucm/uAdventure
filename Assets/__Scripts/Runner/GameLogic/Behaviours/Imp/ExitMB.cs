using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.RageTracker;

namespace uAdventure.Runner
{
    public class ExitMB : MonoBehaviour, Interactuable
    {

        private Exit ed;
        public Exit exitData
        {
            get { return ed; }
            set { ed = value; }
        }
        

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void exit()
        {
            //Game.Instance.hideMenu ();
            if (ConditionChecker.check(ed.getConditions()))
            {
                EffectHolder effect = new EffectHolder(ed.getEffects());

                if (Game.Instance.GameState.isCutscene(ed.getNextSceneId()))
                    effect.effects.Add(new EffectHolderNode(new TriggerCutsceneEffect(ed.getNextSceneId())));
                else
                    effect.effects.Add(new EffectHolderNode(new TriggerSceneEffect(ed.getNextSceneId(), 0, 0, ed.getTransitionTime(), ed.getTransitionType())));

                if (ed.getPostEffects() != null)
                {
                    EffectHolder eh = new EffectHolder(ed.getPostEffects());
                    foreach (EffectHolderNode ehn in eh.effects)
                    {
                        effect.effects.Add(ehn);
                    }
                }

                if (Game.Instance.getAlternativeScene() != null)
                {
                    if (Game.Instance.getAlternativeScene().getXApiType() == "menu")
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeScene().getId(), ed.getNextSceneId(), AlternativeTracker.Alternative.Menu);
                    else
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeScene().getId(), ed.getNextSceneId(), true);
                }

                Game.Instance.Execute(effect);
                GUIManager.Instance.setCursor("default");
            }
            else
            {
                if (Game.Instance.getAlternativeScene() != null)
                {
                    if (Game.Instance.getAlternativeScene().getXApiType() != "menu")
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeScene().getId(), "Incorrect", false);
                }

                if (ed.isHasNotEffects())
                    Game.Instance.Execute(new EffectHolder(ed.getNotEffects()));
            }

            Tracker.T.RequestFlush();
        }

        void OnMouseEnter()
        {
            GUIManager.Instance.showHand(true);
            interactable = true;
        }

        void OnMouseExit()
        {
            GUIManager.Instance.showHand(false);
            interactable = false;
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

        public InteractuableResult Interacted(RaycastHit hit = new RaycastHit())
        {
            exit();
            return InteractuableResult.DOES_SOMETHING;
        }
    }
}
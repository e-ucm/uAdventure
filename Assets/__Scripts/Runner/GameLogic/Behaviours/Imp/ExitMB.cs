using UnityEngine;
using System.Collections;

using uAdventure.Core;
using RAGE.Analytics;
using RAGE.Analytics.Formats;
using System;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class ExitMB : Area, Interactuable, IPointerClickHandler
    {

        private Exit ed;
        public Exit exitData
        {
            get { return ed; }
            set { ed = value; }
        }

        public void Exit()
        {
            //Game.Instance.hideMenu ();
            if (!Game.Instance.GameState.isFirstPerson())
            { 
                // move player to influence area
            }

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

                if (Game.Instance.getAlternativeTarget() != null)
                {
                    if (Game.Instance.getAlternativeTarget().getXApiType() == "menu")
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeTarget().getId(), ed.getNextSceneId(), AlternativeTracker.Alternative.Menu);
                    else
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeTarget().getId(), ed.getNextSceneId(), true);
                }

                Game.Instance.Execute(effect);
                GUIManager.Instance.setCursor("default");
            }
            else
            {
                if (Game.Instance.getAlternativeTarget() != null)
                {
                    if (Game.Instance.getAlternativeTarget().getXApiType() != "menu")
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeTarget().getId(), "Incorrect", false);
                }

                if (ed.isHasNotEffects())
                    Game.Instance.Execute(new EffectHolder(ed.getNotEffects()));
            }

            Tracker.T.RequestFlush();
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
            Exit();
            return InteractuableResult.DOES_SOMETHING;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }
    }
}
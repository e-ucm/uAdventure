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
        public Exit Element
        {
            get { return ed; }
            set { ed = value; }
        }

        private EffectHolder GetExitEffects()
        {
            Effects effects = new Effects();
            if (ConditionChecker.check(ed.getConditions()))
            {
                effects.AddRange(ed.getEffects());

                if (Game.Instance.GameState.isCutscene(ed.getNextSceneId()))
                    effects.Add(new TriggerCutsceneEffect(ed.getNextSceneId()));
                else
                    effects.Add(new TriggerSceneEffect(ed.getNextSceneId(), ed.getDestinyX(), ed.getDestinyY(), ed.getDestinyScale(), ed.getTransitionTime(), ed.getTransitionType()));

                if (ed.getPostEffects() != null)
                    effects.AddRange(ed.getPostEffects());
            }
            else
            {
                if (ed.isHasNotEffects())
                    effects.AddRange(ed.getNotEffects());
            }

            return new EffectHolder(effects);
        }

        private void TrackExit()
        {
            if (Game.Instance.getAlternativeTarget() != null)
            {
                if (ConditionChecker.check(ed.getConditions()))
                {
                    if (Game.Instance.getAlternativeTarget().getXApiType() == "menu")
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeTarget().getId(), ed.getNextSceneId(), AlternativeTracker.Alternative.Menu);
                    else
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeTarget().getId(), ed.getNextSceneId(), true);
                }
                else
                {
                    if (Game.Instance.getAlternativeTarget().getXApiType() != "menu")
                        Tracker.T.alternative.Selected(Game.Instance.getAlternativeTarget().getId(), "Incorrect", false);
                }
                Tracker.T.RequestFlush();
            }
        }

        public void Exit()
        {
            TrackExit();
            Game.Instance.Execute(GetExitEffects());
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
            if (Game.Instance.GameState.IsFirstPerson)
            {
                Exit();
            }
            else
            {
                var sceneMB = FindObjectOfType<SceneMB>();
                var scene = sceneMB.sceneData as Scene;
                Rectangle area = null;
                if (scene != null && scene.getTrajectory() == null)
                {
                    // If no trajectory I have to move the area to the trajectory for it to be connected
                    area = ed.MoveAreaToTrajectory(sceneMB.Trajectory);
                }
                else
                {
                    area = new InfluenceArea(ed.getX() - 20, ed.getY() - 20, ed.getWidth() + 40, ed.getHeight() + 40);
                    if(this.ed.getInfluenceArea() != null && this.ed.getInfluenceArea().isExists())
                    {
                        var points = this.ed.isRectangular() ? this.ed.ToRect().ToPoints() : this.ed.getPoints().ToArray();
                        var topLeft = points.ToRect().position;
                        area = this.ed.getInfluenceArea().MoveArea(topLeft);
                    }
                }
                var exitAction = new Core.Action(Core.Action.CUSTOM) { Effects = new Effects() { new ExecuteExitEffect(this) } };
                exitAction.setNeedsGoTo(true);
                PlayerMB.Instance.Do(exitAction, area);
            }
            return InteractuableResult.DOES_SOMETHING;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }
        
        // Workaround to get execution flow back after the player reaches the target:
        // The ExtecuteExit effect is passed to the player with an custom action.
        // When the player reaches the exit, it executes the action effects, therefore
        // it executes the ExecuteExit effect that gives back the execution flow.
        public class ExecuteExitEffect : IEffect
        {
            public ExitMB exitMB;
            public ExecuteExitEffect(ExitMB exitMB) { this.exitMB = exitMB; }
            public EffectType getType() { return EffectType.CUSTOM_EFFECT; }
        }

        [CustomEffectRunner(typeof(ExecuteExitEffect))]
        public class ExecuteExitEffectRunner : CustomEffectRunner
        {
            private ExecuteExitEffect toRun;
            private EffectHolder exitEffects;

            public IEffect Effect { get { return toRun; } set { toRun = value as ExecuteExitEffect; } }

            public bool execute()
            {
                if(exitEffects == null)
                {
                    toRun.exitMB.TrackExit();
                    exitEffects = toRun.exitMB.GetExitEffects();
                }
                return exitEffects.execute();
            }
        }
    }
}
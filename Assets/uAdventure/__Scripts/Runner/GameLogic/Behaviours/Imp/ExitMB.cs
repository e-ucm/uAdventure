using uAdventure.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TinCan;
using Xasu;
using Xasu.HighLevel;
using Xasu.Util;

namespace uAdventure.Runner
{

    [RequireComponent(typeof(Area))]
    public class ExitMB : MonoBehaviour, Interactuable, IPointerClickHandler
    {
        System.StringComparison IgnoreCase = System.StringComparison.InvariantCultureIgnoreCase;
        private Area area;
        private Exit exit;

        protected void Awake()
        {
            Game.Instance.GameState.OnConditionChanged += OnConditionChanged;
        }

        protected void Start()
        {
            area = GetComponent<Area>();
            exit = area.Element as Exit;
            OnConditionChanged(null, 0);
        }

        private void OnConditionChanged(string name, int value)
        {
            area = GetComponent<Area>();
            exit = area.Element as Exit;
            gameObject.SetActive(exit.isHasNotEffects() || ConditionChecker.check(exit.getConditions()));
        }

        protected void OnDestroy()
        {
            if (Game.Instance)
            {
                Game.Instance.GameState.OnConditionChanged -= OnConditionChanged;
            }
        }

        private EffectHolder GetExitEffects(out bool exited)
        {
            var ed = area.Element as Exit;
            Effects effects = new Effects();
            exited = false;
            if (ConditionChecker.check(ed.getConditions()))
            {
                exited = true;
                effects.AddRange(ed.getEffects());

                if (Game.Instance.GameState.IsCutscene(ed.getNextSceneId()))
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

            var effectHolder = new EffectHolder(effects);
            if (exited)
            {
                effectHolder.effects[ed.getEffects().Count].AddAdditionalInfo("not_trace", true);
            }

            return effectHolder;
        }

        private StatementPromise TraceExit(bool exited, IChapterTarget targetOnExit)
        {
            if (XasuTracker.Instance.Status.State == TrackerState.Uninitialized
                || XasuTracker.Instance.Status.State == TrackerState.Errored
                || XasuTracker.Instance.Status.State == TrackerState.Finalized)
            {
                return null;
            }

            var ed = area.Element as Exit;

            // ALTERNATIVE
            if ("alternative".Equals(targetOnExit.getXApiClass(), IgnoreCase))
            {
                var parsedType = (AlternativeTracker.AlternativeType)Enum.Parse(typeof(AlternativeTracker.AlternativeType), targetOnExit.getXApiType(), true);
                if (ConditionChecker.check(ed.getConditions()))
                {
                    if (targetOnExit.getXApiType() == "menu")
                    {
                        return AlternativeTracker.Instance.Selected(targetOnExit.getId(), ed.getNextSceneId(), parsedType);
                    }
                    else
                    {
                        return AlternativeTracker.Instance.Selected(targetOnExit.getId(), ed.getNextSceneId(), parsedType)
                            .WithSuccess(true);
                    }
                }
                else
                {
                    if (targetOnExit.getXApiType() != "menu")
                    {
                        return AlternativeTracker.Instance.Selected(targetOnExit.getId(), "Incorrect", parsedType)
                            .WithSuccess(false);
                    }
                }
            }
            return null;
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
            var ed = area.Element as Exit;
            if (Game.Instance.GameState.IsFirstPerson)
            {
                Game.Instance.Execute(new EffectHolder(new Effects { new ExecuteExitEffect(this) }));
            }
            else
            {
                var sceneMB = FindObjectOfType<SceneMB>();
                var scene = sceneMB.SceneData as Scene;
                Rectangle actionArea = null;
                if (scene != null && scene.getTrajectory() == null)
                {
                    // If no trajectory I have to move the area to the trajectory for it to be connected
                    actionArea = ed.MoveAreaToTrajectory(sceneMB.Trajectory);
                }
                else
                {
                    actionArea = new InfluenceArea(ed.getX() - 20, ed.getY() - 20, ed.getWidth() + 40, ed.getHeight() + 40);
                    if(ed.getInfluenceArea() != null && ed.getInfluenceArea().isExists())
                    {
                        var points = ed.isRectangular() ? ed.ToRect().ToPoints() : ed.getPoints().ToArray();
                        var topLeft = points.ToRect().position;
                        actionArea = ed.getInfluenceArea().MoveArea(topLeft);
                    }
                }
                var exitAction = new Core.Action(Core.Action.CUSTOM) { Effects = new Effects() { new ExecuteExitEffect(this) } };
                exitAction.setNeedsGoTo(true);
                PlayerMB.Instance.Do(exitAction, actionArea);
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
            public ExitMB ExitMb { get; set; }
            public string GUID { get; set; } = Guid.NewGuid().ToString();

            public ExecuteExitEffect(ExitMB exitMb) { this.ExitMb = exitMb; }

            public object Clone()
            {
                var clone = (ExecuteExitEffect) this.MemberwiseClone();
                clone.ExitMb = ExitMb;
                return clone;
            }

            public EffectType getType() { return EffectType.CUSTOM_EFFECT; }
        }

        [CustomEffectRunner(typeof(ExecuteExitEffect))]
        public class ExecuteExitEffectRunner : CustomEffectRunner
        {
            private ExecuteExitEffect toRun;
            private EffectHolder exitEffects;
            private bool exits;
            private IChapterTarget targetOnExit;
            private StatementPromise trace;

            public IEffect Effect { get { return toRun; } set { toRun = value as ExecuteExitEffect; Init(); } }

            private void Init()
            {
                // First run
                targetOnExit = Game.Instance.GameState.GetChapterTarget(Game.Instance.GameState.CurrentTarget);
                try
                {
                    trace = toRun.ExitMb.TraceExit(exits, targetOnExit);
                    trace?.Statement.SetPartial();
                }
                catch (Exception ex)
                {
                    Debug.Log("Error while tracing the exit! (" + ex.Message + ", " + ex.StackTrace + ")");
                }
                Game.Instance.GameState.BeginChangeAmbit(trace);
                exitEffects = toRun.ExitMb.GetExitEffects(out exits);
            }

            public bool execute()
            {
                var forceWait = exitEffects.execute();
                if (!forceWait)
                {
                    // Last run
                    Game.Instance.GameState.EndChangeAmbitAsExtensions(trace);
                    trace?.Statement.Complete();
                }
                return forceWait;
            }
        }
    }
}
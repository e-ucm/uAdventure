using uAdventure.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using AssetPackage;
using System;

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

        private void TraceExit(bool exited, IChapterTarget targetOnExit)
        {
            var ed = area.Element as Exit;

            // ALTERNATIVE
            if ("alternative".Equals(targetOnExit.getXApiClass(), IgnoreCase))
            {
                var parsedType = (AlternativeTracker.Alternative)Enum.Parse(typeof(AlternativeTracker.Alternative), targetOnExit.getXApiType(), true);
                if (ConditionChecker.check(ed.getConditions()))
                {
                    if (targetOnExit.getXApiType() == "menu")
                        TrackerAsset.Instance.Alternative.Selected(targetOnExit.getId(), ed.getNextSceneId(), parsedType);
                    else
                    {
                        TrackerAsset.Instance.setSuccess(true);
                        TrackerAsset.Instance.Alternative.Selected(targetOnExit.getId(), ed.getNextSceneId(), parsedType);
                    }
                }
                else
                {
                    if (targetOnExit.getXApiType() != "menu")
                    {
                        TrackerAsset.Instance.setSuccess(false);
                        TrackerAsset.Instance.Alternative.Selected(targetOnExit.getId(), "Incorrect", parsedType);
                    }
                }
                TrackerAsset.Instance.Flush();
            }

            // ACCESIBLE

            // If no exited, accesible doesnt matter
            if (!exited)
                return;

            // If no destination accesible doesnt matter
            var destination = Game.Instance.GameState.GetChapterTarget(ed.getNextSceneId());
            if (destination == null)
                return;

            if ("accesible".Equals(destination.getXApiClass(), IgnoreCase))
            {
                var type = ExParsers.ParseDefault(destination.getXApiType(), AccessibleTracker.Accessible.Accessible);
                TrackerAsset.Instance.Accessible.Accessed(destination.getId(), type);
            }

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

            public IEffect Effect { get { return toRun; } set { toRun = value as ExecuteExitEffect; Init(); } }

            private void Init()
            {
                // First run
                targetOnExit = Game.Instance.GameState.GetChapterTarget(Game.Instance.GameState.CurrentTarget);
                Game.Instance.GameState.BeginChangeAmbit();
                exitEffects = toRun.ExitMb.GetExitEffects(out exits);
            }

            public bool execute()
            {
                var forceWait = exitEffects.execute();
                if (!forceWait)
                {
                    // Last run
                    Game.Instance.GameState.EndChangeAmbitAsExtensions();
                    try
                    {
                        toRun.ExitMb.TraceExit(exits, targetOnExit);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Error while tracing the exit! (" + ex.Message + ", " + ex.StackTrace + ")");
                    }
                }
                return forceWait;
            }
        }
    }
}
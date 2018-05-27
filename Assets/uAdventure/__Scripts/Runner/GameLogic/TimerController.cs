using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;

namespace uAdventure.Runner
{

    public class TimerController : MonoBehaviour
    {

        public enum TimerState { New, Running, Finished };
        public enum TimerType { Normal, Countdown };
        public class TimerVars
        {
            public Timer timer;
            public float currentTime;
            public float maxTime;
            public TimerType type;
        }

        #region Singleton

        //#################################################################
        //########################### SINGLETON ###########################
        //#################################################################

        private static TimerController instance;
        public static TimerController Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        [SerializeField]
        private GameObject timerPrefab;
        [SerializeField]
        private GameObject timersContainer;

        private List<Timer> timers = new List<Timer>();
        private Dictionary<Timer, GameObject> timerObject = new Dictionary<Timer, GameObject>();
        private Dictionary<Timer, TimerState> timerState = new Dictionary<Timer, TimerState>();
        private Dictionary<Timer, TimerVars> runningTimers = new Dictionary<Timer, TimerVars>();
        private bool checkAfterwards = false;

        private Queue<EffectHolder> finalizationQueue = new Queue<EffectHolder>();

        public List<Timer> Timers
        {
            get { return timers; }
            set
            {
                // Update the timer states
                var newTimerStates = new Dictionary<Timer, TimerState>();
                foreach (var timer in value)
                    newTimerStates.Add(timer, timerState.ContainsKey(timer) ? timerState[timer] : TimerState.New);
                timerState = newTimerStates;

                this.timers = value;

            }
        }

        //######################################################################
        //########################### TIMER HANDLING ###########################
        //######################################################################

        //###### MONOBEHAVIOUR ######

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
        }

        void Update()
        {
            foreach (var timer in timers)
            {
                if (!runningTimers.ContainsKey(timer))
                    continue;
                
                TimerVars timerVars = runningTimers[timer];
                switch (timerVars.type)
                {
                    case TimerType.Normal:
                        timerVars.currentTime += Time.deltaTime;
                        if (timerVars.currentTime >= timerVars.maxTime)
                            FinalizeTimer(timerVars.timer);
                        break;
                    case TimerType.Countdown:
                        timerVars.currentTime -= Time.deltaTime;
                        if (timerVars.currentTime <= 0)
                            FinalizeTimer(timerVars.timer);
                        break;
                }
            }

            if (checkAfterwards)
            {
                checkAfterwards = false;
                CheckTimers();
            }

            while (finalizationQueue.Count > 0)
            {
                if (Game.Instance.isSomethingRunning())
                    break;
                Game.Instance.Execute(finalizationQueue.Dequeue());
            }
        }

        private void InitTimer(Timer timer, TimerVars timerVars)
        {
            if (timer.isShowTime())
            {
                if (!timerObject.ContainsKey(timer))
                    timerObject[timer] = Instantiate(timerPrefab, timersContainer.transform);
                var uiTimerController = timerObject[timer].GetComponent<UITimerController>();
                uiTimerController.Timer = timer;
                uiTimerController.Running = true;
                uiTimerController.Reset();
            }

            if (timer.isCountDown())
            {
                timerVars.type = TimerType.Countdown;
                timerVars.maxTime = 0;
                timerVars.currentTime = System.Convert.ToInt64(timer.getTime() * 1000) / 1000f;
            }
            else
            {
                timerVars.type = TimerType.Normal;
                timerVars.maxTime = System.Convert.ToInt64(timer.getTime() * 1000) / 1000f;
                timerVars.currentTime = 0;
            }

            timerVars.timer = timer;
        }

        private void FinalizeTimer(Timer timer)
        {
            var timerVars = runningTimers[timer];
            timerVars.currentTime = 0f;
            if (timer.isRunsInLoop())
            {
                InitTimer(timer, timerVars);
            }
            else
            {
                timerState[timer] = TimerState.Finished;
                if (timer.isMultipleStarts())
                {
                    timerState[timer] = TimerState.New;
                    checkAfterwards = true;
                }
                runningTimers.Remove(timer);
            }

            finalizationQueue.Enqueue(new EffectHolder(timerVars.timer.getEffects()));

            // Destroy the visualization (if not shown when stopped)
            if (!timer.isShowWhenStopped() && timerObject.ContainsKey(timer))
            {
                DestroyImmediate(timerObject[timer]);
                timerObject.Remove(timer);
            }
        }

        private void StopTimer(Timer timer)
        {
            runningTimers.Remove(timer);
            timerState[timer] = TimerState.Finished;
            if (timer.isMultipleStarts())
            {
                timerState[timer] = TimerState.New;
                checkAfterwards = true;
            }

            if(timer.getPostEffects() != null)
                finalizationQueue.Enqueue(new EffectHolder(timer.getPostEffects()));

            // Destroy the visualization
            if (timerObject.ContainsKey(timer))
            {
                DestroyImmediate(timerObject[timer]);
                timerObject.Remove(timer);
            }
        }

        public void Run()
        {
            CheckTimers();
        }

        public void CheckTimers()
        {
            foreach (Timer timer in timers)
            {
                // Finished timers dont restart
                if (timerState[timer] == TimerState.Finished)
                    continue;

                if (!IsRunning(timer))
                {
                    if (timer.isShowTime() && timer.isShowWhenStopped())
                    {
                        if (!timerObject.ContainsKey(timer))
                            timerObject[timer] = Instantiate(timerPrefab, timersContainer.transform);

                        var uiTimerController = timerObject[timer].GetComponent<UITimerController>();
                        uiTimerController.Timer = timer;
                        uiTimerController.Running = false;
                        uiTimerController.Reset();
                    }

                    if (ConditionChecker.check(timer.getInitCond()))
                    {
                        TimerVars timerVars = new TimerVars();
                        InitTimer(timer, timerVars);
                        runningTimers[timer] = timerVars;
                    }
                }

                // If the timer has been just started
                if (IsRunning(timer))
                {
                    // Timer can end either by using its end condition or else by not satisfying his init condition
                    if ((timer.isUsesEndCondition() && ConditionChecker.check(timer.getEndCond())) || !ConditionChecker.check(timer.getInitCond()))
                    {
                        StopTimer(timer);
                    }
                }
            }
        }

        private bool IsRunning(Timer t)
        {
            return runningTimers.ContainsKey(t);
        }
    }
}
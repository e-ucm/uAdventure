using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerController : MonoBehaviour {
    
    //#################################################################
    //########################### SINGLETON ###########################
    //#################################################################

    private enum TimerType { NORMAL, COUNTDOWN };
    private class TimerState
    {
        public Timer timer;
        public float current_time;
        public float max_time;
        public TimerType type;
    }

    private static TimerController instance;

    public static TimerController Instance{
        get {
            return instance;
        }
    }

    //######################################################################
    //########################### TIMER HANDLING ###########################
    //######################################################################

    //###### MONOBEHAVIOUR ######

    bool running = false;

    void Awake(){
        instance = this;
    }

    void Start(){
    }

    void Update () {
        if (!Game.Instance.isSomethingRunning() && running_timers.Count > 0) {
            foreach (TimerState ts in running_timers) {
                switch (ts.type) {
                case TimerType.NORMAL:
                    ts.current_time += Time.deltaTime;
                    if (ts.current_time >= ts.max_time) {
                        completed_timers.Push (ts.timer);
                        ts.current_time = 0f;
                    }
                    break;
                case TimerType.COUNTDOWN:
                    ts.current_time -= Time.deltaTime;
                    if (ts.current_time <= 0)
                        completed_timers.Push (ts.timer);
                    break;
                }
            }

            while(completed_timers.Count>0){
                if (Game.Instance.isSomethingRunning ())
                    break;
                Game.Instance.Execute (new EffectHolder (completed_timers.Pop().getEffects()));
            }
        }
    }

    //###### TIMERHANDLER ######
	
    List<Timer> timers = new List<Timer>();
    Stack<Timer> completed_timers = new Stack<Timer>();
    List<TimerState> running_timers = new List<TimerState> ();

    public List<Timer> Timers{
        get { return timers; }
        set { this.timers = value; }
    }

    public void Run(){
        this.running = true;
        checkTimers ();
    }

    public void checkTimers(){
        foreach (Timer t in timers) {
            if (ConditionChecker.check (t.getInitCond ())) {
                if (!isRunning (t)) {
                    TimerState ts = new TimerState ();
                    if (t.isCountDown ()) {
                        ts.type = TimerType.COUNTDOWN;
                        ts.max_time = 0;
                        ts.current_time = System.Convert.ToInt64(t.getTime ()*1000)/1000f;
                    } else {
                        ts.type = TimerType.NORMAL;
                        ts.max_time = System.Convert.ToInt64(t.getTime ()*1000)/1000f;
                        ts.current_time = 0;
                    }
                    ts.timer = t;
                
                    running_timers.Add (ts);
                }
            } else if (isRunning (t))
                removeFromRunning (t);
        }
    }

    private bool isRunning(Timer t){
        foreach (TimerState ts in running_timers) {
            if (t == ts.timer)
                return true;
        }
        return false;
    }

    private void removeFromRunning(Timer t){
        TimerState state = null;
        foreach (TimerState ts in running_timers) {
            if (t == ts.timer) {
                state = ts;
                break;
            }
        }
        if (state != null)
            running_timers.Remove (state);
    }
}

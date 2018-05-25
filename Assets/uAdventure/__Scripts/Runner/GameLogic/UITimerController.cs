using System;
using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public class UITimerController : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text timerText;

        private DateTime timeFormatter;

        public Timer Timer { get; set; }
        public bool Running { get; set; }

        public void Reset()
        {
            Init();
        }

        private void Init()
        {
            if (Timer.getTime() > int.MaxValue)
                Debug.LogWarning("The time is bigger than the 'int' container!");

            timeFormatter = new DateTime(0);

            if (Timer.isCountDown()) timeFormatter = new DateTime(Timer.getTime() * TimeSpan.TicksPerSecond, DateTimeKind.Local);

            UpdateView();
        }

        private void Update()
        {
            if (Running)
            {
                if(Timer.isCountDown())
                    timeFormatter = timeFormatter - new TimeSpan(Math.Min((long) (Time.deltaTime * 1000) * TimeSpan.TicksPerMillisecond, timeFormatter.Ticks));
                else
                    timeFormatter = timeFormatter.AddSeconds(Time.deltaTime);

                if (timeFormatter == new DateTime(0))
                    Running = false;
                UpdateView();
            }

        }

        private void UpdateView()
        {
            string sign = Timer.isCountDown() ? "-" : "";
            timerText.text = Timer.getDisplayName() + ": " + sign + timeFormatter.ToString("HH:mm:ss");
        }
    }
}
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
            timeFormatter = new DateTime(0, 0, 0, 0, 0, 0);
            if (Timer.isCountDown()) timeFormatter.AddSeconds(Timer.getTime());
        }

        private void Update()
        {
            string sign = "";

            if (Timer.isCountDown())
            {
                ///if (Running) timeFormatter = timeFormatter.Subtract(new TimeSpan(0, 0, 0, 0, Mathf.RoundToInt(Time.deltaTime * 1000)));
                sign = "-";
            }
            else
            {
                if (Running) timeFormatter.AddSeconds(Time.deltaTime);
            }

            timerText.text = Timer.getDisplayName() + ": " + sign + timeFormatter.ToString("HH:mm:ss");
        }
    }
}
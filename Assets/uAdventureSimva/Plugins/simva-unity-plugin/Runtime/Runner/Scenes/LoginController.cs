using UnityEngine;
using UnityFx.Async.Promises;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Simva
{
    // Manager for "Simva.Survey"
    public class LoginController : SimvaSceneController
    {
        private const string SIMVA_DISCLAIMER_ACCEPTED = "simva_disclaimer_accepted";
        private const int SIMVA_DISCLAIMER_ACCEPTED_TRUE = 1;
        private const int SIMVA_DISCLAIMER_ACCEPTED_FALSE = 0;

        private bool ready;
        public GameObject disclaimer;
        public GameObject login;
        public GameObject preview;

        public InputField token;

        public bool DisclaimerAccepted 
        { 
            get 
            {
                return PlayerPrefs.HasKey(SIMVA_DISCLAIMER_ACCEPTED) ? PlayerPrefs.GetInt(SIMVA_DISCLAIMER_ACCEPTED) == SIMVA_DISCLAIMER_ACCEPTED_TRUE : false;
            }
            set
            {
                PlayerPrefs.SetInt(SIMVA_DISCLAIMER_ACCEPTED, value ? SIMVA_DISCLAIMER_ACCEPTED_TRUE : SIMVA_DISCLAIMER_ACCEPTED_FALSE);
                PlayerPrefs.Save();
            }
        }

        protected void OnApplicationResume()
        {
        }

        public void Login()
        {
            var simvaExtension = SimvaManager.Instance;
            if (token == null || string.IsNullOrEmpty(token.text))
            {
                simvaExtension.NotifyManagers("Please insert a token");
                return;
            }

            if(token.text.ToLower() == "demo")
            {
                Demo();
            }
            else
            {
                simvaExtension.LoginAndSchedule(token.text);
            }
        }

        public void LoginWithKeykloak()
        {
            SimvaManager.Instance.LoginAndSchedule();
        }

        public void AcceptDisclaimer()
        {
            DisclaimerAccepted = true;
            disclaimer.SetActive(false);
            login.SetActive(true);
            preview.SetActive(true);
        }

        public void Demo()
        {
            SimvaManager.Instance.Demo();
        }

        public override void Destroy()
        {
            GameObject.DestroyImmediate(this.gameObject);
        }

        public override void Render()
        {
            if (DisclaimerAccepted)
            {
                AcceptDisclaimer();
            }
            //var background = GameObject.Find("background").GetComponent<Image>();
            /*var backgroundPath = 
            var backgroundSprite = Game.Instance.ResourceManager.getSprite();
            background.sprite = Game.Instance.ResourceManager.getSprite()*/
            Ready = true;
        }
    }
}


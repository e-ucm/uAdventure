using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using uAdventure.Runner;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace uAdventure.Simva
{
    // Manager for "Simva.Survey"
    public class LoginController : MonoBehaviour, IRunnerChapterTarget
    {
        private const string SIMVA_DISCLAIMER_ACCEPTED = "simva_disclaimer_accepted";
        private const int SIMVA_DISCLAIMER_ACCEPTED_TRUE = 1;
        private const int SIMVA_DISCLAIMER_ACCEPTED_FALSE = 0;

        private bool ready;
        public GameObject disclaimer;
        public GameObject login;
        public GameObject preview;

        public InputField token;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return ready; } }

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
            if(token == null || string.IsNullOrEmpty(token.text))
            {
                SimvaExtension.Instance.NotifyManagers("Please insert a token");
                return;
            }

            if(token.text.ToLower() == "demo")
            {
                Demo();
            }
            else
            {
                SimvaExtension.Instance.LoginAndSchedule(token.text);
            }
        }

        public void LoginWithKeykloak()
        {
            SimvaExtension.Instance.LoginAndSchedule();
        }

        public void RenderScene()
        {
            InventoryManager.Instance.Show = false;
            if (DisclaimerAccepted)
            {
                AcceptDisclaimer();
            }
            //var background = GameObject.Find("background").GetComponent<Image>();
            /*var backgroundPath = 
            var backgroundSprite = Game.Instance.ResourceManager.getSprite();
            background.sprite = Game.Instance.ResourceManager.getSprite()*/
            ready = true;
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
            PreviewManager.Instance.InPreviewMode = true;
            uAdventure.Geo.GeoExtension.Instance.UsingDebugLocation = true;
            uAdventure.Geo.GeoExtension.Instance.Location = new Vector2d(40.4436403741371, -3.72659319639157);
            Game.Instance.Restart();
            Game.Instance.GameState.Data.setAutoSave(false);
            Game.Instance.GameState.Data.setSaveOnSuspend(false);
            Game.Instance.RunTarget(Game.Instance.GameState.InitialChapterTarget.getId());
        }

        public void Destroy(float time, Action onDestroy)
        {
            GameObject.DestroyImmediate(this.gameObject);
            onDestroy();
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void setInteractuable(bool state)
        {
        }
    }
}


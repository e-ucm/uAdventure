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
        private bool ready;

        public InputField token;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return ready; } }

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

            SimvaExtension.Instance.LoginAndSchedule(token.text);
        }

        public void LoginWithKeykloak()
        {
            SimvaExtension.Instance.LoginAndSchedule();
        }

        public void RenderScene()
        {
            InventoryManager.Instance.Show = false;
            //var background = GameObject.Find("background").GetComponent<Image>();
            /*var backgroundPath = 
            var backgroundSprite = Game.Instance.ResourceManager.getSprite();
            background.sprite = Game.Instance.ResourceManager.getSprite()*/
            ready = true;
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


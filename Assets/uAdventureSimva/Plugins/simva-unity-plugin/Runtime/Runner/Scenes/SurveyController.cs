using UnityEngine;
using UnityFx.Async.Promises;

namespace Simva
{
    // Manager for "Simva.Survey"
    public class SurveyController : SimvaSceneController
    {
        private bool surveyOpened;

        public void OpenSurvey()
        {
            var simvaExtension = SimvaManager.Instance;
            simvaExtension.NotifyLoading(true);
            string activityId = simvaExtension.CurrentActivityId;
            string username = simvaExtension.API.Authorization.Agent.name;
            simvaExtension.API.Api.GetActivityTarget(activityId)
                .Then(result =>
                {
                    simvaExtension.NotifyLoading(false);
                    surveyOpened = true;
                    Application.OpenURL(result[username]);
                })
                .Catch(error =>
                {
                    simvaExtension.NotifyManagers(error.Message);
                    simvaExtension.NotifyLoading(false);
                });
        }

        protected void OnApplicationResume()
        {
            if (surveyOpened)
            {
                surveyOpened = false;
                CheckSurvey();
            }
        }

        public void CheckSurvey()
        {
            SimvaManager.Instance.ContinueSurvey();
        }

        public override void Render() 
        {
            Ready = true;
            //var background = GameObject.Find("background").GetComponent<Image>();
            /*var backgroundPath = 
            var backgroundSprite = Game.Instance.ResourceManager.getSprite();
            background.sprite = Game.Instance.ResourceManager.getSprite()*/
        }

        public override void Destroy()
        {
            GameObject.DestroyImmediate(gameObject);
        }
    }
}


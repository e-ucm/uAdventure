using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using uAdventure.Runner;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityFx.Async;
using Simva.Model;

namespace uAdventure.Simva
{
    // Manager for "Simva.Survey"
    public class SurveyController : MonoBehaviour, IRunnerChapterTarget
    {
        private bool surveyOpened;
        private bool ready;
        private GameObject surveyOpener;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return ready; } }

        public void OpenSurvey()
        {
            SimvaExtension.Instance.NotifyLoading(true);
            string activityId = SimvaExtension.Instance.CurrentActivityId;
            string username = SimvaExtension.Instance.API.AuthorizationInfo.Username;
            SimvaExtension.Instance.API.Api.GetActivityTarget(activityId)
                .Then(result =>
                {
                    SimvaExtension.Instance.NotifyLoading(false);
                    surveyOpened = true;
                    Application.OpenURL(result[username]);
                })
                .Catch(error =>
                {
                    SimvaExtension.Instance.NotifyManagers(error.Message);
                    SimvaExtension.Instance.NotifyLoading(false);
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
            SimvaExtension.Instance.NotifyLoading(true);
            string activityId = SimvaExtension.Instance.CurrentActivityId;
            string token = SimvaExtension.Instance.Token;
            string username = SimvaExtension.Instance.API.AuthorizationInfo.Username;
            SimvaExtension.Instance.API.Api.GetCompletion(activityId, username)
                .Then(result =>
                {
                    if (result[username])
                    {
                        return SimvaExtension.Instance.UpdateSchedule();
                    }
                    else
                    {
                        SimvaExtension.Instance.NotifyManagers("Survey not completed");
                        SimvaExtension.Instance.NotifyLoading(false);
                        var nullresult = new AsyncCompletionSource<Schedule>();
                        nullresult.SetResult(null);
                        return nullresult;
                    }
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    if (schedule != null)
                    {
                        StartCoroutine(SimvaExtension.Instance.AsyncCoroutine(SimvaExtension.Instance.LaunchActivity(schedule.Next), result));
                    }
                    else
                    {
                        result.SetException(new Exception("No schedule!"));
                    }
                    return result;
                })
                .Catch(error =>
                {
                    SimvaExtension.Instance.NotifyManagers(error.Message);
                    SimvaExtension.Instance.NotifyLoading(false);
                });
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
            GameObject.DestroyImmediate(gameObject);
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


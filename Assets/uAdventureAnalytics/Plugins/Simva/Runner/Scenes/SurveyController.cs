using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using uAdventure.Runner;
using System;
using UnityEngine.EventSystems;
using UnityFx.Async;
using Simva.Model;

namespace uAdventure.Simva
{
    // Manager for "Simva.Survey"
    public class SurveyController : MonoBehaviour, IRunnerChapterTarget
    {
        private bool surveyOpened;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return true; } }

        public void OpenSurvey()
        {
            var simvaExtension = GameExtension.GetInstance<SimvaExtension>();
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
            var simvaExtension = GameExtension.GetInstance<SimvaExtension>();
            simvaExtension.NotifyLoading(true);
            string activityId = simvaExtension.CurrentActivityId;
            string username = simvaExtension.API.Authorization.Agent.name;
            simvaExtension.API.Api.GetCompletion(activityId, username)
                .Then(result =>
                {
                    if (result[username])
                    {
                        return simvaExtension.UpdateSchedule();
                    }
                    else
                    {
                        simvaExtension.NotifyManagers("Survey not completed");
                        simvaExtension.NotifyLoading(false);
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
                        StartCoroutine(simvaExtension.AsyncCoroutine(simvaExtension.LaunchActivity(schedule.Next), result));
                    }
                    else
                    {
                        result.SetException(new Exception("No schedule!"));
                    }
                    return result;
                })
                .Catch(error =>
                {
                    simvaExtension.NotifyManagers(error.Message);
                    simvaExtension.NotifyLoading(false);
                });
        }

        public void RenderScene() 
        {
            InventoryManager.Instance.Show = false;
            //var background = GameObject.Find("background").GetComponent<Image>();
            /*var backgroundPath = 
            var backgroundSprite = Game.Instance.ResourceManager.getSprite();
            background.sprite = Game.Instance.ResourceManager.getSprite()*/
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


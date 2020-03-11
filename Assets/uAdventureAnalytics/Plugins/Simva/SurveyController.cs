using UnityEngine;
using System.Collections;
using UniRx;
using SimpleJSON;

namespace uAdventure.Analytics
{
    public class SurveyController : MonoBehaviour
    {
        public void OpenSurvey()
        {
            string survey = SimvaController.Instance.CurrentActivityId;
            StartCoroutine(LoadTargetAndOpen(survey));
        }

        public void CheckSurvey()
        {
            string survey = SimvaController.Instance.CurrentActivityId;
            StartCoroutine(CheckStatusAndContinue(survey));
        }

        protected IEnumerator LoadTargetAndOpen(string activityId)
        {

            SimvaController.Instance.NotifyManagers("Loading Link");

            var cd = new SimvaController.CoroutineWithResult(this, SimvaController.Instance.GetTarget(activityId));
            yield return cd.Coroutine;

            var result = (Tuple<string, string>)cd.Result;

            if (result.Item1 != null)
            {
                var body = JSON.Parse(result.Item1);
                SimvaController.Instance.NotifyManagers(body["message"].Value);
            }
            else
            {
                SimvaController.Instance.NotifyManagers("");
                var body = JSON.Parse(result.Item2);
                Application.OpenURL(body[SimvaController.Instance.Token].Value);
            }
        }

        protected IEnumerator CheckStatusAndContinue(string activityId)
        {
            SimvaController.Instance.NotifyLoading(true);
            var cd = new SimvaController.CoroutineWithResult(this, SimvaController.Instance.GetCompletion(activityId));
            yield return cd.Coroutine;

            var result = (Tuple<string, string>)cd.Result;

            if (result.Item1 != null)
            {
                SimvaController.Instance.NotifyLoading(false);
                SimvaController.Instance.NotifyManagers("Error");
                var body = JSON.Parse(result.Item1);
                if (body == null)
                {
                    SimvaController.Instance.NotifyManagers("Unable to connect");
                }
                else
                {
                    SimvaController.Instance.NotifyManagers(body["message"].Value);
                }
            }
            else
            {
                var body = JSON.Parse(result.Item2);
                if (body[SimvaController.Instance.Token].AsBool)
                {
                    cd = new SimvaController.CoroutineWithResult(this, SimvaController.Instance.GetSchedule());
                    yield return cd.Coroutine;

                    SimvaController.Instance.LaunchActivity(SimvaController.Instance.Schedule["next"].Value);
                }
                else
                {
                    SimvaController.Instance.NotifyLoading(false);
                    SimvaController.Instance.NotifyManagers("Survey not completed");
                }
            }
        }
    }
}


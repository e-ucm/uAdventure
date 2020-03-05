using UnityEngine;
using System.Collections;
using UniRx;
using SimpleJSON;

public class SurveyController : MonoBehaviour {
	void Start () {
	}
	void Update () {
	}

    public void OpenSurvey()
    {
        string survey = SimvaController.Instance.getCurrentActivityId();
        StartCoroutine(LoadTargetAndOpen(survey));
    }

    public void CheckSurvey()
    {
        string survey = SimvaController.Instance.getCurrentActivityId();
        StartCoroutine(CheckStatusAndContinue(survey));
    }

    IEnumerator LoadTargetAndOpen(string activityId) {

        SimvaController.Instance.NotifyManagers("Loading Link");

        SimvaController.CoroutineWithData cd = new SimvaController.CoroutineWithData(this, SimvaController.Instance.getTarget(activityId));
        yield return cd.coroutine;

        Tuple<string, string>  result = (Tuple<string, string>) cd.result;

        if(result.Item1 != null)
        {
            JSONNode body = JSON.Parse(result.Item1);
            SimvaController.Instance.NotifyManagers(body["message"].Value);
        }
        else
        {
            SimvaController.Instance.NotifyManagers("");
            JSONNode body = JSON.Parse(result.Item2);
            Application.OpenURL(body[SimvaController.Instance.Token].Value);
        }
    }

    IEnumerator CheckStatusAndContinue(string activityId)
    {
        SimvaController.Instance.NotifyLoading(true);
        SimvaController.CoroutineWithData cd = new SimvaController.CoroutineWithData(this, SimvaController.Instance.getCompletion(activityId));
        yield return cd.coroutine;

        Tuple<string, string> result = (Tuple<string, string>)cd.result;

        

        if (result.Item1 != null)
        {
            SimvaController.Instance.NotifyLoading(false);
            SimvaController.Instance.NotifyManagers("Error");
            JSONNode body = JSON.Parse(result.Item1);
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
            JSONNode body = JSON.Parse(result.Item2);
            if (body[SimvaController.Instance.Token].AsBool)
            {
                cd = new SimvaController.CoroutineWithData(this, SimvaController.Instance.getSchedule());
                yield return cd.coroutine;

                SimvaController.Instance.LaunchActivity(SimvaController.Instance.Schedule["next"].Value);
            }else
            {
                SimvaController.Instance.NotifyLoading(false);
                SimvaController.Instance.NotifyManagers("Survey not completed");
            }
        }
    }
}

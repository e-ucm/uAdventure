using UnityEngine;
using System.Collections;

public class WebOpener : MonoBehaviour {
	void Start () {
	}
	void Update () {
	}

    public void OpenWeb(string web)
    {
        Application.OpenURL(web);
    }

    public void OpenSurvey()
    {
		string survey = PlayerPrefs.GetString ("LimesurveyPre");
		string type = "pre";

		if (PlayerPrefs.HasKey ("CurrentSurvey"))
			type = PlayerPrefs.GetString ("CurrentSurvey");

		if(type == "pre")
			survey = PlayerPrefs.GetString ("LimesurveyPre");
		else if(type == "post")
			survey = PlayerPrefs.GetString ("LimesurveyPost");
		
		string url = PlayerPrefs.GetString("LimesurveyHost") + "surveys/survey/" + survey + "?token=" + PlayerPrefs.GetString("LimesurveyToken");
		if (!url.Contains("http://") && !url.Contains("https://"))
			url = "http://" + url;

		Application.OpenURL(url);
    }
}

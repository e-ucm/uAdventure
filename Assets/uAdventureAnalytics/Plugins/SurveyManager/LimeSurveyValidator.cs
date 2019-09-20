using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LimeSurveyValidator : MonoBehaviour {

    Net connection;
    string host = "localhost";
	string survey_pre = "", survey_post = "", master_token_online = "", master_token_offline = "";

    public Text token, response;
    
	void Start () {
        connection = new Net(this);

        host = "https://analytics.e-ucm.es/api/proxy/surveymanager/";
        survey_pre = "894259";
        survey_post = "783912";

        master_token_online = "online";
        master_token_offline = "offline";

        PlayerPrefs.SetString("LimesurveyHost", host);
		if(survey_pre != "")
			PlayerPrefs.SetString("LimesurveyPre", survey_pre);
		if(survey_post != "")
        	PlayerPrefs.SetString("LimesurveyPost", survey_post);
        PlayerPrefs.Save();
    }
    
    void Update () {
	
	}

    public void validate()
    {
        string token = "";
        if (this.token != null)
            token = this.token.text.ToUpper();
        else if (PlayerPrefs.HasKey("LimesurveyToken"))
            token = PlayerPrefs.GetString("LimesurveyToken");

		PlayerPrefs.SetInt ("online", 1);

		if (token == master_token_online || token == master_token_offline) {
			PlayerPrefs.SetString ("LimesurveyToken", "ADMIN");
			PlayerPrefs.SetString ("name", "ADMIN");

			if (token == master_token_offline) {
				PlayerPrefs.SetInt ("online", 0);
			}else
				PlayerPrefs.SetInt ("online", 1);

			SceneManager.LoadScene ("_Scene1");
		}
        connection.GET(host + "surveys/validate?survey=" + survey_pre + ((token.Length>0)? "&token=" + token : ""), new ValidateListener(response, token));
    }

    public void completed()
    {
        string token = "";
        if (this.token != null)
            token = this.token.text.ToUpper();
        else if (PlayerPrefs.HasKey("LimesurveyToken"))
            token = PlayerPrefs.GetString("LimesurveyToken");

		string survey = PlayerPrefs.GetString ("LimesurveyPre");
		string type = "pre";

		if (PlayerPrefs.HasKey ("CurrentSurvey"))
			type = PlayerPrefs.GetString ("CurrentSurvey");

		if(type == "pre")
			survey = PlayerPrefs.GetString ("LimesurveyPre");
		else if(type == "post")
			survey = PlayerPrefs.GetString ("LimesurveyPost");

		connection.GET(host + "surveys/completed?survey=" + survey + ((token.Length > 0) ? "&token=" + token : ""), new CompleteListener(response, token));
    }

    public class ValidateListener : Net.IRequestListener
    {
        Text response;
        string token;

        public ValidateListener(Text response, string token)
        {
            this.response = response;
            this.token = token;
        }

        public void Error(string error)
        {
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(error);
			if (result != null && result ["message"] != null)
				response.text = result["message"];
			else
				response.text = error != ""? error : "Can't Connect";
        }

        public void Result(string data)
        {
			PlayerPrefs.SetString("name", token);
            PlayerPrefs.SetString("LimesurveyToken", token);
            PlayerPrefs.Save();
			if (PlayerPrefs.HasKey ("LimesurveyPre")) {
				PlayerPrefs.SetString ("CurrentSurvey", "pre");
				SceneManager.LoadScene ("_Survey");
			}else
                SceneManager.LoadScene("_Scene1");
        }
    }

	public class CompleteListener : Net.IRequestListener
    {
        Text response;
        string token;

        public CompleteListener(Text response, string token)
        {
            this.response = response;
            this.token = token;
        }

        public void Error(string error)
        {
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(error);
			response.text = result["message"];
        }

        public void Result(string data)
        {
			string type = "pre";

			if (PlayerPrefs.HasKey ("CurrentSurvey"))
				type = PlayerPrefs.GetString ("CurrentSurvey");

			if (type == "pre")
				SceneManager.LoadScene ("_Scene1");
			else if (type == "post")
				Application.Quit ();
        }
    }
}

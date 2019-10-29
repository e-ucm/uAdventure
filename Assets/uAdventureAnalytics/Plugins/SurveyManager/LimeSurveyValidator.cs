using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LimeSurveyValidator : MonoBehaviour {

    Net connection;
    string host = "localhost";
	string survey_pre = "", survey_post = "", master_token_online = "", master_token_offline = "";

    public Text token, response, loading;
    public GameObject form;
    
	void Start () {
        connection = new Net(this);

        host = "https://analytics.e-ucm.es/api/proxy/surveymanager/";
        survey_pre = "511926";
        survey_post = "173494";

        master_token_online = "online";
        master_token_offline = "offline";

        PlayerPrefs.SetString("LimesurveyHost", host);
		if(survey_pre != "")
			PlayerPrefs.SetString("LimesurveyPre", survey_pre);
		if(survey_post != "")
        	PlayerPrefs.SetString("LimesurveyPost", survey_post);
        PlayerPrefs.Save();

        var token = PlayerPrefs.GetString("name");
        if (!string.IsNullOrEmpty(token))
        {
            if(SceneManager.GetActiveScene().name == "_Login")
            { 
                validate(token);
            }
            if (SceneManager.GetActiveScene().name == "_Survey")
            {
                completed(token);
            }
        }
        else
        {
            loading.gameObject.SetActive(false);
            form.gameObject.SetActive(true);
        }
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

        validate(token);
    }

    private void validate(string token)
    {
        PlayerPrefs.SetInt("online", 1);

        if (token == master_token_online || token == master_token_offline)
        {
            PlayerPrefs.SetString("LimesurveyToken", "ADMIN");
            PlayerPrefs.SetString("name", "ADMIN");

            if (token == master_token_offline)
            {
                PlayerPrefs.SetInt("online", 0);
            }
            else
                PlayerPrefs.SetInt("online", 1);

            SceneManager.LoadScene("_Scene1");
        }
        connection.GET(host + "surveys/validate?survey=" + survey_pre + ((token.Length > 0) ? "&token=" + token : ""), new ValidateListener(response, token, this));

    }

    public void completed()
    {
        string token = "";
        if (this.token != null)
            token = this.token.text.ToUpper();
        else if (PlayerPrefs.HasKey("LimesurveyToken"))
            token = PlayerPrefs.GetString("LimesurveyToken");

        completed(token);	
    }

    public void completed(string token)
    {
        string survey = PlayerPrefs.GetString("LimesurveyPre");
        string type = "pre";

        if (PlayerPrefs.HasKey("CurrentSurvey"))
            type = PlayerPrefs.GetString("CurrentSurvey");

        if (type == "pre")
            survey = PlayerPrefs.GetString("LimesurveyPre");
        else if (type == "post")
            survey = PlayerPrefs.GetString("LimesurveyPost");

        connection.GET(host + "surveys/completed?survey=" + survey + ((token.Length > 0) ? "&token=" + token : ""), new CompleteListener(response, token, this));
    }

    public class ValidateListener : Net.IRequestListener
    {
        Text response;
        string token;
        LimeSurveyValidator validator;

        public ValidateListener(Text response, string token, LimeSurveyValidator validator)
        {
            this.response = response;
            this.token = token;
            this.validator = validator;
        }

        public void Error(string error)
        {
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(error);
			if (result != null && result ["message"] != null)
				response.text = result["message"];
			else
				response.text = error != ""? error : "Can't Connect";

            validator.loading.gameObject.SetActive(false);
            validator.form.gameObject.SetActive(true);
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
        LimeSurveyValidator validator;

        public CompleteListener(Text response, string token, LimeSurveyValidator validator)
        {
            this.response = response;
            this.token = token;
            this.validator = validator;
        }

        public void Error(string error)
        {
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(error);
			response.text = result["message"];
            validator.loading.gameObject.SetActive(false);
            validator.form.gameObject.SetActive(true);
        }

        public void Result(string data)
        {
			string type = "pre";

			if (PlayerPrefs.HasKey ("CurrentSurvey"))
				type = PlayerPrefs.GetString ("CurrentSurvey");

			if (type == "pre")
            {
                validator.loading.text = "Loading game...";
                SceneManager.LoadScene("_Scene1");
            }
			else if (type == "post")
				Application.Quit ();
        }
    }
}

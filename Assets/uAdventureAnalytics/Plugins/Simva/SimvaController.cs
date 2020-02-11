using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UniRx;
using System.Collections.Generic;

public class SimvaController : MonoBehaviour {
    
    string host = "localhost";
    string port = "443";
    string protocol = "https";

    string token = null;
    string jwt = null;

    string URL
    {
        get {
            return protocol + "://" + host + ":" + port;
        }
    }

    string study = "";

    string master_token_online = "online", master_token_offline = "offline";

    public Text tokenText, responseText;
    
	void Start () {

#if UNITY_WEBPLAYER || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IPHONE
#else
        SimpleJSON.JSONNode simvaconf = new SimpleJSON.JSONClass();

        if (System.IO.File.Exists("simva.conf")) {
            simvaconf = SimpleJSON.JSON.Parse(System.IO.File.ReadAllText("simva.conf"));
            this.study = simvaconf["study"];
            this.host = simvaconf["host"];
            this.protocol = simvaconf["protocol"];
            this.port = simvaconf["port"];
        }
#endif

        PlayerPrefs.SetString("simvahost", host);
		PlayerPrefs.SetString("simvastudy", study);
        PlayerPrefs.Save();

        //var token = PlayerPrefs.GetString("name");
        this.token = "trfs";
    }
    
    void Update () {
	
	}

    public void InitUser()
    {
        this.StartCoroutine(LoginAndSchedule());
    }

    public IEnumerator LoginAndSchedule()
    {
        CoroutineWithData cd = new CoroutineWithData(this, Login());
        yield return cd.coroutine;

        Tuple<string, string> result = (Tuple<string, string>) cd.result;

        if (result.Item1 != null)
        {
            Debug.Log("login failed");
            Debug.Log(result.Item1);
        }else
        {
            Debug.Log("login success");
            SimpleJSON.JSONNode body = SimpleJSON.JSON.Parse(result.Item2);
            this.jwt = body["token"];
            Debug.Log(this.jwt);
        }
    }

    public IEnumerator Login()
    {
        /*if (this.token != null)
            token = this.tokenText.text.ToLower();
        else if (PlayerPrefs.HasKey("simvatoken"))
            token = PlayerPrefs.GetString("simvatoken");*/
        CoroutineWithData cd = new CoroutineWithData(this, Login(token, token));
        yield return cd.coroutine;

        yield return cd.result;
    }

    private IEnumerator Login(string username, string password)
    {
        SimpleJSON.JSONNode body = new SimpleJSON.JSONClass();

        body.Add("username", username);
        body.Add("password", password);

        string bodystring = body.ToString();

        UnityWebRequest www = new UnityWebRequest(this.URL + "/users/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodystring);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError) {
            yield return new Tuple<string, string>("{\"message\": \"Unable to connect\"}", null);
        }
        else {
            if(www.responseCode != 200)
            {
                yield return new Tuple<string, string>(www.downloadHandler.text, null);
            }else
            {
                yield return new Tuple<string, string>(null, www.downloadHandler.text);
            }
        }
    }

    private class CoroutineWithData
    {
        public Coroutine coroutine { get; private set; }
        public object result;
        private IEnumerator target;
        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            this.coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (target.MoveNext())
            {
                result = target.Current;
                yield return result;
            }
        }
    }
}

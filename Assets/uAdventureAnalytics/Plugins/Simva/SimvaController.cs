using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UniRx;
using System.Collections.Generic;
using SimpleJSON;

public class SimvaController : MonoBehaviour {

    static SimvaController instance;
    public static SimvaController Instance
    {
        get { return SimvaController.instance; }
    }

    List<SimvaResponseManager> responseManagers = new List<SimvaResponseManager>();
    string host = "localhost",  port = "443", protocol = "https";
    string study = "", master_token_online = "online", master_token_offline = "offline";
    string jwt = null;
    JSONNode schedule;

    string URL
    {
        get {
            return protocol + "://" + host + ":" + port;
        }
    }

    public JSONNode Schedule
    {
        get
        {
            return schedule;
        }
    }

    string user;
    public string Token
    {
        get
        {
            if (user != null)
            {
                return user;
            }
            else if (PlayerPrefs.HasKey("username"))
            {
                return PlayerPrefs.GetString("username");
            }
            else if(PlayerPrefs.HasKey("simvatoken"))
            {
                return PlayerPrefs.GetString("simvatoken");
            }else
            {
                return "";
            }
        }
    }
    
    string Study
    {
        get
        {
            if (this.study != null && this.study != "")
            {
                return this.study;
            }
            else if (PlayerPrefs.HasKey("simvastudy"))
            {
                return PlayerPrefs.GetString("simvastudy");
            }
            else
            {
                return "";
            }
        }
    }

    public bool isActive()
    {
        return (this.jwt != null && this.user != null && Schedule != null);
    }

    private void Awake()
    {
        SimvaController.instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

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
    }

    public void AddResponseManager(SimvaResponseManager manager)
    {
        responseManagers.Add(manager);
    }

    public void RemoveResponseManager(SimvaResponseManager manager)
    {
        responseManagers.Remove(manager);
    }

    public void NotifyManagers(string message)
    {
        foreach(SimvaResponseManager responseManager in responseManagers)
        {
            responseManager.Notify(message);
        }
    }

    public void InitUser()
    {
        this.StartCoroutine(LoginAndSchedule());
    }

    public string getCurrentActivityId()
    {
        if (schedule != null)
        {
            return schedule["next"].Value;
        }
        return null;
    }

    public JSONNode getActivity(string activityId)
    {
        if (schedule != null)
        {
            return schedule["activities"][activityId];
        }
        return null;
    }

    public void LaunchActivity(string activityId)
    {
        if(activityId == null)
        {
            SceneManager.LoadScene("_End");
        }
        else
        {
            JSONNode activity = getActivity(activityId);

            if (activity != null)
            {
                switch (activity["type"].Value)
                {
                    case "limesurvey":
                        SceneManager.LoadScene("_Survey");
                        break;
                    case "activity":
                    default:
                        SceneManager.LoadScene("_Scene1");
                        break;
                }
            }
        }
    }

    public IEnumerator LoginAndSchedule()
    {
        CoroutineWithData cd = new CoroutineWithData(this, Login());
        yield return cd.coroutine;

        Tuple<string, string> result = (Tuple<string, string>) cd.result;

        if (result.Item1 != null)
        {
            Debug.Log("login failed");
            JSONNode body = JSON.Parse(result.Item1);
            NotifyManagers(body["message"].Value);
        }
        else
        {
            NotifyManagers("login success");
            JSONNode body = JSON.Parse(result.Item2);
            this.jwt = body["token"];
            this.user = Token;

            cd = new CoroutineWithData(this, getSchedule());
            yield return cd.coroutine;

            result = (Tuple<string, string>)cd.result;

            if (result.Item1 != null)
            {
                Debug.Log("Getting Schedule Failed");
                body = JSON.Parse(result.Item1);
                NotifyManagers(body["message"].Value);
            }
            else
            {
                body = JSON.Parse(result.Item2);
                LaunchActivity(body["next"].Value);
            }
        }
    }

    public IEnumerator Login()
    {
        CoroutineWithData cd = new CoroutineWithData(this, Login(Token, Token));
        yield return cd.coroutine;
        yield return cd.result;
    }

    private IEnumerator Login(string username, string password)
    {
        JSONNode body = new JSONClass();

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

    public IEnumerator getSchedule()
    {
        UnityWebRequest www = UnityWebRequest.Get(this.URL + "/studies/" + Study + "/schedule");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + this.jwt);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new Tuple<string, string>("{\"message\": \"Unable to connect\"}", null);
        }
        else
        {
            if (www.responseCode != 200)
            {
                yield return new Tuple<string, string>(www.downloadHandler.text, null);
            }
            else
            {
                this.schedule = JSON.Parse(www.downloadHandler.text);
                yield return new Tuple<string, string>(null, www.downloadHandler.text);
            }
        }
    }

    public IEnumerator getTarget(string activityId)
    {
        UnityWebRequest www = UnityWebRequest.Get(this.URL + "/activities/" + activityId + "/target");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + this.jwt);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new Tuple<string, string>("{\"message\": \"Unable to connect\"}", null);
        }
        else
        {
            if (www.responseCode != 200)
            {
                yield return new Tuple<string, string>(www.downloadHandler.text, null);
            }
            else
            {
                yield return new Tuple<string, string>(null, www.downloadHandler.text);
            }
        }
    }

    public IEnumerator getCompletion(string activityId)
    {
        UnityWebRequest www = UnityWebRequest.Get(this.URL + "/activities/" + activityId + "/completion");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + this.jwt);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new Tuple<string, string>("{\"message\": \"Unable to connect\"}", null);
        }
        else
        {
            if (www.responseCode != 200)
            {
                yield return new Tuple<string, string>(www.downloadHandler.text, null);
            }
            else
            {
                yield return new Tuple<string, string>(null, www.downloadHandler.text);
            }
        }
    }

    public IEnumerator setResults(string activityId, string results)
    {
        JSONNode body = new JSONClass();
        body.Add("result", results);
        string bodystring = body.ToString();

        UnityWebRequest www = new UnityWebRequest(this.URL + "/activities/" + activityId + "/result", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodystring);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + this.jwt);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new Tuple<string, string>("{\"message\": \"Unable to connect\"}", null);
        }
        else
        {
            if (www.responseCode != 200)
            {
                yield return new Tuple<string, string>(www.downloadHandler.text, null);
            }
            else
            {
                yield return new Tuple<string, string>(null, www.downloadHandler.text);
            }
        }
    }

    public IEnumerator setCompletion(string activityId, bool completion)
    {
        JSONNode body = new JSONClass();
        body.Add("status", new JSONData(completion));
        string bodystring = body.ToString();

        UnityWebRequest www = new UnityWebRequest(this.URL + "/activities/" + activityId + "/completion", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodystring);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + this.jwt);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new Tuple<string, string>("{\"message\": \"Unable to connect\"}", null);
        }
        else
        {
            if (www.responseCode != 200)
            {
                yield return new Tuple<string, string>(www.downloadHandler.text, null);
            }
            else
            {
                yield return new Tuple<string, string>(null, www.downloadHandler.text);
            }
        }
    }

    public class CoroutineWithData
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

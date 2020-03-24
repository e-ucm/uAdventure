using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UniRx;
using System.Collections.Generic;
using SimpleJSON;
using uAdventure.Runner;

namespace uAdventure.Analytics
{
    public class SimvaController : Singleton<SimvaController>
    {
        public delegate void LoadingDelegate(bool loading);
        public delegate void ResponseDelegate(string message);

        private LoadingDelegate loadingListeners;
        private ResponseDelegate responseListeners;

        private string user;
        private string host = "localhost", port = "443", protocol = "https";
        private string study = "";
        private string jwt = null;

        JSONNode schedule;

        public string Host
        {
            get
            {
                return host;
            }
        }

        public string Port
        {
            get
            {
                return port;
            }
        }

        public string Protocol
        {
            get
            {
                return protocol;
            }
        }

        public string URL
        {
            get
            {
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
                else if (PlayerPrefs.HasKey("simvatoken"))
                {
                    return PlayerPrefs.GetString("simvatoken");
                }
                else
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

        public bool IsActive
        {
            get
            {
                return this.jwt != null && this.user != null && Schedule != null;
            }
        }

        protected void Start()
        {
            var FileName = "simva.conf";
            string contents = "";

            // Platform dependent StreamingAssets Load https://docs.unity3d.com/Manual/StreamingAssets.html
#if UNITY_WEBPLAYER || UNITY_WEBGL
            WWW reader = new WWW(Application.streamingAssetsPath + "/" + FileName);
#elif UNITY_ANDROID
            WWW reader = new WWW("jar: file://" + Application.dataPath + "!/assets/" + FileName);
#elif UNITY_IOS
            var filePath = Path.Combine(Application.dataPath + "/Raw", FileName);
#else
            var filePath = System.IO.Path.Combine(Application.streamingAssetsPath + "/", FileName);
#endif

            // WebGL and Android have to use WWW to load from streaming assets
#if UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_ANDROID 
            while (!reader.isDone) { }
            if (string.IsNullOrEmpty(reader.error))
            {
                contents = System.Text.Encoding.UTF8.GetString(reader.bytes);
            }
#else       // The others can read from System.IO            
            if (System.IO.File.Exists(filePath))
            {
                contents = System.IO.File.ReadAllText(filePath);
            }
#endif
            if (!string.IsNullOrEmpty(contents))
            {
                var simvaconf = SimpleJSON.JSON.Parse(contents);
                this.study = simvaconf["study"];
                this.host = simvaconf["host"];
                this.protocol = simvaconf["protocol"];
                this.port = simvaconf["port"];
            }

            PlayerPrefs.SetString("simvahost", host);
            PlayerPrefs.SetString("simvastudy", study);
            PlayerPrefs.Save();
        }

        public void AddResponseManager(SimvaResponseManager manager)
        {
            if (manager)
            {
                // To make sure we only have one instance of a notify per manager
                // We first remove (as it is ignored if not present)
                responseListeners -= manager.Notify;
                // Then we add it
                responseListeners += manager.Notify;
            }
        }

        public void RemoveResponseManager(SimvaResponseManager manager)
        {
            if (manager)
            {
                // If a delegate is not present the method gets ignored
                responseListeners -= manager.Notify;
            }
        }

        public void AddLoadingManager(SimvaLoadingManager manager)
        {
            if (manager)
            {
                // To make sure we only have one instance of a notify per manager
                // We first remove (as it is ignored if not present)
                loadingListeners -= manager.IsLoading;
                // Then we add it
                loadingListeners += manager.IsLoading;
            }
        }

        public void RemoveLoadingManager(SimvaLoadingManager manager)
        {
            if (manager)
            {
                // If a delegate is not present the method gets ignored
                loadingListeners += manager.IsLoading;
            }
        }

        public void NotifyManagers(string message)
        {
            if(responseListeners != null)
            {
                responseListeners(message);
            }
        }

        public void NotifyLoading(bool state)
        {
            if (loadingListeners != null)
            {
                loadingListeners(state);
            }
        }

        public void InitUser()
        {
            this.StartCoroutine(LoginAndSchedule());
        }

        public string CurrentActivityId
        {
            get
            {
                if (schedule != null)
                {
                    return schedule["next"].Value;
                }
                return null;
            }
        }

        public JSONNode GetActivity(string activityId)
        {
            if (schedule != null)
            {
                return schedule["activities"][activityId];
            }
            return null;
        }

        public void LaunchActivity(string activityId)
        {
            if (activityId == null)
            {
                SceneManager.LoadScene("_End");
            }
            else
            {
                JSONNode activity = GetActivity(activityId);

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

        protected IEnumerator LoginAndSchedule()
        {
            NotifyLoading(true);
            CoroutineWithResult cd = new CoroutineWithResult(this, Login());
            yield return cd.Coroutine;

            Tuple<string, string> result = (Tuple<string, string>)cd.Result;

            if (result.Item1 != null)
            {
                NotifyLoading(false);
                NotifyManagers("Error");
                JSONNode body = JSON.Parse(result.Item1);
                if (body == null)
                {
                    NotifyManagers("Unable to connect");
                }
                else
                {
                    NotifyManagers(body["message"].Value);
                }
            }
            else
            {
                NotifyManagers("Login success");
                JSONNode body = JSON.Parse(result.Item2);
                this.jwt = body["token"];
                this.user = Token;

                cd = new CoroutineWithResult(this, GetSchedule());
                yield return cd.Coroutine;

                result = (Tuple<string, string>)cd.Result;

                if (result.Item1 != null)
                {
                    NotifyLoading(false);
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
            CoroutineWithResult cd = new CoroutineWithResult(this, Login(Token, Token));
            yield return cd.Coroutine;
            yield return cd.Result;
        }

        public IEnumerator Login(string username, string password)
        {
            using (var request = PostJSON(this.URL + "/users/login", new JSONClass
                {
                    { "username", username },
                    { "password", password }
                }))
            {
                yield return ProcessWWW(request);
            }
        }

        public IEnumerator GetSchedule()
        {
            using (var request = UnityWebRequest.Get(this.URL + "/studies/" + Study + "/schedule"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + this.jwt);
                yield return ProcessWWW(request);
            }
        }

        public IEnumerator GetTarget(string activityId)
        {
            using (var request = UnityWebRequest.Get(this.URL + "/activities/" + activityId + "/target"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + this.jwt);
                yield return ProcessWWW(request);
            }
        }

        public IEnumerator GetCompletion(string activityId)
        {
            using (var request = UnityWebRequest.Get(this.URL + "/activities/" + activityId + "/completion"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + this.jwt);
                yield return ProcessWWW(request);
            }
        }

        public IEnumerator SetResults(string activityId, string results, bool tofile)
        {
            using (var request = PostJSON(this.URL + "/activities/" + activityId + "/result", new JSONClass
                {
                    { "result", results },
                    { "tofile", new JSONData(tofile) }
                }))
            {
                request.SetRequestHeader("Authorization", "Bearer " + this.jwt);
                yield return ProcessWWW(request);
            }
        }

        public IEnumerator SetCompletion(string activityId, bool completion)
        {
            using (var request = PostJSON(this.URL + "/activities/" + activityId + "/completion", new JSONClass
                {
                    { "status", new JSONData(completion) }
                }))
            {
                request.SetRequestHeader("Authorization", "Bearer " + this.jwt);
                yield return ProcessWWW(request);
            }
        }

        private static UnityWebRequest PostJSON(string url, JSONClass json)
        {

            UnityWebRequest www = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(EncodeJSONBody(json)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            www.SetRequestHeader("Content-Type", "application/json");
            return www;
        }

        private static byte[] EncodeJSONBody(JSONClass json)
        {
            return System.Text.Encoding.UTF8.GetBytes(json.ToString());
        }


        private IEnumerator ProcessWWW(UnityWebRequest www)
        {
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

        internal class CoroutineWithResult
        {
            public Coroutine Coroutine { get; private set; }
            public object Result { get; private set; }
            private readonly IEnumerator target;
            public CoroutineWithResult(MonoBehaviour owner, IEnumerator target)
            {
                this.target = target;
                this.Coroutine = owner.StartCoroutine(Run());
            }

            private IEnumerator Run()
            {
                while (target.MoveNext())
                {
                    Result = target.Current;
                    yield return Result;
                }
            }
        }
    }
}
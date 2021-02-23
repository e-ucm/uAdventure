
using Newtonsoft.Json;
using Simva;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityFx.Async;

public class Cmi5Helper
{
    public interface ICmi5LaunchConfig
    {
        string Endpoint { get; }
        string Fetch { get; }
        string Actor { get; }
        string Registration { get; }
        string ActivityId { get; }
    }

    public class Cmi5LaunchConfig : ICmi5LaunchConfig
    {
        public string Endpoint { get; set; }
        public string Fetch { get; set; }
        public string Actor { get; set; }
        public string Registration { get; set; }
        public string ActivityId { get; set; }
    }

    [System.Serializable]
    public class Fetch
    {
        [JsonProperty("auth-token")]
        public string AuthToken { get; set; }

        [JsonProperty("error-code")]
        public string ErrorCode { get; set; }

        [JsonProperty("error-text")]
        public string ErrorText { get; set; }
    }

    [System.Serializable]
    public class Context
    {
        [JsonProperty("registration")]
        public string Registration { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("masteryScore")]
        public string MasteryScore { get; set; }

        [JsonProperty("launchMode")]
        public string LaunchMode { get; set; }

        [JsonProperty("launchURL")]
        public string LaunchURL { get; set; }

        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("moveOn")]
        public string MoveOn { get; set; }

        [JsonProperty("launchParameters")]
        public string LaunchParameters { get; set; }
    }

    [System.Serializable]
    public class AgentProfile
    {
        [JsonProperty("languagePreference")]
        public string LanguagePreference { get; set; }

        [JsonProperty("audioPreference")]
        public string AudioPreference { get; set; }
    }

    public ICmi5LaunchConfig GetConfig()
    {
        return new Cmi5LaunchConfig
        {
            Endpoint = GetParam("endpoint"),
            Fetch = GetParam("fetch"),
            Actor = GetParam("actor"),
            Registration = GetParam("registration"),
            ActivityId = GetParam("activityId")
        };
    }

    public static IAsyncOperation<string> FetchAuthToken()
    {
        return DoRequest(UnityWebRequest.Get(GetFetch()));
    }


    public static string GetEndpoint()
    {
        return GetParam("endpoint");
    }

    public static string GetFetch()
    {
        return GetParam("fetch");
    }

    public static string GetActor()
    {
        return GetParam("actor");
    }

    public static string GetRegistration()
    {
        return GetParam("registration");
    }

    public static string GetActivityId()
    {
        return GetParam("activityId");
    }

    private static string GetParam(string name)
    {
        return null;
    }



    private static IAsyncOperation<string> DoRequest(UnityWebRequest webRequest)
    {
        var result = new AsyncCompletionSource<string>();
        var asyncOP = webRequest.SendWebRequest();
        asyncOP.completed += a =>
        {
            if (webRequest.isNetworkError)
            {
                result.SetException(new ApiException((int)webRequest.responseCode, webRequest.error, webRequest.downloadHandler.text));
            }
            else if (webRequest.isHttpError)
            {
                result.SetException(new ApiException((int)webRequest.responseCode, webRequest.error, webRequest.downloadHandler.text));
            }
            else
            {
                result.SetResult(webRequest.downloadHandler.text);
            }

            webRequest.Dispose();
        };

        return result;
    }
}
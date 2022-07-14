
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

    public static ICmi5LaunchConfig GetConfig()
    {
        return uAdventure.Runner.Cmi5Launcher.config;
    }

    public static IAsyncOperation<Fetch> FetchAuthToken()
    {
        return Simva.RequestsUtil.DoRequest<Fetch>(UnityWebRequest.Post(GetConfig().Fetch, ""));
    }
}
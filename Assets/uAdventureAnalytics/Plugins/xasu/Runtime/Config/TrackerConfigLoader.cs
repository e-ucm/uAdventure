using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Xasu.Requests;

namespace Xasu.Config
{
    public static class TrackerConfigLoader
    {
        private const string TrackerConfigFileName = "tracker_config.json";

        public static async Task<TrackerConfig> LoadLocalAsync()
        {
            return await LoadAsync(TrackerConfigFileName);
        }

        public static async Task<TrackerConfig> LoadAsync(string fileName)
        {
            Debug.Log("[TRACKER CONFIG] Loading...");
            string contents = await ReadFileFromStreamingAssets(fileName);
            return ParseContents(contents);
        }

        private static TrackerConfig ParseContents(string contents)
        {
            if (!string.IsNullOrEmpty(contents))
            {
                Debug.Log("[TRACKER CONFIG] tracker_config.json content: " + contents);
                return JsonConvert.DeserializeObject<TrackerConfig>(contents);
            }
            return null;
        }

        private static async Task<string> ReadFileFromStreamingAssets(string fileName)
        {
            UnityWebRequest reader = null;
            var filePath = String.Empty;
            string contents = null;

            // Platform dependent StreamingAssets Load https://docs.unity3d.com/Manual/StreamingAssets.html
            switch (Application.platform)
            {
                case RuntimePlatform.WebGLPlayer:
                    reader = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileName);
                    break;
                case RuntimePlatform.Android:
                    reader = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/" + TrackerConfigFileName);
                    break;
#if !UNITY_WEBGL || UNITY_EDITOR
                case RuntimePlatform.IPhonePlayer:
                    filePath = System.IO.Path.Combine(Application.dataPath + "/Raw", TrackerConfigFileName);
                    break;
                default:
                    filePath = System.IO.Path.Combine(Application.streamingAssetsPath + "/", TrackerConfigFileName);
                    break;
#endif
            }

            if (reader != null)
            {
                Debug.Log("[TRACKER CONFIG] Requesting tracker_config.json from url: " + reader.uri);
                await RequestsUtility.DoRequest(reader);
                contents = reader.downloadHandler.text;
            }
            else
            {
                Debug.Log("[TRACKER CONFIG] Loading tracker_config.json from file: " + filePath);
#if !UNITY_WEBGL || UNITY_EDITOR
                if (!System.IO.File.Exists(filePath))
                {
                    throw new System.IO.FileNotFoundException(filePath);
                }
                contents = System.IO.File.ReadAllText(filePath);
#endif
            }

            return contents;
        }

        /*public void Save()
        {
            var path = GetFilePath();
            if (Application.isEditor && !Application.isPlaying)
            {
                var simvaconf = new JObject
                {
                    ["lrs_endpoint"] = LRSEndpoint,
                    ["auth_protocol"] = AuthProtocol,
                    ["auth_parameters"] = AuthParameters,
                    ["trace_storage"] = TraceStorage.ToString(),
                    ["backup"] = Backup.ToString(),
                    ["realtime"] = Realtime.ToString()
                };
                System.IO.File.WriteAllText(path, simvaconf.ToString(Newtonsoft.Json.Formatting.Indented));
            }
        }*/
    }
}

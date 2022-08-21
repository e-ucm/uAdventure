﻿using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Simva
{
    public class SimvaConf
    {
        private static SimvaConf local;

        public static SimvaConf Local
        {
            get
            {
                return local;
            }
            set
            {
                local = value;
            }
        }

        public string Host { get; set; }

        public string Port { get; set; }

        public string Protocol { get; set; }

        public string SSO { get; private set; }

        public string ClientId { get; set; }

        public string Study { get; set; }

        public string URL
        {
            get
            {
                return Protocol + "://" + Host + ":" + Port;
            }
        }

        public bool TraceStorage { get; set; }
        public bool Backup { get; set; }
        public bool Realtime { get; set; }

        public SimvaConf()
        {
            Debug.Log("[SIMVA CONF] Loading...");
            Host = "localhost";
            Port = "443";
            Protocol = "https";
        }

        public IEnumerator LoadAsync()
        {
            string contents = "";

            // WebGL and Android have to use WWW to load from streaming assets
#if UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_ANDROID 
            Debug.Log("[SIMVA CONF] Doing WebGL / Android read...");
            UnityWebRequest reader = GetReader();
            yield return reader.SendWebRequest();
            if (string.IsNullOrEmpty(reader.error))
            {
                Debug.Log("[SIMVA CONF] Request failed: (" + reader.responseCode + " ) " + reader.error + " - " + reader.downloadHandler.data);
                contents = System.Text.Encoding.UTF8.GetString(reader.downloadHandler.data);
            }
#else       // The others can read from System.IO       
            string filePath = GetFilePath();     
            if (System.IO.File.Exists(filePath))
            {
                contents = System.IO.File.ReadAllText(filePath);
            }
            yield return null;
#endif
            ParseContents(contents);
        }

        private void ParseContents(string contents)
        {
            if (!string.IsNullOrEmpty(contents))
            {
                Debug.Log("[SIMVA CONF] Simva.conf content: " + contents);
                var simvaconf = JObject.Parse(contents);
                Study = simvaconf.Value<string>("study");
                Host = simvaconf.Value<string>("host");
                Protocol = simvaconf.Value<string>("protocol");
                Port = simvaconf.Value<string>("port");
                SSO = simvaconf.Value<string>("sso");
                ClientId = simvaconf.Value<string>("client_id");
                TraceStorage = simvaconf.Value<bool>("trace_storage");
                Backup = simvaconf.Value<bool>("backup");
                Realtime = simvaconf.Value<bool>("realtime");
            }
        }

        private static UnityWebRequest GetReader()
        {
#if UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_ANDROID
            var FileName = "simva.conf";
#endif
            UnityWebRequest reader = null;
            // Platform dependent StreamingAssets Load https://docs.unity3d.com/Manual/StreamingAssets.html
#if UNITY_WEBPLAYER || UNITY_WEBGL
            reader = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + FileName);
#elif UNITY_ANDROID
            reader = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/" + FileName);
#endif
            Debug.Log("[SIMVA CONF] Requesting simva.conf from: " + reader.uri);
            return reader;
        }

        private static string GetFilePath()
        {
            var FileName = "simva.conf";

            // Platform dependent StreamingAssets Load https://docs.unity3d.com/Manual/StreamingAssets.html
#if UNITY_IOS
            var filePath = System.IO.Path.Combine(Application.dataPath + "/Raw", FileName);
#else
            var filePath = System.IO.Path.Combine(Application.streamingAssetsPath + "/", FileName);
#endif
            return filePath;
        }

        public void Save()
        {
            var path = GetFilePath();
            if (Application.isEditor && !Application.isPlaying)
            {
                var simvaconf = new JObject
                {
                    ["study"] = Study,
                    ["host"] = Host,
                    ["protocol"] = Protocol,
                    ["port"] = Port,
                    ["sso"] = SSO,
                    ["client_id"] = ClientId,
                    ["url"] = URL,
                    ["trace_storage"] = TraceStorage.ToString(),
                    ["backup"] = Backup.ToString(),
                    ["realtime"] = Realtime.ToString()
                };
                System.IO.File.WriteAllText(path, simvaconf.ToString(Newtonsoft.Json.Formatting.Indented));
            }
        }
    }
}

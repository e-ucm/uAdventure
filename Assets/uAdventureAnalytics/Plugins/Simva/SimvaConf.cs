using SimpleJSON;
using UnityEngine;

namespace Simva
{
    public class SimvaConf
    {
        private static SimvaConf local;

        public static SimvaConf Local
        {
            get
            {
                if (local == null)
                {
                    local = new SimvaConf();
                }

                return local;
            }
        }

        public string Host { get; set; }

        public string Port { get; set; }

        public string Protocol { get; set; }

        public string SSO { get; private set; }

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
            Host = "localhost";
            Port = "443";
            Protocol = "https";

            string contents = "";

            // WebGL and Android have to use WWW to load from streaming assets
#if UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_ANDROID 
            WWW reader = GetReader();
            while (!reader.isDone) { }
            if (string.IsNullOrEmpty(reader.error))
            {
                contents = System.Text.Encoding.UTF8.GetString(reader.bytes);
            }
#else       // The others can read from System.IO       
            string filePath = GetFilePath();     
            if (System.IO.File.Exists(filePath))
            {
                contents = System.IO.File.ReadAllText(filePath);
            }
#endif
            if (!string.IsNullOrEmpty(contents))
            {
                var simvaconf = SimpleJSON.JSON.Parse(contents);
                Study = simvaconf["study"];
                Host = simvaconf["host"];
                Protocol = simvaconf["protocol"];
                Port = simvaconf["port"];
                SSO = simvaconf["sso"];
                TraceStorage = simvaconf["trace_storage"].AsBool;
                Backup = simvaconf["backup"].AsBool;
                Realtime = simvaconf["realtime"].AsBool;
            }
        }
        private static WWW GetReader()
        {
            var FileName = "simva.conf";
            WWW reader = null;
            // Platform dependent StreamingAssets Load https://docs.unity3d.com/Manual/StreamingAssets.html
#if UNITY_WEBPLAYER || UNITY_WEBGL
            reader = new WWW(Application.streamingAssetsPath + "/" + FileName);
#elif UNITY_ANDROID
            reader = new WWW("jar: file://" + Application.dataPath + "!/assets/" + FileName);
#endif
            return reader;
        }

        private static string GetFilePath()
        {
            var FileName = "simva.conf";

            // Platform dependent StreamingAssets Load https://docs.unity3d.com/Manual/StreamingAssets.html
#if UNITY_IOS
            var filePath = Path.Combine(Application.dataPath + "/Raw", FileName);
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
                var simvaconf = new SimpleJSON.JSONClass();
                simvaconf["study"] = Study;
                simvaconf["host"] = Host;
                simvaconf["protocol"] = Protocol;
                simvaconf["port"] = Port;
                simvaconf["sso"] = SSO;
                simvaconf["url"] = URL;
                simvaconf["trace_storage"] = TraceStorage.ToString();
                simvaconf["backup"] = Backup.ToString();
                simvaconf["realtime"] = Realtime.ToString();
                System.IO.File.WriteAllText(path, simvaconf.ToJSON(4));
            }
        }
    }
}

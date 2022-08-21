using Newtonsoft.Json.Linq;
using Simva;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace uAdventure.Simva
{
    public static class SimvaConfWriter
    {
        public static void Write(this SimvaConf conf)
        {
            JObject toWrite = new JObject
            {
                ["study"] = conf.Study,
                ["host"] = conf.Host,
                ["protocol"] = conf.Protocol,
                ["port"] = conf.Port,
                ["url"] = conf.URL,
                ["trace_storage"] = conf.TraceStorage.ToString(),
                ["backup"] = conf.Backup.ToString(),
                ["realtime"] = conf.Realtime.ToString()
            };

            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/simva.conf", toWrite.ToString());
        }
    }
}

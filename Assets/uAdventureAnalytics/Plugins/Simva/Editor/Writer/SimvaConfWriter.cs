using SimpleJSON;
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
            JSONClass toWrite = new JSONClass();

            toWrite["study"] = conf.Study;
            toWrite["host"] = conf.Host;
            toWrite["protocol"] = conf.Protocol;
            toWrite["port"] = conf.Port;
            toWrite["url"] = conf.URL;
            toWrite["trace_storage"] = conf.TraceStorage.ToString();
            toWrite["backup"] = conf.Backup.ToString();
            toWrite["realtime"] = conf.Realtime.ToString();

            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/simva.conf", toWrite.ToJSON(0));
        }
    }
}

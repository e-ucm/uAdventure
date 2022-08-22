using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xasu.Config
{
    public class TrackerConfig
    {

        // Main Tracker Settings
        [JsonProperty("strict_mode")]
        public bool StrictMode { get; set; }
        [JsonProperty("flush_interval")]
        public float FlushInterval { get; set; }
        [JsonProperty("simva")]
        public bool Simva { get; set; }

        // LRS Settings (Online)
        [JsonProperty("online")]
        public bool Online { get; set; }
        [JsonProperty("batch_size")]
        public int BatchSize { get; set; }
        [JsonProperty("lrs_endpoint")]
        public string LRSEndpoint { get; set; }
        [JsonProperty("fallback")]
        public bool Fallback { get; set; }

        // Auth Settings
        [JsonProperty("auth_protocol")]
        public string AuthProtocol { get; set; }
        [JsonProperty("auth_parameters")]
        public IDictionary<string, string> AuthParameters { get; set; }

        // Local Settings
        [JsonProperty("offline")]
        public bool Offline { get; set; }
        [JsonProperty("trace_format")]
        public TraceFormats TraceFormat { get; set; }
        [JsonProperty("file_name")]
        public string FileName { get; set; } = "traces.log";


        // Backup Settings
        [JsonProperty("backup")]
        public bool Backup { get; set; }
        [JsonProperty("backup_file_name")]
        public string BackupFileName { get; set; } = "backup.log";
        [JsonProperty("backup_trace_format")]
        public TraceFormats BackupTraceFormat { get; set; }
        [JsonProperty("backup_endpoint")]
        public string BackupEndpoint { get; set; }
        [JsonProperty("backup_request_config")]
        public JObject BackupRequestConfig { get; set; }
        // Auth Settings
        [JsonProperty("backup_auth_protocol")]
        public string BackupAuthProtocol { get; set; }
        [JsonProperty("backup_auth_parameters")]
        public IDictionary<string, string> BackupAuthParameters { get; set; }

        public TrackerConfig()
        {
            LRSEndpoint = "https://localhost:443/";
        }

    }
}

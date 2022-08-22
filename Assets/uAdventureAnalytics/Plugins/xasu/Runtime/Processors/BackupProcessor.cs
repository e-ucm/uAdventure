using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinCan;
using UnityEngine;
using Xasu.Auth.Protocols;
using Xasu.Config;
using Xasu.Exceptions;
using Xasu.Requests;

namespace Xasu.Processors
{
    // Oriented to save data in Application.temporaryCachePath and then upload it
    public class BackupProcessor : LocalProcessor
    {
        private readonly string backupEndpoint;
        private readonly JObject backupRequestParameters;
        private IAuthProtocol authorization;
        private IAsyncPolicy policy;

        public BackupProcessor(string backupFileName, TraceFormats traceFormat, string backupEndpoint,
            JObject backupRequestParameters, IAuthProtocol authorization, IAsyncPolicy policy)
            : this(backupFileName, traceFormat, TCAPIVersion.V103, backupEndpoint, backupRequestParameters, authorization, policy)
        {
        }

        public BackupProcessor(string backupFileName, TraceFormats traceFormat, TCAPIVersion version, string backupEndpoint, 
            JObject backupRequestParameters, IAuthProtocol authorization, IAsyncPolicy policy) : base(backupFileName, traceFormat, version, true)
        {
            this.backupEndpoint = backupEndpoint;
            this.backupRequestParameters = backupRequestParameters;
            this.authorization = authorization;
            this.policy = policy;
        }

        public override async Task Finalize(IProgress<float> progress)
        {
            await base.Finalize(progress);

            if (!string.IsNullOrEmpty(backupEndpoint))
            {
                var backupContents = System.IO.File.ReadAllText(file);
                var body = new Dictionary<string, object>
                {
                    { "tofile", true },
                    { "result", backupContents }
                };
                var myRequest = new MyHttpRequest
                {
                    url = backupEndpoint,
                    method = "POST",
                    policy = policy,
                    authorization = authorization,
                    content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }))
                };

                switch (traceFormat)
                {
                    case TraceFormats.XAPI: myRequest.contentType = "application/json"; break;
                    case TraceFormats.CSV:  myRequest.contentType = "text/csv";         break;
                }

                // Add custom parameters from config file
                if (backupRequestParameters != null)
                {
                    // Content type
                    if (backupRequestParameters.ContainsKey("content_type"))
                    {
                        myRequest.contentType = backupRequestParameters["content_type"].Value<string>();
                    }

                    // Request headers
                    if (backupRequestParameters.ContainsKey("headers") && backupRequestParameters["headers"].Type == JTokenType.Object)
                    {
                        foreach(var kv in backupRequestParameters["headers"].ToObject<Dictionary<string, object>>())
                        {
                            myRequest.headers.Add(kv.Key, kv.Value.ToString());
                        }
                    }

                    // Request query parameters
                    if (backupRequestParameters.ContainsKey("query_parameters") && backupRequestParameters["query_parameters"].Type == JTokenType.Object)
                    {
                        foreach (var kv in backupRequestParameters["query_parameters"].ToObject<Dictionary<string, object>>())
                        {
                            myRequest.queryParams.Add(kv.Key, kv.Value.ToString());
                        }
                    }
                }

                var myResponse = await RequestsUtility.DoRequest(myRequest, progress);

                // Response.success == false
                if (myResponse.ex is APIException apiEx)
                {
                    switch ((HttpStatusCode)apiEx.HttpCode)
                    {
                        // Authorization issues are forwarded to the auth protocol
                        case HttpStatusCode.Unauthorized:
                            State = ProcessorState.Errored;
                            ErrorMessage = "Unauthorized: " + apiEx.Message;
                            authorization?.Unauthorized(apiEx);
                            break;
                        case HttpStatusCode.Forbidden:
                            State = ProcessorState.Errored;
                            ErrorMessage = "Forbidden: " + apiEx.Message;
                            authorization?.Forbidden(apiEx);
                            break;
                        default:
                            var text = Encoding.UTF8.GetString(myResponse.content);
                            State = ProcessorState.Errored;
                            XasuTracker.Instance.LogError(string.Format("[TRACKER: Backup Processor] Backup upload returned status {0} with message: {1}", myResponse.status, text));
                            break;
                    }

                    throw apiEx;
                }
            }
            else if (progress != null)
            {
                progress.Report(1);
            }
        }
    }
}

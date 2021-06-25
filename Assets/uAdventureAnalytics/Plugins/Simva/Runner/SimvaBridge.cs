using AssetPackage;
using System;
using uAdventure.Simva;
using UnityFx.Async.Promises;

namespace Simva
{
    public class SimvaBridge : IBridge, IDataStorage, IAppend, IWebServiceRequest, ILog
    {
        private ApiClient apiClient;
        private UnityBridge unityBridge;

        public SimvaBridge(ApiClient apiClient)
        {
            unityBridge = new UnityBridge();
            this.apiClient = apiClient;
        }

        public void Append(string fileId, string fileData)
        {
            unityBridge.Append(fileId, fileData);
        }

        public bool Delete(string fileId)
        {
            return unityBridge.Delete(fileId);
        }

        public bool Exists(string fileId)
        {
            return unityBridge.Exists(fileId);
        }

        public string[] Files()
        {
            return unityBridge.Files();
        }

        public string Load(string fileId)
        {
            return unityBridge.Load(fileId);
        }

        public void Log(Severity severity, string msg)
        {
            if(severity == Severity.Error || severity == Severity.Critical)
            {
                SimvaExtension.Instance.NotifyManagers("Tracker Error: " + msg);
            }

            unityBridge.Log(severity, msg);
        }

        public void Save(string fileId, string fileData)
        {
            unityBridge.Save(fileId, fileData);
        }

		public void WebServiceRequestAsync(RequestSetttings requestSettings, Action<RequestResponse> callback)
		{
            // Force Update auth params
            apiClient.UpdateParamsForAuth(null, requestSettings.requestHeaders, new string[] { "OAuth2" }, true)
                .Then(() =>
                {
                    unityBridge.WebServiceRequestAsync(requestSettings, callback);
                })
                .Catch(ex =>
                {
                    RequestResponse response = new RequestResponse
                    {
                        body = ex.Message,
                        responseCode = ex is ApiException apiEx ? apiEx.ErrorCode : 401
                    };
                });
		}

		public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
        {
            // Force Update auth params
            apiClient.UpdateParamsForAuth(null, requestSettings.requestHeaders, new string[] { "OAuth2" }, false);
            unityBridge.WebServiceRequest(requestSettings, out requestResponse);
        }
	}
}

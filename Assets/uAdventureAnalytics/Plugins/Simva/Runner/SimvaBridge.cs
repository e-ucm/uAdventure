using AssetPackage;
using Simva;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAdventure.Simva
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
            unityBridge.Log(severity, msg);
        }

        public void Save(string fileId, string fileData)
        {
            unityBridge.Save(fileId, fileData);
        }

        public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
        {
            // Force Update auth params
            apiClient.UpdateParamsForAuth(null, requestSettings.requestHeaders, new string[] { "OAuth2" });
            unityBridge.WebServiceRequest(requestSettings, out requestResponse);
        }
    }
}

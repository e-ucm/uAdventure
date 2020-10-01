using System;
using System.Linq;
using System.Collections.Generic;

using UnityFx.Async;
using UnityFx.Async.Promises;
using UnityEngine.Networking;

using Simva;
using Simva.Model;

namespace Simva.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IDefaultApi
    {
        /// <summary>
        /// adds a user. Creates a new group for the current user as owner. 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        IAsyncOperation AddUser (User body);
        /// <summary>
        /// logs in the user obtaining an auth token Creates a new group for the current user as owner. 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        IAsyncOperation LoginUser (LoginBody body);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class DefaultApi : IDefaultApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public DefaultApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApi"/> class.
        /// </summary>
        /// <returns></returns>
        public DefaultApi(String basePath)
        {
            this.ApiClient = new ApiClient(basePath);
        }
    
        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public void SetBasePath(String basePath)
        {
            this.ApiClient.BasePath = basePath;
        }
    
        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public String GetBasePath(String basePath)
        {
            return this.ApiClient.BasePath;
        }
    
        /// <summary>
        /// Gets or sets the API client.
        /// </summary>
        /// <value>An instance of the ApiClient</value>
        public ApiClient ApiClient {get; set;}
    
        /// <summary>
        /// adds a user. Creates a new group for the current user as owner. 
        /// </summary>
        /// <param name="body"></param> 
        /// <returns></returns>            
        public IAsyncOperation AddUser (User body)
        {
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling AddUser");
            
    
            var path = "/users";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling AddUser: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// logs in the user obtaining an auth token Creates a new group for the current user as owner. 
        /// </summary>
        /// <param name="body"></param> 
        /// <returns></returns>            
        public IAsyncOperation LoginUser (LoginBody body)
        {
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling LoginUser");
            
    
            var path = "/users/login";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling LoginUser: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
    }
}

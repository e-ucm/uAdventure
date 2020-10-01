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
    public interface IAdminsApi
    {
        /// <summary>
        /// gets the list of groups where this student pl Obtains the list of groups owned by current user. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param>
        /// <param name="skip">number of records to skip for pagination</param>
        /// <param name="limit">maximum number of records to return</param>
        /// <returns>List&lt;Group&gt;</returns>
        IAsyncOperation<List<Group>> GetGroups (string searchString, int? skip, int? limit);
        /// <summary>
        /// Obtains your own user. Based on the current auth header obtains the user 
        /// </summary>
        /// <returns>User</returns>
        IAsyncOperation<User> GetMe ();
        /// <summary>
        /// gets the list of users. Obtains the list of groups owned by current user. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param>
        /// <param name="skip">number of records to skip for pagination</param>
        /// <param name="limit">maximum number of records to return</param>
        /// <returns>List&lt;User&gt;</returns>
        IAsyncOperation<List<User>> GetUsers (string searchString, int? skip, int? limit);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class AdminsApi : IAdminsApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminsApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public AdminsApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AdminsApi(String basePath)
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
        /// gets the list of groups where this student pl Obtains the list of groups owned by current user. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param> 
        /// <param name="skip">number of records to skip for pagination</param> 
        /// <param name="limit">maximum number of records to return</param> 
        /// <returns>List&lt;Group&gt;</returns>            
        public IAsyncOperation<List<Group>> GetGroups (string searchString, int? skip, int? limit)
        {
            
    
            var path = "/groups";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
             if (searchString != null) queryParams.Add("searchString", ApiClient.ParameterToString(searchString)); // query parameter
 if (skip != null) queryParams.Add("skip", ApiClient.ParameterToString(skip)); // query parameter
 if (limit != null) queryParams.Add("limit", ApiClient.ParameterToString(limit)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<Group>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Group>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Group>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetGroups: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// Obtains your own user. Based on the current auth header obtains the user 
        /// </summary>
        /// <returns>User</returns>            
        public IAsyncOperation<User> GetMe ()
        {
            
    
            var path = "/users/me";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<User>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (User)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(User), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetMe: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the list of users. Obtains the list of groups owned by current user. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param> 
        /// <param name="skip">number of records to skip for pagination</param> 
        /// <param name="limit">maximum number of records to return</param> 
        /// <returns>List&lt;User&gt;</returns>            
        public IAsyncOperation<List<User>> GetUsers (string searchString, int? skip, int? limit)
        {
            
    
            var path = "/users";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
             if (searchString != null) queryParams.Add("searchString", ApiClient.ParameterToString(searchString)); // query parameter
 if (skip != null) queryParams.Add("skip", ApiClient.ParameterToString(skip)); // query parameter
 if (limit != null) queryParams.Add("limit", ApiClient.ParameterToString(limit)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<User>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<User>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<User>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetUsers: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
    }
}

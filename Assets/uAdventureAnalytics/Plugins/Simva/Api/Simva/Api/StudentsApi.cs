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
    public interface IStudentsApi
    {
        /// <summary>
        /// returns if the activity can be opened or not If the activity can be opened, e.g. is hosted in a web, this endpoint will return the target URI 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <returns></returns>
        IAsyncOperation<Dictionary<string, string>> GetActivityTarget (string id);
        /// <summary>
        /// gets the completion status of the activity Obtains the completion status of the activity 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation<Dictionary<string, bool>> GetCompletion (string id, string users);
        /// <summary>
        /// gets the group with the given ID Obtains the requested group 
        /// </summary>
        /// <param name="id">The group ID</param>
        /// <returns>Group</returns>
        IAsyncOperation<Group> GetGroup (string id);
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
        /// returns if the activity can be opened or not If the activity can be opened, e.g. is hosted in a web, the activity will be openable as can be opened. 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <returns>string</returns>
        IAsyncOperation<string> GetOpenable (string id);
        /// <summary>
        /// gets true if has results of the activity Obtains true or false depending on if the user have results or not for the activity. 
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation GetResult (string id, string users);
        /// <summary>
        /// gets the results of the activity Obtains the completion status of the activity 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation GetResult_1 (string id, string users);
        /// <summary>
        /// gets the list of scheduled activities for the student Obtains the list of scheduled activities for the current student and study, and its completion status. Hides the current used test to the user. 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns>InlineResponse200</returns>
        IAsyncOperation<Schedule> GetSchedule (string id);
        /// <summary>
        /// redirects the user to the activity landing If the activity is openable, redirects the user to the activity landing. 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <returns></returns>
        IAsyncOperation OpenActivity (string id);
        /// <summary>
        /// sets the completion status of the activity Set the completion status of the activity for a student 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <param name="user">the user to get its completion status (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation SetCompletion (string id, string user, bool status);
        /// <summary>
        /// sets the result for the activity Set the completion status of the activity for a student 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <param name="user">the user to set its result (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation SetResult (string id, string user, object body);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class StudentsApi : IStudentsApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentsApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public StudentsApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public StudentsApi(String basePath)
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
        /// returns if the activity can be opened or not If the activity can be opened, e.g. is hosted in a web, this endpoint will return the target URI 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <returns></returns>            
        public IAsyncOperation<Dictionary<string, string>> GetActivityTarget (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetActivityTarget");
            
    
            var path = "/activities/{id}/target";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Dictionary<string, string>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Dictionary<string, string>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Dictionary<string, string>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetActivityTarget: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the completion status of the activity Obtains the completion status of the activity 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param> 
        /// <returns></returns>            
        public IAsyncOperation<Dictionary<string, bool>> GetCompletion (string id, string users)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetCompletion");
            
    
            var path = "/activities/{id}/completion";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
             if (users != null) queryParams.Add("users", ApiClient.ParameterToString(users)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Dictionary<string, bool>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Dictionary<string, bool>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Dictionary<string, bool>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetCompletion: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the group with the given ID Obtains the requested group 
        /// </summary>
        /// <param name="id">The group ID</param> 
        /// <returns>Group</returns>            
        public IAsyncOperation<Group> GetGroup (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetGroup");
            
    
            var path = "/groups/{id}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Group>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Group)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Group), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetGroup: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
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
        /// returns if the activity can be opened or not If the activity can be opened, e.g. is hosted in a web, the activity will be openable as can be opened. 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <returns>string</returns>            
        public IAsyncOperation<string> GetOpenable (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetOpenable");
            
    
            var path = "/activities/{id}/openable";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<string>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (string)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(string), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetOpenable: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets true if has results of the activity Obtains true or false depending on if the user have results or not for the activity. 
        /// </summary>
        /// <param name="id">The activity ID</param> 
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param> 
        /// <returns></returns>            
        public IAsyncOperation GetResult (string id, string users)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetResult");
            
    
            var path = "/activities/{id}/hasresult";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
             if (users != null) queryParams.Add("users", ApiClient.ParameterToString(users)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetResult: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the results of the activity Obtains the completion status of the activity 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param> 
        /// <returns></returns>            
        public IAsyncOperation GetResult_1 (string id, string users)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetResult_1");
            
    
            var path = "/activities/{id}/result";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
             if (users != null) queryParams.Add("users", ApiClient.ParameterToString(users)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetResult_1: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the list of scheduled activities for the student Obtains the list of scheduled activities for the current student and study, and its completion status. Hides the current used test to the user. 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns>InlineResponse200</returns>            
        public IAsyncOperation<Schedule> GetSchedule (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetSchedule");
            
    
            var path = "/studies/{id}/schedule";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Schedule>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Schedule)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Schedule), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetSchedule: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// redirects the user to the activity landing If the activity is openable, redirects the user to the activity landing. 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <returns></returns>            
        public IAsyncOperation OpenActivity (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling OpenActivity");
            
    
            var path = "/activities/{id}/open";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling OpenActivity: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }

        /// <summary>
        /// sets the completion status of the activity Set the completion status of the activity for a student 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <param name="user">the user to get its completion status (if logged in student , user is not needed) </param> 
        /// <returns></returns>            
        public IAsyncOperation SetCompletion(string id, string user, bool status)
        {

            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling SetCompletion");


            var path = "/activities/{id}/completion";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = ApiClient.Serialize(new Dictionary<string, bool>{
                { "status", status}
            });
    
             if (user != null) queryParams.Add("user", ApiClient.ParameterToString(user)); // query parameter
                                        
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
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling SetCompletion: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// sets the result for the activity Set the completion status of the activity for a student 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <param name="user">the user to set its result (if logged in student , user is not needed) </param> 
        /// <returns></returns>            
        public IAsyncOperation SetResult(string id, string user, object body)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling SetResult");
            
    
            var path = "/activities/{id}/result";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = ApiClient.Serialize(body);
    
             if (user != null) queryParams.Add("user", ApiClient.ParameterToString(user)); // query parameter
                                        
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            var callApi = ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings, true);

            callApi.AddProgressCallback(p =>
                {
                    if (!result.IsCompleted && !result.IsCanceled)
                    {
                        result.SetProgress(p);
                    }
                });
            callApi.Then(webRequest =>
                {
                    result.SetCompleted();
                })
                .Catch(error =>
                {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling SetResult: " + apiEx.Message, apiEx.ErrorContent));
                })
                ;
    
            return result;
        }
    }
}

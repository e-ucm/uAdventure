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
    public interface ITeachersApi
    {
        /// <summary>
        /// adds a Activity Creates a new activity for the current teacher. 
        /// </summary>
        /// <param name="body">Activity to be created</param>
        /// <returns>Activity</returns>
        IAsyncOperation<Activity> AddActivity (Activity body);
        /// <summary>
        /// adds a Activity to a test Creates a new activity for the current teacher. 
        /// </summary>
        /// <param name="studyid">The Study ID</param>
        /// <param name="testid">The test ID</param>
        /// <param name="body"></param>
        /// <returns>Activity</returns>
        IAsyncOperation<Activity> AddActivityToTest (string studyid, string testid, Activity body);
        /// <summary>
        /// adds a group for the current user as owner. Creates a new group for the current user as owner. 
        /// </summary>
        /// <param name="body"></param>
        /// <returns>Group</returns>
        IAsyncOperation<Group> AddGroup (AddGroupBody body);
        /// <summary>
        /// adds an study for the current teacher Creates a new study for the current teacher. 
        /// </summary>
        /// <param name="body">Study to be created</param>
        /// <returns>Study</returns>
        IAsyncOperation<Study> AddStudy (AddStudyBody body);
        /// <summary>
        /// adds a test to the study Adds a test for the current group 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <param name="body"></param>
        /// <returns>Test</returns>
        IAsyncOperation<Test> AddTestToStudy (string id, Test body);
        /// <summary>
        /// deletes the activity with the given ID Deleted the specified activity 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <returns></returns>
        IAsyncOperation DeleteActivity (string id);
        /// <summary>
        /// Deletes the study with the given ID Deleted the designed study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns></returns>
        IAsyncOperation DeleteStudy (string id);
        /// <summary>
        /// deletes the test with the given ID Deleted the selected test 
        /// </summary>
        /// <param name="studyid">The Study ID</param>
        /// <param name="testid">The test ID</param>
        /// <returns></returns>
        IAsyncOperation DeleteTest (string studyid, string testid);
        /// <summary>
        /// gets the list of activities for the current teacher Obtains the list of activities for the current teacher. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param>
        /// <param name="skip">number of records to skip for pagination</param>
        /// <param name="limit">maximum number of records to return</param>
        /// <returns>List&lt;Activity&gt;</returns>
        IAsyncOperation<List<Activity>> GetActivities (string searchString, int? skip, int? limit);
        /// <summary>
        /// gets the activity with the given ID Obtains the requested activity 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <returns>Activity</returns>
        IAsyncOperation<Activity> GetActivity (string id);
        /// <summary>
        /// returns if the activity can be opened or not If the activity can be opened, e.g. is hosted in a web, this endpoint will return the target URI 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <returns></returns>
        IAsyncOperation GetActivityTarget (string id);
        /// <summary>
        /// gets the types of activities available Obtains the list of activity types available to be created with some additional data 
        /// </summary>
        /// <returns>List&lt;ActivityType&gt;</returns>
        IAsyncOperation<List<ActivityType>> GetActivityTypes ();
        /// <summary>
        /// gets the completion status of the activity Obtains the completion status of the activity 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation GetCompletion (string id, string users);
        /// <summary>
        /// gets the group with the given ID Obtains the requested group 
        /// </summary>
        /// <param name="id">The group ID</param>
        /// <returns>Group</returns>
        IAsyncOperation<Group> GetGroup (string id);
        /// <summary>
        /// gets the list of participants for this group Obtains the list of participants of the group 
        /// </summary>
        /// <param name="id">The group ID</param>
        /// <returns>List&lt;string&gt;</returns>
        IAsyncOperation<List<string>> GetGroupParticipants (string id);
        /// <summary>
        /// gets the printable PDF version of the group Usefull for assistance in the classroom, the printable version of the class allows the teacher to cut and give the codes to the students easily to anonymize them. 
        /// </summary>
        /// <param name="id">The class ID</param>
        /// <returns>byte[]</returns>
        IAsyncOperation<byte[]> GetGroupPrintable (string id);
        /// <summary>
        /// gets the assigned studies to the group Obtains the list of studies assigned to the group 
        /// </summary>
        /// <param name="id">The group ID</param>
        /// <returns>List&lt;Study&gt;</returns>
        IAsyncOperation<List<Study>> GetGroupStudies (string id);
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
        /// gets the list of studies for the current teacher Obtains the list of studies for the current teacher. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param>
        /// <param name="skip">number of records to skip for pagination</param>
        /// <param name="limit">maximum number of records to return</param>
        /// <returns>List&lt;Study&gt;</returns>
        IAsyncOperation<List<Study>> GetStudies (string searchString, int? skip, int? limit);
        /// <summary>
        /// gets the study with the given ID Obtains the requested study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns>Study</returns>
        IAsyncOperation<Study> GetStudy (string id);
        /// <summary>
        /// gets the allocator from a study Obtains the allocator used by the study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns>List&lt;Allocator&gt;</returns>
        IAsyncOperation<List<Allocator>> GetStudyAllocator (string id);
        /// <summary>
        /// gets the assigned groups to the study Obtains the list of groups assigned to the study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns>List&lt;Group&gt;</returns>
        IAsyncOperation<List<Group>> GetStudyGroups (string id);
        /// <summary>
        /// gets the list of participants for this study Obtains the list of participants of the study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns>List&lt;string&gt;</returns>
        IAsyncOperation<List<string>> GetStudyParticipants (string id);
        /// <summary>
        /// gets the tests from a study Obtains the list of tests assigned to the study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <returns>List&lt;Test&gt;</returns>
        IAsyncOperation<List<Test>> GetStudyTests (string id);
        /// <summary>
        /// gets the test with the given ID Obtains the requested test 
        /// </summary>
        /// <param name="studyid">The Study ID</param>
        /// <param name="testid">The test ID</param>
        /// <returns>Test</returns>
        IAsyncOperation<Test> GetTest (string studyid, string testid);
        /// <summary>
        /// gets the list of activities for the selected test Obtains the list of activities for the selected test 
        /// </summary>
        /// <param name="studyid">The Study ID</param>
        /// <param name="testid">The test ID</param>
        /// <returns>List&lt;Activity&gt;</returns>
        IAsyncOperation<List<Activity>> GetTestActivities (string studyid, string testid);
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
        IAsyncOperation SetCompletion (string id, string user);
        /// <summary>
        /// sets the result for the activity Set the completion status of the activity for a student 
        /// </summary>
        /// <param name="id">The test ID</param>
        /// <param name="user">the user to set its result (if logged in student , user is not needed) </param>
        /// <returns></returns>
        IAsyncOperation SetCompletion_2 (string id, string user);
        /// <summary>
        /// set the allocator to the study Updates the allocator from the study 
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <param name="body">study to be modified</param>
        /// <returns></returns>
        IAsyncOperation SetStudyAllocator (string id, Allocator body);
        /// <summary>
        /// updates the test Updates an existing test
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <param name="body">activity to be modified</param>
        /// <returns></returns>
        IAsyncOperation UpdateActivity (string id, Activity body);
        /// <summary>
        /// updates the group Updates an existing group
        /// </summary>
        /// <param name="id">The group ID</param>
        /// <param name="body">Group to be modified</param>
        /// <returns></returns>
        IAsyncOperation UpdateGroup (string id, Group body);
        /// <summary>
        /// updates the study Updates an existing stidy
        /// </summary>
        /// <param name="id">The study ID</param>
        /// <param name="body">study to be modified</param>
        /// <returns></returns>
        IAsyncOperation UpdateStudy (string id, Study body);
        /// <summary>
        /// updates the test Updates an existing test
        /// </summary>
        /// <param name="studyid">The Study ID</param>
        /// <param name="testid">The test ID</param>
        /// <param name="body">test to be modified</param>
        /// <returns></returns>
        IAsyncOperation UpdateTest (string studyid, string testid, Test body);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class TeachersApi : ITeachersApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeachersApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public TeachersApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="TeachersApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TeachersApi(String basePath)
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
        /// adds a Activity Creates a new activity for the current teacher. 
        /// </summary>
        /// <param name="body">Activity to be created</param> 
        /// <returns>Activity</returns>            
        public IAsyncOperation<Activity> AddActivity (Activity body)
        {
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling AddActivity");
            
    
            var path = "/activities";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Activity>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Activity)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Activity), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling AddActivity: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// adds a Activity to a test Creates a new activity for the current teacher. 
        /// </summary>
        /// <param name="studyid">The Study ID</param> 
        /// <param name="testid">The test ID</param> 
        /// <param name="body"></param> 
        /// <returns>Activity</returns>            
        public IAsyncOperation<Activity> AddActivityToTest (string studyid, string testid, Activity body)
        {
            
            // verify the required parameter 'studyid' is set
            if (studyid == null) throw new ApiException(400, "Missing required parameter 'studyid' when calling AddActivityToTest");
            
            // verify the required parameter 'testid' is set
            if (testid == null) throw new ApiException(400, "Missing required parameter 'testid' when calling AddActivityToTest");
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling AddActivityToTest");
            
    
            var path = "/studies/{studyid}/tests/{testid}/activities";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "studyid" + "}", ApiClient.ParameterToString(studyid));
path = path.Replace("{" + "testid" + "}", ApiClient.ParameterToString(testid));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
            UnityEngine.Debug.Log(path);
            UnityEngine.Debug.Log(postBody);
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Activity>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Activity)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Activity), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling AddActivityToTest: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// adds a group for the current user as owner. Creates a new group for the current user as owner. 
        /// </summary>
        /// <param name="body"></param> 
        /// <returns>Group</returns>            
        public IAsyncOperation<Group> AddGroup (AddGroupBody body)
        {
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling AddGroup");
            
    
            var path = "/groups";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Group>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Group)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Group), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling AddGroup: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// adds an study for the current teacher Creates a new study for the current teacher. 
        /// </summary>
        /// <param name="body">Study to be created</param> 
        /// <returns>Study</returns>            
        public IAsyncOperation<Study> AddStudy (AddStudyBody body)
        {
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling AddStudy");
            
    
            var path = "/studies";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Study>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Study)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Study), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling AddStudy: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// adds a test to the study Adds a test for the current group 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <param name="body"></param> 
        /// <returns>Test</returns>            
        public IAsyncOperation<Test> AddTestToStudy (string id, Test body)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling AddTestToStudy");
            
            // verify the required parameter 'body' is set
            if (body == null) throw new ApiException(400, "Missing required parameter 'body' when calling AddTestToStudy");
            
    
            var path = "/studies/{id}/tests";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                postBody = ApiClient.Serialize(body); // http body (model) parameter
    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Test>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPOST, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Test)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Test), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling AddTestToStudy: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// deletes the activity with the given ID Deleted the specified activity 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <returns></returns>            
        public IAsyncOperation DeleteActivity (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling DeleteActivity");
            
    
            var path = "/activities/{id}";
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbDELETE, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling DeleteActivity: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// Deletes the study with the given ID Deleted the designed study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns></returns>            
        public IAsyncOperation DeleteStudy (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling DeleteStudy");
            
    
            var path = "/studies/{id}";
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbDELETE, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling DeleteStudy: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// deletes the test with the given ID Deleted the selected test 
        /// </summary>
        /// <param name="studyid">The Study ID</param> 
        /// <param name="testid">The test ID</param> 
        /// <returns></returns>            
        public IAsyncOperation DeleteTest (string studyid, string testid)
        {
            
            // verify the required parameter 'studyid' is set
            if (studyid == null) throw new ApiException(400, "Missing required parameter 'studyid' when calling DeleteTest");
            
            // verify the required parameter 'testid' is set
            if (testid == null) throw new ApiException(400, "Missing required parameter 'testid' when calling DeleteTest");
            
    
            var path = "/studies/{studyid}/tests/{testid}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "studyid" + "}", ApiClient.ParameterToString(studyid));
path = path.Replace("{" + "testid" + "}", ApiClient.ParameterToString(testid));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbDELETE, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling DeleteTest: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the list of activities for the current teacher Obtains the list of activities for the current teacher. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param> 
        /// <param name="skip">number of records to skip for pagination</param> 
        /// <param name="limit">maximum number of records to return</param> 
        /// <returns>List&lt;Activity&gt;</returns>            
        public IAsyncOperation<List<Activity>> GetActivities (string searchString, int? skip, int? limit)
        {
            
    
            var path = "/activities";
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
    

            var result = new AsyncCompletionSource<List<Activity>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Activity>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Activity>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetActivities: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the activity with the given ID Obtains the requested activity 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <returns>Activity</returns>            
        public IAsyncOperation<Activity> GetActivity (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetActivity");
            
    
            var path = "/activities/{id}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Activity>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Activity)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Activity), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetActivity: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// returns if the activity can be opened or not If the activity can be opened, e.g. is hosted in a web, this endpoint will return the target URI 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <returns></returns>            
        public IAsyncOperation GetActivityTarget (string id)
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
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetActivityTarget: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the types of activities available Obtains the list of activity types available to be created with some additional data 
        /// </summary>
        /// <returns>List&lt;ActivityType&gt;</returns>            
        public IAsyncOperation<List<ActivityType>> GetActivityTypes ()
        {
            
    
            var path = "/activitytypes";
            path = path.Replace("{format}", "json");
                
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<ActivityType>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<ActivityType>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<ActivityType>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetActivityTypes: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the completion status of the activity Obtains the completion status of the activity 
        /// </summary>
        /// <param name="id">The test ID</param> 
        /// <param name="users">the list of users comma separated or none for all (if logged in student , user is not needed) </param> 
        /// <returns></returns>            
        public IAsyncOperation GetCompletion (string id, string users)
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
    

            var result = new AsyncCompletionSource();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
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
        /// gets the list of participants for this group Obtains the list of participants of the group 
        /// </summary>
        /// <param name="id">The group ID</param> 
        /// <returns>List&lt;string&gt;</returns>            
        public IAsyncOperation<List<string>> GetGroupParticipants (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetGroupParticipants");
            
    
            var path = "/groups/{id}/participants";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<string>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<string>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<string>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetGroupParticipants: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the printable PDF version of the group Usefull for assistance in the classroom, the printable version of the class allows the teacher to cut and give the codes to the students easily to anonymize them. 
        /// </summary>
        /// <param name="id">The class ID</param> 
        /// <returns>byte[]</returns>            
        public IAsyncOperation<byte[]> GetGroupPrintable (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetGroupPrintable");
            
    
            var path = "/groups/{id}/printable";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<byte[]>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = webRequest.downloadHandler.data;
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetGroupPrintable: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the assigned studies to the group Obtains the list of studies assigned to the group 
        /// </summary>
        /// <param name="id">The group ID</param> 
        /// <returns>List&lt;Study&gt;</returns>            
        public IAsyncOperation<List<Study>> GetGroupStudies (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetGroupStudies");
            
    
            var path = "/groups/{id}/studies";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<Study>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Study>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Study>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetGroupStudies: " + apiEx.Message, apiEx.ErrorContent));
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
        /// gets the list of studies for the current teacher Obtains the list of studies for the current teacher. 
        /// </summary>
        /// <param name="searchString">pass an optional search string for result filtering</param> 
        /// <param name="skip">number of records to skip for pagination</param> 
        /// <param name="limit">maximum number of records to return</param> 
        /// <returns>List&lt;Study&gt;</returns>            
        public IAsyncOperation<List<Study>> GetStudies (string searchString, int? skip, int? limit)
        {
            
    
            var path = "/studies";
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
    

            var result = new AsyncCompletionSource<List<Study>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Study>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Study>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetStudies: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the study with the given ID Obtains the requested study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns>Study</returns>            
        public IAsyncOperation<Study> GetStudy (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetStudy");
            
    
            var path = "/studies/{id}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Study>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Study)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Study), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetStudy: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the allocator from a study Obtains the allocator used by the study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns>List&lt;Allocator&gt;</returns>            
        public IAsyncOperation<List<Allocator>> GetStudyAllocator (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetStudyAllocator");
            
    
            var path = "/studies/{id}/allocator";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<Allocator>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Allocator>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Allocator>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetStudyAllocator: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the assigned groups to the study Obtains the list of groups assigned to the study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns>List&lt;Group&gt;</returns>            
        public IAsyncOperation<List<Group>> GetStudyGroups (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetStudyGroups");
            
    
            var path = "/studies/{id}/groups";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
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
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetStudyGroups: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the list of participants for this study Obtains the list of participants of the study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns>List&lt;string&gt;</returns>            
        public IAsyncOperation<List<string>> GetStudyParticipants (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetStudyParticipants");
            
    
            var path = "/studies/{id}/participants";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<string>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<string>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<string>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetStudyParticipants: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the tests from a study Obtains the list of tests assigned to the study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <returns>List&lt;Test&gt;</returns>            
        public IAsyncOperation<List<Test>> GetStudyTests (string id)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling GetStudyTests");
            
    
            var path = "/studies/{id}/tests";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<Test>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Test>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Test>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetStudyTests: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the test with the given ID Obtains the requested test 
        /// </summary>
        /// <param name="studyid">The Study ID</param> 
        /// <param name="testid">The test ID</param> 
        /// <returns>Test</returns>            
        public IAsyncOperation<Test> GetTest (string studyid, string testid)
        {
            
            // verify the required parameter 'studyid' is set
            if (studyid == null) throw new ApiException(400, "Missing required parameter 'studyid' when calling GetTest");
            
            // verify the required parameter 'testid' is set
            if (testid == null) throw new ApiException(400, "Missing required parameter 'testid' when calling GetTest");
            
    
            var path = "/studies/{studyid}/tests/{testid}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "studyid" + "}", ApiClient.ParameterToString(studyid));
path = path.Replace("{" + "testid" + "}", ApiClient.ParameterToString(testid));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<Test>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (Test)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(Test), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetTest: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// gets the list of activities for the selected test Obtains the list of activities for the selected test 
        /// </summary>
        /// <param name="studyid">The Study ID</param> 
        /// <param name="testid">The test ID</param> 
        /// <returns>List&lt;Activity&gt;</returns>            
        public IAsyncOperation<List<Activity>> GetTestActivities (string studyid, string testid)
        {
            
            // verify the required parameter 'studyid' is set
            if (studyid == null) throw new ApiException(400, "Missing required parameter 'studyid' when calling GetTestActivities");
            
            // verify the required parameter 'testid' is set
            if (testid == null) throw new ApiException(400, "Missing required parameter 'testid' when calling GetTestActivities");
            
    
            var path = "/studies/{studyid}/tests/{testid}/activities";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "studyid" + "}", ApiClient.ParameterToString(studyid));
path = path.Replace("{" + "testid" + "}", ApiClient.ParameterToString(testid));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] { "OAuth2" };
    

            var result = new AsyncCompletionSource<List<Activity>>();

            // make the HTTP request
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbGET, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    var uniWebRequest = (UnityWebRequest)webRequest;
                    var headers = uniWebRequest.GetResponseHeaders().Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToList();
                    var data = (List<Activity>)ApiClient.Deserialize(webRequest.downloadHandler.text, typeof(List<Activity>), headers);
                    result.SetResult(data);
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling GetTestActivities: " + apiEx.Message, apiEx.ErrorContent));
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
        public IAsyncOperation SetCompletion (string id, string user)
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
            String postBody = null;
    
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
        public IAsyncOperation SetCompletion_2 (string id, string user)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling SetCompletion_2");
            
    
            var path = "/activities/{id}/result";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, String>();
            String postBody = null;
    
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
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling SetCompletion_2: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// set the allocator to the study Updates the allocator from the study 
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <param name="body">study to be modified</param> 
        /// <returns></returns>            
        public IAsyncOperation SetStudyAllocator (string id, Allocator body)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling SetStudyAllocator");
            
    
            var path = "/studies/{id}/allocator";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPUT, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling SetStudyAllocator: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// updates the test Updates an existing test
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <param name="body">activity to be modified</param> 
        /// <returns></returns>            
        public IAsyncOperation UpdateActivity (string id, Activity body)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling UpdateActivity");
            
    
            var path = "/activities/{id}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPUT, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling UpdateActivity: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// updates the group Updates an existing group
        /// </summary>
        /// <param name="id">The group ID</param> 
        /// <param name="body">Group to be modified</param> 
        /// <returns></returns>            
        public IAsyncOperation UpdateGroup (string id, Group body)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling UpdateGroup");
            
    
            var path = "/groups/{id}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPUT, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling UpdateGroup: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// updates the study Updates an existing stidy
        /// </summary>
        /// <param name="id">The study ID</param> 
        /// <param name="body">study to be modified</param> 
        /// <returns></returns>            
        public IAsyncOperation UpdateStudy (string id, Study body)
        {
            
            // verify the required parameter 'id' is set
            if (id == null) throw new ApiException(400, "Missing required parameter 'id' when calling UpdateStudy");
            
    
            var path = "/studies/{id}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "id" + "}", ApiClient.ParameterToString(id));
    
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPUT, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling UpdateStudy: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
        /// <summary>
        /// updates the test Updates an existing test
        /// </summary>
        /// <param name="studyid">The Study ID</param> 
        /// <param name="testid">The test ID</param> 
        /// <param name="body">test to be modified</param> 
        /// <returns></returns>            
        public IAsyncOperation UpdateTest (string studyid, string testid, Test body)
        {
            
            // verify the required parameter 'studyid' is set
            if (studyid == null) throw new ApiException(400, "Missing required parameter 'studyid' when calling UpdateTest");
            
            // verify the required parameter 'testid' is set
            if (testid == null) throw new ApiException(400, "Missing required parameter 'testid' when calling UpdateTest");
            
    
            var path = "/studies/{studyid}/tests/{testid}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "studyid" + "}", ApiClient.ParameterToString(studyid));
path = path.Replace("{" + "testid" + "}", ApiClient.ParameterToString(testid));
    
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
            ApiClient.CallApi(path, UnityWebRequest.kHttpVerbPUT, queryParams, postBody, headerParams, formParams, fileParams, authSettings)
                .Then(webRequest => {
                    result.SetCompleted();
                })
                .Catch(error => {
                    var apiEx = (ApiException)error;
                    result.SetException(new ApiException(apiEx.ErrorCode, "Error calling UpdateTest: " + apiEx.Message, apiEx.ErrorContent));
                });
    
            return result;
        }
    
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityFx.Async;
using UnityFx.Async.Promises;
using UnityEngine.Networking;
using Xasu.Auth;
using Xasu.Auth.Protocols;
using Xasu.Requests;
using System.Threading.Tasks;

namespace Simva
{
    /// <summary>
    /// API client is mainly responible for making the HTTP call to the API backend.
    /// </summary>
    public class ApiClient
    {
        private readonly Dictionary<String, String> _defaultHeaderMap = new Dictionary<String, String>();
  
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient" /> class.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public ApiClient(String basePath="https://virtserver.swaggerhub.com/Synpheros/Simva/1.0.0")
        {
            BasePath = basePath;
        }
    
        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        /// <value>The base path</value>
        public string BasePath { get; set; }
    
        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        /// <value>The authorization path</value>
        public string AuthPath { get; set; }
    
        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        /// <value>The authorization path</value>
        public string TokenPath { get; set; }

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        /// <value>The authorization path</value>
        public OAuth2Protocol Authorization { get; private set; }

        public IAsyncOperation InitOAuth(string clientId, string clientSecret = null,
            string realm = null, string appName = null, string scopeSeparator = ":", bool usePKCE = false,
            Dictionary<string, string> aditionalQueryStringParams = null , bool scope_offline = false)
        {
            String[] scopes = null;
            if (scope_offline)
            {
                scopes = new string[] { "offline_access" };
            }
            else
            {
                scopes = new string[] { };
            }


            var tokenUrl = TokenPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/token";
            var authUrl = AuthPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/auth";

            var done = new AsyncCompletionSource();

            var authorization = AuthManager.InitAuth("oauth2", new Dictionary<string, string>()
            {
                { "grant_type", "code" },
                { "auth_endpoint", authUrl },
                { "token_endpoint", tokenUrl },
                { "client_id", clientId },
                { "code_challenge_method", "S256" },
                { "scope", string.Join(scopeSeparator, scopes) }
            }, null);

            authorization.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    done.SetException(t.Exception);
                    return;
                }

                Authorization = (OAuth2Protocol)t.Result;
                done.SetCompleted();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            return done;
        }

		public IAsyncOperation InitOAuth(string username, string password, string clientId, string clientSecret = null,
		string realm = null, string appName = null, string scopeSeparator = ":", bool usePKCE = false,
		Dictionary<string, string> aditionalQueryStringParams = null, bool scope_offline = false)
		{
			String[] scopes = null;
			if (scope_offline)
			{
				scopes = new string[] { "offline_access" };
			}
			else
			{
				scopes = new string[] { };
			}


			var tokenUrl = TokenPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/token";
			var authUrl = AuthPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/auth";

			var done = new AsyncCompletionSource();
            
            var authorization = AuthManager.InitAuth("oauth2", new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "token_endpoint", tokenUrl },
                { "client_id", clientId },
                { "username", username },
                { "password", password },
                { "code_challenge_method", "S256" },
                { "scope", string.Join(scopeSeparator, scopes) }
            }, null);

            authorization.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    done.SetException(t.Exception);
                    return;
                }

                Authorization = (OAuth2Protocol)t.Result;
                done.SetCompleted();
            }, TaskScheduler.FromCurrentSynchronizationContext());

			return done;
		}

		public IAsyncOperation InitOAuth(string refreshToken, string clientId)
        {
            var scopes = new string[] { };

            var tokenUrl = TokenPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/token";
            var authUrl = AuthPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/auth";

            var done = new AsyncCompletionSource();

			try
			{
                var authorization = AuthManager.InitAuth("oauth2", new Dictionary<string, string>()
                {
                    { "grant_type", "refresh_token" },
                    { "token_endpoint", tokenUrl },
                    { "client_id", clientId },
                    { "refresh_token", refreshToken }
                }, null);

                authorization.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        done.SetException(t.Exception);
                        return;
                    }

                    Authorization = (OAuth2Protocol)t.Result;
                    done.SetCompleted();
                }, TaskScheduler.FromCurrentSynchronizationContext());
			}
            catch(ApiException ex)
            {
                done.SetException(new ApiException(ex.ErrorCode, "Failed to renew AuthorizationInfo: " + ex.Message));
            }

            return done;
        }

        public IAsyncOperation ContinueOAuth(string clientId)
        {
            throw new NotSupportedException("WebGL not yet compatible");
            /*
            var scopes = new string[] { };

            var tokenUrl = TokenPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/token";
            var authUrl = AuthPath ?? "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect/auth";

            var done = new AsyncCompletionSource();
            try
            {
                OpenIdUtility.TryContinueLogin(tokenUrl, clientId)
                    .Then(authInfo =>
                    {
                        AuthorizationInfo = authInfo;
                        done.SetCompleted();
                    });
            }
            catch (ApiException ex)
            {
                done.SetException(new ApiException(ex.ErrorCode, "Failed to renew AuthorizationInfo: " + ex.Message));
            }

            return done;*/
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        public Dictionary<String, String> DefaultHeader
        {
            get { return _defaultHeaderMap; }
        }

        /// <summary>
        /// Makes the HTTP request (Sync).
        /// </summary>
        /// <param name="path">URL path.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="queryParams">Query parameters.</param>
        /// <param name="postBody">HTTP body (POST request).</param>
        /// <param name="headerParams">Header parameters.</param>
        /// <param name="formParams">Form parameters.</param>
        /// <param name="fileParams">File parameters.</param>
        /// <param name="authSettings">Authentication settings.</param>
        /// <returns>IAsyncOperation<UnityWebRequest></returns>
        public IAsyncOperation<UnityWebRequest> CallApi(String path, string method, Dictionary<String, String> queryParams, String postBody,
            Dictionary<String, String> headerParams, Dictionary<String, String> formParams,
            Dictionary<String, String> fileParams, String[] authSettings, bool inBackground = false)
        {
            UnityWebRequest request = null;
            var result = new AsyncCompletionSource<UnityWebRequest>();

            switch (method){
                case UnityWebRequest.kHttpVerbGET:
                    request = UnityWebRequest.Get(BasePath + path);
                    break;
                case UnityWebRequest.kHttpVerbPOST:
                    request = UnityWebRequest.Post(BasePath + path, formParams);
                    break;
                case UnityWebRequest.kHttpVerbPUT:
                    request = UnityWebRequest.Put(BasePath + path, postBody);
                    break;
                case UnityWebRequest.kHttpVerbDELETE:
                    request = UnityWebRequest.Delete(BasePath + path);
                    break;
                default:
                    throw new ApiException(500, "Method not available: " + method);
            }

            UpdateParamsForAuth(queryParams, headerParams)
                .Then(() =>
                {
                    // add default header, if any
                    foreach (var defaultHeader in _defaultHeaderMap)
                        request.SetRequestHeader(defaultHeader.Key, defaultHeader.Value);

                    // add header parameter, if any
                    foreach (var param in headerParams)
                        request.SetRequestHeader(param.Key, param.Value);

                    if (queryParams.Count > 0 || formParams.Count > 0 && method != UnityWebRequest.kHttpVerbPOST)
                    {
                        request.url += "?";
                    }

                    // add query parameter, if any
                    if (queryParams.Count > 0)
                    {
                        request.url += String.Join("&", queryParams.Select(kv => kv.Key + "=" + kv.Value).ToArray());
                    }

                    foreach (var param in fileParams)
                    {
                        throw new ApiException(500, "Uploading files is not implemented.");
                    }

                    // add form parameter, if any
                    if (formParams.Count > 0 && method != UnityWebRequest.kHttpVerbPOST)
                    {
                        request.url += String.Join("&", formParams.Select(kv => kv.Key + "=" + kv.Value).ToArray());
                    }

                    // add file parameter, if any
                    foreach (var param in fileParams)
                    {
                        throw new ApiException(500, "Uploading files is not implemented.");
                    }

                    if (postBody != null)
                    {
                        // http body (model) parameter
                        var bodyBytes = System.Text.Encoding.UTF8.GetBytes(postBody);
                        request.SetRequestHeader("Content-Type", "application/json");
                        request.uploadHandler = new UploadHandlerRaw(bodyBytes);
                        request.downloadHandler = new DownloadHandlerBuffer();
                    }

                    if (inBackground)
                    {
                        RequestsUtil.DoRequestInBackground(request).Wrap(result);
                    }
                    else
                    {
                        RequestsUtil.DoRequest(request).Wrap(result);
                    }
                })
                .Catch(error =>
                {
                    result.SetException(error);
                });

            return result;

        }

    
        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        public void AddDefaultHeader(string key, string value)
        {
            _defaultHeaderMap.Add(key, value);
        }
    
        /// <summary>
        /// If parameter is DateTime, output in a formatted string (default ISO 8601), customizable with Configuration.DateTime.
        /// If parameter is a list of string, join the list with ",".
        /// Otherwise just return the string.
        /// </summary>
        /// <param name="obj">The parameter (header, path, query, form).</param>
        /// <returns>Formatted string.</returns>
        public string ParameterToString(object obj)
        {
            if (obj is DateTime)
                // Return a formatted date string - Can be customized with Configuration.DateTimeFormat
                // Defaults to an ISO 8601, using the known as a Round-trip date/time pattern ("o")
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8
                // For example: 2009-06-15T13:45:30.0000000
                return ((DateTime)obj).ToString (Configuration.DateTimeFormat);
            else if (obj is List<string>)
                return String.Join(",", (obj as List<string>).ToArray());
            else
                return Convert.ToString (obj);
        }
    
        /// <summary>
        /// Deserialize the JSON string into a proper object.
        /// </summary>
        /// <param name="content">HTTP body (e.g. string, JSON).</param>
        /// <param name="type">Object type.</param>
        /// <param name="headers">HTTP headers.</param>
        /// <returns>Object representation of the JSON string.</returns>
        public object Deserialize(string content, Type type, IList<string> headers=null)
        {
            if (type == typeof(System.Object)) // return an object
            {
                return content;
            }

            if (type == typeof(Stream))
            {
                var filePath = String.IsNullOrEmpty(Configuration.TempFolderPath)
                    ? Path.GetTempPath()
                    : Configuration.TempFolderPath;

                var fileName = filePath + Guid.NewGuid();
                if (headers != null)
                {
                    var regex = new Regex(@"Content-Disposition:.*filename=['""]?([^'""\s]+)['""]?$");
                    var match = regex.Match(headers.ToString());
                    if (match.Success)
                        fileName = filePath + match.Value.Replace("\"", "").Replace("'", "");
                }
                File.WriteAllText(fileName, content);
                return new FileStream(fileName, FileMode.Open);

            }

            if (type.Name.StartsWith("System.Nullable`1[[System.DateTime")) // return a datetime object
            {
                return DateTime.Parse(content,  null, System.Globalization.DateTimeStyles.RoundtripKind);
            }

            if (type == typeof(String) || type.Name.StartsWith("System.Nullable")) // return primitive type
            {
                return ConvertType(content, type); 
            }
    
            // at this point, it must be a model (json)
            try
            {
                return JsonConvert.DeserializeObject(content, type);
            }
            catch (IOException e)
            {
                throw new ApiException(500, e.Message);
            }
        }
    
        /// <summary>
        /// Serialize an object into JSON string.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>JSON string.</returns>
        public string Serialize(object obj)
        {
            try
            {
                return obj != null ? JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : null;
            }
            catch (Exception e)
            {
                throw new ApiException(500, e.Message);
            }
        }
    
        /// <summary>
        /// Get the API key with prefix.
        /// </summary>
        /// <param name="apiKeyIdentifier">API key identifier (authentication scheme).</param>
        /// <returns>API key with prefix.</returns>
        public string GetApiKeyWithPrefix (string apiKeyIdentifier)
        {
            var apiKeyValue = "";
            Configuration.ApiKey.TryGetValue (apiKeyIdentifier, out apiKeyValue);
            var apiKeyPrefix = "";
            if (Configuration.ApiKeyPrefix.TryGetValue (apiKeyIdentifier, out apiKeyPrefix))
                return apiKeyPrefix + " " + apiKeyValue;
            else
                return apiKeyValue;
        }
    
        /// <summary>
        /// Update parameters based on authentication.
        /// </summary>
        /// <param name="queryParams">Query parameters.</param>
        /// <param name="headerParams">Header parameters.</param>
        /// <param name="authSettings">Authentication settings.</param>
        public IAsyncOperation UpdateParamsForAuth(Dictionary<String, String> queryParams, Dictionary<String, String> headerParams)
        {
            var result = new AsyncCompletionSource();

            var myHttpRequest = new MyHttpRequest
            {
                queryParams = queryParams,
                headers = headerParams
            };

            Authorization.UpdateParamsForAuth(myHttpRequest)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        result.SetException(t.Exception);
                        return;
                    }

                    result.SetCompleted();
                }, TaskScheduler.FromCurrentSynchronizationContext());

            return result;
        }
 
        /// <summary>
        /// Encode string in base64 format.
        /// </summary>
        /// <param name="text">String to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public static string Base64Encode(string text)
        {
            var textByte = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textByte);
        }
    
        /// <summary>
        /// Dynamically cast the object into target type.
        /// </summary>
        /// <param name="fromObject">Object to be casted</param>
        /// <param name="toObject">Target type</param>
        /// <returns>Casted object</returns>
        public static object ConvertType(object fromObject, Type toObject) {
            return Convert.ChangeType(fromObject, toObject);
        }
  
    }
}

using UnityEngine.Networking;
using UnityFx.Async;
using UnityFx.Async.Promises;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Simva;
using Simva.Api;
using Newtonsoft.Json.Linq;
using Xasu.Auth.Protocols;

namespace SimvaPlugin
{
    public class SimvaApi<T>
    {
        private string jwt = null;
        private static SimvaConf simvaConf;

        public T Api { get; private set; }
        public ApiClient ApiClient { get; private set; }

        public OAuth2Protocol Authorization
        {
            get
            {
                if (ApiClient == null)
                {
                    return null;
                }
                else
                {
                    return ApiClient.Authorization;
                }
            }
        }

        protected string Username 
        {
            get 
            {
                byte[] data = System.Convert.FromBase64String(jwt);
                string decodedString = System.Text.Encoding.UTF8.GetString(data).Split('.')[1];
                var json = JObject.Parse(decodedString);
                return json["data"]["username"].Value<string>();
            }
        }

        private static bool Inherits<A, B>()
        {
            return typeof(A) == typeof(B) || typeof(B).IsAssignableFrom(typeof(A));
        }

        protected SimvaApi(T api)
        {
            var t = typeof(T);
            if (!Inherits<T, IAdminsApi>() && Inherits<T, ITeachersApi>() && Inherits<T, IStudentsApi>() && Inherits<T, IDefaultApi>())
            {
                throw new Exception("Invalid Simva API: " + typeof(T));
            }
            if (Inherits<T, IAdminsApi>())
            {
                var admins = api as AdminsApi;
                ApiClient = admins.ApiClient;
            }
            else if (Inherits<T, ITeachersApi>())
            {
                var teachers = api as TeachersApi;
                ApiClient = teachers.ApiClient;
            }
            else if (Inherits<T, IStudentsApi>())
            {
                var students = api as StudentsApi;
                ApiClient = students.ApiClient;
            }
            else if (Inherits<T, IDefaultApi>())
            {
                var defaults = api as DefaultApi;
                ApiClient = defaults.ApiClient;
            }
            this.Api = api;
        }

        public SimvaConf SimvaConf
        {
            get 
            {
                if (simvaConf == null)
                {
                    simvaConf = SimvaConf.Local;
                }

                return simvaConf; 
            }
        }

        public static IAsyncOperation<SimvaApi<T>> Login(bool offline_access = false)
        {
            var apiClient = new ApiClient
            {
                BasePath = SimvaConf.Local.URL,
                AuthPath = SimvaConf.Local.SSO + "/auth",
                TokenPath = SimvaConf.Local.SSO + "/token"
            };

            Debug.Log("Platform: " + Application.platform.ToString());
            Debug.Log("Use PKCE: " + (Application.platform != RuntimePlatform.WebGLPlayer));

            var result = new AsyncCompletionSource<SimvaApi<T>>();
            apiClient.InitOAuth(SimvaConf.Local.ClientId, null, "simva", null, ":", Application.platform != RuntimePlatform.WebGLPlayer, null, offline_access)
                .Then(() =>
                {
                    if (Inherits<T, IAdminsApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IAdminsApi)new AdminsApi(apiClient)));
                    }
                    else if (Inherits<T, ITeachersApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(ITeachersApi)new TeachersApi(apiClient)));
                    }
                    else if (Inherits<T, IStudentsApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IStudentsApi)new StudentsApi(apiClient)));
                    }
                    else if (Inherits<T, IDefaultApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IDefaultApi)new DefaultApi(apiClient)));
                    }
                    else
                    {
                        throw new Exception("Unsupported api type: " + typeof(T));
                    }
                })
                .Catch(ex =>
                {
                    result.SetException(ex);
                });
            return result;
        }

        public static IAsyncOperation<SimvaApi<T>> Login(string refresh_token)
        {
            var apiClient = new ApiClient
            {
                BasePath = SimvaConf.Local.URL,
                AuthPath = SimvaConf.Local.SSO + "/auth",
                TokenPath = SimvaConf.Local.SSO + "/token"
            };

            var result = new AsyncCompletionSource<SimvaApi<T>>();
            apiClient.InitOAuth(refresh_token, SimvaConf.Local.ClientId).
                Then(() =>
                {
                    if (Inherits<T, IAdminsApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IAdminsApi)new AdminsApi(apiClient)));
                    }
                    else if (Inherits<T, ITeachersApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(ITeachersApi)new TeachersApi(apiClient)));
                    }
                    else if (Inherits<T, IStudentsApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IStudentsApi)new StudentsApi(apiClient)));
                    }
                    else if (Inherits<T, IDefaultApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IDefaultApi)new DefaultApi(apiClient)));
                    }
                    else
                    {
                        throw new Exception("Unsupported api type: " + typeof(T));
                    }

                })
                .Catch(error =>
                {
                    result.SetException(error);
                });

            return result;
        }

        public static IAsyncOperation<SimvaApi<T>> LoginWithToken(string token)
        {
            return LoginWithCredentials(token.ToLowerInvariant(), token.ToLowerInvariant());
        }

        public static IAsyncOperation<SimvaApi<T>> LoginWithCredentials(string username, string password)
        {
			var apiClient = new ApiClient
			{
				BasePath = SimvaConf.Local.URL,
				AuthPath = SimvaConf.Local.SSO + "/auth",
				TokenPath = SimvaConf.Local.SSO + "/token"
			};

			var result = new AsyncCompletionSource<SimvaApi<T>>();
			apiClient.InitOAuth(username, password, SimvaConf.Local.ClientId, null, "simva", null, ":", true, null, true)
                .Then(() =>
				{
					if (Inherits<T, IAdminsApi>())
					{
						result.SetResult(new SimvaApi<T>((T)(IAdminsApi)new AdminsApi(apiClient)));
					}
					else if (Inherits<T, ITeachersApi>())
					{
						result.SetResult(new SimvaApi<T>((T)(ITeachersApi)new TeachersApi(apiClient)));
					}
					else if (Inherits<T, IStudentsApi>())
					{
						result.SetResult(new SimvaApi<T>((T)(IStudentsApi)new StudentsApi(apiClient)));
					}
					else if (Inherits<T, IDefaultApi>())
					{
						result.SetResult(new SimvaApi<T>((T)(IDefaultApi)new DefaultApi(apiClient)));
					}
					else
					{
						throw new Exception("Unsupported api type: " + typeof(T));
					}

				})
				.Catch(error =>
				{
					result.SetException(error);
				});

			return result;
        }

        public static IAsyncOperation<SimvaApi<T>> ContinueLogin()
        {
            var apiClient = new ApiClient
            {
                BasePath = SimvaConf.Local.URL,
                AuthPath = SimvaConf.Local.SSO + "/auth",
                TokenPath = SimvaConf.Local.SSO + "/token"
            };

            var result = new AsyncCompletionSource<SimvaApi<T>>();
            apiClient.ContinueOAuth(SimvaConf.Local.ClientId)
                .Then(() =>
                {
                    if (Inherits<T, IAdminsApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IAdminsApi)new AdminsApi(apiClient)));
                    }
                    else if (Inherits<T, ITeachersApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(ITeachersApi)new TeachersApi(apiClient)));
                    }
                    else if (Inherits<T, IStudentsApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IStudentsApi)new StudentsApi(apiClient)));
                    }
                    else if (Inherits<T, IDefaultApi>())
                    {
                        result.SetResult(new SimvaApi<T>((T)(IDefaultApi)new DefaultApi(apiClient)));
                    }
                    else
                    {
                        throw new Exception("Unsupported api type: " + typeof(T));
                    }

                })
                .Catch(error =>
                {
                    result.SetException(error);
                });

            return result;
        }

        public IAsyncOperation<bool> Register(string username, string email, string password, bool teacher)
        {
            var result = new AsyncCompletionSource<bool>();
            ApiClient.CallApi("/users", UnityWebRequest.kHttpVerbPOST, new Dictionary<string, string>(), new JObject {
                { "username", username },
                { "password", password },
                { "email", email },
                { "role", teacher ? "teacher" : "student" }
            }.ToString(Newtonsoft.Json.Formatting.None), new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, string>(), new string[] { "OAuth2" })
                .Then(_ =>
                {
                    result.SetResult(true);
                })
                .Catch(error =>
                {
                    Debug.LogError(error.Message);
                    result.SetResult(false);
                });
            return result;
        }

        
        public IAsyncOperation<JObject> CreateStudyWithTestAndUsers(string studyName, string testName, string groupName, int numberOfUsers)
        {
            var result = new AsyncCompletionSource<JObject>();
            Simva.Model.Group createdGroup = null;
            string groupId = null;
            string testId = null;
            string studyId = null;
            int totalOperations = numberOfUsers + 4;

            var teachersApi = (ITeachersApi)Api;

            teachersApi.AddGroup(new Simva.Model.AddGroupBody { Name = groupName })
                .Then(group =>
                {
                    createdGroup = group;
                    groupId = group.Id;
                    var createUsers = CreateBatchUsers(numberOfUsers);
                    createUsers.ProgressChanged += (sender, args) =>
                    {
                        result.SetProgress(numberOfUsers * createUsers.Progress / totalOperations);
                    };
                    return createUsers;
                })
                .Then(users =>
                {
                    createdGroup.Participants.AddRange(users);
                    return teachersApi.UpdateGroup(groupId, createdGroup);
                })
                .Then(() =>
                {
                    result.SetProgress((numberOfUsers + 1) / totalOperations);
                    return teachersApi.AddStudy(new Simva.Model.AddStudyBody { Name = studyName });
                })
                .Then(study =>
                {
                    result.SetProgress((numberOfUsers + 2) / totalOperations);
                    studyId = study.Id;
                    study.Groups.Add(groupId);
                    return teachersApi.UpdateStudy(studyId, study);
                })
                .Then(() =>
                {
                    result.SetProgress((numberOfUsers + 3) / totalOperations);
                    return teachersApi.AddTestToStudy(studyId, new Simva.Model.Test { Name = "Test" });
                })
                .Then(test =>
                {
                    result.SetProgress((numberOfUsers + 4) / totalOperations);
                    testId = test.Id;
                    result.SetResult(new JObject
                    {
                        ["groupId"] = groupId,
                        ["studyId"] = studyId,
                        ["testId"]  = testId
                    });
                })
                .Catch(error =>
                {
                    var apiException = error as ApiException;
                    if(apiException != null)
                    {
                        Debug.Log(error.Message + " " + apiException.ErrorContent);
                    }
                    else
                    {
                        Debug.Log(error.Message);
                    }
                    result.SetException(error);
                });
            return result;
        }

        public IAsyncOperation<string[]> CreateBatchUsers(int number)
        {
            var result = new AsyncCompletionSource<string[]>();
            var listOfIds = new string[number];
            var amountDone = 0;
            for(int i = 0; i < number; i++)
            {
                var newUser = GenerateRandomBase58Key(4);
                listOfIds[i] = newUser;
                Register(newUser, newUser + "@simva.e-ucm.es", newUser, false)
                    .Then(registered =>
                    {
                        lock (listOfIds)
                        {
                            amountDone++;
                            result.SetProgress(amountDone / (float)number);
                        }

                        if(amountDone == number)
                        {
                            result.SetResult(listOfIds.ToArray());
                        }
                    });
            }

            return result;
        }


        private string GenerateRandomBase58Key(int length)
        {
            var alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
            return new string(Enumerable.Range(0, length).Select(_ => alphabet[UnityEngine.Random.Range(0, alphabet.Length)]).ToArray()).ToLower();
        }
    }
}
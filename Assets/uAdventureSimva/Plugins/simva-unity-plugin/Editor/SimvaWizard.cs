using Newtonsoft.Json.Linq;
using Simva.Api;
using Simva.Model;
using SimvaPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace Simva
{
    public class SimvaWizard
    {

        private const string DEFAULT_SSO = "https://sso.simva.e-ucm.es/auth/realms/simva/protocol/openid-connect";
        private const string DEFAULT_HOST = "simva-api.simva.e-ucm.es";
        private const string DEFAULT_PROTOCOL = "https";
        private const string DEFAULT_PORT= "443";
        private const string DEFAULT_CLIENTID = "uadventure";

        [DllImport("User32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        protected SimvaConf simvaConf;
        protected SimvaApi<ITeachersApi> simvaController;
        protected bool isLogin;
        protected bool inited;

        private bool preTest, postTest, saveTraces, realTime, backup;
        private int preId, postId, participants;
        private string email, registerUser, registerPassword, user, password;
        private bool tos;
        private List<ActivityType> activityTypes;
        private IAsyncOperation loadingPromise;

        private static GUIStyle linkStyle;
        private static GUIStyle titleStyle;


        private void Init()
        {
            CoroutineRunner.Instance.ExecuteCoroutineInEditor += r => EditorCoroutineUtility.StartCoroutine(r, this);

            var carga = new AsyncCompletionSource();
            SimvaConf.Local = new SimvaConf();
            CoroutineRunner.Instance.RunRoutine(LoadSimvaConf(SimvaConf.Local.LoadAsync(), carga));
            carga.Then(() =>
            {
                if (string.IsNullOrEmpty(SimvaConf.Local.Host) && EditorUtility.DisplayDialog("No config", 
                    "The config file at StreamingAssets/Simva.conf seems to be missing or is incorrect. Do you want to set it to default?", 
                    "Yes", "No"))
                {
                    SimvaConf.Local.Host = DEFAULT_HOST;
                    SimvaConf.Local.Port = DEFAULT_PORT;
                    SimvaConf.Local.SSO = DEFAULT_SSO;
                    SimvaConf.Local.Protocol = DEFAULT_PROTOCOL;
                    SimvaConf.Local.ClientId = DEFAULT_CLIENTID;
                    SimvaConf.Local.Save();
                    AssetDatabase.ImportAsset("StreamingAssets/Simva.conf");
                    AssetDatabase.Refresh();
                }

                if (PlayerPrefs.HasKey("Simva.RefreshToken"))
                {
                    Login();
                }

            });


            if (linkStyle == null)
            {
                linkStyle = new GUIStyle(GUI.skin.label);
                linkStyle.richText = true;
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.richText = true;
            }
            inited = true;
        }

        private void Login()
        {
            SimvaConf.Local = new SimvaConf();
            var carga = new AsyncCompletionSource(); 
            CoroutineRunner.Instance.RunRoutine(LoadSimvaConf(SimvaConf.Local.LoadAsync(), carga));
            isLogin = true;
            carga.Then(() =>
            {
                IAsyncOperation<SimvaApi<ITeachersApi>> loginCall = null;
                if (PlayerPrefs.HasKey("Simva.RefreshToken"))
                {
                    loginCall = SimvaApi<ITeachersApi>.Login(PlayerPrefs.GetString("Simva.RefreshToken"));
                }
                else
                {
                    loginCall = SimvaApi<ITeachersApi>.Login(true);
                }

                loginCall.Then(simvaController =>
                {
                    this.simvaController = simvaController;
                    this.simvaConf = simvaController.SimvaConf;
                    var apiClient = ((TeachersApi)this.simvaController.Api).ApiClient;
                    apiClient.Authorization.RegisterAuthInfoUpdate(auth => PlayerPrefs.SetString("Simva.RefreshToken", auth.RefreshToken));
                    PlayerPrefs.Save();
                })
                    .Catch(error =>
                    {
                        PlayerPrefs.SetString("Simva.RefreshToken", null);
                        PlayerPrefs.Save();
                        Debug.LogException(error);
                        //Controller.Instance.ShowErrorDialog("Simva.Login.Error.Title", "Simva.Login.Error.Message");
                    })
                    .Finally(() => isLogin = false);
            });
        }

        private IEnumerator LoadSimvaConf(IEnumerator coroutine, IAsyncCompletionSource op)
        {
            yield return coroutine;
            op.SetCompleted();
        }

        public void OnGUI()
        {
            if (!inited)
            {
                Init();
            }

            if (simvaController == null)
            {
                // Login Form
                DoRegisterAndLogin();
            }
            else if (string.IsNullOrEmpty(simvaController.SimvaConf.Study))
            {
                // Wizard
                DoWizard();
            }
            else
            {
                // Study overview
                DoDashboard();
            }
        }

        private void DoWizard()
        {
            if (activityTypes == null && loadingPromise == null)
            {
                loadingPromise = simvaController.Api.GetActivityTypes()
                    .Then(activityTypes =>
                    {
                        this.activityTypes = activityTypes;
                    })
                    .Catch(error =>
                    {
                        Debug.Log(error.Message);
                        Debug.LogException(error);
                        EditorUtility.DisplayDialog("Simva: Error happened!", error.Message, "Ok");
                    });
            }

            DoBorderWithTitle("Create your study", () =>
            {
                using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                {
                    preTest = GUILayout.Toggle(preTest, "Pre-Test", EditorStyles.toolbarButton);
                    using (new EditorGUI.DisabledScope(true))
                    {
                        GUILayout.Toggle(true, "Gameplay", EditorStyles.toolbarButton);
                    }
                    postTest = GUILayout.Toggle(postTest, "Post-Test", EditorStyles.toolbarButton);
                }

                using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                {
                    // PreTest
                    using (new EditorGUI.DisabledGroupScope(!preTest))
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true)))
                    {
                        preId = Mathf.Clamp(0, EditorGUILayout.IntField("Survey Id", preId), 9999999);
                        if (GUILayout.Button("Create new"))
                        {
                            var limeSurvey = activityTypes.Where(a => a.Type == "limesurvey").FirstOrDefault();
                            var utils = limeSurvey.Utils as Newtonsoft.Json.Linq.JObject;
                            Application.OpenURL((string)utils["url"] + "/admin/survey/sa/newsurvey");
                        }
                    }
                    // Gameplay
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true)))
                    {
                        saveTraces = EditorGUILayout.Toggle("Save traces in storage", saveTraces);
                        realTime = EditorGUILayout.Toggle("Realtime analysis", realTime);
                        backup = EditorGUILayout.Toggle("Do traces backup", backup);
                    }
                    // PostTest
                    using (new EditorGUI.DisabledGroupScope(!postTest))
                    using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true)))
                    {

                        postId = Mathf.Clamp(0, EditorGUILayout.IntField("Survey Id", postId), 9999999);
                        if (GUILayout.Button("Create new"))
                        {
                            var limeSurvey = activityTypes.Where(a => a.Type == "limesurvey").FirstOrDefault();
                            var utils = limeSurvey.Utils as Newtonsoft.Json.Linq.JObject;
                            Application.OpenURL((string)utils["url"] + "/admin/survey/sa/newsurvey");
                        }
                    }
                }

                DoSeparator(" Participants ");

                participants = Mathf.Clamp(1, EditorGUILayout.IntField("Participants", participants), 10000);


                if (GUILayout.Button("Create"))
                {
                    EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Creation.Info", 0);
                    JObject creationData = null;
                    var createStudy = simvaController.CreateStudyWithTestAndUsers("uAdventure", "uAdventure", DateTime.Now.ToString("dd-M-yyyy"), participants)
                    .Then(result => creationData = result);

                    createStudy.ProgressChanged += (sender, args) =>
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Creation.Info", createStudy.Progress);
                    };

                    EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Info", 0);
                    if (preTest)
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.PreSurvey", 0.25f);
                        createStudy = createStudy.Then(() =>
                        {
                            return simvaController.Api.AddActivityToTest(creationData["studyId"].Value<string>(), creationData["testId"].Value<string>(), new Activity
                            {
                                Name = "PreTest",
                                Type = "limesurvey",
                                CopySurvey = preId.ToString()
                            });
                        });
                    }

                    createStudy = createStudy.Then(() =>
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Gameplay", 0.50f);

                        return simvaController.Api.AddActivityToTest(creationData["studyId"].Value<string>(), creationData["testId"].Value<string>(), new Activity
                        {
                            Name = "Gameplay",
                            Type = "gameplay",
                            Backup = backup,
                            TraceStorage = saveTraces,
                            Realtime = realTime
                        });
                    });

                    if (postTest)
                    {
                        createStudy = createStudy.Then(() =>
                        {
                            EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.PostSurvey", 0.75f);
                            return simvaController.Api.AddActivityToTest(creationData["studyId"].Value<string>(), creationData["testId"].Value<string>(), new Activity
                            {
                                Name = "PostTest",
                                Type = "limesurvey",
                                CopySurvey = postId.ToString()
                            });
                        });
                    }

                    createStudy.Then(() =>
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Completing", 0.99f);
                        return simvaController.Api.GetStudy(creationData["studyId"].Value<string>());
                    })
                    .Then(study =>
                    {
                        EditorUtility.ClearProgressBar();
                        simvaController.SimvaConf.Study = study.Id;
                        simvaController.SimvaConf.Save();
                    })
                    .Catch(error =>
                    {
                        Debug.LogException(error);
                        EditorUtility.DisplayDialog("Simva: Error happened!", error.Message, "Ok");
                        var apiEx = error as ApiException;
                        if (apiEx != null)
                        {
                            Debug.LogError(apiEx.Message + ": " + apiEx.ErrorContent);
                        }
                        else
                        {
                            Debug.LogError(error.Message);
                        }
                    })
                    .Finally(() =>
                    {
                        EditorUtility.ClearProgressBar();
                    });

                }

            });
        }

        private static IAsyncOperation<T[]> Paralell<T>(IEnumerable<IAsyncOperation<T>> promises)
        {
            var result = new AsyncCompletionSource<T[]>();
            int index = 0;
            int completed = 0;
            int total = promises.Count();
            T[] results = new T[total];
            foreach (var p in promises)
            {
                ThenIndex(index, p)
                    .Done(kv =>
                    {
                        results[kv.Key] = kv.Value;
                        completed++;
                        if (completed == total)
                        {
                            result.SetResult(results);
                        }
                    });
            }

            return result;
        }

        private static IAsyncOperation<KeyValuePair<int, T>> ThenIndex<T>(int index, IAsyncOperation<T> promise)
        {
            var result = new AsyncCompletionSource<KeyValuePair<int, T>>();
            promise.Done(t =>
            {
                result.SetResult(new KeyValuePair<int, T>(index, t));
            });
            return result;
        }

        private void DoDashboard()
        {
            DoBorderWithTitle("Dashboard", () =>
            {
                if (GUILayout.Button("Open dashboard in Simva"))
                {
                    var url = simvaController.SimvaConf.Host;
                    url = "https://" + url.Substring(url.IndexOf("api.") + "api.".Length);
                    Application.OpenURL(url + "/studies/" + simvaController.SimvaConf.Study);
                }

                if (GUILayout.Button("Download users in PDF"))
                {
                    simvaController.Api.GetStudy(simvaController.SimvaConf.Study)
                    .Then(study =>
                    {
                        if (study.Groups.Count <= 0)
                        {
                            //Controller.Instance.ShowErrorDialog("No groups", "The study has no groups!");
                            Debug.LogError("The study has no groups!");
                        }
                        else if (study.Groups.Count > 1)
                        {
                            Group[] groupNames = new Group[study.Groups.Count];
                            var promises = study.Groups.Select(g => simvaController.Api.GetGroup(g));
                            Paralell(promises)
                                .Then(groups =>
                                {
                                    foreach(var group in  groups)
                                    {
                                        if(EditorUtility.DisplayDialog("Download group", string.Format("Do you want to download the information of {0}", group.Name), "Yes", "Skip"))
                                        {
                                            DownloadGroup(group.Id);
                                        }
                                    }
                                });

                        }
                        else
                        {
                            DownloadGroup(study.Groups[0]);
                        }
                    });

                }

                DoSeparator(" or ");

                
                if (GUILayout.Button("Set up a new study") && EditorUtility.DisplayDialog("Warning", "Are you sure you wan't to unset the study?", "Yes", "Cancel"))
                {
                    simvaController.SimvaConf.Study = null;
                }
            });
        }


        private void DownloadGroup(string groupId)
        {
            string path = EditorUtility.SaveFilePanel("Select a place to store the participants", "%user%/Documents", "participants.pdf", "pdf");

            simvaController.Api.GetGroupPrintable(groupId)
                .Done(bytes =>
                {
                    File.WriteAllBytes(path, bytes);
                    OpenWithDefaultProgram(path);
                });
        }

        public static void OpenWithDefaultProgram(string path)
        {
            System.Diagnostics.Process fileopener = new System.Diagnostics.Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        private void DoRegisterAndLogin()
        {
            DoBorderWithTitle("Set up your account", () =>
            {
                if (GUILayout.Button("Login") && ValidateLogin())
                {
                    var myWindow = GetActiveWindow();
                    var carga = new AsyncCompletionSource();
                    SimvaConf.Local = new SimvaConf();

                    CoroutineRunner.Instance.RunRoutine(LoadSimvaConf(SimvaConf.Local.LoadAsync(), carga));
                    carga.Then(() =>
                    {
                        SimvaApi<ITeachersApi>.Login(true)
                                .Then(simvaController =>
                                {
                                    if (simvaController != null)
                                    {
                                        this.simvaController = simvaController;
                                        var apiClient = ((TeachersApi)this.simvaController.Api).ApiClient;
                                        apiClient.Authorization.RegisterAuthInfoUpdate(auth => PlayerPrefs.SetString("Simva.RefreshToken", auth.RefreshToken));
                                        PlayerPrefs.Save();
                                    }
                                })
                                .Catch(error =>
                                {
                                    Debug.Log(error.Message);
                                    Debug.LogException(error);
                                    EditorUtility.DisplayDialog("Simva: Error happened!", error.Message, "Ok");
                                })
                                .Finally(() =>
                                {
                                    BringWindowToTop(myWindow);
                                    isLogin = false;
                                });
                    });
                }

                return;
                /*
                // Username field
                registerUser = EditorGUILayout.TextField("Username", registerUser);
                // Password field
                registerPassword = EditorGUILayout.PasswordField("Password", registerPassword);
                // Email field
                email = EditorGUILayout.TextField("Email", email);
                // TOS Field
                tos = DoTermsAndConditionsField(tos);

                using (new EditorGUI.DisabledGroupScope(!tos))
                {
                    if (GUILayout.Button("Create account") && ValidateRegister())
                    {
                        isLogin = true;
                    //var randomUser = GenerateRandomBase58Key(32);
                    SimvaApi<ITeachersApi>.Register(registerUser, email, registerPassword, true)
                        .Then(registered =>
                        {
                            return registered ? SimvaApi<ITeachersApi>.LoginWithCredentials(registerUser, registerPassword) : null;
                        })
                        .Then(simvaController =>
                        {
                            if (simvaController != null)
                            {
                                this.simvaController = simvaController;
                                var apiClient = ((TeachersApi)this.simvaController.Api).ApiClient;
                                apiClient.onAuthorizationInfoUpdate += auth =>
                                {
                                    ProjectConfigData.setProperty("Simva.RefreshToken", auth.RefreshToken);
                                };
                                ProjectConfigData.setProperty("Simva.RefreshToken", apiClient.AuthorizationInfo.RefreshToken);
                                ProjectConfigData.storeToXML();
                            }
                        })
                        .Catch(error =>
                        {
                            Controller.Instance.ShowErrorDialog("Simva.Register.Failed.Title", "Simva.Register.Failed.Message");
                        })
                        .Finally(() => isLogin = false);
                    }
                }

                // Separator
                DoSeparator("Or");

                user = EditorGUILayout.TextField("User", user);
                password = EditorGUILayout.PasswordField("Password", password);
                if (GUILayout.Button("Login") && ValidateLogin())
                {
                    SimvaApi<ITeachersApi>.LoginWithCredentials(user, password)
                            .Then(simvaController =>
                            {
                                if (simvaController != null)
                                {
                                    this.simvaController = simvaController;
                                    var apiClient = ((TeachersApi)this.simvaController.Api).ApiClient;
                                    apiClient.Authorization.RegisterAuthInfoUpdate(auth => ProjectConfigData.setProperty("Simva.RefreshToken", auth.RefreshToken));
                                    ProjectConfigData.storeToXML();
                                }
                            })
                            .Catch(error =>
                            {
                                Controller.Instance.ShowErrorDialog("Simva.Register.Failed.Title", "Simva.Register.Failed.Message");
                            })
                            .Finally(() =>
                            {
                                isLogin = false;
                            });
                }*/
            });
        }

        private static void DoBorderWithTitle(string title, System.Action drawInside)
        {

            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(600)))
                {
                    GUILayout.FlexibleSpace();
                    DrawTitle(title);
                    GUILayout.Space(70);
                    drawInside();
                }

                GUILayout.Space(70);
                GUILayout.FlexibleSpace();
            }
            GUILayout.FlexibleSpace();
        }

        private static void DrawTitle(string title)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Box("<size=20><b>" + title + "</b></size>", titleStyle);
                GUILayout.FlexibleSpace();
            }
        }

        private static void DoSeparator(string content)
        {
            GUILayout.Space(50);
            GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(50));
            var lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.Space(50);
            var or = new GUIContent(content);
            var lineRect = lastRect;
            lineRect.height = 1;
            lineRect.center = lastRect.center;
            if (Event.current.type == EventType.Repaint)
            {
                GUI.skin.box.Draw(lineRect, GUIContent.none, false, false, false, false);
            }
            var labelSize = GUI.skin.label.CalcSize(or);
            var labelRect = new Rect { center = lastRect.center };
            labelRect.position -= labelSize / 2f;
            if (Event.current.type == EventType.Repaint)
            {
                GUI.skin.label.Draw(labelRect, or, false, false, false, false);
            }
        }

        private static bool DoTermsAndConditionsField(bool tos)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.FlexibleSpace();
                tos = EditorGUILayout.Toggle(tos, GUILayout.Width(14));
                EditorGUILayout.LabelField("Acepto los ", GUILayout.Width(63));
                if (GUILayout.Button("<color=#0000ffff>Términos y condiciones</color>", linkStyle, GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL("placeholder");
                }
                EditorGUILayout.LabelField("y la ", GUILayout.Width(24));
                if (GUILayout.Button("<color=#0000ffff>Política de Privacidad</color>", linkStyle, GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL("placeholder");
                }
                GUILayout.FlexibleSpace();
            }
            return tos;
        }

        private bool ValidateLogin()
        {
            return true;
        }

        private bool ValidateRegister()
        {
            return true;
        }

    }

}


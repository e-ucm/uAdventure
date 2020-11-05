using SimpleJSON;
using Simva;
using Simva.Api;
using Simva.Model;
using SimvaPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using uAdventure.Core;
using uAdventure.Editor;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace uAdventure.Simva
{
    [EditorWindowExtension(300, typeof(SimvaWindow))]
    public class SimvaWindow : DefaultButtonMenuEditorWindowExtension
    {
        [DllImport("User32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        protected TabsManager tabsManager;
        protected SimvaConf simvaConf;
        protected SimvaApi<ITeachersApi> simvaController;
        protected bool isLogin;

        private bool preTest, postTest, saveTraces, realTime, backup;
        private int preId, postId, participants;
        private string email, registerUser, registerPassword, user, password;
        private bool tos;
        private List<ActivityType> activityTypes;
        private IAsyncOperation loadingPromise;

        private static GUIStyle linkStyle;
        private static GUIStyle titleStyle;

        public SimvaWindow(Rect aStartPos, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Simva.Title")), aStyle, aOptions)
        { 
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("simva-icon"),
                text = "Simva"
            };

            tabsManager = new TabsManager(this);
            simvaConf = new SimvaConf();
            if(string.IsNullOrEmpty(simvaConf.Host))
            {
                Debug.LogWarning("No Simva Host!");
            }

            if (ProjectConfigData.existsKey("Simva.RefreshToken"))
            {
                Login();
            }

            if(linkStyle == null)
            {
                linkStyle = new GUIStyle(GUI.skin.label);
                linkStyle.richText = true;
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.richText = true;
            }
        }

        private void Login()
        {
            SimvaConf.Local = new SimvaConf();
            var carga = new AsyncCompletionSource();
            Observable.FromCoroutine(() => LoadSimvaConf(SimvaConf.Local.LoadAsync(), carga)).Subscribe();

            isLogin = true;
            carga.Then(() =>
            {
                IAsyncOperation<SimvaApi<ITeachersApi>> loginCall = null;
                if (ProjectConfigData.existsKey("Simva.RefreshToken"))
                {
                    loginCall = SimvaApi<ITeachersApi>.Login(new AuthorizationInfo
                    {
                        ClientId = "uadventure",
                        RefreshToken = ProjectConfigData.getProperty("Simva.RefreshToken") 
                    });
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
                    apiClient.onAuthorizationInfoUpdate += auth =>
                    {
                        ProjectConfigData.setProperty("Simva.RefreshToken", auth.RefreshToken);
                    };
                    ProjectConfigData.setProperty("Simva.RefreshToken", apiClient.AuthorizationInfo.RefreshToken);
                    ProjectConfigData.storeToXML();
                })
                    .Catch(error =>
                    {
                        ProjectConfigData.setProperty("Simva.RefreshToken", null);
                        ProjectConfigData.storeToXML();
                        Controller.Instance.ShowErrorDialog("Simva.Login.Error.Title", "Simva.Login.Error.Message");
                    })
                    .Finally(() => isLogin = false);
            });
        }

        private IEnumerator LoadSimvaConf(IEnumerator coroutine, IAsyncCompletionSource op)
        {
            yield return coroutine;
            op.SetCompleted();           
        }

        public override void Draw(int aID)
        {
            if (!tabsManager.Draw(aID))
            {
                if(simvaController == null)
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
                        Controller.Instance.ShowErrorDialog("Simva.ActivityTypes.Failed.Title", "Simva.ActivityTypes.Failed.Message");
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
                            Application.OpenURL((string)utils["url"]);
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
                            Application.OpenURL((string)utils["url"]);
                        }
                    }
                }

                DoSeparator(" Participants ");

                participants = Mathf.Clamp(1, EditorGUILayout.IntField("Participants", participants), 10000);


                if (GUILayout.Button("Create"))
                {
                    EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Creation.Info", 0);
                    JSONNode creationData = null;
                    var createStudy = simvaController.CreateStudyWithTestAndUsers("uAdventure", "uAdventure", DateTime.Now.ToString("dd-M-yyyy"), participants)
                    .Then(result => creationData = result);

                    createStudy.ProgressChanged += (sender, args) =>
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Creation.Info", createStudy.Progress);
                    };

                    EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Info", 0);
                    if (preTest)
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Info", 0.33f);
                        createStudy = createStudy.Then(() =>
                        {
                            return simvaController.Api.AddActivityToTest(creationData["studyId"], creationData["testId"], new Activity
                            {
                                Name = "PreTest",
                                Type = "limesurvey",
                                CopySurvey = preId.ToString() 
                            });
                        });
                    }

                    createStudy = createStudy.Then(() =>
                    {
                        EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Info", 0.66f);

                        return simvaController.Api.AddActivityToTest(creationData["studyId"], creationData["testId"], new Activity
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
                            EditorUtility.DisplayProgressBar("Simva.Study.Creation.Title", "Simva.Study.Activities.Info", 1f);
                            return simvaController.Api.AddActivityToTest(creationData["studyId"], creationData["testId"], new Activity
                            {
                                Name = "PostTest",
                                Type = "limesurvey",
                                CopySurvey = preId.ToString()
                            });
                        });
                    }

                    createStudy.Then(() =>
                    {
                        return simvaController.Api.GetStudy(creationData["studyId"]);
                    })
                    .Then(study =>
                    {
                        EditorUtility.ClearProgressBar();
                        simvaController.SimvaConf.Study = study.Id;
                        simvaController.SimvaConf.Save();
                    })
                    .Catch(error =>
                    {
                        Controller.Instance.ShowErrorDialog("Simva.Study.Error.Title", "Simva.Study.Error.Message");
                        var apiEx = error as ApiException;
                        if(apiEx != null)
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
                        if(completed == total)
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
                            Controller.Instance.ShowErrorDialog("No groups", "The study has no groups!");
                        }
                        else if (study.Groups.Count > 1)
                        {
                            Group[] groupNames = new Group[study.Groups.Count];
                            var promises = study.Groups.Select(g => simvaController.Api.GetGroup(g));
                            Paralell(promises)
                                .Then(groups =>
                                {
                                    Controller.Instance.ShowInputDialog("Select group", "Select one of the available groups", groups.Select((g,index) => index + ": " + g.Name).ToArray(), (sender, selected) =>
                                    {
                                        DownloadGroup(groups[int.Parse(selected.Split(':')[0])].Id);
                                    });
                                });

                        }
                        else
                        {
                            DownloadGroup(study.Groups[0]);
                        }
                    });

                }

                DoSeparator(" or ");

                if (GUILayout.Button("Set up a new study") && Controller.Instance.ShowStrictConfirmDialog("Warning", "Are you sure you wan't to unset the study?"))
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
                    SimvaApi<ITeachersApi>.Login(true)
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
                                Debug.Log(error.Message);
                                Controller.Instance.ShowErrorDialog("Simva.Register.Failed.Title", "Simva.Register.Failed.Message");
                            })
                            .Finally(() =>
                            {
                                BringWindowToTop(myWindow);
                                isLogin = false;
                            });
                }

                return;

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
                        /*SimvaApi<ITeachersApi>.Register(registerUser, email, registerPassword, true)
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
                            .Finally(() => isLogin = false);*/
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
                            .Finally(() =>
                            {
                                isLogin = false;
                            });
                }
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
        
        protected override void OnButton()
        {
            tabsManager.Reset();
        }

    }

}

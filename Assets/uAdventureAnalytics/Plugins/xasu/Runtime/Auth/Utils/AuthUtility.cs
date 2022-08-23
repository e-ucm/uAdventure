using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Xasu.Util;

namespace Xasu.Auth.Utils
{
    public static class AuthUtility
    {
        private const string responseContent =
            "<HTML>" +
                "<BODY onload=\"javascript: setTimeout('self.close()', 10); \">" +
                    "Please close this window and go back to the application" +
                "</BODY>" +
            "</HTML>";

        /// <summary>
        /// ListenForCallback is a workaround to receive OAuth and OAuth2 redirects.
        /// It behaves differently in different platforms:
        ///     - Editor: Will listen through HTTP server.
        ///     - Windows: Will listen through IPC for the custom URLScheme to be called.
        ///     - WebGL: Does not listen, just returns redirect URL and waits for normal OAuth/2 flow to redirect to the game.
        ///     - Linux and Mac: Not yet defined
        ///     - Android: 
        ///     - IOs:
        /// Once the callback is received, the application will stop listening. 
        /// Cancellation token is only used to abort the listening. 
        /// </summary>
        /// <param name="port">System port to listen to for Http/Ipc calls.</param>
        /// <param name="authListener">Listener to notify when the redirect is done.</param>
        /// <param name="cancelationToken">For aborting the Http/Ipc listeners.</param>
        /// <returns></returns>
        public static string ListenForCallback(int port, IAuthListener authListener, CancellationToken cancelationToken)
        {
            string redirectUrl = null;

            if (Application.isEditor)
            {
                Task.Run(async () =>
                {
                    HttpListener listener = new HttpListener();
                    listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
                    listener.Prefixes.Add("http://localhost:" + port + "/");
                    listener.Start();

                    // Note: The GetContext method blocks while waiting for a request. 
                    HttpListenerContext context = await listener.GetContextAsync();
                    HttpListenerRequest request = context.Request;

                    // Notify Listener
                    authListener.OnAuthReply(context.Request.QueryString);

                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseContent);

                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();

                    // Close the listener
                    listener.Close();
                }, cancelationToken);

                redirectUrl = "http://127.0.0.1:" + port + "/redirect";
            }
            /*else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                int ipcport = SimvaUriHandler.Start(uri =>
                {
                    Debug.Log("uri received!");
                    var query = uri.Split('?')[1].Split('&');
                    var d = new Dictionary<string, string>();
                    foreach (var q in query.Select(q => q.Split('=')))
                    {
                        d.Add(q[0], q[1]);
                    }

                    SimvaUriHandler.Stop();
                    result.SetResult(new LoginResponse
                    {
                        Code = d["code"],
                        SessionState = d["session_state"]
                    });
                });

                redirectUrl = Game.Instance.GameState.Data.getApplicationIdentifier() + "://" + ipcport + "/";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                var objectName = "OpenIdListener";
                var methodName = "OnAuthReply";
                var openIdListener = new GameObject(objectName, typeof(OpenIdListener)).GetComponent<OpenIdListener>();

                openIdListener.onAuthReceived += uri =>
                {
                    Debug.Log("uri received!");
                    var query = uri.Split('?')[1].Split('&');
                    var d = new Dictionary<string, string>();
                    foreach (var q in query.Select(q => q.Split('=')))
                    {
                        d.Add(q[0], q[1]);
                    }

                    GameObject.DestroyImmediate(openIdListener.gameObject);
                    result.SetResult(new LoginResponse
                    {
                        Code = d["code"],
                        SessionState = d["session_state"]
                    });
                };

                redirectUrl = Game.Instance.GameState.Data.getApplicationIdentifier() + "://" + objectName + "/" + methodName + "/";
            }
            else 
            */
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (WebGLUtility.IsWebGLListening())
                {
                    Debug.Log("Getting WebGLListener response from url: " + WebGLUtility.GetCompleteUrl());
                    WebGLUtility.SetWebGLListening(false);
                    string queryString = new Uri(WebGLUtility.GetCompleteUrl()).Query;
#if NET_4_6
                    var queryDictionary = UriHelper.DecodeQueryParameters(queryString);
#else
                    var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);
#endif
                    authListener.OnAuthReply(queryDictionary);
                }
                else
                {
                    WebGLUtility.SetWebGLListening(true);
                }

                redirectUrl = WebGLUtility.GetUrl();
            }
            /*else
            {
                throw new NotImplementedException(string.Format("Platform \"{0}\" is not yet implemented!", Application.platform));
            }*/

            return redirectUrl;
        }
        public static string Value(this IDictionary<string, string> data, string key)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data is null!");
            }

            if (!data.ContainsKey(key))
            {
                return null;
            }

            return data[key];
        }

        public static string GetRequiredValue(this IDictionary<string, string> data, string key, string missingMessage)
        {
            if(data == null)
            {
                throw new ArgumentNullException("data is null!");
            }

            if (!data.ContainsKey(key))
            {
                throw new MissingFieldException(string.Format(missingMessage, key));
            }

            return data[key];
        }

        public static void OpenUrl(string url)
        {
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                WebGLUtility.OpenUrl(url);
            }
            else
            {
                Application.OpenURL(url);
            }
        }
    }
}

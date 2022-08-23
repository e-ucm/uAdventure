using System;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Xasu.Util
{
    internal static class WebGLUtility
    {
        private const string UNITY_TRACKER_WEBGL_LISTENING = "unity_tracker_webgl_listening";

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        public static extern void OpenUrl(string url);

        [DllImport("__Internal")]
        public static extern string GetParameter(string name);

        [DllImport("__Internal")]
        public static extern void ClearUrl();

        [DllImport("__Internal")]
        public static extern string GetUrl();

        [DllImport("__Internal")]
        public static extern string GetCompleteUrl();
#else

        public static void OpenUrl(string url) { throw new NotSupportedException(); }
        public static string GetParameter(string name) { throw new NotSupportedException(); }
        public static void ClearUrl() { throw new NotSupportedException(); }
        public static string GetUrl() { throw new NotSupportedException(); }
        public static string GetCompleteUrl() { throw new NotSupportedException(); }

#endif

        public static bool IsWebGLListening()
        {
            return PlayerPrefs.HasKey(UNITY_TRACKER_WEBGL_LISTENING) && PlayerPrefs.GetInt(UNITY_TRACKER_WEBGL_LISTENING) == 1;
        }

        public static void SetWebGLListening(bool value)
        {
            PlayerPrefs.SetInt(UNITY_TRACKER_WEBGL_LISTENING, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}

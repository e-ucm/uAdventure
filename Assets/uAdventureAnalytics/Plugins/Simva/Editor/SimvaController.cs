using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;
using uAdventure.Core;
using uAdventure.Runner;
using UnityEditor.Build;
using UnityEditor.Callbacks;

namespace uAdventure.Simva
{
    public class SimvaController : IPreprocessBuild
    {
        private static SimvaController instance;
        public static SimvaController Instance
        {
            get { return instance ?? (instance = new SimvaController()); }
        }

        public int callbackOrder { get { return 1; } }

        public SimvaController()
        {
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            switch (target)
            {

                // Windows
                case BuildTarget.StandaloneWindows:
                    break;
                case BuildTarget.StandaloneWindows64:
                    break;

                // Mac OS X
                case BuildTarget.StandaloneOSX:
                    break;

                // Linux
                case BuildTarget.StandaloneLinux:
                    break;
                case BuildTarget.StandaloneLinux64:
                    break;

                // iOS
                case BuildTarget.iOS:
                    break;

                // Android
                case BuildTarget.Android:
                    break;

                // WebGL
                case BuildTarget.WebGL:
                    break;
            }
        }

        [PostProcessBuild(0)] 
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            switch (target)
            {

                // Windows
                case BuildTarget.StandaloneWindows:
                    break;
                case BuildTarget.StandaloneWindows64:
                    break;

                // Mac OS X
                case BuildTarget.StandaloneOSX:
                    {
                        var infoPlist = new UnityEditor.iOS.Xcode.Custom.PlistDocument();
                        var infoPlistPath = pathToBuiltProject + "/Contents/Info.plist";
                        infoPlist.ReadFromFile(infoPlistPath);

                        // Register ios URL scheme for external apps to launch this app.
                        var urlTypeDict = infoPlist.root.CreateArray("CFBundleURLTypes").AddDict();
                        urlTypeDict.SetString("CFBundleURLName", PlayerSettings.applicationIdentifier);
                        var urlSchemes = urlTypeDict.CreateArray("CFBundleURLSchemes");
                        urlSchemes.AddString(PlayerSettings.applicationIdentifier);
                        infoPlist.WriteToFile(infoPlistPath);
                    }
                    break;

                // Linux
                case BuildTarget.StandaloneLinux:
                    break;
                case BuildTarget.StandaloneLinux64:
                    break;

                // iOS
                case BuildTarget.iOS:
                    {
                        var infoPlist = new UnityEditor.iOS.Xcode.Custom.PlistDocument();
                        var infoPlistPath = pathToBuiltProject + "/Info.plist";
                        infoPlist.ReadFromFile(infoPlistPath);

                        // Register ios URL scheme for external apps to launch this app.
                        var urlTypeDict = infoPlist.root.CreateArray("CFBundleURLTypes").AddDict();
                        urlTypeDict.SetString("CFBundleURLName", PlayerSettings.applicationIdentifier);
                        var urlSchemes = urlTypeDict.CreateArray("CFBundleURLSchemes");
                        urlSchemes.AddString(PlayerSettings.applicationIdentifier);
                        infoPlist.WriteToFile(infoPlistPath);
                    }

                    break;

                // Android
                case BuildTarget.Android:
                    break;

                // WebGL
                case BuildTarget.WebGL:
                    break;
            }


            }
        }
}

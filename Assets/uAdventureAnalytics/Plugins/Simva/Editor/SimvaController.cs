using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using WixSharp;
using System.Windows.Forms;
using WixSharp.CommonTasks;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;
using System.Xml;
using System.Xml.Linq;
using uAdventure.Editor;
using System.Collections.Generic;

namespace uAdventure.Simva
{
    public class SimvaController : IPreprocessBuildWithReport
    {
        private static readonly string WIXSHARP_URL = "https://github.com/e-ucm/uAdventure-WIXSHARP/releases/download/v1.15.0.0/WixSharp.1.15.0.0.zip";

        private static SimvaController instance;
        public static SimvaController Instance
        {
            get { return instance ?? (instance = new SimvaController()); }
        }

        public int callbackOrder { get { return 1; } }

        public SimvaController()
        {
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            switch (report.summary.platform)
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
                case BuildTarget.StandaloneLinux64:
                    break;

                // iOS
                case BuildTarget.iOS:
                    break;

                // Android
                case BuildTarget.Android:
                    var docPath = @"Assets\uAdventureAnalytics\Plugins\Simva\Plugins\Android\AndroidManifest.xml";
                    var finalPath = @"Assets/Plugins/Android/AndroidManifest.xml";
                    XDocument doc = XDocument.Load(docPath);
                    Debug.Log("Loaded!");

                    XElement manifest = doc.FirstNode as XElement;
                    XElement application = manifest.Element(XName.Get("application"));
                    var activity = application.Elements(XName.Get("activity")).Where(a => a.Attributes().Any(at => at.Value == "org.identitymodel.unityclient.AuthRedirectActivity")).FirstOrDefault();

                    XElement intentFilter = activity.Element(XName.Get("intent-filter"));
                    var data = intentFilter.Element(XName.Get("data"));
                    foreach (var attr in data.Attributes())
                    {
                        if (attr.Name.ToString() == @"{http://schemas.android.com/apk/res/android}scheme")
                        {
                            attr.Value = PlayerSettings.applicationIdentifier;
                        }
                    }

                    if (!Directory.Exists("Assets/Plugins"))
                    {
                        Directory.CreateDirectory("Assets/Plugins");
                    }

                    if (!Directory.Exists("Assets/Plugins/Android"))
                    {
                        Directory.CreateDirectory("Assets/Plugins/Android");
                    }

                    if (System.IO.File.Exists(finalPath))
                    {
                        EditorUtility.DisplayDialog("Manifest found!", "The project already contains a AndroidManifest.xml file. For Simva to work" +
                            "you must use the AndroidManifest.xml in the Assets/uAdventureAnalytics/Simva/Plugins/Android folder.", "Ok");
                        throw new System.Exception("Multiple manifests found!");
                    }
                    else
                    {
                        using (var writer = XmlWriter.Create(finalPath))
                        {
                            doc.WriteTo(writer);
                            Debug.Log("Written!");
                        }
                    }


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

                    if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\wixsharp"))
                    {
                        Controller.DownloadDependencyZip("WixSharp", "/wixsharp", WIXSHARP_URL, downloaded =>
                        {
                            if (downloaded)
                            {
                                CreateMsiInstaller(pathToBuiltProject);
                            }
                        });
                    }
                    else
                    {
                        CreateMsiInstaller(pathToBuiltProject);
                    }


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

                    var finalPath = @"Assets/Plugins/Android/AndroidManifest.xml";
                    System.IO.File.Delete(finalPath);
                    break;

                // WebGL
                case BuildTarget.WebGL:
                    break;
            }
        }

        private static void CreateMsiInstaller(string pathToBuiltProject)
        {
            EditorUtility.DisplayProgressBar("Creating Installers", "Preparing Wix project...", 0f);
            Compiler.WixLocation = Directory.GetCurrentDirectory() + "\\wixsharp\\Wix_bin\\bin";

            var path = new FileInfo(pathToBuiltProject).Directory.FullName;
            var fullSetup = new Feature(PlayerSettings.productName + " Binaries");
            var productFolder = PlayerSettings.companyName + @"\" + PlayerSettings.productName;

            // Create the project contents
            var projectContents = GetDirContents(path).ToList();

            // Add the unninstall shortcut
            projectContents.Add(new ExeFileShortcut("Uninstall " + PlayerSettings.productName, "[System64Folder]msiexec.exe", "/x [ProductCode]") { WorkingDirectory = "[INSTALLDIR]" });

            // Create the project structure
            var project = new Project(PlayerSettings.productName,

                // Program files folder
                new Dir(@"%ProgramFiles%\" + productFolder, projectContents.ToArray()),

                // Program menu folder
                new Dir(@"%ProgramMenu%\" + productFolder,
                    new ExeFileShortcut("Uninstall " + PlayerSettings.productName , "[System64Folder]msiexec.exe", "/x [ProductCode]") { WorkingDirectory = "[INSTALLDIR]" },
                    new ExeFileShortcut(PlayerSettings.productName, "[INSTALLDIR]" + PlayerSettings.productName + ".exe", arguments: "") { WorkingDirectory = "[INSTALLDIR]" }),

                // Desktop (main shortcut)
                new Dir(@"%Desktop%",
                    new ExeFileShortcut(PlayerSettings.productName, "[INSTALLDIR]" + PlayerSettings.productName + ".exe", arguments: "") { WorkingDirectory = "[INSTALLDIR]" }))
            {
                GUID = new System.Guid(PlayerSettings.productGUID.ToString())
            };

            // Add the registry keys for opening the game through URI Protocol
            string identifier = PlayerSettings.applicationIdentifier;
            project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + identifier,
                new RegValue("", "URL " + PlayerSettings.productName + " Link"),
                new RegValue("URL Protocol", PlayerSettings.productName + " Protocol")));
            project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + identifier + @"\shell"));
            project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + identifier + @"\shell\open"));
            project.AddRegValue(new RegValue(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + identifier
                + @"\shell\open\command", "", "\"[INSTALLDIR]" + PlayerSettings.productName + ".exe -batchmode -nographics \" \"%1\""));

            if (!Directory.Exists("Installers"))
            {
                Directory.CreateDirectory("Installers");
            }

            project.OutDir = "Installers/";
            project.OutFileName = PlayerSettings.productName;
            project.UI = WUI.WixUI_InstallDir; 
            project.Platform = Platform.x64;
            EditorUtility.DisplayProgressBar("Creating Installers", "Building Msi installer...", 0.1f);
            project.BuildMsi();
            EditorUtility.DisplayProgressBar("Creating Installers", "Done!", 1f);
        }

        private static WixEntity[] GetDirContents(string installPath, string folder = "")
        {
            var folderPath = installPath + @"\" + folder;

            List<WixEntity> contents = new List<WixEntity>();

            foreach (var file in Directory.GetFiles(folderPath))
            {
                contents.Add(new WixSharp.File(file));
            }

            foreach (var directory in Directory.GetDirectories(folderPath).Select(d => new DirectoryInfo(d)))
            {
                contents.Add(new Dir(directory.Name, GetDirContents(installPath, folder + directory.Name + @"\")));
            }

            return contents.ToArray();
        }
    }
}

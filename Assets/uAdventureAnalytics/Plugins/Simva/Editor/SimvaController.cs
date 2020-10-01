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

namespace uAdventure.Simva
{
    public class SimvaController : IPreprocessBuildWithReport
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

        private static Dir GetDirContents(string prepend, string localPath, string folder, bool prependIsBase = true)
        {
            var localFolder = prependIsBase ? localPath + folder : localPath + prepend + folder;

            var dir = new Dir(prependIsBase ? prepend + folder : folder);

            foreach (var file in Directory.GetFiles(localFolder))
            {
                dir.AddFile(new WixSharp.File(file));
            }

            foreach(var directory in Directory.GetDirectories(localFolder).Select(d => new DirectoryInfo(d)))
            {
                dir.AddDir(GetDirContents(prependIsBase ? folder : prepend + "\\" + folder + "\\", localPath, directory.Name, false));
            }

            return dir;
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
                    Compiler.WixLocation = "C:\\Users\\Victor\\Documents\\uAdventure\\wixsharp\\Wix_bin\\bin";

                    var path = new FileInfo(pathToBuiltProject).Directory.FullName;

                    var fullSetup = new Feature(PlayerSettings.productName + " Binaries");

                    var project = new Project(PlayerSettings.productName,
                        GetDirContents(@"%ProgramFiles%\" + PlayerSettings.companyName + @"\" + PlayerSettings.productName, path, "\\"))
                    {
                        GUID = new System.Guid(PlayerSettings.productGUID.ToString())
                    };

                    project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier,
                        new RegValue("", "URL " + PlayerSettings.productName + " Link"),
                        new RegValue("URL Protocol", PlayerSettings.productName + " Protocol")));
                    project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier + @"\shell"));
                    project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier + @"\shell\open"));
                    project.AddRegValue(new RegValue(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier 
                        + @"\shell\open\command", "", "\"[INSTALLDIR]" + PlayerSettings.productName +".exe\" \"%1\""));

                    if (!Directory.Exists("Installers"))
                    {
                        Directory.CreateDirectory("Installers");
                    }

                    project.OutDir = "Installers/";
                    project.OutFileName = PlayerSettings.productName + ".msi";
                    project.BuildMsiCmd();
                    //Compiler.BuildMsi(project);

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
    }
}

using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using WixSharp;
using WixSharp.CommonTasks;

public class InstallerCreatorPostProcessor : IPreprocessBuildWithReport
{
    public static readonly string WINDOWS_WIXSHARP_URL = "https://github.com/e-ucm/uAdventure-WIXSHARP/releases/download/v1.15.0.0/WixSharp.1.15.0.0.zip";

    public int callbackOrder => 1;

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

                switch (SystemInfo.operatingSystemFamily)
                {
                    case OperatingSystemFamily.Windows:

                        var projectPath = Directory.GetCurrentDirectory().Replace("\\", "/");
                        var wixsharpPath = projectPath + "/WIXSHARP";
                        if (!Directory.Exists(wixsharpPath))
                            Directory.CreateDirectory(wixsharpPath);

                        if (!System.IO.File.Exists(wixsharpPath + "/Wix_bin/bin/Wix.dll"))
                        {
                            var downloaded = PackageDownloader.Instance.DownloadPackage("WixSharp", "/WIXSHARP", WINDOWS_WIXSHARP_URL)
                                .GetAwaiter().GetResult();

                            if(downloaded)
                                CreateWindowsInstaller(pathToBuiltProject);
                        }
                        else
                        {
                            CreateWindowsInstaller(pathToBuiltProject);
                        }
                        break;
                    default:
                        EditorUtility.DisplayDialog("Not supported!", "Creating installers for native Windows URISchemes (to enable CMI-5 support)" +
                            " is only supported in Windows! Please retry the process in a Windows device.", "Ok");
                        break;
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

                    infoPlist.root.SetString("NSCameraUsageDescription", "Camera is required to read the QR codes in each recycling container.");
                    infoPlist.root.SetString("NSLocationWhenInUseUsageDescription", "Location is used to position the player in the in-game map.");
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

    private static void CreateWindowsInstaller(string pathToBuiltProject)
    {
        var projectPath = Directory.GetCurrentDirectory();

        Compiler.WixLocation = projectPath + "\\WIXSHARP\\Wix_bin\\bin";
        var path = new FileInfo(pathToBuiltProject).Directory.FullName;


        var fullSetup = new Feature(PlayerSettings.productName + " Binaries");

        var project = new Project(PlayerSettings.productName,
            GetDirContents(@"%ProgramFilesFolder%\" + PlayerSettings.companyName + @"\" + PlayerSettings.productName, path, "\\"))
        {
            GUID = new System.Guid(PlayerSettings.productGUID.ToString())
        };

        project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier,
            new RegValue("", "URL " + PlayerSettings.productName + " Link"),
            new RegValue("URL Protocol", PlayerSettings.productName + " Protocol")));
        project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier + @"\shell"));
        project.AddRegKey(new RegKey(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier + @"\shell\open"));
        project.AddRegValue(new RegValue(fullSetup, RegistryHive.CurrentUser, @"Software\Classes\" + PlayerSettings.applicationIdentifier
        //    + @"\shell\open\command", "", "\"[INSTALLDIR]Launcher.exe\" \"%1\""));
            + @"\shell\open\command", "", "\"[INSTALLDIR]" + PlayerSettings.productName + ".exe\" -cmi5 \"%1\""));

        if (!Directory.Exists("Installers"))
        {
            Directory.CreateDirectory("Installers");
        }

        project.OutDir = "Installers/";
        project.OutFileName = PlayerSettings.productName;
        project.BuildMsi();
    }
    private static Dir GetDirContents(string prepend, string localPath, string folder, bool prependIsBase = true)
    {
        var localFolder = prependIsBase ? localPath + folder : localPath + prepend + folder;

        var dir = new Dir(prependIsBase ? prepend + folder : folder);

        foreach (var file in Directory.GetFiles(localFolder))
        {
            dir.AddFile(new WixSharp.File(file));
        }

        foreach (var directory in Directory.GetDirectories(localFolder).Select(d => new DirectoryInfo(d)))
        {
            dir.AddDir(GetDirContents(prependIsBase ? folder : prepend + "\\" + folder + "\\", localPath, directory.Name, false));
        }

        return dir;
    }
}

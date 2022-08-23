using UnityEngine;
using UnityEditor;
using uAdventure;
using uAdventure.Editor;
using System;
using BuildConfigs = uAdventure.Editor.Controller.BuildConfigs;
using System.IO;
using uAdventure.Core;

public class BuildWindow : EditorWindow {

    public event EventHandler OnConfigSelected;

    private FileChooser pathSelector;
    private GUIContent windowsIcon, linuxIcon, macOSIcon, androidIcon, iOSIcon, webGLIcon;

    public static BuildWindow CreateBuildWindow()
    {
        var newWindow = CreateInstance<BuildWindow>();
        newWindow.Show();
        if (!Directory.Exists("./Builds/"))
        {
            Directory.CreateDirectory("./Builds/");
        }
        return newWindow;
    }

    protected void OnEnable()
    {
        pathSelector = new FileChooser()
        {
            Label = "Path",
            FileType = FileType.PATH
        };

        windowsIcon = new GUIContent("Windows", EditorGUIUtility.IconContent("BuildSettings.Metro").image);
        linuxIcon = new GUIContent("Linux", Resources.Load<Texture2D>("EAdventureData/build-icons/linux"));
        macOSIcon = new GUIContent("Mac Os X", Resources.Load<Texture2D>("EAdventureData/build-icons/macosx"));
        androidIcon = new GUIContent("Android", EditorGUIUtility.IconContent("BuildSettings.Android").image);
        iOSIcon = new GUIContent("iPhone", EditorGUIUtility.IconContent("BuildSettings.iPhone").image);
        webGLIcon = new GUIContent("WebGL", EditorGUIUtility.IconContent("BuildSettings.WebGL").image);
    }

    protected void OnGUI()
    {
        GUILayout.Label("Platforms", "OL title", new GUILayoutOption[0]);
        GUILayout.BeginHorizontal();
        BuildConfigs.BuildWindows = GUILayout.Toggle(BuildConfigs.BuildWindows, windowsIcon.image, "Button");
        BuildConfigs.BuildMacOsX = GUILayout.Toggle(BuildConfigs.BuildMacOsX, macOSIcon.image, "Button");
        BuildConfigs.BuildLinux = GUILayout.Toggle(BuildConfigs.BuildLinux, linuxIcon.image, "Button");
        BuildConfigs.BuildAndroid = GUILayout.Toggle(BuildConfigs.BuildAndroid, androidIcon.image, "Button");
        BuildConfigs.BuildIOS = GUILayout.Toggle(BuildConfigs.BuildIOS, iOSIcon.image, "Button");
        BuildConfigs.BuildWebGL = GUILayout.Toggle(BuildConfigs.BuildWebGL, webGLIcon.image, "Button");
        GUILayout.EndHorizontal();

        GUILayout.Label("Folder", "OL title", new GUILayoutOption[0]);
        pathSelector.Path = BuildConfigs.path ?? Path.GetFullPath("./Builds/");
        pathSelector.DoLayout(GUILayout.ExpandWidth(true));
        BuildConfigs.path = pathSelector.Path.Replace("\\", "/");

        GUILayout.Label("Information", "OL title", new GUILayoutOption[0]);
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical("Box");
            {
                PlayerSettings.fullScreenMode = (FullScreenMode)EditorGUILayout.EnumPopup("Fullscreen Mode", PlayerSettings.fullScreenMode);
                PlayerSettings.productName = EditorGUILayout.TextField("Game Name", PlayerSettings.productName);
                PlayerSettings.companyName = EditorGUILayout.TextField("Author", PlayerSettings.companyName);
                PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Version number", PlayerSettings.Android.bundleVersionCode);
                PlayerSettings.applicationIdentifier = EditorGUILayout.TextField("Package name", PlayerSettings.applicationIdentifier);

                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, PlayerSettings.applicationIdentifier);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PlayerSettings.applicationIdentifier);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, PlayerSettings.applicationIdentifier);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, PlayerSettings.applicationIdentifier);
                Controller.Instance.AdventureData.setApplicationIdentifier(PlayerSettings.applicationIdentifier);
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancelar"))
        {
            Close();
        }
        if (GUILayout.Button("Exportar"))
        {
            var exportConfig = BuildConfigs.Instanciate();
            Close();
            OnConfigSelected(this, new ExportConfigSelectedEventArgs(exportConfig));
        }
        GUILayout.EndHorizontal();
    }
}

internal class ExportConfigSelectedEventArgs : EventArgs
{
    public Controller.BuildConfig exportConfig;

    public ExportConfigSelectedEventArgs(Controller.BuildConfig exportConfig)
    {
        this.exportConfig = exportConfig;
    }
}
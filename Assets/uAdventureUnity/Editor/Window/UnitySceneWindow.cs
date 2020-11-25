using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using uAdventure.Editor;
using UnityEditorInternal;
using System.Linq;
using UnityEditor;
using uAdventure.Core;

namespace uAdventure.Unity
{
    [EditorWindowExtension(600, typeof(UnitySceneDataControl))] 
    public class UnitySceneWindow : DataControlListEditorWindowExtension
    {
        public UnitySceneWindow(Rect rect, params GUILayoutOption[] options) : base(rect, options)
        {
            Init();
        }
        public UnitySceneWindow(Rect rect, GUIContent content, params GUILayoutOption[] options) : base(rect, content, options)
        {
            Init();
        }

        public UnitySceneWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options) : base(rect, style, options)
        {
            Init();
        }

        public UnitySceneWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            Init();
        }


        private void Init()
        {
            dataControlList.SetData(UnityScenePluginController.Instance.UnityScenes,
                l => (l as ListDataControl<ChapterDataControl, UnitySceneDataControl>)
                .DataControls
                .Cast<DataControl>()
                .ToList());
            this.Content = new GUIContent("Unity Scenes");
            this.ButtonContent = new GUIContent("Unity Scenes", Resources.Load<Texture2D>("unitylogo"));
        }


        public override void Draw(int aID)
        {
            if (dataControlList.index != -1)
            {
                var unitySceneDataControl = dataControlList.list[dataControlList.index] as UnitySceneDataControl;

                EditorGUI.BeginChangeCheck();
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(unitySceneDataControl.Scene);
                var newScene = EditorGUILayout.ObjectField("Scene", scene, typeof(SceneAsset), true);
                if (EditorGUI.EndChangeCheck())
                {
                    unitySceneDataControl.Scene = AssetDatabase.GetAssetOrScenePath(newScene);
                }
                var isInBuildSettings = IsInBuildSettings(newScene);
                if (isInBuildSettings)
                {
                    EditorGUILayout.HelpBox("UnityPlugin.SceneWindow.InBuildSettings".Traslate(), MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("UnityPlugin.SceneWindow.SceneNotInBuildSettings".Traslate(), MessageType.Error);
                }
                using (new EditorGUI.DisabledScope(!newScene || isInBuildSettings))
                {
                    if (GUILayout.Button("UnityPlugin.SceneWindow.AddSceneToBuildSettings".Traslate()))
                    {
                        var buildSettingsScene = new EditorBuildSettingsScene(unitySceneDataControl.Scene, true);
                        EditorBuildSettings.scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes) { buildSettingsScene }.ToArray();
                    }
                }
            }
            else
            {

                GUILayout.Label("Select an scene in the left");
                var scene1GUID = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("_Scene1")[0]);
                var scene1 = AssetDatabase.LoadAssetAtPath(scene1GUID, typeof(SceneAsset));
                var isInBuildSettings = IsInBuildSettings(scene1);
                if (isInBuildSettings)
                {
                    EditorGUILayout.HelpBox("UnityPlugin.SceneWindow.MainSceneInBuildSettings".Traslate(), MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("UnityPlugin.SceneWindow.MainSceneNotInBuildSettings".Traslate(), MessageType.Error);
                }
                using (new EditorGUI.DisabledScope(!scene1 || isInBuildSettings))
                {
                    if (GUILayout.Button("UnityPlugin.SceneWindow.AddSceneToBuildSettings".Traslate()))
                    {
                        var buildSettingsScene = new EditorBuildSettingsScene(scene1GUID, true);
                        EditorBuildSettings.scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes) { buildSettingsScene }.ToArray();
                    }
                }
            }
        }

        private static bool IsInBuildSettings(Object newScene)
        {
            return newScene && EditorBuildSettings.scenes.Any(s => s.path == AssetDatabase.GetAssetOrScenePath(newScene));
        }

        protected override void OnSelect(ReorderableList r)
        {
            UnityScenePluginController.Instance.SelectedUnityScene = r.index;
        }
    }
}

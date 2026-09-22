using UnityEditor;
using UnityEngine;

namespace uAdventure.Simva
{
    [InitializeOnLoad]
    public static class SimvaPlayModeSettingsApplier
    {
        static SimvaPlayModeSettingsApplier()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                ApplySettingsOnPlayModeEnter();
            }
        }

        private static void ApplySettingsOnPlayModeEnter()
        {
            var settingsPath = "Assets/Resources/SimvaPluginSettings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<SimvaPluginSettings>(settingsPath);

            if (settings == null)
            {
                return;
            }

            var extension = Object.FindObjectOfType<SimvaExtension>();
            if (extension == null)
            {
                return;
            }

            extension.ApplySettings(settings);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Simva
{
    public class SimvaPluginConfigurationWindow : LayoutWindow
    {
        private SimvaPluginSettings settings;
        private string[] languageOptions;
        private Vector2 scrollPosition;

        public SimvaPluginConfigurationWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style, options)
        {
        }

        public override void Draw(int aID)
        {
            if (settings == null)
            {
                LoadSettings();
            }

            if (languageOptions == null || languageOptions.Length == 0)
            {
                LoadLanguageOptions();
            }

            using (var scope = new GUILayout.ScrollViewScope(scrollPosition, Options))
            {
                scrollPosition = scope.scrollPosition;
                EditorGUIUtility.labelWidth = Rect.width - 30;

                EditorGUILayout.LabelField("Simva Plugin Configuration", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                DrawGeneralSettings();
                DrawLanguageSettings();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
                {
                    SaveSettings();
                    EditorUtility.DisplayDialog("Simva", TC.get("Simva.SettingsSaved"), "OK");
                }
            }
        }

        private void DrawGeneralSettings()
        {
            EditorGUILayout.LabelField(TC.get("Simva.Tab.Configuration"), EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            settings.SaveAuthUntilCompleted = EditorGUILayout.Toggle(TC.get("Simva.SaveAuthUntilCompleted"), settings.SaveAuthUntilCompleted);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUI.BeginChangeCheck();
            settings.RunGameIfSimvaIsNotConfigured = EditorGUILayout.Toggle(TC.get("Simva.RunGameIfSimvaIsNotConfigured"), settings.RunGameIfSimvaIsNotConfigured);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUI.BeginChangeCheck();
            settings.ContinueOnQuit = EditorGUILayout.Toggle(TC.get("Simva.ContinueOnQuit"), settings.ContinueOnQuit);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUI.BeginChangeCheck();
            settings.EnableLoginDemoButton = EditorGUILayout.Toggle(TC.get("Simva.EnableLoginDemoButton"), settings.EnableLoginDemoButton);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUI.BeginChangeCheck();
            settings.SaveDisclaimerAccepted = EditorGUILayout.Toggle(TC.get("Simva.SaveDisclaimerAccepted"), settings.SaveDisclaimerAccepted);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUI.BeginChangeCheck();
            settings.BasicScormXAPIDataManagementByGame = EditorGUILayout.Toggle(TC.get("Simva.BasicScormXAPIDataManagementByGame"), settings.BasicScormXAPIDataManagementByGame);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUI.BeginChangeCheck();
            settings.EnableDebugLogging = EditorGUILayout.Toggle(TC.get("Simva.EnableDebugLogging"), settings.EnableDebugLogging);
            if (EditorGUI.EndChangeCheck()) MarkDirty();

            EditorGUILayout.Space();
        }

        private void DrawLanguageSettings()
        {
            EditorGUILayout.LabelField(TC.get("Simva.SelectedLanguages"), EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var currentIndex = string.IsNullOrEmpty(settings.LanguageByDefault) ? -1 : System.Array.IndexOf(languageOptions, settings.LanguageByDefault);
            if (currentIndex < 0 && languageOptions.Length > 0) currentIndex = 0;
            var newIndex = EditorGUILayout.Popup(TC.get("Simva.LanguageByDefault"), currentIndex, languageOptions);
            if (EditorGUI.EndChangeCheck() && newIndex >= 0 && newIndex < languageOptions.Length)
            {
                settings.LanguageByDefault = languageOptions[newIndex];
                MarkDirty();
            }

            EditorGUILayout.Space();
        }

        private void LoadSettings()
        {
            var guids = AssetDatabase.FindAssets("t:SimvaPluginSettings");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                settings = AssetDatabase.LoadAssetAtPath<SimvaPluginSettings>(path);
            }

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<SimvaPluginSettings>();
                var path = "Assets/Resources/SimvaPluginSettings.asset";
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void SaveSettings()
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            ApplyToExtension();
        }

        private void ApplyToExtension()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var extension = Object.FindObjectOfType<SimvaExtension>();
            if (extension != null)
            {
                extension.ApplySettings(settings);
            }
        }

        private void LoadLanguageOptions()
        {
            TextAsset[] allLanguageMarkers = Resources.LoadAll<TextAsset>("Localization");

            Dictionary<string, string> languages = new Dictionary<string, string>();
            foreach (TextAsset asset in allLanguageMarkers)
            {
                if (asset.name == "lang")
                {
                    JObject jObject = JObject.Parse(asset.text);
                    var code = "";
                    var name = "";
                    foreach (var entry in jObject)
                    {
                        if (entry.Key == "code")
                        {
                            code = (string)entry.Value;
                        }
                        if (entry.Key == "displayName")
                        {
                            name = (string)entry.Value;
                        }
                    }
                    var modifName = name + " [" + code + "]";
                    if (!languages.ContainsKey(code))
                    {
                        languages.Add(code, modifName);
                    }
                }
            }
            languageOptions = languages.Values
                .Distinct()
                .OrderBy(n => n)
                .ToArray();

            if (languageOptions.Length > 0 && settings != null)
            {
                if (string.IsNullOrEmpty(settings.LanguageByDefault))
                {
                    settings.LanguageByDefault = languageOptions[0];
                }
            }
        }

        private void MarkDirty()
        {
            if (settings != null)
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}

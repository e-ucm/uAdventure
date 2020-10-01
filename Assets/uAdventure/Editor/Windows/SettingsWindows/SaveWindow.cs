using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SaveWindow : DefaultButtonMenuEditorWindowExtension
    {
        public SaveWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
            ButtonContent = new GUIContent("Storage");
        }

        public override void Draw(int aID)
        {
            EditSaveMode(Controller.Instance.AdventureData, Rect);
        }

        protected override void OnButton()
        {
        }

        private static void EditSaveMode(AdventureDataControl adventureData, Rect rect)
        {
            bool newValue;
            using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
            {
                EditorGUIUtility.labelWidth = rect.width - 30;

                EditorGUILayout.LabelField(TC.get("MenuAdventure.SaveMode"), EditorStyles.boldLabel);

                EditorGUILayout.HelpBox(TC.get("MenuAdventure.SaveMode.Info"), MessageType.Warning);

                if (CheckedField("MenuAdventure.ShowSaveLoad", adventureData.isShowSaveLoad(), out newValue))
                {
                    adventureData.setShowSaveLoad(newValue);
                }

                if (CheckedField("MenuAdventure.ShowReset", adventureData.isShowReset(), out newValue))
                {
                    adventureData.setShowReset(newValue);
                }
                if (CheckedField("MenuAdventure.AutoSave", adventureData.isAutoSave(), out newValue))
                {
                    adventureData.setAutoSave(newValue);
                }

                if (CheckedField("MenuAdventure.SaveOnSuspend", adventureData.isSaveOnSuspend(), out newValue))
                {
                    adventureData.setSaveOnSuspend(newValue);
                }

                if (CheckedField("MenuAdventure.RestoreAfterOpen", adventureData.isRestoreAfterOpen(), out newValue))
                {
                    adventureData.setRestoreAfterOpen(newValue);
                }
            }
        }

        private static bool CheckedField(string name, bool value, out bool newValue)
        {
            EditorGUILayout.Space(20);
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUILayout.Toggle(TC.get(name + ".Checkbox"), value, GUILayout.ExpandWidth(true));
            EditorGUILayout.HelpBox(TC.get(name + ".Info"), MessageType.Info);
            return EditorGUI.EndChangeCheck();
        }
    }
}

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
            using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
            {
                EditorGUIUtility.labelWidth = rect.width - 30;
                EditorGUILayout.LabelField(TC.get("MenuAdventure.SaveMode"), EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                var newAutoSave = EditorGUILayout.Toggle(TC.get("MenuAdventure.AutoSave.Checkbox"), adventureData.isAutoSave(), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setAutoSave(newAutoSave);
                }

                EditorGUI.BeginChangeCheck();
                var newSaveOnSuspend = EditorGUILayout.Toggle(TC.get("MenuAdventure.SaveOnSuspend.Checkbox"), adventureData.isSaveOnSuspend(), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setSaveOnSuspend(newSaveOnSuspend);
                }

                EditorGUI.BeginChangeCheck();
                var newRestoreAfterOpen = EditorGUILayout.Toggle(TC.get("MenuAdventure.RestoreAfterOpen.Checkbox"), adventureData.isSaveOnSuspend(), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setSaveOnSuspend(newRestoreAfterOpen);
                }
            }
        }
    }
}

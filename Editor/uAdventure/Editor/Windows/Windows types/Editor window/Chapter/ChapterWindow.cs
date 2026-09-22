using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChapterWindow : DefaultButtonMenuEditorWindowExtension
    {
        public ChapterWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            ButtonContent = new GUIContent("Chapter");
        }

        protected override void OnButton()
        {
        }

        public override void Draw(int aID)
        {
            var chapter = Controller.Instance.SelectedChapterDataControl;
            
            var targetNames = Controller.Instance.IdentifierSummary.getIds<IChapterTarget>();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(TC.get("Chapter.InitialScene"));
            var initial = EditorGUILayout.Popup(System.Array.FindIndex(targetNames, chapter.getInitialScene().Equals), targetNames);
            if (EditorGUI.EndChangeCheck())
            {
                chapter.setInitialScene(targetNames[initial]);
            }
            GUILayout.Space(20);

            GUILayout.Label(TC.get("Chapter.Title"));
            EditorGUI.BeginChangeCheck();
            var title = EditorGUILayout.TextField(chapter.getTitle());
            if (EditorGUI.EndChangeCheck())
            {
                chapter.setTitle(title);
            }

            GUILayout.Space(20);
            GUILayout.Label(TC.get("Chapter.Description"));
            EditorGUI.BeginChangeCheck();
            var description = EditorGUILayout.TextArea(chapter.getDescription(), GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                chapter.setDescription(description);
            }

        }
    }
}
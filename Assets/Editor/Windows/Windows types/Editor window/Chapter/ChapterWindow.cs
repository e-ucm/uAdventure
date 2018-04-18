using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChapterWindow : LayoutWindow
    {
        private Texture2D clearImg = null;

        public ChapterWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        }


        public override void Draw(int aID)
        {
            var chapter = Controller.Instance.SelectedChapterDataControl;

            GUILayout.Label(TC.get("Chapter.Title"));
            EditorGUI.BeginChangeCheck();
            var title = EditorGUILayout.TextField(chapter.getTitle());
            if (EditorGUI.EndChangeCheck())
                chapter.setTitle(title);

            GUILayout.Space(20);
            GUILayout.Label(TC.get("Chapter.Description"));
            EditorGUI.BeginChangeCheck();
            var description = EditorGUILayout.TextArea(chapter.getDescription(), GUILayout.MinHeight(200));
            if (EditorGUI.EndChangeCheck())
                chapter.setDescription(description);

            GUILayout.Space(20);

            var names = getSceneNames();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(TC.get("Chapter.InitialScene"));
            var initial = EditorGUILayout.Popup(System.Array.FindIndex(names, chapter.getInitialScene().Equals), names);
            if (EditorGUI.EndChangeCheck())
                chapter.setInitialScene(names[initial]);
        }

        private void ChangeSelectedInitialScene(int i)
        {
            Controller.Instance.ChapterList.getSelectedChapterDataControl()
                .setInitialScene(getSceneNames()[i]);
        }

        private string[] getSceneNames()
        {
            return Controller.Instance.IdentifierSummary.getIds<IChapterTarget>();
        }
    }
}
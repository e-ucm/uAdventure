using UnityEngine;

using uAdventure.Core;
using UnityEditor;

namespace uAdventure.Editor
{
    public class BooksWindowDocumentation : LayoutWindow
    {
        public BooksWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }

        public override void Draw(int aID)
        {
            var workingBook = Controller.Instance.ChapterList.getSelectedChapterData().getBooks()[GameRources.GetInstance().selectedBookIndex];
            
            GUILayout.Label(TC.get("Book.Documentation"));
            EditorGUI.BeginChangeCheck();
            var documentation = GUILayout.TextArea(workingBook.getDocumentation(), GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingBook.setDocumentation(documentation);
            }
        }
    }
}
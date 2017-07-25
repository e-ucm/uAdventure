using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BooksWindowDocumentation : LayoutWindow
    {
        private string documentation, documentationLast;

        public BooksWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            string doc = "";

            if (GameRources.GetInstance().selectedCutsceneIndex >= 0)
                doc = Controller.Instance.ChapterList.getSelectedChapterData().getBooks()[
                    GameRources.GetInstance().selectedBookIndex].getDocumentation();
            doc = (doc == null ? "" : doc);
            documentation = documentationLast = doc;
        }

        public override void Draw(int aID)
        {
            GUILayout.Space(20);
            GUILayout.Label(TC.get("Book.Documentation"));
            GUILayout.Space(20);
            documentation = GUILayout.TextArea(documentation, GUILayout.MinHeight(0.4f * m_Rect.height));
            if (!documentation.Equals(documentationLast))
                OnDocumentationChanged(documentation);
        }

        private void OnDocumentationChanged(string s)
        {
            Controller.Instance.ChapterList.getSelectedChapterData().getBooks()[GameRources.GetInstance().selectedBookIndex].setDocumentation(s);
            documentationLast = s;
        }
    }
}
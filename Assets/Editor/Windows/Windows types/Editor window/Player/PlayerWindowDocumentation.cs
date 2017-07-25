using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class PlayerWindowDocumentation : LayoutWindow
    {
        private string documentation, documentationLast;

        public PlayerWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            documentation = documentationLast = Controller.Instance.ChapterList.getSelectedChapterData().getPlayer().getDocumentation();
            if (documentation == null)
                documentation = documentationLast = "";
        }


        public override void Draw(int aID)
        {
            GUILayout.Space(20);
            GUILayout.Label(TC.get("NPC.Documentation"));
            GUILayout.Space(20);
            documentation = GUILayout.TextArea(documentation, GUILayout.MinHeight(0.4f * m_Rect.height));
            if (!documentation.Equals(documentationLast))
                OnDocumentationChanged(documentation);
        }

        private void OnDocumentationChanged(string s)
        {
            Controller.Instance.ChapterList.getSelectedChapterData().getPlayer().setDocumentation(s);
            documentationLast = s;
        }

    }
}
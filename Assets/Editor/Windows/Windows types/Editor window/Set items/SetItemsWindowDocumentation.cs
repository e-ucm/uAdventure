using UnityEngine;

using uAdventure.Core;
using UnityEditor;

namespace uAdventure.Editor
{
    public class SetItemsWindowDocumentation : LayoutWindow
    {
        private AtrezzoDataControl workingAtrezzo;

        public SetItemsWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }


        public override void Draw(int aID)
        {
            var workingAtrezzo = Controller.Instance.ChapterList.getSelectedChapterData().getAtrezzo()[GameRources.GetInstance().selectedSetItemIndex];

            EditorGUILayout.LabelField(TC.get("Atrezzo.DocPanelTitle"));
            EditorGUI.BeginChangeCheck();
            var documentation = GUILayout.TextArea(workingAtrezzo.getDocumentation() ?? string.Empty, GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
                workingAtrezzo.setDocumentation(documentation);
        }
    }
}
using UnityEngine;

using uAdventure.Core;
using UnityEditor;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(AtrezzoDataControl), Name = "Atrezzo.DocPanelTitle", Order = 20)]
    public class SetItemsWindowDocumentation : AbstractEditorComponent
    {

        public SetItemsWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }


        public override void Draw(int aID)
        {
            var workingAtrezzo = Target != null ? Target as AtrezzoDataControl : Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[GameRources.GetInstance().selectedSetItemIndex];

            EditorGUILayout.LabelField(TC.get("Atrezzo.DocPanelTitle"));
            EditorGUI.BeginChangeCheck();
            var documentation = GUILayout.TextArea(workingAtrezzo.getDocumentation() ?? string.Empty, GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
                workingAtrezzo.setDocumentation(documentation);
        }
    }
}
using UnityEngine;
using uAdventure.Core;
using UnityEditor;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(PlayerDataControl), Name = "Atrezzo.DocPanelTitle", Order = 20)]
    public class PlayerWindowDocumentation : AbstractEditorComponent
    {
        public PlayerWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }


        public override void Draw(int aID)
        {
            var workingPlayer = Target as PlayerDataControl;

            EditorGUILayout.LabelField(TC.get("Atrezzo.DocPanelTitle"));
            EditorGUI.BeginChangeCheck();
            var documentation = GUILayout.TextArea(workingPlayer.getDocumentation() ?? string.Empty, GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingPlayer.setDocumentation(documentation);
            }
        }
    }
}
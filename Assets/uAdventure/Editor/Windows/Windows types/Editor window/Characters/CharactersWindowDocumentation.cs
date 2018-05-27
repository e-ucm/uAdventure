using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(NPCDataControl), Name = "NPC.Documentation", Order = 20)]
    public class CharactersWindowDocumentation : AbstractEditorComponent
    {

        private NPCDataControl workingCharacter;
        private DescriptionsEditor descriptionsEditor;

        public CharactersWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            descriptionsEditor = ScriptableObject.CreateInstance<DescriptionsEditor>();
            
        }

        public override void Draw(int aID)
        {
            workingCharacter = Target != null ? Target as NPCDataControl : Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex];


            // -------------
            // Documentation
            // -------------

            GUILayout.Label(TC.get("NPC.Documentation"));
            EditorGUI.BeginChangeCheck();
            var fullItemDescription = GUILayout.TextArea(workingCharacter.getDocumentation() ?? string.Empty);
            if (EditorGUI.EndChangeCheck())
                workingCharacter.setDocumentation(fullItemDescription);


            // -------------
            // Descriptions
            // -------------
            descriptionsEditor.Descriptions = workingCharacter.getDescriptionController();
            descriptionsEditor.OnInspectorGUI();
            GUILayout.Space(20);
        }
    }
}
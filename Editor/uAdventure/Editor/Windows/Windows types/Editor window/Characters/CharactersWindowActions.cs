using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(NPCDataControl), Name = "NPC.ActionsPanelTitle", Order = 10)]
    public class CharactersWindowActions : AbstractEditorComponent
    {
        private readonly ActionsList actionsList;

        public CharactersWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            actionsList = ScriptableObject.CreateInstance<ActionsList>();
        }

        public override void Draw(int aID)
        {
            var isInspector = Target != null;
            var workingCharacter = isInspector ? Target as NPCDataControl : Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex];

            if (isInspector)
            {
                m_Rect.height = 300;
            }

            actionsList.ActionsListDataControl = workingCharacter.getActionsList();
            actionsList.DoList(m_Rect.height - 60f, isInspector);
        }
        
    }
}
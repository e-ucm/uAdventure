using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System.Linq;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(ItemDataControl), Name = "Item.ActionsPanelTitle", Order = 10)]
    public class ItemsWindowActions : AbstractEditorComponent
    {        
        private ActionsList actionsList;

        public ItemsWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            actionsList = ScriptableObject.CreateInstance<ActionsList>();
        }
        
        public override void Draw(int aID)
        {
            var workingItem = Target != null ? Target as ItemDataControl : Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex];
            if (Target != null) m_Rect.height = 300;

            actionsList.ActionsListDataControl = workingItem.getActionsList();
            actionsList.DoList(m_Rect.height - 60f);
        }
    }
}
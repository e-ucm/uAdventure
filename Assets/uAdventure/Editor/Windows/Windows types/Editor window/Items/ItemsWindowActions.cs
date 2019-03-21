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
        private readonly ActionsList actionsList;

        public ItemsWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            actionsList = ScriptableObject.CreateInstance<ActionsList>();
        }
        
        public override void Draw(int aID)
        {
            var isInspector = Target != null;
            var workingItem = isInspector ? Target as ItemDataControl : Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex];
            if (isInspector)
            {
                m_Rect.height = 300;
            }

            actionsList.ActionsListDataControl = workingItem.getActionsList();
            actionsList.DoList(m_Rect.height - 60f, isInspector);
        }
    }
}
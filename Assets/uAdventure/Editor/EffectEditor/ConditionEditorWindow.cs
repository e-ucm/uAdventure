using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

using uAdventure.Core;
using System.Linq;

namespace uAdventure.Editor
{
    public class ConditionEditorWindow : EditorWindow
    {
        private static ConditionEditorWindow editor;

        public void Init(ConditionsController con)
        {
            editor = EditorWindow.GetWindow<ConditionEditorWindow>();
            editor.Conditions = con.Conditions;

            ConditionEditorFactory.Intance.ResetInstance();
        }

        public void Init(Conditions con)
        {
            editor = EditorWindow.GetWindow<ConditionEditorWindow>();
            editor.Conditions = con;

            ConditionEditorFactory.Intance.ResetInstance();
        }

        public Conditions Conditions { get; set; }

		private static bool stylesInited = false;
		private static GUIStyle closeStyle, collapseStyle, conditionStyle, eitherConditionStyle;

		static void InitStyles(){
			if (!stylesInited) {
				if (closeStyle == null)
				{
					closeStyle = new GUIStyle(GUI.skin.button);
					closeStyle.padding = new RectOffset(0, 0, 0, 0);
					closeStyle.margin = new RectOffset(0, 5, 2, 0);
					closeStyle.normal.textColor = Color.red;
					closeStyle.focused.textColor = Color.red;
					closeStyle.active.textColor = Color.red;
					closeStyle.hover.textColor = Color.red;
				}

				if (collapseStyle == null)
				{
					collapseStyle = new GUIStyle(GUI.skin.button);
					collapseStyle.padding = new RectOffset(0, 0, 0, 0);
					collapseStyle.margin = new RectOffset(0, 5, 2, 0);
					collapseStyle.normal.textColor = Color.blue;
					collapseStyle.focused.textColor = Color.blue;
					collapseStyle.active.textColor = Color.blue;
					collapseStyle.hover.textColor = Color.blue;
				}

				if (conditionStyle == null)
				{
					conditionStyle = new GUIStyle(GUI.skin.box);
					conditionStyle.normal.background = TextureUtil.MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
				}

				if (eitherConditionStyle == null)
				{
					eitherConditionStyle = new GUIStyle(GUI.skin.box);
					eitherConditionStyle.normal.background = TextureUtil.MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
					eitherConditionStyle.padding.left = 15;
				}
				stylesInited = true;
			}
		}

        protected void OnGUI()
        {
			InitStyles ();

            GUILayout.BeginVertical(conditionStyle);
            GUILayout.Label(TC.get("Conditions.Title"));
            if (GUILayout.Button(TC.get("Condition.AddBlock")))
            {
                Conditions.Add(new FlagCondition(""));
            }
            GUILayout.EndVertical();

            //##################################################################################
            //############################### CONDITION HANDLING ###############################
			//##################################################################################
			LayoutConditionEditor(Conditions);
        }

		public static void LayoutConditionEditor(Conditions conditions){

			InitStyles ();
			var blocksToRemove = new List<List<Condition>>();
			foreach (var conditionBlock in conditions.GetConditionsList())
            {
                DoConditionBlock(conditionBlock);

                if (conditionBlock.Count == 0)
                {
                    blocksToRemove.Add(conditionBlock);
                }
            }

            foreach (var block in blocksToRemove)
            {
                conditions.GetConditionsList().Remove(block);
            }
		}

        private static void DoConditionBlock(List<Condition> conditionBlock)
        {
            bool eitherStyle = conditionBlock.Count > 1;

            if (eitherStyle)
            {
                GUILayout.BeginVertical(eitherConditionStyle);
            }

            var conditionEditorFactory = ConditionEditorFactory.Intance;

            for (int i = 0; i < conditionBlock.Count; i++)
            {
                var condition = conditionBlock[i];
                using (new GUILayout.HorizontalScope())
                {
                    int previousEditorSelected = conditionEditorFactory.ConditionEditorIndex(condition);
                    int editorSelected = EditorGUILayout.Popup(previousEditorSelected, conditionEditorFactory.CurrentConditionEditors);

                    if (previousEditorSelected != editorSelected)
                    {
                        condition = conditionEditorFactory.Editors[editorSelected].InstanceManagedCondition();
                        conditionBlock[i] = condition;
                    }

                    conditionEditorFactory.getConditionEditorFor(condition).draw(condition);

                    if (GUILayout.Button("+", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
                    {
                        conditionBlock.Add(new FlagCondition(""));
                    }

                    if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                    {
                        conditionBlock.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (eitherStyle)
            {
                GUILayout.EndVertical();
            }
        }
    }


}
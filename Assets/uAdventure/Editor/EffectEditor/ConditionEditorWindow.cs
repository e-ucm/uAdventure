using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ConditionEditorWindow : EditorWindow
    {
        private static ConditionEditorWindow editor;

        public void Init(ConditionsController con)
        {
            editor = EditorWindow.GetWindow<ConditionEditorWindow>();
            editor.s = Color.black;

            editor.Conditions = con.Conditions;

            ConditionEditorFactory.Intance.ResetInstance();
        }

        public void Init(Conditions con)
        {
            editor = EditorWindow.GetWindow<ConditionEditorWindow>();
            editor.s = Color.black;

            editor.Conditions = con;

            ConditionEditorFactory.Intance.ResetInstance();
        }

        private Conditions conditions;

        public Conditions Conditions
        {
            get { return conditions; }
            set { this.conditions = value; }
        }

        private Rect baseRect = new Rect(10, 10, 25, 25);
        //private Dictionary<Condition, Rect> tmpRects = new Dictionary<Condition, Rect>();
        private Dictionary<Condition, ConditionEditor> editors = new Dictionary<Condition, ConditionEditor>();

		private static bool stylesInited = false;
		private static GUIStyle closeStyle, collapseStyle, conditionStyle, eitherConditionStyle;

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        Color s = new Color(0.4f, 0.4f, 0.5f),
        	l = new Color(0.3f, 0.7f, 0.4f),
        	r = new Color(0.8f, 0.2f, 0.2f);

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
					conditionStyle.normal.background = MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
				}

				if (eitherConditionStyle == null)
				{
					eitherConditionStyle = new GUIStyle(GUI.skin.box);
					eitherConditionStyle.normal.background = MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
					eitherConditionStyle.padding.left = 15;
				}
				stylesInited = true;
			}
		}

        void OnGUI()
        {
			InitStyles ();

            GUILayout.BeginVertical(conditionStyle);
            GUILayout.Label(TC.get("Conditions.Title"));
            if (GUILayout.Button(TC.get("Condition.AddBlock")))
            {
                conditions.add(new FlagCondition(""));
            }
            GUILayout.EndVertical();

            //##################################################################################
            //############################### CONDITION HANDLING ###############################
			//##################################################################################
			LayoutConditionEditor(conditions);
        }

		public static void LayoutConditionEditor(Conditions conditions){

			InitStyles ();
			var toRemove = new List<Condition>();
			var listsToRemove = new List<List<Condition>>();
			bool toAdd = false;
			foreach (List<Condition> cl in conditions.getConditionsList())
			{
				if (cl.Count > 1)
					GUILayout.BeginVertical(eitherConditionStyle);
				
				for (int i = 0; i < cl.Count; i++)
				{

					GUILayout.BeginHorizontal();
					int preConEdiSel = ConditionEditorFactory.Intance.ConditionEditorIndex(cl[i]);
					int conEdiSel = EditorGUILayout.Popup(preConEdiSel, ConditionEditorFactory.Intance.CurrentConditionEditors);

					if (preConEdiSel != conEdiSel)
						cl[i] = ConditionEditorFactory.Intance.Editors[conEdiSel].InstanceManagedCondition();

					ConditionEditorFactory.Intance.getConditionEditorFor(cl[i]).draw(cl[i]);

					if (GUILayout.Button("+", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
					{
						toAdd = true;
					}

					if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
					{
						toRemove.Add(cl[i]);

					}

					GUILayout.EndHorizontal();
				}
				if (cl.Count > 1)
					GUILayout.EndVertical();

				foreach(var t in toRemove)
					cl.Remove(t);

				if (toAdd) {
					cl.Add(new FlagCondition(""));
					toAdd = false;
				}

				if (cl.Count == 0)
					listsToRemove.Add(cl);
			}

			foreach(var l in listsToRemove)
				conditions.getConditionsList().Remove(l);
		}
    }


}
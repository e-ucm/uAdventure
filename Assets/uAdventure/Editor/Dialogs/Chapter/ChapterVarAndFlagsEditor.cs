using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
using uAdventure.Runner;

namespace uAdventure.Editor
{
	public class ChapterVarAndFlagsEditor : EditorWindow, DialogReceiverInterface
    {
        private enum WindowType
        {
            FLAGS,
            VARS
        }

        private Texture2D flagsTex = null;
        private Texture2D varTex = null;

        private WindowType openedWindow;

        public static ChapterVarAndFlagsEditor s_ChapterVarAndFlagsEditor;
        private static long s_LastClosedTime;

        private ColumnList variablesAndFlagsList;
        private VarFlagSummary varFlagSummary;
        private string filter;

        internal static bool ShowAtPosition(Rect buttonRect)
        {
            long num = DateTime.Now.Ticks / 10000L;
            if (num >= ChapterVarAndFlagsEditor.s_LastClosedTime + 50L)
            {
                if (Event.current != null)
                {
                    Event.current.Use();
                }
                if (ChapterVarAndFlagsEditor.s_ChapterVarAndFlagsEditor == null)
                {
                    ChapterVarAndFlagsEditor.s_ChapterVarAndFlagsEditor = ScriptableObject.CreateInstance<ChapterVarAndFlagsEditor>();
                }
                ChapterVarAndFlagsEditor.s_ChapterVarAndFlagsEditor.Init(buttonRect);

                return true;
            }
            return false;
        }

        protected void OnDisable()
        {
            ChapterVarAndFlagsEditor.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
            ChapterVarAndFlagsEditor.s_ChapterVarAndFlagsEditor = null;
        }

        private void Init(Rect buttonRect)
        {
            buttonRect.position = GUIUtility.GUIToScreenPoint(buttonRect.position);
            float y = 305f;
            Vector2 windowSize = new Vector2(300f, y);
            base.ShowAsDropDown(buttonRect, windowSize);
        }


        [MenuItem("uAdventure/Flags and variables", priority = 0)]
		public static void Init()
		{
			var window = GetWindow<ChapterVarAndFlagsEditor> ();
			window.Show();
		}

        public void OnEnable()
        {
			if(!flagsTex)
				flagsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/flag16", typeof(Texture2D));
			if(!varTex)
            	varTex = (Texture2D)Resources.Load("EAdventureData/img/icons/vars", typeof(Texture2D));

            variablesAndFlagsList = new ColumnList(new List<int>(), typeof(int))
            {
                RequestRepaint = Repaint,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column(), new ColumnList.Column() { SizeOptions = new GUILayoutOption[] { GUILayout.Width(80) } }
                },
                drawCell = (rect, row, column, isActive, isFocused) =>
                {
                    // The list is only storing indexes
                    var index = (int)variablesAndFlagsList.list[row];
                    var elem = "";
                    switch (openedWindow)
                    {
                        case WindowType.FLAGS: elem = varFlagSummary.getFlag(index); break;
                        case WindowType.VARS: elem = varFlagSummary.getVar(index); break;
                    }

                    switch (column)
                    {
                        case 0:
                            if (Application.isPlaying || !isActive)
                            {
                                EditorGUI.LabelField(rect, elem);
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                var newName = EditorGUI.DelayedTextField(rect, elem);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if(varFlagSummary.getVarsAndFlags().Any(s => s.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        Controller.Instance.ShowErrorDialog("VarFlag.Error.NameIsUsed.Title".Traslate(), "VarFlag.Error.NameIsUsed.Message".Traslate());
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayDialog("WIP", "Rename is WIP", "Ok");
                                    }
                                }
                            }
                            break;
                        case 1:
                            object value = 0;
                            if (Application.isPlaying)
                            {
                                switch (openedWindow)
                                {
                                    case WindowType.FLAGS: value = Game.Instance.GameState.CheckFlag(elem) == 1 ? "inactive" : "active"; break;
                                    case WindowType.VARS: value = Game.Instance.GameState.GetVariable(elem); break;
                                }
                            }
                            else
                            {
                                switch (openedWindow)
                                {
                                    case WindowType.FLAGS: value = varFlagSummary.getFlagReferences(index); break;
                                    case WindowType.VARS: value = varFlagSummary.getVarReferences(index); break;
                                }
                            }
                            EditorGUI.LabelField(rect, value.ToString());
                            break;
                    }
                },
                onRemoveCallback = OnDeleteClicked,
                onAddCallback = OnAddCliked,
                draggable = false
            };
        }

        protected void OnGUI()
        {
            var windowWidth = position.width;
            var windowHeight = position.height;

            if (!Controller.Instance.Loaded)
            {
                return;
            }

            varFlagSummary = Controller.Instance.VarFlagSummary;
            RefreshList();

            /*
            * Upper buttons
            */
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            openedWindow = (WindowType) GUILayout.Toolbar((int)openedWindow, new GUIContent[] { new GUIContent(TC.get("Flags.Title"), flagsTex), new GUIContent(TC.get("Vars.Title"), varTex) });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            filter = EditorGUILayout.TextField("Filter", filter);
            if (EditorGUI.EndChangeCheck())
            {
                RefreshList();
            }

            var height = windowHeight - GUILayoutUtility.GetLastRect().max.y - 90f;
            /*
            * Content part
            */
            switch (openedWindow)
            {
                case WindowType.FLAGS:
                    variablesAndFlagsList.Columns[0].Text = TC.get("Flags.FlagName");
                    variablesAndFlagsList.Columns[1].Text = Application.isPlaying ? TC.get("Conditions.Flag.State") : TC.get("Flags.FlagReferences");
                    break;
                case WindowType.VARS:
                    variablesAndFlagsList.Columns[0].Text = TC.get("Vars.VarName");
                    variablesAndFlagsList.Columns[1].Text = Application.isPlaying ? TC.get("Conditions.Var.Value") : TC.get("Vars.VarReferences");
                    break;
            }
            var playing = Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;
            variablesAndFlagsList.displayAddButton = !playing;
            variablesAndFlagsList.displayRemoveButton = !playing;

            variablesAndFlagsList.DoList(height);
        }

        void OnAddCliked(ReorderableList reorderableList)
        {
            switch (openedWindow)
            {
                case WindowType.FLAGS:
                    Controller.Instance.ShowInputDialog(TC.get("Flags.AddFlag"), TC.get("Flags.AddFlagMessage"), "IdFlag", "Flag", this);
                    break;
                case WindowType.VARS:
                    Controller.Instance.ShowInputDialog(TC.get("Vars.AddVar"), TC.get("Vars.AddVarMessage"), "IdVar", "Var", this);
                    break;
            }
        }

        void OnDeleteClicked(ReorderableList reorderableList)
        {
            if (reorderableList.index >= 0)
            {
                var selected = (int)reorderableList.list[reorderableList.index];
                var summary = varFlagSummary;
                Controller.Instance.updateVarFlagSummary();

                switch (openedWindow)
                {
                    case WindowType.FLAGS:
                        if (summary.getFlagReferences(selected) > 0) 
                        { 
                            Controller.Instance.ShowErrorDialog("Flags.DeleteFlag".Traslate(), "Flags.ErrorFlagWithReferences".Traslate());
                            return;
                        }
                        summary.deleteFlag(varFlagSummary.getFlag(selected));
                        break;
                    case WindowType.VARS:
                        if (summary.getVarReferences(selected) > 0)
                        {
                            Controller.Instance.ShowErrorDialog("Vars.DeleteVar".Traslate(), "Vars.ErrorVarWithReferences".Traslate());
                            return;
                        }
                        summary.deleteVar(varFlagSummary.getVar(selected));
                        break;
                }
            }
            RefreshList();
        }

        public void OnDialogOk(string message, object token = null, object workingObjectSecond = null)
        {
            if ((string)token == "Flag")
            {
                varFlagSummary.addFlag(message);
            }
            else if ((string)token == "Var")
            {
                varFlagSummary.addVar(message);
            }

            RefreshList();
        }

        private void RefreshList()
        {
            var summary = varFlagSummary;
            Func<int, bool> filterFunc = (_) => true;
            IEnumerable<int> indexes = new List<int>();
            switch (openedWindow)
            {
                case WindowType.FLAGS:
                    indexes = Enumerable.Range(0, summary.getFlagCount());
                    filterFunc = (i) => summary.getFlag(i).ToLowerInvariant().Contains(filter.ToLowerInvariant());
                    break;
                case WindowType.VARS:
                    indexes = Enumerable.Range(0, summary.getVarCount());
                    filterFunc = (i) => summary.getVar(i).ToLowerInvariant().Contains(filter.ToLowerInvariant());
                    break;
                default: break;
            }
            variablesAndFlagsList.list = string.IsNullOrEmpty(filter) ? indexes.ToList() : indexes.Where(filterFunc).ToList();
            this.Repaint();
        }

        public void OnDialogCanceled(object workingObject = null)
        {
        }
    }
}
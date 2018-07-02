using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

using uAdventure.Core;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    public class AdvancedFeaturesWindowMacros : LayoutWindow
    {
        private DataControlList macroList;

        public AdvancedFeaturesWindowMacros(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            macroList = new DataControlList()
            {
                RequestRepaint = Repaint,
                footerHeight = 25,
                elementHeight = 40,
                Columns = new System.Collections.Generic.List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = TC.get("MacrosList.ID"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(150) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Macro.Documentation"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Element.Effects"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(220) }
                    }
                },
                drawCell = (rect, index, column, isActive, isFocused) =>
                {
                    var macro = macroList.list[index] as MacroDataControl;
                    switch (column)
                    {
                        case 0:
                            EditorGUI.BeginChangeCheck();
                            var id = EditorGUI.DelayedTextField(rect, macro.getId());
                            if (EditorGUI.EndChangeCheck()) macro.setId(id);
                            break;
                        case 1:
                            EditorGUI.BeginChangeCheck();
                            var documentation = EditorGUI.TextArea(rect, macro.getDocumentation() ?? string.Empty);
                            if (EditorGUI.EndChangeCheck()) macro.setDocumentation(documentation);
                            break;
                        case 2:
                            if (GUI.Button(rect, TC.get("GeneralText.EditEffects")))
                            {
                                EffectEditorWindow window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                                window.Init(macro.getController());
                            }
                            break;

                    }
                }
            };
        }

        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            macroList.SetData(Controller.Instance.SelectedChapterDataControl.getMacrosListDataControl(),
                (data) => (data as MacroListDataControl).getMacros().Cast<DataControl>().ToList());
            macroList.DoList(windowHeight - 60f);
        }
    }
}
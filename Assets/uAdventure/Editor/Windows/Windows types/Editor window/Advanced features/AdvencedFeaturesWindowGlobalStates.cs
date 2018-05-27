using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    public class AdvencedFeaturesWindowGlobalStates : LayoutWindow
    {
        private DataControlList globalStateList;
        private Texture2D conditionsTex;
        private Texture2D noConditionsTex;

        public AdvencedFeaturesWindowGlobalStates(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            globalStateList = new DataControlList()
            {
                footerHeight = 25,
                elementHeight = 40,
                Columns = new System.Collections.Generic.List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = TC.get("GlobalStatesList.ID"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(150) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("GlobalState.Documentation"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("GlobalState.Conditions"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(220) }
                    }
                },
                drawCell = (rect, index, column, isActive, isFocused) =>
                {
                    var globalState = globalStateList.list[index] as GlobalStateDataControl;
                    switch (column)
                    {
                        case 0:
                            EditorGUI.BeginChangeCheck();
                            var id = EditorGUI.DelayedTextField(rect, globalState.getId());
                            if (EditorGUI.EndChangeCheck()) globalState.setId(id);
                            break;
                        case 1:
                            EditorGUI.BeginChangeCheck();
                            var documentation = EditorGUI.TextArea(rect, globalState.getDocumentation() ?? string.Empty);
                            if (EditorGUI.EndChangeCheck()) globalState.setDocumentation(documentation);
                            break;
                        case 2:
                            if (GUI.Button(rect, globalState.getController().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                            {
                                ConditionEditorWindow window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                                window.Init(globalState.getController());
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
            
            globalStateList.SetData(Controller.Instance.SelectedChapterDataControl.getGlobalStatesListDataControl(),
                (data) => (data as GlobalStateListDataControl).getGlobalStates().Cast<DataControl>().ToList());
            globalStateList.DoList(windowHeight - 60f);
        }
    }
}
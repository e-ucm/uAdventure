using UnityEngine;
using uAdventure.Core;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{

    public class ScenesWindowBarriers : SceneEditorWindow
    {

        private DataControlList barriersList;
        private Texture2D conditionsTex, noConditionsTex;
        private SceneDataControl workingScene;

        public ScenesWindowBarriers(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
           params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            barriersList = new DataControlList()
            {
                elementHeight = 20,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column() // Layer column
                    {
                        Text = TC.get("Barriers.Name"),
                        SizeOptions = new GUILayoutOption[]{ GUILayout.ExpandWidth(true) }
                    },
                    new ColumnList.Column() // Enabled Column
                    {
                        Text = TC.get("Conditions.Title"),
                        SizeOptions = new GUILayoutOption[]{ GUILayout.ExpandWidth(true) }
                    }
                },
                drawCell = (columnRect, row, column, isActive, isFocused) =>
                {
                    var element = barriersList.list[row] as BarrierDataControl;
                    switch (column)
                    {
                        case 0:
                            EditorGUI.LabelField(columnRect, "Barrier: " + (row + 1));
                            break;
                        case 1:
                            if (GUI.Button(columnRect, element.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                            {
                                ConditionEditorWindow window =
                                     (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                                window.Init(element.getConditions());
                            }
                            break;
                    }
                },
                onSelectCallback = (list) =>
                {
                    sceneEditor.SelectedElement = barriersList.list[list.index] as BarrierDataControl;
                }
            };

            sceneEditor.onSelectElement += (element) =>
            {
                if (element is BarrierDataControl)
                {
                    barriersList.index = barriersList.list.IndexOf(element as BarrierDataControl);
                }
                else
                {
                    barriersList.index = -1;
                }
            };
        }
        
        protected override void DrawInspector()
        {
            var prevWorkingScene = workingScene;
            workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
            if (workingScene != prevWorkingScene)
            {
                barriersList.SetData(workingScene.getBarriersList(),
                    (dc) => (dc as BarriersListDataControl).getBarriers().Cast<DataControl>().ToList());
            }

            barriersList.DoList(160);
            GUILayout.Space(20);
        }
    }
}
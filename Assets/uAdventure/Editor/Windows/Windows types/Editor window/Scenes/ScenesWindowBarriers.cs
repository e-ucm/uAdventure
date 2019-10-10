using UnityEngine;
using uAdventure.Core;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{

    public class ScenesWindowBarriers : SceneEditorWindow
    {
        private readonly DataControlList barriersList;
        private readonly Texture2D conditionsTex, noConditionsTex;

        public ScenesWindowBarriers(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
           params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {

            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");
            PreviewTitle = "Scene.Preview".Traslate();

            barriersList = new DataControlList()
            {
                RequestRepaint = Repaint,
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
                                barriersList.index = row;
                                ConditionEditorWindow window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                                window.Init(element.getConditions());
                            }
                            break;
                    }
                },
                onSelectCallback = (list) =>
                {
                    sceneEditor.SelectedElement = barriersList.list[list.index] as BarrierDataControl;
                },
                onRemoveCallback = (list) =>
                {
                    sceneEditor.SelectedElement = null;
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

        public override void OnSceneSelected(SceneDataControl scene)
        {
            base.OnSceneSelected(scene);
            if (scene == null)
            {
                return;
            }
            barriersList.SetData(scene.getBarriersList(),
                (dc) => (dc as BarriersListDataControl).getBarriers().Cast<DataControl>().ToList());

        }

        protected override void DrawInspector()
        {
            barriersList.DoList(160);
            GUILayout.Space(20);
        }
    }
}
using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;
using System;

namespace uAdventure.Editor
{
    public class ScenesWindowActiveAreas : SceneEditorWindow
    {
        
        private readonly Texture2D conditionsTex, noConditionsTex;
        
        private readonly DataControlList activeAreasList;

        public ScenesWindowActiveAreas(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new ActiveAreaActionsComponent(Rect.zero, new GUIContent(""), aStyle);
            new ActiveAreaDescriptionsComponent(Rect.zero, new GUIContent(""), aStyle);

            PreviewTitle = "Scene.Preview".Traslate();

            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

            activeAreasList = new DataControlList()
            {
                RequestRepaint = Repaint,
                elementHeight = 20,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column() // Layer column
                    {
                        Text = TC.get("ElementList.Layer"),
                        SizeOptions = new GUILayoutOption[]{ GUILayout.Width(50) }
                    },
                    new ColumnList.Column() // Element name
                    {
                        Text = TC.get("ElementList.Title"),
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
                    var element = activeAreasList.list[row] as ActiveAreaDataControl;
                    switch (column)
                    {
                        case 0: GUI.Label(columnRect, row.ToString()); break;
                        case 1:
                            if (isActive)
                            {
                                EditorGUI.BeginChangeCheck();
                                var newId = EditorGUI.DelayedTextField(columnRect, element.getId());
                                if (EditorGUI.EndChangeCheck()) element.renameElement(newId);
                            }
                            else GUI.Label(columnRect, element.getId());

                            break;
                        case 2:
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
                    sceneEditor.SelectedElement = activeAreasList.list[list.index] as ExitDataControl;
                },
                onRemoveCallback = (list) =>
                {
                    sceneEditor.SelectedElement = null;
                }
            };

            sceneEditor.onSelectElement += (element) =>
            {
                if (element is ActiveAreaDataControl)
                {
                    activeAreasList.index = activeAreasList.list.IndexOf(element as ActiveAreaDataControl);
                }
                else
                {
                    activeAreasList.index = -1;
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

            activeAreasList.SetData(scene.getActiveAreasList(),
                (dc) => (dc as ActiveAreasListDataControl).getActiveAreas().Cast<DataControl>().ToList());
        }

        protected override void DrawInspector()
        {
            activeAreasList.DoList(160);
            GUILayout.Space(20);
        }


        [EditorComponent(typeof(ActiveAreaDataControl), Name = "Item.ActionsPanelTitle", Order = 10)]
        private class ActiveAreaActionsComponent : AbstractEditorComponent
        {
            private readonly ActionsList actionsList;

            public ActiveAreaActionsComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                actionsList = ScriptableObject.CreateInstance<ActionsList>();
            }

            public override void Draw(int aID)
            {
                var workingActiveArea = Target as ActiveAreaDataControl;
                
                actionsList.ActionsListDataControl = workingActiveArea.getActionsList();
                actionsList.DoList(300, true);
            }
        }

        [EditorComponent(typeof(ActiveAreaDataControl), Name = "Item.DocPanelTitle", Order = 20)]
        public class ActiveAreaDescriptionsComponent : AbstractEditorComponent
        {
            private readonly string[] behaviourTypes = { "Behaviour.Normal", "Behaviour.FirstAction" };
            private readonly string[] behaviourTypesDescription = { "Behaviour.Selection.Normal", "Behaviour.Selection.FirstAction" };

            private readonly DescriptionsEditor descriptionsEditor;

            public ActiveAreaDescriptionsComponent(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
                params GUILayoutOption[] aOptions)
                : base(aStartPos, aContent, aStyle, aOptions)
            {
                descriptionsEditor = ScriptableObject.CreateInstance<DescriptionsEditor>();
            }


            public override void Draw(int aID)
            {
                var workingActiveArea = Target as ActiveAreaDataControl;
                
                // -------------
                // Descriptions
                // -------------
                descriptionsEditor.Descriptions = workingActiveArea.getDescriptionsController();
                descriptionsEditor.OnInspectorGUI();
                GUILayout.Space(20);
                
                // -------------
                // Behaviour
                // -------------
                EditorGUI.BeginChangeCheck();
                var selectedBehaviourType = EditorGUILayout.Popup(TC.get("Behaviour"), (int)workingActiveArea.getBehaviour(), behaviourTypes.Select(bt => TC.get(bt)).ToArray());
                Item.BehaviourType type = (selectedBehaviourType == 0 ? Item.BehaviourType.NORMAL : Item.BehaviourType.FIRST_ACTION);
                if (EditorGUI.EndChangeCheck())
                    workingActiveArea.setBehaviour(type);
                EditorGUILayout.HelpBox(TC.get(behaviourTypesDescription[selectedBehaviourType]), MessageType.Info);
                GUILayout.Space(20);
            }
        }
    }
}
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    public class ScenesWindowExits : SceneEditorWindow
    {
        /*
         * 
            GUILayout.Box(TC.get("ExitsList.NextScene"), GUILayout.Width(windowWidth * 0.24f));
            GUILayout.Box(TC.get("ExitsList.Transition"), GUILayout.Width(windowWidth * 0.14f));
            GUILayout.Box(TC.get("ExitsList.Appearance"), GUILayout.Width(windowWidth * 0.34f));
            GUILayout.Box(TC.get("ExitsList.ConditionsAndEffects"), GUILayout.Width(windowWidth * 0.14f));
         * 
         * */

        private string[] sceneNames;

        private string[] transitionTypes;

        private DataControlList exitsList;
        private Texture2D conditionsTex, noConditionsTex;
        private SceneDataControl workingScene;

        public ScenesWindowExits(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {


            transitionTypes = new string[]
            { TC.get("Exit.NoTransition"), TC.get("Exit.TopToBottom"), TC.get("Exit.BottomToTop"), TC.get("Exit.LeftToRight"), TC.get("Exit.RightToLeft"), TC.get("Exit.FadeIn") };


            //new ActiveAreaActionsComponent(Rect.zero, new GUIContent(""), "");
            //new ActiveAreaDescriptionsComponent(Rect.zero, new GUIContent(""), "");

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            exitsList = new DataControlList()
            {
                elementHeight = 20,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column() // Layer column
                    {
                        Text = TC.get("ExitsList.NextScene"),
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
                    var element = exitsList.list[row] as ExitDataControl;
                    switch (column)
                    {
                        case 0:
                            if (isActive)
                            {
                                var selected = sceneNames.ToList().IndexOf(element.getNextSceneId());
                                EditorGUI.BeginChangeCheck();
                                var newId = sceneNames[EditorGUI.Popup(columnRect, selected == -1 ? 0 : selected, sceneNames)];
                                if (EditorGUI.EndChangeCheck()) element.setNextSceneId(newId == "---" ? "" : newId);
                            }
                            else GUI.Label(columnRect, element.getNextSceneId());
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
                    sceneEditor.SelectedElement = exitsList.list[list.index] as ExitDataControl;
                }
            };
        }

        protected override void DrawInspector()
        {
            var namesList = new List<string>();
            namesList.Add("---");
            namesList.AddRange(Controller.Instance.SelectedChapterDataControl.getObjects<IChapterTarget>().ConvertAll(o => o.getId()));
            sceneNames = namesList.ToArray();
            
            var prevWorkingScene = workingScene;
            workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
            if (workingScene != prevWorkingScene)
            {
                exitsList.SetData(workingScene.getExitsList(),
                    (dc) => (dc as ExitsListDataControl).getExits().Cast<DataControl>().ToList());
            }

            exitsList.DoList(160);
            GUILayout.Space(20);
        }
    }
}
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;
using System;

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

        private DataControlList exitsList;
        private Texture2D conditionsTex, noConditionsTex;
        private SceneDataControl workingScene;

        public ScenesWindowExits(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {

            new ExitTransitionComponent(Rect.zero, new GUIContent(""), aStyle);
            new ExitAppearanceComponent(Rect.zero, new GUIContent(""), aStyle);
            new ExitConditionsAndEffectsComponent(Rect.zero, new GUIContent(""), aStyle);
            new ExitPlayerPositionComponent(Rect.zero, new GUIContent(""), aStyle);

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

            sceneEditor.onSelectElement += (element) =>
            {
                if (element is ExitDataControl)
                {
                    exitsList.index = exitsList.list.IndexOf(element as ExitDataControl);
                }
                else
                {
                    exitsList.index = -1;
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

        [EditorComponent(typeof(ExitDataControl), Name = "ExitsList.Transition", Order = 5)]
        public class ExitTransitionComponent : AbstractEditorComponent
        {

            private string[] transitionTypes;
            public ExitTransitionComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                transitionTypes = new string[]
                { TC.get("Exit.NoTransition"), TC.get("Exit.TopToBottom"), TC.get("Exit.BottomToTop"), TC.get("Exit.LeftToRight"), TC.get("Exit.RightToLeft"), TC.get("Exit.FadeIn") };
            }

            public override void Draw(int aID)
            {
                var exit = Target as ExitDataControl;

                // Transition type
                EditorGUI.BeginChangeCheck();
                var newtype = EditorGUILayout.Popup(TC.get("NextScene.Transition"), exit.getTransitionType(), transitionTypes);
                if (EditorGUI.EndChangeCheck()) exit.setTransitionType(newtype);

                // Transition time
                EditorGUI.BeginChangeCheck();
                var newtime = EditorGUILayout.IntField(TC.get("NextScene.TransitionTime"), exit.getTransitionTime());
                if (EditorGUI.EndChangeCheck()) exit.setTransitionTime(newtime);
            }
        }


        [EditorComponent(typeof(ExitDataControl), Name = "ExitsList.Appearance", Order = 10)]
        public class ExitAppearanceComponent : AbstractEditorComponent
        {
            private TextWithSoundField displayField;
            private FileChooser cursorField;

            public ExitAppearanceComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                displayField = new TextWithSoundField()
                {
                    Label = TC.get("Exit.ExitText"),
                    FileType = BaseFileOpenDialog.FileType.EXIT_MUSIC
                };
                
                cursorField = new FileChooser()
                {
                    Label = TC.get("Cursor.exit.Description"),
                    FileType = BaseFileOpenDialog.FileType.EXIT_ICON
                };
            }

            public override void Draw(int aID)
            {
                var exitLook = (Target as ExitDataControl).getExitLookDataControl();

                // Text and sound
                EditorGUI.BeginChangeCheck();
                displayField.Content = exitLook.getCustomizedText();
                displayField.Path = exitLook.getSoundPath();
                displayField.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    exitLook.setExitText(displayField.Content);
                    exitLook.setSoundPath(displayField.Path);
                }

                // Cursor
                EditorGUI.BeginChangeCheck();
                cursorField.Path = exitLook.getCustomizedCursor();
                cursorField.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    exitLook.setCursorPath(cursorField.Path);
                }
            }
        }

        [EditorComponent(typeof(ExitDataControl), Name = "ExitsList.ConditionsAndEffects", Order = 15)]
        public class ExitConditionsAndEffectsComponent : AbstractEditorComponent
        {
            private Texture2D conditionsTex, noConditionsTex;

            public ExitConditionsAndEffectsComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
                noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));
            }

            public override void Draw(int aID)
            {
                var exit = Target as ExitDataControl;


                // Conditions
                GUILayout.Label(TC.get("Exit.EditConditions"));
                if (GUILayout.Button(exit.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                {
                    ConditionEditorWindow window =
                         (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(exit.getConditions());
                }

                GUILayout.Label(TC.get("Exit.ConditionsActive"));
                // Effects
                if (GUILayout.Button(TC.get("GeneralText.EditEffects")))
                {
                    EffectEditorWindow window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                    window.Init(exit.getEffects());
                }

                // Post Effects
                if (GUILayout.Button(TC.get("Exit.EditPostEffects")))
                {
                    EffectEditorWindow window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                    window.Init(exit.getPostEffects());
                }

                GUILayout.Label(TC.get("Exit.ConditionsInactive"));
                // HasNotEffets
                EditorGUI.BeginChangeCheck();
                var hasNotEffects = EditorGUILayout.BeginToggleGroup(TC.get("Exit.ActiveWhenConditionsArent"), exit.isHasNotEffects());
                if (EditorGUI.EndChangeCheck()) exit.setHasNotEffects(hasNotEffects);

                // Not Effects
                if (GUILayout.Button(TC.get("Exit.EditNotEffects")))
                {
                    EffectEditorWindow window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                    window.Init(exit.getNotEffects());
                }

                EditorGUILayout.EndToggleGroup();

            }
        }

        [EditorComponent(typeof(ExitDataControl), Name = "ExitsList.PlayerPosition", Order = 2)]
        public class ExitPlayerPositionComponent : AbstractEditorComponent
        {
            private Texture2D conditionsTex, noConditionsTex;
            private SceneEditor localSceneEditor;
            private Trajectory.Node playerDestination;
            private List<DataControl> elements;

            public ExitPlayerPositionComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                localSceneEditor = new SceneEditor();
                playerDestination = new Trajectory.Node("", 0, 0, 1f);
                localSceneEditor.elements = new List<DataControl>() { new NodeDataControl(null, playerDestination, new Trajectory()) };
            }

            public override void Draw(int aID)
            {
                var exit = Target as ExitDataControl;


                EditorGUI.BeginChangeCheck();
                var has = EditorGUILayout.Toggle(TC.get("NextSceneCell.UsePosition"), exit.hasDestinyPosition());
                if (EditorGUI.EndChangeCheck())
                    exit.setDestinyPosition(has ? 400 : int.MinValue, has ? 300 : int.MinValue);

                if (!has)
                {
                    EditorGUILayout.HelpBox("Destination position will be based on origin position.", MessageType.Info); // TODO LANG
                    return;
                }
            
                EditorGUI.BeginChangeCheck();
                var newPos = EditorGUILayout.Vector2Field(TC.get("Inventory.Position"), new Vector2(exit.getDestinyPositionX(), exit.getDestinyPositionY()));
                if (EditorGUI.EndChangeCheck())
                    exit.setDestinyPosition(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y));

                EditorGUI.BeginChangeCheck();
                bool useDestinyScale = EditorGUILayout.Toggle("Use destiny scale", exit.getDestinyScale() > 0); // TODO LANG
                if (EditorGUI.EndChangeCheck())
                    exit.setDestinyScale(useDestinyScale ? 1f : float.MinValue);

                if (useDestinyScale)
                {
                    EditorGUI.BeginChangeCheck();
                    var newScale = Mathf.Max(0, EditorGUILayout.FloatField(TC.get("SceneLocation.Scale"), exit.getDestinyScale()));
                    if (EditorGUI.EndChangeCheck())
                        exit.setDestinyScale(newScale);
                }
                else
                {
                    EditorGUILayout.HelpBox("The player size will stay as before entering the exit.", MessageType.Info); // TODO LANG
                }

                var scenesList = Controller.Instance.SelectedChapterDataControl.getScenesList();
                var sceneIndex = scenesList.getSceneIndexByID(exit.getNextSceneId());
                if (sceneIndex == -1)
                {
                    EditorGUILayout.HelpBox("Please select a valid destination!", MessageType.Error); // TODO LANG
                    return;
                }

                localSceneEditor.Components = SceneEditor.Current.Components;
                localSceneEditor.Scene = scenesList.getScenes()[sceneIndex];
                playerDestination.setValues(exit.getDestinyPositionX(), exit.getDestinyPositionY(), exit.getDestinyScale() < 0 ? 1f : exit.getDestinyScale());
                
                localSceneEditor.Draw(GUILayoutUtility.GetRect(0, 200, GUILayout.ExpandWidth(true)));
                exit.setDestinyPosition(playerDestination.getX(), playerDestination.getY());
                if(useDestinyScale)
                    exit.setDestinyScale(playerDestination.getScale());

            }
        }
    }


}
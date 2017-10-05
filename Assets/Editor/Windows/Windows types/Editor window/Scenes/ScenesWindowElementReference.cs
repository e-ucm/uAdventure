using UnityEngine;
using UnityEditor;
using uAdventure.Core;
using System.Collections.Generic;
using System;

namespace uAdventure.Editor
{

    public class ScenesWindowElementReference : SceneEditorWindow
    {
        private Texture2D conditionsTex, noConditionsTex;
        
        private int currentIndex = -1;
        private SceneDataControl currentScene;

        private DataControlList referenceList;
        private SceneDataControl workingScene;

        public ScenesWindowElementReference(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new ReferenceComponent(Rect.zero, new GUIContent(""), "");

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            referenceList = new DataControlList()
            {
                elementHeight = 20,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column() // Layer column
                    {
                        Text = TC.get("ElementList.Layer"),
                        SizeOptions = new GUILayoutOption[]{ GUILayout.Width(50) }
                    },
                    new ColumnList.Column() // Enabled Column
                    {
                        Text = "Enabled",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.Width(70) }
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
                    var element = referenceList.list[row] as ElementReferenceDataControl;
                    switch (column)
                    {
                        case 0: GUI.Label(columnRect, row.ToString()); break;
                        case 1: /* TODO */ break;
                        case 2: GUI.Label(columnRect, element.getElementId()); break;
                        case 3:
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
                    sceneEditor.SelectedElement = referenceList.list[list.index] as ElementReferenceDataControl;
                }
            };

        }

        protected override void DrawInspector()
        {
            var prevWorkingScene = workingScene;
            workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
            if(workingScene != prevWorkingScene)
            {
                referenceList.SetData(workingScene.getReferencesList(),
                    (dc) => (dc as ReferencesListDataControl).getAllReferencesDataControl().ConvertAll(ec => ec.getErdc() as DataControl));
            }

            if(referenceList.count != workingScene.getReferencesList().getAllReferencesDataControl().Count)
            {
                referenceList.SetData(workingScene.getReferencesList(),
                    (dc) => (dc as ReferencesListDataControl).getAllReferencesDataControl().ConvertAll(ec => ec.getErdc() as DataControl));
            }

            referenceList.DoList(160);
            GUILayout.Space(20);
        }

        [EditorComponent(typeof(ElementReferenceDataControl), Name = "Transform", Order = 0)]
        private class ReferenceComponent : AbstractEditorComponent
        {
            public ReferenceComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            public override void OnPreRender()
            {
                var reference = Target as ElementReferenceDataControl;
                SceneEditor.Current.PushMatrix();
                var matrix = SceneEditor.Current.Matrix;
                SceneEditor.Current.Matrix = matrix * Matrix4x4.TRS(new Vector3(reference.getElementX(), reference.getElementY(), 0), Quaternion.identity, Vector3.one * reference.getElementScale());
            }

            public override void Draw(int aID)
            {
                var reference = Target as ElementReferenceDataControl;

                var oldPos = new Vector2(reference.getElementX(), reference.getElementY());
                EditorGUI.BeginChangeCheck();
                var newPos = EditorGUILayout.Vector2Field("Position", oldPos);
                if (EditorGUI.EndChangeCheck()) { reference.setElementPosition((int) newPos.x, (int) newPos.y); }
                
                EditorGUI.BeginChangeCheck();
                var newScale = EditorGUILayout.FloatField("Scale", reference.getElementScale());
                if (EditorGUI.EndChangeCheck()) { reference.setElementScale(newScale); }

            }
            

            public override bool Update()
            {
                var elemRef = Target as ElementReferenceDataControl;

                Vector2 size = Vector2.zero;

                var referencedElement = elemRef.getReferencedElementDataControl() as DataControlWithResources;
                Texture2D preview = null;
                if (referencedElement is ItemDataControl) {
                    preview = AssetsController.getImage((referencedElement as ItemDataControl).getPreviewImage()).texture;
                }
                else if (referencedElement is NPCDataControl)
                {
                    preview = AssetsController.getImage((referencedElement as NPCDataControl).getPreviewImage()).texture;
                }
                else if (referencedElement is AtrezzoDataControl)
                {
                    preview = AssetsController.getImage((referencedElement as AtrezzoDataControl).getPreviewImage()).texture;
                }
                 
                var myPos = SceneEditor.Current.Matrix.MultiplyPoint(new Vector2(-0.5f * preview.width, -preview.height));
                var mySize = SceneEditor.Current.Matrix.MultiplyVector(new Vector3(preview.width, preview.height));
                var rect = new Rect(myPos, mySize).AdjustToViewport(800, 600, SceneEditor.Current.Viewport);

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (rect.Contains(Event.current.mousePosition) && GUIUtility.hotControl == 0)
                        {
                            GUIUtility.hotControl = elemRef.getElementId().GetHashCode();
                            return true;
                        }
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == elemRef.getElementId().GetHashCode())
                        {
                            GUIUtility.hotControl = 0;
                        }
                        break;
                    case EventType.MouseMove:
                    case EventType.MouseDrag:

                        if (GUIUtility.hotControl == elemRef.getElementId().GetHashCode())
                        {
                            rect = rect.Move(Event.current.delta);
                            var original = rect.ViewportToScreen(800f, 600f, SceneEditor.Current.Viewport);
                            elemRef.setElementPosition(Mathf.RoundToInt(original.x + 0.5f * original.width), Mathf.RoundToInt(original.y + original.height));
                        }
                        break;
                }


                return false;
            }

            public override void OnPostRender()
            {
                SceneEditor.Current.PopMatrix();
            }
        }

    }
}
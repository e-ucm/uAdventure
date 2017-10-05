using UnityEngine;
using UnityEditor;
using uAdventure.Core;
using System.Collections.Generic;
using System;
using System.Linq;

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

        // ################################################################################################################################################
        // ########################################################### REFERENCE COMPONENT  ###############################################################
        // ################################################################################################################################################

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

            private Rect GetUnscaledRect(ElementReferenceDataControl elem)
            {
                var referencedElement = elem.getReferencedElementDataControl() as DataControlWithResources;
                Texture2D preview = null;
                if (referencedElement is ItemDataControl)
                {
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
                return new Rect(Vector2.zero, new Vector2(preview.width, preview.height));
            }
            
            private Rect GetElementRect(ElementReferenceDataControl element)
            {
                var unscaled = GetUnscaledRect(element);

                var myPos = SceneEditor.Current.Matrix.MultiplyPoint(new Vector2(-0.5f * unscaled.width, -unscaled.height));
                var mySize = SceneEditor.Current.Matrix.MultiplyVector(new Vector3(unscaled.width, unscaled.height));
                var rect = new Rect(myPos, mySize).AdjustToViewport(800, 600, SceneEditor.Current.Viewport);

                return rect;
            }

            public override bool Update()
            {
                var elemRef = Target as ElementReferenceDataControl;

                bool selected = false;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (GetElementRect(elemRef).Contains(Event.current.mousePosition) && GUIUtility.hotControl == 0) selected = true;
                        break;
                }

                return selected;
            }

            public override void OnDrawingGizmos()
            {
                var elemRef = Target as ElementReferenceDataControl;
                var rect = GetElementRect(elemRef);

            }

            public override void OnDrawingGizmosSelected()
            {
                var elemRef = Target as ElementReferenceDataControl;
                var rect = GetElementRect(elemRef);
                var points = rect.ToPoints();

                HandleUtil.DrawPolyLine(points, true, Color.red);
                
                int pointChanged = -1;
                Vector2 oldPointValue = Vector2.zero;
                for(int i = 0; i < points.Length; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    var aux = points[i];
                    points[i] = HandleUtil.HandlePointMovement(elemRef.GetHashCode() + i + 1, points[i], 10f, p => HandleUtil.DrawPoint(p, 4.5f, Color.blue, Color.black));
                    if (EditorGUI.EndChangeCheck())
                    {
                        oldPointValue = aux;
                        points[i] = Event.current.mousePosition;
                        pointChanged = i;
                    }
                }

                if (pointChanged != -1)
                {
                    var diff = points[pointChanged] - oldPointValue;
                    // Fix the change
                    switch (pointChanged)
                    {
                        case 0: points[3].x += diff.x; points[1].y += diff.y; break;
                        case 1: points[2].x += diff.x; points[0].y += diff.y; break;
                        case 2: points[1].x += diff.x; points[3].y += diff.y; break;
                        case 3: points[0].x += diff.x; points[2].y += diff.y; break;
                    }
                    
                    //First we calculate the new original screen rect
                    var original = points.ToRect().ViewportToScreen(800f, 600f, SceneEditor.Current.Viewport);
                    var newRatio = original.width / original.height;
                    // Then we obtain the unscaled rect to calculate the new scale
                    var unscaled = GetUnscaledRect(elemRef);
                    var unscaledRatio = unscaled.width / unscaled.height;
                    // And calculate the scale
                    if(newRatio > unscaledRatio)
                    {
                        original.height = original.width / unscaledRatio;
                    }
                    else
                    {
                        original.width = original.height * unscaledRatio;
                    }
                    // And then we rip the position
                    var position = original.center + new Vector2(0, original.height/2f);
                    var scale = original.size.magnitude / unscaled.size.magnitude;

                    // And then we set the values in the reference
                    elemRef.setElementPosition(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
                    elemRef.setElementScale(scale);

                }

                EditorGUI.BeginChangeCheck();
                rect = HandleUtil.HandleRectMovement(elemRef.GetHashCode(), rect);
                if(EditorGUI.EndChangeCheck())
                {
                    var original = rect.ViewportToScreen(800f, 600f, SceneEditor.Current.Viewport);
                    elemRef.setElementPosition(Mathf.RoundToInt(original.x + 0.5f * original.width), Mathf.RoundToInt(original.y + original.height));
                }
            }

            public override void OnPostRender()
            {
                SceneEditor.Current.PopMatrix();
            }
        }

    }
}
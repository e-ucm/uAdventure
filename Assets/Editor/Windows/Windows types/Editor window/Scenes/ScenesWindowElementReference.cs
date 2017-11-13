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
            if (Controller.Instance.playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                new InfluenceComponent(Rect.zero, new GUIContent(""), "");
            }

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
        public class ReferenceComponent : AbstractEditorComponent
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
                if (EditorGUI.EndChangeCheck()) { reference.setElementPosition(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y)); }
                
                EditorGUI.BeginChangeCheck();
                var newScale = EditorGUILayout.FloatField("Scale", reference.getElementScale());
                if (EditorGUI.EndChangeCheck()) { reference.setElementScale(newScale); }

            }

            public static Rect GetUnscaledRect(DataControl elem)
            {
                Sprite sprite = null;
                if (elem is PlayerDataControl)
                {
                    sprite = AssetsController.getImage((elem as PlayerDataControl).getPreviewImage());
                }
                else if(elem is NodeDataControl)
                {
                    sprite = AssetsController.getImage((elem as NodeDataControl).getPlayerImagePath());
                }
                else if(elem is ElementReferenceDataControl)
                {
                    var referencedElement = (elem as ElementReferenceDataControl).getReferencedElementDataControl();
                    if (referencedElement is ItemDataControl)
                    {
                        sprite = AssetsController.getImage((referencedElement as ItemDataControl).getPreviewImage());
                    }
                    else if (referencedElement is NPCDataControl)
                    {
                        sprite = AssetsController.getImage((referencedElement as NPCDataControl).getPreviewImage());
                    }
                    else if (referencedElement is AtrezzoDataControl)
                    {
                        sprite = AssetsController.getImage((referencedElement as AtrezzoDataControl).getPreviewImage());
                    }
                }

                if (!sprite)
                    return new Rect(Vector2.zero, new Vector2(100f, 100f));

                return new Rect(Vector2.zero, new Vector2(sprite.texture.width, sprite.texture.height));
            }

            public static Rect GetElementRect(DataControl element)
            {
                var unscaled = GetUnscaledRect(element);

                var myPos = SceneEditor.Current.Matrix.MultiplyPoint(new Vector2(-0.5f * unscaled.width, -unscaled.height));
                var mySize = SceneEditor.Current.Matrix.MultiplyVector(new Vector3(unscaled.width, unscaled.height));
                var rect = new Rect(myPos, mySize).AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                return rect;
            }

            public override bool Update()
            {
                var elemRef = Target as ElementReferenceDataControl;

                bool selected = false;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        var rect = GetElementRect(elemRef);
                        if (GUIUtility.hotControl == 0)
                            selected = rect.Contains(Event.current.mousePosition) || rect.ToPoints().ToList().FindIndex(p => (p - Event.current.mousePosition).magnitude <= 10f) != -1;
                        break;
                }

                return selected;
            }

            public override void OnDrawingGizmosSelected()
            {
                var elemRef = Target as ElementReferenceDataControl;
                var rect = GetElementRect(elemRef);

                var newRect = HandleUtil.HandleFixedRatioRect(elemRef.GetHashCode() + 1, rect, rect.width / rect.height, 10f,
                    polygon => HandleUtil.DrawPolyLine(polygon, true, Color.red),
                    point => HandleUtil.DrawPoint(point, 4.5f, Color.blue, Color.black));

                if (newRect != rect)
                {
                    var original = newRect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);
                    var unscaled = ScenesWindowElementReference.ReferenceComponent.GetUnscaledRect(Target);
                    // And then we rip the position
                    var position = original.center + new Vector2(0, original.height / 2f);
                    var scale = original.size.magnitude / unscaled.size.magnitude;

                    // And then we set the values in the reference
                    elemRef.setElementPosition(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
                    elemRef.setElementScale(scale);
                }

                EditorGUI.BeginChangeCheck();
                rect = HandleUtil.HandleRectMovement(elemRef.GetHashCode(), rect);
                if(EditorGUI.EndChangeCheck())
                {
                    var original = rect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);
                    elemRef.setElementPosition(Mathf.RoundToInt(original.x + 0.5f * original.width), Mathf.RoundToInt(original.y + original.height));
                }
            }

            public override void OnPostRender()
            {
                SceneEditor.Current.PopMatrix();
            }
        }



        // ################################################################################################################################################
        // ########################################################### INFLUENCE COMPONENT  ###############################################################
        // ################################################################################################################################################

        [EditorComponent(typeof(ElementReferenceDataControl), typeof(ActiveAreaDataControl), typeof(ExitDataControl), Name = "Influence Area", Order = 1)]
        public class InfluenceComponent : AbstractEditorComponent
        {
            private static InfluenceAreaDataControl getIngluenceArea(DataControl target)
            {
                if(target is ElementReferenceDataControl)
                {
                    var elemRef = target as ElementReferenceDataControl;
                    if(elemRef.getType() == Controller.ATREZZO) return null;
                    return elemRef.getInfluenceArea();
                }
                else if(target is ActiveAreaDataControl)
                {
                    return (target as ActiveAreaDataControl).getInfluenceArea();
                }
                else if(target is ExitDataControl)
                {
                    return (target as ExitDataControl).getInfluenceArea();
                }

                return null;
            }

            private static Rect getElementBoundaries(DataControl target)
            {

                if (target is ElementReferenceDataControl)
                {
                    var elemRef = target as ElementReferenceDataControl;
                    var size = ReferenceComponent.GetUnscaledRect(target).size * elemRef.getElementScale();
                    return new Rect(elemRef.getElementX() - (size.x/2f), elemRef.getElementY() - (size.y), size.x, size.y);
                }
                else if (target is RectangleArea)
                { 
                    var rectangle = (target as RectangleArea).getRectangle();
                    if (rectangle.isRectangular())
                    {
                        return new Rect(rectangle.getX(), rectangle.getY(), rectangle.getWidth(), rectangle.getHeight());
                    }
                    else
                    {
                        return rectangle.getPoints().ToArray().ToRect();
                    }
                }

                return Rect.zero;
            }

            private static Rect fixToBoundaries(Vector2 oldSize, Rect rect, Rect boundaries)
            {
                var oldpos = rect.position;
                var otherCorner = new Vector2(rect.x + rect.width + boundaries.x, rect.y + rect.height + boundaries.y);

                // This works for the top left corner
                rect.x = Mathf.Min(rect.x + boundaries.x, boundaries.x + boundaries.width);
                rect.y = Mathf.Min(rect.y + boundaries.y, boundaries.y + boundaries.height);

                // This works for the bottom right corner
                otherCorner.x = Mathf.Max(otherCorner.x, boundaries.x);
                otherCorner.y = Mathf.Max(otherCorner.y, boundaries.y);

                var newSize = new Vector2(otherCorner.x - rect.x, otherCorner.y - rect.y);

                return new Rect(rect.position, newSize);
            }

            public InfluenceComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            public override void Draw(int aID)
            {
                var influence = getIngluenceArea(Target);

                if(influence != null)
                {
                    var boundaries = getElementBoundaries(Target);

                    var oldPos = new Vector2(boundaries.x + influence.getX(), boundaries.y + influence.getY());
                    var oldSize = new Vector2(influence.getWidth(), influence.getHeight());
                    EditorGUI.BeginChangeCheck();
                    var newPos = EditorGUILayout.Vector2Field("Position", oldPos);
                    var newSize = EditorGUILayout.Vector2Field("Size", oldSize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        var fixedRect = fixToBoundaries(oldSize, new Rect(newPos, newSize), boundaries);
                        influence.setInfluenceArea(Mathf.RoundToInt(fixedRect.x - boundaries.x), Mathf.RoundToInt(fixedRect.y - boundaries.y), Mathf.RoundToInt(fixedRect.width), Mathf.RoundToInt(fixedRect.height));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("This element has no influence area");
                }
            }

            private DataControl wasSelected = null;

            public override bool Update()
            {
                bool selected = false;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        // Calculate the influenceAreaRect
                        var influenceArea = getIngluenceArea(Target);
                        if (influenceArea == null)
                            return false;
                        var boundaries = getElementBoundaries(Target);

                        var rect = new Rect(influenceArea.getX() + boundaries.x, influenceArea.getY() + boundaries.y, influenceArea.getWidth(), influenceArea.getHeight());
                        rect = rect.AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                        // See if its selected (only if it was previously selected)
                        if (GUIUtility.hotControl == 0)
                            selected = (wasSelected == Target) && (rect.Contains(Event.current.mousePosition) || rect.ToPoints().ToList().FindIndex(p => (p - Event.current.mousePosition).magnitude <= 10f) != -1);

                        if(wasSelected == Target)
                            wasSelected = null;

                        break;
                }

                return selected;
            }

            public override void OnDrawingGizmosSelected()
            {
                wasSelected = Target;
                var boundaries = getElementBoundaries(Target);
                var influenceArea = getIngluenceArea(Target);
                var rect = new Rect(boundaries.x + influenceArea.getX(), boundaries.y + influenceArea.getY(), influenceArea.getWidth(), influenceArea.getHeight());
                var originalSize = rect.size;
                rect = rect.AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                EditorGUI.BeginChangeCheck();
                var newRect = HandleUtil.HandleRect(influenceArea.GetHashCode() + 1, rect, 10f,
                    polygon =>
                    {
                        HandleUtil.DrawPolyLine(polygon, true, Color.blue);
                        HandleUtil.DrawPolygon(polygon, new Color(0,0,1f,0.3f));
                    },
                    point => HandleUtil.DrawSquare(point, 6.5f, Color.yellow, Color.black));

                newRect = HandleUtil.HandleRectMovement(influenceArea.GetHashCode(), newRect);
                
                if (EditorGUI.EndChangeCheck())
                {
                    var original = newRect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                    original = fixToBoundaries(originalSize, original, boundaries);
                    // And then we set the values in the reference
                    influenceArea.setInfluenceArea(Mathf.RoundToInt(original.x - boundaries.x), Mathf.RoundToInt(original.y - boundaries.y), Mathf.RoundToInt(original.width), Mathf.RoundToInt(original.height));
                }
            }
        }
    }
}
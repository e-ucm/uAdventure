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
        private class ReferenceList : DataControlList
        {
            private Texture2D conditionsTex, noConditionsTex;

            private Dictionary<Type, Texture2D> icons;

            public ReferencesListDataControl ReferencesListDataControl
            {
                get
                {
                    return DataControl as ReferencesListDataControl;
                }
                set
                {
                    if (DataControl != value)
                    {
                        SetData(value, (dc) => (dc as ReferencesListDataControl).getAllReferencesDataControl().Cast<DataControl>().ToList());
                    }
                }
            }

            public ReferenceList()
            {
                conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
                noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");
                icons = new Dictionary<Type, Texture2D>()
                {
                    { typeof(PlayerDataControl),    Resources.Load<Texture2D>("EAdventureData/img/icons/player-old") },
                    { typeof(ItemDataControl),      Resources.Load<Texture2D>("EAdventureData/img/icons/item") },
                    { typeof(AtrezzoDataControl),   Resources.Load<Texture2D>("EAdventureData/img/icons/atrezzo-1") },
                    { typeof(NPCDataControl),       Resources.Load<Texture2D>("EAdventureData/img/icons/npc") }
                };
                elementHeight = 20;
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
                };
                drawCell = (columnRect, row, column, isActive, isFocused) =>
                {
                    var element = list[row] as ElementContainer;
                    var erdc = element.getErdc();

                    switch (column)
                    {
                        case 0: GUI.Label(columnRect, row.ToString()); break;
                        case 1: /* TODO */ break;
                        case 2:
                            var iconSpace = new Rect(columnRect);
                            var nameSpace = new Rect(columnRect);
                            iconSpace.size = new Vector2(16, nameSpace.size.y);
                            nameSpace.position += new Vector2(16, 0);
                            nameSpace.size += new Vector2(-16, 0);

                            if (erdc == null)
                            {
                                GUI.Label(iconSpace, icons[typeof(PlayerDataControl)]);
                                GUI.Label(nameSpace, TC.get("Element.Name26"));
                            }
                            else
                            {
                                Texture2D icon = null;
                                var type = erdc.getReferencedElementDataControl().GetType();
                                if (icons.ContainsKey(type)) icon = icons[type];
                                if (icon != null)
                                    GUI.Label(iconSpace, icons[type]);
                                GUI.Label(icon != null ? nameSpace : columnRect, erdc.getElementId());
                            }
                            break;
                        case 3:
                            using (new EditorGUI.DisabledScope(erdc == null))
                            {
                                if (GUI.Button(columnRect, erdc == null ? noConditionsTex : erdc.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                                {
                                    ConditionEditorWindow window =
                                            (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                                    window.Init(erdc.getConditions());
                                }
                            }
                            break;
                    }
                };
            }

            protected override void OnRemove()
            {
                DataControl.deleteElement(((ElementContainer)GetChilds(DataControl)[index]).getErdc(), false);
                OnChanged(reorderableList);
            }

            protected override void OnDuplicate()
            {
                DataControl.duplicateElement(((ElementContainer)GetChilds(DataControl)[index]).getErdc());
                OnChanged(reorderableList);
            }

        }


        private int currentIndex = -1;
        private SceneDataControl currentScene;

        private ReferenceList referenceList;
        private SceneDataControl workingScene;

        public ScenesWindowElementReference(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new ReferenceComponent(Rect.zero, new GUIContent(""), aStyle);

            referenceList = new ReferenceList()
            {
                onSelectCallback = (list) =>
                {
                    sceneEditor.SelectedElement = (referenceList.list[list.index] as ElementContainer).getErdc();
                }
            };

            sceneEditor.onSelectElement += (element) =>
            {
                if (element is ElementReferenceDataControl)
                {
                    var erdc = element as ElementReferenceDataControl;
                    referenceList.index = referenceList.list.Cast<ElementContainer>().Select(e => e.getErdc()).ToList().IndexOf(erdc);
                }
                else
                {
                    referenceList.index = -1;
                }
            };

        }

        protected override void DrawInspector()
        {
            var prevWorkingScene = workingScene;
            workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
            if (workingScene != prevWorkingScene)
            {
                referenceList.ReferencesListDataControl = workingScene.getReferencesList();
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
                    sprite = Controller.ResourceManager.getSprite((elem as PlayerDataControl).getPreviewImage());
                }
                else if (elem is NodeDataControl)
                {
                    sprite = Controller.ResourceManager.getSprite((elem as NodeDataControl).getPlayerImagePath());
                }
                else if (elem is ElementReferenceDataControl)
                {
                    var referencedElement = (elem as ElementReferenceDataControl).getReferencedElementDataControl();
                    if (referencedElement is ItemDataControl)
                    {
                        sprite = Controller.ResourceManager.getSprite((referencedElement as ItemDataControl).getPreviewImage());
                    }
                    else if (referencedElement is NPCDataControl)
                    {
                        sprite = Controller.ResourceManager.getSprite((referencedElement as NPCDataControl).getPreviewImage());
                    }
                    else if (referencedElement is AtrezzoDataControl)
                    {
                        sprite = Controller.ResourceManager.getSprite((referencedElement as AtrezzoDataControl).getPreviewImage());
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
                if (EditorGUI.EndChangeCheck())
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
    }
}
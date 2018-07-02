using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SceneEditor
    {
        public static SceneEditor Current { get; private set; }
        public static ElementReferenceDataControl ElementReference { get; private set; }

        public delegate void OnSelectElementDeletage(DataControl selectedElement);
        public OnSelectElementDeletage onSelectElement;

        private readonly Stack<Matrix4x4> matrices;
        private readonly Dictionary<DataControl, List<EditorComponent>> cachedComponents;
        private DataControl selectedElement;
        private SceneDataControl workingScene;
        private Texture2D backgroundPreview;
        private Texture2D foregroundPreview;
        private int lastSelectedResources = -1;

        public Dictionary<Type, List<EditorComponent>> Components { get; internal set; }
        public List<DataControl> Elements { get; set; }
        public Dictionary<Type, bool> TypeEnabling { get; set; }
        public bool Disabled { get; private set; }
        public Rect Viewport { get; private set; }

        public Matrix4x4 Matrix
        {
            get
            {
                return matrices.Peek();
            }
            set
            {
                matrices.Pop();
                matrices.Push(value);
            }
        }

        public Vector2 Size
        {
            get
            {
                var size = new Vector2(800f, 600f);
                if (backgroundPreview)
                {
                    size.x = backgroundPreview.width;
                    size.y = backgroundPreview.height;
                }
                return size;
            }
        }

        public SceneDataControl Scene
        {
            get
            {
                return workingScene;
            }
            set
            {
                if (value != workingScene)
                {
                    workingScene = value;
                    SelectedElement = null;
                    RefreshSceneResources(workingScene);
                }
            }
        }

        public DataControl SelectedElement
        {
            get
            {
                return selectedElement;
            }
            set
            {
                if (value != selectedElement)
                {
                    selectedElement = value;
                    if (onSelectElement != null)
                    {
                        onSelectElement(selectedElement);
                    }
                }
            }
        }

        public SceneEditor()
        {
            Disabled = false;
            TypeEnabling = new Dictionary<Type, bool>();
            Elements = new List<DataControl>();
            cachedComponents = new Dictionary<DataControl, List<EditorComponent>>();
            matrices = new Stack<Matrix4x4>();
            matrices.Push(Matrix4x4.identity);
        }

        public static Color GetColor(Color original)
        {
            if (Current.Disabled)
            {
                original.a = original.a * 0.5f;
            }

            return original;
        }


        public void DoCallForElement(DataControl element, Action<EditorComponent> call)
        {
            foreach (var component in GetComponents(element))
            {
                var oldTarget = component.Target;
                component.Target = element;
                call(component);
                component.Target = oldTarget;
            }
        }

        private List<EditorComponent> GetComponents(DataControl element)
        {
            if (!cachedComponents.ContainsKey(element))
            {
                // Component gathering
                var components = new List<EditorComponent>();
                // Basic
                if (Components.ContainsKey(element.GetType()))
                {
                    components.AddRange(Components[element.GetType()]);
                }

                // Interface
                var typeInterfaceComponents = element.GetType()
                    .GetInterfaces()
                    .Where(i => Components.ContainsKey(i))
                    .SelectMany(i => Components[i]);

                components.AddRange(typeInterfaceComponents);

                // Sorting the components by order
                if (components.Count > 0)
                {
                    components.Sort((c1, c2) => c1.Attribute.Order.CompareTo(c2.Attribute.Order));
                    cachedComponents.Add(element, components);
                }
            }

            return cachedComponents[element];
        }

        public void DoCallForWholeElement(DataControl element, Action<EditorComponent> call)
        {
            var currentReference = ElementReference;
            ElementReference = null;
            // If its a reference
            if (element is ElementReferenceDataControl)
            {
                // First we draw the special components for the element reference
                var elemRef = element as ElementReferenceDataControl;
                SceneEditor.ElementReference = elemRef;
                DoCallForElement(elemRef, call);
                // And then we set it up to be able to draw the referenced element components
                element = elemRef.getReferencedElementDataControl();
                if (element == null)
                {
                    Debug.LogError("Can't find element refferenced as \"" + elemRef.getElementId() + "\"");
                }
            }

            // Component drawing
            if (element != null)
            {
                DoCallForElement(element, call);
            }
            ElementReference = currentReference;
        }

        public void PushMatrix()
        {
            matrices.Push(matrices.Peek() * Matrix4x4.identity);
        }

        public void PopMatrix()
        {
            matrices.Pop();
        }

        private void SetDisabledFor(DataControl element)
        {
            if (element == null)
            {
                return;
            }

            if (!TypeEnabling.ContainsKey(element.GetType()))
            {
                TypeEnabling[element.GetType()] = true;
            }
            Disabled = !TypeEnabling[element.GetType()];
        }

        public void CallAll(List<DataControl> elements, Action<DataControl> call)
        {
            foreach (var element in elements)
            {
                SetDisabledFor(element);
                call(element);
            }
        }

        public void Draw(Rect rect)
        {
            var previousCurrent = Current;
            Current = this;

            Viewport = rect.AdjustToRatio(Size.x / Size.y);

            if (workingScene.getSelectedResources() != lastSelectedResources)
            {
                RefreshSceneResources(workingScene);
            }

            // Background
            if (backgroundPreview)
            {
                GUI.DrawTexture(Viewport, backgroundPreview, ScaleMode.ScaleToFit);
            }

            var wantsToDeselect = Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);

            SelectedElement = UpdateAndRenderElements(wantsToDeselect);
            DrawGizmos();
            DrawGizmosSelected();

            if (foregroundPreview)
            {
                GUI.DrawTexture(Viewport, foregroundPreview, ScaleMode.ScaleToFit);
            }

            Current = previousCurrent;
        }

        public void OnInspectorGUI()
        {
            var previousCurrent = Current;
            Current = this;

            DoCallForWholeElement(SelectedElement, (component) =>
            {
                component.Collapsed = !EditorGUILayout.Foldout(!component.Collapsed, (component.Attribute.Name), true);
                if (!component.Collapsed)
                {
                    EditorGUI.indentLevel++;
                    component.DrawInspector();
                    GUILayout.Space(5);
                    EditorGUI.indentLevel--;
                }
                //
                DrawSplitLine(GUILayoutUtility.GetLastRect().max.y);
            });

            Current = previousCurrent;
        }

        private DataControl UpdateAndRenderElements(bool wantsToDeselect)
        {
            var toSelect = wantsToDeselect ? null : SelectedElement;

            CallAll(Elements,
                (elem) =>
                {
                    DoCallForWholeElement(elem, c => c.OnPreRender());
                    DoCallForWholeElement(elem, c =>
                    {
                        var wantsSelect = c.Update();
                        var maintainsPreviousSelection = toSelect == SelectedElement && SelectedElement != null;

                        if (!Disabled && wantsSelect && !maintainsPreviousSelection)
                        {
                            toSelect = elem;
                        }
                    });

                    if (Event.current.type == EventType.Repaint)
                    {
                        var oldColor = GUI.color;
                        if (Disabled)
                        {
                            GUI.color = new Color(1, 1, 1, 0.5f);
                        }
                        DoCallForWholeElement(elem, c => c.OnRender(Viewport));
                        GUI.color = oldColor;
                    }
                    DoCallForWholeElement(elem, c => c.OnPostRender());
                });

            return toSelect;
        }

        private void DrawGizmosSelected()
        {
            DoCallForWholeElement(SelectedElement, c => c.OnPreRender());
            DoCallForWholeElement(SelectedElement, c => c.OnDrawingGizmosSelected());
            DoCallForWholeElement(SelectedElement, c => c.OnPostRender());
        }

        private void DrawGizmos()
        {
            CallAll(Elements,
                (elem) =>
                {
                    DoCallForWholeElement(elem, c => c.OnPreRender());
                    DoCallForWholeElement(elem, c => c.OnDrawingGizmos());
                    DoCallForWholeElement(elem, c => c.OnPostRender());
                });
        }

        public void RefreshSceneResources(DataControlWithResources resources)
        {
            var scene = resources as SceneDataControl;

            var back = scene.getPreviewBackground();
            var fore = scene.getPreviewForeground();

            backgroundPreview = Controller.ResourceManager.getImage(back);
            foregroundPreview = Controller.ResourceManager.getImage(fore);

            if (backgroundPreview && foregroundPreview)
            {
                foregroundPreview = CreateMask(backgroundPreview, foregroundPreview);
            }

            lastSelectedResources = scene.getSelectedResources();
        }


        protected void DrawSplitLine(float y)
        {
            Rect position = new Rect(-5f, y, 1000f, 1f);
            Rect texCoords = new Rect(0f, 1f, 1f, 1f - 1f / (float)GUI.skin.FindStyle("IN title").normal.background.height);
            GUI.DrawTextureWithTexCoords(position, GUI.skin.FindStyle("IN title").normal.background, texCoords);
        }

        private Texture2D CreateMask(Texture2D background, Texture2D foreground)
        {
            Texture2D toReturn = new Texture2D(background.width, background.height, background.format, false);

            /* TODO: Fix the foreground mask creation 
             *  var foregroundPixels = foreground.GetPixels();
             *  toReturn.SetPixels(background.GetPixels().Select((color, i) => new Color(color.r, color.g, color.b, foregroundPixels[i].r)).ToArray());
             */

            toReturn.Apply();
            return toReturn;
        }
    }
}
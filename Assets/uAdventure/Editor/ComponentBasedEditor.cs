using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class ComponentBasedEditor
    {
        public static ComponentBasedEditor Generic { get; protected set; }

        public abstract Vector2d[] ToRelative(Vector2d[] points);

        public abstract Vector2d[] FromRelative(Vector2d[] points);
    }

    public abstract class ComponentBasedEditor<T> : ComponentBasedEditor where T : ComponentBasedEditor<T>
    {
        public static T Current
        {
            get { return Generic as T; }
            protected set { Generic = value; }
        }

        public static IElementReference ElementReference { get; protected set; }

        public delegate void OnSelectElementDeletage(DataControl selectedElement);
        public OnSelectElementDeletage onSelectElement;
        
        private readonly Dictionary<DataControl, List<EditorComponent>> cachedComponents;
        private DataControl selectedElement;

        public Dictionary<Type, List<EditorComponent>> Components { get; internal set; }
        public List<DataControl> Elements { get; set; }
        public Dictionary<Type, bool> TypeEnabling { get; set; }
        public bool Disabled { get; protected set; }
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

        protected ComponentBasedEditor()
        {
            Disabled = false;
            TypeEnabling = new Dictionary<Type, bool>();
            Elements = new List<DataControl>();
            Components = new Dictionary<Type, List<EditorComponent>>();
            cachedComponents = new Dictionary<DataControl, List<EditorComponent>>();
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
                }
                cachedComponents.Add(element, components);
            }

            return cachedComponents[element];
        }

        public void DoCallForWholeElement(DataControl element, Action<EditorComponent> call)
        {
            var currentReference = ElementReference;
            ElementReference = null;
            // If its a reference
            if (element is IElementReference)
            {
                // First we draw the special components for the element reference
                var elemRef = element as IElementReference;
                ElementReference = elemRef;
                DoCallForElement(element, call);
                // And then we set it up to be able to draw the referenced element components
                element = elemRef.ReferencedDataControl;
                if (element == null)
                {
                    Debug.LogError("Can't find element refferenced as \"" + elemRef.ReferencedId + "\"");
                }
            }

            // Component drawing
            if (element != null)
            {
                DoCallForElement(element, call);
            }
            ElementReference = currentReference;
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

        public virtual void Draw(Rect rect)
        {
            var previousCurrent = Current;
            Current = this as T;

            BeforeDrawElements(rect);

            var wantsToDeselect = Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);

            SelectedElement = UpdateAndRenderElements(wantsToDeselect);
            DrawGizmos();
            DrawGizmosSelected();

            AfterDrawElements(rect);

            Current = previousCurrent;
        }

        protected virtual void BeforeDrawElements(Rect rect) { }

        protected virtual void AfterDrawElements(Rect rect) { }

        public void OnInspectorGUI()
        {
            var previousCurrent = Current;
            Current = this as T;

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
                        DoCallForWholeElement(elem, c => c.OnRender());
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


        protected void DrawSplitLine(float y)
        {
            if(Event.current.type != EventType.Repaint)
            {
                return;
            }
            Rect position = new Rect(-5f, y, 1000f, 25f);
            GetStyle("IN Title").Draw(position, position.Contains(Event.current.mousePosition), false, false, false);
        }

        GUIStyle GetStyle(string styleName)
        {
            GUIStyle s = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (s == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
                s = error;
            }
            return s;
        }

        private GameObject gameObject;
        private static GUIStyle ms_Error;
        internal static GUIStyle error
        {
            get
            {
                if (ms_Error == null)
                {
                    ms_Error = new GUIStyle();
                    ms_Error.name = "StyleNotFoundError";
                }
                return ms_Error;
            }
        }

    }
}
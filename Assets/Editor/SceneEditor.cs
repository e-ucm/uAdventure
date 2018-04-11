using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class SceneEditor {

    public static SceneEditor Current;

    private DataControl selectedElement;

    public delegate void OnSelectElementDeletage(DataControl selectedElement);
    public OnSelectElementDeletage onSelectElement;

    public SceneEditor()
    {
        Disabled = false;
        TypeEnabling = new Dictionary<Type, bool>();
        matrices = new Stack<Matrix4x4>();
        matrices.Push(Matrix4x4.identity);
    }

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
    private Stack<Matrix4x4> matrices;

    public static Color GetColor(Color original)
    {
        if (Current.Disabled)
            original.a = original.a * 0.5f;

        return original;
    }

    public List<DataControl> elements = new List<DataControl>();
    private SceneDataControl workingScene;
    private Texture2D backgroundPreview;
    private Texture2D foregroundPreview;
    private int lastSelectedResources = -1;

    public Vector2 Size
    {
        get
        {
            return backgroundPreview ? new Vector2(backgroundPreview.width, backgroundPreview.height) : new Vector2(800f, 600f);
        }
    }

    public Dictionary<Type, List<EditorComponent>> Components { get; internal set; }
    public Dictionary<Type, bool> TypeEnabling { get; set; }
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
    public bool Disabled { get; private set; }
    public Rect Viewport { get; private set; }

    public DataControl SelectedElement {
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
                    onSelectElement(selectedElement);
            }
        }
    }

    private Dictionary<DataControl, List<EditorComponent>> cachedComponents = new Dictionary<DataControl, List<EditorComponent>>();


    public void DoCallForElement(DataControl element, Action<EditorComponent> call)
    {
        List<EditorComponent> components;
        if (!cachedComponents.ContainsKey(element))
        {
            //if (!Components.ContainsKey(element.GetType())) return;

            // Component gathering
            components = new List<EditorComponent>();
            // Basic
            if (Components.ContainsKey(element.GetType()))
                components.AddRange(Components[element.GetType()]);
            // Interface
            foreach (var inter in element.GetType().GetInterfaces())
                if (Components.ContainsKey(inter))
                    components.AddRange(Components[inter]);

            // Sorting the components by order
            if(components.Count > 0)
            {
                components.Sort((c1, c2) => c1.Attribute.Order.CompareTo(c2.Attribute.Order));
                cachedComponents.Add(element, components);
            }
        }

        // Calling
        foreach (var component in cachedComponents[element])
        {
            var oldTarget = component.Target;
            component.Target = element;
            call(component);
            component.Target = oldTarget;
        }
    }

    public void DoCallForWholeElement(DataControl element, Action<EditorComponent> call)
    {
        var currentSceneEditor = SceneEditor.Current;
        SceneEditor.Current = this;
        var elem = element;
        // If its a reference
        if (element is ElementReferenceDataControl)
        {
            // First we draw the special components for the element reference
            var elemRef = element as ElementReferenceDataControl;
            DoCallForElement(elemRef, call);
            // And then we set it up to be able to draw the referenced element components
            element = elemRef.getReferencedElementDataControl();
        }

        // Component drawing
        DoCallForElement(element, call);
        SceneEditor.Current = currentSceneEditor;
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
            return;

        if (!TypeEnabling.ContainsKey(element.GetType()))
            TypeEnabling[element.GetType()] = true;
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
        Viewport = rect.AdjustToRatio(Size.x / Size.y);
        var lastCurrent = Current;
        Current = this;

        if (workingScene.getSelectedResources() != lastSelectedResources)
        {
            RefreshSceneResources(workingScene);
        }

        // Background
        if (backgroundPreview)
            GUI.DrawTexture(Viewport, backgroundPreview, ScaleMode.ScaleToFit);

        var deSelect = Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);
        var toSelect = deSelect ? null : SelectedElement;
        var eventType = Event.current.type;

        CallAll(elements, 
            (elem) =>
            {
                DoCallForWholeElement(elem, c => c.OnPreRender());
                DoCallForWholeElement(elem, c => { if (c.Update() && !Disabled && (toSelect != SelectedElement || SelectedElement == null)) toSelect = elem; });
                if (Event.current.type == EventType.Repaint)
                {
                    var oldColor = GUI.color;
                    if (Disabled)
                        GUI.color = new Color(1, 1, 1, 0.5f);
                    DoCallForWholeElement(elem, c => c.OnRender(Viewport));
                    GUI.color = oldColor;
                }
                DoCallForWholeElement(elem, c => c.OnPostRender());
            });
        
        SelectedElement = toSelect;

        CallAll(elements,
            (elem) =>
            {
                DoCallForWholeElement(elem, c => c.OnPreRender());
                DoCallForWholeElement(elem, c => c.OnDrawingGizmos());
                DoCallForWholeElement(elem, c => c.OnPostRender());
            });

        CallAll(elements,
            (elem) =>
            {
                if (elem == SelectedElement)
                {
                    DoCallForWholeElement(elem, c => c.OnPreRender());
                    DoCallForWholeElement(elem, c => c.OnDrawingGizmosSelected());
                    DoCallForWholeElement(elem, c => c.OnPostRender());
                }
            });

        Current = lastCurrent;

        if (foregroundPreview)
            GUI.DrawTexture(Viewport, foregroundPreview, ScaleMode.ScaleToFit);
    }

    public void RefreshSceneResources(DataControlWithResources resources)
    {
        var workingScene = resources as SceneDataControl;

        var back = workingScene.getPreviewBackground();
        var fore = workingScene.getPreviewForeground();

        backgroundPreview = string.IsNullOrEmpty(back) ? null : Controller.ResourceManager.getImage(back);
        foregroundPreview = string.IsNullOrEmpty(fore) ? null : Controller.ResourceManager.getImage(fore);

        if (backgroundPreview && foregroundPreview)
            foregroundPreview = CreateMask(backgroundPreview, foregroundPreview);

        lastSelectedResources = workingScene.getSelectedResources();
    }

    private Texture2D CreateMask(Texture2D background, Texture2D foreground)
    {
        Texture2D toReturn = new Texture2D(background.width, background.height, background.format, false);
        var foregroundPixels = foreground.GetPixels();
        //TODO: FIX THIS
        //toReturn.SetPixels(background.GetPixels().Select((color, i) => new Color(color.r, color.g, color.b, foregroundPixels[i].r)).ToArray());
        toReturn.Apply();
        return toReturn;
    }
}

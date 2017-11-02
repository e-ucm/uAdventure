using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Editor;
using UnityEngine;

public class SceneEditor {

    public static SceneEditor Current;

    private DataControl selectedElement;

    public delegate void OnSelectElementDeletage(DataControl selectedElement);
    public OnSelectElementDeletage onSelectElement;

    public SceneEditor()
    {
        Disabled = false;
        EnabledTypes = new List<Type>();
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

    public List<DataControl> elements = new List<DataControl>();
    private SceneDataControl workingScene;
    private Texture2D backgroundPreview;
    private Texture2D foregroundPreview;
    private int lastSelectedResources = -1;

    public Dictionary<Type, List<EditorComponent>> Components { get; internal set; }
    public List<Type> EnabledTypes { get; set; }
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
    }

    public void PushMatrix()
    {
        matrices.Push(matrices.Peek() * Matrix4x4.identity);
    }

    public void PopMatrix()
    {
        matrices.Pop();
    }

    public void CallAll(List<DataControl> elements, Action<DataControl> call)
    {
        foreach (var element in elements)
        {
            Disabled = EnabledTypes.Count == 0 || EnabledTypes.Contains(element.GetType());
            call(element);
        }
    }

    public void Draw(Rect rect)
    {
        Viewport = rect.AdjustToRatio(800f / 600f);

        if(Event.current.type == EventType.MouseDown && Viewport.Contains(Event.current.mousePosition))
        {
            SelectedElement = null;
        }

        var previousWorkingItem = workingScene;
        workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
        if(workingScene.getSelectedResources() != lastSelectedResources)
        {
            RefreshSceneResources(workingScene);
        }

        // Background
        if (backgroundPreview)
            GUI.DrawTexture(Viewport, backgroundPreview, ScaleMode.ScaleToFit);

        Current = this;

        CallAll(elements, 
            (elem) =>
            {
                DoCallForWholeElement(elem, c => c.OnPreRender());
                DoCallForWholeElement(elem, c => { if (c.Update()) SelectedElement = elem; });
                if (Event.current.type == EventType.Repaint)
                    DoCallForWholeElement(elem, c => c.OnRender(Viewport));
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

        Current = null;

        if (foregroundPreview)
            GUI.DrawTexture(Viewport, foregroundPreview, ScaleMode.ScaleToFit);
    }

    public void RefreshSceneResources(DataControlWithResources resources)
    {
        var workingScene = resources as SceneDataControl;

        var back = workingScene.getPreviewBackground();
        var fore = workingScene.getPreviewForeground();

        backgroundPreview = string.IsNullOrEmpty(back) ? null : AssetsController.getImage(back).texture;
        foregroundPreview = string.IsNullOrEmpty(fore) ? null : AssetsController.getImage(fore).texture;

        if (backgroundPreview && foregroundPreview)
            foregroundPreview = CreateMask(backgroundPreview, foregroundPreview);

        lastSelectedResources = workingScene.getSelectedResources();
    }

    private Texture2D CreateMask(Texture2D background, Texture2D foreground)
    {
        Texture2D toReturn = new Texture2D(background.width, background.height, background.format, false);
        var foregroundPixels = foreground.GetPixels();
        toReturn.SetPixels(background.GetPixels().Select((color, i) => new Color(color.r, color.g, color.b, foregroundPixels[i].r)).ToArray());
        toReturn.Apply();
        return toReturn;
    }
}

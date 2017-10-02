﻿using System;
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

    public void DoCallForElement(DataControl element, Action<EditorComponent> call)
    {
        if (!Components.ContainsKey(element.GetType()))
        {
            return;
        }

        foreach (var component in Components[element.GetType()])
        {
            var oldTarget = component.Target;
            component.Target = element;
            call(component);
            component.Target = oldTarget;
        }
    }

    public void DoCallForWholeElement(DataControl element, Action<EditorComponent> call)
    {
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

    public void Draw(Rect rect)
    {
        Viewport = rect.AdjustToRatio(800f / 600f);

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
        foreach (var element in elements)
        {
            Disabled = EnabledTypes.Count == 0 || EnabledTypes.Contains(element.GetType());

            DoCallForWholeElement(element, (c) => c.OnPreRender());
            DoCallForWholeElement(element, (c) => { if (c.Update()) SelectedElement = element; });
            DoCallForWholeElement(element, (c) => c.OnRender(Viewport));

            DoCallForWholeElement(element, (c) => c.OnDrawingGizmos());
            if (SelectedElement == element)
            {
                DoCallForWholeElement(element, (c) => c.OnDrawingGizmosSelected());
            }
            DoCallForWholeElement(element, (c) => c.OnPostRender());

        }
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
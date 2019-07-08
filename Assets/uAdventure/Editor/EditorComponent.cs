
using uAdventure.Editor;
using UnityEngine;

/// <summary>
/// The editor component allows to manipulate a certain part of an element inside of the scene editor.
/// </summary>
public interface EditorComponent
{
    EditorComponentAttribute Attribute { get; }

    DataControl Target { get; set; }


    /// <summary>
    /// Stores the collapsed state in the editor.
    /// </summary>
    bool Collapsed { get; set; }

    /// <summary>
    /// DrawInspector is used to draw the editors for the current component.
    /// </summary>
    void DrawInspector();

    /// <summary>
    /// Update is used to perform all the movements and selections of the current element.
    /// </summary>
    /// <returns>True if the element is selected</returns>
    bool Update();

    /// <summary>
    /// Used to set up values before rendering (i.e. transform matrix).
    /// </summary>
    void OnPreRender();

    /// <summary>
    /// Used to render the element.
    /// </summary>
    void OnRender();

    /// <summary>
    /// Used to do things after rendering.
    /// </summary>
    void OnPostRender();

    /// <summary>
    /// Used to draw visual help and handles.
    /// </summary>
    void OnDrawingGizmos();

    /// <summary>
    /// Used to draw visual help and handles when the element is selected.
    /// </summary>
    void OnDrawingGizmosSelected();
}

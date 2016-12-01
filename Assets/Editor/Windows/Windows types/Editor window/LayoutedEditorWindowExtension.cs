using UnityEngine;
using System.Collections;

public class LayoutedEditorWindowExtension : MonoBehaviour {
    private GUIContent aContent;
    private GUILayoutOption[] aOptions;
    private Rect aStartPos;
    private GUIStyle aStyle;

    public LayoutedEditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, GUILayoutOption[] aOptions)
    {
        this.aStartPos = aStartPos;
        this.aContent = aContent;
        this.aStyle = aStyle;
        this.aOptions = aOptions;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

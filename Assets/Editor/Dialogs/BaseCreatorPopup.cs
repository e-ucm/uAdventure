using UnityEngine;
using System.Collections;
using UnityEditor;

public class BaseCreatorPopup : EditorWindow
{

    protected DialogReceiverInterface reference;
    protected const float windowWidth = 500, windowHeight = 600;

    public virtual void Init(DialogReceiverInterface e)
    {
        BaseCreatorPopup window = this;
        reference = e;
        window.position = new Rect(Screen.width/2-windowWidth/2, Screen.height / 2 - windowHeight/2, windowWidth, windowHeight);
        window.Show();
    }
}

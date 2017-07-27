using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class AnimationField : FileChooser {

    public override void DoLayout(params GUILayoutOption[] options)
    {
        var initialPath = Path;
        var rect = EditorGUILayout.BeginHorizontal(options);
        {
            if (!string.IsNullOrEmpty(newFilePath))
            {
                Path = newFilePath;
                newFilePath = string.Empty;
            }
            drawPath();
            drawSelect();
            drawView();
            drawClear();
        }
        EditorGUILayout.EndHorizontal();
        GUI.changed = initialPath != Path;
    }

    protected void drawView()
    {
        var text = string.IsNullOrEmpty(Path) ? TC.get("Resources.Create") : TC.get("Resources.Edit");
        if (GUILayout.Button(text, GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(text)).x)))
        {                    
            // For not-existing cutscene - show new cutscene name dialog
            if (string.IsNullOrEmpty(Path))
            {
                ScriptableObject.CreateInstance<CutsceneNameInputPopup>().Init(this, "");
            }
            else
            {
                EditCutscene();
            }
        }
    }

    string newFilePath = string.Empty;
    public override void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {

        if(workingObject is CutsceneNameInputPopup)
        {
            newFilePath = message;
            OnSlidesceneCreated(message);
            EditCutscene();
        }
        else if(workingObject is CutsceneSlidesEditor)
        {

        }
        else
        {
            base.OnDialogOk(message, workingObject, workingObjectSecond);
            EditCutscene();
        }
    }

    void OnSlidesceneCreated(string val)
    {
        uAdventure.Core.Animation newAnim = new uAdventure.Core.Animation(val.Split('/').Last(), new EditorImageLoader());
        newAnim.getFrame(0).setUri("assets/special/EmptyAnimation_01.png");
        AnimationWriter.writeAnimation(val, newAnim);
        Path = val;
    }

    void EditCutscene()
    {
        ScriptableObject.CreateInstance<CutsceneSlidesEditor>().Init(this, Path);
    }
}
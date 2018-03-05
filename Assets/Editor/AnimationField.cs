using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class AnimationField : FileChooser {

    const string EMPTY = "assets/special/EmptyAnimation";

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
        GUI.changed = initialPath != Path || frameChanged;
    }

    protected void drawView()
    {
        var text = Path.Equals(EMPTY) ? TC.get("Resources.Create") : TC.get("Resources.Edit");
        if (GUILayout.Button(text, GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(text)).x)))
        {
            // For not-existing cutscene - show new cutscene name dialog
            if (Path.Equals(EMPTY))
            {
                ScriptableObject.CreateInstance<CutsceneNameInputPopup>().Init(this, "");
            }
            else
            {
                EditCutscene();
            }
        }
    }

    protected override void drawClear()
    {
        if (ShowClear)
        {
            using (new EditorGUI.DisabledScope(Path.Equals(EMPTY)))
            {
                if (GUILayout.Button(delTex, GUILayout.Width(delTex.width + 10f)))
                {
                    Path = EMPTY;
                }
            }
        }
    }

    string newFilePath = string.Empty;
    private bool frameChanged = false;

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
            newFilePath = message;
        }
        else if(workingObject is uAdventure.Core.Animation)
        {
            frameChanged = true;
        }
        else
        {
            base.OnDialogOk(message, workingObject, workingObjectSecond);
            EditCutscene();
        }
    }

    void OnSlidesceneCreated(string val)
    {
        uAdventure.Core.Animation newAnim = new uAdventure.Core.Animation(val.Split('/').Last());
        newAnim.getFrame(0).setUri(EMPTY + "_01.png");
        AnimationWriter.writeAnimation(val, newAnim);
        AssetDatabase.ImportAsset("Assets/Resources/CurrentGame/" + val);
        Path = val;
    }

    void EditCutscene()
    {
        ScriptableObject.CreateInstance<CutsceneSlidesEditor>().Init(this, Path);
    }
}
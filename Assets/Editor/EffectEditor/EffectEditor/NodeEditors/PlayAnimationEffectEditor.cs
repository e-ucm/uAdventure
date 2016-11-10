using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class PlayAnimationEffectEditor : EffectEditor, DialogReceiverInterface
{

    private Texture2D clearImg = null;
    private string slidesPath = "";

    private bool collapsed = false;

    public bool Collapsed
    {
        get { return collapsed; }
        set { collapsed = value; }
    }

    private Rect window = new Rect(0, 0, 300, 0);

    public Rect Window
    {
        get
        {
            if (collapsed) return new Rect(window.x, window.y, 50, 30);
            else return window;
        }
        set
        {
            if (collapsed) window = new Rect(value.x, value.y, window.width, window.height);
            else window = value;
        }
    }

    private PlayAnimationEffect effect;

    public PlayAnimationEffectEditor()
    {
        this.effect = new PlayAnimationEffect(slidesPath, 300, 300);
    }

    public void draw()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Effect.PlayAnimation"));
        if (GUILayout.Button(clearImg))
        {
            OnSlidesceneChanged("");
        }
        GUILayout.Box(slidesPath);
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            AnimationFileOpenDialog animationDialog =
                (AnimationFileOpenDialog)ScriptableObject.CreateInstance(typeof(AnimationFileOpenDialog));
            animationDialog.Init(this, BaseFileOpenDialog.FileType.PLAY_ANIMATION_EFFECT);
        }
        // Create/edit slidescene
        if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit")))
        {
            // For not-existing cutscene - show new cutscene name dialog
            if (slidesPath == null || slidesPath.Equals(""))
            {
                CutsceneNameInputPopup createCutsceneDialog =
                    (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                createCutsceneDialog.Init(this, "");
            }
            else
            {
                EditCutscene();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("PlayAnimationEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect
    {
        get { return effect; }
        set { effect = value as PlayAnimationEffect; }
    }

    public string EffectName
    {
        get { return TC.get("PlayAnimationEffect.Title"); }
    }

    public EffectEditor clone()
    {
        return new PlayAnimationEffectEditor();
    }

    public bool manages(AbstractEffect c)
    {

        return c.GetType() == effect.GetType();
    }

    void OnSlidesceneChanged(string val)
    {
        slidesPath = val;
        effect.setPath(val);
    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        switch ((BaseFileOpenDialog.FileType)workingObject)
        {
            case BaseFileOpenDialog.FileType.PLAY_ANIMATION_EFFECT:
                OnSlidesceneChanged(message);
                break;
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
        Debug.Log("Wiadomość nie OK");
    }

    void EditCutscene()
    {
        CutsceneSlidesEditor slidesEditor =
            (CutsceneSlidesEditor)ScriptableObject.CreateInstance(typeof(CutsceneSlidesEditor));
        slidesEditor.Init(this, effect.getPath());
    }
}

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class PlaySoundEffectEditor : EffectEditor, DialogReceiverInterface
{

    private Texture2D clearImg = null;
    private string musicPath = "";

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

    private PlaySoundEffect effect;

    public PlaySoundEffectEditor()
    {
        this.effect = new PlaySoundEffect(false, musicPath);
    }

    public void draw()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Effect.PlaySound"));
        if (GUILayout.Button(clearImg))
        {
            musicPath = "";
            effect.setPath(musicPath);
        }
        GUILayout.Box(musicPath);
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            MusicFileOpenDialog musicDialog =
                (MusicFileOpenDialog) ScriptableObject.CreateInstance(typeof (MusicFileOpenDialog));
            musicDialog.Init(this, BaseFileOpenDialog.FileType.SCENE_MUSIC);
        }

        effect.setBackground(GUILayout.Toggle(effect.isBackground(), TC.get("PlaySoundEffect.BackgroundCheckBox")));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("PlaySoundEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect
    {
        get { return effect; }
        set { effect = value as PlaySoundEffect; }
    }

    public string EffectName
    {
        get { return TC.get("PlaySoundEffect.Title"); }
    }

    public EffectEditor clone()
    {
        return new PlaySoundEffectEditor();
    }

    public bool manages(AbstractEffect c)
    {

        return c.GetType() == effect.GetType();
    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        switch ((BaseFileOpenDialog.FileType) workingObject)
        {
            case BaseFileOpenDialog.FileType.PLAY_SOUND_EFFECT:
                musicPath = message;
                effect.setPath(musicPath);
                break;
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
        Debug.Log("Wiadomość nie OK");
    }
}
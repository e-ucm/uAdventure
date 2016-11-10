using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

public class CutsceneSlidesEditor : BaseCreatorPopup, DialogReceiverInterface
{

    private Texture2D backgroundPreviewTex = null;

    private Texture2D addTexture = null;
    private Texture2D moveLeft, moveRight = null;
    private Texture2D clearImg = null;
    private Texture2D duplicateImg = null;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedFrameSkin;

    private Rect animInfoRect;
    private Rect timelineRect;
    private Rect timelineButtonsRect;
    private Rect frameInfoRect;
    private Rect buttonRect;

    private bool useTransitonFlag, slidesAnimationFlag, useTransitonFlagLast, slidesAnimationFlagLast;

    private string documentationTextContent = "";
    private string animationDurationString = "40", animationDurationStringLast = "40", transitionDurationString = "0", transitionDurationStringLast = "0";
    private string imagePath = "", soundPath = "";

    private Animation workingAnimation;

    private int selectedFrame;

    private Vector2 scrollPosition;

    private string cutscenePath;
    public void Init(DialogReceiverInterface e, string cutsceneFilePath)
    {
        cutscenePath = cutsceneFilePath;
        clearImg = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        addTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));
        moveLeft = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeLeft", typeof (Texture2D));
        moveRight = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeRight", typeof (Texture2D));
        duplicateImg = (Texture2D) Resources.Load("EAdventureData/img/icons/duplicateNode", typeof (Texture2D));

        animInfoRect = new Rect(0f, 0.05f*windowHeight, windowWidth, 0.15f*windowHeight);
        timelineRect = new Rect(0f, 0.25f*windowHeight, windowWidth, 0.3f*windowHeight);
        timelineButtonsRect = new Rect(0f, 0.50f*windowHeight, windowWidth, 0.1f*windowHeight);
        frameInfoRect = new Rect(0f, 0.65f*windowHeight, windowWidth, 0.25f*windowHeight);
        buttonRect = new Rect(0f, 0.9f*windowHeight, windowWidth, 0.1f*windowHeight);

        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));
        selectedFrameSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

        //transitionTypes = new string []{ "None" , "Fade in", "Horizontal", "Vertical"};
        Debug.Log(cutsceneFilePath);

        workingAnimation = Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator(),
            cutsceneFilePath, new EditorImageLoader());

        Debug.Log(workingAnimation.getAboslutePath() + " " + workingAnimation.getFrames().Count + " " + workingAnimation.isSlides() + " " + workingAnimation.getId());
        if (workingAnimation == null)
            workingAnimation = new Animation(cutsceneFilePath, 40, new EditorImageLoader());

        // Initalize
        selectedFrame = 0;
        documentationTextContent = workingAnimation.getFrames()[selectedFrame].getDocumentation();
        imagePath = workingAnimation.getFrames()[selectedFrame].getUri();
        soundPath = workingAnimation.getFrames()[selectedFrame].getSoundUri();
        animationDurationString =
            animationDurationStringLast = workingAnimation.getFrames()[selectedFrame].getTime().ToString();
        transitionDurationString =
            transitionDurationStringLast = workingAnimation.getTransitions()[selectedFrame + 1].getTime().ToString();
        useTransitonFlag = useTransitonFlagLast = workingAnimation.isUseTransitions();
        slidesAnimationFlag = slidesAnimationFlagLast = workingAnimation.isSlides();

        base.Init(e);
    }

    void OnGUI()
    {
        /*
         * Documentation area
         */
        GUILayout.BeginArea(animInfoRect);

        GUILayout.BeginVertical();
        GUILayout.Label(TC.get("Animation.Documentation"));
        GUILayout.Space(5);
        documentationTextContent = GUILayout.TextField(documentationTextContent);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        useTransitonFlag = GUILayout.Toggle(useTransitonFlag, TC.get("Animation.UseTransitions"));
        if (useTransitonFlag != useTransitonFlagLast)
            OnUseTransitonFlagLastChanged(useTransitonFlag);
        slidesAnimationFlag = GUILayout.Toggle(slidesAnimationFlag, TC.get("Animation.Slides"));
        if (slidesAnimationFlag != slidesAnimationFlagLast)
            OnSlidesAnimationFlagLastChanged(slidesAnimationFlag);
        GUILayout.EndVertical();

        GUILayout.EndArea();

        /*
         * Transition panel
         */
        GUILayout.BeginArea(timelineRect);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.MaxHeight(0.25f * windowWidth));
        GUI.skin = noBackgroundSkin;
        GUILayout.BeginHorizontal();
        for (int i = 0; i < workingAnimation.getFrames().Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (selectedFrame == i)
                GUI.skin = selectedFrameSkin;

            if (
                GUILayout.Button(
                    workingAnimation.getFrames()[i].getImage(false, false, global::Animation.ENGINE).texture, GUILayout.MaxHeight(0.2f * windowHeight), GUILayout.MaxWidth(0.2f * windowWidth)))
            {
                OnFrameSelectionChanged(i);
            }
            GUI.skin = noBackgroundSkin;
            GUILayout.EndHorizontal();
        }
        GUI.skin = defaultSkin;
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndArea();


        /*
          * Transition button panel
          */
        GUILayout.BeginArea(timelineButtonsRect);
        GUILayout.BeginHorizontal();
        GUI.skin = noBackgroundSkin;
        if (GUILayout.Button(moveLeft))
        {
            workingAnimation.moveLeft(selectedFrame);
        }
        if (GUILayout.Button(clearImg))
        {
            if (selectedFrame >= 0)
            {
                workingAnimation.removeFrame(selectedFrame);
                selectedFrame--;
            }
        }
        if (GUILayout.Button(addTexture))
        {
            workingAnimation.addFrame(selectedFrame, null);
        }
        if (GUILayout.Button(duplicateImg))
        {
            workingAnimation.addFrame(selectedFrame, workingAnimation.getFrame(selectedFrame));

        }
        if (GUILayout.Button(moveRight))
        {
            workingAnimation.moveRight(selectedFrame);
        }
        GUI.skin = defaultSkin;
        GUILayout.EndHorizontal();
        GUILayout.EndArea();


        /*
         * Frame info panel
         */
        GUILayout.BeginArea(frameInfoRect);

        GUILayout.BeginHorizontal();
        GUILayout.Label(TC.get("Animation.Duration"));
        animationDurationString = GUILayout.TextField(animationDurationString);
        animationDurationString = (Regex.Match(animationDurationString, "^[0-9]{1,4}$").Success
            ? animationDurationString
            : animationDurationStringLast);
        if (!animationDurationString.Equals(animationDurationStringLast))
            OnFrameDurationChanged(animationDurationString);
        GUILayout.Space(5);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(TC.get("Animation.Image"));
        GUILayout.Box(imagePath, GUILayout.MinWidth(0.3f * windowWidth));
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            ImageFileOpenDialog imageDialog =
                (ImageFileOpenDialog) ScriptableObject.CreateInstance(typeof (ImageFileOpenDialog));
            imageDialog.Init(this, BaseFileOpenDialog.FileType.FRAME_IMAGE);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(TC.get("Animation.Sound"));
        GUILayout.Space(5);
        if (GUILayout.Button(clearImg))
        {
        }
        GUILayout.Box(soundPath, GUILayout.MinWidth(0.3f * windowWidth));
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            MusicFileOpenDialog musicDialog =
                (MusicFileOpenDialog) ScriptableObject.CreateInstance(typeof (MusicFileOpenDialog));
            musicDialog.Init(this, BaseFileOpenDialog.FileType.FRAME_MUSIC);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label(TC.get("NextScene.Transition") + " " + TC.get("Animation.Duration"));
        transitionDurationString = GUILayout.TextField(transitionDurationString);
        transitionDurationString = (Regex.Match(transitionDurationString, "^[0-9]{1,4}$").Success
            ? transitionDurationString
            : transitionDurationStringLast);
        if (!transitionDurationString.Equals(transitionDurationStringLast))
            OnTransitionDurationChanged(transitionDurationString);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();


        GUILayout.BeginArea(buttonRect);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            AnimationWriter.writeAnimation(cutscenePath, workingAnimation);
            reference.OnDialogOk("", this);
            this.Close();
        }
        if (GUILayout.Button(TC.get("GeneralText.Cancel")))
        {
            reference.OnDialogCanceled();
            this.Close();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void OnFrameSelectionChanged(int i)
    {
        selectedFrame = i;
        imagePath = workingAnimation.getFrames()[i].getUri();
        soundPath = workingAnimation.getFrames()[i].getSoundUri();

        animationDurationString = animationDurationStringLast = workingAnimation.getFrames()[i].getTime().ToString();

        documentationTextContent = animationDurationStringLast = workingAnimation.getFrames()[i].getDocumentation();

        animationDurationString =
            animationDurationStringLast = workingAnimation.getFrames()[selectedFrame].getTime().ToString();
        transitionDurationString =
            transitionDurationStringLast = workingAnimation.getTransitions()[selectedFrame + 1].getTime().ToString();
    }

    private void OnSlidesAnimationFlagLastChanged(bool val)
    {
        slidesAnimationFlagLast = val;
        workingAnimation.setSlides(val);
    }

    private void OnUseTransitonFlagLastChanged(bool val)
    {
        useTransitonFlagLast = val;
        workingAnimation.setUseTransitions(val);
    }

    private void OnFrameDurationChanged(string dur)
    {
        animationDurationStringLast = dur;
        workingAnimation.getFrame(selectedFrame).setTime(long.Parse(dur));
    }

    private void OnTransitionDurationChanged(string dur)
    {
        transitionDurationStringLast = dur;
        workingAnimation.getTransitions()[selectedFrame+1].setTime(long.Parse(dur));
    }


    private void OnFrameImageChanged(string val)
    {
        imagePath = val;
        workingAnimation.getFrame(selectedFrame).setUri(val);
    }

    private void OnFrameMusicChanged(string val)
    {
        soundPath = val;
        workingAnimation.getFrame(selectedFrame).setSoundUri(val);
    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        if (workingObject is BaseFileOpenDialog.FileType)
        {
            switch ((BaseFileOpenDialog.FileType) workingObject)
            {
                case BaseFileOpenDialog.FileType.FRAME_IMAGE:
                    OnFrameImageChanged(message);
                    break;
                case BaseFileOpenDialog.FileType.FRAME_MUSIC:
                    OnFrameMusicChanged(message);
                    break;
            }
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
      
    }
}
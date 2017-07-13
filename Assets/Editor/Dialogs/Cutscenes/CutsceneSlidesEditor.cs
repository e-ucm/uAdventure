using UnityEngine;
using System.Text.RegularExpressions;

using uAdventure.Core;
using Animation = uAdventure.Core.Animation;
using UnityEditor;

namespace uAdventure.Editor
{
    public class CutsceneSlidesEditor : BaseCreatorPopup, DialogReceiverInterface
    {
        private GUIStyle titleStyle;

        private Texture2D backgroundPreviewTex = null;

        private Texture2D addTexture = null;
        private Texture2D moveLeft, moveRight = null;
        private Texture2D clearImg = null;
        private Texture2D duplicateImg = null;

        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;
        private static GUISkin selectedFrameSkin;

        private bool useTransitonFlag, slidesAnimationFlag, useTransitonFlagLast, slidesAnimationFlagLast;

        private string documentationTextContent = "";
        private string imagePath = "", soundPath = "";

        private long animationDuration = 40, transitionDuration = 0;

        private Animation workingAnimation;

        private int selectedFrame;

        private Vector2 scrollPosition;

        private string cutscenePath;

        private void OnEnable()
        {
            titleStyle = new GUIStyle();
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.margin = new RectOffset(0, 0, 5, 5);
        }

        public void Init(DialogReceiverInterface e, string cutsceneFilePath)
        {
            windowWidth = 800;
            windowHeight = 1000;

            cutscenePath = cutsceneFilePath;
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            moveLeft = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeLeft", typeof(Texture2D));
            moveRight = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeRight", typeof(Texture2D));
            duplicateImg = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

            noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
            selectedFrameSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

            //transitionTypes = new string []{ "None" , "Fade in", "Horizontal", "Vertical"};
            Debug.Log(cutsceneFilePath);

            workingAnimation = Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator(),
                cutsceneFilePath, new EditorImageLoader());

            Debug.Log(workingAnimation.getAboslutePath() + " " + workingAnimation.getFrames().Count + " " + workingAnimation.isSlides() + " " + workingAnimation.getId());
            if (workingAnimation == null)
                workingAnimation = new Animation(cutsceneFilePath, 40, new EditorImageLoader());

            // Initalize
            selectedFrame = -1;
            OnFrameSelectionChanged(selectedFrame);

            base.Init(e);
        }

        void OnGUI()
        {
            EditorGUILayout.PrefixLabel(TC.get("Animation.GeneralInfo"), GUIStyle.none, titleStyle);
            documentationTextContent = EditorGUILayout.TextField(TC.get("Animation.Documentation"), documentationTextContent);

            useTransitonFlag = EditorGUILayout.Toggle(TC.get("Animation.UseTransitions"), useTransitonFlag);
            if (useTransitonFlag != useTransitonFlagLast)
                OnUseTransitonFlagLastChanged(useTransitonFlag);

            slidesAnimationFlag = EditorGUILayout.Toggle(TC.get("Animation.Slides"), slidesAnimationFlag);
            if (slidesAnimationFlag != slidesAnimationFlagLast)
                OnSlidesAnimationFlagLastChanged(slidesAnimationFlag);

            /*
             * Transition panel
             */
            EditorGUILayout.PrefixLabel(TC.get("Animation.Timeline"), GUIStyle.none, titleStyle);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, true, false, GUILayout.Height(125));
            EditorGUILayout.BeginHorizontal();
            GUI.skin = noBackgroundSkin;
            for (int i = 0; i < workingAnimation.getFrames().Count; i++)
            {
                if (selectedFrame == i)
                    GUI.skin = selectedFrameSkin;

                if (
                    GUILayout.Button(
                        workingAnimation.getFrames()[i].getImage(false, false, global::uAdventure.Core.Animation.ENGINE).texture, GUILayout.Height(100), GUILayout.Width(80)))
                {
                    OnFrameSelectionChanged(i);
                }
                GUI.skin = noBackgroundSkin;
            }
            GUI.skin = defaultSkin;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();


            /*
              * Transition button panel
              */
            EditorGUILayout.BeginHorizontal();
            GUI.skin = noBackgroundSkin;
            GUILayout.FlexibleSpace();

            GUI.enabled = selectedFrame > 0;
            if (GUILayout.Button(moveLeft))
            {
                workingAnimation.moveLeft(selectedFrame);
                selectedFrame--;
            }

            GUI.enabled = selectedFrame >= 0;
            if (GUILayout.Button(clearImg))
            {
                workingAnimation.removeFrame(selectedFrame);
            }

            GUI.enabled = true;
            if (GUILayout.Button(addTexture))
            {
                workingAnimation.addFrame(selectedFrame, null);
            }

            GUI.enabled = selectedFrame >= 0;
            if (GUILayout.Button(duplicateImg))
            {
                workingAnimation.addFrame(selectedFrame, workingAnimation.getFrame(selectedFrame));

            }
            GUI.enabled = selectedFrame < workingAnimation.getFrames().Count - 1;
            if (GUILayout.Button(moveRight))
            {
                workingAnimation.moveRight(selectedFrame);
                selectedFrame++;
            }

            GUI.skin = defaultSkin;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            GUI.enabled = selectedFrame != -1;
            /*
             * Frame info panel
             */
            EditorGUILayout.PrefixLabel(TC.get("Animation.Details"), GUIStyle.none, titleStyle);

            EditorGUI.BeginChangeCheck();
            animationDuration = EditorGUILayout.LongField(TC.get("Animation.Duration"), animationDuration);
            if (EditorGUI.EndChangeCheck()) OnFrameDurationChanged(animationDuration);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(TC.get("Animation.Image"));
            if (GUILayout.Button(clearImg, GUILayout.Width(clearImg.width + 20)))
            {
            }
            GUILayout.Box(imagePath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(TC.get("Buttons.Select"))).x + 20)))
            {
                ImageFileOpenDialog imageDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imageDialog.Init(this, BaseFileOpenDialog.FileType.FRAME_IMAGE);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(TC.get("Animation.Sound"));
            if (GUILayout.Button(clearImg, GUILayout.Width(clearImg.width + 20)))
            {
            }
            GUILayout.Box(soundPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(TC.get("Buttons.Select"))).x + 20)))
            {
                MusicFileOpenDialog musicDialog =
                    (MusicFileOpenDialog)ScriptableObject.CreateInstance(typeof(MusicFileOpenDialog));
                musicDialog.Init(this, BaseFileOpenDialog.FileType.FRAME_MUSIC);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            transitionDuration = EditorGUILayout.LongField(TC.get("NextScene.Transition") + " " + TC.get("Animation.Duration"), transitionDuration);
            if (EditorGUI.EndChangeCheck()) OnTransitionDurationChanged(transitionDuration);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
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
            EditorGUILayout.EndHorizontal();
        }

        private void OnFrameSelectionChanged(int i)
        {
            selectedFrame = (i == selectedFrame) ? -1 : i;

            if (selectedFrame != -1)
            {
                imagePath = workingAnimation.getFrames()[selectedFrame].getUri();
                soundPath = workingAnimation.getFrames()[selectedFrame].getSoundUri();
                
                documentationTextContent = workingAnimation.getFrames()[selectedFrame].getDocumentation();

                animationDuration = workingAnimation.getFrames()[selectedFrame].getTime();
                transitionDuration = workingAnimation.getTransitions()[selectedFrame + 1].getTime();
            }
            else
            {
                imagePath = soundPath = documentationTextContent = string.Empty;
                animationDuration = transitionDuration = 0;
            }

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

        private void OnFrameDurationChanged(long dur)
        {
            workingAnimation.getFrame(selectedFrame).setTime(dur);
        }

        private void OnTransitionDurationChanged(long dur)
        {
            workingAnimation.getTransitions()[selectedFrame + 1].setTime(dur);
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
                switch ((BaseFileOpenDialog.FileType)workingObject)
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
}
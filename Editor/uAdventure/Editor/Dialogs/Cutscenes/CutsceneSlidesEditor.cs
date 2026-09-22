using UnityEngine;

using uAdventure.Core;
using Animation = uAdventure.Core.Animation;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class CutsceneSlidesEditor : BaseCreatorPopup
    {
        private GUIStyle titleStyle;

        private Texture2D addTexture = null;
        private Texture2D moveLeft, moveRight = null;
        private Texture2D clearImg = null;
        private Texture2D duplicateImg = null;

        private string[] transitionTypeName;
        private Texture2D[] transitionTypeTexture;
        
        private static GUISkin noBackgroundSkin;
        private static GUISkin selectedFrameSkin;

        private Animation workingAnimation;
        private FileChooser imageChooser, soundChooser;

        private int selectedFrame;
        private readonly Frame emptyFrame = new Frame();
        private readonly Transition emptyTransition = new Transition();

        private Vector2 scrollPosition;

        [SerializeField]
        private string cutscenePath;

        protected void OnEnable()
        {
            titleStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 5, 5)
            };
            if (!string.IsNullOrEmpty(cutscenePath))
            { 
                Init(null, cutscenePath);
            }
        }

        [SerializeField]
        private DialogReceiverInterface parent;

        public void Init(DialogReceiverInterface e, string cutsceneFilePath)
        {
            parent = e;

            windowWidth = 800;
            windowHeight = 1000;

            cutscenePath = cutsceneFilePath;
            clearImg = Resources.Load<Texture2D>("EAdventureData/img/icons/deleteContent");
            addTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/addNode");
            moveLeft = Resources.Load<Texture2D>("EAdventureData/img/icons/moveNodeLeft");
            moveRight = Resources.Load<Texture2D>("EAdventureData/img/icons/moveNodeRight");
            duplicateImg = Resources.Load<Texture2D>("EAdventureData/img/icons/duplicateNode");

            noBackgroundSkin = Resources.Load<GUISkin>("EAdventureData/skin/EditorNoBackgroundSkin");
            selectedFrameSkin = Resources.Load<GUISkin>("EAdventureData/skin/EditorLeftMenuItemSkinConcreteOptions");
            
            transitionTypeName = new string[]
            {
                TC.get("NextScene.NoTransition"),
                TC.get("NextScene.TopToBottom"),
                TC.get("NextScene.BottomToTop"),
                TC.get("NextScene.LeftToRight"),
                TC.get("NextScene.RightToLeft"),
                TC.get("NextScene.FadeIn")
            };
            transitionTypeTexture = new Texture2D[]
            {
                Resources.Load<Texture2D>("EAdventureData/img/icons/transitionNone"),
                Resources.Load<Texture2D>("EAdventureData/img/icons/transitionVertical"),
                Resources.Load<Texture2D>("EAdventureData/img/icons/transitionVerticalReversed"),
                Resources.Load<Texture2D>("EAdventureData/img/icons/transitionHorizontal"),
                Resources.Load<Texture2D>("EAdventureData/img/icons/transitionHorizontalReversed"),
                Resources.Load<Texture2D>("EAdventureData/img/icons/transitionFadein"),
            };

            Debug.Log(cutsceneFilePath);

            var incidences = new List<Incidence>();
            if(Controller.Instance == null || !Controller.Instance.Initialized)
            {  
                Controller.Instance.Init();  
            }

            workingAnimation = Loader.LoadAnimation(cutsceneFilePath, Controller.ResourceManager, incidences) ?? new Animation(cutsceneFilePath, 40);

            imageChooser = new FileChooser
            {
                Empty = SpecialAssetPaths.ASSET_EMPTY_IMAGE,
                FileType = FileType.FRAME_IMAGE,
                Label = TC.get("Animation.Image")
            };

            soundChooser = new FileChooser
            {
                FileType = FileType.FRAME_MUSIC,
                Label = TC.get("Animation.Sound")
            };

            // Initalize
            selectedFrame = -1;

            base.Init(e);
        }

        protected void OnGUI()
        {
            if (workingAnimation == null)
            {
                this.Close();
                return;
            }

            switch (Event.current.type)
            {                
                case EventType.DragUpdated:
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Debug.Log("Dragging (" + Event.current.type + "):" + System.String.Join("\n", DragAndDrop.paths));
                    }
                    break;
                case EventType.DragPerform:
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var path in DragAndDrop.paths)
                        {
                            var uri = AssetsController.AddSingleAsset(AssetsConstants.CATEGORY_ANIMATION_IMAGE, path);
                            var frame = workingAnimation.addFrame(selectedFrame, null);
                            frame.setUri(uri);
                        }
                    }
                    break;
            }

            EditorGUILayout.PrefixLabel(TC.get("Animation.GeneralInfo"), GUIStyle.none, titleStyle);
            EditorGUI.BeginChangeCheck();
            var documentationTextContent = EditorGUILayout.TextField(TC.get("Animation.Documentation"), workingAnimation.getDocumentation());
            if (EditorGUI.EndChangeCheck())
            {
                workingAnimation.setDocumentation(documentationTextContent);
            }

            EditorGUI.BeginChangeCheck();
            var useTransitions = EditorGUILayout.Toggle(TC.get("Animation.UseTransitions"), workingAnimation.isUseTransitions());
            if (EditorGUI.EndChangeCheck())
            {
                workingAnimation.setUseTransitions(useTransitions);
            }

            EditorGUI.BeginChangeCheck();
            var isSlides = EditorGUILayout.Toggle(TC.get("Animation.Slides"), workingAnimation.isSlides());
            if(EditorGUI.EndChangeCheck())
            {
                workingAnimation.setSlides(isSlides);
            }

            /*
             * Transition panel
             */
            EditorGUILayout.PrefixLabel(TC.get("Animation.Timeline"), GUIStyle.none, titleStyle);

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition, true, false, GUILayout.Height(125)))
            using (new EditorGUILayout.HorizontalScope())
            using (new GUIUtil.SkinScope(noBackgroundSkin))
            {
                scrollPosition = scroll.scrollPosition;
                for (int i = 0, frameCount = workingAnimation.getFrames().Count; i < frameCount; i++)
                {
                    if (selectedFrame == i)
                    {
                        GUI.skin = selectedFrameSkin;
                    }

                    var frame = workingAnimation.getFrame(i);
                    var image = Controller.ResourceManager.getImage(frame.getUri());
                    var frameContent = new GUIContent(frame.getTime().ToString(), image);
                    if (GUILayout.Button(frameContent, GUILayout.Height(100), GUILayout.Width(80)))
                    {
                        selectedFrame = (i == selectedFrame) ? -1 : i;
                        GUI.FocusControl(null);
                    }
                    if (useTransitions && i != workingAnimation.getFrames().Count - 1)
                    {
                        var transition = workingAnimation.getTranstionForFrame(i);
                        var transitionContent = new GUIContent(transition.getTime().ToString(), transitionTypeTexture[(int)transition.getType()]);
                        if (GUILayout.Button(transitionContent, GUILayout.Height(100), GUILayout.Width(80)))
                        {
                            selectedFrame = (i == selectedFrame) ? -1 : i;
                            GUI.FocusControl(null);
                        }
                    }
                    GUI.skin = noBackgroundSkin;
                }
            }

            /*
              * Transition button panel
              */
            using (new EditorGUILayout.HorizontalScope())
            using (new GUIUtil.SkinScope(noBackgroundSkin))
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUI.DisabledScope(selectedFrame < 0))
                {
                    if (GUILayout.Button(moveLeft))
                    {
                        workingAnimation.moveLeft(selectedFrame);
                        selectedFrame--;
                    }
                }
                using (new EditorGUI.DisabledScope(selectedFrame < 0 || workingAnimation.getFrames().Count < 2))
                {
                    if (GUILayout.Button(clearImg))
                    {
                        workingAnimation.removeFrame(selectedFrame);
                        selectedFrame--;
                    }
                }

                if (GUILayout.Button(addTexture))
                {
                    var frame = workingAnimation.addFrame(selectedFrame, null);
                    frame.setUri(SpecialAssetPaths.ASSET_EMPTY_ANIMATION + "_01.png");
                }

                using (new EditorGUI.DisabledScope(selectedFrame < 0))
                {
                    if (GUILayout.Button(duplicateImg))
                    {
                        workingAnimation.addFrame(selectedFrame, workingAnimation.getFrame(selectedFrame));
                    }
                }

                using (new EditorGUI.DisabledScope(selectedFrame >= workingAnimation.getFrames().Count - 1))
                {
                    if (GUILayout.Button(moveRight))
                    {
                        workingAnimation.moveRight(selectedFrame);
                        selectedFrame++;
                    }
                }
                GUILayout.FlexibleSpace();
            }


            using (new EditorGUI.DisabledScope(selectedFrame == -1))
            {/*
             * Frame info panel
             */
                var frame = selectedFrame >= 0 ? workingAnimation.getFrame(selectedFrame): emptyFrame;

                EditorGUILayout.PrefixLabel(TC.get("Animation.Details"), GUIStyle.none, titleStyle);

                EditorGUI.BeginChangeCheck();
                var frameDocumentation = EditorGUILayout.TextField(TC.get("Animation.Documentation"), frame.getDocumentation());
                if (EditorGUI.EndChangeCheck())
                {
                    frame.setDocumentation(frameDocumentation);
                }

                EditorGUI.BeginChangeCheck();
                var frameDuration = System.Math.Max(0, EditorGUILayout.LongField(TC.get("Animation.Duration"), frame.getTime()));
                if (EditorGUI.EndChangeCheck())
                {
                    frame.setTime(frameDuration);
                }

                EditorGUI.BeginChangeCheck();
                imageChooser.Path = frame.getUri();
                imageChooser.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    frame.setUri(imageChooser.Path);
                }
                
                EditorGUI.BeginChangeCheck();
                soundChooser.Path = frame.getSoundUri();
                soundChooser.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    frame.setSoundUri(soundChooser.Path);
                }

                var editTransition = useTransitions && selectedFrame.InRange(-1, workingAnimation.getFrames().Count - 1);   
                var transition = editTransition ? workingAnimation.getTranstionForFrame(selectedFrame) : emptyTransition;

                using (new EditorGUI.DisabledScope(!editTransition))
                {
                    EditorGUILayout.PrefixLabel(TC.get("NextScene.Transition"), GUIStyle.none, titleStyle);
                    EditorGUI.BeginChangeCheck();
                    var transitionDuration = EditorGUILayout.LongField(TC.get("Animation.Duration"), transition.getTime());
                    if (EditorGUI.EndChangeCheck())
                    {
                        transition.setTime(transitionDuration);
                    }

                    EditorGUI.BeginChangeCheck();
                    var transitionType = EditorGUILayout.Popup(TC.get("Conditions.Type"), (int)transition.getType(), transitionTypeName);
                    if (EditorGUI.EndChangeCheck())
                    {
                        transition.setType((TransitionType)transitionType);
                    }
                }
            }

            var lastEditorRect = GUILayoutUtility.GetLastRect();


            // Ending buttons
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("OK"))
                {
                    // If it doesnt have an extension its because its an old animation
                    if (!Path.HasExtension(cutscenePath))
                    {
                        cutscenePath = cutscenePath + ".eaa.xml";
                    }

                    AnimationWriter.WriteAnimation(cutscenePath, workingAnimation);
                    AssetDatabase.Refresh(ImportAssetOptions.Default);
                    if (reference != null)
                    {
                        reference.OnDialogOk(cutscenePath, this);
                    }
                    this.Close();
                }
                if (GUILayout.Button(TC.get("GeneralText.Cancel")))
                {
                    if(reference != null)
                    {
                        reference.OnDialogCanceled();
                    }
                    this.Close();
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                var lastButtonRect = GUILayoutUtility.GetLastRect();
                var minheight = lastEditorRect.y + lastEditorRect.height + lastEditorRect.height + 10;
                minSize = new Vector2(400, minheight);
            }
        }
    }
}
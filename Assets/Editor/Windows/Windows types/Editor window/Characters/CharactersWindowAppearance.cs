using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class CharactersWindowAppearance : LayoutWindow, DialogReceiverInterface
    {
        private const int LOOK_GROUP = 0, TALK_GROUP = 1, USE_GROUP = 2, WALK_GROUP = 3;

        public enum CharacterAnimationType
        {
            LOOKING_UP,
            LOOKING_DOWN,
            LOOKING_RIGHT,
            LOOKING_LEFT,
            TALKING_UP,
            TALKING_DOWN,
            TALKING_RIGHT,
            TALKING_LEFT,
            USE_TO_RIGHT,
            USE_TO_LEFT,
            WALKING_UP,
            WALKING_DOWN,
            WALKING_RIGHT,
            WALKING_LEFT
        };

        private Texture2D clearImg = null;

        private Texture2D slidesPreviewLookUp = null;
        private Texture2D slidesPreviewLookDown = null;
        private Texture2D slidesPreviewLookRight = null;
        private Texture2D slidesPreviewLookLeft = null;
        private Texture2D slidesPreviewTalkUp = null;
        private Texture2D slidesPreviewTalkDown = null;
        private Texture2D slidesPreviewTalkRight = null;
        private Texture2D slidesPreviewTalkLeft = null;
        private Texture2D slidesPreviewUseRight = null;
        private Texture2D slidesPreviewUseLeft = null;
        private Texture2D slidesPreviewWalkUp = null;
        private Texture2D slidesPreviewWalkDown = null;
        private Texture2D slidesPreviewWalkRight = null;
        private Texture2D slidesPreviewWalkLeft = null;

        private static Rect previewLabelsRect;
        private static Rect previewRect4_0, previewRect4_1, previewRect4_2, previewRect4_3;
        private static Rect previewRect2_0, previewRect2_1;

        private string slidesPathLookUp = "", slidesPathLookUpPreview = "";
        private string slidesPathLookDown = "", slidesPathLookDownPreview = "";
        private string slidesPathLookRight = "", slidesPathLookRightPreview = "";
        private string slidesPathLookLeft = "", slidesPathLookLeftPreview = "";
        private string slidesPathTalkUp = "", slidesPathTalkUpPreview = "";
        private string slidesPathTalkDown = "", slidesPathTalkDownPreview = "";
        private string slidesPathTalkRight = "", slidesPathTalkRightPreview = "";
        private string slidesPathTalkLeft = "", slidesPathTalkLeftPreview = "";
        private string slidesPathUseRight = "", slidesPathUseRightPreview = "";
        private string slidesPathUseLeft = "", slidesPathUseLeftPreview = "";
        private string slidesPathWalkUp = "", slidesPathWalkUpPreview = "";
        private string slidesPathWalkDown = "", slidesPathWalkDownPreview = "";
        private string slidesPathWalkRight = "", slidesPathWalkRightPreview = "";
        private string slidesPathWalkLeft = "", slidesPathWalkLeftPreview = "";

        private int selectedAnimationGroup, selectedAnimationGroupLast;
        private string[] animationGroupNamesList;

        public CharactersWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));

            selectedAnimationGroup = selectedAnimationGroupLast = 0;
            animationGroupNamesList = new string[]
            {TC.get("Resources.StandingAnimations"), TC.get("Resources.SpeakingAnimations"), TC.get("Resources.UsingAnimations"), TC.get("Resources.WalkingAnimations")};

            if (GameRources.GetInstance().selectedCharacterIndex >= 0)
            {
                slidesPathLookUp =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_STAND_UP);
                slidesPathLookDown =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_STAND_DOWN);
                slidesPathLookRight =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_STAND_RIGHT);
                slidesPathLookLeft =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_STAND_RIGHT);
                slidesPathLookUpPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_STAND_UP);
                slidesPathLookDownPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_STAND_DOWN);
                slidesPathLookRightPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_STAND_RIGHT);
                slidesPathLookLeftPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_STAND_RIGHT);



                slidesPathTalkUp =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_SPEAK_UP);
                slidesPathTalkDown =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_SPEAK_DOWN);
                slidesPathTalkRight =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_SPEAK_RIGHT);
                slidesPathTalkLeft =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_SPEAK_LEFT);
                slidesPathTalkUpPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_SPEAK_UP);
                slidesPathTalkDownPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_SPEAK_DOWN);
                slidesPathTalkRightPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_SPEAK_RIGHT);
                slidesPathTalkLeftPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_SPEAK_LEFT);



                slidesPathUseRight =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_USE_RIGHT);
                slidesPathUseLeft =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_USE_LEFT);
                slidesPathUseRightPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_USE_RIGHT);
                slidesPathUseLeftPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_USE_LEFT);



                slidesPathWalkUp =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_WALK_UP);
                slidesPathWalkDown =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_WALK_DOWN);
                slidesPathWalkRight =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_WALK_RIGHT);
                slidesPathWalkLeft =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPath(NPC.RESOURCE_TYPE_WALK_LEFT);
                slidesPathWalkUpPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_WALK_UP);
                slidesPathWalkDownPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_WALK_DOWN);
                slidesPathWalkRightPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_WALK_RIGHT);
                slidesPathWalkLeftPreview =
                    Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(
                            NPC.RESOURCE_TYPE_WALK_LEFT);


                if (!string.IsNullOrEmpty(slidesPathLookUpPreview))
                    slidesPreviewLookUp = AssetsController.getImage(slidesPathLookUpPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathLookDownPreview))
                    slidesPreviewLookDown = AssetsController.getImage(slidesPathLookDownPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathLookRightPreview))
                    slidesPreviewLookRight = AssetsController.getImage(slidesPathLookRightPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathLookLeftPreview))
                    slidesPreviewLookLeft = AssetsController.getImage(slidesPathLookLeftPreview).texture;


                if (!string.IsNullOrEmpty(slidesPathTalkUpPreview))
                    slidesPreviewTalkUp = AssetsController.getImage(slidesPathTalkUpPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathTalkDownPreview))
                    slidesPreviewTalkDown = AssetsController.getImage(slidesPathTalkDownPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathTalkRightPreview))
                    slidesPreviewTalkRight = AssetsController.getImage(slidesPathTalkRightPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathTalkLeftPreview))
                    slidesPreviewTalkLeft = AssetsController.getImage(slidesPathTalkLeftPreview).texture;


                if (!string.IsNullOrEmpty(slidesPathUseRightPreview))
                    slidesPreviewUseRight = AssetsController.getImage(slidesPathUseRightPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathUseLeftPreview))
                    slidesPreviewUseLeft = AssetsController.getImage(slidesPathUseLeftPreview).texture;


                if (!string.IsNullOrEmpty(slidesPathWalkUpPreview))
                    slidesPreviewWalkUp = AssetsController.getImage(slidesPathWalkUpPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathWalkDownPreview))
                    slidesPreviewWalkDown = AssetsController.getImage(slidesPathWalkDownPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathWalkRightPreview))
                    slidesPreviewWalkRight = AssetsController.getImage(slidesPathWalkRightPreview).texture;
                if (!string.IsNullOrEmpty(slidesPathWalkLeftPreview))
                    slidesPreviewWalkLeft = AssetsController.getImage(slidesPathWalkLeftPreview).texture;
            }

        }

        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            previewRect2_0 = new Rect(0f, 0.5f * windowHeight, 0.5f * windowWidth, 0.48f * windowHeight);
            previewRect2_1 = new Rect(0.5f * windowWidth, 0.5f * windowHeight, 0.5f * windowWidth, 0.48f * windowHeight);

            previewRect4_0 = new Rect(0f, 0.5f * windowHeight, 0.25f * windowWidth, 0.48f * windowHeight);
            previewRect4_1 = new Rect(0.25f * windowWidth, 0.5f * windowHeight, 0.25f * windowWidth, 0.48f * windowHeight);
            previewRect4_2 = new Rect(0.5f * windowWidth, 0.5f * windowHeight, 0.25f * windowWidth, 0.48f * windowHeight);
            previewRect4_3 = new Rect(0.75f * windowWidth, 0.5f * windowHeight, 0.25f * windowWidth, 0.48f * windowHeight);

            previewLabelsRect = new Rect(0f, 0.45f * windowHeight, windowWidth, 0.05f * windowHeight);

            GUILayout.Label(TC.get("Resources.ResourcesGroup"));
            selectedAnimationGroup =
                EditorGUILayout.Popup(selectedAnimationGroup, animationGroupNamesList);
            if (selectedAnimationGroup != selectedAnimationGroupLast)
                OnAnimationGroupChange(selectedAnimationGroup);

            switch (selectedAnimationGroup)
            {
                case LOOK_GROUP:
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandUp"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedLookUp("");
                    }
                    GUILayout.Box(slidesPathLookUp, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.LOOKING_UP);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathLookUp == null || slidesPathLookUp.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.LOOKING_UP);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.LOOKING_UP);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandDown"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedLookDown("");
                    }
                    GUILayout.Box(slidesPathLookDown, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.LOOKING_DOWN);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathLookDown == null || slidesPathLookDown.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.LOOKING_DOWN);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.LOOKING_DOWN);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandRight"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedLookRight("");
                    }
                    GUILayout.Box(slidesPathLookRight, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.LOOKING_RIGHT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathLookRight == null || slidesPathLookRight.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.LOOKING_RIGHT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.LOOKING_RIGHT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandLeft"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedLookLeft("");
                    }
                    GUILayout.Box(slidesPathLookLeft, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.LOOKING_LEFT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathLookLeft == null || slidesPathLookLeft.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.LOOKING_LEFT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.LOOKING_LEFT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginArea(previewLabelsRect);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandLeft"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandRight"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandUp"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationStandDown"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();

                    GUI.DrawTexture(previewRect4_0, slidesPreviewLookLeft, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_1, slidesPreviewLookRight, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_2, slidesPreviewLookUp, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_3, slidesPreviewLookDown, ScaleMode.ScaleToFit);

                    break;




                case TALK_GROUP:

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakUp"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedSpeakUp("");
                    }
                    GUILayout.Box(slidesPathTalkUp, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.TALKING_UP);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathTalkUp == null || slidesPathTalkUp.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.TALKING_UP);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.TALKING_UP);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakDown"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedSpeakDown("");
                    }
                    GUILayout.Box(slidesPathTalkDown, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.TALKING_DOWN);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathTalkDown == null || slidesPathTalkDown.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.TALKING_DOWN);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.TALKING_DOWN);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakRight"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedSpeakRight("");
                    }
                    GUILayout.Box(slidesPathTalkRight, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.TALKING_RIGHT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathTalkRight == null || slidesPathTalkRight.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.TALKING_RIGHT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.TALKING_RIGHT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakLeft"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedSpeakLeft("");
                    }
                    GUILayout.Box(slidesPathTalkLeft, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.TALKING_LEFT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathTalkLeft == null || slidesPathTalkLeft.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.TALKING_LEFT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.TALKING_LEFT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginArea(previewLabelsRect);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakLeft"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakRight"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakUp"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationSpeakDown"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();

                    GUI.DrawTexture(previewRect4_0, slidesPreviewTalkLeft, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_1, slidesPreviewTalkRight, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_2, slidesPreviewTalkUp, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_3, slidesPreviewTalkDown, ScaleMode.ScaleToFit);

                    break;



                case USE_GROUP:
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationUseRight"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedUseRight("");
                    }
                    GUILayout.Box(slidesPathUseRight, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.USE_TO_RIGHT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathUseRight == null || slidesPathUseRight.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.USE_TO_RIGHT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.USE_TO_RIGHT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationUseLeft"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedUseLeft("");
                    }
                    GUILayout.Box(slidesPathUseLeft, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.USE_TO_LEFT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathUseLeft == null || slidesPathUseLeft.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.USE_TO_LEFT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.USE_TO_LEFT);
                        }
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.BeginArea(previewLabelsRect);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationUseLeft"), GUILayout.Width(0.5f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationUseRight"), GUILayout.Width(0.5f * windowWidth));
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();

                    GUI.DrawTexture(previewRect2_0, slidesPreviewUseLeft, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect2_1, slidesPreviewUseRight, ScaleMode.ScaleToFit);

                    break;




                case WALK_GROUP:

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkUp"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedWalkUp("");
                    }
                    GUILayout.Box(slidesPathWalkUp, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.WALKING_UP);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathWalkUp == null || slidesPathWalkUp.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.WALKING_UP);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.WALKING_UP);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkDown"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedWalkDown("");
                    }
                    GUILayout.Box(slidesPathWalkDown, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.WALKING_DOWN);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathWalkDown == null || slidesPathWalkDown.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.WALKING_DOWN);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.WALKING_DOWN);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkRight"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedWalkRight("");
                    }
                    GUILayout.Box(slidesPathWalkRight, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.WALKING_RIGHT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathWalkRight == null || slidesPathWalkRight.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.WALKING_RIGHT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.WALKING_RIGHT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkLeft"));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                    {
                        OnSlidesceneChangedWalkLeft("");
                    }
                    GUILayout.Box(slidesPathWalkLeft, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
                    {
                        ShowAssetChooser(CharacterAnimationType.WALKING_LEFT);
                    }
                    // Create/edit slidescene
                    if (GUILayout.Button(TC.get("Resources.Create") + "/" + TC.get("Resources.Edit"), GUILayout.Width(0.2f * windowWidth)))
                    {
                        // For not-existing cutscene - show new cutscene name dialog
                        if (slidesPathWalkLeft == null || slidesPathWalkLeft.Equals(""))
                        {
                            CutsceneNameInputPopup createCutsceneDialog =
                                (CutsceneNameInputPopup)ScriptableObject.CreateInstance(typeof(CutsceneNameInputPopup));
                            createCutsceneDialog.Init(this, "", CharacterAnimationType.WALKING_LEFT);
                        }
                        else
                        {
                            EditCutscene(CharacterAnimationType.WALKING_LEFT);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginArea(previewLabelsRect);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkLeft"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkRight"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkUp"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.Label(TC.get("Resources.DescriptionCharacterAnimationWalkDown"), GUILayout.Width(0.25f * windowWidth));
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();

                    GUI.DrawTexture(previewRect4_0, slidesPreviewWalkLeft, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_1, slidesPreviewWalkRight, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_2, slidesPreviewWalkUp, ScaleMode.ScaleToFit);
                    GUI.DrawTexture(previewRect4_3, slidesPreviewWalkDown, ScaleMode.ScaleToFit);

                    break;
            }
        }

        public void OnAnimationGroupChange(int i)
        {
            selectedAnimationGroupLast = i;
        }

        void ShowAssetChooser(CharacterAnimationType type)
        {
            AnimationCharacterFileOpenDialog animationDialog =
                (AnimationCharacterFileOpenDialog)
                    ScriptableObject.CreateInstance(typeof(AnimationCharacterFileOpenDialog));
            animationDialog.Init(this, BaseFileOpenDialog.FileType.CHARACTER_ANIM, type);

        }

        void EditCutscene(CharacterAnimationType type)
        {
            CutsceneSlidesEditor slidesEditor =
                (CutsceneSlidesEditor)ScriptableObject.CreateInstance(typeof(CutsceneSlidesEditor));
            switch (type)
            {
                case CharacterAnimationType.LOOKING_UP:
                    slidesEditor.Init(this, slidesPathLookUp);
                    break;
                case CharacterAnimationType.LOOKING_DOWN:
                    slidesEditor.Init(this, slidesPathLookDown);
                    break;
                case CharacterAnimationType.LOOKING_RIGHT:
                    slidesEditor.Init(this, slidesPathLookRight);
                    break;
                case CharacterAnimationType.LOOKING_LEFT:
                    slidesEditor.Init(this, slidesPathLookLeft);
                    break;
                case CharacterAnimationType.TALKING_UP:
                    slidesEditor.Init(this, slidesPathTalkUp);
                    break;
                case CharacterAnimationType.TALKING_DOWN:
                    slidesEditor.Init(this, slidesPathTalkDown);
                    break;
                case CharacterAnimationType.TALKING_RIGHT:
                    slidesEditor.Init(this, slidesPathTalkRight);
                    break;
                case CharacterAnimationType.TALKING_LEFT:
                    slidesEditor.Init(this, slidesPathTalkLeft);
                    break;
                case CharacterAnimationType.USE_TO_RIGHT:
                    slidesEditor.Init(this, slidesPathUseRight);
                    break;
                case CharacterAnimationType.USE_TO_LEFT:
                    slidesEditor.Init(this, slidesPathUseLeft);
                    break;
                case CharacterAnimationType.WALKING_UP:
                    slidesEditor.Init(this, slidesPathWalkUp);
                    break;
                case CharacterAnimationType.WALKING_DOWN:
                    slidesEditor.Init(this, slidesPathWalkDown);
                    break;
                case CharacterAnimationType.WALKING_RIGHT:
                    slidesEditor.Init(this, slidesPathWalkRight);
                    break;
                case CharacterAnimationType.WALKING_LEFT:
                    slidesEditor.Init(this, slidesPathWalkLeft);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            Debug.Log(message);
            // After new cutscene name was choosed
            if (workingObject is CutsceneNameInputPopup)
            {
                CharacterAnimationType type = (CharacterAnimationType)workingObjectSecond;
                //TODO: create file?
                switch (type)
                {
                    case CharacterAnimationType.LOOKING_UP:
                        OnSlidesceneChangedLookUp(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.LOOKING_DOWN:
                        OnSlidesceneChangedLookDown(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.LOOKING_RIGHT:
                        OnSlidesceneChangedLookRight(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.LOOKING_LEFT:
                        OnSlidesceneChangedLookLeft(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;

                    case CharacterAnimationType.TALKING_UP:
                        OnSlidesceneChangedSpeakUp(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.TALKING_DOWN:
                        OnSlidesceneChangedSpeakDown(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.TALKING_RIGHT:
                        OnSlidesceneChangedSpeakRight(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.TALKING_LEFT:
                        OnSlidesceneChangedSpeakLeft(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;

                    case CharacterAnimationType.USE_TO_RIGHT:
                        OnSlidesceneChangedUseRight(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.USE_TO_LEFT:
                        OnSlidesceneChangedUseLeft(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;

                    case CharacterAnimationType.WALKING_UP:
                        OnSlidesceneChangedWalkUp(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.WALKING_DOWN:
                        OnSlidesceneChangedWalkDown(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.WALKING_RIGHT:
                        OnSlidesceneChangedWalkRight(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    case CharacterAnimationType.WALKING_LEFT:
                        OnSlidesceneChangedWalkLeft(message);
                        //Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex].set(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                EditCutscene(type);
                return;
            }

            if (workingObject is BaseFileOpenDialog.FileType)
            {
                CharacterAnimationType type = (CharacterAnimationType)workingObjectSecond;
                //TODO: create file?
                switch (type)
                {
                    case CharacterAnimationType.LOOKING_UP:
                        OnSlidesceneChangedLookUp(message);
                        break;
                    case CharacterAnimationType.LOOKING_DOWN:
                        OnSlidesceneChangedLookDown(message);
                        break;
                    case CharacterAnimationType.LOOKING_RIGHT:
                        OnSlidesceneChangedLookRight(message);
                        break;
                    case CharacterAnimationType.LOOKING_LEFT:
                        OnSlidesceneChangedLookLeft(message);
                        break;

                    case CharacterAnimationType.TALKING_UP:
                        OnSlidesceneChangedSpeakUp(message);
                        break;
                    case CharacterAnimationType.TALKING_DOWN:
                        OnSlidesceneChangedSpeakDown(message);
                        break;
                    case CharacterAnimationType.TALKING_RIGHT:
                        OnSlidesceneChangedSpeakRight(message);
                        break;
                    case CharacterAnimationType.TALKING_LEFT:
                        OnSlidesceneChangedSpeakLeft(message);
                        break;

                    case CharacterAnimationType.USE_TO_RIGHT:
                        OnSlidesceneChangedUseRight(message);
                        break;
                    case CharacterAnimationType.USE_TO_LEFT:
                        OnSlidesceneChangedUseLeft(message);
                        break;

                    case CharacterAnimationType.WALKING_UP:
                        OnSlidesceneChangedWalkUp(message);
                        break;
                    case CharacterAnimationType.WALKING_DOWN:
                        OnSlidesceneChangedWalkDown(message);
                        break;
                    case CharacterAnimationType.WALKING_RIGHT:
                        OnSlidesceneChangedWalkRight(message);
                        break;
                    case CharacterAnimationType.WALKING_LEFT:
                        OnSlidesceneChangedWalkLeft(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log("Wiadomość nie OK");
        }

        #region Change events 

        private void OnSlidesceneChangedWalkLeft(string v)
        {
            slidesPathWalkLeft = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_WALK_LEFT, v);

            slidesPathWalkLeftPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_WALK_LEFT);

            if (!string.IsNullOrEmpty(slidesPathWalkLeftPreview))
                slidesPreviewWalkLeft = AssetsController.getImage(slidesPathWalkLeftPreview).texture;
        }

        private void OnSlidesceneChangedWalkRight(string v)
        {
            slidesPathWalkRight = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_WALK_RIGHT, v);

            slidesPathWalkRightPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_WALK_RIGHT);

            if (!string.IsNullOrEmpty(slidesPathWalkRightPreview))
                slidesPreviewWalkRight = AssetsController.getImage(slidesPathWalkRightPreview).texture;
        }

        private void OnSlidesceneChangedWalkDown(string v)
        {
            slidesPathWalkDown = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_WALK_DOWN, v);

            slidesPathWalkDownPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_WALK_DOWN);

            if (!string.IsNullOrEmpty(slidesPathWalkDownPreview))
                slidesPreviewWalkDown = AssetsController.getImage(slidesPathWalkDownPreview).texture;
        }

        private void OnSlidesceneChangedWalkUp(string v)
        {
            slidesPathWalkUp = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_WALK_UP, v);

            slidesPathWalkUpPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_WALK_UP);

            if (!string.IsNullOrEmpty(slidesPathWalkUpPreview))
                slidesPreviewWalkUp = AssetsController.getImage(slidesPathWalkUpPreview).texture;
        }



        private void OnSlidesceneChangedUseLeft(string v)
        {
            slidesPathUseLeft = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_USE_LEFT, v);

            slidesPathUseLeftPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_USE_LEFT);

            if (!string.IsNullOrEmpty(slidesPathUseLeftPreview))
                slidesPreviewUseLeft = AssetsController.getImage(slidesPathUseLeftPreview).texture;
        }

        private void OnSlidesceneChangedUseRight(string v)
        {
            slidesPathUseRight = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_USE_RIGHT, v);

            slidesPathUseRightPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_USE_RIGHT);

            if (!string.IsNullOrEmpty(slidesPathUseRightPreview))
                slidesPreviewUseRight = AssetsController.getImage(slidesPathUseRightPreview).texture;
        }



        private void OnSlidesceneChangedSpeakLeft(string v)
        {
            slidesPathTalkLeft = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_SPEAK_LEFT, v);

            slidesPathTalkLeftPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_SPEAK_LEFT);

            if (!string.IsNullOrEmpty(slidesPathTalkLeftPreview))
                slidesPreviewTalkLeft = AssetsController.getImage(slidesPathTalkLeftPreview).texture;
        }

        private void OnSlidesceneChangedSpeakRight(string v)
        {
            slidesPathTalkRight = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_SPEAK_RIGHT, v);

            slidesPathTalkRightPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_SPEAK_RIGHT);

            if (!string.IsNullOrEmpty(slidesPathTalkRightPreview))
                slidesPreviewTalkRight = AssetsController.getImage(slidesPathTalkRightPreview).texture;
        }

        private void OnSlidesceneChangedSpeakDown(string v)
        {
            slidesPathTalkDown = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_SPEAK_DOWN, v);

            slidesPathTalkDownPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_SPEAK_DOWN);

            if (!string.IsNullOrEmpty(slidesPathTalkDownPreview))
                slidesPreviewTalkDown = AssetsController.getImage(slidesPathTalkDownPreview).texture;
        }

        private void OnSlidesceneChangedSpeakUp(string v)
        {
            slidesPathTalkUp = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_SPEAK_UP, v);

            slidesPathTalkUpPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_SPEAK_UP);

            if (!string.IsNullOrEmpty(slidesPathTalkUpPreview))
                slidesPreviewTalkUp = AssetsController.getImage(slidesPathTalkUpPreview).texture;
        }



        private void OnSlidesceneChangedLookLeft(string v)
        {
            slidesPathLookLeft = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_STAND_LEFT, v);

            slidesPathLookLeftPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_STAND_LEFT);

            if (!string.IsNullOrEmpty(slidesPathLookLeftPreview))
                slidesPreviewLookLeft = AssetsController.getImage(slidesPathLookLeftPreview).texture;
        }

        private void OnSlidesceneChangedLookRight(string v)
        {
            slidesPathLookRight = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_STAND_RIGHT, v);

            slidesPathLookRightPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_STAND_RIGHT);

            if (!string.IsNullOrEmpty(slidesPathLookRightPreview))
                slidesPreviewLookRight = AssetsController.getImage(slidesPathLookRightPreview).texture;
        }

        private void OnSlidesceneChangedLookDown(string v)
        {
            slidesPathLookDown = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_STAND_DOWN, v);

            slidesPathLookDownPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_STAND_DOWN);

            if (!string.IsNullOrEmpty(slidesPathLookDownPreview))
                slidesPreviewLookDown = AssetsController.getImage(slidesPathLookDownPreview).texture;
        }

        private void OnSlidesceneChangedLookUp(string v)
        {
            slidesPathLookUp = v;

            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].addAnimationPath(NPC.RESOURCE_TYPE_STAND_UP, v);

            slidesPathLookUpPreview =
                Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getAnimationPathPreview(NPC.RESOURCE_TYPE_STAND_UP);

            if (!string.IsNullOrEmpty(slidesPathLookUpPreview))
                slidesPreviewLookUp = AssetsController.getImage(slidesPathLookUpPreview).texture;
        }

        #endregion
    }
}
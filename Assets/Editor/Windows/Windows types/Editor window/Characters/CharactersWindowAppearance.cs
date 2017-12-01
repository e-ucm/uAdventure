using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(NPCDataControl), typeof(PlayerDataControl), typeof(NodeDataControl), Name = "NPC.LookPanelTitle", Order = 5)]
    public class CharactersWindowAppearance : AbstractEditorComponentWithPreview
    {
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

        private const int LOOK_GROUP = 0, TALK_GROUP = 1, USE_GROUP = 2, WALK_GROUP = 3;
        
        private NPCDataControl workingCharacter;

        private AppearanceEditor appearanceEditor;

        private AnimationField standingUp;
        private AnimationField standingLeft;
        private AnimationField standingRight;
        private AnimationField standingDown;
        private AnimationField walkingUp;
        private AnimationField walkingLeft;
        private AnimationField walkingRight;
        private AnimationField walkingDown;
        private AnimationField usingLeft;
        private AnimationField usingRight;
        private AnimationField talkingUp;
        private AnimationField talkingLeft;
        private AnimationField talkingRight;
        private AnimationField talkingDown;

        private Texture2D standingUpTex;
        private Texture2D standingLeftTex;
        private Texture2D standingRightTex;
        private Texture2D standingDownTex;
        private Texture2D walkingUpTex;
        private Texture2D walkingLeftTex;
        private Texture2D walkingRightTex;
        private Texture2D walkingDownTex;
        private Texture2D usingLeftTex;
        private Texture2D usingRightTex;
        private Texture2D talkingUpTex;
        private Texture2D talkingLeftTex;
        private Texture2D talkingRightTex;
        private Texture2D talkingDownTex;

        private int selectedAnimationGroup;
        private string[] animationGroupNamesList;

        public CharactersWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshPathInformation;
            
            animationGroupNamesList = new string[] {TC.get("Resources.StandingAnimations"), TC.get("Resources.SpeakingAnimations"), TC.get("Resources.UsingAnimations"), TC.get("Resources.WalkingAnimations")};
            
            standingUp = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationStandUp"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            standingLeft = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationStandLeft"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            standingRight = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationStandRight"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            standingDown = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationStandDown"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            talkingUp = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationSpeakUp"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            talkingRight = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationSpeakRight"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            talkingLeft = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationSpeakLeft"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            talkingDown = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationSpeakDown"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            usingLeft = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationUseRight"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            usingRight = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationUseLeft"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            walkingUp = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationWalkUp"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            walkingLeft = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationWalkLeft"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            walkingRight = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationWalkRight"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

            walkingDown = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionCharacterAnimationWalkDown"),
                FileType = BaseFileOpenDialog.FileType.CHARACTER_ANIM
            };

        }

        private void DoAnimationSelector(AnimationField field, string animationName)
        {
            EditorGUI.BeginChangeCheck();
            field.Path = workingCharacter.getAnimationPath(animationName);
            field.DoLayout();
            if (EditorGUI.EndChangeCheck())
            {
                workingCharacter.addAnimationPath(animationName, field.Path);
                RefreshPathInformation(workingCharacter);
            }
        }

        private Texture2D LoadCharacterTexturePreview(NPCDataControl data, string animation)
        {
            var auxPath = data.getAnimationPathPreview(animation);
            var image = string.IsNullOrEmpty(auxPath) ? null : AssetsController.getImage(auxPath);
            return image != null ? image.texture : null;
        }

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var npc = data as NPCDataControl;

            standingUpTex        = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_UP);
            standingLeftTex      = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_DOWN);
            standingRightTex     = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_RIGHT);
            standingDownTex      = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_LEFT);
            talkingUpTex         = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_SPEAK_UP);
            talkingLeftTex       = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_SPEAK_DOWN);
            talkingRightTex      = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_SPEAK_RIGHT);
            talkingDownTex       = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_SPEAK_LEFT);
            usingLeftTex         = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_USE_RIGHT);
            usingRightTex        = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_USE_LEFT);
            walkingUpTex         = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_WALK_UP);
            walkingLeftTex       = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_WALK_DOWN);
            walkingRightTex      = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_WALK_RIGHT);
            walkingDownTex       = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_WALK_LEFT);
        }

        protected override void DrawInspector()
        {
            if (Target is NodeDataControl)
                Target = Controller.Instance.SelectedChapterDataControl.getPlayer();

            workingCharacter = Target != null ? Target as NPCDataControl : Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex];

            // Appearance table
            appearanceEditor.Data = workingCharacter;
            appearanceEditor.OnInspectorGUI();

            GUILayout.Label(TC.get("Resources.ResourcesGroup"));
            selectedAnimationGroup = EditorGUILayout.Popup(selectedAnimationGroup, animationGroupNamesList);

            switch (selectedAnimationGroup)
            {
                case LOOK_GROUP:
                    DoAnimationSelector(standingUp, NPC.RESOURCE_TYPE_STAND_UP);
                    DoAnimationSelector(standingDown, NPC.RESOURCE_TYPE_STAND_DOWN);
                    DoAnimationSelector(standingRight, NPC.RESOURCE_TYPE_STAND_RIGHT);
                    DoAnimationSelector(standingLeft, NPC.RESOURCE_TYPE_STAND_LEFT);
                    break;

                case TALK_GROUP:
                    DoAnimationSelector(talkingUp, NPC.RESOURCE_TYPE_SPEAK_UP);
                    DoAnimationSelector(talkingDown, NPC.RESOURCE_TYPE_SPEAK_DOWN);
                    DoAnimationSelector(talkingLeft, NPC.RESOURCE_TYPE_SPEAK_RIGHT);
                    DoAnimationSelector(talkingRight, NPC.RESOURCE_TYPE_SPEAK_LEFT);
                    break;

                case USE_GROUP:
                    DoAnimationSelector(usingLeft, NPC.RESOURCE_TYPE_USE_RIGHT);
                    DoAnimationSelector(usingRight, NPC.RESOURCE_TYPE_USE_LEFT);
                    break;

                case WALK_GROUP:
                    DoAnimationSelector(walkingUp, NPC.RESOURCE_TYPE_WALK_UP);
                    DoAnimationSelector(walkingDown, NPC.RESOURCE_TYPE_WALK_DOWN);
                    DoAnimationSelector(walkingLeft, NPC.RESOURCE_TYPE_WALK_RIGHT);
                    DoAnimationSelector(walkingRight, NPC.RESOURCE_TYPE_WALK_LEFT);
                    break;
            }
        }

        private void DrawSinglePreview(string title, Texture2D texture)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(200, 50), title);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (texture) GUI.DrawTexture(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)), texture, ScaleMode.ScaleToFit);

            EditorGUILayout.EndVertical();
        }

        public override void DrawPreview(Rect rect)
        {
            if(Target != null)
            {
                if(Event.current.type == EventType.repaint)
                {
                    var npc = Target as NPCDataControl;
                    var preview = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_DOWN);
                    if (preview) GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                switch (selectedAnimationGroup)
                {
                    case LOOK_GROUP:
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationStandUp"), standingUpTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationStandLeft"), standingLeftTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationStandRight"), standingRightTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationStandDown"), standingDownTex);
                        break;

                    case TALK_GROUP:
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationSpeakUp"), talkingUpTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationSpeakLeft"), talkingLeftTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationSpeakRight"), talkingRightTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationSpeakDown"), talkingDownTex);
                        break;

                    case USE_GROUP:
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationUseLeft"), usingLeftTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationUseRight"), usingRightTex);
                        break;
                    case WALK_GROUP:
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationWalkUp"), walkingUpTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationWalkLeft"), walkingLeftTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationWalkRight"), walkingRightTex);
                        DrawSinglePreview(TC.get("Resources.DescriptionCharacterAnimationWalkDown"), walkingDownTex);
                        break;
                }
                GUILayout.EndHorizontal();
            }
        }

        public override void OnRender(Rect viewport)
        {
            if (Target is NodeDataControl)
            {
                Target = Controller.Instance.SelectedChapterDataControl.getPlayer();
            }
            
            var npc = Target as NPCDataControl;

            var preview = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_DOWN);

            if (preview)
            {
                var rect = GetViewportRect(new Rect(new Vector2(-0.5f * preview.width, -preview.height), new Vector2(preview.width, preview.height)), viewport);
                
                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }
        }
    }
}
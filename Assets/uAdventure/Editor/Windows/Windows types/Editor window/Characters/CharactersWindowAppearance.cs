using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(NPCDataControl), typeof(PlayerDataControl), typeof(NodeDataControl), Name = "NPC.LookPanelTitle", Order = 5)]
    public class CharactersWindowAppearance : AbstractEditorComponentWithPreview
    {
        public bool IsPlayer { get; set; }

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

        private enum CharacterAnimationsGroup { LOOK_GROUP, TALK_GROUP, USE_GROUP, WALK_GROUP }

        private static Dictionary<CharacterAnimationsGroup, string> groupNames = new Dictionary<CharacterAnimationsGroup, string>()
        {
            { CharacterAnimationsGroup.LOOK_GROUP, "Resources.StandingAnimations" },
            { CharacterAnimationsGroup.TALK_GROUP, "Resources.SpeakingAnimations" },
            { CharacterAnimationsGroup.USE_GROUP,  "Resources.UsingAnimations" },
            { CharacterAnimationsGroup.WALK_GROUP, "Resources.WalkingAnimations" }
        };
        private static Dictionary<CharacterAnimationType, string> fieldNames = new Dictionary<CharacterAnimationType, string>()
        {
            { CharacterAnimationType.LOOKING_UP,    "Resources.DescriptionCharacterAnimationStandUp" },
            { CharacterAnimationType.LOOKING_DOWN,  "Resources.DescriptionCharacterAnimationStandDown" },
            { CharacterAnimationType.LOOKING_RIGHT, "Resources.DescriptionCharacterAnimationStandRight" },
            { CharacterAnimationType.LOOKING_LEFT,  "Resources.DescriptionCharacterAnimationStandLeft" },
            { CharacterAnimationType.TALKING_UP,    "Resources.DescriptionCharacterAnimationSpeakUp" },
            { CharacterAnimationType.TALKING_DOWN,  "Resources.DescriptionCharacterAnimationSpeakDown" },
            { CharacterAnimationType.TALKING_RIGHT, "Resources.DescriptionCharacterAnimationSpeakRight" },
            { CharacterAnimationType.TALKING_LEFT,  "Resources.DescriptionCharacterAnimationSpeakLeft " },
            { CharacterAnimationType.USE_TO_RIGHT,  "Resources.DescriptionCharacterAnimationUseRight" },
            { CharacterAnimationType.USE_TO_LEFT,   "Resources.DescriptionCharacterAnimationUseLeft" },
            { CharacterAnimationType.WALKING_UP,    "Resources.DescriptionCharacterAnimationWalkUp" },
            { CharacterAnimationType.WALKING_DOWN,  "Resources.DescriptionCharacterAnimationWalkDown" },
            { CharacterAnimationType.WALKING_RIGHT, "Resources.DescriptionCharacterAnimationWalkRight" },
            { CharacterAnimationType.WALKING_LEFT,  "Resources.DescriptionCharacterAnimationWalkLeft" }
        };

        private static Dictionary<CharacterAnimationsGroup, Dictionary<CharacterAnimationType, string>> resourceTypeGroups = new Dictionary<CharacterAnimationsGroup, Dictionary<CharacterAnimationType, string>>()
        {
            {
                CharacterAnimationsGroup.LOOK_GROUP, new Dictionary<CharacterAnimationType, string>()
                {
                    { CharacterAnimationType.LOOKING_UP,    NPC.RESOURCE_TYPE_STAND_UP },
                    { CharacterAnimationType.LOOKING_DOWN,  NPC.RESOURCE_TYPE_STAND_DOWN },
                    { CharacterAnimationType.LOOKING_RIGHT, NPC.RESOURCE_TYPE_STAND_RIGHT },
                    { CharacterAnimationType.LOOKING_LEFT,  NPC.RESOURCE_TYPE_STAND_LEFT }
                }
            },
            {
                CharacterAnimationsGroup.TALK_GROUP, new Dictionary<CharacterAnimationType, string>()
                {
                    { CharacterAnimationType.TALKING_UP,    NPC.RESOURCE_TYPE_SPEAK_UP },
                    { CharacterAnimationType.TALKING_DOWN,  NPC.RESOURCE_TYPE_SPEAK_DOWN },
                    { CharacterAnimationType.TALKING_RIGHT, NPC.RESOURCE_TYPE_SPEAK_RIGHT },
                    { CharacterAnimationType.TALKING_LEFT,  NPC.RESOURCE_TYPE_SPEAK_LEFT }
                }
            },
            {
                CharacterAnimationsGroup.USE_GROUP, new Dictionary<CharacterAnimationType, string>()
                {
                    { CharacterAnimationType.USE_TO_RIGHT,  NPC.RESOURCE_TYPE_USE_RIGHT },
                    { CharacterAnimationType.USE_TO_LEFT,   NPC.RESOURCE_TYPE_USE_LEFT }
                }
            },
            {
                CharacterAnimationsGroup.WALK_GROUP, new Dictionary<CharacterAnimationType, string>()
                {
                    { CharacterAnimationType.WALKING_UP,    NPC.RESOURCE_TYPE_WALK_UP },
                    { CharacterAnimationType.WALKING_DOWN,  NPC.RESOURCE_TYPE_WALK_DOWN },
                    { CharacterAnimationType.WALKING_RIGHT, NPC.RESOURCE_TYPE_WALK_RIGHT },
                    { CharacterAnimationType.WALKING_LEFT,  NPC.RESOURCE_TYPE_WALK_LEFT }
                }
            }
        };

        private static CharacterAnimationsGroup[] groups;
        private static CharacterAnimationType[] types;

        private Dictionary<CharacterAnimationType, AnimationField> fields = new Dictionary<CharacterAnimationType, AnimationField>();
        private Dictionary<CharacterAnimationType, Texture2D> textures = new Dictionary<CharacterAnimationType, Texture2D>();
        
        private CharacterAnimationsGroup selectedAnimationGroup;

        private NPCDataControl workingCharacter;

        private AppearanceEditor appearanceEditor;

        public CharactersWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            IsPlayer = false;

            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshPathInformation;

            if(groups == null)
                groups = Enum.GetValues(typeof(CharacterAnimationsGroup)).Cast<CharacterAnimationsGroup>().ToArray();
            if(types == null)
                types = Enum.GetValues(typeof(CharacterAnimationType)).Cast<CharacterAnimationType>().ToArray();
            
            foreach(var animationType in types)
            {
                fields[animationType] = new AnimationField()
                {
                    Label = TC.get(fieldNames[animationType]),
                    FileType = FileType.CHARACTER_ANIM
                };
            }
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
            return string.IsNullOrEmpty(auxPath) ? null : Controller.ResourceManager.getImage(auxPath);
        }

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var npc = data as NPCDataControl;
            foreach(var group in groups) // For each group
            {
                foreach (var animationType in resourceTypeGroups[group])
                {
                    // Reload the texture
                    textures[animationType.Key] = LoadCharacterTexturePreview(npc, resourceTypeGroups[group][animationType.Key]);
                }
            }
        }

        protected override void DrawInspector()
        {
            var prevWorkingChar = workingCharacter;

            var player = Controller.Instance.SelectedChapterDataControl.getPlayer();
            if (Target is NodeDataControl || IsPlayer)
                workingCharacter = player;
            else if (Target is NPCDataControl)
                workingCharacter = Target as NPCDataControl;
            else
                workingCharacter = Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex];

            if (workingCharacter != prevWorkingChar)
                RefreshPathInformation(workingCharacter);

            // Appearance table
            appearanceEditor.Data = workingCharacter;
            appearanceEditor.OnInspectorGUI();

            GUILayout.Label(TC.get("Resources.ResourcesGroup"));
            selectedAnimationGroup = groups[EditorGUILayout.Popup((int)selectedAnimationGroup, groupNames.Select(kv => TC.get(kv.Value)).ToArray())];

            // Draw the animation selector for each animation in the selected resource group
            foreach(var resourceTypeGroup in resourceTypeGroups[selectedAnimationGroup])
            {
                DoAnimationSelector(fields[resourceTypeGroup.Key], resourceTypeGroup.Value);
            }
        }

        private void DrawSinglePreview(Texture2D texture, string title = null)
        {
            EditorGUILayout.BeginVertical();
            if (!string.IsNullOrEmpty(title))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(200, 50), title);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (texture) GUI.DrawTexture(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)), texture, ScaleMode.ScaleToFit);

            EditorGUILayout.EndVertical();
        }

        public override void DrawPreview(Rect rect)
        {
            GUILayout.BeginHorizontal();
            if (Target != null)
            {
                var npc = Target as NPCDataControl;
                var preview = LoadCharacterTexturePreview(npc, NPC.RESOURCE_TYPE_STAND_DOWN);
                DrawSinglePreview(preview);
            }
            else
            {
                // Draw the animation selector for each animation in the selected resource group
                foreach (var resourceTypeGroup in resourceTypeGroups[selectedAnimationGroup])
                {
                    DrawSinglePreview(textures[resourceTypeGroup.Key], TC.get(fieldNames[resourceTypeGroup.Key]));
                }
            }
            GUILayout.EndHorizontal();
        }

        public override void OnRender(Rect viewport)
        {
            if (Target is NodeDataControl)
            {
                Target = Controller.Instance.SelectedChapterDataControl.getPlayer();
            }
            
            var npc = Target as NPCDataControl;
            var previewPath = npc.getPreviewImage();
            if (!string.IsNullOrEmpty(previewPath))
            {
                var preview = Controller.ResourceManager.getImage(previewPath);
                var rect = GetViewportRect(new Rect(new Vector2(-0.5f * preview.width, -preview.height), new Vector2(preview.width, preview.height)), viewport);
                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }
        }
    }
}
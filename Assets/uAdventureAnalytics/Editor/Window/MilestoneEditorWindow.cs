using UnityEngine;
using UnityEditor;
using System;

using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Analytics
{
    public class MilestoneEditorWindow : EditorWindow
    {
        private static MilestoneEditorWindow editor;
        public enum MilestoneType { SCENE, ITEM, CHARACTER, COMPLETABLE, CONDITION };
        private readonly string[] milestonetypes =
        {
            "Scene has been visited",
            "Player interacts with Item",
            "Player interacts with character",
            "A completable has been completed",
            "A condition is true"
        };

        GUIStyle conditionStyle, eitherConditionStyle, closeStyle, collapseStyle;
        
        public MilestoneDataControl Milestone { get; set; }

        public static MilestoneEditorWindow Create(MilestoneDataControl mil)
        {
            editor = ScriptableObject.CreateInstance<MilestoneEditorWindow>();
            editor.Milestone = mil;
            return editor;
        }


        public static void ShowMilestoneEditor(Rect rect, MilestoneDataControl milestone)
        {
            var window = MilestoneEditorWindow.Create(milestone);
            rect.position = GUIUtility.GUIToScreenPoint(rect.position);
            window.ShowAsDropDown(rect, new Vector2(Mathf.Max(rect.width, 250), 300));
        }

        protected void OnGUI()
        {

            if (conditionStyle == null)
            {
                conditionStyle = new GUIStyle(GUI.skin.box);
                conditionStyle.normal.background = TextureUtil.MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
            }

            if (eitherConditionStyle == null)
            {
                eitherConditionStyle = new GUIStyle(GUI.skin.box);
                eitherConditionStyle.normal.background = TextureUtil.MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
                eitherConditionStyle.padding.left = 15;
            }

            if (closeStyle == null)
            {
                closeStyle = new GUIStyle(GUI.skin.button);
                closeStyle.padding = new RectOffset(0, 0, 0, 0);
                closeStyle.margin = new RectOffset(0, 5, 2, 0);
                closeStyle.normal.textColor = Color.red;
                closeStyle.focused.textColor = Color.red;
                closeStyle.active.textColor = Color.red;
                closeStyle.hover.textColor = Color.red;
            }

            if (collapseStyle == null)
            {
                collapseStyle = new GUIStyle(GUI.skin.button);
                collapseStyle.padding = new RectOffset(0, 0, 0, 0);
                collapseStyle.margin = new RectOffset(0, 5, 2, 0);
                collapseStyle.normal.textColor = Color.blue;
                collapseStyle.focused.textColor = Color.blue;
                collapseStyle.active.textColor = Color.blue;
                collapseStyle.hover.textColor = Color.blue;
            }

            using (new GUILayout.VerticalScope())
            {
                var previous = Milestone.getType();
                GUILayout.Label("The milestone will be reached when");
                Milestone.setType((Completable.Milestone.MilestoneType)EditorGUILayout.Popup((int)Milestone.getType(), milestonetypes));
                if (previous != Milestone.getType())
                {
                    Milestone.setId("");
                }

                switch (Milestone.getType())
                {
                    case Completable.Milestone.MilestoneType.CHARACTER:
                        SelectElement<NPC>(Milestone, "Character:");
                        break;
                    case Completable.Milestone.MilestoneType.ITEM:
                        SelectElement<Item>(Milestone, "Item:");
                        break;
                    case Completable.Milestone.MilestoneType.SCENE:
                        SelectElement<IChapterTarget>(Milestone, "Scene:");
                        break;
                    case Completable.Milestone.MilestoneType.COMPLETABLE:
                        SelectElement<Completable>(Milestone, "Completable:");
                        break;
                    case Completable.Milestone.MilestoneType.CONDITION:
                        if (Milestone.getConditions() == null)
                        {
                            Milestone.setConditions(new ConditionsController(new Conditions()));
                        }

                        using (new GUILayout.VerticalScope(conditionStyle))
                        {
                            GUILayout.Label("CONDITIONS");
                            if (GUILayout.Button("Add Block"))
                            {
                                Milestone.getConditions().Conditions.Add(new FlagCondition(""));
                            }

                            //##################################################################################
                            //############################### CONDITION HANDLING ###############################
                            //##################################################################################
                            
                            var conditions = Milestone.getConditions().Conditions;
                            ConditionEditorWindow.LayoutConditionEditor(conditions);

                            //##################################################################################

                            
                        }

                        break;
                }

                if (GUILayout.Button("Save milestone"))
                {
                    this.Close();
                }
            }
        }

        private void SelectElement<T>(MilestoneDataControl milestone, string label)
        {
            var ids = Controller.Instance.IdentifierSummary.getIds<T>();
            if(ids.Length == 0)
            {
                EditorGUILayout.HelpBox("There are no elements for the selected type!", MessageType.Error);
                return;
            }

            var newId = ids[EditorGUILayout.Popup(label, Mathf.Max(0, Array.IndexOf(ids, milestone.getId())), ids)];
            Milestone.setId(newId);
        }
    }
}
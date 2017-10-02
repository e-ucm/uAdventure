using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(NPCDataControl), Name = "NPC.ActionsPanelTitle", Order = 10)]
    public class CharactersWindowActions : AbstractEditorComponent
    {
        private Texture2D conditionsTex = null;
        private Texture2D noConditionsTex = null;

        private string[] itemsNames;
        private string[] charactersNames;
        private string[] joinedNamesList;

        private string[] activeAreas, characters, items;

        private DataControlList actionsList;

        public CharactersWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));
            

            itemsNames = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            charactersNames = Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCsIDs();
            // Both scenes and cutscenes are necessary for next scene popup
            joinedNamesList = new string[itemsNames.Length + charactersNames.Length + 1];
            joinedNamesList[0] = "none";
            itemsNames.CopyTo(joinedNamesList, 1);
            charactersNames.CopyTo(joinedNamesList, itemsNames.Length + 1);
            
            actionsList = new DataControlList()
            {
                elementHeight = 40,
                Columns = new System.Collections.Generic.List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = TC.get("ActionsList.ActionName"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(150) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("DescriptionList.Description"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("ActionsList.NeedsGoTo"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(120) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("ActionsList.Conditions"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(70) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Element.Effects"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(70) }
                    }
                },
                drawCell = (rect, index, column, isActive, isFocused) =>
                {
                    var action = actionsList.list[index] as ActionDataControl;
                    switch (column)
                    {
                        case 0:
                            if (action.hasIdTarget())
                            {
                                var leftHalf = new Rect(rect);
                                leftHalf.width /= 2f;
                                var rightHalf = new Rect(leftHalf);
                                rightHalf.x += leftHalf.width;
                                rightHalf.height = 25;
                                EditorGUI.LabelField(leftHalf, action.getTypeName());
                                if (!isActive)
                                {
                                    EditorGUI.LabelField(rightHalf, !string.IsNullOrEmpty(action.getIdTarget()) ? action.getIdTarget() : "---");
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    string selected = string.Empty;
                                    List<string> choices = new List<string>();
                                    switch ((action.getContent() as Action).getType())
                                    {
                                        case Action.DRAG_TO:
                                        case Action.CUSTOM_INTERACT:
                                            choices.AddRange(activeAreas);
                                            choices.AddRange(characters);
                                            choices.AddRange(items);
                                            break;
                                    }
                                    var selectedIndex = EditorGUI.Popup(rightHalf, choices.FindIndex(t => t.Equals(action.getIdTarget())), choices.ToArray());
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (selectedIndex >= 0 && selectedIndex < choices.Count)
                                        {
                                            selected = choices[selectedIndex];
                                            action.setIdTarget(selected);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                EditorGUI.LabelField(rect, action.getTypeName());
                            }
                            break;
                        case 1:
                            EditorGUI.BeginChangeCheck();
                            var documentation = EditorGUI.TextArea(rect, action.getDocumentation() ?? string.Empty);
                            if (EditorGUI.EndChangeCheck()) action.setDocumentation(documentation);
                            break;
                        case 2:
                            if (Controller.Instance.playerMode() == Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
                            {
                                EditorGUI.LabelField(rect, TC.get("ActionsList.NotRelevant"));
                            }
                            else
                            {
                                var leftHalf = new Rect(rect);
                                leftHalf.width /= 2f;
                                var rightHalf = new Rect(leftHalf);
                                rightHalf.x += leftHalf.width;

                                EditorGUI.BeginChangeCheck();
                                var needsToGo = EditorGUI.Toggle(leftHalf, action.getNeedsGoTo());
                                if (EditorGUI.EndChangeCheck()) action.setNeedsGoTo(needsToGo);

                                EditorGUI.BeginChangeCheck();
                                var distance = EditorGUI.IntField(rightHalf, action.getKeepDistance());
                                if (EditorGUI.EndChangeCheck()) action.setKeepDistance(distance); ;
                            }
                            break;
                        case 3:
                            if (GUI.Button(rect, action.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                            {
                                ConditionEditorWindow window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                                window.Init(action.getConditions());
                            }
                            break;
                        case 4:
                            if (GUI.Button(rect, "Effects"))
                            {
                                EffectEditorWindow window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                                window.Init(action.getEffects());
                            }
                            break;

                    }
                }
            };
        }

        public override void Draw(int aID)
        {

            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            var workingCharacter = Target != null ? Target as NPCDataControl : Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex];

            activeAreas = Controller.Instance.IdentifierSummary.getIds<ActiveArea>();
            characters = Controller.Instance.IdentifierSummary.getIds<NPC>();
            items = Controller.Instance.IdentifierSummary.getIds<Item>();

            actionsList.SetData(workingCharacter.getActionsList(), (data) => (data as ActionsListDataControl).getActions().Cast<DataControl>().ToList());
            actionsList.DoList(Target != null ? 160 : windowHeight - 60f);
        }
        
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System.Linq;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class ItemsWindowActions : LayoutWindow
    {
        private Texture2D conditionsTex = null;
        private Texture2D noConditionsTex = null;

        private Vector2 scrollPosition;

        private int selectedAction;
        private DataControlList actionsList;

        private string documentation = "", documentationLast = "";

        private string[] itemsNames;
        private string[] charactersNames;
        private string[] joinedNamesList;
        private int selectedTarget, selectedTargetLast;

        public ItemsWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
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

            selectedTarget = selectedTargetLast = 0;
            selectedAction = -1;

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
                                        case Action.GIVE_TO:
                                            choices.AddRange(characters);
                                            break;
                                        case Action.USE_WITH:
                                            choices.AddRange(activeAreas);
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

        private string[] activeAreas, characters, items;
        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            var workingItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex];

            activeAreas = Controller.Instance.IdentifierSummary.getIds<ActiveArea>();
            characters  = Controller.Instance.IdentifierSummary.getIds<NPC>();
            items       = Controller.Instance.IdentifierSummary.getIds<Item>();

            actionsList.SetData(workingItem.getActionsList(), (data) => (data as ActionsListDataControl).getActions().Cast<DataControl>().ToList());
            actionsList.DoList(windowHeight - 60f);
        }

        private void OnActionSelectionChange(int i)
        {
            selectedAction = i;
            // Refresh docs
            string doc = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]
                .getDocumentation();
            if (!string.IsNullOrEmpty(doc))
            {
                documentation =
                    documentationLast = doc;
            }
            else
            {
                documentation = documentationLast = "";
            }

            // Refresh target info
            string targetID = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].getIdTarget();

            selectedTarget =
                selectedTargetLast =
                    Controller.Instance.SelectedChapterDataControl.getItemsList()
                        .getItemIndexByID(targetID);
            // if target is not an item, but a npc...
            if (selectedTarget == -1)
            {
                selectedTarget =
                    selectedTargetLast =
                        Controller.Instance.SelectedChapterDataControl.getNPCsList()
                            .getNPCIndexByID(targetID);

                if (selectedTarget == -1)
                {
                    selectedTarget = selectedTargetLast = 0;
                }
                else
                {
                    selectedTarget = selectedTargetLast += itemsNames.Length;
                }
            }

        }

        private void OnDocumentationChanged(string s)
        {
            if (selectedAction >= 0 && Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction] != null)
            {
                documentationLast = s;
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]
                    .setDocumentation(s);
            }
        }

        private void ChangeActionTarget(int i)
        {
            selectedTargetLast = i;
            // Scene was choosed
            if (i < itemsNames.Length)

                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].setIdTarget(
                        itemsNames[i]);
            else
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].setIdTarget(
                        charactersNames[i - itemsNames.Length]);
        }

    }
}
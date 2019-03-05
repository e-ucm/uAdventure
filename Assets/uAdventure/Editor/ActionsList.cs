using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class ActionsList : ScriptableObject {

    private Texture2D conditionsTex = null;
    private Texture2D noConditionsTex = null;

    private DataControlList actionsList;
    private AppearanceEditor appearances;
    private CustomActionDataControl customAction;
    
    private FileChooser file;
    private AnimationField animation;
    private int groupSelected = 0;

    public void Awake()
    {
        appearances = ScriptableObject.CreateInstance<AppearanceEditor>();

        conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
        noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

        actionsList = new DataControlList()
        {
            footerHeight = 25,
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
                        {
                            var name = action.getContent() is CustomAction ? ((CustomAction)action.getContent()).getName() : action.getTypeName();

                            var leftHalf = new Rect(rect);
                            leftHalf.width /= 2f;
                            var rightHalf = new Rect(leftHalf);
                            rightHalf.x += leftHalf.width;

                            if (action.hasIdTarget())
                            {
                                rightHalf.height = 25;
                                EditorGUI.LabelField(leftHalf, name);
                                if (!isActive)
                                {
                                    EditorGUI.LabelField(rightHalf, !string.IsNullOrEmpty(action.getIdTarget()) ? action.getIdTarget() : "---");
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    string selected = string.Empty;
                                    string[] choices = new string[0];
                                    switch ((action.getContent() as Action).getType())
                                    {
                                        case Action.DRAG_TO:
                                        case Action.CUSTOM_INTERACT:
                                            choices = Controller.Instance.IdentifierSummary.combineIds(new System.Type[] { typeof(Item), typeof(NPC), typeof(ActiveArea) });
                                            break;
                                        case Action.GIVE_TO:
                                            choices = Controller.Instance.IdentifierSummary.getIds<NPC>();
                                            break;
                                        case Action.USE_WITH:
                                            choices = Controller.Instance.IdentifierSummary.combineIds(new System.Type[] { typeof(Item), typeof(ActiveArea) });
                                            break;
                                    }

                                    var selectedIndex = EditorGUI.Popup(rightHalf, System.Array.FindIndex(choices, action.getIdTarget().Equals), choices);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (selectedIndex >= 0 && selectedIndex < choices.Length)
                                        {
                                            selected = choices[selectedIndex];
                                            action.setIdTarget(selected);
                                        }
                                    }
                                }
                            }
                            else if (action.getType() == Controller.ACTION_TALK_TO)
                            {
                                EditorGUI.LabelField(leftHalf, name);
                                var triggerConversationEffect = action.getEffects().getEffects().Find(e => e is TriggerConversationEffect) as TriggerConversationEffect;
                                if (triggerConversationEffect != null)
                                {
                                    var conversationId = triggerConversationEffect.getTargetId();
                                    if (GUI.Button(rightHalf, "Open"))
                                    {
                                        var conversationsList = Controller.Instance.SelectedChapterDataControl.getConversationsList();
                                        var conversation = conversationsList.getConversations().Find(c => c.getId() == conversationId);
                                        Controller.Instance.SelectElement(conversation);
                                    }
                                }
                            }
                            else
                            {
                                EditorGUI.LabelField(rect, name);
                            }
                        }
                        break;
                    case 1:
                        EditorGUI.BeginChangeCheck();
                        var documentation = EditorGUI.TextArea(rect, action.getDocumentation() ?? string.Empty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            action.setDocumentation(documentation);
                        }

                        break;
                    case 2:
                        if (Controller.Instance.PlayerMode== Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
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
                            if (EditorGUI.EndChangeCheck())
                            {
                                action.setNeedsGoTo(needsToGo);
                            }

                            EditorGUI.BeginChangeCheck();
                            var distance = EditorGUI.IntField(rightHalf, action.getKeepDistance());
                            if (EditorGUI.EndChangeCheck())
                            {
                                action.setKeepDistance(distance);
                            }

                            ;
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

        actionsList.onSelectCallback += (list) =>
        {
            var i = list.index;
            var actions = ActionsListDataControl.getActions();
            customAction = i != -1 && actions[i] is CustomActionDataControl ? actions[i] as CustomActionDataControl : null;
            if(customAction != null)
            {
                appearances.Data = customAction;
            }
        };

        appearances = CreateInstance<AppearanceEditor>();

        file = new FileChooser()
        {
            FileType = FileType.BUTTON,
        };

        animation = new AnimationField()
        {
            FileType = FileType.CHARACTER_ANIM,
        };
    }


    public ActionsListDataControl ActionsListDataControl {
        get
        {
            return actionsList.DataControl as ActionsListDataControl;
        }
        set
        {
            if(actionsList.DataControl != value)
            {
                actionsList.SetData(value, (data) => (data as ActionsListDataControl).getActions().Cast<DataControl>().ToList());
            }
        }
    }
    

    public void DoList(float height)
    {
        if(ActionsListDataControl == null)
        {
            EditorGUILayout.HelpBox("Actions List not setted!", MessageType.Warning);
            return;
        }
         
        actionsList.DoList(customAction != null ? height - 160 : height);

        if (customAction != null)
        {
            appearances.height = 80;
            appearances.OnInspectorGUI();
            var resources = customAction.getResources()[customAction.getSelectedResources()];
            var titles = new string[resources.getAssetGroupCount()];
            for (int i = 0, end = titles.Length; i < end; ++i)
            {
                titles[i] = resources.getGroupInfo(i);
            }

            groupSelected = EditorGUILayout.Popup(TC.get("Resources.ResourcesGroup"), groupSelected, titles);
            int assetIndex;
            for(int i = 0, end = resources.getGroupAssetCount(groupSelected); i < end; ++i)
            {
                assetIndex = resources.getAssetIndex(groupSelected, i);
                var field = resources.getAssetCategory(assetIndex) == AssetsConstants.CATEGORY_ANIMATION ? animation : file;
                field.Label = resources.getAssetDescription(i);
                field.Path = resources.getAssetPath(assetIndex);
                EditorGUI.BeginChangeCheck();
                field.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    resources.addAsset(resources.getAssetName(i), field.Path);
                }
            }
        }
    }
}

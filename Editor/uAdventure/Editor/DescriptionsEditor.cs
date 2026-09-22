using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class DescriptionsEditor : Editor {

    private Texture2D conditionsTex = null;
    private Texture2D noConditionsTex = null;

    private DescriptionsController descriptions;
    private DescriptionController description;

    private TextWithSoundField nameField, briefField, fullField;

    public DescriptionsController Descriptions {
        get
        {
            return descriptions;
        }
        set
        {
            descriptions = value;
            descriptionsList.SetData(descriptions, (data) => (data as DescriptionsController).getDescriptions().Cast<DataControl>().ToList());
            descriptionsList.index = descriptions.getSelectedDescriptionNumber();
            description = descriptions.getSelectedDescriptionController();
        }
    }

    private DataControlList descriptionsList;

    private void Awake()
    {
        conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
        noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

        descriptionsList = new DataControlList()
        {
            RequestRepaint = Repaint,
            elementHeight = 20,
            Columns = new List<ColumnList.Column>()
            {
                new ColumnList.Column()
                {
                    Text = TC.get("Item.Name")
                },
                new ColumnList.Column()
                {
                    Text = TC.get("Conditions.Title")
                }
            },
            drawCell = (rect, index, column, isActive, isFocused) =>
            {
                var description = descriptions.getDescriptionController(index);
                switch (column)
                {
                    case 0:
                        if (index == descriptionsList.index)
                        {
                            description.renameElement(EditorGUI.TextField(rect, description.getName()));
                        }
                        else
                        {
                            EditorGUI.LabelField(rect, description.getName());
                        }
                        break;
                    case 1:
                        if (GUI.Button(rect, description.getConditionsController().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                        {
                            ConditionEditorWindow window =
                                 (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                            window.Init(description.getConditionsController());
                        }
                        break;
                }
            },
            onSelectCallback = (list) =>
            {
                descriptions.setSelectedDescription(list.index);
                description = descriptions.getSelectedDescriptionController();
            }
        };

        descriptionsList.index = 0;

        nameField = new TextWithSoundField()
        {
            Label = TC.get("Item.Name"),
            FileType = FileType.ITEM_DESCRIPTION_NAME_SOUND
        };

        briefField = new TextWithSoundField()
        {
            Label = TC.get("Item.Description"),
            FileType = FileType.ITEM_DESCRIPTION_BRIEF_SOUND
        };

        fullField = new TextWithSoundField()
        {
            Label = TC.get("Item.DetailedDescription"),
            FileType = FileType.ITEM_DESCRIPTION_DETAILED_SOUND
        };

    }

    private void OnEnable()
    {
        

    }

    public override void OnInspectorGUI()
    {
        descriptionsList.DoList(120);

        string prevContent = string.Empty, prevPath = string.Empty;

        var descriptionData = description.getDescriptionData();

        GUILayout.Space(20);

        // Name field
        prevContent = nameField.Content = descriptionData.getName();
        prevPath = nameField.Path = descriptionData.getNameSoundPath();
        nameField.DoLayout();
        if (prevContent != nameField.Content)
            descriptionData.setName(nameField.Content);
        if (prevPath != nameField.Path)
            descriptionData.setNameSoundPath(nameField.Path);

        // Brief field
        prevContent = briefField.Content = descriptionData.getDescription();
        prevPath = briefField.Path = descriptionData.getDescriptionSoundPath();
        briefField.DoLayout();
        if (prevContent != briefField.Content)
            descriptionData.setDescription(briefField.Content);
        if (prevPath != briefField.Path)
            descriptionData.setDescriptionSoundPath(briefField.Path);

        // Full field
        prevContent = fullField.Content = descriptionData.getDetailedDescription();
        prevPath = fullField.Path = descriptionData.getDetailedDescriptionSoundPath();
        fullField.DoLayout();
        if (prevContent != fullField.Content)
            descriptionData.setDetailedDescription(fullField.Content);
        if (prevPath != fullField.Path)
            descriptionData.setDetailedDescriptionSoundPath(fullField.Path);




    }

}

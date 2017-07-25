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
        }
    }

    private DataControlList descriptionsList;

    private void Awake()
    {
        conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
        noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

        descriptionsList = new DataControlList()
        {
            elementHeight = 25,
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var leftRect = new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
                var rightRect = new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height);

                var description = descriptions.getDescriptionController(index);
                if (index == descriptionsList.index)
                {
                    description.renameElement(EditorGUI.TextField(leftRect, description.getName()));
                }
                else
                {
                    EditorGUI.LabelField(leftRect, description.getName());
                }

                if (GUI.Button(rightRect, description.getConditionsController().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                {
                    ConditionEditorWindow window =
                         (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(description.getConditionsController());
                }
            },
            onSelectCallback = (list) =>
            {
                description = descriptions.getDescriptions()[list.index];
            }
        };

        descriptionsList.index = 0;

        nameField = new TextWithSoundField()
        {
            Label = TC.get("Item.Name"),
            FileType = BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_NAME_SOUND
        };

        briefField = new TextWithSoundField()
        {
            Label = TC.get("Item.Description"),
            FileType = BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_BRIEF_SOUND
        };

        fullField = new TextWithSoundField()
        {
            Label = TC.get("Item.DetailedDescription"),
            FileType = BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_DETAILED_SOUND
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

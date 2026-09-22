using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class ResourcesList : Editor {

    public delegate void ResourceSelected(DataControlWithResources dataControl);
    public ResourceSelected onResourceSelected;

    private Texture2D conditionsTex = null;
    private Texture2D noConditionsTex = null;
    private DataControlWithResources dataControl;
    private DataControlList resourcesList;

    public DataControlWithResources Data
    {
        get
        {
            return dataControl;
        }
        set
        {
            if (dataControl != value)
            {
                dataControl = value;
                resourcesList.SetData(dataControl, (data) => (data as DataControlWithResources).getResources().Cast<DataControl>().ToList());
                resourcesList.index = dataControl.getSelectedResources();
                if (onResourceSelected != null)
                {
                    onResourceSelected(dataControl);
                }
            }
        }
    }

    public int Height { get; internal set; }

    protected void Awake()
    {
        conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
        noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

        resourcesList = new DataControlList()
        {
            RequestRepaint = Repaint,
            headerHeight = 20,
            footerHeight = 20,
            Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column(){
                        Text = TC.get("Item.LookPanelTitle"),
                        SizeOptions = new GUILayoutOption[]
                        {
                            GUILayout.ExpandWidth(true)
                        }
                    },
                    new ColumnList.Column(){
                        Text = TC.get("Conditions.Title"),
                        SizeOptions = new GUILayoutOption[]
                        {
                            GUILayout.ExpandWidth(true)
                        }
                    }
                },
            drawCell = (rect, index, col, isActive, isFocused) =>
            {
                var resources = dataControl.getResources()[index];
                switch (col) 
                {
                    case 0:
                        if (index == resourcesList.index)
                        {
                            EditorGUI.BeginChangeCheck();
                            var newname = EditorGUI.TextField(rect, "Resources " + (index + 1), resources.getName());
                            if (EditorGUI.EndChangeCheck())
                            {
                                resources.renameElement(newname);
                            }
                        }
                        else
                        {
                            EditorGUI.LabelField(rect, "Resources " + (index + 1) + ": " + resources.getName());
                        }
                        break;
                    case 1:
                        if (GUI.Button(rect, resources.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                        {
                            resourcesList.index = index;
                            ConditionEditorWindow window = CreateInstance<ConditionEditorWindow>();
                            window.Init(resources.getConditions());
                        }
                        break;
                }
            },
            onSelectCallback = (list) =>
            {
                if (list.index == -1) list.index = 0;
                dataControl.setSelectedResources(list.index);
                if (onResourceSelected != null)
                {
                    onResourceSelected(dataControl);
                }
            }
        };
    }

    public override void OnInspectorGUI()
    {
        resourcesList.DoList(Height);
    }
}
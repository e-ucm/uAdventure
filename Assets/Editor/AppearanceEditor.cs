using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class AppearanceEditor : Editor {

    public delegate void AppearanceSelected(DataControlWithResources dataControl);
    public AppearanceSelected onAppearanceSelected;

    private Texture2D conditionsTex = null;
    private Texture2D noConditionsTex = null;

    private DataControlWithResources dataControl;

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
                appearanceList.SetData(dataControl, (data) => (data as DataControlWithResources).getResources().Cast<DataControl>().ToList());
                appearanceList.index = dataControl.getSelectedResources();
                if (onAppearanceSelected != null)
                    onAppearanceSelected(dataControl);
            }
        }
    }

    public int height { get; internal set; }

    private DataControlList appearanceList;

    private void Awake()
    {
        conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
        noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

        appearanceList = new DataControlList()
        {
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
                        if (index == appearanceList.index)
                        {
                            resources.renameElement(EditorGUI.TextField(rect, resources.getName()));
                        }
                        else
                        {
                            EditorGUI.LabelField(rect, resources.getName());
                        }
                        break;
                    case 1:
                        if (GUI.Button(rect, resources.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                        {
                            ConditionEditorWindow window =
                                 (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                            window.Init(resources.getConditions());
                        }
                        break;
                }
            },
            onSelectCallback = (list) =>
            {
                if (list.index == -1) list.index = 0;
                dataControl.setSelectedResources(list.index);
                if (onAppearanceSelected != null)
                    onAppearanceSelected(dataControl);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        appearanceList.DoList(height);
    }
}
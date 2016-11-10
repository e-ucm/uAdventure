using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class MilestoneEditorWindow : EditorWindow
{
    private static MilestoneEditorWindow editor;
    public enum MilestoneType { SCENE, ITEM, CHARACTER, COMPLETABLE, CONDITION };
    private string[] milestonetypes =
    {
        "Scene has been visited"
        ,"Player interacts with Item"
        ,"Player interacts with character"
        ,"A completable has been completed"
        ,"A condition is true"
    };
    private string[] scenes;
    private string[] characters;
    private string[] items;
    private string[] completables;

    private int selectedId = 0;
    Completable.Milestone.MilestoneType antiguo;

    GUIStyle conditionStyle, eitherConditionStyle, closeStyle, collapseStyle;

    public void Init(Completable.Milestone mil)
    {
        editor = EditorWindow.GetWindow<MilestoneEditorWindow>();

        editor.Milestone = mil;

        scenes = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenesIDs();
        characters = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCsIDs();
        items = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItemsIDs();

        List<string> tmplist = new List<string>();
        foreach (Completable c in Controller.getInstance().getSelectedChapterDataControl().getCompletables())
            tmplist.Add(c.getId());
        completables = tmplist.ToArray();

        selectedId = 0;
        antiguo = milestone.getType();

        if (milestone.getId() != "")
        {
            string[] tmp = { };
            switch (milestone.getType())
            {
                case Completable.Milestone.MilestoneType.CHARACTER: tmp = characters; break;
                case Completable.Milestone.MilestoneType.ITEM: tmp = items; break;
                case Completable.Milestone.MilestoneType.SCENE: tmp = scenes; break;
                case Completable.Milestone.MilestoneType.COMPLETABLE: break;
            }

            for(int i = 0; i < tmp.Length; i++)
                if(tmp[i] == milestone.getId())
                {
                    selectedId = i;
                    break;
                }
        }
    }

    private Completable.Milestone milestone;

    public Completable.Milestone Milestone
    {
        get { return milestone; }
        set { this.milestone = value; }
    }
    

    void OnGUI()
    {
        
        if (conditionStyle == null)
        {
            conditionStyle = new GUIStyle(GUI.skin.box);
            conditionStyle.normal.background = MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
        }

        if (eitherConditionStyle == null)
        {
            eitherConditionStyle = new GUIStyle(GUI.skin.box);
            eitherConditionStyle.normal.background = MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
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

        GUILayout.BeginVertical();

        antiguo = milestone.getType();
        GUILayout.Label("The milestone will be reached when");
        milestone.setType( (Completable.Milestone.MilestoneType) EditorGUILayout.Popup((int)milestone.getType(), milestonetypes));
        if(antiguo != milestone.getType())
        {
            selectedId = 0;
        }

        switch (milestone.getType())
        {
            case Completable.Milestone.MilestoneType.CHARACTER:
                EditorGUILayout.LabelField("Character:");
                selectedId = EditorGUILayout.Popup(selectedId, characters);
                milestone.setId(characters[selectedId]);
                break;
            case Completable.Milestone.MilestoneType.ITEM:
                EditorGUILayout.LabelField("Item:");
                selectedId = EditorGUILayout.Popup(selectedId, items);
                milestone.setId(items[selectedId]);
                break;
            case Completable.Milestone.MilestoneType.SCENE:
                EditorGUILayout.LabelField("Scene:");
                selectedId = EditorGUILayout.Popup(selectedId, scenes);
                milestone.setId(scenes[selectedId]);
                break;
            case Completable.Milestone.MilestoneType.COMPLETABLE:
                EditorGUILayout.LabelField("Completable:");
                selectedId = EditorGUILayout.Popup(selectedId, completables);
                milestone.setId(completables[selectedId]);
                break;
            case Completable.Milestone.MilestoneType.CONDITION:
                if (milestone.getConditions() == null)
                    milestone.setConditions(new Conditions());

                GUILayout.BeginVertical(conditionStyle);
                GUILayout.Label("CONDITIONS");
                if (GUILayout.Button("Add Block"))
                {
                    milestone.getConditions().add(new FlagCondition(""));
                }

                foreach (List<Condition> cl in milestone.getConditions().getConditionsList())
                {
                    if (cl.Count > 1)
                        GUILayout.BeginVertical(eitherConditionStyle);
                    for (int i = 0; i < cl.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        int preConEdiSel = ConditionEditorFactory.Intance.ConditionEditorIndex(cl[i]);
                        int conEdiSel = EditorGUILayout.Popup(preConEdiSel,
                            ConditionEditorFactory.Intance.CurrentConditionEditors);

                        if (preConEdiSel != conEdiSel)
                            cl[i] = ConditionEditorFactory.Intance.Editors[conEdiSel].InstanceManagedCondition();

                        ConditionEditorFactory.Intance.getConditionEditorFor(cl[i]).draw(cl[i]);

                        if (GUILayout.Button("+", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
                        {
                            cl.Add(new FlagCondition(""));
                        }

                        if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                        {
                            cl.Remove(cl[i]);

                            if (cl.Count == 0)
                                milestone.getConditions().getConditionsList().Remove(cl);
                        }

                        GUILayout.EndHorizontal();
                    }
                    if (cl.Count > 1)
                        GUILayout.EndVertical();
                }

                GUILayout.EndVertical();

                break;
        }

        if(GUILayout.Button("Save milestone"))
        {
            this.Close();
        }

        GUILayout.EndVertical();
    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}

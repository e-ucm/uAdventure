using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ProgressEditorWindow : EditorWindow
{
    private static ProgressEditorWindow editor;
    string[] typeoptions = { "Milestones reached/total", "Max progress of all milestones" };

    int type = 0;

    private Texture2D addTexture = null;
    private Texture2D moveUp, moveDown = null;
    private Texture2D clearImg = null;

    private static Vector2 scrollPosition;

    private Completable.Milestone selectedMilestone;
    private Completable.Progress progress;

    public void Init(Completable.Progress progress)
    {
        editor = EditorWindow.GetWindow<ProgressEditorWindow>();

        clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
        moveUp = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof(Texture2D));
        moveDown = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof(Texture2D));

        this.progress = progress;
    }

    public void OnGUI()
    {
        GUILayout.Label("Progress is given by: ");

        progress.setType((Completable.Progress.ProgressType) EditorGUILayout.Popup((int) progress.getType(), typeoptions));

        GUILayout.Label("Milestones: ");

        //GUILayout.BeginArea(tableRect);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(position.width * 0.9f));
        GUILayout.BeginHorizontal();
        if (progress.getType() == Completable.Progress.ProgressType.SUM)
        {
            GUILayout.Box("Time", GUILayout.Width(position.width * 0.88f));
        }
        else
        {
            GUILayout.Box("Time", GUILayout.Width(position.width * 0.44f));
            GUILayout.Box("Progress", GUILayout.Width(position.width * 0.44f));
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (Completable.Milestone milestone in progress.getMilestones())
        {
            GUILayout.BeginHorizontal();
            if (progress.getType() == Completable.Progress.ProgressType.SUM)
            {
                if (GUILayout.Button(milestone.ToString(), GUILayout.Width(position.width * 0.88f)))
                {
                    MilestoneEditorWindow window = ScriptableObject.CreateInstance<MilestoneEditorWindow>();
                    window.Init(milestone);
                }
            }
            else
            {
                if (GUILayout.Button(milestone.ToString(), GUILayout.Width(position.width * 0.44f)))
                {
                    MilestoneEditorWindow window = ScriptableObject.CreateInstance<MilestoneEditorWindow>();
                    window.Init(milestone);
                }

                milestone.setProgress(EditorGUILayout.Slider(milestone.getProgress(), 0, 1, GUILayout.Width(position.width * 0.44f)));
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();


        //GUILayout.EndArea();


        //GUILayout.BeginArea(rightPanelRect);
        GUILayout.BeginVertical(GUILayout.Width(0.1f * position.width));
        if (GUILayout.Button(addTexture))
        {
            progress.addMilestone(new Completable.Milestone());
        }
        if (GUILayout.Button(moveUp))
        {
            int pos = progress.getMilestones().IndexOf(selectedMilestone);
            if (pos > 0)
            {
                Completable.Milestone tmp = progress.getMilestones()[pos - 1];
                progress.getMilestones()[pos - 1] = progress.getMilestones()[pos];
                progress.getMilestones()[pos] = tmp;
            }
        }
        if (GUILayout.Button(moveDown))
        {
            int pos = progress.getMilestones().IndexOf(selectedMilestone);
            if (pos < progress.getMilestones().Count - 1)
            {
                Completable.Milestone tmp = progress.getMilestones()[pos + 1];
                progress.getMilestones()[pos + 1] = progress.getMilestones()[pos];
                progress.getMilestones()[pos] = tmp;
            }
        }
        if (GUILayout.Button(clearImg))
        {
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

}



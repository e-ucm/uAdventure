using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System;

namespace uAdventure.Editor
{
    public class ProgressEditorWindow : EditorWindow
    {
        private static ProgressEditorWindow editor;
        string[] typeoptions = { "Milestones reached/total", "Max progress of all milestones" };

        int type = 0;

        private Texture2D addTexture = null;
        private Texture2D moveUp, moveDown = null;
        private Texture2D clearImg = null;

        private static Vector2 scrollPosition;

        private DataControlList progressList;
        private ProgressDataControl progress;
        private ColumnList.Column progressColumn;

        private Completable.Progress.ProgressType currentMode = Completable.Progress.ProgressType.SUM;

        public void Init(ProgressDataControl progress)
        {
            clearImg = Resources.Load<Texture2D>("EAdventureData/img/icons/deleteContent");
            addTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/addNode");
            moveUp = Resources.Load<Texture2D>("EAdventureData/img/icons/moveNodeUp");
            moveDown = Resources.Load<Texture2D>("EAdventureData/img/icons/moveNodeDown");

            this.progress = progress;

            progressList = new DataControlList()
            {
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = "Time"
                    }
                },
                drawCell = (rect, row, column, isActive, isFocused) =>
                {
                    var milestone = progressList.list[row] as MilestoneDataControl;
                    switch (column)
                    {
                        case 0:
                            if (GUI.Button(rect, milestone.getContent().ToString()))
                                MilestoneEditorWindow.ShowMilestoneEditor(rect, milestone);
                            break;
                        case 1:
                            milestone.setProgress(EditorGUI.Slider(rect, milestone.getProgress(), 0, 1));
                            break;
                    }
                }
            };

            progressColumn = new ColumnList.Column()
            {
                Text = "Progress"
            };

        }

        private bool hasToRepaint = false;
        public void OnGUI()
        {
            GUILayout.Label("Progress is given by: ");
            
            progress.setType((Completable.Progress.ProgressType)EditorGUILayout.Popup((int)progress.getType(), typeoptions));

            if(progress.getType() != currentMode)
            {
                currentMode = progress.getType();
                switch (currentMode)
                {
                    case Completable.Progress.ProgressType.SUM:
                        if (progressList.Columns.Contains(progressColumn))
                            progressList.Columns.Remove(progressColumn);
                        break;
                    case Completable.Progress.ProgressType.SPECIFIC:
                        if (!progressList.Columns.Contains(progressColumn))
                            progressList.Columns.Add(progressColumn);
                        break;
                }
                hasToRepaint = true;
            }

            if (hasToRepaint && Event.current.type == EventType.Layout)
            {
                this.Repaint();
                hasToRepaint = false;
            }

            progressList.SetData(progress, (p) => (p as ProgressDataControl).getMilestones().ConvertAll(m => m as DataControl));
            progressList.DoList(position.height - 55);
        }


        public static ProgressEditorWindow Create(ProgressDataControl progress)
        {
            editor = ScriptableObject.CreateInstance<ProgressEditorWindow>();
            editor.Init(progress);
            return editor;
        }


        public static void ShowProgressEditor(Rect rect, ProgressDataControl progress)
        {
            var window = ProgressEditorWindow.Create(progress);
            rect.position = GUIUtility.GUIToScreenPoint(rect.position);
            window.ShowAsDropDown(rect, new Vector2(Mathf.Max(rect.width, 250), 300));
        }
    }
}
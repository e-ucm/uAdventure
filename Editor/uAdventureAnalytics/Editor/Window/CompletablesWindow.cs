using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using uAdventure.Core;
using uAdventure.Editor;
using System;
using System.Linq;

namespace uAdventure.Analytics
{
    public class CompletablesWindow : LayoutWindow
    {
        private readonly string[] endOptions = { TC.get("Analytics.EndOptions.FinalScene")/*, TC.get("Analytics.EndOptions.AllLevels")*/ };
        private readonly string[] progressOptions = { TC.get("Analytics.ProgressOptions.NumberOfLevels")/*, TC.get("Analytics.ProgressOptions.Manual")*/ };
        private readonly DataControlList completablesList;

        private int end = 0;
        private int progress = 0;
        private CompletableListDataControl completables;

		private bool Available { get; set; }

        public CompletablesWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            completablesList = new DataControlList()
            {
                RequestRepaint = Repaint,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = TC.get("Analytics.Completable.Id")
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Analytics.Completable.Start")
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Analytics.Completable.End")
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Analytics.Completable.Progress")
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Analytics.Completable.Score")
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Repeatable"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(70) }
                    }
                },
                drawCell = (rect, row, column, isActive, isFocused) =>
                {
                    var completable = completablesList.list[row] as CompletableDataControl;
                    switch (column)
                    {
                        case 0:
                            completable.renameElement(EditorGUI.TextField(rect, completable.getId()));
                            break;
                        case 1:
                            if (GUI.Button(rect, completable.getStart().getContent().ToString()))
                                MilestoneEditorWindow.ShowMilestoneEditor(rect, completable.getStart());
                            break;
                        case 2:
                            if (GUI.Button(rect, completable.getEnd().getContent().ToString()))
                                MilestoneEditorWindow.ShowMilestoneEditor(rect, completable.getEnd());
                            break;
                        case 3:
                            if (GUI.Button(rect, TC.get("Analytics.Completable.Define")))
                                ProgressEditorWindow.ShowProgressEditor(rect, completable.getProgress());
                            break;
                        case 4:
                            {
                                if (Available)
                                {
                                    ScoreEditor(rect, completable.getScore());
                                }
                                else
                                {
                                    EditorGUI.HelpBox(rect, TC.get("Condition.Var.Warning"), MessageType.Error);
                                }
                            }
                            break;
                        case 5:
                            completable.setRepeatable(GUI.Toggle(rect, completable.getRepeatable(), "?"));
                            break;
                    }
                }
            };
        }

        public static void ScoreEditor(Rect rect, ScoreDataControl score)
        {
            var rects = rect.Divide(3);
            var previousMethod = score.getMethod();
            var newMethod = (Completable.Score.ScoreMethod)EditorGUI.EnumPopup(rects[0], previousMethod);
            score.setMethod(newMethod);

            if (newMethod != previousMethod)
            {
                if (newMethod == Completable.Score.ScoreMethod.SINGLE)
                {
                    Controller.Instance.VarFlagSummary.addVarReference(score.getId());
                }
                else
                {
                    Controller.Instance.VarFlagSummary.deleteVarReference(score.getId());
                }
            }

            switch (score.getMethod())
            {
                case Completable.Score.ScoreMethod.AVERAGE:
                case Completable.Score.ScoreMethod.SUM:
                    rects[1].width += rects[2].width;
                    if (GUI.Button(rects[1], "SubScores"))
                    {
                        CompletableScoreEditorWindow.Create(score);
                    }
                    break;
                case Completable.Score.ScoreMethod.SINGLE:
                    EditorGUI.BeginChangeCheck();
                    var newType = (Completable.Score.ScoreType) EditorGUI.EnumPopup(rects[1], score.getType());
                    if (EditorGUI.EndChangeCheck())
                    {
                        score.setType(newType);
                    }

                    string[] switchOn = null;
                    switch (score.getType())
                    {
                        case Completable.Score.ScoreType.VARIABLE: switchOn = Controller.Instance.VarFlagSummary.getVars(); break;
                        case Completable.Score.ScoreType.COMPLETABLE: switchOn = Controller.Instance.IdentifierSummary.getIds<Completable>(); break;
                    }
                    EditorGUI.BeginChangeCheck();
                    var newSwitchOn = switchOn[EditorGUI.Popup(rects[2], Mathf.Max(0, Array.IndexOf(switchOn, score.getId())), switchOn)];
                    if (EditorGUI.EndChangeCheck() || score.getId() == null)
                    {
                        score.setId(newSwitchOn);
                    }

                    break;
            }
        }

        public override void Draw(int aID)
		{
			var variables = Controller.Instance.VarFlagSummary.getVars();
			Available = variables != null && variables.Length > 0;
            
            var windowHeight = Rect.height;

			GUILayout.Label(TC.get("Analytics.GameStart") + Controller.Instance.SelectedChapterDataControl.getInitialScene());
			GUILayout.Label(TC.get("Analytics.GameEnd"));

            end = EditorGUILayout.Popup(end, endOptions);

			GUILayout.Label(TC.get("Analytics.GameProgress"));

            progress = EditorGUILayout.Popup(progress, progressOptions);

            if (progress == 1)
            {
				GUILayout.Button(TC.get("Analytics.EditProgress"));
            }

            completables = AnalyticsController.Instance.Completables;
            completablesList.SetData(completables, (c) => (c as CompletableListDataControl).getCompletables().Cast<DataControl>().ToList());
            completablesList.DoList(windowHeight - 150);
        }
    }

    public class CompletableScoreEditorWindow : EditorWindow
    {
        private ScoreDataControl score;
        private DataControlList scoresList;

        public static void Create(ScoreDataControl score)
        {
            var window = CreateInstance<CompletableScoreEditorWindow>();
            window.score = score;
            window.ShowUtility();
        }

        protected void Awake()
        {
            scoresList = new DataControlList()
            {
                RequestRepaint = Repaint,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = "Sub scores"
                    }
                },
                drawCell = (rect, row, column, isActive, isFocused) =>
                {
                    CompletablesWindow.ScoreEditor(rect, scoresList.list[row] as ScoreDataControl);
                }
            };
        }

        protected void OnGUI()
        {
            if(score.getMethod() == Completable.Score.ScoreMethod.SINGLE)
            {
                EditorGUILayout.HelpBox("The score subscores you're trying to edit is in SINGLE mode. Select the SUM or AVERAGE modes to edit its subscores.", MessageType.Error);
                return;
            }

            scoresList.SetData(score, s => (s as ScoreDataControl).getScores().Cast<DataControl>().ToList());
            scoresList.DoList(position.height-15);
        }
    }
}
 
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(200, typeof(CompletableDataControl))]
    public class CompletablesWindow : DefaultButtonMenuEditorWindowExtension
    {
        private readonly string[] endOptions = { TC.get("Analytics.EndOptions.FinalScene"), TC.get("Analytics.EndOptions.AllLevels") };
        private readonly string[] progressOptions = { TC.get("Analytics.ProgressOptions.NumberOfLevels"), TC.get("Analytics.ProgressOptions.Manual") };
        private readonly DataControlList completablesList;

        private int end = 0;
        private int progress = 0;
        private CompletableListDataControl completables;

		private bool Available { get; set; }

        public CompletablesWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Analytics.Title")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/assessmentProfiles"),
                text = "Analytics.Title"
            };

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
                            {
                                MilestoneEditorWindow.ShowMilestoneEditor(rect, completable.getStart());
                            }

                            break;
                        case 2:
                            if (GUI.Button(rect, completable.getEnd().getContent().ToString()))
                            {
                                MilestoneEditorWindow.ShowMilestoneEditor(rect, completable.getEnd());
                            }

                            break;
                        case 3:
                            if (GUI.Button(rect, TC.get("Analytics.Completable.Define")))
                            {
                                ProgressEditorWindow.ShowProgressEditor(rect, completable.getProgress());
                            }

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
                    }
                }
            };
        }

        internal static Rect[] DivideRect(Rect r, int slices)
        {
            Rect[] rects = new Rect[slices];
            var sliceWidth = r.width / slices;
            rects[0] = new Rect(r.x, r.y, sliceWidth, r.height);
            for(int i = 1; i<slices; ++i)
            {
                rects[i] = rects[i - 1];
                rects[i].x += sliceWidth;
            }
            return rects;
        }

        public static void ScoreEditor(Rect rect, ScoreDataControl score)
        {
            var rects = DivideRect(rect, 3);

            score.setMethod((Completable.Score.ScoreMethod)EditorGUI.EnumPopup(rects[0], score.getMethod()));
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
                    score.setType((Completable.Score.ScoreType)EditorGUI.EnumPopup(rects[1], score.getType()));
                    string[] switchOn = null;
                    switch (score.getType())
                    {
                        case Completable.Score.ScoreType.VARIABLE: switchOn = Controller.Instance.VarFlagSummary.getVars(); break;
                        case Completable.Score.ScoreType.COMPLETABLE: switchOn = Controller.Instance.IdentifierSummary.getIds<Completable>(); break;
                    }
                    score.renameElement(switchOn[EditorGUI.Popup(rects[2], Mathf.Max(0, Array.IndexOf(switchOn, score.getId())), switchOn)]);
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

            completables = Controller.Instance.SelectedChapterDataControl.getCompletables();
            completablesList.SetData(completables, (c) => (c as CompletableListDataControl).getCompletables().Cast<DataControl>().ToList());
            completablesList.DoList(windowHeight - 130);
        }

        protected override void OnButton() {}
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
 
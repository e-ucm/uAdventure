using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [EditorWindowExtension(200, typeof(Completable))]
    public class AssesmentProfileWindow : DefaultButtonMenuEditorWindowExtension
    {
		string[] endoptions = { TC.get("Analytics.EndOptions.FinalScene"), TC.get("Analytics.EndOptions.AllLevels") };
		string[] progressoptions = { TC.get("Analytics.ProgressOptions.NumberOfLevels"), TC.get("Analytics.ProgressOptions.Manual") };
        int end = 0;
        int progress = 0;
        int num_colums = 5;
        float col_width;

        private Texture2D addTexture = null;
        private Texture2D moveUp, moveDown = null;
        private Texture2D clearImg = null;
        
        private static Rect tableRect;
        private Rect rightPanelRect;
        private static Vector2 scrollPosition;

        private Completable selectedCompletable;
        private Dictionary<Completable, int> selected_variable = new Dictionary<Completable, int>();

        private string[] variables;

        private List<Completable> completables = new List<Completable>();
		private bool Available { get; set; }

        public AssesmentProfileWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Analytics.Title")), aStyle, aOptions)
        {
            var buttonContent = new GUIContent();
            buttonContent.image = (Texture2D)Resources.Load("EAdventureData/img/icons/assessmentProfiles", typeof(Texture2D)); ;
			buttonContent.text = TC.get("Analytics.Title");
            ButtonContent = buttonContent;

            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            moveUp = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof(Texture2D));
            moveDown = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof(Texture2D));


            col_width = 0.8f / num_colums;

            if(Controller.Instance.SelectedChapterDataControl != null)
            {
                this.completables = Controller.Instance.SelectedChapterDataControl.getCompletables();

                foreach (Completable c in completables)
                {
                    int pos = 0;
                    for (int i = 0; i < variables.Length; i++)
                        if (variables[i] == c.getScore().getId())
                        {
                            pos = i;
                            break;
                        }

                    selected_variable.Add(c, pos);
                }
            }
        }

        public override void Draw(int aID)
		{

			variables = Controller.Instance.VarFlagSummary.getVars();
			Available = variables != null && variables.Length > 0;

            var windowWidth = Rect.width;
            var windowHeight = Rect.height;

            tableRect = new Rect(0f, 200, 0.8f * windowWidth, windowHeight * 0.33f);
            rightPanelRect = new Rect(0.85f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.33f * windowHeight);

			GUILayout.Label(TC.get("Analytics.GameStart") + Controller.Instance.SelectedChapterDataControl.getInitialScene());
			GUILayout.Label(TC.get("Analytics.GameEnd"));

            end = EditorGUILayout.Popup(end, endoptions);

			GUILayout.Label(TC.get("Analytics.GameProgress"));

            progress = EditorGUILayout.Popup(progress, progressoptions);

            if (progress == 1)
            {
				GUILayout.Button(TC.get("Analytics.EditProgress"));
            }

            //GUILayout.BeginArea(tableRect);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
			GUILayout.Box(TC.get("Analytics.Completable.Id"), GUILayout.Width(windowWidth * col_width));
			GUILayout.Box(TC.get("Analytics.Completable.Start"), GUILayout.Width(windowWidth * col_width));
			GUILayout.Box(TC.get("Analytics.Completable.End"), GUILayout.Width(windowWidth * col_width));
			GUILayout.Box(TC.get("Analytics.Completable.Progress"), GUILayout.Width(windowWidth * col_width));
			GUILayout.Box(TC.get("Analytics.Completable.Score"), GUILayout.Width(windowWidth * col_width));
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (Completable completable in completables)
            {
                GUILayout.BeginHorizontal();
                completable.setId(GUILayout.TextField(completable.getId(), GUILayout.Width(windowWidth * col_width)));

                if (GUILayout.Button(completable.getStart().ToString(), GUILayout.Width(windowWidth * col_width)))
                {
                    MilestoneEditorWindow window = ScriptableObject.CreateInstance<MilestoneEditorWindow>();
                    window.Init(completable.getStart());
                }

                if (GUILayout.Button(completable.getEnd().ToString(), GUILayout.Width(windowWidth * col_width)))
                {
                    MilestoneEditorWindow window = ScriptableObject.CreateInstance<MilestoneEditorWindow>();
                    window.Init(completable.getEnd());
                }

				if (GUILayout.Button(TC.get("Analytics.Completable.Define"), GUILayout.Width(windowWidth * col_width)))
                {
                    ProgressEditorWindow window = ScriptableObject.CreateInstance<ProgressEditorWindow>();
                    window.Init(completable.getProgress());
                }

				if (Available)
				{

					selected_variable[completable] = EditorGUILayout.Popup(selected_variable[completable], variables, GUILayout.Width(windowWidth * col_width));
					completable.getScore().setId(variables[selected_variable[completable]]);

				}
				else
				{
					EditorGUILayout.HelpBox(TC.get("Condition.Var.Warning"), MessageType.Error);
				}
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();


            //GUILayout.EndArea();


            //GUILayout.BeginArea(rightPanelRect);
            GUILayout.BeginVertical(GUILayout.Width(0.1f * windowWidth));
            if (GUILayout.Button(addTexture))
            {
                Completable nc = new Completable();
                Completable.Score score = new Completable.Score();
                score.setMethod(Completable.Score.ScoreMethod.SINGLE);
                score.setType(Completable.Score.ScoreType.VARIABLE);
                nc.setScore(score);
                completables.Add(nc);
                selected_variable.Add(nc, 0);
            }
            if (GUILayout.Button(moveUp))
            {
                int pos = completables.IndexOf(selectedCompletable);
                if (pos > 0)
                {
                    Completable tmp = completables[pos - 1];
                    completables[pos - 1] = completables[pos];
                    completables[pos] = tmp;
                }
            }
            if (GUILayout.Button(moveDown))
            {
                int pos = completables.IndexOf(selectedCompletable);
                if (pos < completables.Count - 1)
                {
                    Completable tmp = completables[pos + 1];
                    completables[pos + 1] = completables[pos];
                    completables[pos] = tmp;
                }
            }
            if (GUILayout.Button(clearImg))
            {
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        protected override void OnButton() {}
    }
}
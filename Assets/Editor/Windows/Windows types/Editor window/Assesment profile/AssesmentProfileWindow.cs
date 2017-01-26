using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [EditorWindowExtension(100, typeof(Completable))]
    public class AssesmentProfileWindow : DefaultButtonMenuEditorWindowExtension
    {
        string[] endoptions = { "Final Scene is reached", "All levels completed" };
        string[] progressoptions = { "Number of levels completed", "Manual progress..." };
        int end = 0;
        int progress = 0;
        int num_colums = 5;
        float col_width;

        private Texture2D addTexture = null;
        private Texture2D moveUp, moveDown = null;
        private Texture2D clearImg = null;

        private static float windowWidth, windowHeight;
        private static Rect tableRect;
        private Rect rightPanelRect;
        private static Vector2 scrollPosition;

        private Completable selectedCompletable;
        private Dictionary<Completable, int> selected_variable = new Dictionary<Completable, int>();

        private string[] variables;

        private List<Completable> completables = new List<Completable>();

        public AssesmentProfileWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("AssessmentFeatures.Title")), aStyle, aOptions)
        {
            var buttonContent = new GUIContent();
            buttonContent.image = (Texture2D)Resources.Load("EAdventureData/img/icons/assessmentProfiles", typeof(Texture2D)); ;
            buttonContent.text = TC.get("AssessmentFeatures.Title");
            ButtonContent = buttonContent;

            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            moveUp = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof(Texture2D));
            moveDown = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof(Texture2D));

            windowWidth = aStartPos.width;
            windowHeight = aStartPos.height;

            tableRect = new Rect(0f, 200, 0.9f * windowWidth, windowHeight * 0.33f);
            rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.33f * windowHeight);

            col_width = 0.88f / num_colums;

            variables = Controller.getInstance().getVarFlagSummary().getVars();

            this.completables = Controller.getInstance().getSelectedChapterDataControl().getCompletables();

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

        public override void Draw(int aID)
        {
            GUILayout.Label("Game starts in " + Controller.getInstance().getSelectedChapterDataControl().getInitialScene());
            GUILayout.Label("Ends when: ");

            end = EditorGUILayout.Popup(end, endoptions);

            GUILayout.Label("Game progress: ");

            progress = EditorGUILayout.Popup(progress, progressoptions);

            if (progress == 1)
            {
                GUILayout.Button("Edit Progress");
            }

            //GUILayout.BeginArea(tableRect);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(windowWidth * 0.9f));
            GUILayout.BeginHorizontal();
            GUILayout.Box("ID", GUILayout.Width(windowWidth * col_width));
            GUILayout.Box("Start", GUILayout.Width(windowWidth * col_width));
            GUILayout.Box("End", GUILayout.Width(windowWidth * col_width));
            GUILayout.Box("Progress", GUILayout.Width(windowWidth * col_width));
            GUILayout.Box("Score", GUILayout.Width(windowWidth * col_width));
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

                if (GUILayout.Button("Define progress", GUILayout.Width(windowWidth * col_width)))
                {
                    ProgressEditorWindow window = ScriptableObject.CreateInstance<ProgressEditorWindow>();
                    window.Init(completable.getProgress());
                }
                selected_variable[completable] = EditorGUILayout.Popup(selected_variable[completable], variables, GUILayout.Width(windowWidth * col_width));
                completable.getScore().setId(variables[selected_variable[completable]]);

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

            //GUILayout.EndArea();
        }

        protected override void OnButton() {}
    }
}
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

// needed for Regex

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class CutscenesWindowEndConfiguration : LayoutWindow
    {
        private string[] possibleOptions;
        private int selectedOption, selectedOptionLast;

        private string[] transitionTypes;
        private int selectedTransitionType, selectedTransitionTypeLast;

        private string[] scenesNextNames;
        private string[] cutscenesNextNames;
        private string[] joinedNextScenesList;
        private int selectedSceneNext, selectedSceneNextLast;
        
        private Rect selectorRect, goesToNewSceneRect;

        private string timeString, timeStringLast;
        private int timeInt;


        public CutscenesWindowEndConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            possibleOptions = new string[] { TC.get("Cutscene.ReturnToLastScene"), TC.get("Cutscene.ChapterEnd"), TC.get("Cutscene.GoToNextScene") };

            transitionTypes = new string[]
            {TC.get("NextScene.NoTransition"),TC.get("NextScene.TopToBottom"),  TC.get("NextScene.BottomToTop"), TC.get("NextScene.LeftToRight"), TC.get("NextScene.RightToLeft"), TC.get("NextScene.FadeIn")};

            scenesNextNames = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenesIDs();
            cutscenesNextNames = Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenesIDs();
            // Both scenes and cutscenes are necessary for next scene popup
            joinedNextScenesList = new string[scenesNextNames.Length + cutscenesNextNames.Length];
            scenesNextNames.CopyTo(joinedNextScenesList, 0);
            cutscenesNextNames.CopyTo(joinedNextScenesList, scenesNextNames.Length);
            
            selectedOption = selectedOptionLast = 0;
            selectedSceneNext = selectedSceneNextLast = 0;
            timeInt = 0;

            if (GameRources.GetInstance().selectedCutsceneIndex >= 0)
            {
                selectedOption =
                    selectedOptionLast =
                        Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                            GameRources.GetInstance().selectedCutsceneIndex].getNext();

                timeInt = Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                    GameRources.GetInstance().selectedCutsceneIndex].getTransitionTime();

                selectedTransitionType = selectedTransitionTypeLast =
                    Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                        GameRources.GetInstance().selectedCutsceneIndex].getTransitionType();

                string nextSceneID =
                    Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                        GameRources.GetInstance().selectedCutsceneIndex].getNextSceneId();

                selectedSceneNext =
                    selectedSceneNextLast =
                        Controller.Instance                            .SelectedChapterDataControl                            .getScenesList()
                            .getSceneIndexByID(nextSceneID);
                // if next scene is not a scene, but a cutscene...
                if (selectedSceneNext == -1)
                {
                    selectedSceneNext =
                     selectedSceneNextLast =
                     Controller.Instance                         .SelectedChapterDataControl                         .getCutscenesList()
                         .getCutsceneIndexByID(nextSceneID) + scenesNextNames.Length;
                }
            }

            timeString = timeStringLast = timeInt.ToString();
        }


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            selectorRect = new Rect(0f, 0.2f * windowHeight, 0.9f * windowWidth, 0.2f * windowHeight);
            goesToNewSceneRect = new Rect(0.2f * windowWidth, 0.35f * windowHeight, 0.6f * windowWidth, 0.35f * windowHeight);

            GUILayout.Label(TC.get("Cutscene.CutsceneEndReached"));
            GUILayout.Space(20);

            GUILayout.BeginArea(selectorRect);
            selectedOption = GUILayout.SelectionGrid(selectedOption, possibleOptions, 3, GUILayout.Width(0.9f * windowWidth));
            if (selectedOption != selectedOptionLast)
                ChangeSelectedOption(selectedOption);
            GUILayout.EndArea();

            GUILayout.BeginArea(goesToNewSceneRect);
            if (selectedOption == 2)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(TC.get("NextScene.Title"));

                selectedSceneNext = EditorGUILayout.Popup(selectedSceneNext, joinedNextScenesList);
                if (selectedSceneNext != selectedSceneNextLast)
                    ChangeSelectedNextScene(selectedSceneNext);

                if (GUILayout.Button(TC.get("GeneralText.EditEffects")))
                {
                    EffectEditorWindow window =
                            (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                    window.Init(Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                            GameRources.GetInstance().selectedCutsceneIndex].getEffects());
                }
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label(TC.get("NextScene.Transition"));

                selectedTransitionType = EditorGUILayout.Popup(selectedTransitionType, transitionTypes);
                if (selectedTransitionType != selectedTransitionTypeLast)
                    ChangeSelectedTransitionType(selectedTransitionType);

                GUILayout.Label(TC.get("NextScene.TransitionTime"));
                timeString = GUILayout.TextField(timeString, 3);
                timeString = Regex.Replace(timeString, @"[^0-9 ]", "");
                if (!timeString.Equals(timeStringLast))
                    ChangeSelectedTransitionTime(timeString);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void ChangeSelectedOption(int i)
        {
            selectedOptionLast = i;
            Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                      GameRources.GetInstance().selectedCutsceneIndex].setNext(i);
            Debug.Log("ChangeSelectedOption");
        }

        private void ChangeSelectedTransitionType(int i)
        {
            selectedTransitionTypeLast = i;
            Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                      GameRources.GetInstance().selectedCutsceneIndex].setTransitionType(i);
            Debug.Log("ChangeSelectedTransitionType");
        }

        private void ChangeSelectedTransitionTime(string t)
        {
            timeStringLast = t;
            Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                      GameRources.GetInstance().selectedCutsceneIndex].setTransitionTime(int.Parse(t));
            Debug.Log("ChangeSelectedTransitionTime");
        }

        private void ChangeSelectedNextScene(int i)
        {
            selectedSceneNextLast = i;
            // Scene was choosed
            if (i < scenesNextNames.Length)
                Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                    GameRources.GetInstance().selectedCutsceneIndex].setNextSceneId(scenesNextNames[i]);
            else
                Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                    GameRources.GetInstance().selectedCutsceneIndex].setNextSceneId(cutscenesNextNames[i - scenesNextNames.Length]);
            Debug.Log("ChangeSelectedNextScene");
        }
    }
}
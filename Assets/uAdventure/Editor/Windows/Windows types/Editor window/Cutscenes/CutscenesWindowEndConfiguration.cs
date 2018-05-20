using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

// needed for Regex

using uAdventure.Core;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class CutscenesWindowEndConfiguration : LayoutWindow
    {
        private CutsceneDataControl current;

        private string[] possibleOptions;
        private int selectedOption, selectedOptionLast;

        private string[] transitionTypes;
        private int selectedTransitionType, selectedTransitionTypeLast;

        private int selectedSceneNext, selectedSceneNextLast;
        private int time, timeLast;

        private Rect selectorRect, goesToNewSceneRect;

        
        public CutscenesWindowEndConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            possibleOptions = new string[] { TC.get("Cutscene.ReturnToLastScene"), TC.get("Cutscene.ChapterEnd"), TC.get("Cutscene.GoToNextScene") };

            transitionTypes = new string[]
            {TC.get("NextScene.NoTransition"),TC.get("NextScene.TopToBottom"),  TC.get("NextScene.BottomToTop"), TC.get("NextScene.LeftToRight"), TC.get("NextScene.RightToLeft"), TC.get("NextScene.FadeIn")};
            
        }


        public override void Draw(int aID)
        {

            List<string> allTargets = getSceneNames();

            current = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];
            
            selectedTransitionType = selectedTransitionTypeLast = current.getTransitionType();
            
            GUILayout.Label(TC.get("Cutscene.CutsceneEndReached"));
            GUILayout.Space(20);

            // Selected option
            selectedOptionLast = current.getNext();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            selectedOption = GUILayout.Toolbar(selectedOptionLast, possibleOptions, GUILayout.Width(m_Rect.width*0.8f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (selectedOption != selectedOptionLast)
                current.setNext(selectedOption);

            GUILayout.Space(20);
            EditorGUI.indentLevel++;

            if (selectedOption == 2)
            {
                // Next target
                selectedSceneNextLast = allTargets.IndexOf(current.getNextSceneId());
                selectedSceneNext = EditorGUILayout.Popup(TC.get("NextScene.Title"), selectedSceneNextLast, allTargets.ToArray());
                if (selectedSceneNext != selectedSceneNextLast)
                    current.setNextSceneId(allTargets[selectedSceneNext]);

                if (GUILayout.Button(TC.get("GeneralText.EditEffects")))
                {
                    EffectEditorWindow window =
                            (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                    window.Init(Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                            GameRources.GetInstance().selectedCutsceneIndex].getEffects());
                }

                selectedTransitionTypeLast = current.getTransitionType();
                selectedTransitionType = EditorGUILayout.Popup(TC.get("NextScene.Transition"), selectedTransitionTypeLast, transitionTypes);
                if (selectedTransitionType != selectedTransitionTypeLast)
                    current.setTransitionType(selectedTransitionType);

                timeLast = current.getTransitionTime();
                time = Mathf.Clamp(EditorGUILayout.IntField(TC.get("NextScene.TransitionTime"), timeLast), 0, 1000);
                if (!time.Equals(timeLast))
                    current.setTransitionTime(time);

            }
            EditorGUI.indentLevel--;
        }

        private List<string> getSceneNames()
        {
            var all = Controller.Instance.ChapterList.getSelectedChapterData().getObjects();

            var names = new List<object>();
            foreach (var e in all) names.Add(e);

            return names.FindAll(o => o is IChapterTarget).ConvertAll(o => (o as IChapterTarget).getId());
        }
    }
}
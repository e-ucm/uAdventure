using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

// needed for Regex

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    public class CutscenesWindowEndConfiguration : LayoutWindow
    {
        private CutsceneDataControl current;

        private readonly string[] possibleOptions, transitionTypes;
        
        public CutscenesWindowEndConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            possibleOptions = new string[]
            {
                TC.get("Cutscene.ReturnToLastScene"),
                TC.get("Cutscene.ChapterEnd"),
                TC.get("Cutscene.GoToNextScene")
            };

            transitionTypes = new string[]
            {
                TC.get("NextScene.NoTransition"),
                TC.get("NextScene.TopToBottom"),
                TC.get("NextScene.BottomToTop"),
                TC.get("NextScene.LeftToRight"),
                TC.get("NextScene.RightToLeft"),
                TC.get("NextScene.FadeIn")
            };
            
        }


        public override void Draw(int aID)
        {
            var allTargets = GetSceneNames();

            current = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];
            
            GUILayout.Label(TC.get("Cutscene.CutsceneEndReached"));
            GUILayout.Space(20);

            // Selected option
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                var selectedOption = GUILayout.Toolbar(current.getNext(), possibleOptions, GUILayout.Width(m_Rect.width * 0.8f));
                if (EditorGUI.EndChangeCheck())
                {
                    current.setNext(selectedOption);
                }
                GUILayout.FlexibleSpace();
            }

            if (current.getNext() != 2)
            {
                return;
            }

            GUILayout.Space(20);

            using (new EditorGUI.IndentLevelScope())
            {
                // Next target
                var nextSceneIndex = Mathf.Max(0, allTargets.IndexOf(current.getNextSceneId()));
                EditorGUI.BeginChangeCheck();
                nextSceneIndex = EditorGUILayout.Popup(TC.get("NextScene.Title"), nextSceneIndex, allTargets.ToArray());
                if (EditorGUI.EndChangeCheck() && nextSceneIndex.InRange(0, allTargets.Count-1))
                {
                    current.setNextSceneId(allTargets[nextSceneIndex]);
                }

                // Effects
                if (GUILayout.Button(TC.get("GeneralText.EditEffects")))
                {
                    var window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                    window.Init(Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[
                        GameRources.GetInstance().selectedCutsceneIndex].getEffects());
                }
                
                // Transition Type
                EditorGUI.BeginChangeCheck();
                var newType = EditorGUILayout.Popup(TC.get("NextScene.Transition"), (int)current.getTransitionType(), transitionTypes);
                if (EditorGUI.EndChangeCheck())
                {
                    current.setTransitionType((TransitionType)newType);
                }

                // Transition Time
                EditorGUI.BeginChangeCheck();
                var time = Mathf.Clamp(EditorGUILayout.IntField(TC.get("NextScene.TransitionTime"), current.getTransitionTime()), 0, 5000);
                if (EditorGUI.EndChangeCheck())
                {
                    current.setTransitionTime(time);
                }
            }
        }

        private List<string> GetSceneNames()
        {
            return Controller.Instance.ChapterList.getSelectedChapterData()
                .getObjects()
                .OfType<IChapterTarget>()
                .Select(t => t.getId())
                .ToList();
        }
    }
}
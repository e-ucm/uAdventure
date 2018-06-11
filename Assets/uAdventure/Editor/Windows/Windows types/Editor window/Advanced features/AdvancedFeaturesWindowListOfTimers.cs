using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    public class AdvancedFeaturesWindowListOfTimers : LayoutWindow
    {
        
        private GUIStyle smallFontStyle;
        private DataControlList timerList;

        public AdvancedFeaturesWindowListOfTimers(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
      
            smallFontStyle = new GUIStyle();
            smallFontStyle.fontSize = 8;


            timerList = new DataControlList()
            {
                RequestRepaint = Repaint,
                footerHeight = 25,
                elementHeight = 25,
                Columns = new System.Collections.Generic.List<ColumnList.Column>()
                {
                    new ColumnList.Column()
                    {
                        Text = TC.get("TimersList.Timer"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("TimersList.Time"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("TimersList.Display"),
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    }
                },
                drawCell = (rect, index, column, isActive, isFocused) =>
                {
                    var timer = timerList.list[index] as TimerDataControl;
                    switch (column)
                    {
                        case 0:
                            EditorGUI.BeginChangeCheck();
                            var id = EditorGUI.DelayedTextField(rect, timer.getDisplayName());
                            if (EditorGUI.EndChangeCheck()) timer.setDisplayName(id);
                            break;
                        case 1:
                            EditorGUI.BeginChangeCheck();
                            var time = System.Math.Max(0, EditorGUI.LongField(rect, timer.getTime()));
                            if (EditorGUI.EndChangeCheck()) timer.setTime(time);
                            break;
                        case 2:
                            EditorGUI.BeginChangeCheck();
                            var showTime = EditorGUI.Toggle(rect, timer.isShowTime());
                            if (EditorGUI.EndChangeCheck()) timer.setShowTime(showTime);
                            break;

                    }
                }
            };
        }

        public override void Draw(int aID)
        {
            var workingTimerList = Controller.Instance.SelectedChapterDataControl.getTimersList();

            timerList.SetData(workingTimerList, (data) => (data as TimersListDataControl).getTimers().Cast<DataControl>().ToList());
            timerList.DoList(220);

            using (new EditorGUI.DisabledScope(timerList.index < 0 || timerList.index >= workingTimerList.getTimers().Count))
            {
                var workingTimer = timerList.index >= 0 ? workingTimerList.getTimers()[timerList.index] : new TimerDataControl(new Timer());

                // ################
                // ### Time section
                // ################

                EditorGUI.BeginChangeCheck();
                var showTime = EditorGUILayout.BeginToggleGroup(TC.get("TimersList.Display"), workingTimer.isShowTime());
                if (EditorGUI.EndChangeCheck()) workingTimer.setShowTime(showTime);
                {
                    EditorGUI.indentLevel++;

                    // Display name
                    EditorGUI.BeginChangeCheck();
                    var id = EditorGUILayout.TextField(TC.get("Item.Name"), workingTimer.getDisplayName());
                    if (EditorGUI.EndChangeCheck()) workingTimer.setDisplayName(id);

                    // CountDown
                    EditorGUI.BeginChangeCheck();
                    var countDown = EditorGUILayout.ToggleLeft(TC.get("Timer.CountDown"), workingTimer.isCountDown());
                    if (EditorGUI.EndChangeCheck()) workingTimer.setCountDown(countDown);

                    // Show when stopped
                    EditorGUI.BeginChangeCheck();
                    var shownWhenStopped = EditorGUILayout.ToggleLeft(TC.get("Timer.ShowWhenStopped"), workingTimer.isShowWhenStopped());
                    if (EditorGUI.EndChangeCheck()) workingTimer.setShowWhenStopped(shownWhenStopped);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();

                // ################
                // ### Loop Control
                // ################

                GUILayout.Label(TC.get("Timer.LoopControl"));
                {
                    EditorGUI.indentLevel++;

                    // Show when stopped
                    EditorGUI.BeginChangeCheck();
                    var multipleStarts = EditorGUILayout.ToggleLeft(TC.get("Timer.MultipleStarts"), workingTimer.isMultipleStarts());
                    if (EditorGUI.EndChangeCheck()) workingTimer.setMultipleStarts(multipleStarts);

                    EditorGUILayout.HelpBox(TC.get("Timer.MultipleStartsDesc"), MessageType.Info);

                    // Show when stopped
                    EditorGUI.BeginChangeCheck();
                    var runsInLoop = EditorGUILayout.ToggleLeft(TC.get("Timer.RunsInLoop"), workingTimer.isRunsInLoop());
                    if (EditorGUI.EndChangeCheck()) workingTimer.setRunsInLoop(runsInLoop);

                    EditorGUILayout.HelpBox(TC.get("Timer.RunsInLoopDesc"), MessageType.Info);

                    EditorGUI.indentLevel--;
                }

                // ################
                // ### Conditions
                // ################

                // Init conditions
                GUILayout.Label(TC.get("Timer.InitConditions"));
                {
                    EditorGUI.indentLevel++;

                    if (GUILayout.Button(TC.get("GeneralText.EditInitConditions")))
                    {
                        ConditionEditorWindow window = (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                        window.Init(workingTimer.getInitConditions());
                    }

                    EditorGUI.indentLevel--;
                }

                // End conditions
                GUILayout.Label(TC.get("Timer.EndConditions"));
                {
                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck();
                    var usesEndCondition = EditorGUILayout.BeginToggleGroup(TC.get("Timer.UsesEndConditionShort"), workingTimer.isUsesEndCondition());
                    if (EditorGUI.EndChangeCheck()) workingTimer.setUsesEndCondition(usesEndCondition);

                    if (GUILayout.Button(TC.get("GeneralText.EditEndConditions")))
                    {
                        ConditionEditorWindow window = (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                        window.Init(workingTimer.getEndConditions());
                    }

                    EditorGUILayout.EndToggleGroup();
                    EditorGUI.indentLevel--;
                }

                // ################
                // ### Effects
                // ################

                // Header
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(TC.get("Timer.Effects"));
                GUILayout.FlexibleSpace();
                GUILayout.Label(TC.get("Timer.PostEffects"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                // Buttons
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(TC.get("GeneralText.EditEffects")))
                {
                    EffectEditorWindow window =
                    (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                    window.Init(workingTimer.getEffects());
                }
                if (GUILayout.Button(TC.get("GeneralText.EditPostEffects")))
                {
                    EffectEditorWindow window =
                    (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                    window.Init(workingTimer.getPostEffects());
                }
                GUILayout.EndHorizontal();


                GUILayout.Label(TC.get("Timer.Documentation"));
                EditorGUI.BeginChangeCheck();
                var newDocumentation= GUILayout.TextArea(workingTimer.getDocumentation() ?? string.Empty, GUILayout.ExpandHeight(true));
                if (EditorGUI.EndChangeCheck())
                    workingTimer.setDocumentation(newDocumentation);
            }
        }
    }
}
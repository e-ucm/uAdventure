using UnityEngine;
using UnityEditor;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class FlagConditionEditor : ConditionEditor, DialogReceiverInterface
    {
        private const string DEFAULT_FLAG_ID = "IdFlag";
        private FlagCondition condition = new FlagCondition("");
        private readonly string[] types = { TC.get("Conditions.Flag.Active"), TC.get("Conditions.Flag.Inactive") };
        private readonly string name = TC.get("Flags.Flag");
        private static GUIStyle collapseStyle;

        public FlagConditionEditor()
        {
            if (Available)
            {
                condition = new FlagCondition(Controller.Instance.VarFlagSummary.getFlags()[0]);
            }

            if (collapseStyle == null)
            {
                collapseStyle = new GUIStyle(GUI.skin.button);
                collapseStyle.padding = new RectOffset(0, 0, 0, 0);
                collapseStyle.margin = new RectOffset(0, 5, 2, 0);
                collapseStyle.normal.textColor = Color.blue;
                collapseStyle.focused.textColor = Color.blue;
                collapseStyle.active.textColor = Color.blue;
                collapseStyle.hover.textColor = Color.blue;
            }
        }

        public void draw(Condition c)
        {
            condition = c as FlagCondition;
            using (new EditorGUILayout.HorizontalScope())
            {
                var target = c.getId();
                if (Available)
                {
                    var flags = Controller.Instance.VarFlagSummary.getFlags();
                    var index = Mathf.Max(0, Array.IndexOf(flags, c.getId()));
                    c.setId(flags[EditorGUILayout.Popup(index >= 0 ? index : 0, flags)]);
                }
                else
                {
                    using(new GUILayout.HorizontalScope(GUILayout.Height(15)))
                    {
                        EditorGUILayout.HelpBox(TC.get("Condition.Flag.Warning"), MessageType.Error);
                    }
                }

                if (GUILayout.Button("New", collapseStyle, GUILayout.Width(35), GUILayout.Height(15)))
                {
                    Controller.Instance.ShowInputDialog(TC.get("Flags.AddFlag"), TC.get("Flags.AddFlagMessage"), DEFAULT_FLAG_ID, condition, this);
                }

                if (Available)
                {
                    c.setState(EditorGUILayout.Popup(c.getState(), types));
                }

                var newTarget = c.getId();
                if (target != newTarget)
                {
                    if (!string.IsNullOrEmpty(target))
                    {
                        Controller.Instance.VarFlagSummary.deleteFlagReference(target);
                    }
                    Controller.Instance.VarFlagSummary.addFlagReference(newTarget);
                    Controller.Instance.DataModified();
                }
            }
        }

        public bool manages(Condition c)
        {
            return c.GetType() == condition.GetType();
        }

        public string conditionName()
        {
            return name;
        }

        public Condition InstanceManagedCondition()
        {
            return new FlagCondition("");
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            Controller.Instance.VarFlagSummary.addFlag(message);
            if(condition == null)
            {
                condition = new FlagCondition(message);
            }
            else
            {
                if (!string.IsNullOrEmpty(condition.getId()))
                {
                    Controller.Instance.VarFlagSummary.deleteFlagReference(condition.getId());
                }
                condition.setId(message);
            }

            Controller.Instance.VarFlagSummary.addFlagReference(message);
            Controller.Instance.DataModified();
        }

        public void OnDialogCanceled(object workingObject = null) {}

        public bool Collapsed { get; set; }
        public Rect Window { get; set; }
        public bool Available { get { return Controller.Instance.VarFlagSummary.getFlags().Length > 0; } }
    }
}
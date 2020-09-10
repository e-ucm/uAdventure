using UnityEngine;
using UnityEditor;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AdventureWindow : DefaultButtonMenuEditorWindowExtension
    {
        private Vector2 scroll;

        public AdventureWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
            ButtonContent = new GUIContent("Settings");
        }

        public override void Draw(int id)
        {
            if (!Controller.Instance.Loaded)
            {
                EditorGUILayout.HelpBox("Adventure is not loaded.", MessageType.Error);
                return;
            }

            using (var scope = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = scope.scrollPosition;
                EditorGUIUtility.labelWidth = Rect.width - 30;

                var adventureData = Controller.Instance.AdventureData;
                EditData(adventureData);
                EditMode(adventureData);
                EditComentaries(adventureData);
                EditKeepShowing(adventureData);
                EditDefaultClickAction(adventureData);
                EditPerspective(adventureData);
                EditDragBehaviour(adventureData);
                EditKeyboardNavigation(adventureData);
                ShowVersionNumber(adventureData);

                if (Event.current.type == EventType.Repaint)
                {
                    var lastRect = GUILayoutUtility.GetLastRect();
                    var size = new Vector2(400, lastRect.y + lastRect.height + 2);
                }
            }
        }

        private static void ShowVersionNumber(AdventureDataControl adventureData)
        {
            EditorGUILayout.LabelField(TC.get("VersionNumber"), EditorStyles.boldLabel);
            GUILayout.Box(adventureData.getVersionNumber(), GUILayout.ExpandWidth(true));
        }

        private static void EditKeyboardNavigation(AdventureDataControl adventureData)
        {
            EditorGUILayout.LabelField(TC.get("MenuAdventure.KeyboardNavigationEnabled"), EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true)) // TODO add navigation control by keyboard
            {
                EditorGUI.BeginChangeCheck();
                var newKeyboardNavigation = EditorGUILayout.Toggle(TC.get("MenuAdventure.KeyboardNavigationEnabled.Checkbox"), adventureData.isKeyboardNavigationEnabled(), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setKeyboardNavigation(newKeyboardNavigation);
                }
            }
        }

        private static void EditDragBehaviour(AdventureDataControl adventureData)
        {
            EditorGUILayout.LabelField(TC.get("DragBehaviour.Explanation"), EditorStyles.boldLabel);
            int[] dragValues = { (int)DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS, (int)DescriptorData.DragBehaviour.IGNORE_NON_TARGETS };
            string[] dragTexts = { TC.get("DragBehaviour.ConsiderNonTargets"), TC.get("DragBehaviour.IgnoreNonTrargets") };
            using (new EditorGUI.DisabledScope(true)) // TODO add drag values
            {
                EditorGUI.BeginChangeCheck();
                var newDrag = EditorGUILayout.IntPopup((int)adventureData.getDragBehaviour(), dragTexts, dragValues);
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setDragBehaviour((DescriptorData.DragBehaviour)newDrag);
                }
            }
        }

        private static void EditPerspective(AdventureDataControl adventureData)
        {
            // Perspectives
            EditorGUILayout.LabelField(TC.get("Perspective.Explanation"), EditorStyles.boldLabel);
            int[] perspectiveValues = { (int)DescriptorData.Perspective.REGULAR, (int)DescriptorData.Perspective.ISOMETRIC };
            string[] perspectiveTexts = { TC.get("Perspective.Regular"), TC.get("Perspective.Isometric") };
            using (new EditorGUI.DisabledScope(true)) // TODO add perspectives
            {
                EditorGUI.BeginChangeCheck();
                var newPerspective = EditorGUILayout.IntPopup((int)adventureData.getPerspective(), perspectiveTexts, perspectiveValues);
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setPerspective((DescriptorData.Perspective)newPerspective);
                }
            }
        }

        private static void EditDefaultClickAction(AdventureDataControl adventureData)
        {
            // Default click action
            EditorGUILayout.LabelField(TC.get("DefaultClickAction.Explanation"), EditorStyles.boldLabel);
            int[] clickValues = { (int)DescriptorData.DefaultClickAction.SHOW_DETAILS, (int)DescriptorData.DefaultClickAction.SHOW_ACTIONS };
            string[] clickTexts = { TC.get("DefaultClickAction.ShowDetails"), TC.get("DefaultClickAction.ShowActions") };
            using (new EditorGUI.DisabledScope(true)) // TODO add clickAction control
            {
                EditorGUI.BeginChangeCheck();
                var newDefaultClickAction = EditorGUILayout.IntPopup((int)adventureData.getDefaultClickAction(), clickTexts, clickValues);
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setDefaultClickAction((DescriptorData.DefaultClickAction)newDefaultClickAction);
                }
            }
        }

        private static void EditKeepShowing(AdventureDataControl adventureData)
        {
            // Keep showing texts or change after time
            EditorGUILayout.LabelField(TC.get("MenuAdventure.KeepShowing"), EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var newKeep = EditorGUILayout.Toggle(TC.get("MenuAdventure.KeepText"), adventureData.isKeepShowing(), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                adventureData.setKeepShowing(newKeep);
            }
        }

        private static void EditComentaries(AdventureDataControl adventureData)
        {
            // Adventure commentaries
            EditorGUILayout.LabelField(TC.get("MenuAdventure.Commentaries"), EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var newCommentaries = EditorGUILayout.Toggle(TC.get("MenuAdventure.CommentariesLabel"), adventureData.isCommentaries(), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                adventureData.setCommentaries(newCommentaries);
            }
        }

        private static void EditMode(AdventureDataControl adventureData)
        {
            EditorGUILayout.LabelField(TC.get("Adventure.PlayerMode"), EditorStyles.boldLabel);
            EditorGUILayout.LabelField(TC.get("Adventure.CurrentPlayerMode"));

            string modeName;
            string modeDescription;

            switch (adventureData.getPlayerMode())
            {
                case DescriptorData.MODE_PLAYER_3RDPERSON:
                    modeName = TC.get("Adventure.ModePlayerTransparent.Name");
                    modeDescription = TC.get("Adventure.ModePlayerTransparent.Description");
                    break;
                default:
                    modeName = TC.get("Adventure.ModePlayerVisible.Name");
                    modeDescription = TC.get("Adventure.ModePlayerVisible.Description");
                    break;
            }
            GUILayout.Box(modeName, GUILayout.ExpandWidth(true));
            GUILayout.Box(modeDescription);
        }

        private static void EditData(AdventureDataControl adventureData)
        {
            EditorGUILayout.LabelField(TC.get("Adventure.Title"), EditorStyles.boldLabel);

            EditorGUILayout.LabelField(TC.get("Adventure.AdventureTitle"));
            EditorGUI.BeginChangeCheck();
            var newTitle = EditorGUILayout.TextField(adventureData.getTitle());
            if (EditorGUI.EndChangeCheck())
            {
                adventureData.setTitle(newTitle);
            }

            EditorGUILayout.LabelField(TC.get("Adventure.AdventureDescription"));
            EditorGUI.BeginChangeCheck();
            var newDescription = EditorGUILayout.TextArea(adventureData.getDescription(), GUILayout.Height(80));
            if (EditorGUI.EndChangeCheck())
            {
                adventureData.setDescription(newDescription);
            }
        }

        protected override void OnButton()
        {
        }
    }
}
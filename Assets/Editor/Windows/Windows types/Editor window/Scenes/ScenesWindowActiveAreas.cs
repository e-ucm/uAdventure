using UnityEngine;
using  UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ScenesWindowActiveAreas : LayoutWindow, DialogReceiverInterface
    {

        private Texture2D backgroundPreviewTex = null;
        private Texture2D conditionTex = null;

        private Texture2D addTexture = null;
        private Texture2D moveUp, moveDown = null;
        private Texture2D clearImg = null;
        private Texture2D duplicateImg = null;

        private string backgroundPath = "";

        private static float windowWidth, windowHeight;
        private static Rect tableRect;
        private static Rect actionRect;
        private static Rect previewRect;
        private static Rect infoPreviewRect;
        private Rect rightPanelRect;
        private Rect actionRightPanelRect;

        private static Vector2 scrollPosition;
        private static Vector2 scrollPositionAction;

        private static GUISkin selectedAreaSkin;
        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;

        private int selectedArea;
        private int selectedAction;
        private AddItemActionMenu addMenu;

        public ScenesWindowActiveAreas(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            moveUp = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof(Texture2D));
            moveDown = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof(Texture2D));
            duplicateImg = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

            windowWidth = aStartPos.width;
            windowHeight = aStartPos.height;

            if (GameRources.GetInstance().selectedSceneIndex >= 0)
                backgroundPath =
                    Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();
            if (backgroundPath != null && !backgroundPath.Equals(""))
                backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

            conditionTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));
            noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));

            tableRect = new Rect(0f, 0.1f * windowHeight, 0.9f * windowWidth, windowHeight * 0.33f);
            rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.33f * windowHeight);
            infoPreviewRect = new Rect(0f, 0.65f * windowHeight, windowWidth, windowHeight * 0.05f);
            previewRect = new Rect(0f, 0.7f * windowHeight, windowWidth, windowHeight * 0.25f);
            actionRect = new Rect(0f, 0.45f * windowHeight, 0.9f * windowWidth, windowHeight * 0.2f);
            actionRightPanelRect = new Rect(0.9f * windowWidth, 0.45f * windowHeight, 0.1f * windowWidth, windowHeight * 0.2f);

            selectedArea = -1;
            selectedAction = -1;
        }

        public override void Draw(int aID)
        {
            GUILayout.BeginArea(tableRect);
            GUILayout.BeginHorizontal();
            GUILayout.Box(TC.get("ActiveAreasList.Id"), GUILayout.Width(windowWidth * 0.54f));
            GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth * 0.14f));
            GUILayout.Box(TC.get("ActiveAreasList.Documentation"), GUILayout.Width(windowWidth * 0.18f));
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0;
                i <
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreasList().Count;
                i++)
            {
                if (i == selectedArea)
                    GUI.skin = selectedAreaSkin;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreasList()[i].getId(),
                    GUILayout.Width(windowWidth * 0.54f)))
                {
                    OnSelectionChanged(i);
                }

                if (GUILayout.Button(conditionTex, GUILayout.Width(windowWidth * 0.14f)))
                {
                    OnSelectionChanged(i);

                    ConditionEditorWindow window =
                        (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreasList()[i]
                        .getConditions());
                }
                if (GUILayout.Button(TC.get("GeneralText.EditDocumentation"), GUILayout.Width(windowWidth * 0.18f)))
                {
                    OnSelectionChanged(i);
                }

                GUILayout.EndHorizontal();
                GUI.skin = defaultSkin;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();



            /*
            * Right panel
            */
            GUILayout.BeginArea(rightPanelRect);
            GUI.skin = noBackgroundSkin;
            if (GUILayout.Button(addTexture, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                ActiveAreaNewName window =
                    (ActiveAreaNewName)ScriptableObject.CreateInstance(typeof(ActiveAreaNewName));
                window.Init(this, "IdObject");
            }
            if (GUILayout.Button(duplicateImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                    .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]);
            }
            if (GUILayout.Button(moveUp, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                    .moveElementUp(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]);
            }
            if (GUILayout.Button(moveDown, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                    .moveElementDown(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]);
            }
            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                    .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea],
                        false);
                if (selectedArea >= 0)
                    selectedArea--;
                if (selectedAction >= 0)
                    selectedAction--;
            }
            GUI.skin = defaultSkin;
            GUILayout.EndArea();

            /**
            * ACTION EDITOR
            */
            GUILayout.BeginArea(actionRect);
            GUILayout.BeginHorizontal();
            GUILayout.Box(TC.get("Element.Action"), GUILayout.Width(windowWidth * 0.39f));
            GUILayout.Box(TC.get("ActionsList.NeedsGoTo"), GUILayout.Width(windowWidth * 0.39f));
            GUILayout.Box(TC.get("Element.Effects") + "/" + TC.get("SmallAction.Conditions"), GUILayout.Width(windowWidth * 0.1f));
            GUILayout.EndHorizontal();
            scrollPositionAction = GUILayout.BeginScrollView(scrollPositionAction, GUILayout.ExpandWidth(false));
            if (selectedArea >= 0)
            {
                // Action table
                for (int i = 0;
                    i < Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]
                        .getActionsList().getActions().Count;
                    i++)
                {
                    if (i == selectedAction)
                        GUI.skin = selectedAreaSkin;
                    else
                        GUI.skin = noBackgroundSkin;

                    GUILayout.BeginHorizontal();
                    if (i == selectedAction)
                    {

                        GUILayout.Label(
                            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                    selectedArea
                                ].getActionsList().getActions()[i].getTypeName(),
                            GUILayout.Width(windowWidth * 0.39f));
                        if (Controller.getInstance().playerMode() == Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
                        {
                            if (GUILayout.Button(TC.get("ActionsList.NotRelevant"), GUILayout.Width(windowWidth * 0.39f)))
                            {
                                selectedAction = i;
                            }
                        }
                        else
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(windowWidth * 0.39f));
                            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                    selectedArea].getActionsList().getActions()[i].setNeedsGoTo(
                                        GUILayout.Toggle(
                                            Controller.getInstance()
                                                .getSelectedChapterDataControl()
                                                .getScenesList()
                                                .getScenes()[
                                                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                                                .getActiveAreas()[selectedArea].getActionsList().getActions()[i]
                                                .getNeedsGoTo(), ""));
                            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                    selectedArea].getActionsList().getActions()[i].setKeepDistance(
                                        EditorGUILayout.IntField(
                                            Controller.getInstance()
                                                .getSelectedChapterDataControl()
                                                .getScenesList()
                                                .getScenes()[
                                                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                                                .getActiveAreas()[selectedArea].getActionsList().getActions()[i]
                                                .getKeepDistance()));

                            GUILayout.EndHorizontal();
                        }


                        GUILayout.BeginVertical();
                        if (GUILayout.Button(TC.get("ActiveAreasList.Conditions"), GUILayout.Width(windowWidth * 0.1f)))
                        {
                            ConditionEditorWindow window =
                                (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                            window.Init(
                                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                        selectedArea
                                    ].getActionsList().getActions()[i].getConditions());
                        }
                        if (GUILayout.Button(TC.get("Element.Effects"), GUILayout.Width(windowWidth * 0.1f)))
                        {
                            EffectEditorWindow window =
                                (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                            window.Init(
                                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                        selectedArea
                                    ].getActionsList().getActions()[i].getEffects());
                        }
                        if (GUILayout.Button(TC.get("SmallAction.EditNotEffects"), GUILayout.Width(windowWidth * 0.1f)))
                        {
                            EffectEditorWindow window =
                                (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                            window.Init(
                                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                        selectedArea
                                    ].getActionsList().getActions()[i].getNotEffectsController());
                        }
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (
                            GUILayout.Button(
                                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                        selectedArea].getActionsList().getActions()[i].getTypeName(),
                                GUILayout.Width(windowWidth * 0.39f)))
                        {
                            selectedAction = i;
                        }

                        if (Controller.getInstance().playerMode() == Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
                        {
                            if (GUILayout.Button(TC.get("ActionsList.NotRelevant"), GUILayout.Width(windowWidth * 0.39f)))
                            {
                                selectedAction = i;
                            }
                        }
                        else
                        {
                            if (
                                GUILayout.Button(
                                    Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                                            selectedArea].getActionsList().getActions()[i].getNeedsGoTo().ToString(),
                                    GUILayout.Width(windowWidth * 0.39f)))
                            {
                                selectedAction = i;
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = defaultSkin;
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            /*
            * Right action panel
            */
            GUILayout.BeginArea(actionRightPanelRect);
            GUI.skin = noBackgroundSkin;
            if (GUILayout.Button(addTexture, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                addMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(duplicateImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]
                    .getActionsList()
                    .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]
                        .getActionsList().getActions()[selectedAction]);
            }
            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]
                    .getActionsList()
                    .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[selectedArea]
                        .getActionsList().getActions()[selectedAction], false);
                if (selectedAction >= 0)
                    selectedAction--;
            }
            GUI.skin = defaultSkin;
            GUILayout.EndArea();





            if (backgroundPath != "")
            {

                GUILayout.BeginArea(infoPreviewRect);
                // Show preview dialog
                if (GUILayout.Button(TC.get("DefaultClickAction.ShowDetails") + "/" + TC.get("GeneralText.Edit")))
                {
                    ActiveAreasEditor window =
                        (ActiveAreasEditor)ScriptableObject.CreateInstance(typeof(ActiveAreasEditor));
                    window.Init(this, Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex], selectedArea);
                }
                GUILayout.EndArea();
                GUI.DrawTexture(previewRect, backgroundPreviewTex, ScaleMode.ScaleToFit);

            }
            else
            {
                GUILayout.BeginArea(infoPreviewRect);
                GUILayout.Button("No background!");
                GUILayout.EndArea();
            }
        }

        private void OnSelectionChanged(int i)
        {
            selectedArea = i;
            selectedAction = -1;
            addMenu = new AddItemActionMenu(i);
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            if (workingObject is ActiveAreaNewName)
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList()
                    .addElement(Controller.ACTIVE_AREA, message);
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log(TC.get("GeneralText.Cancel"));
        }

        #region Add item action options

        class AddItemActionMenu : WindowMenuContainer
        {
            private AddUseAction useAction;
            private AddExamineAction examineAction;
            private AddGrabAction grabAction;
            private AddCustomAction customAction;
            private static int selectedActiveArea;

            public static int GetSelectedActiveArea()
            {
                return selectedActiveArea;
            }

            public AddItemActionMenu(int i)
            {
                selectedActiveArea = i;
                SetMenuItems();
            }

            protected override void Callback(object obj)
            {
                if ((obj as AddUseAction) != null)
                    useAction.OnCliked();
                else if ((obj as AddExamineAction) != null)
                    examineAction.OnCliked();
                else if ((obj as AddGrabAction) != null)
                    grabAction.OnCliked();
                else if ((obj as AddCustomAction) != null)
                    customAction.OnCliked();
            }

            protected override void SetMenuItems()
            {
                menu = new GenericMenu();

                useAction = new AddUseAction(TC.get("TreeNode.AddElement23"));
                examineAction = new AddExamineAction(TC.get("TreeNode.AddElement21"));
                grabAction = new AddGrabAction(TC.get("TreeNode.AddElement22"));
                customAction = new AddCustomAction("Add \"Custom\" action");


                menu.AddItem(new GUIContent(useAction.Label), false, Callback, useAction);
                menu.AddItem(new GUIContent(examineAction.Label), false, Callback, examineAction);
                menu.AddItem(new GUIContent(grabAction.Label), false, Callback, grabAction);
                // menu.AddItem(new GUIContent(customAction.Label), false, Callback, customAction);
            }
        }

        class AddUseAction : IMenuItem
        {
            public AddUseAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                        AddItemActionMenu.GetSelectedActiveArea()].getActionsList().addElement(Controller.ACTION_USE, "");
            }
        }

        class AddExamineAction : IMenuItem
        {
            public AddExamineAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                        AddItemActionMenu.GetSelectedActiveArea()].getActionsList()
                    .addElement(Controller.ACTION_EXAMINE, "");
            }
        }

        class AddGrabAction : IMenuItem
        {
            public AddGrabAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                        AddItemActionMenu.GetSelectedActiveArea()].getActionsList().addElement(Controller.ACTION_GRAB, "");
            }
        }

        class AddCustomAction : IMenuItem
        {
            public AddCustomAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {

                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getActiveAreasList().getActiveAreas()[
                        AddItemActionMenu.GetSelectedActiveArea()].getActionsList().addElement(Controller.ACTION_CUSTOM, "");
            }
        }

        #endregion
    }
}
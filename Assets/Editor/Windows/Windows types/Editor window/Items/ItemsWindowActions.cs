using UnityEngine;
using UnityEditor;
using System.Collections;

public class ItemsWindowActions : LayoutWindow
{

    private Texture2D addTex = null;
    private Texture2D duplicateTex = null;
    private Texture2D clearTex = null;
    private Texture2D conditionsTex = null;
    private Texture2D noConditionsTex = null;
    private Texture2D moveUp, moveDown = null;
    private Texture2D tmpTex = null;

    private static float windowWidth, windowHeight;
    private static Rect actionTableRect, rightPanelRect, descriptionRect, effectsRect;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedAreaSkin;

    private Vector2 scrollPosition;

    private int selectedAction;

    private AddItemActionMenu addMenu;

    private string documentation = "", documentationLast = "";

    private string[] itemsNames;
    private string[] charactersNames;
    private string[] joinedNamesList;
    private int selectedTarget, selectedTargetLast;

    public ItemsWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        clearTex = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        addTex = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));
        duplicateTex = (Texture2D) Resources.Load("EAdventureData/img/icons/duplicateNode", typeof (Texture2D));

        conditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof (Texture2D));
        noConditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof (Texture2D));

        moveUp = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof (Texture2D));
        moveDown = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof (Texture2D));

        itemsNames = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItemsIDs();
        charactersNames = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCsIDs();
        // Both scenes and cutscenes are necessary for next scene popup
        joinedNamesList = new string[itemsNames.Length + charactersNames.Length + 1];
        joinedNamesList[0] = "none";
        itemsNames.CopyTo(joinedNamesList, 1);
        charactersNames.CopyTo(joinedNamesList, itemsNames.Length + 1);

        selectedTarget = selectedTargetLast = 0;

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));
        selectedAreaSkin = (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof (GUISkin));

        actionTableRect = new Rect(0f, 0.1f*windowHeight, 0.9f*windowWidth, 0.5f*windowHeight);
        rightPanelRect = new Rect(0.9f*windowWidth, 0.1f*windowHeight, 0.08f*windowWidth, 0.5f*windowHeight);
        descriptionRect = new Rect(0f, 0.6f*windowHeight, 0.95f*windowWidth, 0.2f*windowHeight);
        effectsRect = new Rect(0f, 0.8f*windowHeight, windowWidth, windowHeight*0.15f);

        selectedAction = -1;

        addMenu = new AddItemActionMenu();
    }


    public override void Draw(int aID)
    {
        GUILayout.BeginArea(actionTableRect);
        GUILayout.BeginHorizontal();
        GUILayout.Box(TC.get("Element.Action"), GUILayout.Width(windowWidth*0.39f));
        GUILayout.Box(TC.get("ActionsList.NeedsGoTo"), GUILayout.Width(windowWidth*0.39f));
        GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth*0.1f));
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(false));

        // Action table
        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions().Count;
            i++)
        {
            if (i == selectedAction)
                GUI.skin = selectedAreaSkin;
            else
                GUI.skin = noBackgroundSkin;

            tmpTex = (Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].getConditions()
                .getBlocksCount() > 0
                ? conditionsTex
                : noConditionsTex);

            GUILayout.BeginHorizontal();
            if (i == selectedAction)
            {
                int t = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].getType();

                if (t == Controller.ACTION_USE_WITH || t == Controller.ACTION_GIVE_TO || t == Controller.ACTION_DRAG_TO)
                {
                    selectedTarget =
                        EditorGUILayout.Popup(
                            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i]
                                .getTypeName(),
                            selectedTarget, joinedNamesList,
                            GUILayout.Width(windowWidth*0.39f));
                    if (selectedTarget != selectedTargetLast)
                        ChangeActionTarget(selectedTarget);
                }
                else
                {
                    GUILayout.Label(
                        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                            GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].getTypeName(),
                        GUILayout.Width(windowWidth*0.39f));
                }


                if (Controller.getInstance().playerMode() == Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
                {
                    if (GUILayout.Button(TC.get("ActionsList.NotRelevant"), GUILayout.Width(windowWidth*0.39f)))
                    {
                        OnActionSelectionChange(i);
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(windowWidth * 0.39f));

                    Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].setNeedsGoTo(
                            GUILayout.Toggle(
                                Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i]
                                    .getNeedsGoTo(), ""));
                    Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].setKeepDistance(
                            EditorGUILayout.IntField(
                                Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i]
                                    .getKeepDistance()));

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth*0.1f)))
                {
                    ConditionEditorWindow window =
                        (ConditionEditorWindow) ScriptableObject.CreateInstance(typeof (ConditionEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].getConditions());
                }
            }
            else
            {
                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i].getTypeName(),
                    GUILayout.Width(windowWidth*0.39f)))
                {
                    OnActionSelectionChange(i);
                }


                if (Controller.getInstance().playerMode() == Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
                {
                    if (GUILayout.Button(TC.get("ActionsList.NotRelevant"), GUILayout.Width(windowWidth*0.39f)))
                    {
                        OnActionSelectionChange(i);
                    }
                }
                else
                {
                    if (
                        GUILayout.Button(
                            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[i]
                                .getNeedsGoTo().ToString(), GUILayout.Width(windowWidth*0.39f)))
                    {
                        OnActionSelectionChange(i);
                    }
                }
                if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth*0.1f)))
                {
                    OnActionSelectionChange(i);
                }
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
        if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            addMenu.menu.ShowAsContext();
        }
        if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList()
                .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]);
        }
        //if (GUILayout.Button(moveUp, GUILayout.MaxWidth(0.08f * windowWidth)))
        //{
        //    Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
        //      GameRources.GetInstance().selectedItemIndex].getActionsList().moveElementUp(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
        //          GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]);
        //}
        //if (GUILayout.Button(moveDown, GUILayout.MaxWidth(0.08f * windowWidth)))
        //{
        //    Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
        //      GameRources.GetInstance().selectedItemIndex].getActionsList().moveElementDown(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
        //          GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]);
        //}
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList()
                .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction], false);
            if (selectedAction >= 0)
                selectedAction--;
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();

        GUILayout.BeginArea(descriptionRect);
        GUILayout.Label(TC.get("Action.Documentation"));
        GUILayout.Space(20);
        documentation = GUILayout.TextArea(documentation);
        if (!documentation.Equals(documentationLast))
            OnDocumentationChanged(documentation);
        GUILayout.EndArea();

        GUILayout.BeginArea(effectsRect);
        if (selectedAction < 0)
            GUI.enabled = false;
        if (GUILayout.Button(TC.get("Element.Effects")))
        {
            EffectEditorWindow window =
                (EffectEditorWindow) ScriptableObject.CreateInstance(typeof (EffectEditorWindow));
            window.Init(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].getEffects());
        }
        GUI.enabled = true;
        GUILayout.EndArea();
    }

    private void OnActionSelectionChange(int i)
    {
        selectedAction = i;
        // Refresh docs
        string doc = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]
            .getDocumentation();
        if (!string.IsNullOrEmpty(doc))
        {
            documentation =
                documentationLast = doc;
        }
        else
        {
            documentation = documentationLast = "";
        }

        // Refresh target info
        string targetID = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].getIdTarget();

        selectedTarget =
            selectedTargetLast =
                Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getItemsList()
                    .getItemIndexByID(targetID);
        // if target is not an item, but a npc...
        if (selectedTarget == -1)
        {
            selectedTarget =
                selectedTargetLast =
                    Controller.getInstance()
                        .getSelectedChapterDataControl()
                        .getNPCsList()
                        .getNPCIndexByID(targetID);

            if (selectedTarget == -1)
            {
                selectedTarget = selectedTargetLast = 0;
            }
            else
            {
                selectedTarget = selectedTargetLast += itemsNames.Length;
            }
        }

    }

    private void OnDocumentationChanged(string s)
    {
        if (selectedAction >= 0 && Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction] != null)
        {
            documentationLast = s;
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]
                .setDocumentation(s);
        }
    }

    private void ChangeActionTarget(int i)
    {
        selectedTargetLast = i;
        // Scene was choosed
        if (i < itemsNames.Length)

            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].setIdTarget(
                    itemsNames[i]);
        else
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].setIdTarget(
                    charactersNames[i - itemsNames.Length]);
    }


    #region Add item action options

    class AddItemActionMenu : WindowMenuContainer
    {
        private AddUseAction useAction;
        private AddExamineAction examineAction;
        private AddGrabAction grabAction;
        private AddCustomAction customAction;
        private AddUseWithAction useWithAction;
        private AddGiveToAction giveToAction;
        private AddDragToAction dragToAction;

        public AddItemActionMenu()
        {
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
            else if ((obj as AddUseWithAction) != null)
                useWithAction.OnCliked();
            else if ((obj as AddGiveToAction) != null)
                giveToAction.OnCliked();
            else if ((obj as AddDragToAction) != null)
                dragToAction.OnCliked();
        }

        protected override void SetMenuItems()
        {
            menu = new GenericMenu();

            useAction = new AddUseAction(TC.get("TreeNode.AddElement23"));
            examineAction = new AddExamineAction(TC.get("TreeNode.AddElement21"));
            grabAction = new AddGrabAction(TC.get("TreeNode.AddElement22"));
            customAction = new AddCustomAction("Add \"Custom\" action");
            useWithAction = new AddUseWithAction(TC.get("TreeNode.AddElement24"));
            giveToAction = new AddGiveToAction(TC.get("TreeNode.AddElement25"));
            dragToAction = new AddDragToAction(TC.get("TreeNode.AddElement251"));

            menu.AddItem(new GUIContent(useAction.Label), false, Callback, useAction);
            menu.AddItem(new GUIContent(examineAction.Label), false, Callback, examineAction);
            menu.AddItem(new GUIContent(grabAction.Label), false, Callback, grabAction);
           // menu.AddItem(new GUIContent(customAction.Label), false, Callback, customAction);
            menu.AddItem(new GUIContent(useWithAction.Label), false, Callback, useWithAction);
            menu.AddItem(new GUIContent(giveToAction.Label), false, Callback, giveToAction);
            menu.AddItem(new GUIContent(dragToAction.Label), false, Callback, dragToAction);
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
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_USE, "");
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
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_EXAMINE, "");
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
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_GRAB, "");
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
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_CUSTOM, "");
        }
    }

    class AddUseWithAction : IMenuItem
    {
        public AddUseWithAction(string name_)
        {
            this.Label = name_;
        }

        public string Label { get; set; }

        public void OnCliked()
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_USE_WITH, "");
        }
    }

    class AddGiveToAction : IMenuItem
    {
        public AddGiveToAction(string name_)
        {
            this.Label = name_;
        }

        public string Label { get; set; }

        public void OnCliked()
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_GIVE_TO, "");
        }
    }

    class AddDragToAction : IMenuItem
    {
        public AddDragToAction(string name_)
        {
            this.Label = name_;
        }

        public string Label { get; set; }

        public void OnCliked()
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_DRAG_TO, "");
        }
    }

    #endregion
}
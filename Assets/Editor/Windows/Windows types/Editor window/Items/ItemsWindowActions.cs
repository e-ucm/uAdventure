using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System.Linq;

namespace uAdventure.Editor
{
    public class ItemsWindowActions : LayoutWindow
    {

        private Texture2D addTex = null;
        private Texture2D duplicateTex = null;
        private Texture2D clearTex = null;
        private Texture2D conditionsTex = null;
        private Texture2D noConditionsTex = null;
        private Texture2D moveUp, moveDown = null;
        private Texture2D tmpTex = null;
        
        private static Rect actionTableRect, rightPanelRect, descriptionRect, effectsRect;

        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;
        private static GUISkin selectedAreaSkin;

        private Vector2 scrollPosition;

        private int selectedAction;

        private AddItemActionMenu addMenu;
        private DataControlList actionsList;

        private string documentation = "", documentationLast = "";

        private string[] itemsNames;
        private string[] charactersNames;
        private string[] joinedNamesList;
        private int selectedTarget, selectedTargetLast;

        public ItemsWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTex = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            duplicateTex = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            moveUp = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof(Texture2D));
            moveDown = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof(Texture2D));

            itemsNames = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            charactersNames = Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCsIDs();
            // Both scenes and cutscenes are necessary for next scene popup
            joinedNamesList = new string[itemsNames.Length + charactersNames.Length + 1];
            joinedNamesList[0] = "none";
            itemsNames.CopyTo(joinedNamesList, 1);
            charactersNames.CopyTo(joinedNamesList, itemsNames.Length + 1);

            selectedTarget = selectedTargetLast = 0;

            noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
            selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

            selectedAction = -1;

            addMenu = new AddItemActionMenu();

            actionsList = new DataControlList();

            actionsList.elementHeight = 40;
            actionsList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                float nameRectWidth = 120, needsToGoWidth = 120, conditionsWidth = 60, effectsWidth = 60;
                float descriptionWidth = rect.width - nameRectWidth - needsToGoWidth - conditionsWidth - effectsWidth;
                
                var nameRect = new Rect(rect.x, rect.y, nameRectWidth, rect.height);
                var descriptionRect = new Rect(nameRect.x + nameRect.width, rect.y, descriptionWidth, rect.height);
                var needsToGoRect = new Rect(descriptionRect.x + descriptionRect.width, rect.y, needsToGoWidth, rect.height);
                var conditionsRect = new Rect(needsToGoRect.x + needsToGoRect.width, rect.y, conditionsWidth, rect.height);
                var effectsRect = new Rect(conditionsRect.x + conditionsRect.width, rect.y, effectsWidth, rect.height);

                var action = actionsList.list[index] as ActionDataControl;
                switch(action.getType())
                {
                    case Controller.ACTION_USE_WITH:
                    case Controller.ACTION_GIVE_TO:
                    case Controller.ACTION_DRAG_TO:
                        var leftHalf = new Rect(nameRect);
                        leftHalf.width /= 2f;
                        var rightHalf = new Rect(leftHalf);
                        rightHalf.x += leftHalf.width;
                        EditorGUI.LabelField(leftHalf, action.getTypeName());
                        if (isActive)
                        {
                            EditorGUI.LabelField(rightHalf, action.hasIdTarget() ? action.getIdTarget() : "---");
                        }
                        else
                        {
                            // TODO
                        }
                        break;
                    default:
                        EditorGUI.LabelField(nameRect, action.getTypeName());
                        break;
                }

                EditorGUI.BeginChangeCheck();
                var documentation = EditorGUI.TextArea(descriptionRect, action.getDocumentation() ?? string.Empty);
                if(EditorGUI.EndChangeCheck()) action.setDocumentation(documentation);

                if (Controller.Instance.playerMode() == Controller.FILE_ADVENTURE_1STPERSON_PLAYER)
                {
                    EditorGUI.LabelField(needsToGoRect, TC.get("ActionsList.NotRelevant"));
                }
                else
                {
                    var leftHalf = new Rect(needsToGoRect);
                    leftHalf.width /= 2f;
                    var rightHalf = new Rect(leftHalf);
                    rightHalf.x += leftHalf.width;

                    EditorGUI.BeginChangeCheck();
                    var needsToGo = EditorGUI.Toggle(leftHalf, action.getNeedsGoTo());
                    if (EditorGUI.EndChangeCheck()) action.setNeedsGoTo(needsToGo);
                    
                    EditorGUI.BeginChangeCheck();
                    var distance = EditorGUI.IntField(rightHalf, action.getKeepDistance());
                    if (EditorGUI.EndChangeCheck()) action.setKeepDistance(distance);;
                }

                if (GUI.Button(conditionsRect, action.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                {
                    ConditionEditorWindow window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                    window.Init(action.getConditions());
                }
                if (GUI.Button(effectsRect, "Effects"))
                {
                    EffectEditorWindow window = ScriptableObject.CreateInstance<EffectEditorWindow>();
                    window.Init(action.getEffects());
                }
            };
        }


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            var workingItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex];

            actionsList.SetData(workingItem.getActionsList(), (data) => (data as ActionsListDataControl).getActions().Cast<DataControl>().ToList());
            actionsList.DoList(windowHeight- 60f);
        }

        private void OnActionSelectionChange(int i)
        {
            selectedAction = i;
            // Refresh docs
            string doc = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
            string targetID = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].getIdTarget();

            selectedTarget =
                selectedTargetLast =
                    Controller.Instance                        .SelectedChapterDataControl                        .getItemsList()
                        .getItemIndexByID(targetID);
            // if target is not an item, but a npc...
            if (selectedTarget == -1)
            {
                selectedTarget =
                    selectedTargetLast =
                        Controller.Instance                            .SelectedChapterDataControl                            .getNPCsList()
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
            if (selectedAction >= 0 && Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction] != null)
            {
                documentationLast = s;
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction]
                    .setDocumentation(s);
            }
        }

        private void ChangeActionTarget(int i)
        {
            selectedTargetLast = i;
            // Scene was choosed
            if (i < itemsNames.Length)

                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().getActions()[selectedAction].setIdTarget(
                        itemsNames[i]);
            else
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
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
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getActionsList().addElement(Controller.ACTION_DRAG_TO, "");
            }
        }

        #endregion
    }
}
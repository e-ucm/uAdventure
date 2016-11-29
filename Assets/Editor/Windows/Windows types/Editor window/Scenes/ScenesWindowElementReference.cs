using UnityEngine;
using UnityEditor;
using uAdventure.Core;

namespace uAdventure.Editor
{

    public class ScenesWindowElementReference : LayoutWindow, DialogReceiverInterface
    {

        private Texture2D backgroundPreviewTex = null;
        private Texture2D conditionTex = null;

        private Texture2D addTexture = null;
        private Texture2D moveUp, moveDown = null;
        private Texture2D clearImg = null;

        private string backgroundPath = "";

        private static float windowWidth, windowHeight;
        private static Rect tableRect;
        private static Rect previewRect;
        private Rect rightPanelRect;
        private static Rect infoPreviewRect;

        private static Vector2 scrollPosition;

        private static GUISkin selectedElementSkin;
        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;

        private ElementContainer selectedElement;
        private AddItemActionMenu addMenu;

        private int currentIndex = -1;
        private SceneDataControl currentScene;

        public ScenesWindowElementReference(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            moveUp = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof(Texture2D));
            moveDown = (Texture2D)Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof(Texture2D));

            windowWidth = aStartPos.width;
            windowHeight = aStartPos.height;

            if (GameRources.GetInstance().selectedSceneIndex >= 0)
            {
                currentIndex = GameRources.GetInstance().selectedSceneIndex;
                currentScene = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[currentIndex];
                backgroundPath = currentScene.getPreviewBackground();
            }
            if (backgroundPath != null && !backgroundPath.Equals(""))
                backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

            conditionTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            selectedElementSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));
            noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));

            tableRect = new Rect(0f, 0.1f * windowHeight, 0.9f * windowWidth, windowHeight * 0.33f);
            rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.33f * windowHeight);
            infoPreviewRect = new Rect(0f, 0.45f * windowHeight, windowWidth, windowHeight * 0.05f);
            previewRect = new Rect(0f, 0.5f * windowHeight, windowWidth, windowHeight * 0.45f);

            selectedElement = null;
            addMenu = new AddItemActionMenu();
        }

        public override void Draw(int aID)
        {
            if (currentScene == null)
                if (GameRources.GetInstance().selectedSceneIndex >= 0 && currentIndex != GameRources.GetInstance().selectedSceneIndex)
                {
                    currentIndex = GameRources.GetInstance().selectedSceneIndex;
                    currentScene = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[currentIndex];
                }
                else
                    return;

            GUILayout.BeginArea(tableRect);
            GUILayout.BeginHorizontal();
            GUILayout.Box(TC.get("ElementList.Layer"), GUILayout.Width(windowWidth * 0.12f));
            GUILayout.Box("", GUILayout.Width(windowWidth * 0.06f));
            GUILayout.Box(TC.get("ElementList.Title"), GUILayout.Width(windowWidth * 0.39f));
            GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth * 0.29f));
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            int i = 0;
            foreach (ElementContainer element in currentScene.getReferencesList().getAllReferencesDataControl())
            {
                if (element == selectedElement)
                    GUI.skin = selectedElementSkin;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(element.getLayer().ToString(), GUILayout.Width(windowWidth * 0.12f)))
                    selectedElement = element;

                if (element.getErdc() != null)
                {
                    // FOR ELEMENT ERDC
                    element.getErdc().setVisible(GUILayout.Toggle(element.getErdc().isVisible(), "", GUILayout.Width(windowWidth * 0.06f)));
                    if (GUILayout.Button(element.getErdc().getElementId(), GUILayout.Width(windowWidth * 0.39f)))
                    {
                        selectedElement = element;
                    }

                    if (GUILayout.Button(conditionTex, GUILayout.Width(windowWidth * 0.29f)))
                    {
                        selectedElement = element;
                        ConditionEditorWindow window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                        window.Init(element.getErdc().getConditions());
                    }
                }
                else
                {
                    if (GUILayout.Button("", GUILayout.Width(windowWidth * 0.06f)))
                        selectedElement = element;
                    if (GUILayout.Button("", GUILayout.Width(windowWidth * 0.39f)))
                        selectedElement = element;
                    if (GUILayout.Button(conditionTex, GUILayout.Width(windowWidth * 0.29f)))
                        selectedElement = element;
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
                addMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(moveUp, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                currentScene.getReferencesList().moveElementUp(selectedElement.getErdc());
            }
            if (GUILayout.Button(moveDown, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                currentScene.getReferencesList().moveElementDown(selectedElement.getErdc());
            }
            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                currentScene.getReferencesList().deleteElement(selectedElement.getErdc(), false);
            }
            GUI.skin = defaultSkin;
            GUILayout.EndArea();


            if (backgroundPath != "")
            {

                GUILayout.BeginArea(infoPreviewRect);
                // Show preview dialog
                // Button visible only is there is at least 1 object
                if (currentScene != null && selectedElement != null)
                {
                    if (GUILayout.Button(TC.get("DefaultClickAction.ShowDetails") + "/" + TC.get("GeneralText.Edit")))
                    {
                        ObjectInSceneRefrencesEditor window = ScriptableObject.CreateInstance<ObjectInSceneRefrencesEditor>();
                        window.Init(this, currentScene, currentScene.getReferencesList().getAllReferencesDataControl().IndexOf(selectedElement));
                    }
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

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            Debug.Log("Apply");
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log(TC.get("GeneralText.Cancel"));
        }

        #region Add ref action options

        class AddItemActionMenu : WindowMenuContainer
        {
            private AddItemAction itemAction;
            private AddSetItemAction setItemAction;
            private AddNPCAction npcAction;

            public AddItemActionMenu()
            {
                SetMenuItems();
            }

            protected override void Callback(object obj)
            {
                if ((obj as AddItemAction) != null)
                    itemAction.OnCliked();
                else if ((obj as AddSetItemAction) != null)
                    setItemAction.OnCliked();
                else if ((obj as AddNPCAction) != null)
                    npcAction.OnCliked();
            }

            protected override void SetMenuItems()
            {
                menu = new GenericMenu();

                itemAction = new AddItemAction(TC.get("TreeNode.AddElement6"));
                setItemAction = new AddSetItemAction(TC.get("TreeNode.AddElement60"));
                npcAction = new AddNPCAction(TC.get("TreeNode.AddElement8"));

                menu.AddItem(new GUIContent(itemAction.Label), false, Callback, itemAction);
                menu.AddItem(new GUIContent(setItemAction.Label), false, Callback, setItemAction);
                menu.AddItem(new GUIContent(npcAction.Label), false, Callback, npcAction);
            }
        }

        class AddItemAction : IMenuItem, DialogReceiverInterface
        {
            public AddItemAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {
                ObjectAddItemReference window = ScriptableObject.CreateInstance<ObjectAddItemReference>();
                window.Init(this);
            }

            public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
            {
                if (workingObject is ObjectAddItemReference)
                {
                    Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getReferencesList()
                        .addElement(Controller.ITEM_REFERENCE, message);
                }
            }

            public void OnDialogCanceled(object workingObject = null)
            {
                Debug.Log(TC.get("GeneralText.Cancel"));
            }
        }

        class AddSetItemAction : IMenuItem, DialogReceiverInterface
        {
            public AddSetItemAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {

                ObjectAddSetItemReference window = ScriptableObject.CreateInstance<ObjectAddSetItemReference>();
                window.Init(this);
            }

            public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
            {
                if (workingObject is ObjectAddSetItemReference)
                {
                    Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getReferencesList()
                        .addElement(Controller.ATREZZO_REFERENCE, message);
                }
            }

            public void OnDialogCanceled(object workingObject = null)
            {
                Debug.Log(TC.get("GeneralText.Cancel"));
            }
        }

        class AddNPCAction : IMenuItem, DialogReceiverInterface
        {
            public AddNPCAction(string name_)
            {
                this.Label = name_;
            }

            public string Label { get; set; }

            public void OnCliked()
            {
                ObjectAddNPCReference window =
                    (ObjectAddNPCReference)ScriptableObject.CreateInstance(typeof(ObjectAddNPCReference));
                window.Init(this);
            }

            public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
            {
                if (workingObject is ObjectAddNPCReference)
                {
                    Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getReferencesList()
                        .addElement(Controller.NPC_REFERENCE, message);
                }
            }

            public void OnDialogCanceled(object workingObject = null)
            {
                Debug.Log(TC.get("GeneralText.Cancel"));
            }
        }

        #endregion
    }
}
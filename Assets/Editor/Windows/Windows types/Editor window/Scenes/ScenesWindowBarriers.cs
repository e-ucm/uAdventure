using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Editor
{

    public class ScenesWindowBarriers : LayoutWindow, DialogReceiverInterface
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
        private static Rect previewRect;
        private static Rect infoPreviewRect;
        private Rect rightPanelRect;

        private static Vector2 scrollPosition;

        private static GUISkin selectedAreaSkin;
        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;

        private int selectedArea;

        public ScenesWindowBarriers(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
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
            infoPreviewRect = new Rect(0f, 0.45f * windowHeight, windowWidth, windowHeight * 0.05f);
            previewRect = new Rect(0f, 0.5f * windowHeight, windowWidth, windowHeight * 0.45f);

            selectedArea = 0;
        }

        public override void Draw(int aID)
        {
            GUILayout.BeginArea(tableRect);
            GUILayout.BeginHorizontal();
            GUILayout.Box(TC.get("Barrier.Title"), GUILayout.Width(windowWidth * 0.45f));
            GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth * 0.45f));
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0;
                i <
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriersList().Count;
                i++)
            {
                if (i == selectedArea)
                    GUI.skin = selectedAreaSkin;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriersList()[i].getId(),
                    GUILayout.Width(windowWidth * 0.44f)))
                {
                    selectedArea = i;
                }

                if (GUILayout.Button(conditionTex, GUILayout.Width(windowWidth * 0.44f)))
                {
                    selectedArea = i;
                    ConditionEditorWindow window =
                         (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriersList()[i].getConditions());
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
                BarrierNewName window =
                      (BarrierNewName)ScriptableObject.CreateInstance(typeof(BarrierNewName));
                window.Init(this, "IdBarrier");
            }
            if (GUILayout.Button(duplicateImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList()
                    .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriers()[selectedArea]);
            }
            if (GUILayout.Button(moveUp, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList()
                    .moveElementUp(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriers()[selectedArea]);
            }
            if (GUILayout.Button(moveDown, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList()
                    .moveElementDown(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriers()[selectedArea]);
            }
            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList()
                    .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getBarriersList().getBarriers()[selectedArea],
                        false);
            }
            GUI.skin = defaultSkin;
            GUILayout.EndArea();


            if (backgroundPath != "")
            {

                GUILayout.BeginArea(infoPreviewRect);
                // Show preview dialog
                if (GUILayout.Button(TC.get("DefaultClickAction.ShowDetails") + "/" + TC.get("GeneralText.Edit")))
                {
                    //
                    BarrierEditor window =
                        (BarrierEditor)ScriptableObject.CreateInstance(typeof(BarrierEditor));
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

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            Debug.Log("Apply");
            if (workingObject is BarrierNewName)
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getBarriersList()
                    .addElement(Controller.BARRIER, message);
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log(TC.get("GeneralText.Cancel"));
        }
    }
}
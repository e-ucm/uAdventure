using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ActiveAreasEditor : BaseAreaEditablePopup
    {
        private SceneDataControl sceneRef;
        private Texture2D backgroundPreviewTex = null;
        private Texture2D activeAreaTex = null;
        private Texture2D selectedAreaTex = null;
        private Texture2D exitTex = null;

        private Rect imageBackgroundRect;
        private Vector2 scrollPosition;

        private int x, y, width, heigth;

        private int calledAreaIndexRef;

        private Rect currentRect;
        private bool dragging;
        private Vector2 startPos;
        private Vector2 currentPos;

        public void Init(DialogReceiverInterface e, SceneDataControl scene, int areaIndex)
        {
            sceneRef = scene;
            calledAreaIndexRef = areaIndex;

            string backgroundPath =
                Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();

            backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

            activeAreaTex = (Texture2D)Resources.Load("Editor/ActiveArea", typeof(Texture2D));
            exitTex = (Texture2D)Resources.Load("Editor/ExitArea", typeof(Texture2D));
            selectedAreaTex = (Texture2D)Resources.Load("Editor/SelectedArea", typeof(Texture2D));
            float bgwidth = backgroundPreviewTex.width * (600f / backgroundPreviewTex.height);
            imageBackgroundRect = new Rect(0f, 0f, bgwidth, 600);

            x = sceneRef.getActiveAreasList().getActiveAreasList()[areaIndex].getX();
            y = sceneRef.getActiveAreasList().getActiveAreasList()[areaIndex].getY();
            width = sceneRef.getActiveAreasList().getActiveAreasList()[areaIndex].getWidth();
            heigth = sceneRef.getActiveAreasList().getActiveAreasList()[areaIndex].getHeight();

            base.Init(e, bgwidth, 600);
        }

        void OnGUI()
        {
            // Dragging events
            if (Event.current.type == EventType.mouseDrag)
            {
                if (currentRect.Contains(Event.current.mousePosition))
                {
                    if (!dragging)
                    {
                        dragging = true;
                        startPos = currentPos;
                    }
                }
                currentPos = Event.current.mousePosition;
            }

            if (Event.current.type == EventType.mouseUp)
            {
                dragging = false;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUI.DrawTexture(imageBackgroundRect, backgroundPreviewTex);

            for (int i = 0;
                i <
                sceneRef.getActiveAreasList().getActiveAreasList().Count;
                i++)
            {
                Rect aRect = new Rect(sceneRef.getActiveAreasList().getActiveAreasList()[i].getX(),
                    sceneRef.getActiveAreasList().getActiveAreasList()[i].getY(),
                    sceneRef.getActiveAreasList().getActiveAreasList()[i].getWidth(),
                    sceneRef.getActiveAreasList().getActiveAreasList()[i].getHeight());
                GUI.DrawTexture(aRect, activeAreaTex);

                // Frame around current area
                if (calledAreaIndexRef == i)
                {
                    currentRect = aRect;
                    GUI.DrawTexture(aRect, selectedAreaTex);
                }
            }

            for (int i = 0;
                i <
                sceneRef.getExitsList().getExitsList().Count;
                i++)
            {
                Rect eRect = new Rect(sceneRef.getExitsList().getExitsList()[i].getX(),
                    sceneRef.getExitsList().getExitsList()[i].getY(), sceneRef.getExitsList().getExitsList()[i].getWidth(),
                    sceneRef.getExitsList().getExitsList()[i].getHeight());
                GUI.DrawTexture(eRect, exitTex);
            }

            /*
             *HANDLE EVENTS
             */
            if (dragging)
            {
                OnBeingDragged();
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.Box("X", GUILayout.Width(0.25f * backgroundPreviewTex.width));
            GUILayout.Box("Y", GUILayout.Width(0.25f * backgroundPreviewTex.width));
            GUILayout.Box(TC.get("SPEP.Width"), GUILayout.Width(0.25f * backgroundPreviewTex.width));
            GUILayout.Box(TC.get("SPEP.Height"), GUILayout.Width(0.25f * backgroundPreviewTex.width));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            x = EditorGUILayout.IntField(sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].getX(),
             GUILayout.Width(0.25f * backgroundPreviewTex.width));
            y = EditorGUILayout.IntField(sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].getY(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));
            width = EditorGUILayout.IntField(sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].getWidth(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));
            heigth = EditorGUILayout.IntField(sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].getHeight(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));
            sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].setValues(x, y, width, heigth);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Ok"))
            {
                reference.OnDialogOk("Applied");
                this.Close();
            }
            //if (GUILayout.Button(TC.get("GeneralText.Cancel")))
            //{
            //    reference.OnDialogCanceled();
            //    this.Close();
            //}
            GUILayout.EndHorizontal();
        }

        private void OnBeingDragged()
        {
            x = (int)currentPos.x - (int)(0.5f * sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].getWidth());
            y = (int)currentPos.y - (int)(0.5f * sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].getHeight());
            sceneRef.getActiveAreasList().getActiveAreasList()[calledAreaIndexRef].setValues(x, y, width, heigth);
            Repaint();
        }
    }
}
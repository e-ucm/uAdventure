using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ExitsEditor : BaseAreaEditablePopup
    {
        private SceneDataControl sceneRef;

        private Texture2D backgroundPreviewTex = null;

        private Texture2D activeAreaTex = null;
        private Texture2D exitTex = null;
        private Texture2D selectedExitTex = null;

        private Rect imageBackgroundRect;
        private Vector2 scrollPosition;

        //private string xString, yString, widthString, heightString;
        //private string xStringLast, yStringLast, widthStringLast, heightStringLast;

        private int x, y, width, heigth;

        private int HALF_WIDTH, HALF_HEIGHT;

        private int calledExitIndexRef;

        private Rect currentRect;
        private bool dragging;
        private Vector2 startPos;
        private Vector2 currentPos;

        public void Init(DialogReceiverInterface e, SceneDataControl scene, int exitIndex)
        {
            sceneRef = scene;
            calledExitIndexRef = exitIndex;

            string backgroundPath =
                Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();

            backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

            activeAreaTex = (Texture2D)Resources.Load("Editor/ActiveArea", typeof(Texture2D));
            exitTex = (Texture2D)Resources.Load("Editor/ExitArea", typeof(Texture2D));
            selectedExitTex = (Texture2D)Resources.Load("Editor/SelectedArea", typeof(Texture2D));

            imageBackgroundRect = new Rect(0f, 0f, backgroundPreviewTex.width, backgroundPreviewTex.height);

            x = sceneRef.getExitsList().getExitsList()[exitIndex].getX();
            y = sceneRef.getExitsList().getExitsList()[exitIndex].getY();
            width = sceneRef.getExitsList().getExitsList()[exitIndex].getWidth();
            heigth = sceneRef.getExitsList().getExitsList()[exitIndex].getHeight();
            HALF_WIDTH = (int)(0.5f * sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getWidth());
            HALF_HEIGHT = (int)(0.5f * sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getHeight());


            base.Init(e, backgroundPreviewTex.width, backgroundPreviewTex.height);
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
            }

            for (int i = 0;
                i <
                sceneRef.getExitsList().getExitsList().Count;
                i++)
            {
                Rect eRect = new Rect(sceneRef.getExitsList().getExitsList()[i].getX(),
                    sceneRef.getExitsList().getExitsList()[i].getY(),
                    sceneRef.getExitsList().getExitsList()[i].getWidth(),
                    sceneRef.getExitsList().getExitsList()[i].getHeight());
                GUI.DrawTexture(eRect, exitTex);

                // Frame around current area
                if (calledExitIndexRef == i)
                {
                    currentRect = eRect;
                    GUI.DrawTexture(eRect, selectedExitTex);
                }
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

            x = EditorGUILayout.IntField(sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getX(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));
            y = EditorGUILayout.IntField(sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getY(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));
            width = EditorGUILayout.IntField(sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getWidth(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));
            heigth = EditorGUILayout.IntField(sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getHeight(),
                GUILayout.Width(0.25f * backgroundPreviewTex.width));

            HALF_WIDTH = (int)(0.5f * sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getWidth());
            HALF_HEIGHT = (int)(0.5f * sceneRef.getExitsList().getExitsList()[calledExitIndexRef].getHeight());

            sceneRef.getExitsList().getExitsList()[calledExitIndexRef].setValues(x, y, width, heigth);

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
            x = (int)currentPos.x - HALF_WIDTH;
            y = (int)currentPos.y - HALF_HEIGHT;
            sceneRef.getExitsList().getExitsList()[calledExitIndexRef].setValues(x, y, width, heigth);
            Repaint();
        }
    }
}
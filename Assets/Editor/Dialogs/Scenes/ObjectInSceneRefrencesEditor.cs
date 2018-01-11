using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ObjectInSceneRefrencesEditor : BaseAreaEditablePopup
    {
        private SceneDataControl sceneRef;
        private Texture2D backgroundPreviewTex = null;
        private Texture2D selectedObjectTex = null;
        private List<Sprite> objectsTex = null;

        private Rect imageBackgroundRect;
        private Vector2 scrollPosition;

        private int calledItemIndexRef;

        private Rect currentRect;
        private bool dragging;
        private Vector2 startPos;
        private Vector2 currentPos;

        private int x, y;

        public void Init(DialogReceiverInterface e, SceneDataControl scene, int areaIndex)
        {
            sceneRef = scene;
            calledItemIndexRef = areaIndex;

            string backgroundPath = Controller.Instance.SelectedChapterDataControl.getScenesList()
                .getScenes()[GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();

            backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

            selectedObjectTex = (Texture2D)Resources.Load("Editor/SelectedArea", typeof(Texture2D));

            imageBackgroundRect = new Rect(0f, 0f, backgroundPreviewTex.width, backgroundPreviewTex.height);

            objectsTex = new List<Sprite>();
            foreach (
                ElementContainer element in
                    Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getReferencesList().getAllReferencesDataControl())
            {
                Debug.Log(element.getImage());
                objectsTex.Add(element.getImage());
            }

            base.Init(e, backgroundPreviewTex.width, backgroundPreviewTex.height);
        }

        void OnGUI()
        {
            // Dragging events
            if (Event.current.type == EventType.MouseDrag)
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

            if (Event.current.type == EventType.MouseUp)
            {
                dragging = false;
            }


            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUI.DrawTexture(imageBackgroundRect, backgroundPreviewTex);

            Texture2D tmptex;
            float tmpscale;
            ElementReferenceDataControl tmperdc;

            for (int i = 0; i < sceneRef.getReferencesList().getAllReferencesDataControl().Count; i++)
            {
                if (sceneRef.getReferencesList().getAllReferencesDataControl()[i].getErdc() != null)
                {
                    tmptex = objectsTex[i].texture;
                    tmpscale = sceneRef.getReferencesList().getAllReferencesDataControl()[i].getErdc().getElementScale();
                    tmperdc = sceneRef.getReferencesList().getAllReferencesDataControl()[i].getErdc();

                    Rect aRect = new Rect(
                            tmperdc.getElementX() - (tmpscale * tmptex.width) / 2,
                            tmperdc.getElementY() - (tmpscale * tmptex.height),
                            tmpscale * tmptex.width, tmpscale * tmptex.height);

                    if (objectsTex[i] != null)
                        GUI.DrawTexture(aRect, objectsTex[i].texture);

                    // Frame around current area
                    if (calledItemIndexRef == i)
                    {
                        currentRect = aRect;
                        GUI.DrawTexture(aRect, selectedObjectTex);
                    }
                }
            }
            GUILayout.EndScrollView();
            /*
              *HANDLE EVENTS
              */
            if (dragging)
            {
                OnBeingDragged();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box("X", GUILayout.Width(0.33f * backgroundPreviewTex.width));
            GUILayout.Box("Y", GUILayout.Width(0.33f * backgroundPreviewTex.width));
            GUILayout.Box(TC.get("SceneLocation.Scale"), GUILayout.Width(0.3f * backgroundPreviewTex.width));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            x = EditorGUILayout.IntField(
                    sceneRef.getReferencesList().getAllReferencesDataControl()[calledItemIndexRef].getErdc().getElementX(),
                    GUILayout.Width(0.33f * backgroundPreviewTex.width));
            y = EditorGUILayout.IntField(
                    sceneRef.getReferencesList().getAllReferencesDataControl()[calledItemIndexRef].getErdc().getElementY(),
                    GUILayout.Width(0.33f * backgroundPreviewTex.width));

            sceneRef.getReferencesList().getAllReferencesDataControl()[calledItemIndexRef].getErdc().setElementPosition(x, y);

            sceneRef.getReferencesList().getAllReferencesDataControl()[calledItemIndexRef].getErdc()
                .setElementScale(
                    EditorGUILayout.FloatField(
                        sceneRef.getReferencesList().getAllReferencesDataControl()[calledItemIndexRef].getErdc()
                            .getElementScale(), GUILayout.Width(0.33f * backgroundPreviewTex.width)));

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
            x = (int)currentPos.x + (int)(0.5f * currentRect.width);
            y = (int)currentPos.y + (int)(0.5f * currentRect.height);

            sceneRef.getReferencesList().getAllReferencesDataControl()[calledItemIndexRef].getErdc().setElementPosition(x, y);
        }
    }
}
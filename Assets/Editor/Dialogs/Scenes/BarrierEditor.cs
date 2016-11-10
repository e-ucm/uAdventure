using UnityEngine;
using UnityEditor;

public class BarrierEditor : BaseAreaEditablePopup
{
    private SceneDataControl sceneRef;
    private Texture2D backgroundPreviewTex = null;
    private Texture2D barrierTex = null;
    private Texture2D selectedBarrierTex = null;

    private Rect imageBackgroundRect;
    private Vector2 scrollPosition;

    private int x, y, width, heigth;

    private int calledBarrierIndexRef;

    private Rect currentRect;
    private bool dragging;
    private Vector2 startPos;
    private Vector2 currentPos;


    public void Init(DialogReceiverInterface e, SceneDataControl scene, int areaIndex)
    {
        sceneRef = scene;
        calledBarrierIndexRef = areaIndex;

        string backgroundPath =
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();

        backgroundPreviewTex =
            (Texture2D) Resources.Load(backgroundPath.Substring(0, backgroundPath.LastIndexOf(".")), typeof (Texture2D));

        barrierTex = (Texture2D) Resources.Load("Editor/BarrierArea", typeof (Texture2D));
        selectedBarrierTex = (Texture2D) Resources.Load("Editor/SelectedArea", typeof (Texture2D));

        imageBackgroundRect = new Rect(0f, 0f, backgroundPreviewTex.width, backgroundPreviewTex.height);

        x = sceneRef.getBarriersList().getBarriersList()[areaIndex].getX();
        y = sceneRef.getBarriersList().getBarriersList()[areaIndex].getY();
        width = sceneRef.getBarriersList().getBarriersList()[areaIndex].getWidth();
        heigth = sceneRef.getBarriersList().getBarriersList()[areaIndex].getHeight();

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
            sceneRef.getBarriersList().getBarriersList().Count;
            i++)
        {
            Rect aRect = new Rect(sceneRef.getBarriersList().getBarriersList()[i].getX(),
                sceneRef.getBarriersList().getBarriersList()[i].getY(),
                sceneRef.getBarriersList().getBarriersList()[i].getWidth(),
                sceneRef.getBarriersList().getBarriersList()[i].getHeight());
            GUI.DrawTexture(aRect, barrierTex);

            // Frame around current area
            if (calledBarrierIndexRef == i)
            {
                currentRect = aRect;
                GUI.DrawTexture(currentRect, selectedBarrierTex);
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
        GUILayout.Box("X", GUILayout.Width(0.25f*backgroundPreviewTex.width));
        GUILayout.Box("Y", GUILayout.Width(0.25f*backgroundPreviewTex.width));
        GUILayout.Box(TC.get("SPEP.Width"), GUILayout.Width(0.25f*backgroundPreviewTex.width));
        GUILayout.Box(TC.get("SPEP.Height"), GUILayout.Width(0.25f*backgroundPreviewTex.width));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        x = EditorGUILayout.IntField(sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].getX(),
            GUILayout.Width(0.25f * backgroundPreviewTex.width));
        y = EditorGUILayout.IntField(sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].getY(),
            GUILayout.Width(0.25f * backgroundPreviewTex.width));
        width = EditorGUILayout.IntField(sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].getWidth(),
            GUILayout.Width(0.25f * backgroundPreviewTex.width));
        heigth = EditorGUILayout.IntField(sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].getHeight(),
            GUILayout.Width(0.25f * backgroundPreviewTex.width));
        sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].setValues(x, y, width, heigth);

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
        x = (int)currentPos.x - (int)(0.5f * sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].getWidth());
        y = (int)currentPos.y - (int)(0.5f * sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].getHeight());
        sceneRef.getBarriersList().getBarriersList()[calledBarrierIndexRef].setValues(x, y, width, heigth);
        Repaint();
    }
}
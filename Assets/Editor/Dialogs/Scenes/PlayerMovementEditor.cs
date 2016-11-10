using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class PlayerMovementEditor : BaseAreaEditablePopup
{
    private enum TrajectoryToolType
    {
        EDIT_NODE,
        EDIT_SIDE,
        INIT_NODE,
        DELETE_NODE
    }


    private SceneDataControl sceneRef;
    private Texture2D backgroundPreviewTex = null;
    private Texture2D selectedPlayerTex = null;
    private Texture2D playerTex = null;
    private Texture2D scaleTex = null;

    private static GUISkin selectedAreaSkin;
    private static GUISkin defaultSkin;

    private Vector2 scrollPosition;

    private Rect imageBackgroundRect;

    bool dragging = false;
    Vector2 startPos;
    Vector2 currentPos;

    private bool useTrajectory;
    private bool useInitialPosition, useInitialPositionLast;

    private int x, y;


    // Wrzucić do listy?
    private Rect playerRect;
    private float nextActionTime = 0.0f;
    public float period = 0.3f;


    // Trajectory
    private Texture2D editNodeTex = null;
    private Texture2D editSideTex = null;
    private Texture2D setInitialNodeTex = null;
    private Texture2D deleteTex = null;
    private Texture2D initialNodeTex = null;

    private int beginSideIndex = -1;
    private Texture2D lineTex = null;

    private Trajectory trajectory;

    private TrajectoryToolType trajectoryTool;

    

    public void Init(DialogReceiverInterface e, SceneDataControl scene)
    {
        sceneRef = scene;

        string backgroundPath =
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();
        backgroundPreviewTex =
            (Texture2D) Resources.Load(backgroundPath.Substring(0, backgroundPath.LastIndexOf(".")), typeof (Texture2D));

        string playerPath =
            Controller.getInstance().getSelectedChapterDataControl().getPlayer().getPreviewImage();
        playerTex = (Texture2D) Resources.Load(playerPath.Substring(0, playerPath.LastIndexOf(".")), typeof (Texture2D));

        selectedPlayerTex = (Texture2D) Resources.Load("Editor/SelectedArea", typeof (Texture2D));

        editNodeTex = (Texture2D) Resources.Load("EAdventureData/img/icons/nodeEdit", typeof (Texture2D));
        editSideTex = (Texture2D) Resources.Load("EAdventureData/img/icons/sideEdit", typeof (Texture2D));
        setInitialNodeTex = (Texture2D) Resources.Load("EAdventureData/img/icons/selectStartNode", typeof (Texture2D));
        deleteTex = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteTool", typeof (Texture2D));
        deleteTex = (Texture2D)Resources.Load("EAdventureData/img/icons/ScaleArea", typeof(Texture2D));

        initialNodeTex = (Texture2D)Resources.Load("EAdventureData/img/icons/selectStartNode", typeof(Texture2D));

        lineTex = (Texture2D)Resources.Load("Editor/LineTex", typeof(Texture2D));

        selectedAreaSkin = (GUISkin) Resources.Load("Editor/ButtonSelected", typeof (GUISkin));

        imageBackgroundRect = new Rect(0f, 0f, backgroundPreviewTex.width, backgroundPreviewTex.height);
        playerRect = new Rect(0f, 0f, playerTex.width, playerTex.height);

        useTrajectory = sceneRef.getTrajectory().hasTrajectory();
        useInitialPosition = useInitialPositionLast = !useTrajectory;
        trajectory = sceneRef.getTrajectory().GetTrajectory();

        x = sceneRef.getDefaultInitialPositionX();
        y = sceneRef.getDefaultInitialPositionY();

        playerRect = new Rect(x, y, playerTex.width,
            playerTex.height);

        base.Init(e, backgroundPreviewTex.width, backgroundPreviewTex.height);
    }

    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUI.DrawTexture(imageBackgroundRect, backgroundPreviewTex);
        //if (dragging)
        //    GUI.DrawTexture(playerRect, selectedPlayerTex);
        GUILayout.EndScrollView();

        if (Event.current.type == EventType.mouseDrag)
        {
            if (!useTrajectory)
            {
                // Check if start position is over player
                if (playerRect.Contains(Event.current.mousePosition))
                {
                    if (!dragging)
                    {
                        dragging = true;
                        startPos = currentPos;
                    }
                }
            }
            // For editing trajectory nodes recognizing object under mouse pointer is done
            // during iterating over all nodes
            else
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

        /*
        * Properties part
        */
        GUILayout.Label(TC.get("Scene.UseTrajectoryPanel"));
        GUILayout.Space(5);
        useTrajectory = GUILayout.Toggle(!useInitialPosition, new GUIContent(TC.get("Scene.UseTrajectory")));
        useInitialPosition = GUILayout.Toggle(!useTrajectory, new GUIContent(TC.get("Scene.UseInitialPosition")));
        if (useInitialPosition != useInitialPositionLast)
            OnMovementTypeChange(useInitialPosition);
        GUILayout.Space(5);



        /*
        * Initial positon
        */
        if (useInitialPosition)
        {
            // EVENT
            if (dragging)
            {
                OnBeingDragged();
            }

            playerRect = new Rect(x - 0.5f * playerTex.width * sceneRef.getPlayerScale(), y - playerTex.height * sceneRef.getPlayerScale(), playerTex.width * sceneRef.getPlayerScale(), playerTex.height * sceneRef.getPlayerScale());
            GUI.DrawTexture(playerRect, playerTex);

            GUILayout.BeginHorizontal();
            GUILayout.Box("X", GUILayout.Width(0.33f*backgroundPreviewTex.width));
            GUILayout.Box("Y", GUILayout.Width(0.33f*backgroundPreviewTex.width));
            GUILayout.Box(TC.get("SceneLocation.Scale"), GUILayout.Width(0.3f*backgroundPreviewTex.width));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            x = EditorGUILayout.IntField(
                    sceneRef.getDefaultInitialPositionX(),
                    GUILayout.Width(0.33f*backgroundPreviewTex.width));
            y = EditorGUILayout.IntField(
                    sceneRef.getDefaultInitialPositionY(),
                    GUILayout.Width(0.33f*backgroundPreviewTex.width));

            sceneRef.setDefaultInitialPosition(x,y);

            sceneRef.setPlayerScale(
                EditorGUILayout.FloatField(
                    sceneRef.getPlayerScale(), GUILayout.Width(0.33f*backgroundPreviewTex.width)));
            GUILayout.EndHorizontal();
        }
        /*
        * Trajectory
        */
        else
        {
            // EVENTS
            if (Event.current.type == EventType.mouseDown && imageBackgroundRect.Contains(Event.current.mousePosition))
            {
                int clickedIndex = -1;

                for (int i = 0; i < trajectory.getNodes().Count; i++)
                {
                    if (
                        trajectory.getNodes()[i].getEditorRect(playerTex.width, playerTex.height)
                            .Contains(Event.current.mousePosition))
                    {
                        clickedIndex = i;
                    }
                }

                if (trajectoryTool == TrajectoryToolType.EDIT_NODE)
                {
                    if (clickedIndex == -1)
                        AddNode(Event.current.mousePosition);
                }
                else if (trajectoryTool == TrajectoryToolType.DELETE_NODE)
                {
                    if (clickedIndex != -1)
                        DeleteNode(clickedIndex);
                }
                else if (trajectoryTool == TrajectoryToolType.INIT_NODE)
                {
                    if (clickedIndex != -1)
                        SetInitNode(clickedIndex);
                }
                else if (trajectoryTool == TrajectoryToolType.EDIT_SIDE)
                {
                    if (clickedIndex != -1)
                        SetSideNode(clickedIndex);
                }
            }

            if (dragging)
            {
                if (trajectoryTool == TrajectoryToolType.EDIT_NODE)
                {
                    int clickedIndex = -1;

                    for (int i = 0; i < trajectory.getNodes().Count; i++)
                    {
                        if (
                            trajectory.getNodes()[i].getEditorRect(playerTex.width, playerTex.height)
                                .Contains(Event.current.mousePosition))
                            clickedIndex = i;
                    }
                    if (clickedIndex != -1)
                    {

                        // LEFT MOUSE BUTTON - move node
                        if (Event.current.button == 0)
                            OnBeingDraggedTrajectoryNode(clickedIndex);
                    }
                }
            }

            //if (Event.current.type == EventType.ScrollWheel)
            if (Event.current.keyCode == KeyCode.Plus || Event.current.keyCode == KeyCode.KeypadPlus || Event.current.keyCode == KeyCode.Minus || Event.current.keyCode == KeyCode.KeypadMinus)
            {
                if (trajectoryTool == TrajectoryToolType.EDIT_NODE)
                {
                    int clickedIndex = -1;

                    for (int i = 0; i < trajectory.getNodes().Count; i++)
                    {
                        if (
                            trajectory.getNodes()[i].getEditorRect(playerTex.width, playerTex.height)
                                .Contains(Event.current.mousePosition))
                        {
                            clickedIndex = i;
                        }
                    }

                    if (clickedIndex != -1)
                    {
                        if(Event.current.keyCode == KeyCode.Plus || Event.current.keyCode == KeyCode.KeypadPlus)
                            OnBeingTrajectoryNodeRescaled(clickedIndex, 0.01f);
                        else if (Event.current.keyCode == KeyCode.Minus || Event.current.keyCode == KeyCode.KeypadMinus)
                            OnBeingTrajectoryNodeRescaled(clickedIndex, -0.01f);
                    }
                }
            }

            if (trajectoryTool == TrajectoryToolType.EDIT_SIDE)
            {
                // If selected begin of side
                if (beginSideIndex != -1)
                    DrawLine(
                        new Vector2(trajectory.getNodes()[beginSideIndex].getX(),
                            trajectory.getNodes()[beginSideIndex].getY()), Event.current.mousePosition, 2);
            }

            // DRAW NODES
            foreach (Trajectory.Node node in trajectory.getNodes())
            {
                GUI.DrawTexture(node.getEditorRect(playerRect.width, playerRect.height), playerTex);
            }

            // DRAW SIDES
            foreach (Trajectory.Side side in trajectory.getSides())
            {
                DrawLine(
                    new Vector2(trajectory.getNodeForId(side.getIDStart()).getX(),
                        trajectory.getNodeForId(side.getIDStart()).getY()),
                    new Vector2(trajectory.getNodeForId(side.getIDEnd()).getX(),
                        trajectory.getNodeForId(side.getIDEnd()).getY()), 2);
            }

            // DRAW INITIAL NODE
            if (trajectory.getInitial() != null)
                GUI.DrawTexture(trajectory.getInitial().getEditorRect(initialNodeTex.width, initialNodeTex.height),
                    initialNodeTex);

            // BUTTONS
            GUILayout.BeginHorizontal();

            if (trajectoryTool == TrajectoryToolType.EDIT_NODE)
                GUI.skin = selectedAreaSkin;
            if (GUILayout.Button(editNodeTex, GUILayout.MaxWidth(0.15f*backgroundPreviewTex.width)))
            {
                OnEditNodeSelected();
            }
            if (trajectoryTool == TrajectoryToolType.EDIT_NODE)
                GUI.skin = defaultSkin;

            if (trajectoryTool == TrajectoryToolType.EDIT_SIDE)
                GUI.skin = selectedAreaSkin;
            if (GUILayout.Button(editSideTex, GUILayout.MaxWidth(0.15f*backgroundPreviewTex.width)))
            {
                OnEditSideSelected();
            }
            if (trajectoryTool == TrajectoryToolType.EDIT_SIDE)
                GUI.skin = defaultSkin;

            if (trajectoryTool == TrajectoryToolType.INIT_NODE)
                GUI.skin = selectedAreaSkin;
            if (GUILayout.Button(setInitialNodeTex, GUILayout.MaxWidth(0.15f*backgroundPreviewTex.width)))
            {
                OnInitialNodeSelected();
            }
            if (trajectoryTool == TrajectoryToolType.INIT_NODE)
                GUI.skin = defaultSkin;

            if (trajectoryTool == TrajectoryToolType.DELETE_NODE)
                GUI.skin = selectedAreaSkin;
            if (GUILayout.Button(deleteTex, GUILayout.MaxWidth(0.15f*backgroundPreviewTex.width)))
            {
                OnDeleteNodeSelected();
            }
            if (trajectoryTool == TrajectoryToolType.DELETE_NODE)
                GUI.skin = defaultSkin;

            GUILayout.EndHorizontal();
        }
    }

    private void UpdatePlayerRect()
    {
        float scale = sceneRef.getPlayerScale();
        float newX = Mathf.Clamp(currentPos.x, -0.5f*playerTex.width*scale,
            backgroundPreviewTex.width + 0.5f*playerTex.width*scale);
        float newY = Mathf.Clamp(currentPos.y, -0.5f*playerTex.height*scale,
            backgroundPreviewTex.height + 0.5f*playerTex.height*scale);
        sceneRef.setDefaultInitialPosition((int)newX, (int)newY);
    }

    private void OnMovementTypeChange(bool val)
    {
        useInitialPositionLast = val;
        useTrajectory = !val;
        if (useTrajectory)
        {
            trajectory = sceneRef.getTrajectory().GetTrajectory();
            if (trajectory == null)
            {
                trajectory = new Trajectory();
                sceneRef.setTrajectoryDataControl(new TrajectoryDataControl(sceneRef, trajectory));
                sceneRef.setTrajectory(trajectory);
            }
            beginSideIndex = -1;
        }
        else
        {
            sceneRef.setTrajectoryDataControl(new TrajectoryDataControl(sceneRef, null));
            sceneRef.setTrajectory(null);
            beginSideIndex = -1;
        }
    }

    /*
    * Initial pos
    */

    private void OnBeingDragged()
    {
        if (useInitialPosition)
        {
            x = (int) currentPos.x;
            y = (int) currentPos.y;
            sceneRef.setDefaultInitialPosition((int)currentPos.x, (int)currentPos.y);
            Repaint();
            UpdatePlayerRect();
        }
    }


    /*
    * Trajectory
    */

    private void OnBeingDraggedTrajectoryNode(int nodeIndex)
    {
        if (useTrajectory)
        {
            trajectory.getNodes()[nodeIndex].setValues((int)currentPos.x, (int)(currentPos.y + 0.5f* playerRect.height * trajectory.getNodes()[nodeIndex].getScale()), trajectory.getNodes()[nodeIndex].getScale());
            Repaint();
        }
    }

    private void OnBeingTrajectoryNodeRescaled(int nodeIndex, float val)
    {
        trajectory.getNodes()[nodeIndex].setScale(trajectory.getNodes()[nodeIndex].getScale() + val);
        Repaint();
    }


    void OnEditNodeSelected()
    {
        trajectoryTool = TrajectoryToolType.EDIT_NODE;
        beginSideIndex = -1;
    }

    void OnEditSideSelected()
    {
        trajectoryTool = TrajectoryToolType.EDIT_SIDE;
        beginSideIndex = -1;
    }

    void OnInitialNodeSelected()
    {
        trajectoryTool = TrajectoryToolType.INIT_NODE;
        beginSideIndex = -1;
    }

    void OnDeleteNodeSelected()
    {
        trajectoryTool = TrajectoryToolType.DELETE_NODE;
        beginSideIndex = -1;
    }

    void AddNode(Vector2 pos)
    {
        trajectory.addNode(UnityEngine.Random.Range(0, 10000).ToString(), (int)pos.x, (int)pos.y, sceneRef.getPlayerScale());
    }

    void DeleteNode(int i)
    {
        trajectory.removeNode(trajectory.getNodes()[i]);
    }

    void SetInitNode(int i)
    {
        trajectory.setInitial(trajectory.getNodes()[i].getID());
    }

    void SetSideNode(int i)
    {
        if (beginSideIndex == -1)
            beginSideIndex = i;
        else
        {
            trajectory.addSide(trajectory.getNodes()[beginSideIndex].getID(), trajectory.getNodes()[i].getID(), -1);
            beginSideIndex = -1;
        }
    }

    private void DrawLine(Vector2 start, Vector2 end, int width)
    {
        Vector2 d = end - start;
        float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
        if (d.x < 0)
            a += 180;

        int width2 = (int)Mathf.Ceil(width / 2);

        GUIUtility.RotateAroundPivot(a, start);
        GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), lineTex);
        GUIUtility.RotateAroundPivot(-a, start);
    }

    //void Update()
    //{
    //    if (EditorApplication.timeSinceStartup > nextActionTime)
    //    {
    //        nextActionTime = (float)EditorApplication.timeSinceStartup + period;
    //        RecalculatePosition();
    //    }
    //}

    //void RecalculatePosition()
    //{
    //    //TODO:
    //    // wprowadzenie wielu graczy
    //    playerRect = new Rect(currentPos.x - 0.5f* playerTex.width, currentPos.y - 0.5f * playerTex.height, playerTex.width, playerRect.height);
    //}
}

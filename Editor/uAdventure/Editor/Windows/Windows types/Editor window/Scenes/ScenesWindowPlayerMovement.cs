using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ScenesWindowPlayerMovement : SceneEditorWindow
    {
        public enum PlayerMode { NoPlayer, InitialPosition, Trajectory }
        private readonly GUIContent[] tools;
        private SceneDataControl workingScene;
        private static TrajectoryComponent trajectoryComponent;
        private int action = 0;

        public ScenesWindowPlayerMovement(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new PlayerInitialPositionComponent(Rect.zero, new GUIContent(), null);
            new SideComponent(Rect.zero, new GUIContent(), null);
            PreviewTitle = "PlayerMovement.Preview".Traslate();
            trajectoryComponent = new TrajectoryComponent(Rect.zero, new GUIContent(), null)
            {
                Action = -1
            };
            if (Controller.Instance.PlayerMode== DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                // Creating this component registers it in the scene editor
                new InfluenceComponent(Rect.zero, new GUIContent(""), aStyle);
            }

            sceneEditor.TypeEnabling[typeof(Player)] = false;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(SideDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = false;

            tools = new GUIContent[]
            {
                new GUIContent("SceneEditor.PlayerTrajectory.None"), 
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/nodeEdit"), "SceneEditor.PlayerTrajectory.Edit"),
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/sideEdit"), "SceneEditor.PlayerTrajectory.AddSide"),
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/selectStartNode"), "SceneEditor.PlayerTrajectory.StartNode"),
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/deleteTool"), "SceneEditor.PlayerTrajectory.Delete")
            };
        }
        
        protected override void DrawInspector()
        {
            workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex];

            GUILayout.Space(20);

            EditorGUI.BeginChangeCheck();
            var playerMode = GetScenePlayerMode(workingScene);
            var playerModeTexts = new string[] { TC.get("Scene.NoPlayer"), TC.get("Scene.UseInitialPosition"), TC.get("Scene.UseTrajectory") };
            playerMode = (PlayerMode) GUILayout.Toolbar((int) playerMode, playerModeTexts);
            if (EditorGUI.EndChangeCheck())
            {
                OnPlayerModeChange(playerMode);
            }

            switch (playerMode)
            {
                case PlayerMode.NoPlayer: // No Player
                    break;
                case PlayerMode.InitialPosition: // No trajectory
                    componentBasedEditor.SelectedElement = Controller.Instance.SelectedChapterDataControl.getPlayer();
                    break;
                case PlayerMode.Trajectory: // Trajectory
                    {
                        trajectoryComponent.Action = GUILayout.Toolbar(action, tools
                            .Select(t => t.image ? new GUIContent(t.image, t.tooltip.Traslate()) : new GUIContent(t.text.Traslate())).ToArray());
                    }
                    break;
            }
        }

        public static PlayerMode GetScenePlayerMode(SceneDataControl scene)
        {
            if (!scene.isAllowPlayer())
            {
                return PlayerMode.NoPlayer;
            }
            else
            {
                var hasTrajectory = scene.getTrajectory().hasTrajectory();
                return hasTrajectory ? PlayerMode.Trajectory : PlayerMode.InitialPosition;
            }
        }

        public override void Draw(int aID)
        {
            foreach (var elem in componentBasedEditor.Elements)
            {
                componentBasedEditor.TypeEnabling[elem.GetType()] = false;
            }

            componentBasedEditor.TypeEnabling[typeof(PlayerDataControl)] = true;
            componentBasedEditor.TypeEnabling[typeof(TrajectoryDataControl)] = true;
            componentBasedEditor.TypeEnabling[typeof(SideDataControl)] = true;
            componentBasedEditor.TypeEnabling[typeof(NodeDataControl)] = true;

            base.Draw(aID);
            action = trajectoryComponent.Action;
            trajectoryComponent.Action = -1;

            foreach (var elem in componentBasedEditor.Elements)
            {
                componentBasedEditor.TypeEnabling[elem.GetType()] = true;
            }

            componentBasedEditor.TypeEnabling[typeof(PlayerDataControl)] = false;
            componentBasedEditor.TypeEnabling[typeof(TrajectoryDataControl)] = false;
            componentBasedEditor.TypeEnabling[typeof(SideDataControl)] = false;
            componentBasedEditor.TypeEnabling[typeof(NodeDataControl)] = false;
        }


        private void OnPlayerModeChange(PlayerMode val)
        {
            switch (val)
            {
                default:
                    Debug.LogError("Wrong player mode: " + val);
                    break;
                case PlayerMode.NoPlayer:
                    workingScene.changeAllowPlayerLayer(false);
                    componentBasedEditor.SelectedElement = null;
                    break;
                case PlayerMode.InitialPosition:
                    {
                        var trajectoryDataControl = workingScene.getTrajectory();
                        var trajectory = trajectoryDataControl.GetTrajectory();
                        var initialPos = new Vector2(workingScene.getDefaultInitialPositionX(),
                            workingScene.getDefaultInitialPositionY());
                        var initialScale = workingScene.getPlayerScale() >= 0 ? workingScene.getPlayerScale() : workingScene.getPlayerAppropiateScale();
                        if (trajectory != null)
                        {
                            initialPos = new Vector2(trajectory.getInitial().getX(), trajectory.getInitial().getY());
                            initialScale = trajectory.getInitial().getScale();

                            // Swap from any of the selected nodes to the player
                            if (componentBasedEditor.SelectedElement != null && trajectoryDataControl.getNodes()
                                    .Any(componentBasedEditor.SelectedElement.Equals))
                            {
                                componentBasedEditor.SelectedElement =
                                    Controller.Instance.SelectedChapterDataControl.getPlayer();
                            }
                        }
                        workingScene.setTrajectoryDataControl(new TrajectoryDataControl(workingScene, null));
                        workingScene.setTrajectory(null);
                        workingScene.setDefaultInitialPosition((int)initialPos.x, (int)initialPos.y);
                        workingScene.setPlayerScale(initialScale);
                    }
                    break;
                case PlayerMode.Trajectory:
                    {
                        var trajectory = workingScene.getTrajectory().GetTrajectory();
                        if (trajectory == null)
                        {
                            trajectory = new Trajectory();
                            trajectory.addNode("initial", workingScene.getDefaultInitialPositionX(), 
                                workingScene.getDefaultInitialPositionY(), workingScene.getPlayerScale());
                            trajectory.setInitial("initial");
                            var tdc = new TrajectoryDataControl(workingScene, trajectory);
                            workingScene.setTrajectoryDataControl(tdc);
                            workingScene.setTrajectory(trajectory);
                        }


                        // Swap from player to first node
                        if (componentBasedEditor.SelectedElement == Controller.Instance.SelectedChapterDataControl.getPlayer())
                        {
                            componentBasedEditor.SelectedElement = workingScene.getTrajectory().getInitialNode();
                        }
                    }
                    break;
            }

            if (val != PlayerMode.NoPlayer && !workingScene.isAllowPlayer())
            {
                workingScene.changeAllowPlayerLayer(true);
            }
        }

        // ################################################################################################################################################
        // ########################################################### INFLUENCE COMPONENT  ###############################################################
        // ################################################################################################################################################

        [EditorComponent(typeof(ElementReferenceDataControl), typeof(ActiveAreaDataControl), typeof(ExitDataControl), Name = "Influence Area", Order = 1)]
        public class InfluenceComponent : AbstractEditorComponent
        {
            private static InfluenceAreaDataControl getInfluenceArea(DataControl target)
            {
                if (target is ElementReferenceDataControl)
                {
                    var elemRef = target as ElementReferenceDataControl;
                    if (elemRef.getType() == Controller.ATREZZO) return null;
                    return elemRef.getInfluenceArea();
                }
                else if (target is ActiveAreaDataControl)
                {
                    return (target as ActiveAreaDataControl).getInfluenceArea();
                }
                else if (target is ExitDataControl)
                {
                    return (target as ExitDataControl).getInfluenceArea();
                }

                return null;
            }

            private static Rect GetElementBoundaries(DataControl target)
            {

                if (target is ElementReferenceDataControl)
                {
                    var elemRef = target as ElementReferenceDataControl;
                    var size = ScenesWindowElementReference.ReferenceComponent.GetUnscaledRect(target).size * elemRef.getElementScale();
                    return new Rect(elemRef.getElementX() - (size.x / 2f), elemRef.getElementY() - (size.y), size.x, size.y);
                }
                else if (target is RectangleArea)
                {
                    var rectangle = (target as RectangleArea).getRectangle();
                    if (rectangle.isRectangular())
                    {
                        return new Rect(rectangle.getX(), rectangle.getY(), rectangle.getWidth(), rectangle.getHeight());
                    }
                    else
                    {
                        return rectangle.getPoints().ToArray().ToRect();
                    }
                }

                return Rect.zero;
            }

            private static RectInt FixToBoundaries(RectInt rect, Rect boundaries)
            {
                var otherCorner = rect.position + rect.size;

                // This works for the top left corner
                rect.x = Mathf.Min(rect.x, (int) boundaries.width);
                rect.y = Mathf.Min(rect.y, (int) boundaries.height);

                // This works for the bottom right corner
                otherCorner.x = Mathf.Max(otherCorner.x, 0);
                otherCorner.y = Mathf.Max(otherCorner.y, 0);

                var newSize = otherCorner - rect.position;

                return new RectInt(rect.position, newSize);
            }

            public InfluenceComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            public override void Draw(int aID)
            {
                if (GetScenePlayerMode(SceneEditor.Current.Scene) != PlayerMode.Trajectory)
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.HelpBox("Influence areas are only available in trajectory mode.", MessageType.Info);
                    EditorGUI.indentLevel++;
                    return;
                }

                var influence = getInfluenceArea(Target);

                if (influence != null)
                {
                    var boundaries = GetElementBoundaries(Target);

                    EditorGUI.BeginChangeCheck();
                    var rect = influence.ScreenRect(boundaries);
                    rect.position -= boundaries.position;
                    if (!influence.hasInfluenceArea())
                    {
                        rect.position -= new Vector2(20, 20);
                        rect.size = boundaries.size + new Vector2(40, 40);
                    }

                    var newRect = EditorGUILayout.RectIntField("Influence", new RectInt((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
                    if (EditorGUI.EndChangeCheck())
                    {
                        var fixedRect = FixToBoundaries(newRect, boundaries);
                        influence.setInfluenceArea(fixedRect.x, fixedRect.y, fixedRect.width, fixedRect.height);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("This element has no influence area");
                }
            }

            private DataControl wasSelected = null;

            public override bool Update()
            {
                if (GetScenePlayerMode(SceneEditor.Current.Scene) != PlayerMode.Trajectory)
                {
                    return false;
                }

                bool selected = false;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        // Calculate the influenceAreaRect
                        var influenceArea = getInfluenceArea(Target);
                        if (influenceArea == null)
                            return false;

                        var boundaries = GetElementBoundaries(Target);
                        var rect = influenceArea.ScreenRect(boundaries);

                        if (!influenceArea.hasInfluenceArea())
                        {
                            rect.position -= new Vector2(20, 20);
                            rect.size = boundaries.size + new Vector2(40, 40);
                        }

                        rect = rect.AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                        // See if its selected (only if it was previously selected)
                        if (GUIUtility.hotControl == 0)
                            selected = (wasSelected == Target) && (rect.Contains(Event.current.mousePosition) || rect.ToPoints().ToList().FindIndex(p => (p - Event.current.mousePosition).magnitude <= 10f) != -1);

                        if (wasSelected == Target)
                            wasSelected = null;

                        break;
                }

                return selected;
            }

            public override void OnDrawingGizmosSelected()
            {
                if (GetScenePlayerMode(SceneEditor.Current.Scene) != PlayerMode.Trajectory)
                {
                    return;
                }

                wasSelected = Target;
                var influenceArea = getInfluenceArea(Target);
                if (influenceArea == null)
                    return;

                var boundaries = GetElementBoundaries(Target);
                var rect = influenceArea.ScreenRect(boundaries);

                if (!influenceArea.hasInfluenceArea())
                {
                    rect.position -= new Vector2(20, 20);
                    rect.size = boundaries.size + new Vector2(40, 40);
                }
                
                rect = rect.AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                EditorGUI.BeginChangeCheck();
                var newRect = HandleUtil.HandleRect(influenceArea.GetHashCode() + 1, rect, 10f,
                    (polygon, over, active) =>
                    {
                        HandleUtil.DrawPolyLine(polygon, true, Color.blue);
                        HandleUtil.DrawPolygon(polygon, new Color(0, 0, 1f, 0.3f));
                    },
                    (point, over, active) => HandleUtil.DrawSquare(point, 6.5f, Color.yellow, Color.black));

                newRect = HandleUtil.HandleRectMovement(influenceArea.GetHashCode(), newRect);

                if (EditorGUI.EndChangeCheck())
                {
                    var original = newRect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                    original.position -= boundaries.position;
                    var rectFixed = FixToBoundaries(new RectInt((int)original.x, (int)original.y, (int)original.width, (int)original.height), boundaries);
                    // And then we set the values in the reference
                    influenceArea.setInfluenceArea(rectFixed.x, rectFixed.y, rectFixed.width, rectFixed.height);
                }
            }
        }


        // ################################################################################################################################################
        // ########################################################## PLAYER/NODE COMPONENT ###############################################################
        // ################################################################################################################################################

        [EditorComponent(typeof(PlayerDataControl), typeof(NodeDataControl), Name = "Initial position", Order = 0)]
        public class PlayerInitialPositionComponent : AbstractEditorComponent
        {
            public PlayerInitialPositionComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            public override void Draw(int aID)
            {
                if(Target is PlayerDataControl)
                {
                    var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex];

                    EditorGUI.BeginChangeCheck();
                    var newPos = EditorGUILayout.Vector2Field("Position", new Vector2(workingScene.getDefaultInitialPositionX(), workingScene.getDefaultInitialPositionY()));
                    if (EditorGUI.EndChangeCheck()) workingScene.setDefaultInitialPosition(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y));

                    EditorGUI.BeginChangeCheck();
                    var newScale = Mathf.Max(0, EditorGUILayout.FloatField("Scale", workingScene.getPlayerScale()));
                    if (EditorGUI.EndChangeCheck()) workingScene.setPlayerScale(newScale);
                }
                else if(Target is NodeDataControl)
                {
                    var target = Target as NodeDataControl;

                    EditorGUI.BeginChangeCheck();
                    var newPos = EditorGUILayout.Vector2Field("Position", new Vector2(target.getX(), target.getY()));
                    if (EditorGUI.EndChangeCheck()) target.setNode(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), target.getScale());

                    EditorGUI.BeginChangeCheck();
                    var newScale = Mathf.Max(0, EditorGUILayout.FloatField("Scale", target.getScale()));
                    if (EditorGUI.EndChangeCheck()) target.setNode(target.getX(), target.getY(), newScale);
                }

            }
            public static void PutTransform(DataControl target)
            {
                SceneEditor.Current.PushMatrix();
                var matrix = SceneEditor.Current.Matrix;
                if (target is PlayerDataControl)
                {
                    var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
                    SceneEditor.Current.Matrix = matrix * Matrix4x4.TRS(new Vector3(workingScene.getDefaultInitialPositionX(), workingScene.getDefaultInitialPositionY(), 0), Quaternion.identity, Vector3.one * workingScene.getPlayerScale());
                }
                else if (target is NodeDataControl)
                {
                    var node = target as NodeDataControl;
                    SceneEditor.Current.Matrix = matrix * Matrix4x4.TRS(new Vector3(node.getX(), node.getY(), 0), Quaternion.identity, Vector3.one * node.getScale());
                }
            }

            public static void RemoveTransform(DataControl target)
            {
                SceneEditor.Current.PopMatrix();
            }

            public override void OnPreRender()
            {
                PutTransform(Target);
            }

            public override bool Update()
            {
                bool selected = false;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        var rect = ScenesWindowElementReference.ReferenceComponent.GetElementRect(Target);
                        if (GUIUtility.hotControl == 0)
                            selected = rect.Contains(Event.current.mousePosition) || (rect.ToPoints().ToList().FindIndex(p => (p - Event.current.mousePosition).magnitude <= 10f) != -1);
                        break;
                }

                return selected;
            }

            public override void OnDrawingGizmosSelected()
            {
                var rect = ScenesWindowElementReference.ReferenceComponent.GetElementRect(Target);

                // Rect resizing
                var id = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                EditorGUI.BeginChangeCheck();
                var newRect = HandleUtil.HandleFixedRatioRect(id, rect, rect.width / rect.height, 10f, 
                    (polygon, over, active) => HandleUtil.DrawPolyLine(polygon, true, SceneEditor.GetColor(Color.red)),
                    (point, over, active) =>   HandleUtil.DrawPoint(point, 4.5f, SceneEditor.GetColor(Color.blue), SceneEditor.GetColor(Color.black)));
                if (EditorGUI.EndChangeCheck())
                {
                    var original = newRect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);
                    var unscaled = ScenesWindowElementReference.ReferenceComponent.GetUnscaledRect(Target);
                    // And then we rip the position
                    var position = original.center + new Vector2(0, original.height / 2f);
                    var scale = original.size.magnitude / unscaled.size.magnitude;

                    if(Target is PlayerDataControl)
                    {
                        var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                            GameRources.GetInstance().selectedSceneIndex];
                        // And then we set the values in the reference
                        workingScene.setDefaultInitialPosition(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
                        workingScene.setPlayerScale(scale);
                    }
                    else if(Target is NodeDataControl)
                    {
                        var node = Target as NodeDataControl;
                        node.setNode(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), scale);
                    }
                }

                // Rect movement
                var movementId = GUIUtility.GetControlID(GetHashCode() + 1, FocusType.Passive);
                EditorGUI.BeginChangeCheck();
                rect = HandleUtil.HandleRectMovement(movementId, rect);
                if (EditorGUI.EndChangeCheck())
                {
                    var original = rect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);
                    var rectBase = original.Base();
                    if (Target is PlayerDataControl)
                    {
                        var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
                        workingScene.setDefaultInitialPosition(Mathf.RoundToInt(rectBase.x), Mathf.RoundToInt(rectBase.y));
                    }
                    else if (Target is NodeDataControl)
                    {
                        var node = Target as NodeDataControl;
                        node.setNode(Mathf.RoundToInt(rectBase.x), Mathf.RoundToInt(rectBase.y), node.getScale());
                    }
                }
            }
            
            protected Rect GetViewportRect(Rect rect, Rect viewport)
            {
                var myPos = SceneEditor.Current.Matrix.MultiplyPoint(rect.position);
                var mySize = SceneEditor.Current.Matrix.MultiplyVector(rect.size);
                return new Rect(myPos, mySize).AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, viewport);
            }
            public override void OnPostRender()
            {
                RemoveTransform(Target);
            }
        }

        // ################################################################################################################################################
        // ############################################################## SIDE COMPONENT ##################################################################
        // ################################################################################################################################################
        [EditorComponent(typeof(SideDataControl), Name = "Side", Order = 0)]
        public class SideComponent : AbstractEditorComponent
        {
            private readonly GUIContent lengthContent;
            public SideComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                lengthContent = new GUIContent();
            }

            public override void Draw(int aID) {}

            private Rect[] GetEditingRects(SideDataControl side)
            {
                var p1 = GetPivot(side.getStart());
                var p2 = GetPivot(side.getEnd());
                
                lengthContent.text = side.getLength().ToString();
                Rect rect = new Rect();
                rect.size = GUI.skin.textField.CalcSize(lengthContent);
                rect.center = (p1 + p2) / 2f;

                var deleteRect = rect;
                deleteRect.position += new Vector2(deleteRect.size.x, 0);
                deleteRect.size = new Vector2(deleteRect.size.y, deleteRect.size.y);

                return new Rect[] { rect, deleteRect };
            }

            public override bool Update()
            {
                var side = Target as SideDataControl;
                if (Event.current.type == EventType.MouseDown)
                {
                    if (SceneEditor.Current.SelectedElement == Target)
                    {
                        var rects = GetEditingRects(side);
                        if (rects.Any(r => r.Contains(Event.current.mousePosition)))
                        {
                            return true;
                        }
                    }
                    var selected = DistanceToPoint(side, Event.current.mousePosition) < 8;
                    if(SceneEditor.Current.SelectedElement != Target && selected)
                    {
                        GUIUtility.hotControl = this.GetHashCode();
                        GUIUtility.keyboardControl = this.GetHashCode();
                    }

                    return selected;
                }
                return false;
            }

            public override void OnRender()
            {
                var side = Target as SideDataControl;
                var p1 = GetPivot(side.getStart());
                var p2 = GetPivot(side.getEnd());

                HandleUtil.DrawPolyLine(new Vector2[] { p1, p2 }, false, SceneEditor.GetColor(Color.black), 5f);
                HandleUtil.DrawPolyLine(new Vector2[] { p1, p2 }, false, SceneEditor.GetColor(Color.white), 3f);

                var bcColor = GUI.color;
                if (side.getLength() != side.getRealLength())
                {
                    GUI.color = SceneEditor.GetColor(Color.yellow);
                }
                EditorGUI.DropShadowLabel(new Rect(((p1 + p2) / 2f) - new Vector2(100f, 25f), new Vector2(200, 30)), new GUIContent(Mathf.RoundToInt(side.getLength()).ToString()));
                GUI.color = bcColor;
            }

            public override void OnDrawingGizmosSelected()
            {
                var side = Target as SideDataControl;
                var rects = GetEditingRects(side);

                EditorGUI.BeginChangeCheck();
                var newlength = EditorGUI.FloatField(rects[0], side.getLength());
                if (EditorGUI.EndChangeCheck())
                {
                    side.setLength(newlength);
                }

                if (side.getLength() != side.getRealLength() && GUI.Button(rects[1], "X"))
                {
                    side.setLength(side.getRealLength());
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
                if (GUIUtility.hotControl == this.GetHashCode())
                {
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
            }
        }

        // ################################################################################################################################################
        // ########################################################### TRAJECTORY COMPONENT ###############################################################
        // ################################################################################################################################################
        [EditorComponent(typeof(TrajectoryDataControl), Name = "Trajectory", Order = 0)]
        public class TrajectoryComponent : AbstractEditorComponent
        {
            private int action = 0;

            private NodeDataControl pairing = null;

            private TrajectoryDataControl previousTrajectoryDataControl;
            public int Action
            {
                get { return action; }
                set
                {
                    if (action == value)
                    {
                        return;
                    }

                    action = value;
                    if (action != 2 && action >= 0)
                    {
                        pairing = null;
                    }
                }
            }

            public TrajectoryComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            public override void OnDrawingGizmos()
            {
                var trajectory = Target as TrajectoryDataControl;
                if (trajectory == null)
                {
                    return;
                }

                if (SceneEditor.Current.Disabled && pairing != null)
                {
                    pairing = null;
                }

                if (previousTrajectoryDataControl != trajectory)
                {
                    previousTrajectoryDataControl = trajectory;
                    pairing = null;
                }

                if (Event.current.type == EventType.MouseDown)
                {
                    var selected = SceneEditor.Current.SelectedElement;
                    var node = selected as NodeDataControl;
                    var side = selected as SideDataControl;

                    var isNode = node != null && trajectory.getNodes().Contains(node);
                    var isSide = side != null && trajectory.getSides().Contains(side);

                    switch (Action)
                    {
                        // Moving
                        case 1:
                            if (SceneEditor.Current.SelectedElement == null)
                            {
                                var pos = (Event.current.mousePosition - SceneEditor.Current.Viewport.position);
                                pos.x = (pos.x / SceneEditor.Current.Viewport.size.x) * SceneEditor.Current.Size.x;
                                pos.y = (pos.y / SceneEditor.Current.Viewport.size.y) * SceneEditor.Current.Size.y;
                                var initialNode = trajectory.getInitialNode();
                                if (trajectory.addNode(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)))
                                {
                                    var addedNode = trajectory.getLastNode();
                                    var trajectoryNode = (Trajectory.Node)addedNode.getContent();
                                    if(initialNode != null)
                                    {
                                        trajectoryNode.setScale(initialNode.getScale());
                                    }
                                }
                            }
                            break;

                        // Pariring
                        case 2:
                            if (isNode)
                            {
                                if (pairing == null)
                                {
                                    pairing = node;
                                } 
                                else
                                {
                                    var duplicated = trajectory.getSides().Find(s => IsPairingStartOrEnd(s, node)) != null;
                                    if (!duplicated)
                                    {
                                        trajectory.addSide(pairing, node);
                                    }
                                    pairing = null;
                                }
                            }
                            else
                            {
                                pairing = null;
                            }
                            break;

                        // Initial
                        case 3:
                            if (isNode)
                            {
                                trajectory.setInitialNode(node);
                            }
                            break;

                        // Deleting
                        case 4:
                            if ((isNode || isSide) && trajectory.deleteElement(selected, false))
                            {
                                SceneEditor.Current.SelectedElement = null;
                            }
                            break;
                    }
                }
                
                foreach (var node in trajectory.getNodes())
                {
                    HandleUtil.DrawPoint(GetPivot(node), 10f, SceneEditor.GetColor(node.isInitial() ? Color.red : Color.blue), SceneEditor.GetColor(Color.black));
                }

                if (pairing != null)
                {
                    HandleUtil.DrawPolyLine(new Vector2[] { GetPivot(pairing), Event.current.mousePosition }, false, SceneEditor.GetColor(Color.white), 3f);
                }
            }

            private bool IsPairingStartOrEnd(SideDataControl s, NodeDataControl node)
            {
                return (s.getStart() == pairing && s.getEnd() == node) || (s.getEnd() == pairing && s.getStart() == node);
            }

            public override void Draw(int aID) {}

            
        }

        private static float DistanceToPoint(SideDataControl s, Vector2 point)
        {
            var p1 = GetPivot(s.getStart());
            var p2 = GetPivot(s.getEnd());

            return HandleUtility.DistancePointToLineSegment(point, p1, p2);
        }

        private static Vector2 GetPivot(DataControl dataControl)
        {
            PlayerInitialPositionComponent.PutTransform(dataControl);
            var rect = ScenesWindowElementReference.ReferenceComponent.GetElementRect(dataControl);
            PlayerInitialPositionComponent.RemoveTransform(dataControl);

            return rect.center + new Vector2(0, rect.height / 2f);
        }
    }

    internal static class ExInfluences
    {
        public static Rect ScreenRect(this InfluenceAreaDataControl influence, Rect boundaries)
        {
            return new Rect(boundaries.x + influence.getX(), boundaries.y + influence.getY(), influence.getWidth(), influence.getHeight());
        }
    }
}
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
        private GUIContent[] tools;
        private static Rect previewRect;
        private SceneDataControl workingScene;
        private static TrajectoryComponent trajectoryComponent;
        private static InfluenceComponent influenceComponent;
        private int action = 0;

        public ScenesWindowPlayerMovement(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new PlayerInitialPositionComponent(Rect.zero, new GUIContent(), null);
            new SideComponent(Rect.zero, new GUIContent(), null);
            trajectoryComponent = new TrajectoryComponent(Rect.zero, new GUIContent(), null)
            {
                Action = -1
            };
            if (Controller.Instance.playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                influenceComponent = new InfluenceComponent(Rect.zero, new GUIContent(""), aStyle);
            }

            sceneEditor.TypeEnabling[typeof(Player)] = false;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(SideDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = false;

            tools = new GUIContent[]
            {
                new GUIContent("None"), // TODO language
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/nodeEdit")),
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/sideEdit")),
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/selectStartNode")),
                new GUIContent(Resources.Load<Texture2D>("EAdventureData/img/icons/deleteTool"))
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
            if (EditorGUI.EndChangeCheck()) OnPlayerModeChange(playerMode);

            switch (playerMode)
            {
                case PlayerMode.NoPlayer: // No Player
                    {
                    }
                    break;
                case PlayerMode.InitialPosition: // No trajectory
                    {

                    }
                    break;
                case PlayerMode.Trajectory: // Trajectory
                    {
                        trajectoryComponent.Action = GUILayout.Toolbar(action, tools);
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
            foreach (var elem in sceneEditor.Elements)
                sceneEditor.TypeEnabling[elem.GetType()] = false;

            sceneEditor.TypeEnabling[typeof(PlayerDataControl)] = true;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = true;
            sceneEditor.TypeEnabling[typeof(SideDataControl)] = true;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = true;

            base.Draw(aID);
            action = trajectoryComponent.Action;
            trajectoryComponent.Action = -1;

            foreach (var elem in sceneEditor.Elements)
                sceneEditor.TypeEnabling[elem.GetType()] = true;

            sceneEditor.TypeEnabling[typeof(PlayerDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(SideDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = false;
        }


        private void OnPlayerModeChange(PlayerMode val)
        {
            switch (val)
            {
                default:
                case PlayerMode.NoPlayer:
                    workingScene.changeAllowPlayerLayer(false);
                    break;
                case PlayerMode.InitialPosition:
                    {
                        var trajectory = workingScene.getTrajectory().GetTrajectory();
                        var initialPos = new Vector2(trajectory.getInitial().getX(), trajectory.getInitial().getY());
                        var initialScale = trajectory.getInitial().getScale();
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
                            trajectory.addNode("initial", workingScene.getDefaultInitialPositionX(), workingScene.getDefaultInitialPositionY(), workingScene.getPlayerScale());
                            trajectory.setInitial("initial");
                            var tdc = new TrajectoryDataControl(workingScene, trajectory);
                            workingScene.setTrajectoryDataControl(tdc);
                            workingScene.setTrajectory(trajectory);
                        }
                    }
                    break;
            }

            if(val != PlayerMode.NoPlayer && !workingScene.isAllowPlayer())
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
            private static InfluenceAreaDataControl getIngluenceArea(DataControl target)
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

            private static Rect getElementBoundaries(DataControl target)
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

            private static RectInt fixToBoundaries(Vector2 oldSize, RectInt rect, Rect boundaries)
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

                var influence = getIngluenceArea(Target);

                if (influence != null)
                {
                    var boundaries = getElementBoundaries(Target);

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
                        var fixedRect = fixToBoundaries(rect.size, newRect, boundaries);
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
                        var influenceArea = getIngluenceArea(Target);
                        if (influenceArea == null)
                            return false;
                        var boundaries = getElementBoundaries(Target);

                        var rect = influenceArea.ScreenRect(boundaries)
                            .AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

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
                var influenceArea = getIngluenceArea(Target);
                if (influenceArea == null)
                    return;

                var boundaries = getElementBoundaries(Target);
                var rect = influenceArea.ScreenRect(boundaries);

                if (!influenceArea.hasInfluenceArea())
                {
                    rect.position -= new Vector2(20, 20);
                    rect.size = boundaries.size + new Vector2(40, 40);
                }

                var originalSize = rect.size;
                rect = rect.AdjustToViewport(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                EditorGUI.BeginChangeCheck();
                var newRect = HandleUtil.HandleRect(influenceArea.GetHashCode() + 1, rect, 10f,
                    polygon =>
                    {
                        HandleUtil.DrawPolyLine(polygon, true, Color.blue);
                        HandleUtil.DrawPolygon(polygon, new Color(0, 0, 1f, 0.3f));
                    },
                    point => HandleUtil.DrawSquare(point, 6.5f, Color.yellow, Color.black));

                newRect = HandleUtil.HandleRectMovement(influenceArea.GetHashCode(), newRect);

                if (EditorGUI.EndChangeCheck())
                {
                    var original = newRect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);

                    original.position -= boundaries.position;
                    var rectFixed = fixToBoundaries(originalSize, new RectInt((int)original.x, (int)original.y, (int)original.width, (int)original.height), boundaries);
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
                    var target = Target as PlayerDataControl;
                    var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex];

                    EditorGUI.BeginChangeCheck();
                    var newPos = EditorGUILayout.Vector2Field("Position", new Vector2(workingScene.getDefaultInitialPositionX(), workingScene.getDefaultInitialPositionY()));
                    if (EditorGUI.EndChangeCheck()) workingScene.setDefaultInitialPosition(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y));

                    EditorGUI.BeginChangeCheck();
                    var newScale = EditorGUILayout.FloatField("Scale", workingScene.getPlayerScale());
                    if (EditorGUI.EndChangeCheck()) workingScene.setPlayerScale(newScale);
                }
                else if(Target is NodeDataControl)
                {
                    var target = Target as NodeDataControl;

                    EditorGUI.BeginChangeCheck();
                    var newPos = EditorGUILayout.Vector2Field("Position", new Vector2(target.getX(), target.getY()));
                    if (EditorGUI.EndChangeCheck()) target.setNode(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), target.getScale());

                    EditorGUI.BeginChangeCheck();
                    var newScale = EditorGUILayout.FloatField("Scale", target.getScale());
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
                EditorGUI.BeginChangeCheck();
                var newRect = HandleUtil.HandleFixedRatioRect(Target.GetHashCode() + 1, rect, rect.width / rect.height, 10f, 
                    polygon => HandleUtil.DrawPolyLine(polygon, true, SceneEditor.GetColor(Color.red)),
                    point =>   HandleUtil.DrawPoint(point, 4.5f, SceneEditor.GetColor(Color.blue), SceneEditor.GetColor(Color.black)));
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
                EditorGUI.BeginChangeCheck();
                rect = HandleUtil.HandleRectMovement(Target.GetHashCode(), rect);
                if (EditorGUI.EndChangeCheck())
                {
                    var original = rect.ViewportToScreen(SceneEditor.Current.Size.x, SceneEditor.Current.Size.y, SceneEditor.Current.Viewport);
                    if (Target is PlayerDataControl)
                    {
                        var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
                        workingScene.setDefaultInitialPosition(Mathf.RoundToInt(original.x + 0.5f * original.width), Mathf.RoundToInt(original.y + original.height));
                    }
                    else if (Target is NodeDataControl)
                    {
                        var node = Target as NodeDataControl;
                        node.setNode(Mathf.RoundToInt(original.x + 0.5f * original.width), Mathf.RoundToInt(original.y + original.height), node.getScale());
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
            private GUIContent lengthContent;
            public SideComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                lengthContent = new GUIContent();
            }

            public override void Draw(int aID) {}

            private Rect[] getEditingRects(SideDataControl side)
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
                        var rects = getEditingRects(side);
                        if (rects.Any(r => r.Contains(Event.current.mousePosition)))
                            return true;
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

            public override void OnRender(Rect viewport)
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
                var rects = getEditingRects(side);
                var p1 = GetPivot(side.getStart());
                var p2 = GetPivot(side.getEnd());

                EditorGUI.BeginChangeCheck();
                var newlength = EditorGUI.FloatField(rects[0], side.getLength());
                if (EditorGUI.EndChangeCheck())
                {
                    side.setLength(newlength);
                }

                if (side.getLength() != side.getRealLength())
                {
                    if (GUI.Button(rects[1], "X"))
                    {
                        side.setLength(side.getRealLength());
                        GUIUtility.hotControl = 0;
                        GUIUtility.keyboardControl = 0;
                    }
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
            public int Action { get; set; }

            public TrajectoryComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            private NodeDataControl pairing = null;

            public override void OnDrawingGizmos()
            {
                var trajectory = Target as TrajectoryDataControl;

                if (Event.current.type == EventType.MouseDown)
                {
                    DataControl selected = SceneEditor.Current.SelectedElement;
                    NodeDataControl node = selected as NodeDataControl;
                    SideDataControl side = selected as SideDataControl;

                    bool isNode = node != null && trajectory.getNodes().Contains(node);
                    bool isSide = side != null && trajectory.getSides().Contains(side);

                    switch (Action)
                    {
                        // Moving
                        case 1:
                            if (SceneEditor.Current.SelectedElement == null)
                            {
                                var pos = (Event.current.mousePosition - SceneEditor.Current.Viewport.position);
                                pos.x = (pos.x / SceneEditor.Current.Viewport.size.x) * SceneEditor.Current.Size.x;
                                pos.y = (pos.y / SceneEditor.Current.Viewport.size.y) * SceneEditor.Current.Size.y;
                                trajectory.addNode(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
                            }
                            else if (isSide)
                            {

                            }
                            break;

                        // Pariring
                        case 2:
                            if (isNode)
                            {
                                if (pairing == null) pairing = node;
                                else
                                {
                                    var duplicated = trajectory.getSides().Find(s => (s.getStart() == pairing && s.getEnd() == node) || (s.getEnd() == pairing && s.getStart() == node)) != null;
                                    if (!duplicated) trajectory.addSide(pairing, node);
                                    pairing = null;
                                }
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
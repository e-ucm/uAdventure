using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ScenesWindowPlayerMovement : SceneEditorWindow
    {
        private GUIContent[] tools;
        private static Rect previewRect;
        private SceneDataControl workingScene;
        private static TrajectoryComponent trajectoryComponent;
        private int action = 0;

        public ScenesWindowPlayerMovement(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new PlayerInitialPositionComponent(Rect.zero, new GUIContent(), null);
            trajectoryComponent = new TrajectoryComponent(Rect.zero, new GUIContent(), null);
            trajectoryComponent.Action = -1;

            sceneEditor.TypeEnabling[typeof(Player)] = false;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = false;

            tools = new GUIContent[]
            {
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
            var hasTrajectory = GUILayout.Toolbar(workingScene.getTrajectory().hasTrajectory() ? 1 : 0, new string[] { TC.get("Scene.UseInitialPosition"), TC.get("Scene.UseTrajectory") });
            if (EditorGUI.EndChangeCheck()) OnMovementTypeChange(hasTrajectory == 1);

            switch (hasTrajectory)
            {
                case 0: // No trajectory
                    {
                    }
                    break;
                case 1: // Trajectory
                    {
                        trajectoryComponent.Action = GUILayout.Toolbar(action, tools);
                    }
                    break;
            }
        }

        public override void Draw(int aID)
        {
            sceneEditor.TypeEnabling[typeof(Player)] = true;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = true;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = true;

            base.Draw(aID);
            action = trajectoryComponent.Action;
            trajectoryComponent.Action = -1;

            sceneEditor.TypeEnabling[typeof(Player)] = false;
            sceneEditor.TypeEnabling[typeof(TrajectoryDataControl)] = false;
            sceneEditor.TypeEnabling[typeof(NodeDataControl)] = false;
        }


        private void OnMovementTypeChange(bool val)
        {
            if (val)
            {
                var trajectory = workingScene.getTrajectory().GetTrajectory();
                if (trajectory == null)
                {
                    trajectory = new Trajectory();
                    var tdc = new TrajectoryDataControl(workingScene, trajectory);
                    tdc.addNode(0,0);
                    tdc.getLastNode().setNode(workingScene.getDefaultInitialPositionX(), workingScene.getDefaultInitialPositionY(), workingScene.getPlayerScale());
                    workingScene.setTrajectoryDataControl(tdc);
                    workingScene.setTrajectory(trajectory);
                }
            }
            else
            {
                workingScene.setTrajectoryDataControl(new TrajectoryDataControl(workingScene, null));
                workingScene.setTrajectory(null);
            }
        }

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


        [EditorComponent(typeof(TrajectoryDataControl), Name = "Trajectory", Order = 0)]
        public class TrajectoryComponent : AbstractEditorComponent
        {
            public int Action { get; set; }

            public TrajectoryComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            private NodeDataControl pairing = null;

            public override bool Update()
            {
                if(Event.current.type == EventType.MouseDown)
                {
                    var trajectory = Target as TrajectoryDataControl;
                    var selected = SceneEditor.Current.SelectedElement as NodeDataControl;
                    if (selected != null && trajectory.getNodes().Contains(selected))
                    {
                        switch (Action)
                        {
                            case 1:
                                if (pairing == null) pairing = selected;
                                else
                                {
                                    var duplicated = trajectory.getSides().Find(s => (s.getStart() == pairing && s.getEnd() == selected) || (s.getEnd() == pairing && s.getStart() == selected)) != null;
                                    if (!duplicated) trajectory.addSide(pairing, selected);
                                    pairing = null;
                                }

                                break;

                            case 2:
                                trajectory.setInitialNode(selected);
                                break;
                            case 3:
                                if(trajectory.deleteElement(selected, false))
                                    SceneEditor.Current.SelectedElement = null;
                                break;
                        }
                    }
                    else if(SceneEditor.Current.SelectedElement == null && Action == 0)
                    {
                        var pos = (Event.current.mousePosition - SceneEditor.Current.Viewport.position);
                        pos.x = (pos.x / SceneEditor.Current.Viewport.size.x) * SceneEditor.Current.Size.x;
                        pos.y = (pos.y / SceneEditor.Current.Viewport.size.y) * SceneEditor.Current.Size.y;
                        trajectory.addNode(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
                    }
                }

                return false;
            }

            public override void Draw(int aID) {}

            private static Vector2 GetPivot(DataControl dataControl)
            {
                PlayerInitialPositionComponent.PutTransform(dataControl);
                var rect = ScenesWindowElementReference.ReferenceComponent.GetElementRect(dataControl);
                PlayerInitialPositionComponent.RemoveTransform(dataControl);

                return rect.center + new Vector2(0, rect.height / 2f);
            }

            public override void OnDrawingGizmos()
            {
                var trajectory = Target as TrajectoryDataControl;
                foreach(var node in trajectory.getNodes())
                {    
                    HandleUtil.DrawPoint(GetPivot(node), 10f, SceneEditor.GetColor(node.isInitial() ? Color.red : Color.blue), SceneEditor.GetColor(Color.black));
                }

                foreach(var connection in trajectory.getSides())
                {
                    var p1 = GetPivot(connection.getStart());
                    var p2 = GetPivot(connection.getEnd());
                    var distance = (p1 - p2).magnitude;

                    HandleUtil.DrawPolyLine(new Vector2[] { p1, p2 }, false, SceneEditor.GetColor(Color.black), 5f);
                    HandleUtil.DrawPolyLine(new Vector2[] { p1, p2 }, false, SceneEditor.GetColor(Color.white), 3f);
                    
                    EditorGUI.DropShadowLabel(new Rect(((p1 + p2) / 2f) - new Vector2(100f, 25f), new Vector2(200, 30)), new GUIContent(Mathf.RoundToInt(distance) + ""));
                }

                if(pairing != null)
                {
                    HandleUtil.DrawPolyLine(new Vector2[] { GetPivot(pairing), Event.current.mousePosition }, false, SceneEditor.GetColor(Color.white), 3f);
                }
            }
        }
    }
}
using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using System;

namespace uAdventure.Editor
{
    public class ScenesWindowPlayerMovement : SceneEditorWindow
    {

        private string backgroundPath = "";
        private Texture2D backgroundPreviewTex = null;
        private GUIContent[] tools;
        private static Rect previewRect;
        private SceneDataControl workingScene;
        private static TrajectoryComponent trajectoryComponent;

        public ScenesWindowPlayerMovement(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            new PlayerInitialPositionComponent(Rect.zero, new GUIContent(), null);
            trajectoryComponent = new TrajectoryComponent(Rect.zero, new GUIContent(), null);

            if (GameRources.GetInstance().selectedSceneIndex >= 0)
                backgroundPath =
                    Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();
            if (backgroundPath != null && !backgroundPath.Equals(""))
                backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

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
                        trajectoryComponent.Action = GUILayout.Toolbar(trajectoryComponent.Action, tools);
                    }
                    break;
            }

            GUI.DrawTexture(previewRect, backgroundPreviewTex, ScaleMode.ScaleToFit);
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
            public override void OnPreRender()
            {
                SceneEditor.Current.PushMatrix();
                var matrix = SceneEditor.Current.Matrix;
                if(Target is PlayerDataControl)
                {
                    var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
                    SceneEditor.Current.Matrix = matrix * Matrix4x4.TRS(new Vector3(workingScene.getDefaultInitialPositionX(), workingScene.getDefaultInitialPositionY(), 0), Quaternion.identity, Vector3.one * workingScene.getPlayerScale());
                }
                else if(Target is NodeDataControl)
                {
                    var node = Target as NodeDataControl;
                    SceneEditor.Current.Matrix = matrix * Matrix4x4.TRS(new Vector3(node.getX(), node.getY(), 0), Quaternion.identity, Vector3.one * node.getScale());
                }
            }

            public override bool Update()
            {
                bool selected = false;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        var rect = ScenesWindowElementReference.ReferenceComponent.GetElementRect(Target);
                        if (rect.Contains(Event.current.mousePosition) && GUIUtility.hotControl == 0) selected = true;
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
                    polygon => HandleUtil.DrawPolyLine(polygon, true, Color.red),
                    point =>   HandleUtil.DrawPoint(point, 4.5f, Color.blue, Color.black));
                if (EditorGUI.EndChangeCheck())
                {
                    var original = newRect.ViewportToScreen(800f, 600f, SceneEditor.Current.Viewport);
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
                    var original = rect.ViewportToScreen(800f, 600f, SceneEditor.Current.Viewport);
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
                return new Rect(myPos, mySize).AdjustToViewport(800, 600, viewport);
            }
            public override void OnPostRender()
            {
                SceneEditor.Current.PopMatrix();
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
                                trajectory.deleteElement(selected, false);
                                break;
                        }
                    }
                    else if(SceneEditor.Current.SelectedElement == null && Action == 0)
                    {
                        var pos = (Event.current.mousePosition - SceneEditor.Current.Viewport.position);
                        pos.x = (pos.x / SceneEditor.Current.Viewport.size.x) * 800f;
                        pos.y = (pos.y / SceneEditor.Current.Viewport.size.y) * 600f;
                        trajectory.addNode(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
                    }
                }

                return false;
            }

            public override void Draw(int aID) {}

            public override void OnDrawingGizmos()
            {
                var trajectory = Target as TrajectoryDataControl;
                foreach(var connection in trajectory.getSides())
                {
                    var p1 = ScenesWindowElementReference.ReferenceComponent.GetElementRect(connection.getStart());
                    var p2 = ScenesWindowElementReference.ReferenceComponent.GetElementRect(connection.getStart());

                    HandleUtil.DrawPolyLine(new Vector2[] { p1.center, p2.center }, false, Color.white);

                    var distance = new GUIContent(Mathf.RoundToInt((p1.center - p2.center).magnitude) + "");
                    
                    EditorGUI.DropShadowLabel(new Rect((p1.center + p2.center / 2f), new Vector2(200, 30)), distance);
                }
            }
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using UnityEditor;
using System.Linq;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Core.Geometry;
using UniRx;

namespace uAdventure.Editor
{
    [EditorWindowExtension(10, typeof(SceneDataControl))]
    public class ScenesWindow : TabsEditorWindowExtension
    {
        private enum ScenesWindowType
        {
            ActiveAreas,
            Appearance,
            Documentation,
            ElementReference,
            Exits,
            Barriers,
            PlayerMovement
        }

        private static ChapterPreview chapterPreview;

        private SceneEditor sceneEditor;

        DataControlList sceneList;

        private void CreateSceneEditorTab<T>(Rect rect, GUIContent title, GUIStyle style, Enum identifier, SceneEditor sceneEditor) where T : LayoutWindow
        {
            var sceneEditorTab = (T)Activator.CreateInstance(typeof(T), rect, title, style, sceneEditor, new GUILayoutOption[0]);
            sceneEditorTab.OnRequestRepaint = () => Repaint();
            AddTab(title.text, identifier, sceneEditorTab);
        }

        public ScenesWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, new GUIContent(TC.get("Element.Name1")), style, options)
        {
            new RectangleComponentEditor(Rect.zero, new GUIContent(""), style);

            // Button
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/scenes"),
                text = "Element.Name1"
            };

            sceneEditor = new SceneEditor();
            sceneEditor.onSelectElement += (element) =>
            {
                if (sceneEditor.SelectedElement == null)
                {
                    return;
                }

                switch (sceneEditor.SelectedElement.ToString())
                {
                    case "uAdventure.Editor.ExitDataControl": this.OpenedWindow = ScenesWindowType.Exits; break;
                    case "uAdventure.Editor.ElementReferenceDataControl": this.OpenedWindow = ScenesWindowType.ElementReference; break;
                    case "uAdventure.Editor.BarrierDataControl": this.OpenedWindow = ScenesWindowType.Barriers; break;
                    case "uAdventure.Editor.ActiveAreaDataControl": this.OpenedWindow = ScenesWindowType.ActiveAreas; break;
                }
            };

            // Chapter preview subwindow
            chapterPreview = new ChapterPreview(rect, new GUIContent(""), "Window")
            {
                OnRequestRepaint = () => Repaint(),
                BeginWindows = () => BeginWindows(),
                EndWindows = () => EndWindows()
            };
            chapterPreview.OnSelectElement += (scene) =>
            {
                var index = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes().FindIndex(s => s == scene as SceneDataControl);
                ShowItemWindowView(index);
            };

            // Windows
            CreateSceneEditorTab<ScenesWindowAppearance>(rect, new GUIContent(TC.get("Scene.LookPanelTitle")), "Window", ScenesWindowType.Appearance, sceneEditor);
            CreateSceneEditorTab<ScenesWindowDocumentation>(rect, new GUIContent(TC.get("Scene.DocPanelTitle")), "Window", ScenesWindowType.Documentation, sceneEditor);
            CreateSceneEditorTab<ScenesWindowElementReference>(rect, new GUIContent(TC.get("ItemReferencesList.Title")), "Window", ScenesWindowType.ElementReference, sceneEditor);
            CreateSceneEditorTab<ScenesWindowActiveAreas>(rect, new GUIContent(TC.get("ActiveAreasList.Title")), "Window", ScenesWindowType.ActiveAreas, sceneEditor);
            CreateSceneEditorTab<ScenesWindowExits>(rect, new GUIContent(TC.get("ExitsList.Title")), "Window", ScenesWindowType.Exits, sceneEditor);

            if (Controller.Instance.PlayerMode == DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                CreateSceneEditorTab<ScenesWindowBarriers>(rect, new GUIContent(TC.get("BarriersList.Title")), "Window", ScenesWindowType.Barriers, sceneEditor);
                CreateSceneEditorTab<ScenesWindowPlayerMovement>(rect, new GUIContent(TC.get("Trajectory.Title")), "Window", ScenesWindowType.PlayerMovement, sceneEditor);
            }

            DefaultOpenedWindow = ScenesWindowType.Appearance;
            OpenedWindow = ScenesWindowType.Appearance;
        }


        public override void Draw(int aID)
        {
            dataControlList.index = GameRources.GetInstance().selectedSceneIndex;
            // SceneEditor population
            if (GameRources.GetInstance().selectedSceneIndex != -1)
            {
                var scene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];

                sceneEditor.Components = uAdventureWindowMain.Components;
                var allElements = new List<DataControl>();
                allElements.AddRange(scene
                    .getReferencesList()
                    .getAllReferencesDataControl()
                    .FindAll(elem => elem.getErdc() != null)
                    .ConvertAll(elem => elem.getErdc() as DataControl));
                allElements.AddRange(scene.getActiveAreasList().getActiveAreas().Cast<DataControl>());
                allElements.AddRange(scene.getExitsList().getExits().Cast<DataControl>());

                var playerMode = ScenesWindowPlayerMovement.PlayerMode.NoPlayer;
                if (Controller.Instance.PlayerMode == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER)
                {
                    allElements.AddRange(scene.getBarriersList().getBarriers().Cast<DataControl>());
                    playerMode = ScenesWindowPlayerMovement.GetScenePlayerMode(scene);
                }
                switch (playerMode)
                {
                    case ScenesWindowPlayerMovement.PlayerMode.InitialPosition:
                        allElements.Add(Controller.Instance.SelectedChapterDataControl.getPlayer());
                        break;
                    case ScenesWindowPlayerMovement.PlayerMode.Trajectory:
                        allElements.AddRange(scene.getTrajectory().getNodes().Cast<DataControl>());
                        allElements.AddRange(scene.getTrajectory().getSides().Cast<DataControl>());
                        allElements.Add(scene.getTrajectory());
                        break;
                }
                sceneEditor.Elements = allElements;
            }

            // Send the callback back
            base.Draw(aID);
        }

        protected override void OnDrawMainView(int aID)
        {
            chapterPreview.Rect = this.Rect;
            chapterPreview.Draw(aID);
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            GameRources.GetInstance().selectedSceneIndex = -1;
        }

        public void ShowItemWindowView(int s)
        {
            if (GameRources.GetInstance().selectedSceneIndex != s)
            {
                var selectedScene = s >= 0 && s < dataControlList.list.Count
                    ? dataControlList.list[s] as SceneDataControl
                    : null;
                foreach (var sceneEditorWindow in Childs.Values
                    .Where(w => w is SceneEditorWindow)
                    .Cast<SceneEditorWindow>())
                {
                    try
                    {
                        sceneEditorWindow.OnSceneSelected(selectedScene);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
                GameRources.GetInstance().selectedSceneIndex = s;
            }

        }

        // ---------------------------------------------
        //         Reorderable List Handlers
        // ---------------------------------------------

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnButton()
        {
            base.OnButton();
            ShowBaseWindowView();

            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getScenesList(),
                sceneList => (sceneList as ScenesListDataControl).getScenes().Cast<DataControl>().ToList());
        }

        private class ChapterPreview : PreviewLayoutWindow, ProjectConfigDataConsumer
        {
            private const float SceneScaling = 0.2f;
            private const float SpaceWidth = 800f;
            private const float SpaceHeight = 600f;
            private Rect space;

            private readonly Dictionary<string, Color> sceneColors;

            private readonly Dictionary<string, Vector2> positions;
            private readonly Dictionary<string, Texture2D> images;
            private readonly Dictionary<string, Vector2> sizes;

            private readonly Texture2D noBackground;

            public delegate void OnSelectElementDelegate(DataControl selected);
            public event OnSelectElementDelegate OnSelectElement;

            private readonly DataControlList sceneList;

            public ChapterPreview(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                ProjectConfigData.addConsumer(this);

                PreviewTitle = "Chapter.Preview".Traslate();

                noBackground = Controller.ResourceManager.getImage(SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);

                sceneColors = new Dictionary<string, Color>();
                positions = new Dictionary<string, Vector2>();
                images = new Dictionary<string, Texture2D>();
                sizes = new Dictionary<string, Vector2>();

                // SceneList
                sceneList = new DataControlList()
                {
                    RequestRepaint = Repaint,
                    footerHeight = 10,
                    elementHeight = 20,
                    Columns = new List<ColumnList.Column>()
                    {
                        new ColumnList.Column()
                        {
                            Text =  TC.get("Element.Name1"),
                            SizeOptions = new GUILayoutOption[]{ GUILayout.ExpandWidth(true) }
                        },
                        new ColumnList.Column()
                        {
                            Text = "Open",
                            SizeOptions = new GUILayoutOption[]{ GUILayout.Width(250) }
                        }
                    },
                    drawCell = (cellRect, row, column, isActive, isFocused) =>
                    {
                        var scene = ((ScenesListDataControl)sceneList.DataControl).getScenes()[row];
                        switch (column)
                        {
                            case 0: GUI.Label(cellRect, scene.getId()); break;
                            case 1:
                                if (GUI.Button(cellRect, TC.get("GeneralText.Edit")))
                                    GameRources.GetInstance().selectedSceneIndex = row;
                                break;
                        }
                    }
                };

            }

            protected override void DrawInspector()
            {
                var scenesListDataControl = Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList();
                sceneList.SetData(scenesListDataControl, data => (data as ScenesListDataControl).getScenes().Cast<DataControl>().ToList());

                sceneList.DoList(160);

            }

            protected override void DrawPreviewHeader()
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal("preToolbar");
                GUILayout.Label("Chapter.Preview".Traslate(), "preToolbar", GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Layout", "preButton"))
                {
                    Layout();
                }
                GUILayout.EndHorizontal();
            }

            public override void DrawPreview(Rect rect)
            {
                space = rect.AdjustToRatio(SpaceWidth, SpaceHeight);
                foreach (var scene in Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList().getScenes())
                {
                    DrawScene(scene);
                }

                foreach (var scene in Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList().getScenes())
                {
                    foreach (var exit in scene.getExitsList().getExits())
                        DrawExit(scene, exit);
                }
            }

            public void updateData()
            {
                positions.Clear();
            }

            // AUX FUNCTIONS

            private void DrawScene(SceneDataControl scene)
            {
                var rect = AdaptToViewport(GetSceneRect(scene), space);

                switch (Event.current.type)
                {
                    case EventType.Repaint:
                        GUI.DrawTexture(rect, images[scene.getPreviewBackground()] ?? noBackground);
                        if (sceneList.index != -1 && Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[sceneList.index] == scene)
                        {
                            HandleUtil.DrawPolyLine(rect.ToPoints().ToArray(), true, Color.red);
                        }
                        break;
                }

                EditorGUI.DropShadowLabel(new Rect(rect.position - new Vector2(20, 0), rect.size), scene.getId());

                var prevHot = GUIUtility.hotControl;
                EditorGUI.BeginChangeCheck();
                rect = HandleUtil.HandleRectMovement(scene.GetHashCode(), rect);
                if (EditorGUI.EndChangeCheck())
                {
                    rect = RevertFromViewport(rect, space);
                    positions[scene.getId()] = rect.position;

                    // Update in the project data
                    var id = GetScenePropertyId(Controller.Instance.SelectedChapterDataControl, scene);
                    ProjectConfigData.setProperty(id + ".X", ((int)positions[scene.getId()].x).ToString());
                    ProjectConfigData.setProperty(id + ".Y", ((int)positions[scene.getId()].y).ToString());
                }
                if (GUIUtility.hotControl != prevHot)
                {
                    sceneList.index = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes().IndexOf(scene);
                    if (Event.current.clickCount == 2)
                    {
                        OnSelectElement(scene);
                    }
                }

            }

            private void DrawExit(SceneDataControl scene, ExitDataControl exit)
            {
                var scenes = Controller.Instance.SelectedChapterDataControl.getScenesList();
                var index = scenes.getSceneIndexByID(exit.getNextSceneId());

                // If the exit points to a cutscene it normally is out of the array
                if (index < 0 || index > scenes.getScenes().Count)
                    return;

                var polygon = AdaptToViewport(GetExitArea(scene, exit), space);
                var c = sceneColors[scene.getPreviewBackground()];
                c = new Color(c.r, c.g, c.b, 0.8f);
                HandleUtil.DrawPolygon(polygon, c);

                var nextScene = scenes.getScenes()[index];
                var sceneRect = AdaptToViewport(GetSceneRect(nextScene), space);

                Vector2 origin = Center(polygon), destination;
                if (exit.hasDestinyPosition())
                {
                    destination = new Vector2(exit.getDestinyPositionX(), exit.getDestinyPositionY());
                    destination.x = sceneRect.x + (destination.x / sizes[nextScene.getPreviewBackground()].x) * sceneRect.width;
                    destination.y = sceneRect.y + (destination.y / sizes[nextScene.getPreviewBackground()].y) * sceneRect.height;
                }
                else
                {
                    destination = Center(sceneRect.ToPoints().ToArray());
                }

                HandleUtil.DrawPolyLine(new Vector2[] { origin, destination }, false, sceneColors[scene.getPreviewBackground()], 4);

                DrawArrowCap(destination, (destination - origin), 15f);
            }

            private void DrawArrowCap(Vector2 point, Vector2 direction, float size)
            {
                var halfSide = size / Mathf.Tan(60 * Mathf.Deg2Rad);
                var rotatedVector = new Vector2(-direction.y, direction.x).normalized;
                var basePoint = point - (direction.normalized * size);

                Vector3[] capPoints = new Vector3[]
                {
                    point,
                    basePoint + rotatedVector * halfSide,
                    basePoint - rotatedVector * halfSide
                };
                Handles.BeginGUI();
                Handles.DrawAAConvexPolygon(capPoints);
                Handles.EndGUI();
            }

            private static Vector3 ToHSV(Color color)
            {
                float hue;
                float saturation;
                float lightness;
                Color.RGBToHSV(color, out hue, out saturation, out lightness);
                return new Vector3(hue, saturation, lightness);
            }

            private static Color FromHSV(Vector3 hsv)
            {
                return Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
            }

            private Rect AdaptToViewport(Rect rect, Rect viewport)
            {
                // ??? PROFIT
                return rect.ToPoints().ToList().ConvertAll(p => AdaptToViewport(p, viewport)).ToArray().ToRect();
            }
            private Rect RevertFromViewport(Rect rect, Rect viewport)
            {
                // ??? PROFIT
                return rect.ToPoints().ToList().ConvertAll(p => RevertFromViewport(p, viewport)).ToArray().ToRect();
            }

            private Vector2[] AdaptToViewport(Vector2[] points, Rect viewport)
            {
                return points.ToList().ConvertAll(p => AdaptToViewport(p, viewport)).ToArray();
            }
            private Vector2[] RevertFromViewport(Vector2[] points, Rect viewport)
            {
                return points.ToList().ConvertAll(p => RevertFromViewport(p, viewport)).ToArray();
            }

            private Vector2 AdaptToViewport(Vector2 point, Rect viewport)
            {
                return viewport.position + new Vector2(point.x * viewport.width / SpaceWidth, point.y * viewport.height / SpaceHeight);
            }
            private Vector2 RevertFromViewport(Vector2 point, Rect viewport)
            {
                return new Vector2((point.x - viewport.position.x) * SpaceWidth / viewport.width, (point.y - viewport.position.y) * SpaceHeight / viewport.height);
            }

            private Rect GetSceneRect(SceneDataControl scene)
            {
                if (!this.positions.ContainsKey(scene.getId()))
                {
                    string id = GetScenePropertyId(Controller.Instance.SelectedChapterDataControl, scene);
                    string X = ProjectConfigData.getProperty(id + ".X");
                    string Y = ProjectConfigData.getProperty(id + ".Y");
                    positions.Add(scene.getId(), new Vector2(ExParsers.ParseDefault(X, 0), ExParsers.ParseDefault(Y, 0)));
                }

                var background = scene.getPreviewBackground();
                if (!sizes.ContainsKey(background))
                {
                    Texture2D scenePreview = null;
                    Vector2 previewSize = new Vector2(800, 600);
                    if (!images.ContainsKey(background))
                    {
                        scenePreview = Controller.ResourceManager.getImage(background) ?? Controller.ResourceManager.getImage(SpecialAssetPaths.ASSET_EMPTY_IMAGE);
                        images[background] = scenePreview;
                    }
                    else
                    {
                        scenePreview = images[background];
                    }

                    if (scenePreview)
                    {
                        previewSize = new Vector2(scenePreview.width, scenePreview.height);
                    }

                    sizes.Add(background, previewSize);
                    Color color;
                    try
                    {
                        var pixel = scenePreview.GetPixel(scenePreview.width / 2, scenePreview.height / 2);
                        var colorAsVector = ToHSV(new Color(1f - pixel.r, 1f - pixel.g, 1f - pixel.b));
                        colorAsVector.y *= 2f;
                        colorAsVector.z *= 1.5f;
                        color = FromHSV(colorAsVector);
                    }
                    catch
                    {
                        // Error getting the pixel
                        color = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.5f, 1f, 1f, 1f);
                    }

                    sceneColors[background] = color;
                }

                return new Rect(positions[scene.getId()], sizes[background] * SceneScaling);
            }

            private Vector2[] GetExitArea(SceneDataControl scene, ExitDataControl exit)
            {
                var holder = GetSceneRect(scene);
                var xRatio = holder.width / images[scene.getPreviewBackground()].width;
                var yRatio = holder.height / images[scene.getPreviewBackground()].height;

                Vector2[] polygon = null;
                var rectangle = exit.getRectangle();
                if (rectangle.isRectangular())
                {
                    polygon = new Rect(rectangle.getX(), rectangle.getY(), rectangle.getWidth(), rectangle.getHeight()).ToPoints();
                }
                else
                {
                    polygon = rectangle.getPoints().ToArray();
                }

                return polygon.ToList().ConvertAll(p => (new Vector2(p.x * xRatio, p.y * yRatio) + holder.position)).ToArray();
            }

            private Vector2 Center(Vector2[] polygon)
            {
                Vector2 sum = Vector2.zero;
                for (int i = 0; i < polygon.Length; i++)
                    sum += polygon[i];
                return sum / polygon.Length;
            }

            private string GetScenePropertyId(ChapterDataControl chapter, SceneDataControl scene)
            {
                var index = Controller.Instance.ChapterList.getChapters().IndexOf(chapter);
                return "Chapter" + index + "." + scene.getId();
            }

            private void Layout()
            {
                try
                {
                    var scenes = Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList();

                    // Layout algorithm
                    var settings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings();
                    settings.MaxAspectRatioEccentricity = 1.6;
                    settings.NodeSeparation = 10;
                    settings.PackingMethod = PackingMethod.Compact;

                    // Graph
                    GeometryGraph graph = new GeometryGraph();
                    graph.BoundingBox = new Microsoft.Msagl.Core.Geometry.Rectangle(0, 0, SpaceWidth, SpaceHeight);
                    graph.UpdateBoundingBox();

                    Dictionary<SceneDataControl, Node> sceneToNode = new Dictionary<SceneDataControl, Node>();
                    Dictionary<Tuple<Node, Node>, bool> present = new Dictionary<Tuple<Node, Node>, bool>();

                    foreach (var scene in scenes.getScenes())
                    {
                        sizes.Remove(scene.getPreviewBackground());
                        var rect = GetSceneRect(scene);
                        var node = new Node(CurveFactory.CreateRectangle(rect.width, rect.height, new Point()), scene.getId());
                        graph.Nodes.Add(node);
                        sceneToNode.Add(scene, node);
                    }

                    foreach (var scene in scenes.getScenes())
                    {
                        var node = sceneToNode[scene];
                        foreach (var exit in scene.getExitsList().getExits())
                        {
                            var index = scenes.getSceneIndexByID(exit.getNextSceneId());

                            // If the exit points to a cutscene it normally is out of the array
                            if (index < 0 || index >= scenes.getScenes().Count)
                            {
                                continue;
                            }

                            var nextScene = scenes.getScenes()[index];

                            var t = new Tuple<Node, Node>(node, sceneToNode[nextScene]);
                            if (!present.ContainsKey(t))
                            {
                                present.Add(t, true);
                                graph.Edges.Add(new Edge(node, sceneToNode[nextScene]));

                                var exitOrigin = GetExitArea(scene, exit).ToRect().center;
                                var originRect = GetSceneRect(scene);

                                var pos = exitOrigin - originRect.position;
                                pos.x = Mathf.Clamp01(pos.x / originRect.width);
                                pos.y = Mathf.Clamp01(pos.y / originRect.height);

                                // Positioning constraints
                                if (pos.x < 0.3)
                                {
                                    settings.AddLeftRightConstraint(t.Item2, t.Item1);
                                }
                                if (pos.x > 0.7)
                                {
                                    settings.AddLeftRightConstraint(t.Item1, t.Item2);
                                }
                            }
                        }
                    }


                    // Do the layouting
                    LayoutHelpers.CalculateLayout(graph, settings, null);

                    // Extract the results
                    var graphRect = new Rect((float)graph.Left, (float)graph.Bottom, (float)graph.Width, (float)graph.Height);
                    var canvasRect = new Rect(0, 0, SpaceWidth, SpaceHeight);

                    foreach (var scene in scenes.getScenes())
                    {
                        var n = sceneToNode[scene];
                        positions[scene.getId()] = TransformPoint(new Vector2((float)(n.Center.X - n.Width / 2f), (float)(n.Center.Y + n.Height / 2f)), graphRect, canvasRect, true);

                        // Update in the project data
                        var id = GetScenePropertyId(Controller.Instance.SelectedChapterDataControl, scene);
                        ProjectConfigData.setProperty(id + ".X", ((int)positions[scene.getId()].x).ToString());
                        ProjectConfigData.setProperty(id + ".Y", ((int)positions[scene.getId()].y).ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message + " : " + ex.StackTrace);
                }
            }

            Vector2 TransformPoint(Vector2 point, Rect from, Rect to, bool invertY)
            {
                float absoluteX = (point.x - from.x) / from.width,
                    absoluteY = (point.y - from.y) / from.height;

                if (invertY)
                {
                    absoluteY = 1 - absoluteY;
                }

                return new Vector2(absoluteX * to.width + to.x, absoluteY * to.height + to.y);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq; 
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class MapSceneWindowAppearance : MapEditorWindow
    {
        private readonly string[] renderStyles, cameraTypes, tileMetaNames;
        private readonly Dictionary<Type, bool> disabledTypes;

        public MapSceneWindowAppearance(Rect rect, GUIContent content, GUIStyle style, MapEditor componentBasedEditor, params GUILayoutOption[] options) 
            : base(rect, content, style, componentBasedEditor, options)
        {
            renderStyles = Enum.GetNames(typeof(RenderStyle))
                .Select(e => TC.get("Geo.MapScene.Appearance.RenderStyle." + e)).ToArray();
            cameraTypes = Enum.GetNames(typeof(CameraType))
                .Select(e => TC.get("Geo.MapScene.Appearance.CameraType." + e)).ToArray();
            tileMetaNames = TileProvider.Instance.PublicMetas.Select(m => TC.get(m.Name)).ToArray();

            new GameplayAreaComponent(rect, content, style, options);

            disabledTypes = new Dictionary<Type, bool>
            { 
                { typeof(GeoElementRefDataControl), false },
                { typeof(ExtElementRefDataControl), false }
            };
        }

        public override void Draw(int aID)
        {
            var prevDisabled = componentBasedEditor.TypeEnabling;
            var workingMapScene = GeoController.Instance.MapScenes[GeoController.Instance.SelectedMapScene];
            componentBasedEditor.SelectedElement = workingMapScene.GameplayArea;
            componentBasedEditor.TypeEnabling = disabledTypes;
            base.Draw(aID);
            componentBasedEditor.TypeEnabling = prevDisabled;
            componentBasedEditor.SelectedElement = null;
        }

        protected override void DrawInspector()
        {
            var workingMapScene = GeoController.Instance.MapScenes[GeoController.Instance.SelectedMapScene];

            EditorGUILayout.HelpBox("Geo.MapScene.Appearance.RenderStyle.Info".Traslate(), MessageType.Info);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.BeginChangeCheck();
                var newRenderStyle = (RenderStyle) EditorGUILayout.Popup(
                    TC.get("Geo.MapScene.Appearance.RenderStyle.Title"),
                    (int) workingMapScene.RenderStyle, renderStyles);
                if (EditorGUI.EndChangeCheck())
                {
                    workingMapScene.RenderStyle = newRenderStyle;
                }
            }

            using (new EditorGUI.DisabledScope(workingMapScene.RenderStyle != RenderStyle.Tile))
            {
                var tileMetas = TileProvider.Instance.PublicMetas;
                var currentTileMetaIndex = Mathf.Max(0,
                    Array.FindIndex(tileMetas, tm => tm.Identifier == workingMapScene.GameplayArea.TileMetaIdentifier));

                EditorGUI.BeginChangeCheck();
                var newTileMeta = tileMetas[EditorGUILayout.Popup(TC.get("Geo.MapScene.Appearance.TileStyle.Title"),
                    currentTileMetaIndex, tileMetaNames)];
                if (EditorGUI.EndChangeCheck())
                {
                    workingMapScene.GameplayArea.TileMetaIdentifier = newTileMeta.Identifier;
                    componentBasedEditor.TileMeta = newTileMeta;
                }
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.BeginChangeCheck();
                var newCameraType = (CameraType)EditorGUILayout.Popup(TC.get("Geo.MapScene.Appearance.CameraType.Title"),
                    (int)workingMapScene.CameraType, cameraTypes);
                if (EditorGUI.EndChangeCheck())
                {
                    workingMapScene.CameraType = newCameraType;
                }
            }
        }

        [EditorComponent(typeof(GameplayAreaDataControl), Name = "Geo.GameplayAreaComponent.Name", Order = 0)]
        public class GameplayAreaComponent : AbstractEditorComponent
        {
            public GameplayAreaComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
            }

            public override void Draw(int aID)
            {
                var gameplayArea = Target as GameplayAreaDataControl;

                EditorGUILayout.HelpBox("Geo.MapScene.GameplayArea.Info".Traslate(), MessageType.Info);
                
                EditorGUI.BeginChangeCheck();
                var usesGameplayArea = EditorGUILayout.Toggle("Geo.MapScene.GameplayArea.UsesGameplayArea".Traslate(),
                    gameplayArea.UsesGameplayArea);
                if (EditorGUI.EndChangeCheck())
                {
                    gameplayArea.UsesGameplayArea = usesGameplayArea;
                }

                EditorGUI.BeginChangeCheck();
                var newBoundingBox = EditorGUILayout.RectField("Geo.MapScene.GameplayArea.BoundingBox".Traslate(),
                    gameplayArea.BoundingBox.ToRect()).ToRectD();
                if (EditorGUI.EndChangeCheck())
                {
                    gameplayArea.BoundingBox = newBoundingBox;
                }

                using (new EditorGUI.DisabledScope(!gameplayArea.IsModified))
                {
                    if (GUILayout.Button("Geo.MapScene.GameplayArea.Apply".Traslate()))
                    {
                        gameplayArea.DownloadAndApplyChanges();
                    }
                }
            }

            public override void OnDrawingGizmos()
            {
                var gameplayArea = Target as GameplayAreaDataControl;
                if (!gameplayArea.UsesGameplayArea)
                {
                    return;
                }

                var viewRect = MapEditor.Current.ScreenRect.ToPoints();
                var gameplayAreaRect = MapEditor.Current.ToRelative(MapEditor.Current.LatLonToPixels(gameplayArea.BoundingBox.ToPoints())).ToRectD().ToRect().ToPoints();

                var points = new Vector2[]
                {
                    // TopLeft
                    viewRect[0],
                    gameplayAreaRect[0],
                    // TopCenter
                    new Vector2(gameplayAreaRect[0].x, viewRect[0].y),
                    gameplayAreaRect[1],
                    // TopRight
                    new Vector2(gameplayAreaRect[1].x, viewRect[0].y),
                    new Vector2(viewRect[1].x, gameplayAreaRect[1].y),
                    // MiddleRight
                    gameplayAreaRect[1],
                    new Vector2(viewRect[1].x, gameplayAreaRect[2].y),
                    // BottomRight
                    gameplayAreaRect[2],
                    viewRect[2],
                    // BottomCenter
                    gameplayAreaRect[3],
                    new Vector2(gameplayAreaRect[2].x, viewRect[2].y),
                    // BottomLeft
                    new Vector2(viewRect[3].x, gameplayAreaRect[3].y),
                    new Vector2(gameplayAreaRect[3].x, viewRect[3].y),
                    // MiddleLeft
                    new Vector2(viewRect[0].x, gameplayAreaRect[0].y),
                    gameplayAreaRect[3]
                };

                for (int i = 0; i < points.Length; i+=2)
                {
                    var rect = new Rect(points[i], points[i + 1] - points[i]);
                    if (rect.width > 0 || rect.height > 0)
                    {

                        Handles.BeginGUI();
                        Handles.color = MapEditor.GetColor(new Color(0,0,0,0.5f));
                        Handles.DrawAAConvexPolygon(rect.ToPoints3());
                        Handles.EndGUI();
                    }
                }
            }

            public override bool Update()
            {
                if (Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    var gameplayArea = Target as GameplayAreaDataControl;
                    if (!gameplayArea.UsesGameplayArea)
                    {
                        return false;
                    }

                    var mouseD = Event.current.mousePosition.ToVector2d();
                    var gameplayAreaRect = MapEditor.Current.ToRelative(MapEditor.Current.LatLonToPixels(gameplayArea.BoundingBox.ToPoints()));
                    foreach (var p in gameplayAreaRect)
                    {
                        if ((p - mouseD).sqrMagnitude < 100)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public override void OnDrawingGizmosSelected()
            {
                var gameplayArea = Target as GameplayAreaDataControl;
                if (!gameplayArea.UsesGameplayArea)
                {
                    return;
                }
                
                var gameplayAreaRect = MapEditor.Current.ToRelative(MapEditor.Current.LatLonToPixels(gameplayArea.BoundingBox.ToPoints()))
                    .ToRectD().ToRect();


                var handleRectID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                EditorGUI.BeginChangeCheck();
                Handles.BeginGUI();
                var newRect = HandleUtil.HandleRect(handleRectID, gameplayAreaRect, 10, (_, __, ___) => { },
                    (p, hover, active) => HandleUtil.DrawSquare(p, 10, MapEditor.GetColor(Color.yellow),
                        hover ? MapEditor.GetColor(Color.red) : MapEditor.GetColor(Color.black)));
                Handles.EndGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    var latLonRect = MapEditor.Current
                        .PixelsToLatLon(MapEditor.Current.FromRelative(newRect.ToRectD().ToPoints())).ToRectD();
                    gameplayArea.BoundingBox = latLonRect;
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

using uAdventure.Editor;
using UnityEditorInternal;

using UnityEditor;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Geo
{
    [EditorWindowExtension(110, typeof(MapSceneDataControl))]
    public class MapSceneWindow : PreviewDataControlExtension
    {
        public enum MapSceneWindows
        {
            Appearance, References, Documentation
        }

        /* ----------------------------------
         * GUI ELEMENTS
         * -----------------------------------*/
        private readonly MapEditor mapEditor;

        private readonly MapSceneWindowAppearance appearanceWindow;

        public MapSceneWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, new GUIContent("Map Scenes"), aStyle, aOptions)
        {
            var _ = GeoController.Instance;
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/map"),
                text = "Geo.MapScene.Title".Traslate()
            };

            mapEditor = new MapEditor()
            {
                Components = uAdventureWindowMain.Components,
                Elements = new List<DataControl> { null },
                Repaint = () => Repaint(),
                PlaceSearcher = { OnRequestRepaint = () => OnRequestRepaint() }
            };

            appearanceWindow = new MapSceneWindowAppearance(aStartPos, new GUIContent(), aStyle, mapEditor, aOptions);

            AddTab(TC.get("Geo.MapScene.AppearanceWindow.Title"), MapSceneWindows.Appearance, appearanceWindow);
            AddTab(TC.get("Geo.MapScene.ReferencesWindow.Title"), MapSceneWindows.References, new MapSceneWindowReferences(aStartPos, new GUIContent(), aStyle, mapEditor, aOptions));
            AddTab(TC.get("Geo.MapScene.DocumentationWindow.Title"), MapSceneWindows.Documentation, new MapSceneWindowDocumentation(aStartPos, new GUIContent(), aStyle, aOptions));

        }

        /* ----------------------------------
          * ON GUI: Used for drawing the window every unity event
          * ----------------------------------*/


        protected override void OnSelect(ReorderableList r)
        {
            GeoController.Instance.SelectedMapScene = r.index;
            if (GeoController.Instance.SelectedMapScene >= 0)
            {
                var mapScene = GeoController.Instance.MapScenes[GeoController.Instance.SelectedMapScene];
                mapEditor.Elements = mapScene.Elements.DataControls.Cast<DataControl>().Union(new []{mapScene.GameplayArea}).ToList();
                mapEditor.Center = mapScene.LatLon;
                mapEditor.Zoom = mapScene.Zoom;
                mapEditor.TileMeta = TileProvider.Instance.PublicMetas.First(m => m.Identifier == mapScene.GameplayArea.TileMetaIdentifier);
            }
        }


        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.SetData(GeoController.Instance.MapScenes,
                mapScenesList => (mapScenesList as ListDataControl<ChapterDataControl, MapSceneDataControl>).DataControls.Cast<DataControl>().ToList());
            GeoController.Instance.SelectedMapScene = -1;
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            var mapSceneDataControl = dataControlList.list[index] as MapSceneDataControl;
            var eventType = Event.current.type;
            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
            {
                // Force the event ussage to prevent the map interaction
                Event.current.Use();
            }

            mapEditor.Elements = mapSceneDataControl.Elements.DataControls.Cast<DataControl>().Union(new[] { mapSceneDataControl.GameplayArea }).ToList();
            mapEditor.Center = mapSceneDataControl.LatLon;
            mapEditor.Zoom = mapSceneDataControl.Zoom;
            mapEditor.TileMeta = TileProvider.Instance.GetTileMeta(mapSceneDataControl.GameplayArea.TileMetaIdentifier);
            mapEditor.Draw(rect);

            Event.current.type = eventType;
        }

        public override void Draw(int aID)
        {
            var mapScene = GeoController.Instance.SelectedMapScene >= 0 ? GeoController.Instance.MapScenes[GeoController.Instance.SelectedMapScene] : null;
            if (mapScene != null)
            {
                mapEditor.Elements = mapScene.Elements.DataControls.Cast<DataControl>().Union(new[] { mapScene.GameplayArea }).ToList();
            }

            base.Draw(aID);

            if (mapScene != null)
            {
                mapScene.LatLon = mapEditor.Center;
                mapScene.Zoom = mapEditor.Zoom;
            }
        }
    }
}
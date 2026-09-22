using UnityEngine;
using UnityEditorInternal;
using System.Linq;
using uAdventure.Editor;
using System.Collections.Generic;
using uAdventure.Core;

namespace uAdventure.Geo
{

    [EditorWindowExtension(115, typeof(GeoElementDataControl))]
    public class GeoElementWindow : PreviewDataControlExtension
    {
        public enum GeoElementWindows
        {
            Geometry, Description, Actions
        }

        /* ----------------------------------
         * GUI ELEMENTS
         * -----------------------------------*/
        private readonly MapEditor mapEditor;

        private readonly GeoElementWindowGeometry geometryComponent; 

        public GeoElementWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, new GUIContent("Geo.GeoElement.Title".Traslate()), aStyle, aOptions)
        {
            ButtonContent = new GUIContent
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/poi"),
                text = "Geo.GeoElement.Title".Traslate()
            };

            mapEditor = new MapEditor()
            {
                Components = uAdventureWindowMain.Components,
                Elements = new List<DataControl> { null },
                Repaint = () => Repaint(),
                PlaceSearcher = { OnRequestRepaint = () => OnRequestRepaint() }
            };

            geometryComponent = new GeoElementWindowGeometry(aStartPos, new GUIContent(), aStyle, mapEditor, aOptions);

            AddTab(TC.get("Geo.GeoElement.GeometryWindow.Title"), GeoElementWindows.Geometry, geometryComponent);
            AddTab(TC.get("Geo.GeoElement.DescriptionWindow.Title"), GeoElementWindows.Description, new GeoElementWindowDescription(aStartPos, new GUIContent(), aStyle, aOptions));
            AddTab(TC.get("Geo.GeoElement.ActionsWindow.Title"), GeoElementWindows.Actions, new GeoElementWindowActions(aStartPos, new GUIContent(), aStyle, aOptions));

        }

        /* ----------------------------------
          * ON GUI: Used for drawing the window every unity event
          * ----------------------------------*/


        protected override void OnSelect(ReorderableList r)
        {
            GeoController.Instance.SelectedGeoElement = r.index;
            if (GeoController.Instance.SelectedGeoElement >= 0)
            {
                var geoElement = GeoController.Instance.GeoElements[GeoController.Instance.SelectedGeoElement];

                mapEditor.Elements[0] = geoElement;

                var geometry = geoElement.GMLGeometries[geoElement.SelectedGeometry];
                mapEditor.Center = geometry.Center;
                if (geometry.Points.Length > 0)
                {
                    mapEditor.ZoomToBoundingBox(geometry.Points.ToArray().ToRectD());
                }
            }
        }


        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.SetData(GeoController.Instance.GeoElements,
                geoElementsList => (geoElementsList as ListDataControl<ChapterDataControl, GeoElementDataControl>).DataControls.Cast<DataControl>().ToList());
            GeoController.Instance.SelectedGeoElement = -1;
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            var geoElementDataControl = dataControlList.list[index] as GeoElementDataControl;
            var eventType = Event.current.type;
            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
            {
                // Force the event ussage to prevent the map interaction
                Event.current.Use();
            }
            
            mapEditor.Elements[0] = geoElementDataControl;
            mapEditor.Center = geoElementDataControl.GMLGeometries[geoElementDataControl.SelectedGeometry].Center;
            mapEditor.ZoomToBoundingBox(geoElementDataControl.GMLGeometries[geoElementDataControl.SelectedGeometry].BoundingBox);
            mapEditor.Draw(rect);

            Event.current.type = eventType;
            geometryComponent.Target = null;
        }
    }


}


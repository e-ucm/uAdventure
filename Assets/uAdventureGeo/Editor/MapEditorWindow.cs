

using System;
using MapzenGo.Helpers.Search;
using MapzenGo.Models.Settings.Editor;
using System.Collections.Generic;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class MapEditorWindow : ComponentBasedEditorWindow<MapEditor>
    {
        public MapEditorWindow(Rect rect, GUIContent content, GUIStyle style, MapEditor componentBasedEditor, params GUILayoutOption[] options) : base(rect, content, style, componentBasedEditor, options)
        {
            LeaveWindowSpace = false;
            Margin = 0;
        }

        protected override void DrawPreviewHeader()
        {
            GUILayout.Space(10);
            using (new EditorGUILayout.HorizontalScope("preToolbar"))
            {
                GUILayout.Label(PreviewTitle, "preToolbar", GUILayout.Width(180));
                componentBasedEditor.PlaceSearcher.Source = GUILayout.Toggle(componentBasedEditor.PlaceSearcher.Source == 1, "WorldWide", "toolbarbutton", GUILayout.Width(70)) ? 1 : 0;
                if (componentBasedEditor.PlaceSearcher.DoLayout("ToolbarSeachTextField"))
                {
                    var place = componentBasedEditor.PlaceSearcher.Place;
                    componentBasedEditor.Center = place.LatLon;
                    componentBasedEditor.ZoomToBoundingBox(place.RectBoundingBox);
                }

                if (GUILayout.Button("", string.IsNullOrEmpty(componentBasedEditor.PlaceSearcher.Value) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
                {
                    componentBasedEditor.PlaceSearcher.Value = "";
                    GUI.FocusControl(null);
                    GUIUtility.hotControl = -1;
                    GUIUtility.keyboardControl = -1;
                }

                EditorGUI.BeginChangeCheck();
                var latLon = componentBasedEditor.Center;
                GUILayout.Label("Lat: ", "preToolbar", GUILayout.Width(35));
                var locationX = EditorGUILayout.DoubleField(latLon.x, "ToolbarTextField", GUILayout.Width(100));
                GUILayout.Label("Lon: ", "preToolbar", GUILayout.Width(35));
                var locationY = EditorGUILayout.DoubleField(latLon.y, "ToolbarTextField", GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    componentBasedEditor.Center = new Vector2d(locationX, locationY);
                }
            }

            var lastRect = GUILayoutUtility.GetLastRect();
            var window = m_Rect;
            window.height += 35;

            previewHeight = previewResizer.ResizeHandle(window, 50, 50, 20, lastRect);
            if (!previewResizer.GetExpanded())
            {
                previewResizer.ToggleExpanded();
            }
        }
    }
}

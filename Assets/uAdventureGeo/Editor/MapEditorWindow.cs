

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
                GUILayout.Label(PreviewTitle, "preToolbar", GUILayout.Width(250));
                if (componentBasedEditor.PlaceSearcher.DoLayout("ToolbarSeachTextField"))
                {
                    componentBasedEditor.Center = componentBasedEditor.PlaceSearcher.LatLon;
                    componentBasedEditor.ZoomToBoundingBox(componentBasedEditor.PlaceSearcher.BoundingBox);
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
        }
    }
}

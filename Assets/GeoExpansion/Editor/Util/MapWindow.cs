using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MapzenGo.Helpers.Search;
using MapzenGo.Models.Settings.Editor;
using MapzenGo.Helpers;
using UnityEditorInternal;

namespace uAdventure.Geo
{
    public class MapWindow : EditorWindow
    {


        const string PATH_SAVE_SCRIPTABLE_OBJECT = "Assets/GeoExpansion/MapzenGo/Resources/Settings/";

        /* ----------------------------------------
         * CREATE: static method for window creation
         * ---------------------------------------- */
        [MenuItem("Window/Map Window")]
        static void Create()
        {
            // Get existing open window or if none, make a new one:
            MapWindow window = (MapWindow)EditorWindow.GetWindow(typeof(MapWindow));

            window.place = FindObjectOfType<SearchPlace>();
            if (window.place == null)
            {
                SearchPlace search = new GameObject("Searcher").AddComponent<SearchPlace>();
                window.place = search;
            }


            window.Init();
            window.Show();
        }

        /* ---------------------------------
         * Attributes
         * -------------------------------- */

        private SearchPlace place;
        private string address = "";
        private Vector2 location;
        private string lastSearch = "";
        private float timeSinceLastWrite;
        private List<GMLGeometry> geometries;
        private GMLGeometry editing;

        /* ----------------------------------
         * GUI ELEMENTS
         * -----------------------------------*/
        private DropDown addressDropdown;
        private GUIMap map;
        private ReorderableList geometriesReorderableList;


        /* ----------------------------------
         * INIT: Used for late initialization after constructor
         * ----------------------------------*/
        void Init()
        {
            EditorApplication.update += this.Update;

            place.DataStructure = HelperExtention.GetOrCreateSObjectReturn<StructSearchData>(ref place.DataStructure, PATH_SAVE_SCRIPTABLE_OBJECT);
            place.namePlaceСache = "";
            place.DataStructure.dataChache.Clear();

            addressDropdown = new DropDown("Address");
            map = new GUIMap();
            map.Repaint += Repaint;
            map.Zoom = 19;

            geometriesReorderableList = new ReorderableList(new ArrayList(), typeof(GMLGeometry), true, true, true, true);
            geometriesReorderableList.drawHeaderCallback += DrawGMLGeometryHeader;
            geometriesReorderableList.drawElementCallback += DrawGMLGeometry;
            geometriesReorderableList.onAddCallback += AddGMLGeometry;
            geometriesReorderableList.onRemoveCallback += RemoveGMLGeometry;
            geometriesReorderableList.onReorderCallback += ReorderGMLGeometries;

            // Creating the geometry list
            geometries = new List<GMLGeometry>();
            // Set geometries list reference
            geometriesReorderableList.list = geometries;
            map.Geometries = geometries;
        }
        /* ----------------------------------
         * ON GUI: Used for drawing the window every unity event
         * ----------------------------------*/
        protected void OnGUI()
        {
            if (addressDropdown == null)
            {
                Init();
            }

            var prevAddress = address;
            address = addressDropdown.LayoutBegin();
            if (address != prevAddress)
            {
                timeSinceLastWrite = 0;
            }


            // Location control
            location = EditorGUILayout.Vector2Field("Location", location);
            var lastRect = GUILayoutUtility.GetLastRect();
            if (location != map.Center.ToVector2())
            {
                map.Center = new Vector2d(location.x, location.y);
            }

            GUILayout.BeginHorizontal();
            // Geometries control
            var geometriesWidth = 150;
            geometriesReorderableList.elementHeight = geometriesReorderableList.list.Count == 0 ? 20 : 70;
            var rect = GUILayoutUtility.GetRect(geometriesWidth, position.height - lastRect.y - lastRect.height);
            geometriesReorderableList.DoList(rect);

            // Map drawing
            map.selectedGeometry = geometriesReorderableList.index >= 0 ? geometries[geometriesReorderableList.index] : null;
            if (map.DrawMap(GUILayoutUtility.GetRect(position.width - geometriesWidth, position.height - lastRect.y - lastRect.height)))
            {
                Debug.Log(map.GeoMousePosition);
                if (editing != null)
                {
                    switch (editing.Type)
                    {
                        case GMLGeometry.GeometryType.Point:
                            if (editing.Points.Count == 1)
                            {
                                editing.Points[0] = map.GeoMousePosition;
                            }
                            else
                            {
                                editing.Points.Add(map.GeoMousePosition);
                            }

                            break;
                        case GMLGeometry.GeometryType.LineString:
                            editing.Points.Add(map.GeoMousePosition);
                            break;
                        case GMLGeometry.GeometryType.Polygon:
                            if (editing.Points.Count <= 1)
                            {
                                editing.Points.Add(map.GeoMousePosition);
                            }
                            else
                            {
                                // Find the closest index
                                var min = editing.Points.Min(p => (p - map.GeoMousePosition).magnitude);
                                var closest = editing.Points.FindIndex(p => (p - map.GeoMousePosition).magnitude == min);

                                // Fix the previous and next
                                var prev = closest == 0 ? editing.Points.Count - 1 : closest - 1;
                                var next = (closest + 1) % editing.Points.Count;
                                // Calculate the normal to both adjacent axis to closest point
                                var c = editing.Points[closest];
                                var v1 = (editing.Points[closest] - editing.Points[prev]).normalized;
                                var v2 = (editing.Points[closest] - editing.Points[next]).normalized;

                                var closestNormal = (v1 + v2).normalized;
                                var convex = Vector3.Cross(v1.ToVector2(), v2.ToVector2()).z > 0;

                                var mouseVector = (map.GeoMousePosition - c);
                                var left = Vector3.Cross(closestNormal.ToVector2(), mouseVector.ToVector2()).z > 0;

                                Debug.Log(convex ? "Convex" : "Concave");
                                if ((left && convex) || (!left && !convex))
                                {
                                    Debug.Log("Prev");
                                    // We insert at the closest
                                    editing.Points.Insert(closest, map.GeoMousePosition);
                                }
                                else
                                {
                                    Debug.Log("Next");
                                    // We insert at the next
                                    editing.Points.Insert(next, map.GeoMousePosition);
                                }
                            }
                            break;
                    }
                }
            }


            location = map.Center.ToVector2();
            geometriesReorderableList.index = map.selectedGeometry != null ? geometries.IndexOf(map.selectedGeometry) : -1;

            GUILayout.EndHorizontal();

            if (addressDropdown.LayoutEnd())
            {
                // If new Location is selected from the dropdown
                lastSearch = address = addressDropdown.Value;
                foreach (var l in place.DataStructure.dataChache)
                {
                    if (l.label == address)
                    {
                        location = l.coordinates;
                    }
                }

                var geometry = new GMLGeometry();
                geometry.Type = GMLGeometry.GeometryType.Polygon;

                var points = 5f;
                var radius = 0.00005;
                for (float i = 0; i < 5; i++)
                {
                    geometry.Points.Add(new Vector2d(location.x + radius * Mathf.Sin(i * 2f * Mathf.PI / points) * 1.33333f, location.y + radius * Mathf.Cos(i * 2f * Mathf.PI / points)));
                }


                geometries.Add(geometry);

                place.DataStructure.dataChache.Clear();
                Repaint();
            }
        }

        /* ------------------------------------------
         * Update: used for taking care of the http requests
         * ------------------------------------------ */
        void Update()
        {
            timeSinceLastWrite += Time.fixedDeltaTime;
            if (timeSinceLastWrite > 3f)
            {
                PerformSearch();
            }

            if (place.DataStructure.dataChache.Count > 0)
            {
                var addresses = new List<string>();
                foreach (var r in place.DataStructure.dataChache)
                {
                    addresses.Add(r.label);
                }

                addressDropdown.Elements = addresses;
                Repaint();
            }
        }

        /* ---------------------------------------
         * PerformSearch: Used to control the start of searches
         * --------------------------------------- */
        private void PerformSearch()
        {
            if (address != null && address.Trim() != "" && lastSearch != address)
            {
                place.namePlace = address;
                place.SearchInOSM();
                lastSearch = address;
            }
        }


        /* ------------------------------
         * OnDestroy: Used to deregister and destroy events
         * ------------------------------ */
        protected void OnDestroy()
        {
            EditorApplication.update -= this.Update;
        }

        /*----------------------------
         * GML GEOMETRY OPERATIONS
         *----------------------------*/

        private readonly Rect typePopupRect = new Rect(0, 2, 150, 15);
        private readonly Rect infoRect = new Rect(9, 20, 150, 15);
        private readonly Rect centerButtonRect = new Rect(0, 40, 75, 15);
        private readonly Rect editButtonRect = new Rect(75, 40, 75, 15);

        private void DrawGMLGeometryHeader(Rect rect)
        {
            GUI.Label(rect, "Geometries");
        }

        private void DrawGMLGeometry(Rect rect, int index, bool active, bool focused)
        {
            GMLGeometry geo = (GMLGeometry)geometriesReorderableList.list[index];

            EditorGUI.LabelField(infoRect.GUIAdapt(rect), "Points: " + geo.Points.Count);

            geo.Type = (GMLGeometry.GeometryType)EditorGUI.EnumPopup(typePopupRect.GUIAdapt(rect), geo.Type);

            if (GUI.Button(centerButtonRect.GUIAdapt(rect), "Center") && geo.Points.Count > 0)
            {
                location = geo.Points.Aggregate(new Vector2(), (p, n) => p + n.ToVector2()) / geo.Points.Count;
                map.Center = location.ToVector2d();
            }

            if (GUI.Button(editButtonRect.GUIAdapt(rect), editing != geo ? "Edit" : "Finish"))
            {
                editing = editing == geo ? null : geo;
            }
        }

        private void AddGMLGeometry(ReorderableList list)
        {
            geometries.Add(new GMLGeometry());
        }

        private void RemoveGMLGeometry(ReorderableList list)
        {
            geometries.RemoveAt(list.index);
        }

        private void ReorderGMLGeometries(ReorderableList list)
        {
            geometries = (List<GMLGeometry>)geometriesReorderableList.list;
        }

    }
}
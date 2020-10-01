using MapzenGo.Helpers.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapzenGo.Helpers;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;
using uAdventure.Core;
using uAdventure.Runner;

namespace uAdventure.Geo
{
    public class GeoController
    {
        private static GeoController instance;
        public static GeoController Instance {
            get { return instance ?? (instance = new GeoController()); }
        }

        private ChapterDataControl lastSelectedChapterDataControl;
        private ListDataControl<ChapterDataControl, GeoElementDataControl> geoElements;
        private ListDataControl<ChapterDataControl, MapSceneDataControl> mapScenes;


        public ListDataControl<ChapterDataControl, GeoElementDataControl> GeoElements
        {
            get
            {
                UpdateChapter();
                return geoElements;
            }
        }

        public ListDataControl<ChapterDataControl, MapSceneDataControl> MapScenes
        {
            get
            {
                UpdateChapter();
                return mapScenes;
            }
        }

        public int SelectedGeoElement { get; set; }
        public int SelectedMapScene { get; set; }
        

        private GeoController()
        {
            UpdateChapter();
        }

        private void UpdateChapter()
        {
            if (Controller.Instance.SelectedChapterDataControl != null && lastSelectedChapterDataControl != Controller.Instance.SelectedChapterDataControl)
            {
                // Map scenes list manages only map scenes
                mapScenes = new ListDataControl<ChapterDataControl, MapSceneDataControl>(
                    Controller.Instance.SelectedChapterDataControl,
                    Controller.Instance.SelectedChapterDataControl.getObjects<MapScene>(),
                    new ListDataControl<ChapterDataControl, MapSceneDataControl>.ElementFactoryView
                    {
                        Titles = { { 8923478, "Geo.Create.Title.MapScene" } },
                        DefaultIds = { { 8923478, "MapScene" } },
                        Errors = { { 8923478, "Geo.Create.Error.MapScene" } },
                        Messages = { { 8923478, "Geo.Create.Message.MapScene" } },
                        ElementFactory = new DefaultElementFactory<MapSceneDataControl>(
                            new DefaultElementFactory<MapSceneDataControl>.ElementCreator()
                            {
                                CreateDataControl = ms => new MapSceneDataControl(ms as MapScene),
                                CreateElement = (type, id, _) => new MapSceneDataControl(new MapScene(id)),
                                TypeDescriptors = new[]
                                {
                                    new DefaultElementFactory<MapSceneDataControl>.ElementCreator.TypeDescriptor
                                    {
                                        Type = 8923478,
                                        ContentType = typeof(MapScene),
                                        RequiresId = true
                                    }
                                }
                            })
                    });
                Controller.Instance.SelectedChapterDataControl.RegisterExtraDataControl(mapScenes);
                SelectedMapScene = -1;

                // Geo Elements list manages only geo elements
                geoElements = new ListDataControl<ChapterDataControl, GeoElementDataControl>(
                    Controller.Instance.SelectedChapterDataControl,
                    Controller.Instance.SelectedChapterDataControl.getObjects<GeoElement>(),
                    new ListDataControl<ChapterDataControl, GeoElementDataControl>.ElementFactoryView
                    {
                        Titles = { { 6493512, "Geo.Create.Title.GeoElement" } },
                        DefaultIds = { { 6493512, "GeoElement" } },
                        Errors = { { 6493512, "Geo.Create.Error.GeoElement" } },
                        Messages = { { 6493512, "Geo.Create.Message.GeoElement" } },
                        ElementFactory = new DefaultElementFactory<GeoElementDataControl>(
                            new DefaultElementFactory<GeoElementDataControl>.ElementCreator()
                            {
                                CreateDataControl = g => new GeoElementDataControl(g as GeoElement),
                                CreateElement = (type, id, extra) =>
                                {
                                    var geoElement = new GeoElement(id);
                                    var place = extra[0] as Place;
                                    if (place != null)
                                    {
                                        geoElement.Geometries[0].Type = GMLGeometry.GeometryType.Polygon;
                                        geoElement.Geometries[0].Points = place.RectBoundingBox.ToPoints();
                                    }
                                    return new GeoElementDataControl(geoElement);
                                },
                                TypeDescriptors = new[]
                                {
                                    new DefaultElementFactory<GeoElementDataControl>.ElementCreator.TypeDescriptor
                                    {
                                        Type = 6493512,
                                        ContentType = typeof(GeoElement),
                                        RequiresId = true,
                                        ExtraParameters = new Action<Action<object>> []
                                        {
                                            callback => { ScriptableObject.CreateInstance<PlaceInputDialog>().Init(sd => callback(sd)); }
                                        }
                                    }
                                }
                            })
                    });

                Controller.Instance.SelectedChapterDataControl.RegisterExtraDataControl(geoElements);
                SelectedGeoElement = -1;

                lastSelectedChapterDataControl = Controller.Instance.SelectedChapterDataControl;
                Controller.Instance.updateVarFlagSummary();
            }
        }

        public class PlaceInputDialog : EditorWindow
        {
            private bool hasToFocus = true;
            private PlaceSearcher placeSearcher;
            private static Place searchData;
            private Action<Place> callback;

            public virtual void Init(Action<Place> callback)
            {
                this.callback = callback;
                placeSearcher = ScriptableObject.CreateInstance<PlaceSearcher>();
                placeSearcher.Label = "Geo.PlaceInput.PlaceSearcher.Label".Traslate();
                placeSearcher.OnRequestRepaint = Repaint;

                if (searchData != null)
                {
                    placeSearcher.Place = searchData;
                }
                position = new Rect(Screen.width / 2 - 250, Screen.height / 2 - 150, 500, 300);
                ShowUtility();
            }

            protected void OnGUI()
            {
                this.wantsMouseMove = true;
                this.wantsMouseEnterLeaveWindow = true;
                EditorWindow.FocusWindowIfItsOpen<InputDialog>();

                // Message 
                GUI.SetNextControlName("InputField");
                placeSearcher.Source = GUILayout.Toolbar(placeSearcher.Source, placeSearcher.Sources);
                EditorGUILayout.LabelField("Geo.PlaceInput.Message".Traslate(), EditorStyles.boldLabel);
                GUILayout.Space(20);

                // Input field
                var isEnterPressed = IsEnterPressed();
                if (placeSearcher.DoLayout())
                {
                    searchData = placeSearcher.Place;
                    DestroyImmediate(placeSearcher);
                    callback(searchData);
                    this.Close();
                }

                if (GUI.GetNameOfFocusedControl() == "InputField" && isEnterPressed)
                {
                    searchData = placeSearcher.Place;
                    callback(searchData);
                    this.Close();
                }

                if (hasToFocus)
                {
                    hasToFocus = false;
                    GUI.FocusControl("InputField");
                    TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.hotControl);
                    te.SelectAll();
                    EditorGUIUtility.editingTextField = true;
                }

                // Bottom buttons
                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(TC.get("Geo.PlaceInput.WithoutLocation")))
                    {
                        searchData = null;
                        DestroyImmediate(placeSearcher);
                        callback(null);
                        this.Close();
                    }
                    
                    if (GUILayout.Button(TC.get("GeneralText.Cancel")))
                    {
                        DestroyImmediate(placeSearcher);
                        this.Close();
                    }
                }
            }

            private static bool IsEnterPressed()
            {
                return Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
            }
        }
    }
}

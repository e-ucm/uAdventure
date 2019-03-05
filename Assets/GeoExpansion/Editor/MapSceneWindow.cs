﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Editor;
using UnityEditorInternal;

using MapzenGo.Helpers.Search;
using UnityEditor;
using System.Linq;
using MapzenGo.Helpers;
using MapzenGo.Models.Settings.Editor;
using MoreLinq;

namespace uAdventure.Geo
{
    [EditorWindowExtension(110, typeof(MapScene))]
    public class MapSceneWindow : ReorderableListEditorWindowExtension
    {

        private int selectedElement;


        /* ---------------------------------
         * Attributes
         * -------------------------------- */

        private PlaceSearcher placeSearcher;
        private MapScene mapScene;

        /* ----------------------------------
         * GUI ELEMENTS
         * -----------------------------------*/
        private GUIMap map;
        private ReorderableList mapElementReorderableList;

        public MapSceneWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, new GUIContent("Map Scenes"), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/map"),
                text = "Map Scenes"
            };
            
            Init();
        }

        /* ----------------------------------
         * INIT: Used for late initialization after constructor
         * ----------------------------------*/
        void Init()
        {
            placeSearcher = new PlaceSearcher("Address");
            placeSearcher.OnRequestRepaint += Repaint;

            map = new GUIMap();
            map.Repaint += Repaint;
            map.Zoom = 19;

            mapElementReorderableList = new ReorderableList(new ArrayList(), typeof(MapElement), true, true, true, true);
            mapElementReorderableList.drawHeaderCallback += DrawMapElementsHeader;
            mapElementReorderableList.drawElementCallback += DrawMapElement;
            mapElementReorderableList.onAddDropdownCallback += OnAddMapElementDropdown;
            mapElementReorderableList.onRemoveCallback += RemoveMapElement;

            this.positionManagers = new Dictionary<ExtElemReference, ExtElemReferenceGUIMapPositionManager>();
        }
        /* ----------------------------------
      * ON GUI: Used for drawing the window every unity event
      * ----------------------------------*/
        public override void Draw(int aID)
        {
            if (mapScene == null)
            {
                GUILayout.Label("There is no selected element. Please select or create one.");
                return;
            }


            if (placeSearcher == null)
            {
                Init();
            }

            placeSearcher.LayoutBegin();

            // Location control
            mapScene.LatLon = EditorGUILayout.Vector2Field("Location", mapScene.LatLon.ToVector2()).ToVector2d();
            if (mapScene.LatLon != map.Center)
            {
                map.Center = mapScene.LatLon;
            }

            EditorGUILayout.BeginHorizontal();
            // Map Elements
            mapElementReorderableList.list = mapScene.Elements;
            var elementsWidth = 150;
            mapElementReorderableList.elementHeight = mapElementReorderableList.list.Count == 0 ? 20 : 70;
            EditorGUILayout.BeginVertical(GUILayout.Width(elementsWidth), GUILayout.ExpandHeight(true));
            mapElementReorderableList.DoLayoutList();
            EditorGUILayout.EndVertical();

            var mapRect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            // Map drawing
            if (map.DrawMap(mapRect) && movingReference != null)
            {
                this.positionManagers[movingReference].Repositionate(map, mapRect);
                movingReference = null;
            }

            if (movingReference != null && Event.current.type == EventType.Repaint)
            {
                this.positionManagers[movingReference].Repositionate(map, mapRect);
            }

            mapScene.LatLon = map.Center;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (placeSearcher.LayoutEnd())
            {
                mapScene.LatLon = placeSearcher.LatLon;
                Repaint();
            }
        }

        private List<DataControlWithResources> extElems;
        private void UpdateMapResources()
        {
            extElems = getAllElements();
            // Put the elements into the map
            map.Geometries = mapScene.Elements
                .FindAll(e => e is GeoReference)
                .ConvertAll(e => FindGeoElem(e.getTargetId()).Geometry);
            var allElements = mapScene.Elements
                .FindAll(e => e is ExtElemReference)
                .ConvertAll(e => e as ExtElemReference);

            positionManagers.Clear();
            foreach (var elem in allElements)
            {
                // Create the positionManager based on the descriptor
                var guiMapPositionManager = ExtElemReferenceGUIMapPositionManagerFactory.Instance.CreateInstance(elem.TransformManagerDescriptor, elem);
                positionManagers.Add(elem, guiMapPositionManager);

                // Look for the texture
                var extElem = FindExtElem(elem.getTargetId());
                var previewImage = extElem.GetType().GetMethod("getPreviewImage");
                if (previewImage != null)
                {
                    var image = previewImage.Invoke(extElem, null) as string;
                    if (image != null)
                    {
                        guiMapPositionManager.Texture = Controller.ResourceManager.getImage(image);
                    }
                }
            }

            // Update the positioned resources
            map.PositionedResources = positionManagers.Values.ToList();

        }

        private Dictionary<ExtElemReference, ExtElemReferenceGUIMapPositionManager> positionManagers;

        private GeoElement FindGeoElem(string id)
        {
            return Controller.Instance.SelectedChapterDataControl.getObjects<GeoElement>().Find(e => e.Id == id);
        }

        private DataControlWithResources FindExtElem(string id)
        {
            return extElems.Find(ext => id == (ext.GetType().GetMethod("getId").Invoke(ext, null) as string));
        }

        // -----------------------------
        //  MapScenes management
        // -----------------------------
        protected override void OnAdd(ReorderableList r)
        {
            Controller.Instance.SelectedChapterDataControl.getObjects<MapScene>().Add(new MapScene("newMapscene"));
        }

        protected override void OnAddOption(ReorderableList r, string option) { }

        protected override void OnButton()
        {
            reorderableList.index = -1;
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.Instance.SelectedChapterDataControl.getObjects<MapScene>()[index].Id = newName;
        }

        protected override void OnRemove(ReorderableList r)
        {
            Controller.Instance.SelectedChapterDataControl.getObjects<MapScene>().RemoveAt(r.index);
        }

        protected override void OnReorder(ReorderableList r) 
		{ 
			string idToMove = r.list [r.index] as string;
			var temp = Controller.Instance .SelectedChapterDataControl .getObjects<MapScene> ();
			MapScene toMove = temp.Find (map => map.getId () == idToMove);
			temp.Remove (toMove);
			temp.Insert (r.index, toMove);
		}

        protected override void OnSelect(ReorderableList r)
        {
            selectedElement = r.index;

            if (selectedElement != -1)
            {
                mapScene = Controller.Instance.SelectedChapterDataControl.getObjects<MapScene>()[selectedElement];
                // Set geometries list reference
                mapElementReorderableList.list = mapScene.Elements;
                // Update map resources
                UpdateMapResources();
            }
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            r.list = Controller.Instance.SelectedChapterDataControl.getObjects<MapScene>().ConvertAll(s => s.Id);
        }


        // -----------------------------
        //  MapScene elements management
        // -----------------------------

        private readonly Rect typePopupRect = new Rect(0, 2, 110, 15);
        private readonly Rect typeConfigRect = new Rect(111, 2, 15, 15);
        private readonly Rect infoRect = new Rect(9, 20, 150, 15);
        private readonly Rect centerButtonRect = new Rect(0, 40, 75, 15);
        private readonly Rect editButtonRect = new Rect(75, 40, 75, 15);

        private void DrawMapElementsHeader(Rect rect)
        {
            GUI.Label(rect, "Geometries");
        }

        private ExtElemReference movingReference = null;
        private void DrawMapElement(Rect rect, int index, bool active, bool focused)
        {
            MapElement mapElement = (MapElement)mapElementReorderableList.list[index];

            EditorGUI.LabelField(infoRect.GUIAdapt(rect), mapElement.getTargetId());
            
            if (mapElement is ExtElemReference)
            {
                var extReference = mapElement as ExtElemReference;

                // Trasnform manager descriptor selection
                var avaliableDescriptors = TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.ToList();
                var selected = avaliableDescriptors.FindIndex(d => d.Key == extReference.TransformManagerDescriptor.GetType());
                var newSelected = EditorGUI.Popup(typePopupRect.GUIAdapt(rect), selected, avaliableDescriptors.ConvertAll(d => d.Value).ToArray());

                if (newSelected != selected)
                {
                    // In case of change, reinstance a new one
                    extReference.TransformManagerDescriptor = TransformManagerDescriptorFactory.Instance.CreateDescriptor(avaliableDescriptors[newSelected].Key);
                    UpdateMapResources();
                }

                using (new EditorGUI.DisabledScope())
                {
                    if (GUI.Button(typeConfigRect.GUIAdapt(rect), "*"))
                    {
                        var o = DrawerParametersMenu.ShowAtPosition(typeConfigRect.GUIAdapt(rect));
                        DrawerParametersMenu.s_DrawerParametersMenu.ExtElemReference = extReference;
                        if (o)
                        {
                            GUIUtility.ExitGUI();
                        }
                    }
                }

                if (GUI.Button(centerButtonRect.GUIAdapt(rect), "Move"))
                {
                    movingReference = extReference;
                }
            }

            if (GUI.Button(editButtonRect.GUIAdapt(rect), "Conditions"))
            {
                ConditionEditorWindow window =
                    (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                window.Init(mapElement.Conditions);
            }
        }

        public List<DataControlWithResources> getAllElements()
        {
            var all = new List<DataControlWithResources>();

            all.AddRange(Controller.Instance.SelectedChapterDataControl.getItemsList().getItems().ConvertAll(i => i as DataControlWithResources));
            all.AddRange(Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList().ConvertAll(i => i as DataControlWithResources));
            all.AddRange(Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs().ConvertAll(i => i as DataControlWithResources));

            return all;
        }

        private Dictionary<string, string> getObjectIDReferences()
        {
            Dictionary<string, string> objects = new Dictionary<string, string>();
            Controller.Instance.IdentifierSummary.getIds<Core.Item>().ForEach(i => objects.Add("Item/" + i, i));
            Controller.Instance.IdentifierSummary.getIds<Core.Atrezzo>().ForEach(a => objects.Add("Atrezzo/" + a, a));
            Controller.Instance.IdentifierSummary.getIds<Core.NPC>().ForEach(c => objects.Add("Character/" + c, c));

            return objects;
        }

        protected void OnAddMapElementDropdown(Rect r, ReorderableList rl)
        {
            var menu = new GenericMenu();

            var mapElements = Controller.Instance.SelectedChapterDataControl.getObjects<GeoElement>();
            mapElements.ForEach(me =>
            {
                menu.AddItem(new GUIContent("GeoElement/" + me.Id), false, (id) =>
                {
                    mapScene.Elements.Add(new GeoReference(id as string));
                    UpdateMapResources();
                }, me.Id);
            });

            foreach (var pair in getObjectIDReferences())
            {
                menu.AddItem(new GUIContent(pair.Key), false, (id) =>
                {
                    mapScene.Elements.Add(new ExtElemReference(id as string));
                    UpdateMapResources();
                }, pair.Value);
            }
            menu.ShowAsContext();
        }

        private void RemoveMapElement(ReorderableList list)
        {
            mapScene.Elements.RemoveAt(list.index);
        }
    }
}
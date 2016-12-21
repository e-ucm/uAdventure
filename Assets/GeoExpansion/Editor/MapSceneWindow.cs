using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Editor;
using UnityEditorInternal;

using uAdventure.Geo;
using MapzenGo.Helpers.Search;
using UnityEditor;
using System.Linq;
using MapzenGo.Helpers;
using MapzenGo.Models.Settings.Editor;
using System;

public class MapSceneWindow : ReorderableListEditorWindowExtension {

    private int selectedElement;

    const string PATH_SAVE_SCRIPTABLE_OBJECT = "Assets/MapzenGo/Resources/Settings/";

    /* ---------------------------------
     * Attributes
     * -------------------------------- */

    private SearchPlace place;
    private string address = "";
    private Vector2 location;
    private string lastSearch = "";
    private float timeSinceLastWrite;
    private MapScene mapScene;
    private Rect mm_Rect;

    /* ----------------------------------
     * GUI ELEMENTS
     * -----------------------------------*/
    private DropDown addressDropdown;
    private GUIMap map;
    private ReorderableList mapElementReorderableList;

    public MapSceneWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
    {
        mm_Rect = aStartPos;

        var bc = new GUIContent();
        bc.image = (Texture2D)Resources.Load("EAdventureData/img/icons/map", typeof(Texture2D));
        bc.text = "MapScenes";  //TC.get("Element.Name1");
        ButtonContent = bc;

        // Get existing open window or if none, make a new one:
        place = UnityEngine.Object.FindObjectOfType<SearchPlace>();
        if (place == null)
        {
            SearchPlace search = new GameObject("Searcher").AddComponent<SearchPlace>();
            place = search;
        }

        Init();
    }

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

        mapElementReorderableList = new ReorderableList(new ArrayList(), typeof(MapElement), true, true, true, true);
        mapElementReorderableList.drawHeaderCallback += DrawMapElementsHeader;
        mapElementReorderableList.drawElementCallback += DrawMapElement;
        mapElementReorderableList.onAddDropdownCallback += OnAddMapElementDropdown;
        mapElementReorderableList.onRemoveCallback += RemoveMapElement;

    }
    /* ----------------------------------
  * ON GUI: Used for drawing the window every unity event
  * ----------------------------------*/
    public override void Draw(int aID)
    {

      
        if (addressDropdown == null)
            Init();

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
            map.Center = new Vector2d(location.x, location.y);

        GUILayout.BeginHorizontal();
        // Geometries control
        var geometriesWidth = 150;
        mapElementReorderableList.elementHeight = mapElementReorderableList.list.Count == 0 ? 20 : 70;
        var rect = GUILayoutUtility.GetRect(geometriesWidth, mm_Rect.height - lastRect.y - lastRect.height);
        mapElementReorderableList.DoList(rect);

        // Map drawing
        if (map.DrawMap(GUILayoutUtility.GetRect(mm_Rect.width - geometriesWidth, mm_Rect.height - lastRect.y - lastRect.height)))
        {

        }


        location = map.Center.ToVector2();
        //geometriesReorderableList.index = map.selectedGeometry != null ? geometries.IndexOf(map.selectedGeometry) : -1;

        GUILayout.EndHorizontal();

        if (addressDropdown.LayoutEnd())
        {
            // If new Location is selected from the dropdown
            lastSearch = address = addressDropdown.Value;
            foreach (var l in place.DataStructure.dataChache)
                if (l.label == address)
                    location = l.coordinates;
            

            place.DataStructure.dataChache.Clear();
            Repaint();
        }
    }

    // -----------------------------
    //  MapScenes management
    // -----------------------------
    protected override void OnAdd(ReorderableList r)
    {
        Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>().Add(new MapScene("newMapscene"));
    }

    protected override void OnAddOption(ReorderableList r, string option){}

    protected override void OnButton()
    {
        reorderableList.index = -1;
    }

    protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
    {
        Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>()[index].Id = newName;
    }

    protected override void OnRemove(ReorderableList r)
    {
        Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>().RemoveAt(r.index);
    }

    protected override void OnReorder(ReorderableList r)
    {
    }

    protected override void OnSelect(ReorderableList r)
    {
        selectedElement = r.index;

        if(selectedElement != -1)
        {
            mapScene = Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>()[selectedElement];
            // Set geometries list reference
            mapElementReorderableList.list = mapScene.Elements;
        }
    }

    protected override void OnUpdateList(ReorderableList r)
    {
        r.list = Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>().ConvertAll(s => s.Id);
    }

    /* ------------------------------------------
     * Update: used for taking care of the http requests
     * ------------------------------------------ */
    void Update()
    {
        //Debug.Log(Time.fixedDeltaTime);
        timeSinceLastWrite += Time.fixedDeltaTime;
        if (timeSinceLastWrite > 3f)
        {
            PerformSearch();
        }

        if (place.DataStructure.dataChache.Count > 0)
        {
            var addresses = new List<string>();
            foreach (var r in place.DataStructure.dataChache)
                addresses.Add(r.label);
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
            place.SearchInMapzen();
            lastSearch = address;
        }
    }


    // -----------------------------
    //  MapScene elements management
    // -----------------------------

    private void OnAddElement()
    {

    }

    private void OnDrawMapElementsHeader(Rect rect)
    {

    }

    private void OnDrawMapElement(Rect rect, int index, int a, int b)
    {

    }

    Rect typePopupRect = new Rect(0, 2, 150, 15);
    Rect infoRect = new Rect(9, 20, 150, 15);
    Rect centerButtonRect = new Rect(0, 40, 75, 15);
    Rect editButtonRect = new Rect(75, 40, 75, 15);

    private void DrawMapElementsHeader(Rect rect)
    {
        GUI.Label(rect, "Geometries");
    }

    private void DrawMapElement(Rect rect, int index, bool active, bool focused)
    {
        MapElement mapElement = (MapElement)mapElementReorderableList.list[index];

        EditorGUI.LabelField(infoRect.GUIAdapt(rect), mapElement.getTargetId());

        //geo.Type = (GMLGeometry.GeometryType)EditorGUI.EnumPopup(typePopupRect.GUIAdapt(rect), geo.Type);
        var center = map.Center;
        if (mapElement is GeoReference)
        {
            var geoReference = mapElement as GeoReference;
            var geoElement = Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>().Find(e => e.Id == geoReference.getTargetId());
            if (geoElement != null && geoElement.Geometry.Points.Count > 0)
            {
                center = geoElement.Geometry.Center;
            }
        }
        else if (mapElement is ExtElemReference)
        {
            var extReference = mapElement as ExtElemReference;
            var extElement = findExternalReferenceById(extReference.getTargetId());
            if (extElement != null && extReference != null)
            {
                center = extReference.Position.ToVector2d();
            }
        }

        if (GUI.Button(centerButtonRect.GUIAdapt(rect), "Center"))
        {
            map.Center = center;
        }

        /*if (GUI.Button(editButtonRect.GUIAdapt(rect), editing != geo ? "Unlocked" : "Locked"))
        {
            editing = editing == geo ? null : geo;
        }*/

        if (GUI.Button(editButtonRect.GUIAdapt(rect), "Conditions"))
        {
            ConditionEditorWindow window =
                (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
            window.Init(mapElement.Conditions);
        }
    }

    private object findExternalReferenceById(string id)
    {
        // TODO extend here
        var item = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems().Find(i => i.getId() == id);
        if (item != null)
            return item;

        var atrezzo = Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList().Find(a => a.getId() == id);
        if (atrezzo != null)
            return atrezzo;

        var npc = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPC(id);
        if (npc != null)
            return npc;

        return null;
    }

    private Dictionary<string, object> getObjectReferences()
    {
        Dictionary<string, object> objects = new Dictionary<string, object>();
        // TODO extend here
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems().ForEach(i => objects.Add("Item/" + i.getId(), i));
        Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList().ForEach(a => objects.Add("Atrezzo/" + a.getId(), a));
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs().ForEach(npc => objects.Add("Character/"+ npc.getId(), npc));

        return null;
    }

    protected void OnAddMapElementDropdown(Rect r, ReorderableList rl)
    {
        var menu = new GenericMenu();

        var mapElements = Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>();
        mapElements.ForEach(me =>
        {
            menu.AddItem(new GUIContent("GeoElement/"+me.Id), false, (elem) => mapScene.Elements.Add(elem as MapElement), me);
        });

        foreach(var pair in getObjectReferences())
        {
            menu.AddItem(new GUIContent(pair.Key), false, (elem) => mapScene.Elements.Add(elem as MapElement), pair.Value);
        }
        menu.ShowAsContext();
    }

    private void RemoveMapElement(ReorderableList list)
    {
        mapScene.Elements.RemoveAt(list.index);
    }
}

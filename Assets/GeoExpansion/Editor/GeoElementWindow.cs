using UnityEngine;
using System.Collections;
using System;
using UnityEditorInternal;
using UnityEditor;
using System.Linq;
using MapzenGo.Helpers;
using uAdventure.Geo;
using System.Collections.Generic;
using MapzenGo.Helpers.Search;
using MapzenGo.Models.Settings.Editor;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Geo
{

    [EditorWindowExtension(115, typeof(GeoElement))]
    public class GeoElementWindow : ReorderableListEditorWindowExtension, DialogReceiverInterface
    {
        private int selectedElement;

        const string PATH_SAVE_SCRIPTABLE_OBJECT = "Assets/MapzenGo/Resources/Settings/";

        /* ---------------------------------
         * Attributes
         * -------------------------------- */
         
        private Vector2 location;
        private bool editing;
        private GeoElement element;
        private Rect mm_Rect;
        private string[] menus;
        private Texture2D imagePreview;

        /* ----------------------------------
         * GUI ELEMENTS
         * -----------------------------------*/
        private PlaceSearcher placeSearcher;
        private GUIMap map;
        private ReorderableList actionsList;

        public GeoElementWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, new GUIContent("Geo Elements"), aStyle, aOptions)
        {
            mm_Rect = aStartPos;

            var bc = new GUIContent();
            bc.image = (Texture2D)Resources.Load("EAdventureData/img/icons/poi", typeof(Texture2D));
            bc.text = "GeoElements";  //TC.get("Element.Name1");
            ButtonContent = bc;
            
            menus = new string[] { "Position", "Attributes", "Actions" };

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

            actionsList = new ReorderableList(new List<GeoAction>(), typeof(GeoAction));
            actionsList.headerHeight = 40;
            actionsList.elementHeight = 60;
            actionsList.drawHeaderCallback = DrawActionsHeader;
            actionsList.drawElementCallback = DrawActionElement;
            actionsList.onAddDropdownCallback = OnAddActionDropdown;
        }

        private void DrawActionsHeader(Rect r)
        {
            GUILayout.BeginArea(new Rect(r.x, r.y, r.width, r.height / 2f));
            GUILayout.Label("Actions");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(r.x, r.y + (r.height / 2f), r.width, r.height / 2f), "", "toolbar");
            GUILayout.BeginHorizontal(); 
            GUILayout.Label("Name", GUILayout.Width(r.width * 0.2f));
            GUILayout.Label("Parameters", GUILayout.Width(r.width * 0.6f));
            GUILayout.Label("C&E", GUILayout.Width(r.width * 0.2f));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawActionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            GUILayout.BeginArea(rect);

            var action = actionsList.list[index] as GeoAction;
            GUILayout.BeginHorizontal();
            GUILayout.Label(action.Name, GUILayout.MaxWidth(rect.width * 0.2f));

            //Action Parameters
            GUILayout.BeginVertical(GUILayout.MaxWidth(rect.width * 0.6f));
            
            if(action is EnterAction)
            {
                var specialized = action as EnterAction;
                specialized.OnlyFromOutside = EditorGUILayout.Toggle("Only from outside", specialized.OnlyFromOutside);
            }
            if (action is ExitAction)
            {
                var specialized = action as ExitAction;
                specialized.OnlyFromInside = EditorGUILayout.Toggle("Only from inside", specialized.OnlyFromInside);

            }
            if (action is InspectAction)
            {
                var specialized = action as InspectAction;
                specialized.Inside = EditorGUILayout.Toggle("On range", specialized.Inside);

            }
            if (action is LookToAction)
            {
                var specialized = action as LookToAction;
                specialized.Inside = EditorGUILayout.Toggle("On range", specialized.Inside);
                specialized.Center = EditorGUILayout.Toggle("Look to center", specialized.Center);
                if(!specialized.Center)
                    specialized.Direction = EditorGUILayout.Vector2Field("Look to direction", specialized.Direction);
            }

            GUILayout.EndVertical();

            // Conditions and effects
            GUILayout.BeginVertical(GUILayout.MaxWidth(rect.width * 0.2f));
            if (GUILayout.Button("Conditions"))
            {
                ConditionEditorWindow window =
                    (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                window.Init(action.Conditions);
            }
            if(GUILayout.Button("Effects")){

                EffectEditorWindow window =
                    (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                window.Init(action.Effects);
            }
            GUILayout.EndVertical();

            // End eleemnt
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        protected virtual void OnAddActionDropdown(Rect r, ReorderableList rl)
        {
            var menu = new GenericMenu();
            foreach(var a in GeoActionsFactory.Instance.AvaliableActions)
                menu.AddItem(new GUIContent(a.Value), false, (t) => element.Actions.Add(GeoActionsFactory.Instance.CreateActionFor(t as Type)), a.Key);
            menu.ShowAsContext();
        }

        /* ----------------------------------
          * ON GUI: Used for drawing the window every unity event
          * ----------------------------------*/
        int selected = 0;
        public override void Draw(int aID)
        {

            if (selectedElement == -1)
            {
                GUILayout.Label("Nothing selected", GUILayout.Width(mm_Rect.width), GUILayout.Height(mm_Rect.height));
                return;
            }

            element = Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>()[selectedElement];


            actionsList.list = element.Actions;
            // Set geometries list reference
            map.Geometries = new List<GMLGeometry>() { element.Geometry };

            selected = GUILayout.Toolbar(selected, menus);

            switch (selected)
            {
                case 0: // Map view
                    {
                        element.Geometry.Type = (GMLGeometry.GeometryType)EditorGUILayout.EnumPopup("Geometry type", element.Geometry.Type);
                        EditorGUILayout.LabelField("Points: " + element.Geometry.Points.Count);
                        element.Geometry.Influence = EditorGUILayout.FloatField("Influence Radius", element.Geometry.Influence);


                        if (GUILayout.Button("Center") && element.Geometry.Points.Count > 0)
                        {
                            Center(element);
                        }

                        if (GUILayout.Button(!editing ? "Edit" : "Finish"))
                        {
                            editing = !editing;
                        }
                        
                        EditorGUILayout.Separator();
                        placeSearcher.LayoutBegin();

                        // Location control
                        location = EditorGUILayout.Vector2Field("Location", location);
                        var lastRect = GUILayoutUtility.GetLastRect();
                        if (location != map.Center.ToVector2())
                            map.Center = new Vector2d(location.x, location.y);


                        // Map drawing
                        if(editing)
                            map.selectedGeometry = element.Geometry;

                        if (map.DrawMap(GUILayoutUtility.GetRect(mm_Rect.width, mm_Rect.height - lastRect.y - lastRect.height)))
                        {
                            Debug.Log(map.GeoMousePosition);
                            if (element != null && editing)
                            {
                                element.Geometry.AddPoint(map.GeoMousePosition);
                            }
                        }
                        
                        location = map.Center.ToVector2();
                        

                        if (placeSearcher.LayoutEnd())
                        {
                            // If new Location is selected from the dropdown
                            location = placeSearcher.LatLon.ToVector2();
                            Repaint();
                        }
                    } break;
                case 1:
                    {
                        GUILayout.Label("Full description");
                        element.FullDescription = GUILayout.TextArea(element.FullDescription, GUILayout.Height(250));
                        
                        element.Name = EditorGUILayout.TextField("Name", element.Name);
                        element.BriefDescription = EditorGUILayout.TextField("Brief description", element.BriefDescription);
                        element.DetailedDescription = EditorGUILayout.TextField("Detailed description", element.DetailedDescription);
                        
                        GUILayout.Label("Element image");
                        GUILayout.BeginHorizontal();
                       /* if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
                        {
                            foregroundMaskPath = "";
                        }*/
                        GUILayout.Box(element.Image, GUILayout.Width(0.78f * m_Rect.width));
                        if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.2f * m_Rect.width)))
                        {
                            ShowAssetChooser(AssetType.Image);
                        }
                        GUILayout.EndHorizontal();
                    }
                    break;
                case 2:
                    {
                        actionsList.list = element.Actions;
                        actionsList.DoList(new Rect(0, 50, m_Rect.width * 0.99f, m_Rect.height));
                    }
                    break;
            }

            
        }

        protected override void OnAdd(ReorderableList r)
        {
            Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>().Add(new GeoElement("newGeoElement"));
        }

        protected override void OnAddOption(ReorderableList r, string option) { }

        protected override void OnButton()
        {
            reorderableList.index = -1;
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>()[index].Id = newName;
        }

        protected override void OnRemove(ReorderableList r)
        {
            Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>().RemoveAt(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
        }

        protected override void OnSelect(ReorderableList r)
        {
            selectedElement = r.index;
            if(r.index >= 0 )
                Center(Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>()[selectedElement]);
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            r.list = Controller.getInstance().getSelectedChapterDataControl().getObjects<GeoElement>().ConvertAll(s => s.Id);
        }

        private void Center(GeoElement element)
        {
            if(element.Geometry.Points.Count > 0)
            {
                map.Center = element.Geometry.Center;
                location = map.Center.ToVector2();
            }

        }

        enum AssetType
        {
            Image
        }

        // --------------------------
        // Dialog methods: show and receive
        // --------------------------
        void ShowAssetChooser(AssetType type)
        {
            switch (type)
            {
                case AssetType.Image:
                    ImageFileOpenDialog backgroundDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                    backgroundDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_IMAGE);
                    break;
            }

        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            switch ((BaseFileOpenDialog.FileType)workingObject)
            {
                case BaseFileOpenDialog.FileType.ITEM_IMAGE:
                    element.Image = message;
                    Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                       GameRources.GetInstance().selectedSceneIndex].setPreviewBackground(message);
                    if (element.Image != null && !element.Image.Equals(""))
                        imagePreview = AssetsController.getImage(element.Image).texture;
                    break;
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log("Canceled");
        }
    }


}


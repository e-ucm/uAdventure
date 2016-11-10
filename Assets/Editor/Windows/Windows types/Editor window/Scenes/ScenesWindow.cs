using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ScenesWindow : LayoutWindow
{
    private enum ScenesWindowType
    {
        ActiveAreas,
        Appearance,
        Documentation,
        ElementRefrence,
        Exits, 
        Barriers,
        PlayerMovement
    }

    private static ScenesWindowType openedWindow = ScenesWindowType.Appearance;
    private static ScenesWindowActiveAreas scenesWindowActiveAreas;
    private static ScenesWindowAppearance scenesWindowAppearance;
    private static ScenesWindowDocumentation scenesWindowDocumentation;
    private static ScenesWindowElementReference scenesWindowElementReference;
    private static ScenesWindowExits scenesWindowExits;
    private static ScenesWindowBarriers scenesWindowBarriers;
    private static ScenesWindowPlayerMovement scenesWindowPlayerMovement;

    private static float windowWidth, windowHeight;
    private static List<bool> toggleList;

    private static Rect thisRect;

    // Flag determining visibility of concrete item information
    private bool isConcreteItemVisible = false;

    private static GUISkin selectedButtonSkin;
    private static GUISkin defaultSkin;

    public ScenesWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        thisRect = aStartPos;
        scenesWindowActiveAreas = new ScenesWindowActiveAreas(aStartPos,
            new GUIContent(TC.get("ActiveAreasList.Title")), "Window");
        scenesWindowAppearance = new ScenesWindowAppearance(aStartPos, new GUIContent(TC.get("Scene.LookPanelTitle")),
            "Window");
        scenesWindowDocumentation = new ScenesWindowDocumentation(aStartPos,
            new GUIContent(TC.get("Scene.DocPanelTitle")), "Window");
        scenesWindowElementReference = new ScenesWindowElementReference(aStartPos,
            new GUIContent(TC.get("ItemReferencesList.Title")), "Window");
        scenesWindowExits = new ScenesWindowExits(aStartPos, new GUIContent(TC.get("Element.Name3")), "Window");

        scenesWindowBarriers = new ScenesWindowBarriers(aStartPos, new GUIContent(TC.get("BarriersList.Title")), "Window");
        scenesWindowPlayerMovement = new ScenesWindowPlayerMovement(aStartPos, new GUIContent(TC.get("Trajectory.Title")), "Window");


        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));

        GenerateToggleList();
    }


    public override void Draw(int aID)
    {
        // Show information of concrete item
        if (isConcreteItemVisible)
        {
            /**
            UPPER MENU
            */
            GUILayout.BeginHorizontal();
            if (openedWindow == ScenesWindowType.Appearance)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Scene.LookPanelTitle")))
            {
                OnWindowTypeChanged(ScenesWindowType.Appearance);
            }
            if (openedWindow == ScenesWindowType.Appearance)
                GUI.skin = defaultSkin;

            if (openedWindow == ScenesWindowType.Documentation)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Scene.DocPanelTitle")))
            {
                OnWindowTypeChanged(ScenesWindowType.Documentation);
            }
            if (openedWindow == ScenesWindowType.Documentation)
                GUI.skin = defaultSkin;

            if (openedWindow == ScenesWindowType.ElementRefrence)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("ItemReferencesList.Title")))
            {
                OnWindowTypeChanged(ScenesWindowType.ElementRefrence);
            }
            if (openedWindow == ScenesWindowType.ElementRefrence)
                GUI.skin = defaultSkin;

            if (openedWindow == ScenesWindowType.ActiveAreas)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("ActiveAreasList.Title")))
            {
                OnWindowTypeChanged(ScenesWindowType.ActiveAreas);
            }
            if (openedWindow == ScenesWindowType.ActiveAreas)
                GUI.skin = defaultSkin;

            if (openedWindow == ScenesWindowType.Exits)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Element.Name3")))
            {
                OnWindowTypeChanged(ScenesWindowType.Exits);
            }
            if (openedWindow == ScenesWindowType.Exits)
                GUI.skin = defaultSkin;
            // Only visible for 3rd person
            if (Controller.getInstance().playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                if (openedWindow == ScenesWindowType.Barriers)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("BarriersList.Title")))
                {
                    OnWindowTypeChanged(ScenesWindowType.Barriers);
                }
                if (openedWindow == ScenesWindowType.Barriers)
                    GUI.skin = defaultSkin;

                if (openedWindow == ScenesWindowType.PlayerMovement)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Trajectory.Title")))
                {
                    OnWindowTypeChanged(ScenesWindowType.PlayerMovement);
                }
                if (openedWindow == ScenesWindowType.PlayerMovement)
                    GUI.skin = defaultSkin;
            }
            GUILayout.EndHorizontal();

            switch (openedWindow)
            {
                case ScenesWindowType.ActiveAreas:
                    scenesWindowActiveAreas.Draw(aID);
                    break;
                case ScenesWindowType.Appearance:
                    scenesWindowAppearance.Draw(aID);
                    break;
                case ScenesWindowType.Documentation:
                    scenesWindowDocumentation.Draw(aID);
                    break;
                case ScenesWindowType.ElementRefrence:
                    scenesWindowElementReference.Draw(aID);
                    break;
                case ScenesWindowType.Exits:
                    scenesWindowExits.Draw(aID);
                    break;
                case ScenesWindowType.Barriers:
                    scenesWindowBarriers.Draw(aID);
                    break;
                case ScenesWindowType.PlayerMovement:
                    scenesWindowPlayerMovement.Draw(aID);
                    break;
            }
        }
        // Show information of whole scenes (global-scene view)
        else
        {
            //GUILayout.Label(TC.get("SCENES"));

            //GUILayout.BeginHorizontal();
            //GUILayout.Box(TC.get("SHOW_?"), GUILayout.MaxWidth(windowWidth*0.2f));
            //GUILayout.Box(TC.get("SCENE_ID"), GUILayout.Width(windowWidth*0.55f));
            //GUILayout.Box(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth*0.2f));
            //GUILayout.EndHorizontal();

            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count;
                i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes()[i].getId(),
                    GUILayout.Width(windowWidth*0.65f));
                if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth*0.3f)))
                {
                    ShowItemWindowView(i);
                }

                GUILayout.EndHorizontal();

            }

        }
    }

    void OnWindowTypeChanged(ScenesWindowType type_)
    {
        openedWindow = type_;
    }


    // Two methods responsible for showing right window content 
    // - concrete item info or base window view
    public void ShowBaseWindowView()
    {
        isConcreteItemVisible = false;
        GameRources.GetInstance().selectedSceneIndex = -1;
        GenerateToggleList();
    }

    public void ShowItemWindowView(int s)
    {
        GameRources.GetInstance().selectedSceneIndex = s;
        isConcreteItemVisible = true;
        // Generate new toogle list - maybe user already created new scenes?
        GenerateToggleList();

        // Reload windows for newly selected scene
        scenesWindowActiveAreas = new ScenesWindowActiveAreas(thisRect, new GUIContent(TC.get("ActiveAreasList.Title")),
            "Window");
        scenesWindowAppearance = new ScenesWindowAppearance(thisRect, new GUIContent(TC.get("Scene.LookPanelTitle")),
            "Window");
        scenesWindowDocumentation = new ScenesWindowDocumentation(thisRect,
            new GUIContent(TC.get("Scene.DocPanelTitle")), "Window");
        scenesWindowElementReference = new ScenesWindowElementReference(thisRect,
            new GUIContent(TC.get("ItemReferencesList.Title")), "Window");
        scenesWindowExits = new ScenesWindowExits(thisRect, new GUIContent(TC.get("Element.Name3")), "Window");

        // Only visible for 3rd person
        if (Controller.getInstance().playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
        {
            scenesWindowBarriers = new ScenesWindowBarriers(thisRect, new GUIContent(TC.get("BarriersList.Title")), "Window");
            scenesWindowPlayerMovement = new ScenesWindowPlayerMovement(thisRect, new GUIContent(TC.get("Trajectory.Title")), "Window");
        }
    }

    void GenerateToggleList()
    {
        toggleList =
            new List<bool>(Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count);
        for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count; i++)
            toggleList.Add(true);
    }
}
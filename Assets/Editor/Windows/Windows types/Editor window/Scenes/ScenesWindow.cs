using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using UnityEditorInternal;

namespace uAdventure.Editor
{
    [EditorWindowExtension(10, typeof(Scene))]
    public class ScenesWindow : ReorderableListEditorWindowExtension
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
        
        private static List<bool> toggleList;
        
        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public ScenesWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, new GUIContent(TC.get("Element.Name1")), style, options)
        {
            var content = new GUIContent();

            // Button
            content.image = (Texture2D) Resources.Load("EAdventureData/img/icons/scenes", typeof(Texture2D));
            content.text = TC.get("Element.Name1");
            ButtonContent = content;

            // Windows
            scenesWindowActiveAreas = new ScenesWindowActiveAreas(rect,
                new GUIContent(TC.get("ActiveAreasList.Title")), "Window");
            scenesWindowAppearance = new ScenesWindowAppearance(rect, new GUIContent(TC.get("Scene.LookPanelTitle")),
                "Window");
            scenesWindowDocumentation = new ScenesWindowDocumentation(rect,
                new GUIContent(TC.get("Scene.DocPanelTitle")), "Window");
            scenesWindowElementReference = new ScenesWindowElementReference(rect,
                new GUIContent(TC.get("ItemReferencesList.Title")), "Window");
            scenesWindowExits = new ScenesWindowExits(rect, new GUIContent(TC.get("Element.Name3")), "Window");

            scenesWindowBarriers = new ScenesWindowBarriers(rect, new GUIContent(TC.get("BarriersList.Title")), "Window");
            scenesWindowPlayerMovement = new ScenesWindowPlayerMovement(rect, new GUIContent(TC.get("Trajectory.Title")), "Window");
            

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
                        scenesWindowActiveAreas.Rect = this.Rect;
                        scenesWindowActiveAreas.Draw(aID);
                        break;
                    case ScenesWindowType.Appearance:
                        scenesWindowAppearance.Rect = this.Rect;
                        scenesWindowAppearance.Draw(aID);
                        break;
                    case ScenesWindowType.Documentation:
                        scenesWindowDocumentation.Rect = this.Rect;
                        scenesWindowDocumentation.Draw(aID);
                        break;
                    case ScenesWindowType.ElementRefrence:
                        scenesWindowElementReference.Rect = this.Rect;
                        scenesWindowElementReference.Draw(aID);
                        break;
                    case ScenesWindowType.Exits:
                        scenesWindowExits.Rect = this.Rect;
                        scenesWindowExits.Draw(aID);
                        break;
                    case ScenesWindowType.Barriers:
                        scenesWindowBarriers.Rect = this.Rect;
                        scenesWindowBarriers.Draw(aID);
                        break;
                    case ScenesWindowType.PlayerMovement:
                        scenesWindowPlayerMovement.Rect = this.Rect;
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
					i < Controller.getInstance().getCharapterList().getSelectedChapterDataControl().getScenesList ().getScenes().Count;
                    i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(
						Controller.getInstance().getCharapterList().getSelectedChapterDataControl().getScenesList ().getScenes()[i].getId(),
                        GUILayout.Width(m_Rect.width * 0.65f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(m_Rect.width * 0.3f)))
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
            scenesWindowActiveAreas = new ScenesWindowActiveAreas(m_Rect, new GUIContent(TC.get("ActiveAreasList.Title")),
                "Window");
            scenesWindowAppearance = new ScenesWindowAppearance(m_Rect, new GUIContent(TC.get("Scene.LookPanelTitle")),
                "Window");
            scenesWindowDocumentation = new ScenesWindowDocumentation(m_Rect,
                new GUIContent(TC.get("Scene.DocPanelTitle")), "Window");
            scenesWindowElementReference = new ScenesWindowElementReference(m_Rect,
                new GUIContent(TC.get("ItemReferencesList.Title")), "Window");
            scenesWindowExits = new ScenesWindowExits(m_Rect, new GUIContent(TC.get("Element.Name3")), "Window");

            // Only visible for 3rd person
            if (Controller.getInstance().playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                scenesWindowBarriers = new ScenesWindowBarriers(m_Rect, new GUIContent(TC.get("BarriersList.Title")), "Window");
                scenesWindowPlayerMovement = new ScenesWindowPlayerMovement(m_Rect, new GUIContent(TC.get("Trajectory.Title")), "Window");
            }
        }

        void GenerateToggleList()
        {
            toggleList =
                new List<bool>(Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count);
            for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count; i++)
                toggleList.Add(true);
        }

        // ---------------------------------------------
        //         Reorderable List Handlers
        // ---------------------------------------------

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes()[index].setId(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if(r.index != -1 && r.index < r.list.Count)
            {
                Controller.getInstance()
                           .getCharapterList()
                           .getSelectedChapterDataControl()
                           .getScenesList()
                           .duplicateElement(
                               Controller.getInstance()
                                   .getCharapterList()
                                   .getSelectedChapterDataControl()
                                   .getScenesList()
                                   .getScenes()[r.index]);
            }
            else
            {
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().addElement(Controller.SCENE, "newScene");
            }
            
        }

        protected override void OnAddOption(ReorderableList r, string option)
        {
            // No options
        }

        protected override void OnRemove(ReorderableList r)
        {
            if(r.index != -1)
            {
                Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterDataControl()
                              .getScenesList()
                              .deleteElement(
                                  Controller.getInstance()
                                      .getCharapterList()
                                      .getSelectedChapterDataControl()
                                      .getScenesList()
                                      .getScenes()[r.index], false);

                ShowBaseWindowView();
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
            List<Scene> previousList = Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterData()
                              .getScenes();

            List<Scene> scenes = new List<Scene>();
            foreach (string sceneName in r.list)
                scenes.Add(previousList.Find(s => s.getId() == sceneName));


            previousList.Clear();
            previousList.AddRange(scenes);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.getInstance().getCharapterList().getSelectedChapterDataControl().getScenesList ().getScenes().ConvertAll(s => s.getId());
        }
    }
}
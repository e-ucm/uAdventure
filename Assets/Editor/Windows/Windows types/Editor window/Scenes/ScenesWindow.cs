using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(10, typeof(Scene))]
    public class ScenesWindow : DataControlListEditorWindowExtension
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

        private SceneEditor sceneEditor;

        private List<KeyValuePair<string, ScenesWindowType>> tabs;

        public ScenesWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, new GUIContent(TC.get("Element.Name1")), style, options)
        {
            var content = new GUIContent();

            new RectangleComponentEditor(Rect.zero, new GUIContent(""), "");

            // Button
            content.image = (Texture2D) Resources.Load("EAdventureData/img/icons/scenes", typeof(Texture2D));
            content.text = TC.get("Element.Name1");
            ButtonContent = content;

            sceneEditor = new SceneEditor();

            RequestRepaint repaint = () => Repaint();

            // Windows
            scenesWindowActiveAreas = new ScenesWindowActiveAreas(rect,
                new GUIContent(TC.get("ActiveAreasList.Title")), "Window", sceneEditor);
            scenesWindowActiveAreas.OnRequestRepaint = repaint;
            scenesWindowAppearance = new ScenesWindowAppearance(rect, new GUIContent(TC.get("Scene.LookPanelTitle")),
                "Window", sceneEditor);
            scenesWindowAppearance.OnRequestRepaint = repaint;
            scenesWindowDocumentation = new ScenesWindowDocumentation(rect,
                new GUIContent(TC.get("Scene.DocPanelTitle")), "Window", sceneEditor);
            scenesWindowDocumentation.OnRequestRepaint = repaint;
            scenesWindowElementReference = new ScenesWindowElementReference(rect,
                new GUIContent(TC.get("ItemReferencesList.Title")), "Window", sceneEditor);
            scenesWindowElementReference.OnRequestRepaint = repaint;
            scenesWindowExits = new ScenesWindowExits(rect, new GUIContent(TC.get("Element.Name3")), "Window", sceneEditor);
            scenesWindowExits.OnRequestRepaint = repaint;

            scenesWindowBarriers = new ScenesWindowBarriers(rect, new GUIContent(TC.get("BarriersList.Title")), "Window", sceneEditor);
            scenesWindowPlayerMovement = new ScenesWindowPlayerMovement(rect, new GUIContent(TC.get("Trajectory.Title")), "Window", sceneEditor);

            tabs = new List<KeyValuePair<string, ScenesWindowType>>()
                {
                    new KeyValuePair<string, ScenesWindowType>(TC.get("Scene.LookPanelTitle"),      ScenesWindowType.Appearance),
                    new KeyValuePair<string, ScenesWindowType>(TC.get("Scene.DocPanelTitle"),       ScenesWindowType.Documentation),
                    new KeyValuePair<string, ScenesWindowType>(TC.get("ItemReferencesList.Title"),  ScenesWindowType.ElementRefrence),
                    new KeyValuePair<string, ScenesWindowType>(TC.get("ActiveAreasList.Title"),     ScenesWindowType.ActiveAreas),
                    new KeyValuePair<string, ScenesWindowType>(TC.get("Element.Name3"),             ScenesWindowType.Exits)
                };
            if (Controller.Instance.playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
            {
                tabs.Add(new KeyValuePair<string, ScenesWindowType>(TC.get("BarriersList.Title"), ScenesWindowType.Barriers));
                tabs.Add(new KeyValuePair<string, ScenesWindowType>(TC.get("Trajectory.Title"), ScenesWindowType.PlayerMovement));
            }

            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
        }


        public override void Draw(int aID)
        {
            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                var scene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];

                sceneEditor.Components = EditorWindowBase.Components;
                var allElements = new List<DataControl>();
                allElements.AddRange(scene.getReferencesList().getAllReferencesDataControl().ConvertAll(elem => elem.getErdc() as DataControl));
                allElements.AddRange(scene.getActiveAreasList().getActiveAreas().Cast<DataControl>());
                allElements.AddRange(scene.getExitsList().getExits().Cast<DataControl>());

                if (Controller.Instance.playerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
                {
                    allElements.AddRange(scene.getBarriersList().getBarriers().Cast<DataControl>());
                    var hasTrajectory = scene.getTrajectory().hasTrajectory();
                    if (hasTrajectory)
                    {
                        allElements.AddRange(scene.getTrajectory().getNodes().Cast<DataControl>());
                        allElements.Add(scene.getTrajectory());
                    }
                    else
                        allElements.Add(Controller.Instance.SelectedChapterDataControl.getPlayer());

                }
                sceneEditor.elements = allElements;

                /**
                 UPPER MENU
                */
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                openedWindow = tabs[GUILayout.Toolbar(tabs.FindIndex(t => t.Value == openedWindow), tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
                GUILayout.FlexibleSpace();
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
                for (int i = 0;
					i < Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList ().getScenes().Count;
                    i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(
						Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList ().getScenes()[i].getId(),
                        GUILayout.Width(m_Rect.width * 0.65f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(m_Rect.width * 0.3f)))
                    {
                        ShowItemWindowView(i);
                    }
                    GUILayout.EndHorizontal();

                }

            }
        }

        public override void OnDrawMoreWindows()
        {
            if (isConcreteItemVisible)
            {
                switch (openedWindow)
                {
                    case ScenesWindowType.ActiveAreas:
                        scenesWindowActiveAreas.OnDrawMoreWindows();
                        break;
                    case ScenesWindowType.Appearance:
                        scenesWindowAppearance.OnDrawMoreWindows();
                        break;
                    case ScenesWindowType.Documentation:
                        scenesWindowDocumentation.OnDrawMoreWindows();
                        break;
                    case ScenesWindowType.ElementRefrence:
                        scenesWindowElementReference.OnDrawMoreWindows();
                        break;
                    case ScenesWindowType.Exits:
                        scenesWindowExits.OnDrawMoreWindows();
                        break;
                    case ScenesWindowType.Barriers:
                        scenesWindowBarriers.OnDrawMoreWindows();
                        break;
                    case ScenesWindowType.PlayerMovement:
                        scenesWindowPlayerMovement.OnDrawMoreWindows();
                        break;
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
        }

        public void ShowItemWindowView(int s)
        {
            GameRources.GetInstance().selectedSceneIndex = s;
            isConcreteItemVisible = true;
        }

        // ---------------------------------------------
        //         Reorderable List Handlers
        // ---------------------------------------------

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();

            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getScenesList(),
                sceneList => (sceneList as ScenesListDataControl).getScenes().Cast<DataControl>().ToList());
        }
    }
}
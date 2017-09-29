using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using UnityEditor;

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

        private SceneEditor sceneEditor;

        public ScenesWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, new GUIContent(TC.get("Element.Name1")), style, options)
        {
            var content = new GUIContent();

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

            //scenesWindowBarriers = new ScenesWindowBarriers(rect, new GUIContent(TC.get("BarriersList.Title")), "Window", sceneEditor);
            //scenesWindowPlayerMovement = new ScenesWindowPlayerMovement(rect, new GUIContent(TC.get("Trajectory.Title")), "Window", sceneEditor);


            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));

            GenerateToggleList();
            
        }


        public override void Draw(int aID)
        {
            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                var scene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];

                sceneEditor.Components = EditorWindowBase.Components;
                sceneEditor.elements = scene.getReferencesList().getAllReferencesDataControl().ConvertAll(elem => elem.getErdc() as DataControl);

                /**
                 UPPER MENU
                */
                List<KeyValuePair<string, ScenesWindowType>> tabs = new List<KeyValuePair<string, ScenesWindowType>>()
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
            GenerateToggleList();
        }

        public void ShowItemWindowView(int s)
        {
            GameRources.GetInstance().selectedSceneIndex = s;
            isConcreteItemVisible = true;
            // Generate new toogle list - maybe user already created new scenes?
            GenerateToggleList();
        }

        void GenerateToggleList()
        {
            toggleList =
                new List<bool>(Controller.Instance.ChapterList.getSelectedChapterData().getScenes().Count);
            for (int i = 0; i < Controller.Instance.ChapterList.getSelectedChapterData().getScenes().Count; i++)
                toggleList.Add(true);
        }

        // ---------------------------------------------
        //         Reorderable List Handlers
        // ---------------------------------------------

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
			Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList().getScenes ()[index].renameElement(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if(r.index != -1 && r.index < r.list.Count)
            {
                Controller.Instance                           .ChapterList                           .getSelectedChapterDataControl()
                           .getScenesList()
                           .duplicateElement(
                               Controller.Instance                                   .ChapterList                                   .getSelectedChapterDataControl()
                                   .getScenesList()
                                   .getScenes()[r.index]);
            }
            else
            {
                Controller.Instance.SelectedChapterDataControl.getScenesList().addElement(Controller.SCENE, "newScene");
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
                Controller.Instance                              .ChapterList                              .getSelectedChapterDataControl()
                              .getScenesList()
                              .deleteElement(
                                  Controller.Instance                                      .ChapterList                                      .getSelectedChapterDataControl()
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
			var dataControlList = Controller.Instance 				.ChapterList .getSelectedChapterDataControl ().getScenesList ();

			var toPos = r.index;
			var fromPos = dataControlList.getScenes ().FindIndex (i => i.getId () == r.list [r.index] as string);

			dataControlList.MoveElement (dataControlList.getScenes ()[fromPos], fromPos, toPos);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.Instance.ChapterList.getSelectedChapterDataControl().getScenesList ().getScenes().ConvertAll(s => s.getId());
        }
    }
}
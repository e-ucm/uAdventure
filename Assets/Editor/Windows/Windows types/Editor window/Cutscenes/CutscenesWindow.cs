using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class CutscenesWindow : ReorderableListEditorWindowExtension
    {
        private enum CutscenesWindowType { Appearance, Documentation, EndConfiguration }

        private static CutscenesWindowType openedWindow = CutscenesWindowType.Appearance;
        private static CutscenesWindowAppearance cutscenesWindowAppearance;
        private static CutscenesWindowDocumentation cutscenesWindowDocumentation;
        private static CutscenesWindowEndConfiguration cutscenesWindowEndConfiguration;

        private static float windowWidth, windowHeight;


        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static Rect thisRect;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public CutscenesWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            GUIContent content = new GUIContent();
            // Button
            content.image = (Texture2D)Resources.Load("EAdventureData/img/icons/cutscenes", typeof(Texture2D));
            content.text = TC.get("Element.Name9");
            ButtonContent = content;

            cutscenesWindowAppearance = new CutscenesWindowAppearance(aStartPos, new GUIContent(TC.get("Cutscene.App")), "Window");
            cutscenesWindowDocumentation = new CutscenesWindowDocumentation(aStartPos, new GUIContent(TC.get("Cutscene.Doc")), "Window");
            cutscenesWindowEndConfiguration = new CutscenesWindowEndConfiguration(aStartPos, new GUIContent(TC.get("Cutscene.CutsceneEnd")), "Window");

            windowWidth = aStartPos.width;
            windowHeight = aStartPos.height;

            thisRect = aStartPos;
            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
        }


        public override void Draw(int aID)
        {
            Elements = Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes().ConvertAll(s => s.getId());

            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                /**
                UPPER MENU
                */
                GUILayout.BeginHorizontal();
                if (openedWindow == CutscenesWindowType.Appearance)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Cutscene.App")))
                {
                    OnWindowTypeChanged(CutscenesWindowType.Appearance);
                }
                if (openedWindow == CutscenesWindowType.Appearance)
                    GUI.skin = defaultSkin;

                if (openedWindow == CutscenesWindowType.Documentation)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Cutscene.Doc")))
                {
                    OnWindowTypeChanged(CutscenesWindowType.Documentation);
                }
                if (openedWindow == CutscenesWindowType.Documentation)
                    GUI.skin = defaultSkin;

                if (openedWindow == CutscenesWindowType.EndConfiguration)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Cutscene.CutsceneEnd")))
                {
                    OnWindowTypeChanged(CutscenesWindowType.EndConfiguration);
                }
                if (openedWindow == CutscenesWindowType.EndConfiguration)
                    GUI.skin = defaultSkin;
                GUILayout.EndHorizontal();

                switch (openedWindow)
                {
                    case CutscenesWindowType.Appearance:
                        cutscenesWindowAppearance.Draw(aID);
                        break;
                    case CutscenesWindowType.Documentation:
                        cutscenesWindowDocumentation.Draw(aID);
                        break;
                    case CutscenesWindowType.EndConfiguration:
                        cutscenesWindowEndConfiguration.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth * 0.2f)))
                    {
                        ShowItemWindowView(i);
                    }

                    GUILayout.EndHorizontal();

                }
            }
        }


        void OnWindowTypeChanged(CutscenesWindowType type_)
        {
            openedWindow = type_;
        }


        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            isConcreteItemVisible = false;
            GameRources.GetInstance().selectedCutsceneIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            isConcreteItemVisible = true;
            GameRources.GetInstance().selectedCutsceneIndex = o;

            // Reload windows for newly selected scene
            cutscenesWindowAppearance = new CutscenesWindowAppearance(thisRect, new GUIContent(TC.get("Cutscene.App")), "Window");
            cutscenesWindowDocumentation = new CutscenesWindowDocumentation(thisRect, new GUIContent(TC.get("Cutscene.Doc")), "Window");
            cutscenesWindowEndConfiguration = new CutscenesWindowEndConfiguration(thisRect, new GUIContent(TC.get("Cutscene.CutsceneEnd")), "Window");
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[index].setId(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 || r.index >= r.list.Count)
            {
                Controller.getInstance()
                           .getCharapterList()
                           .getSelectedChapterDataControl()
                           .getCutscenesList()
                           .duplicateElement(
                               Controller.getInstance()
                                   .getCharapterList()
                                   .getSelectedChapterDataControl()
                                   .getCutscenesList()
                                   .getCutscenes()[r.index]);
            }

        }

        protected override void OnAddOption(ReorderableList r, string option)
        {

            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getCutscenesList()
                .addElement(option == "Slides" ? Controller.CUTSCENE_SLIDES : Controller.CUTSCENE_VIDEO, "new Cutscene");
        }

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterDataControl()
                              .getCutscenesList()
                              .deleteElement(
                                  Controller.getInstance()
                                      .getCharapterList()
                                      .getSelectedChapterDataControl()
                                      .getCutscenesList()
                                      .getCutscenes()[r.index], false);

                ShowBaseWindowView();
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            if (GameRources.GetInstance().selectedCutsceneIndex == r.index)
            {
                r.index = -1;
                this.Options = new List<string>() { "Video", "Slides" };
            }
            else
            {
                this.Options = new List<string>();
            }

            ShowItemWindowView(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
            List<Cutscene> previousList = Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterData()
                              .getCutscenes();

            List<Cutscene> cutscenes = new List<Cutscene>();
            foreach (string sceneName in r.list)
                cutscenes.Add(previousList.Find(s => s.getId() == sceneName));


            previousList.Clear();
            previousList.AddRange(cutscenes);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }
    }
}
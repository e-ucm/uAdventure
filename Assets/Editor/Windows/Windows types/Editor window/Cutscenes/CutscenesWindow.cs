using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(20, typeof(Cutscene), typeof(Videoscene), typeof(Slidescene))]
    public class CutscenesWindow : ReorderableListEditorWindowExtension
    {
        private enum CutscenesWindowType { Appearance, Documentation, EndConfiguration }

        private static CutscenesWindowType openedWindow = CutscenesWindowType.Appearance;
        private static CutscenesWindowAppearance cutscenesWindowAppearance;
        private static CutscenesWindowDocumentation cutscenesWindowDocumentation;
        private static CutscenesWindowEndConfiguration cutscenesWindowEndConfiguration;


        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static Rect thisRect;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public CutscenesWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name9")), aStyle, aOptions)
        {
            GUIContent content = new GUIContent();
            // Button
            content.image = (Texture2D)Resources.Load("EAdventureData/img/icons/cutscenes", typeof(Texture2D));
            content.text = TC.get("Element.Name9");
            ButtonContent = content;

            cutscenesWindowAppearance = new CutscenesWindowAppearance(aStartPos, new GUIContent(TC.get("Cutscene.App")), "Window");
            cutscenesWindowDocumentation = new CutscenesWindowDocumentation(aStartPos, new GUIContent(TC.get("Cutscene.Doc")), "Window");
            cutscenesWindowEndConfiguration = new CutscenesWindowEndConfiguration(aStartPos, new GUIContent(TC.get("Cutscene.CutsceneEnd")), "Window");


            thisRect = aStartPos;
            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
        }


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;
            

            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                /**
                 UPPER MENU
                */
                List<KeyValuePair<string, CutscenesWindowType>> tabs = new List<KeyValuePair<string, CutscenesWindowType>>()
                {
                    new KeyValuePair<string, CutscenesWindowType>(TC.get("Cutscene.App"), CutscenesWindowType.Appearance),
                    new KeyValuePair<string, CutscenesWindowType>(TC.get("Cutscene.Doc"), CutscenesWindowType.Documentation),
                    new KeyValuePair<string, CutscenesWindowType>(TC.get("Cutscene.CutsceneEnd"), CutscenesWindowType.EndConfiguration)
                };

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                openedWindow = tabs[GUILayout.Toolbar(tabs.FindIndex(t => t.Value == openedWindow), tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                switch (openedWindow)
                {
                    case CutscenesWindowType.Appearance:
                        cutscenesWindowAppearance.Rect = this.Rect;
                        cutscenesWindowAppearance.Draw(aID);
                        break;
                    case CutscenesWindowType.Documentation:
                        cutscenesWindowDocumentation.Rect = this.Rect;
                        cutscenesWindowDocumentation.Draw(aID);
                        break;
                    case CutscenesWindowType.EndConfiguration:
                        cutscenesWindowEndConfiguration.Rect = this.Rect;
                        cutscenesWindowEndConfiguration.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
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
            GameRources.GetInstance().selectedCutsceneIndex = o;
            isConcreteItemVisible = GameRources.GetInstance().selectedCutsceneIndex >= 0
                && GameRources.GetInstance().selectedCutsceneIndex < Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes().Count;
            
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[index].renameElement(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.Instance.ChapterList.getSelectedChapterDataControl()
                           .getCutscenesList()
                           .duplicateElement(
                               Controller.Instance.ChapterList.getSelectedChapterDataControl()
                                   .getCutscenesList()
                                   .getCutscenes()[r.index]);
            }

        }

        protected override void OnAddOption(ReorderableList r, string option)
        {
            Controller.Instance.SelectedChapterDataControl.getCutscenesList()
                .addElement(
                    option == "Slides" ? Controller.CUTSCENE_SLIDES : Controller.CUTSCENE_VIDEO,
                    option == "Slides" ? "newSlidescene" : "newVideoscene");
        }

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                Controller.Instance.ChapterList.getSelectedChapterDataControl()
                              .getCutscenesList()
                              .deleteElement(
                                  Controller.Instance.ChapterList.getSelectedChapterDataControl()
                                      .getCutscenesList()
                                      .getCutscenes()[r.index], false);

                ShowBaseWindowView();
                this.Options = new List<string>() { "Video", "Slides" };
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            if (GameRources.GetInstance().selectedCutsceneIndex == r.index || r.index == -1)
            {
                r.index = -1;
                this.Options = new List<string>() { "Video", "Slides" };
                ShowBaseWindowView();
            }
            else
            {
                this.Options = new List<string>();
                ShowItemWindowView(r.index);
            }

        }

        protected override void OnReorder(ReorderableList r)
        {
            var dataControlList = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList();

            var toPos = r.index;
            var fromPos = dataControlList.getCutscenes().FindIndex(i => i.getId() == r.list[r.index] as string);

            dataControlList.MoveElement(dataControlList.getCutscenes()[fromPos], fromPos, toPos);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
            this.Options = new List<string>() { "Video", "Slides" };
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            Elements = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes().ConvertAll(s => s.getId());
        }
    }
}
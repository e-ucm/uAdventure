using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(20, typeof(Cutscene), typeof(Videoscene), typeof(Slidescene))]
    public class CutscenesWindow : PreviewDataControlExtension
    {
        private enum CutscenesWindowType { Appearance, Documentation, EndConfiguration }

        private static CutscenesWindowType openedWindow = CutscenesWindowType.Appearance;
        private static CutscenesWindowAppearance cutscenesWindowAppearance;
        private static CutscenesWindowDocumentation cutscenesWindowDocumentation;
        private static CutscenesWindowEndConfiguration cutscenesWindowEndConfiguration;

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

            AddTab(TC.get("Cutscene.LookPanelTitle"), CutscenesWindowType.Appearance, cutscenesWindowAppearance);
            AddTab(TC.get("Cutscene.Documentation"), CutscenesWindowType.Documentation, cutscenesWindowDocumentation);
            AddTab(TC.get("Cutscene.DialogPanelTitle"), CutscenesWindowType.EndConfiguration, cutscenesWindowEndConfiguration);
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            GameRources.GetInstance().selectedCutsceneIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            GameRources.GetInstance().selectedCutsceneIndex = o;
            
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }
        

        protected override void OnButton()
        {
            base.OnButton();
            ShowBaseWindowView();
            dataControlList.index = -1;
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getCutscenesList(),
                sceneList => (sceneList as CutscenesListDataControl).getCutscenes().Cast<DataControl>().ToList());
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            cutscenesWindowAppearance.Target = dataControlList.list[index] as DataControl;
            cutscenesWindowAppearance.DrawPreview(rect);
            cutscenesWindowAppearance.Target = null;
        }
    }
}
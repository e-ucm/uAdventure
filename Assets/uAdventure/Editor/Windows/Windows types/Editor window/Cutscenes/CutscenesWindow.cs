using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(20, typeof(CutsceneDataControl))]
    public class CutscenesWindow : PreviewDataControlExtension
    {
        private enum CutscenesWindowType { Appearance, Documentation, EndConfiguration }
        
        private static CutscenesWindowAppearance cutscenesWindowAppearance;

        public CutscenesWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name9")), aStyle, aOptions)
        {
            // Button
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/cutscenes"),
                text = "Element.Name9"
            };

            cutscenesWindowAppearance = new CutscenesWindowAppearance(aStartPos, new GUIContent(TC.get("Cutscene.App")), "Window");
            var cutscenesWindowDocumentation = new CutscenesWindowDocumentation(aStartPos, new GUIContent(TC.get("Cutscene.Doc")), "Window");
            var cutscenesWindowEndConfiguration = new CutscenesWindowEndConfiguration(aStartPos, new GUIContent(TC.get("Cutscene.CutsceneEnd")), "Window");

            AddTab(TC.get("Cutscene.App"), CutscenesWindowType.Appearance, cutscenesWindowAppearance);
            AddTab(TC.get("Cutscene.Doc"), CutscenesWindowType.Documentation, cutscenesWindowDocumentation);
            AddTab(TC.get("Cutscene.CutsceneEnd"), CutscenesWindowType.EndConfiguration, cutscenesWindowEndConfiguration);
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
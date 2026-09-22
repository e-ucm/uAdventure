using Simva;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEngine;

namespace uAdventure.Simva
{
    public enum SimvaTab
    {
        Configuration,
        Wizard
    }

    [EditorWindowExtension(300, typeof(SimvaWindow))]
    public class SimvaWindow : DefaultButtonMenuEditorWindowExtension
    {
        protected TabsManager tabsManager;
        protected SimvaWizard simvaWizard;
        protected SimvaPluginConfigurationWindow configWindow;

        public SimvaWindow(Rect aStartPos, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Simva.Title")), aStyle, aOptions)
        { 
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("simva-icon"),
                text = "Simva"
            };

            tabsManager = new TabsManager(this);
            simvaWizard = new SimvaWizard();
            configWindow = new SimvaPluginConfigurationWindow(new Rect(0, 0, 600, 400), new GUIContent("Configuration"), null);

            tabsManager.AddTab(TC.get("Simva.Tab.Configuration"), SimvaTab.Configuration, configWindow);
            tabsManager.AddTab(TC.get("Simva.Tab.Wizard"), SimvaTab.Wizard, new SimvaWizardLayoutWindow(simvaWizard));
            tabsManager.DefaultOpenedWindow = SimvaTab.Configuration;
        }


        public override void Draw(int aID)
        {
            tabsManager.Draw(aID);
        }
        
        protected override void OnButton()
        {
            tabsManager.Reset();
        }

        private class SimvaWizardLayoutWindow : LayoutWindow
        {
            private readonly SimvaWizard wizard;

            public SimvaWizardLayoutWindow(SimvaWizard wizard)
                : base(new Rect(0, 0, 600, 400), new GUIContent("Wizard"), null)
            {
                this.wizard = wizard;
            }

            public override void Draw(int aID)
            {
                wizard.OnGUI();
            }
        }
    }

}

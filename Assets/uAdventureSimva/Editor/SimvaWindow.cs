using Simva;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEngine;

namespace uAdventure.Simva
{
    [EditorWindowExtension(300, typeof(SimvaWindow))]
    public class SimvaWindow : DefaultButtonMenuEditorWindowExtension
    {
        protected TabsManager tabsManager;
        protected SimvaWizard simvaWizard;

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
        }


        public override void Draw(int aID)
        {
            if (!tabsManager.Draw(aID))
            {
                simvaWizard.OnGUI();
            }
        }
        
        protected override void OnButton()
        {
            tabsManager.Reset();
        }

    }

}

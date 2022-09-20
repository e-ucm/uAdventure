using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Simva
{
    public class SimvaWizardWindow : EditorWindow
    {
        private SimvaWizard simvaWizard;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Simva/Configuration")]
        static void Open()
        {
            // Get existing open window or if none, make a new one:
            SimvaWizardWindow window = (SimvaWizardWindow)EditorWindow.GetWindow(typeof(SimvaWizardWindow));
            window.simvaWizard = new SimvaWizard();
            window.Show();
        }

        private void OnGUI()
        {
            simvaWizard.OnGUI();
        }
    }
}


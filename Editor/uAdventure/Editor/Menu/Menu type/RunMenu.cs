using UnityEditor;
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class RunMenu : WindowMenuContainer
    {
        private RunDebugMenuItem debug;
        private RunNormalMenuItem normal;

        public RunMenu()
        {
            SetMenuItems();
        }

        protected override void Callback(object obj)
        {
            if ((obj as RunDebugMenuItem) != null)
                debug.OnCliked();
            else if ((obj as RunNormalMenuItem) != null)
                normal.OnCliked();
        }

        protected override void SetMenuItems()
        {
            menu = new GenericMenu();

            debug = new RunDebugMenuItem("DEBUG");
            normal = new RunNormalMenuItem("NORMAL");

            menu.AddItem(new GUIContent(Language.GetText(debug.Label)), false, Callback, debug);
            menu.AddItem(new GUIContent(Language.GetText(normal.Label)), false, Callback, normal);
        }
    }
}
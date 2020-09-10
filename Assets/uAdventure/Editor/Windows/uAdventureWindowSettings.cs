using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class uAdventureWindowSettings : EditorWindowBase
    {
        public static void OpenAdventureWindow()
        {
            if (!Language.Initialized)
                Language.Initialize();

            var window = ScriptableObject.CreateInstance<uAdventureWindowSettings>();
            window.ShowUtility();
        }


        protected override void InitWindows()
        {
            WantsMouseMove = true;
            AddExtension(new AdventureWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window"));
            AddExtension(new ButtonsWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window"));
            AddExtension(new CursorsWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window"));
            AddExtension(new InventoryWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window"));
            AddExtension(new SaveWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window"));
        }
    }
}

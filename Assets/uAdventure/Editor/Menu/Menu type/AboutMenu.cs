using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AboutMenu : WindowMenuContainer
    {
        private AboutEAMenuItem about;
        private AboutEASendMenuItem send;

        public AboutMenu()
        {
            SetMenuItems();
        }

        protected override void Callback(object obj)
        {
            if ((obj as AboutEAMenuItem) != null)
                about.OnCliked();
            else if ((obj as AboutEASendMenuItem) != null)
                send.OnCliked();
        }

        protected override void SetMenuItems()
        {
            menu = new GenericMenu();

            about = new AboutEAMenuItem("Menu.AboutEAD");
            send = new AboutEASendMenuItem("Menu.SendComments");

            menu.AddItem(new GUIContent(TC.get(about.Label)), false, Callback, about);
            menu.AddItem(new GUIContent(TC.get(send.Label)), false, Callback, send);
        }
    }
}
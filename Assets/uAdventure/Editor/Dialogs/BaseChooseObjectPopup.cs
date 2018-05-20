using UnityEngine;
using UnityEditor;

namespace uAdventure.Editor
{
    public class BaseChooseObjectPopup : EditorWindow
    {

        protected DialogReceiverInterface reference;
        protected string[] elements;
        protected string selectedElementID;

        public virtual void Init(DialogReceiverInterface e)
        {
            BaseChooseObjectPopup window = this;
            reference = e;
            window.position = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 500, 100);
            window.ShowUtility();
        }

    }
}

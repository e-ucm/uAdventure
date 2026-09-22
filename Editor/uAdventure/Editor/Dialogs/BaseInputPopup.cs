using UnityEngine;
using UnityEditor;

namespace uAdventure.Editor
{
    public abstract class BaseInputPopup : EditorWindow
    {
        protected DialogReceiverInterface reference;

        public virtual void Init(DialogReceiverInterface e)
        {
            BaseInputPopup window = this;
            reference = e;
            window.position = new Rect(Screen.width / 2 - 250, Screen.height / 2 - 150, 500, 300);
            window.ShowUtility();
        }

    }
}
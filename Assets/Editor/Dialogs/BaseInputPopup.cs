using UnityEngine;
using UnityEditor;

namespace uAdventure.Editor
{
    public abstract class BaseInputPopup : EditorWindow
    {
        protected DialogReceiverInterface reference;
        protected string textContent;

        public virtual void Init(DialogReceiverInterface e, string startTextContent)
        {
            BaseInputPopup window = this;
            reference = e;
            textContent = startTextContent;
            window.position = new Rect(Screen.width / 2 - 250, Screen.height / 2 - 150, 500, 300);
            window.Show();
        }

    }
}
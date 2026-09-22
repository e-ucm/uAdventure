using UnityEngine;
using UnityEditor;

namespace uAdventure.Editor
{
    public class BaseAreaEditablePopup : EditorWindow
    {
        protected DialogReceiverInterface reference;

        public virtual void Init(DialogReceiverInterface e, float width, float height)
        {
            BaseAreaEditablePopup window = this;
            reference = e;
            window.position = new Rect(0, 0, width, height * 1.2f);
            window.Show();
        }
    }
}
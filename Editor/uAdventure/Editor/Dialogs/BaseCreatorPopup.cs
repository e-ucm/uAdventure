using UnityEngine;
using UnityEditor;

namespace uAdventure.Editor
{
    public class BaseCreatorPopup : EditorWindow
    {
        [SerializeField]
        protected bool shown = false;
        protected DialogReceiverInterface reference;
        protected float windowWidth = 500, windowHeight = 600;

        public virtual void Init(DialogReceiverInterface e)
        {
            BaseCreatorPopup window = this;
            reference = e;
            if (window && !shown)
            { 
                window.position = new Rect(Screen.width / 2 - windowWidth / 2, Screen.height / 2 - windowHeight / 2, windowWidth, windowHeight);
                window.Show();
                shown = true;
            }
        }
    }
}
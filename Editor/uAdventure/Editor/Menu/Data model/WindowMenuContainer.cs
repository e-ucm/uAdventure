using UnityEditor;

namespace uAdventure.Editor
{
    public abstract class WindowMenuContainer
    {
        public GenericMenu menu { get; set; }

        protected abstract void SetMenuItems();
        protected abstract void Callback(object obj);
    }
}
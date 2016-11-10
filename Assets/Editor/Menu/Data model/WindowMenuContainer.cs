using System.Collections.Generic;
using UnityEditor;

public abstract class WindowMenuContainer{
    public GenericMenu menu { get; set; }

    protected abstract void SetMenuItems();
    protected abstract void Callback(object obj);
}

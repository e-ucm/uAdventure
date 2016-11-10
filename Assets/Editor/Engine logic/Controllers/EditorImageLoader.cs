using UnityEngine;
using System.Collections;

public class EditorImageLoader : ImageLoaderFactory
{
    public Sprite getImageFromPath(string uri)
    {
        return AssetsController.getImage(uri);
    }

    public void showErrorDialog(string title, string message)
    {
        Controller.getInstance().showErrorDialog(title, message);
    }
}

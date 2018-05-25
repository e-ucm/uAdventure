using UnityEngine;
using System.Collections;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class EditorImageLoader : ImageLoaderFactory
    {
        public Sprite getImageFromPath(string uri)
        {
            return AssetsController.getImage(uri);
        }

        public void showErrorDialog(string title, string message)
        {
            Controller.Instance.ShowErrorDialog(title, message);
        }
    }
}
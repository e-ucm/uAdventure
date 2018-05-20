using UnityEngine;

namespace uAdventure.Core
{
    /**
     * This interface allows to load images in different ways from the editor o the engine
     * while not using their specific methods.
     *
     */
    public interface ImageLoaderFactory
    {
        Sprite getImageFromPath(string uri);

        void showErrorDialog(string title, string message);
    }
}
using UnityEngine;

namespace uAdventure.Editor
{
    public interface IMenuItem
    {
        string Label
        {
            get; set;
        }

        void OnCliked();
    }

    public interface IMenuConcreteViewItem
    {
        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        void ShowBaseWindowView();

        // TODO - change Object to adequate class
        void ShowItemWindowView(Object o);

        // Flag determining visibility of concrete item information
        bool isConcreteItemVisible { get; }
    }
}
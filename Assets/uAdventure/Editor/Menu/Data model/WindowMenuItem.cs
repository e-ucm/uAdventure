using UnityEngine;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class WindowMenuItem : MonoBehaviour, IMenuItem
    {

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Debug.Log("MenuItem: " + Label);
        }
    }
}
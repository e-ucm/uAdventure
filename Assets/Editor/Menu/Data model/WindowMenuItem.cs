using UnityEngine;
using System.Collections;
using System;

public class WindowMenuItem : MonoBehaviour, IMenuItem {

    public string Label
    {
        get; set;
    }

    public void OnCliked()
    {
        Debug.Log("MenuItem: " + Label);
    }
}

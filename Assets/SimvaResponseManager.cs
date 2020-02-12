using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimvaResponseManager : MonoBehaviour {

    public Text response;

    void Start () {
        SimvaController.Instance.AddResponseManager(this);
    }

    private void OnDestroy()
    {
        SimvaController.Instance.RemoveResponseManager(this);
    }

    public void Notify(string message)
    {
        response.text = message;
    }
}

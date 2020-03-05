using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimvaLoadingManager : MonoBehaviour
{

    public GameObject Loading;
    public GameObject Form;

    void Start()
    {
        SimvaController.Instance.AddLoadingManager(this);
    }

    private void OnDestroy()
    {
        SimvaController.Instance.RemoveLoadingManager(this);
    }

    public void IsLoading(bool state)
    {
        if (state)
        {
            Loading.active = true;
            Form.active = false;
        }
        else
        {
            Loading.active = false;
            Form.active = true;
        }
    }
}

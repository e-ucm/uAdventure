﻿using uAdventure.Simva;
using UnityEngine;

namespace Simva
{
    public class SimvaLoadingManager : MonoBehaviour
    {
        public GameObject Loading;
        public GameObject Form;

        protected void Start()
        {
            SimvaExtension.Instance.AddLoadingManager(this);
        }

        protected void OnDestroy()
        {
            SimvaExtension.Instance.RemoveLoadingManager(this);
        }

        public void IsLoading(bool state)
        {
            if (state)
            {
                Loading.SetActive(true);
                Form.SetActive(false);
            }
            else
            {
                Loading.SetActive(false);
                Form.SetActive(true);
            }
        }
    }
}

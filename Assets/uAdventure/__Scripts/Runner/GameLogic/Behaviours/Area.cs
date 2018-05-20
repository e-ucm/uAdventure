using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Interactuable))]
    public class Area : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Interactuable interactuable;

        private void OnEnable()
        {
            interactuable = GetComponent<Interactuable>();
            if(interactuable == null)
            {
                this.enabled = false;
            }
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            GUIManager.Instance.showHand(true);
            interactuable.setInteractuable(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GUIManager.Instance.showHand(false);
            interactuable.setInteractuable(false);
        }
    }
}
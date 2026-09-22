using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class InventorySlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        private RectTransform rectTransform;
        private Vector2 velocity;

        public float time = 0.2f;

        public bool KeepOpen { get; set; }
        public bool KeepClose { get; set; }

        public Transform elementHolder;

        public bool IsOpen { get; private set; }


        public void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Update()
        {
            var openPos = new Vector2(0, rectTransform.rect.height);
            var closedPos = Vector2.zero;
            var showingY = new Vector2(0, 25);
            if(rectTransform.rect.y < 0)
            {
                openPos = new Vector2(0, -rectTransform.rect.height);
                showingY = new Vector2(Screen.height - 25, Screen.height);
            }

            var mouseInside = showingY.x < Input.mousePosition.y && Input.mousePosition.y < showingY.y;
            if (!KeepClose && (mouseInside || KeepOpen || IsOpen))
            {
                rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, openPos, ref velocity, time);
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, closedPos, ref velocity, time);
            }

            for(int i = 0; i<elementHolder.childCount; i++)
            {
                var child = elementHolder.GetChild(i);
                var rt = child.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.y, rt.sizeDelta.y);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsOpen = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (MenuMB.Instance.IsShowing)
                return;
            IsOpen = false;
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace uAdventure.Runner
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        public Button openButton;
        [SerializeField]
        public Button closeButton;
        [SerializeField]
        private GameObject addedItemParticlePrefab;
        [SerializeField]
        private GameObject insideElement;
        [SerializeField]
        private GameObject elementHolder;
        
        public bool Opened { get { return insideElement.activeSelf; } set { insideElement.SetActive(value); } }

        public void AddElement(GameObject element)
        {
            AddElement(element, true);
        }

        public void AddElement(GameObject element, bool animate)
        {
            if (animate)
            {
                var addedItemParticle = GameObject.Instantiate(addedItemParticlePrefab, openButton.transform.parent);
                var itemParticle = addedItemParticle.GetComponent<ItemParticle>();
                itemParticle.Image = element.GetComponent<Image>().sprite;
                openButton.animator.SetTrigger("Highlight");
            }

            element.transform.SetParent(elementHolder.transform);
            var rectTransform = element.transform as RectTransform;
            rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0);
            rectTransform.localRotation = Quaternion.identity;
            element.transform.localScale = Vector3.one;
        }

        public void RemoveElement(GameObject element)
        {
            element.transform.SetParent(null);
        }
    }
}

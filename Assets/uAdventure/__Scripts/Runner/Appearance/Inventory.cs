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
        private InventorySlider topElement;
        [SerializeField]
        private InventorySlider bottomElement;
        [SerializeField]
        private GameObject elementHolder;

        private Dictionary<GameObject, GameObject[]> clones = new Dictionary<GameObject, GameObject[]>();

        private int inventoryType = DescriptorData.INVENTORY_ICON_FREEPOS;
        private bool opened = false;

        public int InventoryType 
        { 
            get 
            {
                return inventoryType;
            }
            set
            {
                openButton.gameObject.SetActive(false);
                insideElement.gameObject.SetActive(false);
                topElement.gameObject.SetActive(false);
                bottomElement.gameObject.SetActive(false);

                switch (value)
                {
                    case DescriptorData.INVENTORY_NONE:
                        break;
                    case DescriptorData.INVENTORY_TOP_BOTTOM:
                        topElement.gameObject.SetActive(true);
                        topElement.KeepOpen = false;
                        bottomElement.gameObject.SetActive(true);
                        bottomElement.KeepOpen = false;
                        break;
                    case DescriptorData.INVENTORY_TOP:
                        topElement.gameObject.SetActive(true);
                        topElement.KeepOpen = false;
                        break;
                    case DescriptorData.INVENTORY_BOTTOM:
                        bottomElement.gameObject.SetActive(true);
                        bottomElement.KeepOpen = false;
                        break;
                    case DescriptorData.INVENTORY_FIXED_TOP:
                        topElement.gameObject.SetActive(true);
                        topElement.KeepOpen = true;
                        break;
                    case DescriptorData.INVENTORY_FIXED_BOTTOM:
                        bottomElement.gameObject.SetActive(true);
                        bottomElement.KeepOpen = true;
                        break;
                    case DescriptorData.INVENTORY_ICON_FREEPOS:
                        openButton.gameObject.SetActive(true);
                        break;
                }
                inventoryType = value;
            }
        }

        public bool Opened
        {
            get { return opened; }
            set
            {
                switch (inventoryType)
                {
                    case DescriptorData.INVENTORY_TOP_BOTTOM:
                        topElement.KeepOpen = value;
                        bottomElement.KeepOpen = value;
                        break;
                    case DescriptorData.INVENTORY_TOP:
                        topElement.KeepOpen = value;
                        break;
                    case DescriptorData.INVENTORY_BOTTOM:
                        bottomElement.KeepOpen = value;
                        break;
                    case DescriptorData.INVENTORY_FIXED_TOP:
                        topElement.KeepOpen = true;
                        break;
                    case DescriptorData.INVENTORY_FIXED_BOTTOM:
                        bottomElement.KeepOpen = true;
                        break;
                    case DescriptorData.INVENTORY_ICON_FREEPOS:
                        insideElement.SetActive(value);
                        break;
                }
                opened = value;
            }
        }

        public void AddElement(GameObject element)
        {
            AddElement(element, true);
        }

        public void AddElement(GameObject element, bool animate)
        {
            if (inventoryType == DescriptorData.INVENTORY_ICON_FREEPOS && animate)
            {
                var addedItemParticle = GameObject.Instantiate(addedItemParticlePrefab, openButton.transform.parent);
                addedItemParticle.transform.position = openButton.transform.position;
                var itemParticle = addedItemParticle.GetComponent<ItemParticle>();
                itemParticle.Image = element.GetComponent<Image>().sprite;
                openButton.animator.SetTrigger("Highlight");
            }

            var uiItem = element.GetComponent<UIItem>();
            element.transform.SetParent(elementHolder.transform);
            ResetPRS(element);

            var topClone = GameObject.Instantiate(element);
            topClone.GetComponent<UIItem>().Element = uiItem.Element;
            topClone.transform.SetParent(topElement.elementHolder);
            ResetPRS(topClone);

             var bottomClone = GameObject.Instantiate(element);
            bottomClone.GetComponent<UIItem>().Element = uiItem.Element;
            bottomClone.transform.SetParent(bottomElement.elementHolder);
            ResetPRS(bottomClone);

            clones.Add(element, new GameObject[]{ topClone, bottomClone });
        }

        private static void ResetPRS(GameObject element)
        {
            var rectTransform = element.transform as RectTransform;
            rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, 0);
            rectTransform.localRotation = Quaternion.identity;
            element.transform.localScale = Vector3.one;
        }

        public void RemoveElement(GameObject element)
        {
            element.transform.SetParent(null);
            if (clones.ContainsKey(element))
            {
                var elementClones = clones[element];
                foreach(var clone in elementClones)
                {
                    clone.transform.SetParent(null);
                    DestroyImmediate(clone);
                }
                clones.Remove(element);
            }
        }
    }
}

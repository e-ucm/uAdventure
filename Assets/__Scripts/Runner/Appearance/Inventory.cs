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
        private GameObject addedItemParticlePrefab;
        [SerializeField]
        private Button openButton;
        [SerializeField]
        private GameObject insideElement;
        [SerializeField]
        private GameObject elementHolder;

        private Button closeButton;
        
        public bool Show { get { return insideElement.activeSelf; } set { insideElement.SetActive(value); } }

        public void AddElement(GameObject element)
        {
            var addedItemParticle = GameObject.Instantiate(addedItemParticlePrefab, openButton.transform.parent);
            var itemParticle = addedItemParticle.GetComponent<ItemParticle>();
            itemParticle.Image = element.GetComponent<Image>().sprite;

            element.transform.SetParent(elementHolder.transform);
            element.transform.localScale = Vector3.one;
            openButton.animator.SetTrigger("Highlight");
        }

        public void RemoveElement(GameObject element)
        {
            element.transform.SetParent(null);
        }
    }
}

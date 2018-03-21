using System.Collections.Generic;
using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; protected set; }

        // Editable fields
        [SerializeField]
        private GameObject inventoryHolder;
        [SerializeField]
        private GameObject inventoryPrefab;
        [SerializeField]
        private GameObject elementPrefab;

        // Private fields
        private Inventory inventory;
        private Dictionary<Element, GameObject> elementObjects;

        // Properties
        public GameObject InventoryHolder
        {
            get { return inventoryHolder; }
            set
            {
                if (value != inventoryHolder)
                {
                    inventoryHolder = InventoryHolder;
                    if (inventoryHolder == null)
                    {
                        DestroyImmediate(inventory);
                    }
                    else
                    {
                        inventory.transform.SetParent(inventoryHolder.transform);
                    }
                }
            }
        }

        public bool Show
        {
            get
            {
                return inventory ? inventory.Show : false;
            }
            set
            {
                if (inventory) inventory.Show = value;
            }
        }

        private void Start()
        {
            if (InventoryManager.Instance)
                Debug.LogWarning("Multiple inventory managers have been found!");

            InventoryManager.Instance = this;

            var inventoryGO = GameObject.Instantiate(inventoryPrefab, inventoryHolder.transform);
            inventory = inventoryGO.GetComponent<Inventory>();
            if (!inventory)
                Debug.LogWarning("Inventory gameobject MUST have an Inventory component!");

            elementObjects = new Dictionary<Element, GameObject>();
        }

        public void AddElement(Element element)
        {
            if (elementObjects.ContainsKey(element))
            {
                Debug.LogWarning("Adding the same element to the inventory twice!");
                return;
            }
            // Create the element instance
            var elementGO = GameObject.Instantiate(elementPrefab);
            elementGO.GetComponent<UIItem>().Element = element;
            elementGO.SendMessage("Start"); // Force start

            // Add the element to the gamestate
            Game.Instance.GameState.addInventoryItem(element.getId());
            // Save the element in the cache
            elementObjects.Add(element, elementGO);
            // Add the element to the rendered inventory
            inventory.AddElement(elementGO);
        }

        public void RemoveElement(Element element)
        {
            if (!elementObjects.ContainsKey(element))
            {
                Debug.LogWarning("Removing an element not existing in the inventory!");
                return;
            }

            // Remove the element from the gamestate
            Game.Instance.GameState.removeInventoryItem(element.getId());
            // Remove from the inventory
            inventory.RemoveElement(elementObjects[element]);
            // Destroy the object
            DestroyImmediate(elementObjects[element]);
            // Removing from the refferences
            elementObjects.Remove(element);
        }
    }
}
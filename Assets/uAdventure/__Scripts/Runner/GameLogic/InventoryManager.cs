using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using uAdventure.Core;
using Xasu;
using Xasu.HighLevel;

namespace uAdventure.Runner
{
    public class InventoryManager : Singleton<InventoryManager>
    {

        // Editable fields
        [SerializeField]
        private GameObject inventoryHolder;
        [SerializeField]
        private GameObject inventoryPrefab;
        [SerializeField]
        private GameObject elementPrefab;

        // Private fields
        private Inventory inventory;
        private Dictionary<Element, GameObject> elementObjects = new Dictionary<Element, GameObject>();

        [Range(0, DescriptorData.INVENTORY_ICON_FREEPOS)]
        public int InventoryType;

        private bool started = false;
        private bool show = true;

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

        public bool Opened
        {
            get
            {
                return inventory && inventory.Opened;
            }
            set
            {
                if (MenuMB.Instance)
                {
                    MenuMB.Instance.hide(true);
                }
                if (inventory)
                {
                    if(inventory.Opened != value && XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                    {
                        if (value)
                        {
                            AccessibleTracker.Instance.Accessed("Inventory", AccessibleTracker.AccessibleType.Inventory);
                        }
                        else
                        {
                            AccessibleTracker.Instance.Skipped("Inventory", AccessibleTracker.AccessibleType.Inventory);
                        }
                    }

                    inventory.Opened = value;
                }
            }
        }

        public bool Show
        {
            get
            {
                return show;
            }
            set
            {
                if (!started)
                {
                    Start();
                }

                show = value;
                if (!value)
                {
                    inventory.InventoryType = DescriptorData.INVENTORY_NONE;
                }
                else
                {
                    inventory.InventoryType = InventoryType;
                }
            }
        }

        private void Start()
        {
            if (started)
            {
                return;
            }
            started = true;

            if (Instance && Instance != this)
            {
                Debug.LogWarning("Multiple inventory managers have been found!");
            }
            if (!inventoryPrefab)
            {
                Debug.LogWarning("Inventory prefab not set!");
                return;
            }
            var inventoryGo = GameObject.Instantiate(inventoryPrefab, inventoryHolder.transform);
            inventory = inventoryGo.GetComponent<Inventory>();
            if (!inventory)
            {
                Debug.LogWarning("Inventory gameobject MUST have an Inventory component!");
                return;
            }

            inventory.openButton.onClick.AddListener(delegate { Open(); });
            inventory.closeButton.onClick.AddListener(delegate { Close(); });
        }

        public void Open()
        {
            Opened = true;
        }

        public void Close()
        {
            Opened = false;
        }

        public void Restore()
        {
            if (!started)
            {
                Start();
            }

            Clear();
            var gs = Game.Instance.GameState;
            foreach (var item in gs.GetInventoryItems())
            {
                var element = gs.GetObject(item);
                AddElementToInventory(element, false);
            }
            InventoryType = gs.Data.getInventoryPosition();
            var inventoryImage = gs.Data.getInventoryImage();
            if (string.IsNullOrEmpty(inventoryImage))
            {
                inventoryImage = SpecialAssetPaths.ASSET_DEFAULT_INVENTORY;
            }

            var inventoryIcon = Game.Instance.ResourceManager.getSprite(inventoryImage);

            inventory.openButton.image.sprite = inventoryIcon;

        }

        private void Clear()
        {
            var toRemove = elementObjects.Keys.ToList();
            foreach (var item in toRemove)
            {
                RemoveElementFromInventory(item);
            }
        }

        public void AddElement(Element element)
        {
            if (AddElementToInventory(element, true))
            {
                // Add the element to the gamestate
                Game.Instance.GameState.AddInventoryItem(element.getId());
            }
        }

        private bool AddElementToInventory(Element element, bool animate)
        {
            if (elementObjects.ContainsKey(element))
            {
                Debug.LogWarning("Adding the same element to the inventory twice!");
                return false;
            }
            // Create the element instance
            var elementGO = GameObject.Instantiate(elementPrefab);
            elementGO.GetComponent<UIItem>().Element = element;
            elementGO.SendMessage("Start"); // Force start
            // Save the element in the cache
            elementObjects.Add(element, elementGO);
            // Add the element to the rendered inventory
            inventory.AddElement(elementGO, animate);
            return true;
        }

        public void RemoveElement(Element element)
        {
            if (RemoveElementFromInventory(element))
            {
                // Remove the element from the gamestate
                Game.Instance.GameState.RemoveInventoryItem(element.getId());
            }
        }

        private bool RemoveElementFromInventory(Element element)
        {
            if (!elementObjects.ContainsKey(element))
            {
                Debug.LogWarning("Removing an element not existing in the inventory!");
                return false;
            }
            // Remove from the inventory
            inventory.RemoveElement(elementObjects[element]);
            // Destroy the object
            DestroyImmediate(elementObjects[element]);
            // Removing from the refferences
            elementObjects.Remove(element);
            return true;
        }

        private void Update()
        {
            if(inventory.InventoryType != InventoryType && Show)
            {
                inventory.InventoryType = InventoryType;
            }

            var rectTransform = (RectTransform)inventory.openButton.transform;
            var parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
            var inventoryIcon = inventory.openButton.image.sprite;
            if (!inventoryIcon)
            {
                return;
            }
            var gs = Game.Instance.GameState;
            var position = gs.Data.getInventoryCoords();
            var ratio = new Vector2(position.x / 800, 1- (position.y / 600));
            parentRectTransform.anchorMax = parentRectTransform.anchorMin = ratio;
            parentRectTransform.anchoredPosition = new Vector2(0,0);
            var scale = gs.Data.getInventoryScale();
            parentRectTransform.localScale = new Vector3(scale, scale, 1);
            rectTransform.sizeDelta = new Vector2(inventoryIcon.texture.width, inventoryIcon.texture.height);
        }
    }
}
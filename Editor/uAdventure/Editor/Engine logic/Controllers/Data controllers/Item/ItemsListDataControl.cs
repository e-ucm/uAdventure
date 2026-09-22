using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ItemsListDataControl : DataControl
    {
        /**
             * List of items.
             */
        private List<Item> itemsList;

        /**
         * List of item controllers.
         */
        private List<ItemDataControl> itemsDataControlList;

        /**
         * Constructor.
         * 
         * @param itemsList
         *            List of items
         */
        public ItemsListDataControl(List<Item> itemsList)
        {

            this.itemsList = itemsList;

            // Create subcontrollers
            itemsDataControlList = new List<ItemDataControl>();
            foreach (Item item in itemsList)
                itemsDataControlList.Add(new ItemDataControl(item));
        }

        /**
         * Returns the list of item controllers.
         * 
         * @return Item controllers
         */
        public List<ItemDataControl> getItems()
        {

            return itemsDataControlList;
        }

        /**
         * Returns the last item controller from the list.
         * 
         * @return Last item controller
         */
        public ItemDataControl getLastItem()
        {

            return itemsDataControlList[itemsDataControlList.Count - 1];
        }


        public string[] getItemsIDs()
        {
            string[] scenesInfo = null;
            scenesInfo = new string[itemsList.Count];
            for (int i = 0; i < itemsList.Count; i++)
            {
                scenesInfo[i] = itemsList[i].getId();
            }

            return scenesInfo;
        }

        public int getItemIndexByID(string id)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                if (itemsList[i].getId().Equals(id))
                    return i;
            }
            return -1;
        }

        /**
         * Returns the info of the items contained in the list.
         * 
         * @return Array with the information of the items. It contains the
         *         identifier of each item, and the number of actions
         */
        public string[][] getItemsInfo()
        {

            string[][] itemsInfo = null;

            // Create the list for the items
            itemsInfo = new string[itemsList.Count][];
            for (int i = 0; i < itemsList.Count; i++)
                itemsInfo[i] = new string[2];

            // Fill the array with the info
            for (int i = 0; i < itemsList.Count; i++)
            {
                Item item = itemsList[i];
                itemsInfo[i][0] = item.getId();
                itemsInfo[i][1] = TC.get("ItemsList.ActionsNumber", item.getActions().Count.ToString());
            }

            return itemsInfo;
        }


        public override System.Object getContent()
        {

            return itemsList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.ITEM };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new items
            return type == Controller.ITEM;
        }


        public override bool canBeDeleted()
        {

            return false;
        }


        public override bool canBeMoved()
        {

            return false;
        }


        public override bool canBeRenamed()
        {

            return false;
        }


        public override bool addElement(int type, string itemId)
        {
            bool elementAdded = false;

            if (type == Controller.ITEM)
            {
                if (string.IsNullOrEmpty(itemId))
                    controller.ShowInputIdDialog(TC.get("Operation.AddItemTitle"), TC.get("Operation.AddItemMessage"),
                        Controller.Instance.makeElementValid(TC.get("Operation.AddItemDefaultValue")), performAddElement);
                else
                {
                    performAddElement(null, itemId);
                    elementAdded = true;
                }
            }

            return elementAdded;
        }

        private void performAddElement(object sender, string itemId)
        {
            if (!controller.isElementIdValid(itemId))
                itemId = controller.makeElementValid(itemId);

            Item newItem = new Item(itemId);
            itemsList.Add(newItem);
            itemsDataControlList.Add(new ItemDataControl(newItem));
            controller.IdentifierSummary.addId<Item>(itemId);
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is ItemDataControl))
                return false;


            Item newElement = (Item)(((Item)(dataControl.getContent())).Clone());
            string id = newElement.getId();

			if (!controller.isElementIdValid(id))
				id = controller.makeElementValid(id);
			
            newElement.setId(id);
            itemsList.Add(newElement);
            itemsDataControlList.Add(new ItemDataControl(newElement));
            controller.IdentifierSummary.addId<Item>(id);
            return true;
        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddItemDefaultValue");
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;
            string itemId = ((ItemDataControl)dataControl).getId();
            string references = controller.countIdentifierReferences(itemId).ToString();

            // Ask for confirmation
            if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { itemId, references })))
            {
                if (itemsList.Remove((Item)dataControl.getContent()))
                {
                    itemsDataControlList.Remove((ItemDataControl)dataControl);
                    controller.deleteIdentifierReferences(itemId);
                    controller.IdentifierSummary.deleteId<Item>(itemId);
                    controller.updateVarFlagSummary();
                    controller.DataModified();
                    elementDeleted = true;
                }
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = itemsList.IndexOf((Item)dataControl.getContent());

            if (elementIndex > 0)
            {
                Item e = itemsList[elementIndex];
                ItemDataControl c = itemsDataControlList[elementIndex];
                itemsList.RemoveAt(elementIndex);
                itemsDataControlList.RemoveAt(elementIndex);
                itemsList.Insert(elementIndex - 1, e);
                itemsDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = itemsList.IndexOf((Item)dataControl.getContent());

            if (elementIndex < itemsList.Count - 1)
            {
                Item e = itemsList[elementIndex];
                ItemDataControl c = itemsDataControlList[elementIndex];
                itemsList.RemoveAt(elementIndex);
                itemsDataControlList.RemoveAt(elementIndex);
                itemsList.Insert(elementIndex + 1, e);
                itemsDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Iterate through each item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                itemDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.ITEMS_LIST);

            // Iterate through the items
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
            {
                string itemPath = currentPath + " >> " + itemDataControl.getId();
                valid &= itemDataControl.isValid(itemPath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                count += itemDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                itemDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                itemDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                count += itemDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                itemDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every item
            foreach (ItemDataControl itemDataControl in itemsDataControlList)
                itemDataControl.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            // TODO Auto-generated method stub
            return false;
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.itemsDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, itemsDataControlList.Cast<Searchable>().ToList());
        }
    }
}
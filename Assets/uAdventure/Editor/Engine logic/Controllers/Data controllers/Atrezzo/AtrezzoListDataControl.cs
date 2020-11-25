using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AtrezzoListDataControl : DataControl
    {
        /**
            * List of atrezzo items.
            */
        private List<Atrezzo> atrezzoList;

        /**
         * List of atrezzo item controllers.
         */
        private List<AtrezzoDataControl> atrezzoDataControlList;

        /**
         * Constructor.
         * 
         * @param itemsList
         *            List of items
         */
        public AtrezzoListDataControl(List<Atrezzo> atrezzoList)
        {

            this.atrezzoList = atrezzoList;

            // Create subcontrollers
            atrezzoDataControlList = new List<AtrezzoDataControl>();
            foreach (Atrezzo atrezzo in atrezzoList)
                atrezzoDataControlList.Add(new AtrezzoDataControl(atrezzo));
        }

        /**
         * Returns the list of atrezzo item controllers.
         * 
         * 
         * @return Atrezzo controllers
         */
        public List<AtrezzoDataControl> getAtrezzoList()
        {

            return atrezzoDataControlList;
        }

        /**
         * Returns the last atrezzo item controller from the list.
         * 
         * @return Last item controller
         */
        public AtrezzoDataControl getLastAtrezzo()
        {

            return atrezzoDataControlList[atrezzoDataControlList.Count - 1];
        }

        /**
         * Returns the info of the atrezzo items contained in the list.
         * 
         * @return Array with the information of the atrezzo items. It contains the
         *         identifier of each atrezzo item.
         */
        public string[][] getItemsInfo()
        {

            string[][] atrezzoInfo = null;

            // Create the list for the items
            atrezzoInfo = new string[atrezzoList.Count][];
            for (int i = 0; i < atrezzoList.Count; i++)
                atrezzoInfo[i] = new string[1];

            // Fill the array with the info
            for (int i = 0; i < atrezzoList.Count; i++)
                atrezzoInfo[i][0] = atrezzoList[i].getId();

            return atrezzoInfo;
        }

        public string[] getItemIDs()
        {
            string[] atrezzoInfo = null;

            // Create the list for the items
            atrezzoInfo = new string[atrezzoList.Count];

            // Fill the array with the info
            for (int i = 0; i < atrezzoList.Count; i++)
                atrezzoInfo[i] = atrezzoList[i].getId();
            return atrezzoInfo;
        }

        public int getAtrezzoIndexByID(string id)
        {
            for (int i = 0; i < atrezzoList.Count; i++)
            {
                if (atrezzoList[i].getId().Equals(id))
                    return i;
            }
            return -1;
        }

        public override bool addElement(int type, string atrezzoId)
        {
            bool elementAdded = false;

            if (type == Controller.ATREZZO)
            {
                // Show a dialog asking for the item id
                if (string.IsNullOrEmpty(atrezzoId))
                    controller.ShowInputIdDialog(TC.get("Operation.AddAtrezzoTitle"), TC.get("Operation.AddAtrezzoMessage"),
                        Controller.Instance.makeElementValid(TC.get("Operation.AddAtrezzoDefaultValue")), performAddElement);
                else
                {
                    elementAdded = true;
                    performAddElement(null, atrezzoId);
                }
            }

            return elementAdded;
        }

        private void performAddElement(object sender, string atrezzoId)
        {
            // If some value was typed and the identifier is valid
            if (!controller.isElementIdValid(atrezzoId))
                atrezzoId = controller.makeElementValid(atrezzoId);

            // Add thew new item
            Atrezzo newAtrezzo = new Atrezzo(atrezzoId);
            atrezzoList.Add(newAtrezzo);
            atrezzoDataControlList.Add(new AtrezzoDataControl(newAtrezzo));
            controller.IdentifierSummary.addId<Atrezzo>(atrezzoId);
            controller.DataModified();
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is AtrezzoDataControl))
                return false;

            Atrezzo newElement = (Atrezzo)(((Atrezzo)(dataControl.getContent())).Clone());
			string id = newElement.getId();
			if (!controller.isElementIdValid(id))
				id = controller.makeElementValid(id);
			
            newElement.setId(id);
            atrezzoList.Add(newElement);
            atrezzoDataControlList.Add(new AtrezzoDataControl(newElement));
            controller.IdentifierSummary.addId<Atrezzo>(id);
            return true;

        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddAtrezzoDefaultValue");
        }


        public override bool canAddElement(int type)
        {

            // It can always add new atrezzo items
            return type == Controller.ATREZZO;
        }


        public override bool canBeDeleted()
        {

            return false;
        }


        public override bool canBeDuplicated()
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


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each atrezzo item
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
                count += atrezzoDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each atrezzo item
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
                count += atrezzoDataControl.countIdentifierReferences(id);

            return count;

        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each atrezzo item
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
                atrezzoDataControl.deleteIdentifierReferences(assetPath);

        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            bool elementDeleted = false;
            string atrezzoId = ((AtrezzoDataControl)dataControl).getId();
            string references = controller.countIdentifierReferences(atrezzoId).ToString();

            // Ask for confirmation
            if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { atrezzoId, references })))
            {
                if (atrezzoList.Remove((Atrezzo)dataControl.getContent()))
                {
                    atrezzoDataControlList.Remove((AtrezzoDataControl)dataControl);
                    controller.deleteIdentifierReferences(atrezzoId);
                    controller.IdentifierSummary.deleteId<Atrezzo>(atrezzoId);
                    controller.updateVarFlagSummary();
                    controller.DataModified();
                    elementDeleted = true;
                }
            }

            return elementDeleted;
        }


        public override void deleteIdentifierReferences(string id)
        {

            // This method is empty

        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.ATREZZO };
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each atrezzo item
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
                atrezzoDataControl.getAssetReferences(assetPaths, assetTypes);

        }


        public override System.Object getContent()
        {

            return atrezzoList;
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.ATREZZO_LIST);

            // Iterate through the atrezzo items
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
            {
                string itemPath = currentPath + " >> " + atrezzoDataControl.getId();
                valid &= atrezzoDataControl.isValid(itemPath, incidences);
            }

            return valid;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = atrezzoList.IndexOf((Atrezzo)dataControl.getContent());

            if (elementIndex < atrezzoList.Count - 1)
            {
                Atrezzo e = atrezzoList[elementIndex];
                AtrezzoDataControl c = atrezzoDataControlList[elementIndex];
                atrezzoList.RemoveAt(elementIndex);
                atrezzoDataControlList.RemoveAt(elementIndex);
                atrezzoList.Insert(elementIndex - 1, e);
                atrezzoDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = atrezzoList.IndexOf((Atrezzo)dataControl.getContent());

            if (elementIndex > 0)
            {
                Atrezzo e = atrezzoList[elementIndex];
                AtrezzoDataControl c = atrezzoDataControlList[elementIndex];
                atrezzoList.RemoveAt(elementIndex);
                atrezzoDataControlList.RemoveAt(elementIndex);
                atrezzoList.Insert(elementIndex + 1, e);
                atrezzoDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each item
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
                atrezzoDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Iterate through each item
            foreach (AtrezzoDataControl atrezzoDataControl in atrezzoDataControlList)
                atrezzoDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.atrezzoDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, atrezzoDataControlList.Cast<Searchable>().ToList());
        }

    }
}
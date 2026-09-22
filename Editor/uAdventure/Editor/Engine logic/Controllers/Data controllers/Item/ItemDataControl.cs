using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = System.Object;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ItemDataControl : DataControlWithResources
    {

        /**
         * Contained item.
         */
        private readonly Item item;

        /**
         * Actions list controller.
         */
        private readonly ActionsListDataControl actionsListDataControl;

        /**
         * Controller for descriptions
         */
        private readonly DescriptionsController descriptionController;

        /**
         * Constructor.
         * 
         * @param item
         *            Contained item
         */
        public ItemDataControl(Item item)
        {

            this.item = item;
            this.resourcesList = item.getResources();

            selectedResources = 0;

            // Add a new resource if the list is empty
            if (resourcesList.Count == 0)
                resourcesList.Add(new ResourcesUni());

            // Create the subcontrollers
            resourcesDataControlList = new List<ResourcesDataControl>();
            foreach (ResourcesUni resources in resourcesList)
            {
                resourcesDataControlList.Add(new ResourcesDataControl(resources, Controller.ITEM));
            }

            actionsListDataControl = new ActionsListDataControl(item.getActions(), this);

            descriptionController = new DescriptionsController(item.getDescriptions());
        }

        /**
         * Returns the actions list controller.
         * 
         * @return Actions list controller
         */
        public ActionsListDataControl getActionsList()
        {

            return actionsListDataControl;
        }

        /**
         * Returns the path to the selected preview image.
         * 
         * @return Path to the image, null if not present
         */
        public string getPreviewImage()
        {

            return resourcesDataControlList[selectedResources].getAssetPath("image");
        }

        public void setPreviewImage(string val)
        {
            resourcesDataControlList[selectedResources].addAsset("image", val);
        }

        /**
       * Returns the path to the selected preview image.
       * 
       * @return Path to the image, null if not present
       */
        public string getIconImage()
        {
            return resourcesDataControlList[selectedResources].getAssetPath("icon");
        }
        public void setIconImage(string val)
        {
            resourcesDataControlList[selectedResources].addAsset("icon", val);
        }
        /**
       * Returns the path to the selected preview image.
       * 
       * @return Path to the image, null if not present
       */
        public string getMouseOverImage()
        {
            return resourcesDataControlList[selectedResources].getAssetPath("imageover");
        }
        public void setMouseOverImage(string val)
        {
            resourcesDataControlList[selectedResources].addAsset("imageover", val);
        }

        /**
         * Returns the id of the item.
         * 
         * @return Item's id
         */
        public string getId()
        {

            return item.getId();
        }

        /**
         * Returns the documentation of the item.
         * 
         * @return Item's documentation
         */
        public string getDocumentation()
        {

            return item.getDocumentation();
        }

        public void setDocumentation(string val)
        {
            item.setDocumentation(val);
        }


        public void setReturnsWhenDragged(bool returnsWhenDragged)
        {
            if (returnsWhenDragged != item.isReturnsWhenDragged())
            {
                Controller.Instance.AddTool(new ChangeBooleanValueTool(item, returnsWhenDragged, "isReturnsWhenDragged", "setReturnsWhenDragged"));
                Controller.Instance.DataModified();
            }
        }

        public bool isReturnsWhenDragged()
        {
            return item.isReturnsWhenDragged();
        }


        public override System.Object getContent()
        {

            return item;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.RESOURCES };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new resources
            return type == Controller.RESOURCES;
        }


        public override bool canBeDeleted()
        {

            return true;
        }


        public override bool canBeMoved()
        {

            return true;
        }


        public override bool canBeRenamed()
        {

            return true;
        }


        public override bool addElement(int type, string id)
        {

            bool elementAdded = false;

            if (type == Controller.RESOURCES)
            {
                elementAdded = Controller.Instance.AddTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, Controller.ITEM, this));
            }

            return elementAdded;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

            if (elementIndex > 0)
            {
                ResourcesUni e = resourcesList[elementIndex];
                ResourcesDataControl c = resourcesDataControlList[elementIndex];
                resourcesList.RemoveAt(elementIndex);
                resourcesDataControlList.RemoveAt(elementIndex);
                resourcesList.Insert(elementIndex - 1, e);
                resourcesDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

            if (elementIndex < resourcesList.Count - 1)
            {
                ResourcesUni e = resourcesList[elementIndex];
                ResourcesDataControl c = resourcesDataControlList[elementIndex];
                resourcesList.RemoveAt(elementIndex);
                resourcesDataControlList.RemoveAt(elementIndex);
                resourcesList.Insert(elementIndex + 1, e);
                resourcesDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string newName)
        {
            string oldItemId = item.getId();
            string references = controller.countIdentifierReferences(oldItemId).ToString();

            // Ask for confirmation
            if (newName != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameItemTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldItemId, references })))
            {

                // Show a dialog asking for the new item id
                if (newName == null)
                {
                    controller.ShowInputDialog(TC.get("Operation.RenameItemTitle"), TC.get("Operation.RenameItemMessage"), oldItemId, (o, s) => performRenameElement(s));
                }
                else
                {
                    return performRenameElement(newName);
                }
            }

            return null;
        }

        private string performRenameElement(string newItemId)
        {
            string oldItemId = item.getId();


            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newItemId))
            {
                newItemId = controller.makeElementValid(newItemId);
            }

            item.setId(newItemId);
            controller.replaceIdentifierReferences(oldItemId, newItemId);
            controller.IdentifierSummary.deleteId<Item>(oldItemId);
            controller.IdentifierSummary.addId<Item>(newItemId);
            controller.DataModified();

            return newItemId;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            actionsListDataControl.updateVarFlagSummary(varFlagSummary);

            //1.4
            descriptionController.updateVarFlagSummary(varFlagSummary);

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            {
                resourcesDataControl.updateVarFlagSummary(varFlagSummary);
            }
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Iterate through the resources
            for (int i = 0; i < resourcesDataControlList.Count; i++)
            {
                string resourcesPath = currentPath + " >> " + TC.getElement(Controller.RESOURCES) + " #" + (i + 1);
                valid &= resourcesDataControlList[i].isValid(resourcesPath, incidences);
            }

            // Spread the call to the actions
            valid &= actionsListDataControl.isValid(currentPath, incidences);

            //1.4
            valid &= descriptionController.isValid(currentPath, incidences);

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            {
                count += resourcesDataControl.countAssetReferences(assetPath);
            }

            // Add the references in the actions
            count += actionsListDataControl.countAssetReferences(assetPath);

            //v1.4
            count += this.descriptionController.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            {
                resourcesDataControl.getAssetReferences(assetPaths, assetTypes);
            }

            // Add the references in the actions
            actionsListDataControl.getAssetReferences(assetPaths, assetTypes);

            //v1.4
            this.descriptionController.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            {
                resourcesDataControl.deleteAssetReferences(assetPath);
            }

            // Delete the references from the actions
            actionsListDataControl.deleteAssetReferences(assetPath);

            //1.4
            this.descriptionController.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            return actionsListDataControl.countIdentifierReferences(id) + this.descriptionController.countIdentifierReferences(id);
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            actionsListDataControl.replaceIdentifierReferences(oldId, newId);

            //1.4
            descriptionController.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            actionsListDataControl.deleteIdentifierReferences(id);

            //1.4
            this.descriptionController.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {
            check(this.getDocumentation(), TC.get("Search.Documentation"));
            check(this.getId(), "ID");
            check(this.getPreviewImage(), TC.get("Search.PreviewImage"));
            this.descriptionController.recursiveSearch();
            this.getActionsList().recursiveSearch();
            foreach (ResourcesDataControl r in resourcesDataControlList)
            {
                r.recursiveSearch();
            }
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            List<Searchable> path = getPathFromChild(dataControl, resourcesDataControlList.Cast<Searchable>().ToList());
            if (path != null)
            {
                return path;
            }
            path = getPathFromChild(dataControl, this.descriptionController);
            if (path != null)
            {
                return path;
            }
            path = getPathFromChild(dataControl, actionsListDataControl);
            return path;
        }


        public DescriptionsController getDescriptionController()
        {

            return descriptionController;
        }

        //v1.4
        public void setBehaviour(Item.BehaviourType behaviour)
        {
            if (behaviour != item.getBehaviour())
            {
                Controller.Instance.AddTool(new ChangeIntegerValueTool(item, (int)behaviour, "getBehaviourInteger", "setBehaviourInteger"));
                Controller.Instance.DataModified();
            }
        }

        public Item.BehaviourType getBehaviour()
        {
            return item.getBehaviour();
        }

        public long getResourcesTransitionTime()
        {
            return item.getResourcesTransitionTime();
        }

        public void setResourcesTransitionTime(long resourcesTransitionTime)
        {
            if (resourcesTransitionTime != item.getResourcesTransitionTime())
            {
                Controller.Instance.AddTool(new ChangeLongValueTool(item, resourcesTransitionTime, "getResourcesTransitionTime", "setResourcesTransitionTime"));
                Controller.Instance.DataModified();
            }
        }
    }
}
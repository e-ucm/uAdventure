using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class DataControlWithResources : DataControl
    {
        /**
         * List of resources.
         */
        protected List<ResourcesUni> resourcesList;

        /**
         * List of resources controllers.
         */
        protected List<ResourcesDataControl> resourcesDataControlList;

        /**
         * The resources that must be used in the previews.
         */
        protected int selectedResources;

        public List<ResourcesDataControl> getResources()
        {

            return resourcesDataControlList;
        }

        public int getResourcesCount()
        {

            return resourcesDataControlList.Count;
        }

        /**
         * Returns the last resources controller of the list.
         * 
         * @return Last resources controller
         */
        public ResourcesDataControl getLastResources()
        {

            return resourcesDataControlList[resourcesDataControlList.Count - 1];
        }

        /**
         * Returns the selected resources block of the list.
         * 
         * @return Selected block of resources
         */
        public int getSelectedResources()
        {

            return selectedResources;
        }

        /**
         * Sets the new selected resources block of the list.
         * 
         * @param selectedResources
         *            New selected block of resources
         */
        public void setSelectedResources(int selectedResources)
        {

            this.selectedResources = selectedResources;
        }

        // This method only caters for deleting RESOURCES. Subclasses should override this method
        // to implement removal of other element types
        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return controller.AddTool(new DeleteResourcesBlockTool(resourcesList, resourcesDataControlList, dataControl, this));
        }

        public bool duplicateResources(DataControl dataControl)
        {

            return controller.AddTool(new DuplicateResourcesBlockTool(dataControl, resourcesList, resourcesDataControlList, this));
        }
    }
}
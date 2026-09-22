using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DuplicateResourcesBlockTool : Tool
    {

        private DataControl dataControl;

        /**
         * List of resources.
         */
        protected List<ResourcesUni> resourcesList;

        /**
         * List of resources controllers.
         */
        protected List<ResourcesDataControl> resourcesDataControlList;

        // Temp data
        private ResourcesUni newElement;

        private ResourcesDataControl newDataControl;

        private DataControlWithResources parent;

        public DuplicateResourcesBlockTool(DataControl dataControl, List<ResourcesUni> resourcesList, List<ResourcesDataControl> resourcesDataControlList, DataControlWithResources parent)
        {

            this.dataControl = dataControl;
            this.resourcesList = resourcesList;
            this.resourcesDataControlList = resourcesDataControlList;
            this.parent = parent;
        }


        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return true;
        }


        public override bool combine(Tool other)
        {

            return false;
        }


        public override bool doTool()
        {

            if (!(dataControl is ResourcesDataControl))
                return false;

            newElement = (ResourcesUni)(((ResourcesUni)(dataControl.getContent())));
            newDataControl = new ResourcesDataControl(newElement, ((ResourcesDataControl)dataControl).getResourcesType());
            resourcesList.Add(newElement);
            resourcesDataControlList.Add(newDataControl);
            parent.setSelectedResources(resourcesList.Count - 1);
            Controller.Instance.updateVarFlagSummary();
            return true;

        }


        public override bool redoTool()
        {

            resourcesList.Add(newElement);
            resourcesDataControlList.Add(newDataControl);
            parent.setSelectedResources(resourcesList.Count - 1);
            Controller.Instance.updateVarFlagSummary();
            Controller.Instance.reloadPanel();
            return true;
        }


        public override bool undoTool()
        {

            bool undone = resourcesList.Remove(newElement) && resourcesDataControlList.Remove(newDataControl);
            if (undone)
            {
                parent.setSelectedResources(resourcesList.Count - 1);
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.reloadPanel();
            }
            return undone;
        }
    }
}
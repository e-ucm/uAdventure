using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AddResourcesBlockTool : Tool
    {

        /**
         * Arguments
         */
        private List<ResourcesUni> resourcesList;

        private List<ResourcesDataControl> resourcesDataControlList;

        private int resourcesType;

        private DataControlWithResources parent;

        /*
         * Temporal data for undo/redo
         */
        private ResourcesUni newResources;

        private ResourcesDataControl newResourcesDataControl;

        public AddResourcesBlockTool(List<ResourcesUni> resourcesList, List<ResourcesDataControl> resourcesDataControlList, int resourcesType, DataControlWithResources parent)
        {

            this.resourcesList = resourcesList;
            this.resourcesDataControlList = resourcesDataControlList;
            this.resourcesType = resourcesType;
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

            newResources = new ResourcesUni();
            newResourcesDataControl = new ResourcesDataControl(newResources, resourcesType);
            resourcesList.Add(newResources);
            resourcesDataControlList.Add(newResourcesDataControl);
            return true;
        }


        public override bool redoTool()
        {

            resourcesList.Add(newResources);
            resourcesDataControlList.Add(newResourcesDataControl);
            parent.setSelectedResources(resourcesList.Count - 1);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            bool undone = resourcesList.Remove(newResources) && resourcesDataControlList.Remove(newResourcesDataControl);

            if (undone)
            {
                parent.setSelectedResources(resourcesList.Count - 1);
                Controller.Instance.updatePanel();
            }
            return undone;
        }
    }
}
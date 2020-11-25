using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteResourcesBlockTool : Tool
    {
        /**
       * Arguments
       */
        private List<ResourcesUni> resourcesList;

        private List<ResourcesDataControl> resourcesDataControlList;

        // The data control (Resources) to be removed
        private DataControl dataControl;

        // The parent of the resources block. This is required for updating selectedResources
        private DataControlWithResources parentDataControl;

        /*
         * Elements for UNDO REDO
         */
        private int lastSelectedResources;

        private int resourcesIndex;

        public DeleteResourcesBlockTool(List<ResourcesUni> resourcesList, List<ResourcesDataControl> resourcesDataControlList, DataControl dataControl, DataControlWithResources parentDataControl)
        {

            this.resourcesDataControlList = resourcesDataControlList;
            this.resourcesList = resourcesList;
            this.dataControl = dataControl;
            this.parentDataControl = parentDataControl;
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

            bool elementDeleted = false;

            // Delete the block only if it is not the last one
            lastSelectedResources = parentDataControl.getSelectedResources();
            if (resourcesList.Count > 1)
            {
                if (resourcesList.Remove((ResourcesUni)dataControl.getContent()))
                {
                    resourcesIndex = resourcesDataControlList.IndexOf((ResourcesDataControl)dataControl);
                    resourcesDataControlList.Remove((ResourcesDataControl)dataControl);

                    int selectedResources = parentDataControl.getSelectedResources();
                    // Decrease the selected index if necessary
                    if (selectedResources > 0 && selectedResources >= resourcesIndex)
                    {
                        parentDataControl.setSelectedResources(selectedResources - 1);
                    }

                    Controller.Instance.updateVarFlagSummary();
                    Controller.Instance.DataModified();
                    elementDeleted = true;
                }
            }

            // If it was the last one, show an error message
            else
                Controller.Instance.ShowErrorDialog(TC.get("Operation.DeleteResourcesTitle"), TC.get("Operation.DeleteResourcesErrorLastResources"));

            return elementDeleted;
        }


        public override bool redoTool()
        {

            bool redone = doTool();
            if (redone)
            {
                Controller.Instance.updatePanel();
            }
            return redone;
        }


        public override bool undoTool()
        {

            // Add deleted elements
            resourcesList.Insert(resourcesIndex, (ResourcesUni)dataControl.getContent());
            resourcesDataControlList.Insert(resourcesIndex, (ResourcesDataControl)dataControl);
            parentDataControl.setSelectedResources(lastSelectedResources);
            Controller.Instance.reloadPanel();
            Controller.Instance.updateVarFlagSummary();
            return true;
        }
    }
}
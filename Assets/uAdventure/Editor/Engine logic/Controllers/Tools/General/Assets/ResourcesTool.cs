using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Abstract class for ResourcesUni modification. It contains the common data that
     * tools EditResourceTool, SetResourceTool and DeleteResourceTool will use.
     */
    public abstract class ResourcesTool : Tool
    {

        /**
         * Controller
         */
        protected Controller controller;

        /**
         * Contained resources. This field is kept updated all the time.
         */
        protected ResourcesUni resources;

        /**
         * Old resources. This is a backup copy that is done when the tool is built
         * (for undo)
         */
        protected ResourcesUni oldResourcesUni;

        /**
         * The assets information of the resources.
         */
        protected AssetInformation[] assetsInformation;

        /**
         * indicates if the resource block belongs to a NPC, the player or other
         * element
         */
        protected int resourcesType;

        /**
         * The index of the resource to be modified
         */
        protected int index;

        /**
         * Default constructor
         * 
         * @throws CloneNotSupportedException
         */
        public ResourcesTool(ResourcesUni resources, AssetInformation[] assetsInformation, int resourcesType, int index)
        {

            this.resources = resources;
            this.assetsInformation = assetsInformation;
            this.resourcesType = resourcesType;
            this.controller = Controller.Instance;
            this.index = index;
            this.oldResourcesUni = resources;
        }

        public override bool undoTool()
        {

            // Restores the resources object with the information stored in oldResoures
            //try
            //{
            ResourcesUni temp = resources;
            resources.clearAssets();
            string[] oldResourceTypes = oldResourcesUni.getAssetTypes();
            foreach (string type in oldResourceTypes)
            {
                resources.addAsset(type, oldResourcesUni.getAssetPath(type));
            }

            // Update older data
            oldResourcesUni.clearAssets();
            oldResourceTypes = temp.getAssetTypes();
            foreach (string type in oldResourceTypes)
            {
                oldResourcesUni.addAsset(type, temp.getAssetPath(type));
            }
            //			controller.reloadPanel();
            return true;
            //}
            //catch (CloneNotSupportedException e)
            //{
            //    e.printStackTrace();
            //    return false;
            //}
        }

        public override bool redoTool()
        {

            return undoTool();
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
    }
}
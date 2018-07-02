using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class CustomActionDataControl : ActionDataControl
    {
        /**
             * Contained customAction structure
             */
        private CustomAction customAction;

        /**
         * Default constructor
         * 
         * @param action
         *            the custom Action
         */
        public CustomActionDataControl(CustomAction action) : base(action)
        {

            customAction = action;

            this.resourcesList = customAction.getResources();
            if (this.resourcesList.Count == 0)
                this.resourcesList.Add(new ResourcesUni());
            selectedResources = 0;

            resourcesDataControlList = new List<ResourcesDataControl>();
            foreach (ResourcesUni resources in resourcesList)
            {
                resourcesDataControlList.Add(new ResourcesDataControl(resources, Controller.ACTION_CUSTOM));
            }

        }

        /**
         * @param name
         *            the name to set
         */
        public void setName(string name)
        {

            controller.AddTool(new ChangeNameTool(customAction, name));
        }


        /**
         * @return the value of name
         */
        public string getName()
        {

            return customAction.getName();
        }


        public override void recursiveSearch()
        {
            base.recursiveSearch();
            check(this.getName(), TC.get("Search.Name"));
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, resourcesDataControlList.Cast<Searchable>().ToList());
        }



        public override bool addElement(int type, string id)
        {
            bool elementAdded = false;

            if (type == Controller.RESOURCES)
            {
                elementAdded = Controller.Instance.AddTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, Controller.ACTION_CUSTOM, this));
            }

            return elementAdded;

        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return controller.AddTool(new DeleteResourcesBlockTool(resourcesList, resourcesDataControlList, dataControl, this));
        }
    }
}
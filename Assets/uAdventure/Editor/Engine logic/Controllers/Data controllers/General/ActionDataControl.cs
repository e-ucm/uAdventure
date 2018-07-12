using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ActionDataControl : DataControlWithResources
    {

        private static readonly System.Type[] allTypes = { typeof(ActiveArea), typeof(NPC), typeof(Item) };
        private static readonly System.Type[] itemBasedTypes = { typeof(NPC), typeof(Item) };

        /**
         * Contained action structure.
         */
        private readonly Action action;

        /**
         * Type of the action.
         */
        private readonly int actionType;

        /**
         * Conditions controller.
         */
        private readonly ConditionsController conditionsController;

        /**
         * Effects controller
         */
        private readonly EffectsController effectsController;

        /**
         * Not-Effects controller
         */
        private readonly EffectsController notEffectsController;

        /**
         * Contructor.
         * 
         * @param action
         *            Next scenes of the data control structure
         *            
         *            
         */
        public ActionDataControl(Action action, string name)
        {

            this.action = action;

            this.resourcesList = new List<ResourcesUni>();
            this.resourcesDataControlList = new List<ResourcesDataControl>();

            actionType = Controller.ACTION_TALK_TO;

            // Create subcontrollers
            conditionsController = new ConditionsController(action.getConditions(), actionType, name);
            effectsController = new EffectsController(action.getEffects());
            notEffectsController = new EffectsController(action.getNotEffects());

        }


        public ActionDataControl(Action action)
        {

            this.action = action;

            this.resourcesList = new List<ResourcesUni>();
            this.resourcesDataControlList = new List<ResourcesDataControl>();

            string actionName = null;
            // Store the type of the action
            switch (action.getType())
            {
                case Action.EXAMINE:
                    actionType = Controller.ACTION_EXAMINE;
                    actionName = "";
                    break;
                case Action.GRAB:
                    actionType = Controller.ACTION_GRAB;
                    actionName = "";
                    break;
                case Action.USE:
                    actionType = Controller.ACTION_USE;
                    actionName = "";
                    break;
                case Action.CUSTOM:
                    actionType = Controller.ACTION_CUSTOM;
                    CustomAction custom = (CustomAction)action;
                    actionName = custom.getName();
                    break;
                case Action.USE_WITH:
                    actionType = Controller.ACTION_USE_WITH;
                    actionName = action.getTargetId();
                    break;
                case Action.GIVE_TO:
                    actionType = Controller.ACTION_GIVE_TO;
                    actionName = action.getTargetId();
                    break;
                case Action.CUSTOM_INTERACT:
                    actionType = Controller.ACTION_CUSTOM_INTERACT;
                    CustomAction custom2 = (CustomAction)action;
                    actionName = custom2.getName() + " " + action.getTargetId();
                    break;
                case Action.TALK_TO:
                    actionType = Controller.ACTION_TALK_TO;
                    actionName = action.getTargetId();
                    break;
                case Action.DRAG_TO:
                    actionType = Controller.ACTION_DRAG_TO;
                    actionName = action.getTargetId();
                    break;
            }

            // Create subcontrollers
            conditionsController = new ConditionsController(action.getConditions(), actionType, actionName);
            effectsController = new EffectsController(action.getEffects());
            notEffectsController = new EffectsController(action.getNotEffects());
        }

        /**
         * Returns the conditions of the action.
         * 
         * @return Conditions of the action
         */
        public ConditionsController getConditions()
        {

            return conditionsController;
        }

        /**
         * Returns the effects of the action.
         * 
         * @return Effects of the action
         */
        public EffectsController getEffects()
        {

            return effectsController;
        }

        /**
         * Returns the type of the contained effect.
         * 
         * @return Type of the contained effect
         */
        public int getType()
        {

            return actionType;
        }

        public string getTypeName()
        {
            string name = "";

            switch (action.getType())
            {
                case Action.EXAMINE:
                    name = "Examine";
                    break;
                case Action.GRAB:
                    name = "Grab";
                    break;
                case Action.USE:
                    name = "Use";
                    break;
                case Action.CUSTOM:
                    name = "Custom";
                    break;
                case Action.USE_WITH:
                    name = "Use with";
                    break;
                case Action.GIVE_TO:
                    name = "Give to";
                    break;
                case Action.CUSTOM_INTERACT:
                    name = "Custom interact";
                    break;
                case Action.TALK_TO:
                    name = "Talk to";
                    break;
                case Action.DRAG_TO:
                    name = "Drag to";
                    break;
            }
            return name;
        }

        /**
         * Returns whether the action accepts id target or not.
         * 
         * @return True if the action accepts id target, false otherwise
         */
        public bool hasIdTarget()
        {

            // The use-with and give-to actions accept id target
            return action.getType() == Action.USE_WITH || action.getType() == Action.GIVE_TO || action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.DRAG_TO;
        }

        /**
         * Returns the list of elements to select for the actions with targets.
         * 
         * @return List of elements, null if the action doesn't accept targets
         */
        public string[] getElementsList()
        {

            string[] elements = null;

            switch (action.getType())
            {
                default:
                    Debug.LogError("Wrong action type: " + action.getType());
                    break;
                case Action.GIVE_TO:
                    elements = controller.IdentifierSummary.getIds<NPC>();
                    break;
                case Action.USE_WITH:
                    elements = controller.IdentifierSummary.combineIds(itemBasedTypes);
                    break;
                case Action.DRAG_TO:
                case Action.CUSTOM_INTERACT:
                    elements = controller.IdentifierSummary.combineIds(allTypes);
                    break;
            }

            return elements;
        }

        /**
         * Returns the target id of the contained effect.
         * 
         * @return Target id of the contained effect
         */
        public string getIdTarget()
        {

            return action.getTargetId();
        }

        /**
         * Returns the documentation of the action.
         * 
         * @return Action's documentation
         */
        public string getDocumentation()
        {

            return action.getDocumentation();
        }

        /**
         * Sets the new documentation of the action.
         * 
         * @param documentation
         *            Documentation of the action
         */
        public void setDocumentation(string documentation)
        {

            controller.AddTool(new ChangeDocumentationTool(action, documentation));
        }

        /**
         * Sets the new id target of the action.
         * 
         * @param idTarget
         *            Id target of the action
         */
        public void setIdTarget(string idTarget)
        {

            controller.AddTool(new ChangeTargetIdTool(action, idTarget, true, false));
        }

        public override System.Object getContent()
        {

            return action;
        }


        public override int[] getAddableElements()
        {

            return new int[] { };
        }


        public override bool canAddElement(int type)
        {

            return false;
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

            return false;
        }


        public override bool addElement(int type, string id)
        {

            return false;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }


        public override string renameElement(string name)
        {

            //TODO: IS this right?
            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Update the flag summary with the effects of the action
            EffectsController.updateVarFlagSummary(varFlagSummary, action.getEffects());
            if (action.getNotEffects() != null)
                EffectsController.updateVarFlagSummary(varFlagSummary, action.getNotEffects());
            ConditionsController.updateVarFlagSummary(varFlagSummary, action.getConditions());
            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                    resourcesDataControl.updateVarFlagSummary(varFlagSummary);
            }
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;
            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                // Iterate through the resources
                for (int i = 0; i < resourcesDataControlList.Count; i++)
                {
                    string resourcesPath = currentPath + " >> " + TC.getElement(Controller.RESOURCES) + " #" + (i + 1);
                    valid &= resourcesDataControlList[i].isValid(resourcesPath, incidences);
                }
            }

            // Check the effects of the action
            valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.Effects"), incidences, action.getEffects());
            valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.NotEffects"), incidences, action.getNotEffects());
            // valid &= EffectsController.isValid( currentPath + " >> " + TC.get( "Element.Effects" ), incidences, action.getClickEffects( ) );
            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;
            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                // Iterate through the resources
                foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                    count += resourcesDataControl.countAssetReferences(assetPath);
            }
            // Return the asset references from the effects
            count += EffectsController.countAssetReferences(assetPath, action.getEffects());
            count += EffectsController.countAssetReferences(assetPath, action.getNotEffects());
            //  count += EffectsController.countAssetReferences( assetPath, action.getClickEffects( ) );
            return count;
        }


        public override void deleteAssetReferences(string assetPath)
        {
            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                // Iterate through the resources
                foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                    resourcesDataControl.deleteAssetReferences(assetPath);
            }
            EffectsController.deleteAssetReferences(assetPath, action.getEffects());
            EffectsController.deleteAssetReferences(assetPath, action.getNotEffects());
            // EffectsController.deleteAssetReferences( assetPath, action.getClickEffects( ) );
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // If the action references to the given identifier, increase the counter
            if ((action.getType() == Action.GIVE_TO || action.getType() == Action.USE_WITH || action.getType() == Action.DRAG_TO || action.getType() == Action.CUSTOM_INTERACT) && action.getTargetId().Equals(id))
                count++;

            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                // Iterate through the resources
                foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                    resourcesDataControl.countIdentifierReferences(id);
            }

            // Add to the counter the references in the effects block
            count += EffectsController.countIdentifierReferences(id, action.getEffects());
            count += EffectsController.countIdentifierReferences(id, action.getNotEffects());
            //  count += EffectsController.countIdentifierReferences( id, action.getClickEffects( ) );

            count += conditionsController.countIdentifierReferences(id);
            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                // Iterate through the resources
                foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                    resourcesDataControl.replaceIdentifierReferences(oldId, newId);
            }
            // Only the "Give to" and "Use with" have item references
            if ((action.getType() == Action.GIVE_TO || action.getType() == Action.USE_WITH || action.getType() == Action.DRAG_TO || action.getType() == Action.CUSTOM_INTERACT) && action.getTargetId().Equals(oldId))
                action.setTargetId(newId);

            EffectsController.replaceIdentifierReferences(oldId, newId, action.getEffects());
            EffectsController.replaceIdentifierReferences(oldId, newId, action.getNotEffects());
            //  EffectsController.replaceIdentifierReferences( oldId, newId, action.getClickEffects( ) );
            conditionsController.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {
            if (action.getType() == Action.CUSTOM_INTERACT || action.getType() == Action.CUSTOM)
            {
                // Iterate through the resources
                foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                    resourcesDataControl.deleteIdentifierReferences(id);
            }

            EffectsController.deleteIdentifierReferences(id, action.getEffects());
            EffectsController.deleteIdentifierReferences(id, action.getNotEffects());
            //  EffectsController.deleteIdentifierReferences( id, action.getClickEffects( ) );
            conditionsController.deleteIdentifierReferences(id);
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {


            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                resourcesDataControl.getAssetReferences(assetPaths, assetTypes);
            EffectsController.getAssetReferences(assetPaths, assetTypes, action.getEffects());
            EffectsController.getAssetReferences(assetPaths, assetTypes, action.getNotEffects());
            //   EffectsController.getAssetReferences( assetPaths, assetTypes, action.getClickEffects( ) );
        }


        public override bool canBeDuplicated()
        {

            return true;
        }

        /**
         * @return the value of needsGoTo
         */
        public bool getNeedsGoTo()
        {

            return action.isNeedsGoTo();
        }

        /**
         * @param needsGoTo
         *            the needsGoTo to set
         */
        public void setNeedsGoTo(bool needsGoTo)
        {
            controller.AddTool(new ChangeBooleanValueTool(action, needsGoTo, "isNeedsGoTo", "setNeedsGoTo"));
        }

        /**
         * @return the value of keepDistance
         */
        public int getKeepDistance()
        {

            return action.getKeepDistance();
        }

        /**
         * @param keepDistance
         *            the keepDistance to set
         */
        public void setKeepDistance(int keepDistance)
        {
            controller.AddTool(new ChangeIntegerValueTool(action, keepDistance, "getKeepDistance", "setKeepDistance"));
        }

        /**
         * Change activated not effects
         * 
         * @param activated
         */
        public void setActivatedNotEffects(bool activated)
        {

            action.setActivatedNotEffects(activated);
        }

        public bool isActivatedNotEffects()
        {

            return action.isActivatedNotEffects();
        }

        /**
         * @return the notEffectsController
         */
        public EffectsController getNotEffectsController()
        {
            return notEffectsController;
        }


        public override void recursiveSearch()
        {
            check(this.getConditions(), TC.get("Search.Conditions"));
            check(this.getIdTarget(), TC.get("Search.IDTarget"));

            for (int i = 0; i < this.getEffects().getEffectCount(); i++)
            {
                check(this.getEffects().getEffectInfo(i), TC.get("Search.Effect"));
                check(this.getEffects().getConditionController(i), TC.get("Search.Conditions"));
            }

            for (int i = 0; i < this.getNotEffectsController().getEffectCount(); i++)
            {
                check(this.getNotEffectsController().getEffectInfo(i), TC.get("Search.Effect"));
                check(this.getNotEffectsController().getConditionController(i), TC.get("Search.Conditions"));
            }

            foreach (ResourcesDataControl r in resourcesDataControlList)
            {
                r.recursiveSearch();
            }
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }
    }
}
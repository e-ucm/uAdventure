using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    public class BarrierDataControl : DataControl, RectangleArea
    {

        /**
        * Scene controller that contains this element reference (used to extract
        * the id of the scene).
        */
        private SceneDataControl sceneDataControl;

        /**
         * Contained barrier.
         */
        private Barrier barrier;

        /**
         * Conditions controller.
         */
        private ConditionsController conditionsController;

        /**
         * Controller for descriptions
         */
        private DescriptionsController descriptionController;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Parent scene controller
         * @param activeArea
         *            Exit of the data control structure
         */
        public BarrierDataControl(SceneDataControl sceneDataControl, Barrier barrier)
        {

            this.sceneDataControl = sceneDataControl;
            this.barrier = barrier;

            // Create subcontrollers
            Dictionary<string, ConditionsController.ConditionContextProperty> context1 = new Dictionary<string, ConditionsController.ConditionContextProperty>();
            ConditionsController.ConditionOwner parent = new ConditionsController.ConditionOwner(Controller.SCENE, sceneDataControl.getId());
            ConditionsController.ConditionOwner owner = new ConditionsController.ConditionOwner(Controller.BARRIER, barrier.getId(), parent);

            context1.Add(ConditionsController.CONDITION_OWNER, owner);

            conditionsController = new ConditionsController(barrier.getConditions(), context1);

            descriptionController = new DescriptionsController(barrier.getDescriptions());

            //Barriers can only have name, and only one description, so we set selectedDEscription to 0
            descriptionController.setSelectedDescription(0);

        }

        /**
         * Returns the id of the scene that contains this element reference.
         * 
         * @return Parent scene id
         */
        public string getParentSceneId()
        {

            return sceneDataControl.getId();
        }

        /**
         * Returns the id of the item.
         * 
         * @return Item's id
         */
        public string getId()
        {

            return barrier.getId();
        }

        /**
         * Returns the documentation of the item.
         * 
         * @return Item's documentation
         */
        public string getDocumentation()
        {

            return barrier.getDocumentation();
        }

        /**
         * Sets the new documentation of the item.
         * 
         * @param documentation
         *            Documentation of the item
         */
        public void setDocumentation(string documentation)
        {

            controller.AddTool(new ChangeDocumentationTool(barrier, documentation));
        }

        /**
         * Sets the new name of the activeArea.
         * 
         * @param name
         *            Name of the activeArea
         */
        public void setName(string name)
        {

            controller.AddTool(new ChangeNameTool(descriptionController.getSelectedDescription(), name));
        }

        /**
         * Sets the new brief description of the activeArea.
         * 
         * @param description
         *            Description of the activeArea
         */
        public void setBriefDescription(string description)
        {

            controller.AddTool(new ChangeDescriptionTool(descriptionController.getSelectedDescription(), description));
        }

        /**
         * Sets the new detailed description of the activeArea.
         * 
         * @param detailedDescription
         *            Detailed description of the activeArea
         */
        public void setDetailedDescription(string detailedDescription)
        {

            controller.AddTool(new ChangeDetailedDescriptionTool(descriptionController.getSelectedDescription(), detailedDescription));
        }

        /**
         * Returns the X coordinate of the upper left position of the exit.
         * 
         * @return X coordinate of the upper left point
         */
        public int getX()
        {

            return barrier.getX();
        }

        /**
         * Returns the Y coordinate of the upper left position of the exit.
         * 
         * @return Y coordinate of the upper left point
         */
        public int getY()
        {

            return barrier.getY();
        }

        /**
         * Returns the width of the exit.
         * 
         * @return Width of the exit
         */
        public int getWidth()
        {

            return barrier.getWidth();
        }

        /**
         * Returns the height of the exit.
         * 
         * @return Height of the exit
         */
        public int getHeight()
        {

            return barrier.getHeight();
        }

        /**
         * Sets the new values for the exit.
         * 
         * @param x
         *            X coordinate of the upper left point
         * @param y
         *            Y coordinate of the upper left point
         * @param width
         *            Width of the exit area
         * @param height
         *            Height of the exit area
         */
        public void setBarrier(int x, int y, int width, int height)
        {

            controller.AddTool(new ChangeRectangleValueTool(barrier, x, y, width, height));
        }


        public override System.Object getContent()
        {

            return barrier;
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

            bool elementAdded = false;
            return elementAdded;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;
            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            return elementMoved;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            ConditionsController.updateVarFlagSummary(varFlagSummary, barrier.getConditions());
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            return true;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // DO nothing
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Delete the references from the actions
            // Do nothing
        }


        public override int countIdentifierReferences(string id)
        {

            return conditionsController.countIdentifierReferences(id);
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            conditionsController.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            conditionsController.deleteIdentifierReferences(id);
        }

        /**
         * Returns the conditions of the element reference.
         * 
         * @return Conditions of the element reference
         */
        public ConditionsController getConditions()
        {

            return conditionsController;
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            // barriers have no brief descriptions
            // check( this.getBriefDescription( ), TC.get( "Search.BriefDescription" ) );
            check(this.getConditions(), TC.get("Search.Conditions"));
            // barriers have no detailed descriptions
            //check( this.getDetailedDescription( ), TC.get( "Search.DetailedDescription" ) );
            check(this.getDocumentation(), TC.get("Search.Documentation"));
            // check( this.getId( ), "ID" );
            //Barriers have no name
            //check( this.getName( ), TC.get( "Search.Name" ) );
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }

        public bool isRectangular()
        {
            return true;
        }

        public List<Vector2> getPoints()
        {
            return null;
        }

        public void addPoint(int x, int y)
        {
        }

        public Vector2 getLastPoint()
        {
            return Vector2.zero;
        }

        public void deletePoint(Vector2 point)
        {
        }

        public void setRectangular(bool selected)
        {

        }

        public Rectangle getRectangle()
        {
            return barrier;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ExitDataControl : DataControl, RectangleArea
    {
        /**
           * Scene controller that contains this element reference (used to extract
           * the id of the scene).
           */
        private SceneDataControl sceneDataControl;

        /**
         * Contained exit.
         */
        private Exit exit;

        /**
         * Conditions controller.
         */
        private ConditionsController conditionsController;

        private ExitLookDataControl exitLookDataControl;

        private InfluenceAreaDataControl influenceAreaDataControl;

        private EffectsController effectsController;

        private EffectsController postEffectsController;

        private EffectsController notEffectsController;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Parent scene controller
         * @param exit
         *            Exit of the data control structure
         */
        public ExitDataControl(SceneDataControl sceneDataControl, Exit exit)
        {

            this.sceneDataControl = sceneDataControl;
            this.exit = exit;

            this.influenceAreaDataControl = new InfluenceAreaDataControl(sceneDataControl, exit.getInfluenceArea(), this);
            effectsController = new EffectsController(exit.getEffects());
            postEffectsController = new EffectsController(exit.getPostEffects());
            notEffectsController = new EffectsController(exit.getNotEffects());
            conditionsController = new ConditionsController(new Conditions());
            exitLookDataControl = new ExitLookDataControl(exit);
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
         * Returns the X coordinate of the upper left position of the exit.
         * 
         * @return X coordinate of the upper left point
         */
        public int getX()
        {

            return exit.getX();
        }

        /**
         * Returns the Y coordinate of the upper left position of the exit.
         * 
         * @return Y coordinate of the upper left point
         */
        public int getY()
        {

            return exit.getY();
        }

        /**
         * Returns the width of the exit.
         * 
         * @return Width of the exit
         */
        public int getWidth()
        {

            return exit.getWidth();
        }

        /**
         * Returns the height of the exit.
         * 
         * @return Height of the exit
         */
        public int getHeight()
        {

            return exit.getHeight();
        }

        /**
         * Returns the documentation of the exit.
         * 
         * @return Exit's documentation
         */
        public string getDocumentation()
        {

            return exit.getDocumentation();
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
        public void setExit(int x, int y, int width, int height)
        {

            controller.AddTool(new ChangeRectangleValueTool(exit, x, y, width, height));
        }

        /**
         * Sets the new documentation of the exit.
         * 
         * @param documentation
         *            Documentation of the exit
         */
        public void setDocumentation(string documentation)
        {

            controller.AddTool(new ChangeDocumentationTool(exit, documentation));
        }

        public override System.Object getContent()
        {

            return exit;
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

            ConditionsController.updateVarFlagSummary(varFlagSummary, exit.getConditions());
            if (exit.getEffects() != null)
                EffectsController.updateVarFlagSummary(varFlagSummary, exit.getEffects());
            if (exit.getPostEffects() != null)
                EffectsController.updateVarFlagSummary(varFlagSummary, exit.getPostEffects());
            if (exit.getNotEffects() != null)
                EffectsController.updateVarFlagSummary(varFlagSummary, exit.getNotEffects());
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            if (exit.getEffects() != null)
                valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.Effects"), incidences, exit.getEffects());
            if (exit.getPostEffects() != null)
                valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.PostEffects"), incidences, exit.getPostEffects());
            if (exit.getNotEffects() != null)
                valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.NotEffects"), incidences, exit.getNotEffects());

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            //return exitLookDataControl.countAssetReferences( assetPath );
            int assetsRefs = 0;
            assetsRefs += exitLookDataControl.countAssetReferences(assetPath);

            if (exit.getEffects() != null)
                assetsRefs += EffectsController.countAssetReferences(assetPath, exit.getEffects());
            if (exit.getPostEffects() != null)
                assetsRefs += EffectsController.countAssetReferences(assetPath, exit.getPostEffects());
            if (exit.getNotEffects() != null)
                assetsRefs += EffectsController.countAssetReferences(assetPath, exit.getNotEffects());
            return assetsRefs;

        }


        public override void deleteAssetReferences(string assetPath)
        {

            exitLookDataControl.deleteAssetReferences(assetPath);
            if (exit.getEffects() != null)
                EffectsController.deleteAssetReferences(assetPath, exit.getEffects());
            if (exit.getPostEffects() != null)
                EffectsController.deleteAssetReferences(assetPath, exit.getPostEffects());
            if (exit.getNotEffects() != null)
                EffectsController.deleteAssetReferences(assetPath, exit.getNotEffects());
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            if (id.Equals(exit.getNextSceneId()))
                count = 1;
            count += EffectsController.countIdentifierReferences(id, exit.getEffects());
            count += EffectsController.countIdentifierReferences(id, exit.getPostEffects());
            count += EffectsController.countIdentifierReferences(id, exit.getNotEffects());
            count += conditionsController.countIdentifierReferences(id);
            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            if (oldId.Equals(exit.getNextSceneId()))
                exit.setNextSceneId(newId);
            EffectsController.replaceIdentifierReferences(oldId, newId, exit.getEffects());
            EffectsController.replaceIdentifierReferences(oldId, newId, exit.getPostEffects());
            EffectsController.replaceIdentifierReferences(oldId, newId, exit.getNotEffects());
            conditionsController.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            EffectsController.deleteIdentifierReferences(id, exit.getEffects());
            EffectsController.deleteIdentifierReferences(id, exit.getPostEffects());
            EffectsController.deleteIdentifierReferences(id, exit.getNotEffects());
            conditionsController.deleteIdentifierReferences(id);
            if (id.Equals(exit.getNextSceneId()))
                exit.setNextSceneId(null);
        }

        /**
         * @return the exitLookDataControl
         */
        public ExitLookDataControl getExitLookDataControl()
        {

            return exitLookDataControl;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            exitLookDataControl.getAssetReferences(assetPaths, assetTypes);
            EffectsController.getAssetReferences(assetPaths, assetTypes, exit.getEffects());
            EffectsController.getAssetReferences(assetPaths, assetTypes, exit.getPostEffects());
            EffectsController.getAssetReferences(assetPaths, assetTypes, exit.getNotEffects());
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            check(this.conditionsController, TC.get("Search.Conditions"));
            //        check( this.getDocumentation( ), TC.get( "Search.Documentation" ) );
            //        check( this.getExitLookDataControl( ).getCustomizedText( ), TC.get( "Search.CustomizedText" ) );
            if (exitLookDataControl != null)
                exitLookDataControl.recursiveSearch();
            for (int i = 0; i < this.getEffects().getEffectCount(); i++)
            {
                check(this.getEffects().getEffectInfo(i), TC.get("Search.Effect"));
                check(this.getEffects().getConditionController(i), TC.get("Search.Conditions"));
            }

            for (int i = 0; i < this.getNotEffects().getEffectCount(); i++)
            {
                check(this.getNotEffects().getEffectInfo(i), TC.get("Search.Effect"));
                check(this.getNotEffects().getConditionController(i), TC.get("Search.Conditions"));
            }

            for (int i = 0; i < this.getPostEffects().getEffectCount(); i++)
            {
                check(this.getPostEffects().getEffectInfo(i), TC.get("Search.Effect"));
                check(this.getPostEffects().getConditionController(i), TC.get("Search.Conditions"));
            }
        }

        public bool isRectangular()
        {

            return exit.isRectangular();
        }

        public List<Vector2> getPoints()
        {

            return exit.getPoints();
        }

        public void addPoint(int x, int y)
        {

            controller.AddTool(new AddNewPointTool(exit, x, y));
        }

        public Vector2 getLastPoint()
        {

            if (exit.getPoints().Count > 0)
                return exit.getPoints()[exit.getPoints().Count - 1];
            return Vector2.zero;
        }

        public void deletePoint(Vector2 point)
        {

            controller.AddTool(new DeletePointTool(exit, point));
        }

        public void setRectangular(bool selected)
        {

            controller.AddTool(new ChangeRectangularValueTool(exit, selected));
        }

        public Rectangle getRectangle()
        {

            return (Rectangle)this.getContent();
        }

        public InfluenceAreaDataControl getInfluenceArea()
        {

            return influenceAreaDataControl;
        }

        public SceneDataControl getSceneDataControl()
        {

            return sceneDataControl;
        }

        /**
         * Returns the conditions of the element reference.
         * 
         * @return Conditions of the element reference
         */
        public ConditionsController getConditions()
        {

            Dictionary<string, ConditionsController.ConditionContextProperty> context1 = new Dictionary<string, ConditionsController.ConditionContextProperty>();
            ConditionsController.ConditionOwner parent = new ConditionsController.ConditionOwner(Controller.SCENE, sceneDataControl.getId());
            ConditionsController.ConditionOwner owner = new ConditionsController.ConditionOwner(Controller.EXIT, exit.getNextSceneId(), parent);
            context1.Add(ConditionsController.CONDITION_OWNER, owner);

            conditionsController = new ConditionsController(exit.getConditions(), context1);

            return conditionsController;
        }

        public string getNextSceneId()
        {

            return exit.getNextSceneId();
        }

        public void setNextSceneId(string value)
        {

            Controller.Instance.AddTool(new ChangeStringValueTool(exit, value, "getNextSceneId", "setNextSceneId"));
        }

        public int getDestinyPositionX()
        {

            return exit.getDestinyX();
        }

        public int getDestinyPositionY()
        {

            return exit.getDestinyY();
        }

        public void setDestinyScale(float destinyScale)
        {
            exit.setDestinyScale(destinyScale);
        }

        public float getDestinyScale()
        {
            return exit.getDestinyScale();
        }

        /**
         * Sets the new destiny position of the next scene.
         * 
         * @param positionX
         *            X coordinate of the destiny position
         * @param positionY
         *            Y coordinate of the destiny position
         */
        public void setDestinyPosition(int positionX, int positionY)
        {

            controller.AddTool(new ChangeNSDestinyPositionTool(exit, positionX, positionY));
        }

        /**
         * Toggles the destiny position. If the next scene has a destiny position
         * deletes it, if it doesn't have one, set initial values for it.
         */
        public void toggleDestinyPosition()
        {

            if (exit.hasPlayerPosition())
                controller.AddTool(new ChangeNSDestinyPositionTool(exit, int.MinValue, int.MinValue));
            else
                controller.AddTool(new ChangeNSDestinyPositionTool(exit, 0, 0));
        }

        /**
         * Returns whether the next scene has a destiny position or not.
         * 
         * @return True if the next scene has a destiny position, false otherwise
         */
        public bool hasDestinyPosition()
        {

            return exit.hasPlayerPosition();
        }

        public int getTransitionType()
        {

            return exit.getTransitionType();
        }

        public int getTransitionTime()
        {

            return exit.getTransitionTime();
        }

        public void setTransitionTime(int value)
        {

            Controller.Instance.AddTool(new ChangeIntegerValueTool(exit, value, "getTransitionTime", "setTransitionTime"));
        }

        public void setTransitionType(int selectedIndex)
        {

            Controller.Instance.AddTool(new ChangeIntegerValueTool(exit, selectedIndex, "getTransitionType", "setTransitionType"));
        }

        public bool isHasNotEffects()
        {

            return exit.isHasNotEffects();
        }

        public void setHasNotEffects(bool selected)
        {

            Controller.Instance.AddTool(new ChangeBooleanValueTool(exit, selected, "isHasNotEffects", "setHasNotEffects"));
        }

        public EffectsController getEffects()
        {

            return effectsController;
        }

        public EffectsController getPostEffects()
        {

            return postEffectsController;
        }

        public EffectsController getNotEffects()
        {

            return notEffectsController;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            List<Searchable> path = getPathFromSearchableChild(dataControl, exitLookDataControl);
            return path;
        }
    }
}
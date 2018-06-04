using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class InfluenceAreaDataControl : DataControl
    {


        /**
         * Scene controller that contains this element reference (used to extract
         * the id of the scene).
         */
        private SceneDataControl sceneDataControl;

        /**
         * Contained influenceArea.
         */
        private InfluenceArea influenceArea;

        private DataControl dataControl;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Parent scene controller
         * @param dataControl
         * @param activeArea
         *            Exit of the data control structure
         */
        public InfluenceAreaDataControl(SceneDataControl sceneDataControl, InfluenceArea influenceArea, DataControl dataControl)
        {

            this.sceneDataControl = sceneDataControl;
            this.influenceArea = influenceArea;
            this.dataControl = dataControl;
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
         * Returns the X coordinate of the upper left position of the influenceArea.
         * 
         * @return X coordinate of the upper left point
         */
        public int getX()
        {

            return influenceArea.getX();
        }

        /**
         * Returns the Y coordinate of the upper left position of the influenceArea.
         * 
         * @return Y coordinate of the upper left point
         */
        public int getY()
        {

            return influenceArea.getY();
        }

        /**
         * Returns the width of the influenceArea.
         * 
         * @return Width of the influenceArea
         */
        public int getWidth()
        {

            return influenceArea.getWidth();
        }

        /**
         * Returns the height of the influenceArea.
         * 
         * @return Height of the influenceArea
         */
        public int getHeight()
        {

            return influenceArea.getHeight();
        }

        /**
         * Sets the new values for the influenceArea.
         * 
         * @param x
         *            X coordinate of the upper left point
         * @param y
         *            Y coordinate of the upper left point
         * @param width
         *            Width of the influenceArea area
         * @param height
         *            Height of the influenceArea area
         */
        public void setInfluenceArea(int x, int y, int width, int height)
        {

            influenceArea.setExists(true);
            controller.AddTool(new ChangeRectangleValueTool(influenceArea, x, y, width, height));
        }

        public void setInfluenceArea(InfluenceArea influenceArea)
        {

            this.influenceArea = influenceArea;
            this.influenceArea.setExists(true);
        }


        public override System.Object getContent()
        {

            return influenceArea;
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

        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

        }


        public override void deleteAssetReferences(string assetPath)
        {

        }


        public override int countIdentifierReferences(string id)
        {

            return 0;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

        }


        public override void deleteIdentifierReferences(string id)
        {

        }


        public override bool canBeDuplicated()
        {

            return true;
        }

        public bool hasInfluenceArea()
        {

            return influenceArea.isExists();
        }

        public DataControl getDataControl()
        {

            return dataControl;
        }

        public void referenceScaleChanged(int incrementX, int incrementY)
        {

            if (influenceArea.isExists())
            {
                controller.DataModified();
                influenceArea.setWidth(influenceArea.getWidth() + incrementX);
                influenceArea.setHeight(influenceArea.getHeight() + incrementY);
            }
        }


        public override void recursiveSearch()
        {

        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }

    }
}
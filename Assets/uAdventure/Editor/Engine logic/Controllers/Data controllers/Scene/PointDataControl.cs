using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class PointDataControl : DataControl
    {


        /**
         * Contained node.
         */
        private Vector2 point;

        /**
         * Constructor.
         * 
         * @param activeArea
         *            Exit of the data control structure
         */

        public PointDataControl(Vector2 point)
        {

            this.point = point;
        }

        /**
         * Returns the X coordinate of the upper left position of the exit.
         * 
         * @return X coordinate of the upper left point
         */

        public int getX()
        {

            return (int)point.x;
        }

        /**
         * Returns the Y coordinate of the upper left position of the exit.
         * 
         * @return Y coordinate of the upper left point
         */

        public int getY()
        {

            return (int)point.y;
        }

        /**
         * Sets the new values for the exit.
         * 
         * @param x
         *            X coordinate of the upper left point
         * @param y
         *            Y coordinate of the upper left point
         */

        public void setPoint(int x, int y)
        {

            controller.AddTool(new ChangePointValueTool(point, x, y));
        }


        public override System.Object getContent()
        {

            return point;
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

            return 0;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            //actionsListDataControl.replaceIdentifierReferences( oldId, newId );
        }


        public override void deleteIdentifierReferences(string id)
        {

            //actionsListDataControl.deleteIdentifierReferences( id );
        }


        public override bool canBeDuplicated()
        {

            return true;
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
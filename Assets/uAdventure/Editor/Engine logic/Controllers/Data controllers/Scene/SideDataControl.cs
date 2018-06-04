using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using Side = uAdventure.Core.Trajectory.Side;

namespace uAdventure.Editor
{
    public class SideDataControl : DataControl
    {

        /**
         * Scene controller that contains this element reference (used to extract
         * the id of the scene).
         */
        private SceneDataControl sceneDataControl;

        private TrajectoryDataControl trajectoryDataControl;

        /**
         * Contained side.
         */
        private Side side;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Parent scene controller
         * @param activeArea
         *            Exit of the data control structure
         */
        public SideDataControl(SceneDataControl sceneDataControl, TrajectoryDataControl trajectoryDataControl, Side side)
        {

            this.sceneDataControl = sceneDataControl;
            this.trajectoryDataControl = trajectoryDataControl;
            this.side = side;
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


        public override System.Object getContent()
        {

            return side;
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

        public NodeDataControl getStart()
        {

            foreach (NodeDataControl ndc in trajectoryDataControl.getNodes())
            {
                if (ndc.getID().Equals(side.getIDStart()))
                    return ndc;
            }
            return null;
        }

        public NodeDataControl getEnd()
        {

            foreach (NodeDataControl ndc in trajectoryDataControl.getNodes())
            {
                if (ndc.getID().Equals(side.getIDEnd()))
                    return ndc;
            }
            return null;
        }


        public override void recursiveSearch()
        {

        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }

        public float getLength()
        {
            return (float)side.getLength();
        }

        public float getRealLength()
        {
            return (float)side.getRealLength();
        }

        public void setLength(float value)
        {
            Controller.Instance.AddTool(new SetSideLengthTool(side, value));
        }
    }
}
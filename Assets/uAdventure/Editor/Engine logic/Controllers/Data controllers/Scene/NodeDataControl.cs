using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class NodeDataControl : DataControl
    {

        /* Scene controller that contains this element reference(used to extract
           * the id of the scene).
           */
        private readonly SceneDataControl sceneDataControl;

        private readonly Trajectory trajectory;

        /**
         * Contained node.
         */
        private readonly Trajectory.Node node;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Parent scene controller
         * @param activeArea
         *            Exit of the data control structure
         */
        public NodeDataControl(SceneDataControl sceneDataControl, Trajectory.Node node, Trajectory trajectory)
        {
            this.sceneDataControl = sceneDataControl;
            this.node = node;
            this.trajectory = trajectory;
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
            return node.getX();
        }

        /**
         * Returns the Y coordinate of the upper left position of the exit.
         * 
         * @return Y coordinate of the upper left point
         */
        public int getY()
        {
            return node.getY();
        }

        /**
         * Sets the new values for the exit.
         * 
         * @param x
         *            X coordinate of the upper left point
         * @param y
         *            Y coordinate of the upper left point
         * @param scale
         *            the scale of the player on the node
         */
        public void setNode(int x, int y, float scale)
        {
            scale = Mathf.Max(0, scale);
            controller.AddTool(new SetNodeValuesTool(node, trajectory, x, y, scale));
        }


        public override System.Object getContent()
        {
            return node;
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


        public override string renameElement(string newName)
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

        public float getScale()
        {

            return node.getScale();
        }

        public string getID()
        {

            return node.getID();
        }

        public string getPlayerImagePath()
        {

            return controller.getPlayerImagePath();
        }

        public int getPlayerLayer()
        {
            return sceneDataControl.getPlayerLayer();
        }

        public void setInitial(bool initial)
        {
            var was = isInitial();
            if (was && !initial)
            {
                trajectory.setInitial(trajectory.getNodes()[0].getID());
            }
            else if (!was && initial)
            {
                trajectory.setInitial(node.getID());
            }
        }

        public bool isInitial()
        {
            return trajectory.getInitial() == node || trajectory.getInitial().getID() == node.getID();
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
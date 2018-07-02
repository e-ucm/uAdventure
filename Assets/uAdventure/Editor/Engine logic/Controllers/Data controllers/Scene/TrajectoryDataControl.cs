using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

using Node = uAdventure.Core.Trajectory.Node;
using Side = uAdventure.Core.Trajectory.Side;

namespace uAdventure.Editor
{
    /**
     * Data Control for the trajectory
     */
    public class TrajectoryDataControl : DataControl
    {

        /**
         * Scene controller that contains this element reference.
         */
        private SceneDataControl sceneDataControl;

        /**
         * The trajectory
         */
        private Trajectory trajectory;

        /**
         * List of node controllers.
         */
        private List<NodeDataControl> nodeDataControlList;

        /**
         * List of side controllers.
         */
        private List<SideDataControl> sideDataControlList;

        /**
         * Initial node controller
         */
        public NodeDataControl initialNode;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Link to the parent scene controller
         * @param barriersList
         *            List of activeAreas
         */
        public TrajectoryDataControl(SceneDataControl sceneDataControl, Trajectory trajectory)
        {

            this.sceneDataControl = sceneDataControl;
            this.trajectory = trajectory;

            sideDataControlList = new List<SideDataControl>();
            nodeDataControlList = new List<NodeDataControl>();
            if (trajectory != null)
            {
                foreach (Node node in trajectory.getNodes())
                {
                    nodeDataControlList.Add(new NodeDataControl(sceneDataControl, node, trajectory));
                    if (node == trajectory.getInitial())
                    {
                        initialNode = nodeDataControlList[nodeDataControlList.Count - 1];
                        initialNode.setInitial(true);
                    }
                }
                foreach (Side side in trajectory.getSides())
                    sideDataControlList.Add(new SideDataControl(sceneDataControl, this, side));
            }
        }

        /**
         * Returns the list of node data controllers
         * 
         * @return the list of node data controllers
         */
        public List<NodeDataControl> getNodes()
        {

            return nodeDataControlList;
        }

        /**
         * Returns the list of side data controllers
         * 
         * @return the list of side data controllers
         */
        public List<SideDataControl> getSides()
        {

            return sideDataControlList;
        }

        public Trajectory GetTrajectory()
        {
            return trajectory;
        }

        /**
         * Returns the last node data control in the list
         * 
         * @return the last node data control
         */
        public NodeDataControl getLastNode()
        {

            return nodeDataControlList[nodeDataControlList.Count - 1];
        }

        /**
         * Returns the last side data control in the list
         * 
         * @return the last side data control in the list
         */
        public SideDataControl getLastSide()
        {

            return sideDataControlList[sideDataControlList.Count - 1];
        }

        /**
         * Returns the id of the scene that contains this activeAreas list.
         * 
         * @return Parent scene id
         */
        public string getParentSceneId()
        {

            return sceneDataControl.getId();
        }


        public override System.Object getContent()
        {

            return trajectory;
        }


        public override int[] getAddableElements()
        {

            return new int[] { };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new barrier
            return false;
        }


        public override bool canBeDeleted()
        {

            return false;
        }


        public override bool canBeMoved()
        {

            return false;
        }


        public override bool canBeRenamed()
        {

            return false;
        }

        /**
         * Add a new node to the trajectory
         * 
         * @param x
         *            The position along the x-axis of the node
         * @param y
         *            The position along the y-axis of the node
         * @return bool indicating if the node was added
         */
        public bool addNode(int x, int y)
        {

            if (trajectory == null)
            {
                return false;
            }

            Controller.Instance.AddTool(new AddTrajectoryNodeTool(trajectory, this, x, y, sceneDataControl));

            return true;
        }

        /**
         * Add a new side to the trajectory
         * 
         * @param startNode
         *            the start node data control of the side
         * @param endNode
         *            the end node data control of the side
         * @return bool indicating if the side was added
         */
        public bool addSide(NodeDataControl startNode, NodeDataControl endNode)
        {

            if (startNode == endNode)
                return false;

            string sID = startNode.getID(), eID = endNode.getID();

            if (trajectory.getSides()
                .Any(s => (s.getIDStart() == sID && s.getIDEnd() == eID) 
                       || (s.getIDStart() == eID && s.getIDEnd() == sID)))
            {
                return false;
            }

            Controller.Instance.AddTool(new AddTrajectorySideTool(startNode, endNode, trajectory, this, sceneDataControl));

            return true;
        }


        public override bool addElement(int type, string id)
        {

            bool elementAdded = false;

            return elementAdded;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            if (dataControl is NodeDataControl && nodeDataControlList.Contains((NodeDataControl)dataControl))
            {
                if (nodeDataControlList.Count > 1)
                {
                    Controller.Instance.AddTool(new DeleteTrajectoryNodeTool(dataControl, trajectory, this));
                    return true;
                }
                else
                    return false;
            }
            if (dataControl is SideDataControl && sideDataControlList.Contains((SideDataControl)dataControl))
            {
                Controller.Instance.AddTool(new DeleteTrajectorySideTool((SideDataControl)dataControl, trajectory, this));
                trajectory.getSides().Remove((Side)dataControl.getContent());
                sideDataControlList.Remove((SideDataControl)dataControl);
                return true;
            }
            return false;
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

            /*for( NodeDataControl nodeDataControl : nodeDataControlList )
                nodeDataControl.updateVarFlagSummary( varFlagSummary );
            for( SideDataControl sideDataControl : sideDataControlList )
                sideDataControl.updateVarFlagSummary( varFlagSummary );*/
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            /*for( int i = 0; i < nodeDataControlList.size( ); i++ ) {
                string activeAreaPath = currentPath + " >> " + TC.getElement( Controller.NODE ) + " #" + ( i + 1 );
                valid &= nodeDataControlList.get( i ).isValid( activeAreaPath, incidences );
            }
            for( int i = 0; i < sideDataControlList.size( ); i++ ) {
                string activeAreaPath = currentPath + " >> " + TC.getElement( Controller.SIDE ) + " #" + ( i + 1 );
                valid &= sideDataControlList.get( i ).isValid( activeAreaPath, incidences );
            }*/

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            /*int count = 0;

            for( NodeDataControl nodeDataControl : nodeDataControlList )
                count += nodeDataControl.countAssetReferences( assetPath );
            for( SideDataControl sideDataControl : sideDataControlList )
                count += sideDataControl.countAssetReferences( assetPath );

            return count;*/
            return 0;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            /*for( NodeDataControl nodeDataControl : nodeDataControlList )
                nodeDataControl.getAssetReferences( assetPaths, assetTypes );
            for( SideDataControl sideDataControl : sideDataControlList )
                sideDataControl.getAssetReferences( assetPaths, assetTypes );*/

        }


        public override void deleteAssetReferences(string assetPath)
        {

            /*for( NodeDataControl nodeDataControl : nodeDataControlList )
                nodeDataControl.deleteAssetReferences( assetPath );
            for( SideDataControl sideDataControl : sideDataControlList )
                sideDataControl.deleteAssetReferences( assetPath );/*/
        }


        public override int countIdentifierReferences(string id)
        {

            /*int count = 0;

            for( NodeDataControl nodeDataControl : nodeDataControlList )
                count += nodeDataControl.countIdentifierReferences( id );
            for( SideDataControl sideDataControl : sideDataControlList )
                count += sideDataControl.countIdentifierReferences( id );

            return count;*/
            return 0;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            /*for( NodeDataControl nodeDataControl : nodeDataControlList )
                nodeDataControl.replaceIdentifierReferences( oldId, newId );
            for( SideDataControl sideDataControl : sideDataControlList )
                sideDataControl.replaceIdentifierReferences( oldId, newId );*/
        }


        public override void deleteIdentifierReferences(string id)
        {

            /*for( NodeDataControl nodeDataControl : nodeDataControlList )
                nodeDataControl.deleteIdentifierReferences( id );
            for( SideDataControl sideDataControl : sideDataControlList )
                sideDataControl.deleteIdentifierReferences( id );*/
        }


        public override bool canBeDuplicated()
        {

            // TODO Auto-generated method stub
            return false;
        }

        /**
         * bool indicating if there is a trajectory
         * 
         * @return True if it has a trajectory
         */
        public bool hasTrajectory()
        {

            return trajectory != null;
        }

        /**
         * Set the initial node of the trajectory to the given one
         * 
         * @param nodeDataControl
         *            The new initial node data control
         */
        public void setInitialNode(NodeDataControl nodeDataControl)
        {

            Controller.Instance.AddTool(new SetTrajectoryInitialNodeTool(trajectory, this, nodeDataControl));
        }

        /**
         * Returns the initial node data control
         * 
         * @return the initial node data control
         */
        public NodeDataControl getInitialNode()
        {

            return initialNode;
        }


        public override void recursiveSearch()
        {

            // TODO Auto-generated method stub

        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            List<Searchable> path = getPathFromChild(dataControl, nodeDataControlList.Cast<System.Object>().ToList());
            if (path != null)
                return path;
            return getPathFromChild(dataControl, sideDataControlList.Cast<System.Object>().ToList());
        }
    }
}
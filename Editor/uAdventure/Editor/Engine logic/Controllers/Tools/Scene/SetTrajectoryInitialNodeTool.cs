using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SetTrajectoryInitialNodeTool : Tool
    {


        private Trajectory trajectory;

        private TrajectoryDataControl trajectoryDataControl;

        private NodeDataControl nodeDataControl;

        private NodeDataControl oldInitialNodeDataControl;

        public SetTrajectoryInitialNodeTool(Trajectory trajectory, TrajectoryDataControl trajectoryDataControl, NodeDataControl nodeDataControl)
        {

            this.trajectory = trajectory;
            this.trajectoryDataControl = trajectoryDataControl;
            this.nodeDataControl = nodeDataControl;
            this.oldInitialNodeDataControl = trajectoryDataControl.getInitialNode();
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


        public override bool doTool()
        {

            if (trajectory.getInitial() != null && trajectory.getInitial().getID().Equals(nodeDataControl.getID()))
                return false;

            trajectory.setInitial(nodeDataControl.getID());

            if (trajectoryDataControl.initialNode != null)
            {
                trajectoryDataControl.initialNode.setInitial(false);
            }

            trajectoryDataControl.initialNode = nodeDataControl;
            trajectoryDataControl.initialNode.setInitial(true);

            return true;
        }


        public override bool redoTool()
        {

            trajectory.setInitial(nodeDataControl.getID());

            if (trajectoryDataControl.initialNode != null)
            {
                trajectoryDataControl.initialNode.setInitial(false);
            }

            trajectoryDataControl.initialNode = nodeDataControl;
            trajectoryDataControl.initialNode.setInitial(true);

            Controller.Instance.updatePanel();

            return true;
        }


        public override bool undoTool()
        {

            nodeDataControl.setInitial(false);
            trajectoryDataControl.initialNode = oldInitialNodeDataControl;

            if (trajectoryDataControl.initialNode != null)
            {
                trajectory.setInitial(trajectoryDataControl.getInitialNode().getID());
                trajectoryDataControl.initialNode.setInitial(true);
            }
            else
            {
                trajectory.setInitial("");
            }

            Controller.Instance.updatePanel();
            return true;
        }
    }
}
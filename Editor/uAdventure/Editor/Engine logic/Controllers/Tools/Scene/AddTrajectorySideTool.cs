using UnityEngine;
using System.Collections;
using uAdventure.Core;
using Side = uAdventure.Core.Trajectory.Side;

namespace uAdventure.Editor
{
    public class AddTrajectorySideTool : Tool
    {
        private NodeDataControl startNode;

        private NodeDataControl endNode;

        private Trajectory trajectory;

        private TrajectoryDataControl trajectoryDataControl;

        private SceneDataControl sceneDataControl;

        private Side newSide;

        private SideDataControl newSideDataControl;

        public AddTrajectorySideTool(NodeDataControl startNode, NodeDataControl endNode, Trajectory trajectory, TrajectoryDataControl trajectoryDataControl, SceneDataControl sceneDataControl)
        {

            this.startNode = startNode;
            this.endNode = endNode;
            this.trajectory = trajectory;
            this.trajectoryDataControl = trajectoryDataControl;
            this.sceneDataControl = sceneDataControl;
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

            newSide = trajectory.addSide(startNode.getID(), endNode.getID(), -1);
            if (newSide != null)
            {
                newSideDataControl = new SideDataControl(sceneDataControl, trajectoryDataControl, newSide);
                trajectoryDataControl.getSides().Add(newSideDataControl);
                return true;
            }
            return false;
        }


        public override bool redoTool()
        {

            trajectory.getSides().Add(newSide);
            trajectoryDataControl.getSides().Add(newSideDataControl);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            trajectory.getSides().Remove(newSide);
            trajectoryDataControl.getSides().Remove(newSideDataControl);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}
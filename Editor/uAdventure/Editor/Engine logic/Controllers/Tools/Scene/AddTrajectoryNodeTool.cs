using UnityEngine;
using System.Collections;

using uAdventure.Core;
using Node = uAdventure.Core.Trajectory.Node;

namespace uAdventure.Editor
{
    public class AddTrajectoryNodeTool : Tool
    {

        private Trajectory trajectory;

        private TrajectoryDataControl trajectoryDataControl;

        private int x;

        private int y;

        private Node newNode;

        private NodeDataControl newNodeDataControl;

        private SceneDataControl sceneDataControl;

        private bool wasInitial;

        public AddTrajectoryNodeTool(Trajectory trajectory, TrajectoryDataControl trajectoryDataControl, int x, int y, SceneDataControl sceneDataControl)
        {

            this.trajectory = trajectory;
            this.trajectoryDataControl = trajectoryDataControl;
            this.x = x;
            this.y = y;
            this.sceneDataControl = sceneDataControl;
            this.wasInitial = false;
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

            string id = "node" + (new System.Random().Next(10000));
            newNode = trajectory.addNode(id, x, y, 1.0f);
            newNodeDataControl = new NodeDataControl(sceneDataControl, newNode, trajectory);
            trajectoryDataControl.getNodes().Add(newNodeDataControl);
            if (trajectory.getInitial() == newNode)
            {
                trajectoryDataControl.initialNode = newNodeDataControl;
                wasInitial = true;
            }
            return true;
        }


        public override bool redoTool()
        {

            trajectory.getNodes().Add(newNode);
            trajectoryDataControl.getNodes().Add(newNodeDataControl);
            if (wasInitial)
            {
                trajectory.setInitial(newNode.getID());
                trajectoryDataControl.initialNode = newNodeDataControl;
            }
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            trajectoryDataControl.getNodes().Remove(newNodeDataControl);
            if (wasInitial)
            {
                trajectoryDataControl.initialNode = null;
                trajectory.setInitial(null);
            }
            trajectory.getNodes().Remove(newNode);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}
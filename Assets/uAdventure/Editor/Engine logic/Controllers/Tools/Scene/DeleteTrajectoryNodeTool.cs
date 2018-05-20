using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

using Side = uAdventure.Core.Trajectory.Side;
using Node = uAdventure.Core.Trajectory.Node;

namespace uAdventure.Editor
{
    public class DeleteTrajectoryNodeTool : Tool
    {


        NodeDataControl oldNodeDataControl;

        Trajectory trajectory;

        TrajectoryDataControl trajectoryDataControl;

        List<SideDataControl> oldSides;

        private bool wasInitial;

        public DeleteTrajectoryNodeTool(DataControl dataControl, Trajectory trajectory, TrajectoryDataControl trajectoryDataControl)
        {

            this.oldNodeDataControl = (NodeDataControl)dataControl;
            this.trajectory = trajectory;
            this.trajectoryDataControl = trajectoryDataControl;
            this.oldSides = new List<SideDataControl>();
            this.wasInitial = (trajectoryDataControl.getInitialNode() == oldNodeDataControl);
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

            Node temp = (Node)oldNodeDataControl.getContent();
            trajectory.removeNode(temp.getX(), temp.getY());
            trajectoryDataControl.getNodes().Remove(oldNodeDataControl);

            if (wasInitial)
            {
                trajectory.setInitial(null);
                trajectoryDataControl.initialNode = null;

                trajectory.setInitial(trajectory.getNodes()[0].getID());
                trajectoryDataControl.initialNode = trajectoryDataControl.getNodes()[0];
            }

            foreach (SideDataControl side in trajectoryDataControl.getSides())
            {
                if (!trajectory.getSides().Contains((Side)side.getContent()))
                {
                    oldSides.Add(side);
                }
            }
            foreach (SideDataControl side in oldSides)
            {
                trajectoryDataControl.getSides().Remove(side);
            }

            return true;
        }


        public override bool redoTool()
        {

            Node temp = (Node)oldNodeDataControl.getContent();
            trajectory.removeNode(temp.getX(), temp.getY());
            trajectoryDataControl.getNodes().Remove(oldNodeDataControl);

            if (wasInitial)
            {
                trajectory.setInitial(null);
                trajectoryDataControl.initialNode = null;

                trajectory.setInitial(trajectory.getNodes()[0].getID());
                trajectoryDataControl.initialNode = trajectoryDataControl.getNodes()[0];
            }

            foreach (SideDataControl side in trajectoryDataControl.getSides())
            {
                if (!trajectory.getSides().Contains((Side)side.getContent()))
                {
                    oldSides.Add(side);
                }
            }
            foreach (SideDataControl side in oldSides)
            {
                trajectoryDataControl.getSides().Remove(side);
            }

            Controller.Instance.updatePanel();

            return true;
        }


        public override bool undoTool()
        {

            Node temp = (Node)oldNodeDataControl.getContent();
            trajectory.getNodes().Add(temp);
            trajectoryDataControl.getNodes().Add(oldNodeDataControl);

            if (wasInitial)
            {
                trajectory.setInitial(temp.getID());
                trajectoryDataControl.initialNode = oldNodeDataControl;
            }

            foreach (SideDataControl side in oldSides)
            {
                trajectory.getSides().Add((Side)side.getContent());
                trajectoryDataControl.getSides().Add(side);
            }

            Controller.Instance.updatePanel();

            return true;
        }
    }
}
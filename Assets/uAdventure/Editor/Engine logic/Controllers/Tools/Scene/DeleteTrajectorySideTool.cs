using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteTrajectorySideTool : Tool
    {

        private SideDataControl sideDataControl;

        private Trajectory trajectory;

        private TrajectoryDataControl trajectoryDataControl;

        public DeleteTrajectorySideTool(SideDataControl dataControl, Trajectory trajectory, TrajectoryDataControl trajectoryDataControl)
        {

            this.sideDataControl = dataControl;
            this.trajectory = trajectory;
            this.trajectoryDataControl = trajectoryDataControl;
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

            trajectoryDataControl.getSides().Remove(sideDataControl);
            trajectory.getSides().Remove((Trajectory.Side)sideDataControl.getContent());
            return true;
        }


        public override bool redoTool()
        {

            trajectoryDataControl.getSides().Remove(sideDataControl);
            trajectory.getSides().Remove((Trajectory.Side)sideDataControl.getContent());
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            trajectoryDataControl.getSides().Add(sideDataControl);
            trajectory.getSides().Add((Trajectory.Side)sideDataControl.getContent());
            Controller.Instance.updatePanel();
            return true;
        }

    }
}
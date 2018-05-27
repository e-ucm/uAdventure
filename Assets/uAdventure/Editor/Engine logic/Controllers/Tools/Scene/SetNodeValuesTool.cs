using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using Node = uAdventure.Core.Trajectory.Node;

namespace uAdventure.Editor
{
    public class SetNodeValuesTool : Tool
    {

        private int oldX;

        private int oldY;

        private float oldScale;

        private int newX;

        private int newY;

        private Dictionary<string, float> oldLength;

        private float newScale;

        private Node node;

        private Trajectory trajectory;

        public SetNodeValuesTool(Node node, Trajectory trajectory, int newX, int newY, float newScale)
        {
            this.newX = newX;
            this.newY = newY;
            this.newScale = newScale;
            this.oldX = node.getX();
            this.oldY = node.getY();
            this.oldScale = node.getScale();
            this.oldLength = new Dictionary<string, float>();
            bool isEnd;
            foreach (Trajectory.Side side in trajectory.getSides())
            {
                isEnd = side.getIDEnd().Equals(node.getID());
                if (isEnd || side.getIDStart().Equals(node.getID()))
                {
                    oldLength.Add(isEnd ? side.getIDStart() : side.getIDEnd(), side.getLength());
                }
            }


            this.node = node;
            this.trajectory = trajectory;
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

            if (other is SetNodeValuesTool)
            {
                SetNodeValuesTool crvt = (SetNodeValuesTool)other;
                if (crvt.node != node)
                    return false;
                newX = crvt.newX;
                newY = crvt.newY;
                newScale = crvt.newScale;
                timeStamp = crvt.timeStamp;
                return true;
            }
            return false;
        }


        public override bool doTool()
        {
            node.setValues(newX, newY, newScale);
            if (newX != oldX || newY != oldY)
                foreach (Trajectory.Side side in trajectory.getSides())
                {

                    if (side.getIDEnd().Equals(node.getID()) || side.getIDStart().Equals(node.getID()))
                    {
                        Node start = trajectory.getNodeForId(side.getIDStart());
                        Node end = trajectory.getNodeForId(side.getIDEnd());
                        float x = start.getX() - end.getX();
                        float y = start.getY() - end.getY();
                        var newLength = new Vector2(x,y).magnitude;
                        if (Mathf.Approximately(side.getLength(), side.getRealLength()))
                            side.setLenght(newLength);
                        side.setRealLength(newLength);
                    }
                }
            return true;
        }


        public override bool redoTool()
        {

            node.setValues(newX, newY, newScale);
            if (newX != oldX || newY != oldY)
                foreach (Trajectory.Side side in trajectory.getSides())
                {
                    if (side.getIDEnd().Equals(node.getID()) || side.getIDStart().Equals(node.getID()))
                    {
                        Node start = trajectory.getNodeForId(side.getIDStart());
                        Node end = trajectory.getNodeForId(side.getIDEnd());
                        float x = start.getX() - end.getX();
                        float y = start.getY() - end.getY();
                        var newLength = new Vector2(x, y).magnitude;
                        if (Mathf.Approximately(side.getLength(), side.getRealLength()))
                            side.setLenght(newLength);
                        side.setRealLength(newLength);
                    }
                }
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            node.setValues(oldX, oldY, oldScale);
            bool isEnd;
            if (newX != oldX || newY != oldY)
                foreach (Trajectory.Side side in trajectory.getSides())
                {
                    isEnd = side.getIDEnd().Equals(node.getID());
                    if (side.getIDEnd().Equals(node.getID()) || side.getIDStart().Equals(node.getID()))
                    {
                        Node start = trajectory.getNodeForId(side.getIDStart());
                        Node end = trajectory.getNodeForId(side.getIDEnd());
                        float x = start.getX() - end.getX();
                        float y = start.getY() - end.getY();
                        side.setRealLength(new Vector2(x,y).magnitude);
                        side.setLenght(isEnd ? oldLength[side.getIDStart()] : oldLength[side.getIDEnd()]);
                    }
                }

            Controller.Instance.updatePanel();
            return true;
        }
    }
}
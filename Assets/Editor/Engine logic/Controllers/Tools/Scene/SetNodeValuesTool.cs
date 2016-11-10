using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Node = Trajectory.Node;

public class SetNodeValuesTool : Tool
{

    private int oldX;

    private int oldY;

    private float oldScale;

    private int newX;

    private int newY;

    private float newScale;

    private Node node;

    private Trajectory trajectory;

    private Dictionary<string, float> oldLengths;

    public SetNodeValuesTool(Node node, Trajectory trajectory, int newX, int newY, float newScale)
    {
        this.newX = newX;
        this.newY = newY;
        this.newScale = newScale;
        this.oldX = node.getX();
        this.oldY = node.getY();
        this.oldScale = node.getScale();
        this.node = node;
        this.trajectory = trajectory;
        this.oldLengths = new Dictionary<string, float>();
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

        if (other is SetNodeValuesTool ) {
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
                    oldLengths.Add(side.getIDStart() + ";" + side.getIDEnd(), side.getLength());
                    Trajectory.Node start = trajectory.getNodeForId(side.getIDStart());
                    Trajectory.Node end = trajectory.getNodeForId(side.getIDEnd());
                    float x = start.getX() - end.getX();
                    float y = start.getY() - end.getY();
                    side.setLenght((float)Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
                    side.setRealLength((float)Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
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
                    side.setLenght((float)Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
                    side.setRealLength((float)Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
                }
            }
        Controller.getInstance().updatePanel();
        return true;
    }

    
    public override bool undoTool()
    {

        node.setValues(oldX, oldY, oldScale);
        if (newX != oldX || newY != oldY)
            foreach (Trajectory.Side side in trajectory.getSides())
            {
                if (side.getIDEnd().Equals(node.getID()) || side.getIDStart().Equals(node.getID()))
                {
                    Node start = trajectory.getNodeForId(side.getIDStart());
                    Node end = trajectory.getNodeForId(side.getIDEnd());
                    float x = start.getX() - end.getX();
                    float y = start.getY() - end.getY();
                    side.setRealLength((float)Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
                }

                float temp = oldLengths[side.getIDStart() + ";" + side.getIDEnd()];
                if (temp != null)
                    side.setLenght(temp);
            }

        Controller.getInstance().updatePanel();
        return true;
    }
}

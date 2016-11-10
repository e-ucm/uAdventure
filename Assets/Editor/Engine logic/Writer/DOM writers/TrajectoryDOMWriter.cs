using System;
using UnityEngine;
using System.Collections;
using System.Xml;

public class TrajectoryDOMWriter
{

    /**
     * Private constructor.
     */

    private TrajectoryDOMWriter()
    {

    }

    public static XmlNode buildDOM(Trajectory trajectory)
    {

        XmlElement itemElement = null;

        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        itemElement = doc.CreateElement("trajectory");

        foreach (Trajectory.Node node in trajectory.getNodes())
        {
            XmlElement nodeElement = doc.CreateElement("node");
            nodeElement.SetAttribute("id", node.getID());
            nodeElement.SetAttribute("x", node.getX().ToString());
            nodeElement.SetAttribute("y", node.getY().ToString());
            nodeElement.SetAttribute("scale", node.getScale().ToString());
            itemElement.AppendChild(nodeElement);
        }

        if (trajectory.getInitial() != null)
        {
            XmlElement initialNodeElement = doc.CreateElement("initialnode");
            initialNodeElement.SetAttribute("id", trajectory.getInitial().getID());
            itemElement.AppendChild(initialNodeElement);
        }

        foreach (Trajectory.Side side in trajectory.getSides())
        {
            XmlElement sideElement = doc.CreateElement("side");
            sideElement.SetAttribute("idStart", side.getIDStart());
            sideElement.SetAttribute("idEnd", side.getIDEnd());
            sideElement.SetAttribute("length", ((int) side.getLength()).ToString());
            itemElement.AppendChild(sideElement);
        }

        return itemElement;
    }
}
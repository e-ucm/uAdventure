
using System;
using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System.Globalization;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Trajectory))]
    public class TrajectoryDOMWriter : ParametrizedDOMWriter
    {

        /**
         * Private constructor.
         */

        public TrajectoryDOMWriter()
        {

        }

        protected override void FillNode(XmlNode xmlNode, object target, params IDOMWriterParam[] options)
        {
            var trajectory = target as Trajectory;

            var doc = Writer.GetDoc();

            foreach (Trajectory.Node node in trajectory.getNodes())
            {
                XmlElement nodeElement = doc.CreateElement("node");
                nodeElement.SetAttribute("id", node.getID());
                nodeElement.SetAttribute("x", node.getX().ToString());
                nodeElement.SetAttribute("y", node.getY().ToString());
                nodeElement.SetAttribute("scale", node.getScale().ToString(CultureInfo.InvariantCulture));
                xmlNode.AppendChild(nodeElement);
            }

            if (trajectory.getInitial() != null)
            {
                XmlElement initialNodeElement = doc.CreateElement("initialnode");
                initialNodeElement.SetAttribute("id", trajectory.getInitial().getID());
                xmlNode.AppendChild(initialNodeElement);
            }

            foreach (Trajectory.Side side in trajectory.getSides())
            {
                XmlElement sideElement = doc.CreateElement("side");
                sideElement.SetAttribute("idStart", side.getIDStart());
                sideElement.SetAttribute("idEnd", side.getIDEnd());
                sideElement.SetAttribute("length", ((int)side.getLength()).ToString());
                xmlNode.AppendChild(sideElement);
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "trajectory";
        }
    }
}
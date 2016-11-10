using UnityEngine;
using System.Collections;
using System.Xml;
using System.Globalization;

public class TrajectorySubParser_ : Subparser_
{

    /**
      * Trajectory being parsed.
      */
    private Trajectory trajectory;

    /**
     * Subparser for effects and conditions.
     */
    private SubParser subParser;

    /**
     * Scene to add the trajectory
     */
    private Scene scene;

    public TrajectorySubParser_(Chapter chapter, Scene scene) : base(chapter)
    {
        this.trajectory = new Trajectory();
        //scene.setTrajectory(trajectory);
        this.scene = scene;
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            nodes = element.SelectNodes("node"),
            sides = element.SelectNodes("side"),
            initialnodes = element.SelectNodes("initialnode");

        string tmpArgVal;

        foreach (XmlElement el in nodes)
        {
            int x = 0, y = 0;
            float scale = 1.0f;

            string id = "";

            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                id = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("scale");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                scale = float.Parse(tmpArgVal, CultureInfo.InvariantCulture);
            }
            trajectory.addNode(id, x, y, scale);
        }

        foreach (XmlElement el in sides)
        {
            string idStart = "";
            string idEnd = "";
            int length = -1;

            tmpArgVal = el.GetAttribute("idStart");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idStart = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("idEnd");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idEnd = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("length");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                length = int.Parse(tmpArgVal);
            }

            trajectory.addSide(idStart, idEnd, length);
        }

        foreach (XmlElement el in initialnodes)
        {
            string id = "";

            tmpArgVal = el.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                id = tmpArgVal;
            }

            trajectory.setInitial(id);
        }

        if (trajectory.getNodes().Count != 0)
        {
            trajectory.deleteUnconnectedNodes();
            scene.setTrajectory(trajectory);

        }
    }
}
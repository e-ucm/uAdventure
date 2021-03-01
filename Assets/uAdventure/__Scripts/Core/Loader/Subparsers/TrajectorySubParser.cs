using UnityEngine;
using System.Collections;
using System.Xml;
using System.Globalization;

namespace uAdventure.Core
{
	[DOMParser("trajectory")]
	[DOMParser(typeof(Trajectory))]
	public class TrajectorySubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			Trajectory trajectory = new Trajectory();

			foreach (XmlElement el in element.SelectNodes("node"))
			{
				string id = el.GetAttribute("id");
				int x = ExParsers.ParseDefault(el.GetAttribute("x"), 0),
					y = ExParsers.ParseDefault(el.GetAttribute("y"), 0);
				float scale = ExParsers.ParseDefault(el.GetAttribute("scale"), CultureInfo.InvariantCulture, 1.0f);
				scale = Mathf.Max(0, scale);
				trajectory.addNode(id, x, y, scale);
            }

			foreach (XmlElement el in element.SelectNodes("side"))
            {
				string idStart = el.GetAttribute("idStart");
				string idEnd = el.GetAttribute("idEnd");
				int length = ExParsers.ParseDefault(el.GetAttribute("length"), -1);

                trajectory.addSide(idStart, idEnd, length);
            }

			var initialNode = element.SelectSingleNode("initialnode") as XmlElement;
			if(initialNode != null)
				trajectory.setInitial(initialNode.GetAttribute("id"));

            if (trajectory.getNodes().Count != 0)
            {
                // trajectory.deleteUnconnectedNodes();
				return trajectory;
            }

			return null;
        }
    }
}
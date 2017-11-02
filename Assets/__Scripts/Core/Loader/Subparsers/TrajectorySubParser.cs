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
				string id = el.GetAttribute("id") ?? "";
				int x = int.Parse(el.GetAttribute("x") ?? "0"),
					y = int.Parse(el.GetAttribute("y") ?? "0");
				float scale = float.Parse(el.GetAttribute("scale") ?? "1.0", CultureInfo.InvariantCulture);

                trajectory.addNode(id, x, y, scale);
            }

			foreach (XmlElement el in element.SelectNodes("side"))
            {
				string idStart = el.GetAttribute("idStart") ?? "";
				string idEnd = el.GetAttribute("idEnd") ?? "";
				//int length = int.Parse(el.GetAttribute("length") ?? "-1");

                trajectory.addSide(idStart, idEnd, -1);
            }

			var initialNode = element.SelectSingleNode("initialnode");
			if(initialNode != null)
				trajectory.setInitial(element.GetAttribute("id") ?? "");

            if (trajectory.getNodes().Count != 0)
            {
                trajectory.deleteUnconnectedNodes();
				return trajectory;
            }

			return null;
        }
    }
}
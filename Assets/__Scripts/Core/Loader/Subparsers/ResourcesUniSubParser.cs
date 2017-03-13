﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
namespace uAdventure.Core {

	[DOMParser("resources")]
	[DOMParser(typeof(ResourcesUni))]
	public class ResourcesUniSubParser : IDOMParser 
	{

		public object DOMParse(XmlElement element, params object[] parameters)
		{
			var r = new ResourcesUni();
			r.setName(element.GetAttribute("name") ?? "");

			foreach (XmlElement asset in element.SelectNodes("asset"))
			{
				r.addAsset(
					asset.GetAttribute("type") ?? "", 
					asset.GetAttribute("uri") ?? "");
			}

			r.setConditions(DOMParserUtility.DOMParse (element.SelectSingleNode("condition") as XmlElement, parameters) ?? new Conditions());

			return r;
		}

	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("atrezzo")]
	[DOMParser(typeof(Atrezzo))]
	public class AtrezzoSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			XmlNodeList
				resourcess = element.SelectNodes ("resources"),
				descriptions = element.SelectNodes ("description"),
				assets,
				conditions;

            string tmpArgVal;

            string atrezzoId = element.GetAttribute("id");
            Atrezzo atrezzo = new Atrezzo(atrezzoId);

            if (element.SelectSingleNode("documentation") != null)
                atrezzo.setDocumentation(element.SelectSingleNode("documentation").InnerText);

            foreach (XmlElement el in resourcess)
            {
				ResourcesUni currentResources = new ResourcesUni();
                currentResources.setName(el.GetAttribute("name") ?? "");

                assets = el.SelectNodes("asset");
                foreach (XmlElement ell in assets)
                {
					string type = ell.GetAttribute("type") ?? "";
					string path = ell.GetAttribute("uri") ?? "";

                    currentResources.addAsset(type, path);
                }

				currentResources.setConditions(DOMParserUtility.DOMParse<Conditions>(el.SelectSingleNode("condition"), parameters) ?? new Conditions());
                atrezzo.addResources(currentResources);
            }

			atrezzo.setDescriptions(DOMParserUtility.DOMParse<Description>(descriptions, parameters).ToList());

            return atrezzo;
        }
    }
}
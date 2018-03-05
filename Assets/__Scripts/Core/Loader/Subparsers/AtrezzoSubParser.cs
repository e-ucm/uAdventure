using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("atrezzo", "atrezzoobject")]
	[DOMParser(typeof(Atrezzo))]
	public class AtrezzoSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
		{
            XmlNodeList
                resourcess = element.SelectNodes("resources"),
                descriptions = element.SelectNodes("description");
            XmlElement documentation = element.SelectSingleNode("documentation") as XmlElement;

            Atrezzo atrezzo = new Atrezzo(element.GetAttribute("id"));

            // DOCUMENTATION
            if (documentation != null)
                atrezzo.setDocumentation(documentation.InnerText);

            // RESOURCES
            foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(resourcess, parameters))
                atrezzo.addResources(res);

            // DESCRIPTIONS
            atrezzo.setDescriptions(DOMParserUtility.DOMParse<Description>(descriptions, parameters).ToList());

            return atrezzo;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("description")]
	[DOMParser(typeof(Description))]
	public class DescriptionsSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
        {
			Description description = new Description ();

			XmlElement
			name = element.SelectSingleNode ("name") as XmlElement,
			brief = element.SelectSingleNode ("brief") as XmlElement,
			detailed = element.SelectSingleNode ("detailed") as XmlElement;

			if(name != null)
            {
				string soundPath = name.GetAttribute("soundPath");
                description.setNameSoundPath(soundPath);
				description.setName(name.InnerText);
            }

			if(brief != null)
			{
				string soundPath = brief.GetAttribute("soundPath");
                description.setDescriptionSoundPath(soundPath);
				description.setDescription(brief.InnerText);
			}

			if(detailed != null)
			{
				string soundPath = detailed.GetAttribute("soundPath");
                description.setDetailedDescriptionSoundPath(soundPath);
				description.setDetailedDescription(detailed.InnerText);
            }

			description.setConditions(DOMParserUtility.DOMParse<Conditions>(element.SelectSingleNode ("condition"), parameters) ?? new Conditions());
			return description;
        }
    }
}
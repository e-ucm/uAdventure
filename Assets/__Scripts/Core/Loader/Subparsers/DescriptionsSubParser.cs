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
			name = element.SelectNodes ("name"),
			brief = element.SelectNodes ("brief"),
			detailed = element.SelectNodes ("detailed");

			if(name != null)
            {
				string soundPath = name.GetAttribute("soundPath") ?? "";
                description.setNameSoundPath(soundPath);
				description.setName(name.InnerText);
            }

			if(brief != null)
			{
				string soundPath = brief.GetAttribute("soundPath") ?? "";
                description.setDescriptionSoundPath(soundPath);
				description.setDescription(brief.InnerText);
			}

			if(detailed != null)
			{
				string soundPath = detailed.GetAttribute("soundPath") ?? "";
                description.setDetailedDescriptionSoundPath(soundPath);
				description.setDetailedDescription(detailed.InnerText);
            }

			description.setConditions(DOMParserUtility.DOMParse (element.SelectSingleNode ("condition")) as Conditions ?? new Conditions());
        }
    }
}
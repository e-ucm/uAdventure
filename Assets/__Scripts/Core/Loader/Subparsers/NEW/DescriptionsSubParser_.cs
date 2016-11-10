using UnityEngine;
using System.Collections;
using System.Xml;

public class DescriptionsSubParser_ : Subparser_
{
    private Conditions currentConditions;

    private Description description;

    public DescriptionsSubParser_(Description description, Chapter chapter) : base(chapter)
    {
        this.description = description;
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            names = element.SelectNodes("name"),
            briefs = element.SelectNodes("brief"),
            detaileds = element.SelectNodes("detailed"),
            conditions = element.SelectNodes("condition");

        string tmpArgVal;
        foreach (XmlElement el in names)
        {
            string soundPath = "";

            tmpArgVal = el.GetAttribute("soundPath");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                soundPath = tmpArgVal;
            }
            description.setNameSoundPath(soundPath);
            description.setName(el.InnerText);
        }

        foreach (XmlElement el in briefs)
        {
            string soundPath = "";

            tmpArgVal = el.GetAttribute("soundPath");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                soundPath = tmpArgVal;
            }
            description.setDescriptionSoundPath(soundPath);
            description.setDescription(el.InnerText);
        }
        foreach (XmlElement el in detaileds)
        {
            string soundPath = "";

            tmpArgVal = el.GetAttribute("soundPath");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                soundPath = tmpArgVal;
            }
            description.setDetailedDescriptionSoundPath(soundPath);
            description.setDetailedDescription(el.InnerText);
        }
        foreach (XmlElement el in conditions)
        {
            currentConditions = new Conditions();
            new ConditionSubParser_(currentConditions, chapter).ParseElement(el);
            this.description.setConditions(currentConditions);
        }
    }
}
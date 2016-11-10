using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class AtrezzoSubParser_ : Subparser_
{
    /**
     * Atrezzo object being parsed.
     */
    private Atrezzo atrezzo;

    /**
     * Current resources being parsed.
     */
    private ResourcesUni currentResources;

    /**
     * Current conditions being parsed.
     */
    private Conditions currentConditions;

    /**
     * Current effects being parsed.
     */
    private Effects currentEffects;

    private List<Description> descriptions;

    private Description description;
    private string currentstring;

    public AtrezzoSubParser_(Chapter chapter) : base(chapter)
    {
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            resourcess = element.SelectNodes("resources"),
            descriptionss = element.SelectNodes("description"),
            assets,
            conditions,
            effects = element.SelectNodes("effect");

        string tmpArgVal;

        string atrezzoId = element.GetAttribute("id");
        atrezzo = new Atrezzo(atrezzoId);

        descriptions = new List<Description>();
        atrezzo.setDescriptions(descriptions);

        if (element.SelectSingleNode("documentation") != null)
            atrezzo.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        foreach (XmlElement el in resourcess)
        {
            currentResources = new ResourcesUni();
            tmpArgVal = el.GetAttribute("name");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentResources.setName(el.GetAttribute(tmpArgVal));
            }

            assets = el.SelectNodes("asset");
            foreach (XmlElement ell in assets)
            {
                string type = "";
                string path = "";

                tmpArgVal = ell.GetAttribute("type");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    type = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("uri");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    path = tmpArgVal;
                }
                currentResources.addAsset(type, path);
            }

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentResources.setConditions(currentConditions);
            }

            atrezzo.addResources(currentResources);
        }

        foreach (XmlElement el in descriptionss)
        {
            description = new Description();
            new DescriptionsSubParser_(description, chapter).ParseElement(el);
            this.descriptions.Add(description);
        }

        foreach (XmlElement el in effects)
        {
            currentEffects = new Effects();
            new EffectSubParser_(currentEffects, chapter).ParseElement(el);
        }

        chapter.addAtrezzo(atrezzo);
    }
}
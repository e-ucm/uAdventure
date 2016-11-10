using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ItemSubParser_ : Subparser_
{

    /**
     * parsedObject being parsed.
     */
    private Item parsedObject;

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

    /**
     * Subparser for effects and conditions.
     */
    private SubParser subParser;


    private List<Description> descriptions;

    private Description description;

    public ItemSubParser_(Chapter chapter) : base(chapter)
    {
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            resourcess = element.SelectNodes("resources"),
            descriptionss = element.SelectNodes("description"),
            assets,
            conditions,
            actionss = element.SelectNodes("actions"),
            effects = element.SelectNodes("effect");

        string tmpArgVal;

        string parsedObjectId = "";
        bool returnsWhenDragged = true;

        Item.BehaviourType behaviour = Item.BehaviourType.NORMAL;
        long resourceTransition = 0;

        if (element.SelectSingleNode("documentation") != null)
            parsedObject.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        tmpArgVal = element.GetAttribute("id");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            parsedObjectId = tmpArgVal;
        }

        tmpArgVal = element.GetAttribute("returnsWhenDragged");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            returnsWhenDragged = (tmpArgVal.Equals("yes") ? true : false);
        }

        tmpArgVal = element.GetAttribute("behaviour");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            if (tmpArgVal.Equals("normal"))
            {
                behaviour = Item.BehaviourType.NORMAL;
            }
            else if (tmpArgVal.Equals("atrezzo"))
            {
                behaviour = Item.BehaviourType.ATREZZO;
            }
            else if (tmpArgVal.Equals("first-action"))
            {
                behaviour = Item.BehaviourType.FIRST_ACTION;
            }
        }

        tmpArgVal = element.GetAttribute("resources-transition-time");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            resourceTransition = long.Parse(tmpArgVal);
        }

        parsedObject = new Item(parsedObjectId);
        parsedObject.setReturnsWhenDragged(returnsWhenDragged);

        parsedObject.setResourcesTransitionTime(resourceTransition);
        parsedObject.setBehaviour(behaviour);

        descriptions = new List<Description>();
        parsedObject.setDescriptions(descriptions);

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

            parsedObject.addResources(currentResources);
        }

        foreach (XmlElement el in descriptionss)
        {
            description = new Description();
            new DescriptionsSubParser_(description, chapter).ParseElement(el);
            this.descriptions.Add(description);
        }
        foreach (XmlElement el in actionss)
        {
            new ActionsSubParser_(chapter, parsedObject).ParseElement(el);
        }
        foreach (XmlElement el in effects)
        {
            new ActionsSubParser_(chapter, parsedObject).ParseElement(el);
        }

        chapter.addItem(parsedObject);
    }
}
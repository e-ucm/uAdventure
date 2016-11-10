using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ActiveAreaSubParser_ : Subparser_
{
    /**
     * ActiveArea being parsed.
     */
    private ActiveArea activeArea;

    /**
     * Current conditions being parsed.
     */
    private Conditions currentConditions;

    /**
     * Current effects being parsed.
     */
    private Effects currentEffects;

    private Scene scene;
    private int nAreas ;

    private List<Description> descriptions;

    private Description description;

    public ActiveAreaSubParser_(Chapter chapter, Scene scene, int nAreas): base(chapter)
    {
        this.nAreas = nAreas;
        this.scene = scene;

    }


    private string generateId()
    {

        return "area" + (nAreas + 1) + "scene" + scene.getId();
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            points = element.SelectNodes("point"),
            descriptionss = element.SelectNodes("description"),
            actionss = element.SelectNodes("actions"),
            conditions = element.SelectNodes("condition");
            //,
              //effects = element.SelectNodes("effect");

        string tmpArgVal;


        int x = 0, y = 0, width = 0, height = 0;
        string id = null;
        bool rectangular = true;
        int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
        bool hasInfluence = false;

        tmpArgVal = element.GetAttribute("rectangular");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            rectangular = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("x");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            x = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("y");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            y = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("width");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            width = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("height");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            height = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("id");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            id = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("hasInfluenceArea");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            hasInfluence = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("influenceX");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            influenceX = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("influenceY");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            influenceY = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("influenceWidth");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            influenceWidth = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("influenceHeight");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            influenceHeight = int.Parse(tmpArgVal);
        }
        
        activeArea = new ActiveArea((id == null ? generateId() : id), rectangular, x, y, width, height);
        if (hasInfluence)
        {
            InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
            activeArea.setInfluenceArea(influenceArea);
        }

        if (element.SelectSingleNode("documentation") != null)
            activeArea.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        descriptions = new List<Description>();
        activeArea.setDescriptions(descriptions);

        foreach (XmlElement el in descriptionss)
        {
            description = new Description();
            new DescriptionsSubParser_(description, chapter).ParseElement(el);
            this.descriptions.Add(description);
        }

        foreach (XmlElement el in points)
        {
            if (activeArea != null)
            {
                int x_ = 0, y_ = 0;

                tmpArgVal = el.GetAttribute("x");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    x_ = int.Parse(tmpArgVal);
                }
                tmpArgVal = el.GetAttribute("y");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    y_ = int.Parse(tmpArgVal);
                }

                Vector2 point = new Vector2(x_, y_);
                activeArea.addVector2(point);
            }
        }

        foreach (XmlElement el in actionss)
        {
            new ActionsSubParser_(chapter, activeArea).ParseElement(el);
        }
        
        foreach (XmlElement el in conditions)
        {
            currentConditions = new Conditions();
            new ConditionSubParser_(currentConditions, chapter).ParseElement(el);
            this.activeArea.setConditions(currentConditions);
        }
        scene.addActiveArea(activeArea);
    }
    //TODO: test if it's working
    //public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    //{
   
    //        // If it is a effect tag, create new effects and switch the state
    //        else if (qName.Equals("effect"))
    //        {
    //            subParser = new EffectSubParser(currentEffects, chapter);
    //            subParsing = SUBPARSING_EFFECT;
    //        }
    //    }

}

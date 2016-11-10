using UnityEngine;
using System.Collections;
using System.Xml;

public class BarrierSubParser_ : Subparser_
{

    private Scene scene;

    private int nBarriers;
    /**
   * Barrier being parsed.
   */
    private Barrier barrier;
    /**
   * Current conditions being parsed.
   */
    private Conditions currentConditions;

    public BarrierSubParser_(Chapter chapter, Scene scene, int nBarriers) : base(chapter)
    {
        this.nBarriers = nBarriers;
        this.scene = scene;
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            conditions = element.SelectNodes("condition");

        string tmpArgVal;


        int x = 0, y = 0, width = 0, height = 0;

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
        barrier = new Barrier(generateId(), x, y, width, height);

        if (element.SelectSingleNode("documentation") != null)
            barrier.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        foreach (XmlElement ell in conditions)
        {
            currentConditions = new Conditions();
            new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
            this.barrier.setConditions(currentConditions);
        }

        scene.addBarrier(barrier);
    }

    private string generateId()
    {

        return (nBarriers + 1).ToString();
    }

}
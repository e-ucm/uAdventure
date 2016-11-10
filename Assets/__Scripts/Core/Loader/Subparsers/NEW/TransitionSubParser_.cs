using UnityEngine;
using System.Collections;
using System.Xml;

public class TransitionSubParser_ : Subparser_ {
    private Animation animation;

    private Transition transition;

    public TransitionSubParser_(Animation animation):base(null)
    {
        this.animation = animation;
        transition = new Transition();
    }

    public override void ParseElement(XmlElement element)
    {
        string tmpArgVal = "";

        tmpArgVal = element.GetAttribute("type");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            if (tmpArgVal.Equals("none"))
                transition.setType(Transition.TYPE_NONE);
            else if (tmpArgVal.Equals("fadein"))
                transition.setType(Transition.TYPE_FADEIN);
            else if (tmpArgVal.Equals("vertical"))
                transition.setType(Transition.TYPE_VERTICAL);
            else if (tmpArgVal.Equals("horizontal"))
                transition.setType(Transition.TYPE_HORIZONTAL);
        }

        tmpArgVal = element.GetAttribute("time");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            transition.setTime(long.Parse(tmpArgVal));
        }

        animation.getTransitions().Add(transition);
    }
}
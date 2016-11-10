using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TransitionSubParser : SubParser {
    private Animation animation;

    private Transition transition;

    public TransitionSubParser(Animation animation):base(null)
    {
        this.animation = animation;
        transition = new Transition();
    }

    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        if (qName.Equals("transition"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("type"))
                {
                    if (entry.Value.ToString().Equals("none"))
                        transition.setType(Transition.TYPE_NONE);
                    else if (entry.Value.ToString().Equals("fadein"))
                        transition.setType(Transition.TYPE_FADEIN);
                    else if (entry.Value.ToString().Equals("vertical"))
                        transition.setType(Transition.TYPE_VERTICAL);
                    else if (entry.Value.ToString().Equals("horizontal"))
                        transition.setType(Transition.TYPE_HORIZONTAL);
                }
                else if (entry.Key.Equals("time"))
                {
                    transition.setTime(long.Parse(entry.Value.ToString()));
                }
            }
        }
    }

    public override void endElement(string namespaceURI, string sName, string qName)
    {

        if (qName.Equals("transition"))
        {
            animation.getTransitions().Add(transition);
        }
    }

}

using UnityEngine;
using System.Collections;
using System.Xml;

public class AnimationHandler_
{
    /**
     * Resources to store the current resources being read
     */
    ResourcesUni currentResources;
    /**
     * Animation being read.
     */
    private Animation animation;

    private Frame frame;
    private Transition transition;
    /**
     * InputStreamCreator used in resolveEntity to find dtds (only required in
     * Applet mode)
     */
    private InputStreamCreator isCreator;

    private ImageLoaderFactory factory;

    public AnimationHandler_(InputStreamCreator isCreator, ImageLoaderFactory imageloader)
    {
        this.factory = imageloader;
        this.isCreator = isCreator;
    }

    public void Parse(string path_)
    {
        XmlDocument xmld = new XmlDocument();
        xmld.Load(path_);

        XmlElement element = xmld.DocumentElement;

        XmlNodeList
            frames = element.SelectNodes("/animation/frame"),
            transitions = element.SelectNodes("/animation/transition"),
            resources = element.SelectNodes("/animation/resources"),
            assets;

        string tmpArgVal;

        XmlNode animationNode = element.SelectSingleNode("/animation");
      
        tmpArgVal = animationNode.Attributes["id"].Value;
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            animation = new Animation(tmpArgVal, factory);
            animation.getFrames().Clear();
            animation.getTransitions().Clear();
        }

        tmpArgVal = animationNode.Attributes["slides"].Value;
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            if (tmpArgVal.Equals("yes"))
                animation.setSlides(true);
            else
                animation.setSlides(false);
        }

        tmpArgVal = animationNode.Attributes["usetransitions"].Value;
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            if (tmpArgVal.Equals("yes"))
                animation.setUseTransitions(true);
            else
                animation.setUseTransitions(false);
        }

        if (element.SelectSingleNode("documentation") != null)
            animation.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        foreach (XmlElement el in frames)
        {
            new FrameSubParser_(animation).ParseElement(el);
        }
        foreach (XmlElement el in transitions)
        {
            new TransitionSubParser_(animation).ParseElement(el);
        }

        foreach (XmlElement el in resources)
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

            animation.addResources(currentResources);
        }

    }

    public Animation getAnimation()
    {

        return animation;
    }
}
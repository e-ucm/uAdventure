using UnityEngine;
using System.Collections;
using System.Xml;

public class FrameSubParser_ : Subparser_ {

    private Animation animation;

    private Frame frame;

    private ResourcesUni currentResources;

    public FrameSubParser_(Animation animation):base(null)
    {
        this.animation = animation;
        frame = new Frame(animation.getImageLoaderFactory());
        animation.getFrames().Add(frame);
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            resourcess = element.SelectNodes("resources"),
            assets = element.SelectNodes("next-scene");

        string tmpArgVal;

        tmpArgVal = element.GetAttribute("uri");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            frame.setUri(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("type");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            if (tmpArgVal.Equals("image"))
                frame.setType(Frame.TYPE_IMAGE);
            if (tmpArgVal.Equals("video"))
                frame.setType(Frame.TYPE_VIDEO);
        }
        tmpArgVal = element.GetAttribute("time");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            frame.setTime(long.Parse(tmpArgVal));
        }
        tmpArgVal = element.GetAttribute("waitforclick");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            frame.setWaitforclick(tmpArgVal.Equals("yes"));
        }
        tmpArgVal = element.GetAttribute("soundUri");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            frame.setSoundUri(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("maxSoundTime");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            frame.setMaxSoundTime(int.Parse(tmpArgVal));
        }

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
        }

        frame.addResources(currentResources);
    }
}

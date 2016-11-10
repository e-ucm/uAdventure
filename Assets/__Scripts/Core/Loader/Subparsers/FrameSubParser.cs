using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class FrameSubParser : SubParser {
    private Animation animation;

    private Frame frame;

    private ResourcesUni currentResources;

    public FrameSubParser(Animation animation):base(null)
    {
        this.animation = animation;
        frame = new Frame(animation.getImageLoaderFactory());
        animation.getFrames().Add(frame);
    } 

    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        if (qName.Equals("frame"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("uri"))
                    frame.setUri(entry.Value.ToString());
                if (entry.Key.Equals("type"))
                {
                    if (entry.Value.ToString().Equals("image"))
                        frame.setType(Frame.TYPE_IMAGE);
                    if (entry.Value.ToString().Equals("video"))
                        frame.setType(Frame.TYPE_VIDEO);
                }
                if (entry.Key.Equals("time"))
                {
                    frame.setTime(long.Parse(entry.Value.ToString()));
                }
                if (entry.Key.Equals("waitforclick"))
                    frame.setWaitforclick(entry.Value.ToString().Equals("yes"));
                if (entry.Key.Equals("soundUri"))
                    frame.setSoundUri(entry.Value.ToString());
                if (entry.Key.Equals("maxSoundTime"))
                    frame.setMaxSoundTime(int.Parse(entry.Value.ToString()));
            }
        }

        if (qName.Equals("resources"))
        {
            currentResources = new ResourcesUni();

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("name"))
                    currentResources.setName(entry.Value.ToString());
            }

        }

        if (qName.Equals("asset"))
        {
            string type = "";
            string path = "";

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("type"))
                    type = entry.Value.ToString();
                if (entry.Key.Equals("uri"))
                    path = entry.Value.ToString();
            }

            // If the asset is not an special one
            //if( !AssetsController.isAssetSpecial( path ) )
            currentResources.addAsset(type, path);
        }
    }

    public override void endElement(string namespaceURI, string sName, string qName)
    {

        if (qName.Equals("resources"))
        {
            frame.addResources(currentResources);
        }

    }
}

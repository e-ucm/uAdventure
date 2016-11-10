using UnityEngine;
using System.Collections;
using System.Xml;

public class AdaptationSubParser_ : Subparser_
{

    private AdaptationProfile profile;

    public AdaptationSubParser_(Chapter chapter):base(chapter)
    {
        profile = new AdaptationProfile();
    }

    public override void ParseElement(XmlElement element)
    {
    }
}

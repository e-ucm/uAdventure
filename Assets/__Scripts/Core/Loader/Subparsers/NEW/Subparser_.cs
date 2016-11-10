using UnityEngine;
using System.Collections;
using System.Xml;

public abstract class Subparser_ {

    protected Chapter chapter;

    /**
     * Constructor.
     */
    public Subparser_(Chapter chapter)
    {
        this.chapter = chapter;
    }

    public abstract void ParseElement(XmlElement element);
}

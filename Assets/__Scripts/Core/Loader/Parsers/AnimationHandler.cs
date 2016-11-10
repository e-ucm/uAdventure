using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class AnimationHandler : XMLHandler {


    /**
     * string to store the current string in the XML file
     */
    string currentstring;

    /**
     * Resources to store the current resources being read
     */
    ResourcesUni currentResources;

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading transition
     */
    private const int READING_TRANSITION = 1;

    /**
     * Constant for reading frame
     */
    private const int READING_FRAME = 2;

    /**
     * Stores the current element being read.
     */
    private int reading = READING_NONE;

    /**
     * Current subparser being used
     */
    private SubParser subParser;

    /**
     * Animation being read.
     */
    private Animation animation;

    /**
     * InputStreamCreator used in resolveEntity to find dtds (only required in
     * Applet mode)
     */
    private InputStreamCreator isCreator;

    private ImageLoaderFactory factory;

    public AnimationHandler(InputStreamCreator isCreator, ImageLoaderFactory imageloader)
    {
        this.factory = imageloader;
        this.isCreator = isCreator;
    }

    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        if (this.reading == READING_NONE)
        {

            if (qName.Equals("animation"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                    {
                        animation = new Animation(entry.Value.ToString(), factory);
                        animation.getFrames().Clear();
                        animation.getTransitions().Clear();
                    }

                    if (entry.Key.Equals("slides"))
                    {
                        if (entry.Value.ToString().Equals("yes"))
                            animation.setSlides(true);
                        else
                            animation.setSlides(false);
                    }

                    if (entry.Key.Equals("usetransitions"))
                    {
                        if (entry.Value.ToString().Equals("yes"))
                            animation.setUseTransitions(true);
                        else
                            animation.setUseTransitions(false);
                    }
                }
            }

            if (qName.Equals("documentation"))
            {
                currentstring = string.Empty;
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

            else if (qName.Equals("asset"))
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

                currentResources.addAsset(type, path);
            }

            if (qName.Equals("frame"))
            {
                subParser = new FrameSubParser(animation);
                reading = READING_FRAME;
            }

            if (qName.Equals("transition"))
            {
                subParser = new TransitionSubParser(animation);
                reading = READING_TRANSITION;
            }
        }
        if (reading != READING_NONE)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
        }

    }

    public override void endElement(string namespaceURI, string sName, string qName)
    {

        if (qName.Equals("documentation"))
        {
            if (reading == READING_NONE)
                animation.setDocumentation(currentstring.ToString().Trim());
        }
        else if (qName.Equals("resources"))
        {
            animation.addResources(currentResources);
        }

        if (reading != READING_NONE)
        {
            subParser.endElement(namespaceURI, sName, qName);
            if (qName.Equals("transition") || qName.Equals("frame"))
                reading = READING_NONE;
        }

    }

    //@Override
    //public void error(SAXParseException exception) throws SAXParseException
    //{

    //    // On validation, propagate exception
    //    exception.printStackTrace( );
    //    throw exception;
    //}

    public override void characters(char[] buf, int offset, int len)
    {

        // Append the new characters
        currentstring += new string(buf, offset, len);
        if (reading != READING_NONE)
        {
            subParser.characters(buf, offset, len);
        }
    }

    public Animation getAnimation()
    {

        return animation;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.EntityResolver#resolveEntity(java.lang.string, java.lang.string)
     */
    //@Override
    //public InputSource resolveEntity(string publicId, string systemId)
    //{

    //    int startFilename = systemId.LastIndexOf("/") + 1;
    //    string filename = systemId.Substring(startFilename, systemId.Length);
    //    InputStream inputStream = isCreator.buildInputStream(filename);
    //    return new InputSource(inputStream);
    //}
}

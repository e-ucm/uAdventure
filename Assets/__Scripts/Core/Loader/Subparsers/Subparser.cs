using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public abstract class SubParser {


    /* Attributes */

    /**
     * string to store the current string in the XML file.
     */
    protected string currentstring;

    /**
     * Chapter in which the data will be stored.
     */
    protected Chapter chapter;

    /* Methods */

    /**
     * Constructor.
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public SubParser(Chapter chapter)
    {
        this.chapter = chapter;
        currentstring = string.Empty;
    }

    /**
     * Receive notification of the start of an element.
     * 
     * @param namespaceURI
     *            The Namespace URI, or the empty string if the element has no
     *            Namespace URI or if Namespace processing is not being
     *            performed
     * @param sName
     *            The local name (without prefix), or the empty string if
     *            Namespace processing is not being performed
     * @param qName
     *            The qualified name (with prefix), or the empty string if
     *            qualified names are not available
     * @param attrs
     *            The attributes attached to the element. If there are no
     *            attributes, it shall be an empty Attributes object
     */
    public abstract void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs);

    /**
     * Receive notification of the end of an element.
     * 
     * @param namespaceURI
     *            The Namespace URI, or the empty string if the element has no
     *            Namespace URI or if Namespace processing is not being
     *            performed
     * @param sName
     *            The local name (without prefix), or the empty string if
     *            Namespace processing is not being performed
     * @param qName
     *            The qualified name (with prefix), or the empty string if
     *            qualified names are not available
     */
    public abstract void endElement(string namespaceURI, string sName, string qName);

    /**
     * Receive notification of character data inside an element.
     * 
     * @param buf
     *            The characters
     * @param offset
     *            The start position in the character array
     * @param len
     *            The number of characters to use from the character array
     */
    public virtual void characters(char[] buf, int offset, int len)
    {
        // Append the new characters
        currentstring += new string(buf, offset, len);
       // Debug.Log(currentstring);
    }
}

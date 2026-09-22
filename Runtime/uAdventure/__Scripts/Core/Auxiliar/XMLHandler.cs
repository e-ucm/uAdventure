using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System;

namespace uAdventure.Core
{
    public abstract class XMLHandler
    {
        //static string LineEnd = "\n";

        protected string value = null;
        /*Stack stack = new Stack();
        string title = null;
        string link = null;
        string desc = null;*/

        // we have to write this method ourselves, since it's
        // not provided by the API
        public virtual void Parse(string url)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(url);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            string namespaceURI = reader.NamespaceURI;
                            string name = reader.Name;
                            Dictionary<String, String> att = new Dictionary<String, String>();
                            if (reader.HasAttributes)
                            {
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    att.Add(reader.Name.ToString(), reader.Value.ToString());
                                }
                            }
                            startElement(namespaceURI, name, name, att);
                            break;
                        case XmlNodeType.EndElement:
                            endElement(reader.NamespaceURI,
                                   reader.Name, reader.Name);
                            break;
                        case XmlNodeType.Text:
                            //Debug.Log("TEXT: " + reader.Value);
                            characters(reader.Value.ToCharArray(), 0, reader.Value.Length);
                            break;
                            // There are many other types of nodes, but
                            // we are not interested in them
                    }
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public abstract void startElement(string namespaceURI, string sName,
            string qName, Dictionary<string, string> attrs);

        public abstract void endElement(string namespaceURI, string sName,
            string qName);

        public virtual void characters(char[] buf, int offset, int len)
        {
            value += new string(buf, offset, len);
        }
    }
}
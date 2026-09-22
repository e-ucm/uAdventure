using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace uAdventure.Editor
{
    public abstract class ParametrizedDOMWriter : IDOMWriter
    {
        public void BuildDOM(XmlNode parent, object target, params IDOMWriterParam[] options)
        {
            var doc = Writer.GetDoc();
            var name = GetElementNameFor(target);

            foreach (var o in options)
            {
                if (o is DOMWriterUtility.DontCreateElementParam)
                {
                    FillNode(parent, target, options);
                    return;
                }
                if(o is DOMWriterUtility.NameParam)
                {
                    name = (o as DOMWriterUtility.NameParam).Name;
                }
            }

            
            var element = doc.CreateElement(name);
            FillNode(element, target, options);
            doc.ImportNode(element, true);
            parent.AppendChild(element);
        }

        protected abstract string GetElementNameFor(object target);
        protected abstract void FillNode(XmlNode node, object target, params IDOMWriterParam[] options);

        protected XmlElement AddNode(XmlNode parent, string name, string content)
        {
            var element = Writer.GetDoc().CreateElement(name);
            element.InnerText = content;
            parent.AppendChild(element);
            return element;
        }
    }
}

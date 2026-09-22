using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Description))]
    public class DescriptionDOMWriter : ParametrizedDOMWriter
    {

        public DescriptionDOMWriter()
        {
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            if (target is Description)
            {
                FillNode(node, target as Description, options);
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "description";
        }

        /**
         * Builds the DOM element for a global state
         * 
         * @param globalState
         * @return
         */

        protected void FillNode(XmlNode node, Description description, params IDOMWriterParam[] options)
        {
            var doc = Writer.GetDoc();
            // Create the description

            // Append the conditions (if available)
            if (description.getConditions() != null && !description.getConditions().IsEmpty())
            {
                DOMWriterUtility.DOMWrite(node, description.getConditions());
            }

            // Create and append the name, brief description and detailed description and its soundPaths
            XmlElement nameNode = doc.CreateElement("name");

            if (description.getNameSoundPath() != null && !description.getNameSoundPath().Equals(""))
            {
                nameNode.SetAttribute("soundPath", description.getNameSoundPath());
            }
            nameNode.AppendChild(doc.CreateTextNode(description.getName()));
            node.AppendChild(nameNode);

            XmlElement briefNode = doc.CreateElement("brief");
            if (description.getDescriptionSoundPath() != null && !description.getDescriptionSoundPath().Equals(""))
            {
                briefNode.SetAttribute("soundPath", description.getDescriptionSoundPath());
            }
            briefNode.AppendChild(doc.CreateTextNode(description.getDescription()));
            node.AppendChild(briefNode);

            XmlElement detailedNode = doc.CreateElement("detailed");
            if (description.getDetailedDescriptionSoundPath() != null &&
                !description.getDetailedDescriptionSoundPath().Equals(""))
            {
                detailedNode.SetAttribute("soundPath", description.getDetailedDescriptionSoundPath());
            }
            detailedNode.AppendChild(doc.CreateTextNode(description.getDetailedDescription()));
            node.AppendChild(detailedNode);
        }
    }
}
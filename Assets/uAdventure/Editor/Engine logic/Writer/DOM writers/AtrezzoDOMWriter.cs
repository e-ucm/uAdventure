using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Atrezzo))]
    public class AtrezzoDOMWriter : ParametrizedDOMWriter
    {
        /**
         * Private constructor.
         */

        public AtrezzoDOMWriter()
        {

        }
        

        protected override string GetElementNameFor(object target)
        {
            return "atrezzoobject";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var atrezzo = target as Atrezzo;
            XmlElement atrezzoElement = node as XmlElement;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            atrezzoElement.SetAttribute("id", atrezzo.getId());

            // Append the documentation (if avalaible)
            if (atrezzo.getDocumentation() != null)
            {
                XmlNode atrezzoDocumentationNode = doc.CreateElement("documentation");
                atrezzoDocumentationNode.AppendChild(doc.CreateTextNode(atrezzo.getDocumentation()));
                atrezzoElement.AppendChild(atrezzoDocumentationNode);
            }

            // Append the resources
            foreach (ResourcesUni resources in atrezzo.getResources())
            {
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_ITEM);
                doc.ImportNode(resourcesNode, true);
                atrezzoElement.AppendChild(resourcesNode);
            }

            // atrezzo only have name
            // Create the description
            XmlNode descriptionNode = doc.CreateElement("description");

            // Create and append the name, brief description and detailed description and its soundPaths
            XmlElement nameNode = doc.CreateElement("name");
            if (atrezzo.getDescription(0).getNameSoundPath() != null &&
                !atrezzo.getDescription(0).getNameSoundPath().Equals(""))
            {
                nameNode.SetAttribute("soundPath", atrezzo.getDescription(0).getNameSoundPath());
            }
            nameNode.AppendChild(doc.CreateTextNode(atrezzo.getDescription(0).getName()));
            descriptionNode.AppendChild(nameNode);

            XmlElement briefNode = doc.CreateElement("brief");
            /* if (description.getDescriptionSoundPath( )!=null && !description.getDescriptionSoundPath( ).Equals( "" )){
                     briefNode.SetAttribute( "soundPath", description.getDescriptionSoundPath( ) );
                 }
                 briefNode.AppendChild( doc.CreateTextNode( description.getDescription( ) ) );*/
            briefNode.AppendChild(doc.CreateTextNode(""));
            descriptionNode.AppendChild(briefNode);

            XmlElement detailedNode = doc.CreateElement("detailed");
            /* if (description.getDetailedDescriptionSoundPath( )!=null && !description.getDetailedDescriptionSoundPath( ).Equals( "" )){
                     detailedNode.SetAttribute( "soundPath", description.getDetailedDescriptionSoundPath( ) );
                 }
                 detailedNode.AppendChild( doc.CreateTextNode( description.getDetailedDescription( ) ) );*/
            detailedNode.AppendChild(doc.CreateTextNode(""));
            descriptionNode.AppendChild(detailedNode);

            // Append the description
            atrezzoElement.AppendChild(descriptionNode);
        }
    }
}
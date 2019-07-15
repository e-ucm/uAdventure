using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Item))]
    public class ItemDOMWriter : ParametrizedDOMWriter
    {
        /**
        * Private constructor.
        */

        public ItemDOMWriter()
        {

        }

        protected override string GetElementNameFor(object target)
        {
            return "object";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var item = target as Item;

            XmlElement itemElement = node as XmlElement;


            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();
            // Create the root node
            itemElement.SetAttribute("id", item.getId());
            itemElement.SetAttribute("returnsWhenDragged", (item.isReturnsWhenDragged() ? "yes" : "no"));

            //v1.4
            if (item.getBehaviour() == Item.BehaviourType.NORMAL)
                itemElement.SetAttribute("behaviour", "normal");
            if (item.getBehaviour() == Item.BehaviourType.ATREZZO)
                itemElement.SetAttribute("behaviour", "atrezzo");
            if (item.getBehaviour() == Item.BehaviourType.FIRST_ACTION)
                itemElement.SetAttribute("behaviour", "first-action");
            itemElement.SetAttribute("resources-transition-time", item.getResourcesTransitionTime().ToString());
            //v1.4

            // Append the documentation (if avalaible)
            if (item.getDocumentation() != null)
            {
                XmlNode itemDocumentationNode = doc.CreateElement("documentation");
                itemDocumentationNode.AppendChild(doc.CreateTextNode(item.getDocumentation()));
                itemElement.AppendChild(itemDocumentationNode);
            }

            // Append the resources
            foreach (ResourcesUni resources in item.getResources())
            {
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_ITEM);
                doc.ImportNode(resourcesNode, true);
                itemElement.AppendChild(resourcesNode);
            }

            DOMWriterUtility.DOMWrite(itemElement, item.getDescriptions());

            // Append the actions (if there is at least one)
            if (item.getActions().Count > 0)
            {
                // Create the actions node
                DOMWriterUtility.DOMWrite(itemElement, item.getActions());
            }
            
            
        }
    }
}
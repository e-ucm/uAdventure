using UnityEngine;
using System.Collections;
using System.Xml;

public class ItemDOMWriter
{
    /**
        * Private constructor.
        */

    private ItemDOMWriter()
    {

    }

    public static XmlNode buildDOM(Item item)
    {

        XmlElement itemElement = null;


        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();
        // Create the root node
        itemElement = doc.CreateElement("object");
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

        foreach (Description description in item.getDescriptions())
        {

            // Create the description
            XmlNode descriptionNode = doc.CreateElement("description");

            // Append the conditions (if available)
            if (description.getConditions() != null && !description.getConditions().isEmpty())
            {
                XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(description.getConditions());
                doc.ImportNode(conditionsNode, true);
                descriptionNode.AppendChild(conditionsNode);
            }

            // Create and append the name, brief description and detailed description and its soundPaths
            XmlElement nameNode = doc.CreateElement("name");

            if (description.getNameSoundPath() != null && !description.getNameSoundPath().Equals(""))
            {
                nameNode.SetAttribute("soundPath", description.getNameSoundPath());
            }
            nameNode.AppendChild(doc.CreateTextNode(description.getName()));
            descriptionNode.AppendChild(nameNode);

            XmlElement briefNode = doc.CreateElement("brief");
            if (description.getDescriptionSoundPath() != null && !description.getDescriptionSoundPath().Equals(""))
            {
                briefNode.SetAttribute("soundPath", description.getDescriptionSoundPath());
            }
            briefNode.AppendChild(doc.CreateTextNode(description.getDescription()));
            descriptionNode.AppendChild(briefNode);

            XmlElement detailedNode = doc.CreateElement("detailed");
            if (description.getDetailedDescriptionSoundPath() != null &&
                !description.getDetailedDescriptionSoundPath().Equals(""))
            {
                detailedNode.SetAttribute("soundPath", description.getDetailedDescriptionSoundPath());
            }
            detailedNode.AppendChild(doc.CreateTextNode(description.getDetailedDescription()));
            descriptionNode.AppendChild(detailedNode);

            // Append the description
            itemElement.AppendChild(descriptionNode);

        }

        // Append the actions (if there is at least one)
        if (item.getActions().Count > 0)
        {
            // Create the actions node
            XmlNode actionsNode = ActionsDOMWriter.buildDOM(item.getActions());
            doc.ImportNode(actionsNode, true);
            // Append the actions node
            itemElement.AppendChild(actionsNode);
        }



        return itemElement;
    }
}
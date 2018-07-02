using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Player))]
    public class PlayerDOMWriter : ParametrizedDOMWriter
    {

        public PlayerDOMWriter()
        {

        }
        

        protected override string GetElementNameFor(object target)
        {
            return "player";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var player = target as Player;

            XmlNode playerNode = node;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();
            

            // Append the documentation (if avalaible)
            if (player.getDocumentation() != null)
            {
                XmlNode playerDocumentationNode = doc.CreateElement("documentation");
                playerDocumentationNode.AppendChild(doc.CreateTextNode(player.getDocumentation()));
                playerNode.AppendChild(playerDocumentationNode);
            }

            // Append the resources
            foreach (ResourcesUni resources in player.getResources())
            {
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CHARACTER);
                doc.ImportNode(resourcesNode, true);
                playerNode.AppendChild(resourcesNode);
            }

            // Create the textcolor
            XmlElement textColorNode = doc.CreateElement("textcolor");
            textColorNode.SetAttribute("showsSpeechBubble", (player.getShowsSpeechBubbles() ? "yes" : "no"));
            textColorNode.SetAttribute("bubbleBkgColor", ColorConverter.ColorToHex(player.getBubbleBkgColor()));
            textColorNode.SetAttribute("bubbleBorderColor", ColorConverter.ColorToHex(player.getBubbleBorderColor()));

            // Create and append the frontcolor
            XmlElement frontColorElement = doc.CreateElement("frontcolor");
            frontColorElement.SetAttribute("color", ColorConverter.ColorToHex(player.getTextFrontColor()));
            textColorNode.AppendChild(frontColorElement);

            // Create and append the bordercolor
            XmlElement borderColoElement = doc.CreateElement("bordercolor");
            borderColoElement.SetAttribute("color", ColorConverter.ColorToHex(player.getTextBorderColor()));
            textColorNode.AppendChild(borderColoElement);

            // Append the textcolor
            playerNode.AppendChild(textColorNode);

            foreach (Description description in player.getDescriptions())
            {

                // Create the description
                XmlNode descriptionNode = doc.CreateElement("description");

                // Append the conditions (if available)
                if (description.getConditions() != null && !description.getConditions().IsEmpty())
                {
                    DOMWriterUtility.DOMWrite(descriptionNode, description.getConditions());
                }

                // Create and append the name, brief description and detailed description
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
                playerNode.AppendChild(descriptionNode);

            }

            // Create the voice tag
            XmlElement voiceNode = doc.CreateElement("voice");
            // Create and append the voice name and if is alwaysSynthesizer
            voiceNode.SetAttribute("name", player.getVoice());
            if (player.isAlwaysSynthesizer())
                voiceNode.SetAttribute("synthesizeAlways", "yes");
            else
                voiceNode.SetAttribute("synthesizeAlways", "no");

            // Append the voice tag

            playerNode.AppendChild(voiceNode);
        }
    }
}
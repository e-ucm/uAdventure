using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(NPC))]
    public class CharacterDOMWriter : ParametrizedDOMWriter
    {

        public CharacterDOMWriter()
        {

        }

        protected override string GetElementNameFor(object target)
        {
            return "character";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var character = target as NPC;

            XmlElement characterElement = node as XmlElement;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Add root node attributes
            characterElement.SetAttribute("id", character.getId());

            // Append the documentation (if avalaible)
            if (character.getDocumentation() != null)
            {
                XmlNode characterDocumentationNode = doc.CreateElement("documentation");
                characterDocumentationNode.AppendChild(doc.CreateTextNode(character.getDocumentation()));
                characterElement.AppendChild(characterDocumentationNode);
            }

            // Append the resources
            foreach (ResourcesUni resources in character.getResources())
            {
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CHARACTER);
                doc.ImportNode(resourcesNode, true);
                characterElement.AppendChild(resourcesNode);
            }

            // Create the textcolor
            XmlElement textColorNode = doc.CreateElement("textcolor");
            textColorNode.SetAttribute("showsSpeechBubble", (character.getShowsSpeechBubbles() ? "yes" : "no"));
            textColorNode.SetAttribute("bubbleBkgColor", ColorConverter.ColorToHex(character.getBubbleBkgColor()));
            textColorNode.SetAttribute("bubbleBorderColor", ColorConverter.ColorToHex(character.getBubbleBorderColor()));

            // Create and append the frontcolor
            XmlElement frontColorElement = doc.CreateElement("frontcolor");
            frontColorElement.SetAttribute("color", ColorConverter.ColorToHex(character.getTextFrontColor()));
            textColorNode.AppendChild(frontColorElement);

            // Create and append the bordercolor
            XmlElement borderColoElement = doc.CreateElement("bordercolor");
            borderColoElement.SetAttribute("color", ColorConverter.ColorToHex(character.getTextBorderColor()));
            textColorNode.AppendChild(borderColoElement);

            // Append the textcolor
            characterElement.AppendChild(textColorNode);
            
            //v1.4
            if (character.getBehaviour() == Item.BehaviourType.NORMAL)
                characterElement.SetAttribute("behaviour", "normal");
            if (character.getBehaviour() == Item.BehaviourType.ATREZZO)
                characterElement.SetAttribute("behaviour", "atrezzo");
            if (character.getBehaviour() == Item.BehaviourType.FIRST_ACTION)
                characterElement.SetAttribute("behaviour", "first-action");
            //v1.4


            foreach (Description description in character.getDescriptions())
            {
                // Create the description
                XmlNode descriptionNode = doc.CreateElement("description");
                // Append the conditions (if available)
                if (description.getConditions() != null && !description.getConditions().IsEmpty())
                {
                    DOMWriterUtility.DOMWrite(descriptionNode, description.getConditions());
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
                characterElement.AppendChild(descriptionNode);

            }

            // Create the voice tag
            XmlElement voiceNode = doc.CreateElement("voice");
            // Create and append the voice name and if is alwaysSynthesizer
            voiceNode.SetAttribute("name", character.getVoice());
            if (character.isAlwaysSynthesizer())
                voiceNode.SetAttribute("synthesizeAlways", "yes");
            else
                voiceNode.SetAttribute("synthesizeAlways", "no");

            // Append the voice tag

            characterElement.AppendChild(voiceNode);
            if (character.getActionsCount() > 0)
            {
                DOMWriterUtility.DOMWrite(characterElement, character.getActions());
            }
        }
    }
}
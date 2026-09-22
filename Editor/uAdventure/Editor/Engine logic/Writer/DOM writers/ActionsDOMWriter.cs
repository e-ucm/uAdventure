using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(List<Action>))]
    public class ActionsDOMWriter : IDOMWriter
    {

        /**
         * Private constructor.
         */

        public ActionsDOMWriter()
        {

        }

        /**
         * Build a node from a list of actions
         * 
         * @param actions
         *            the list of actions
         * @return the xml node with the list of actions
         */

        public void BuildDOM(XmlNode parent, object target, params IDOMWriterParam[] options)
        {
            var actions = target as List<Action>;
            XmlElement actionsElement = null;


            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            actionsElement = doc.CreateElement("actions");

            // Append the actions (if there is at least one)
            if (actions.Count > 0)
            {
                // For every action
                foreach (Action action in actions)
                {
                    XmlElement actionElement = null;

                    // Create the element
                    switch (action.getType())
                    {
                        case Action.EXAMINE:
                            actionElement = doc.CreateElement("examine");
                            break;
                        case Action.GRAB:
                            actionElement = doc.CreateElement("grab");
                            break;
                        case Action.USE:
                            actionElement = doc.CreateElement("use");
                            break;
                        case Action.TALK_TO:
                            actionElement = doc.CreateElement("talk-to");
                            break;
                        case Action.USE_WITH:
                            actionElement = doc.CreateElement("use-with");
                            actionElement.SetAttribute("idTarget", action.getTargetId());
                            break;
                        case Action.GIVE_TO:
                            actionElement = doc.CreateElement("give-to");
                            actionElement.SetAttribute("idTarget", action.getTargetId());
                            break;
                        case Action.DRAG_TO:
                            actionElement = doc.CreateElement("drag-to");
                            actionElement.SetAttribute("idTarget", action.getTargetId());
                            break;
                        case Action.CUSTOM:
                            actionElement = doc.CreateElement("custom");
                            actionElement.SetAttribute("name", ((CustomAction)action).getName());
                            foreach (ResourcesUni resources in ((CustomAction)action).getResources())
                            {
                                XmlNode resourcesXmlNode = ResourcesDOMWriter.buildDOM(resources,
                                    ResourcesDOMWriter.RESOURCES_CUSTOM_ACTION);
                                //doc.adoptXmlNode(resourcesXmlNode);
                                doc.ImportNode(resourcesXmlNode, true);
                                actionElement.AppendChild(resourcesXmlNode);
                            }
                            break;
                        case Action.CUSTOM_INTERACT:
                            actionElement = doc.CreateElement("custom-interact");
                            actionElement.SetAttribute("idTarget", action.getTargetId());
                            actionElement.SetAttribute("name", ((CustomAction)action).getName());
                            foreach (ResourcesUni resources in ((CustomAction)action).getResources())
                            {
                                XmlNode resourcesXmlNode = ResourcesDOMWriter.buildDOM(resources,
                                    ResourcesDOMWriter.RESOURCES_CUSTOM_ACTION);
                                doc.ImportNode(resourcesXmlNode, true);
                                actionElement.AppendChild(resourcesXmlNode);
                            }
                            break;
                    }

                    actionElement.SetAttribute("needsGoTo", (action.isNeedsGoTo() ? "yes" : "no"));
                    actionElement.SetAttribute("keepDistance", "" + action.getKeepDistance());
                    actionElement.SetAttribute("not-effects", action.isActivatedNotEffects() ? "yes" : "no");

                    // Append the documentation (if avalaible)
                    if (action.getDocumentation() != null)
                    {
                        XmlNode actionDocumentationXmlNode = doc.CreateElement("documentation");
                        actionDocumentationXmlNode.AppendChild(doc.CreateTextNode(action.getDocumentation()));
                        actionElement.AppendChild(actionDocumentationXmlNode);
                    }

                    // Append the conditions (if avalaible)
                    if (!action.getConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(actionElement, action.getConditions());
                    }

                    // Append the effects (if avalaible)
                    if (!action.getEffects().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(actionElement, action.getEffects());
                    }
                    // Append the not effects (if avalaible)
                    if (action.getNotEffects() != null && !action.getNotEffects().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(actionElement, action.getNotEffects(), DOMWriterUtility.Name(EffectsDOMWriter.NOT_EFFECTS));
                    }

                    // Append the action element
                    actionsElement.AppendChild(actionElement);
                }
            }

            parent.AppendChild(actionsElement);
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ActionsDOMWriter
{

    /**
     * Private constructor.
     */

    private ActionsDOMWriter()
    {

    }

    /**
     * Build a node from a list of actions
     * 
     * @param actions
     *            the list of actions
     * @return the xml node with the list of actions
     */

    public static XmlNode buildDOM(List<Action> actions)
    {

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
                        actionElement.SetAttribute("name", ((CustomAction) action).getName());
                        foreach (ResourcesUni resources in ((CustomAction) action).getResources())
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
                        actionElement.SetAttribute("name", ((CustomAction) action).getName());
                        foreach (ResourcesUni resources in ((CustomAction) action).getResources())
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
                if (!action.getConditions().isEmpty())
                {
                    XmlNode conditionsXmlNode = ConditionsDOMWriter.buildDOM(action.getConditions());
                    doc.ImportNode(conditionsXmlNode, true);
                    actionElement.AppendChild(conditionsXmlNode);
                }

                // Append the effects (if avalaible)
                if (!action.getEffects().isEmpty())
                {
                    XmlNode effectsXmlNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, action.getEffects());
                    doc.ImportNode(effectsXmlNode, true);
                    actionElement.AppendChild(effectsXmlNode);
                }
                // Append the not effects (if avalaible)
                if (action.getNotEffects() != null && !action.getNotEffects().isEmpty())
                {
                    XmlNode notEffectsXmlNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.NOT_EFFECTS,
                        action.getNotEffects());
                    doc.ImportNode(notEffectsXmlNode, true);
                    actionElement.AppendChild(notEffectsXmlNode);
                }

                // Append the action element
                actionsElement.AppendChild(actionElement);
            }
        }

        return actionsElement;
    }
}
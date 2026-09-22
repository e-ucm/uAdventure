using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AdaptationDOMWriter
    {
        /**
         * Private constructor.
         */

        private AdaptationDOMWriter()
        {

        }




        /**
         * Returns the DOM element for the chapter
         * 
         * @param chapter
         *            Chapter data to be written
         * @return DOM element with the chapter data
         */

        public static XmlElement buildDOM(List<AdaptationRule> rules, AdaptedState initialState, bool scorm12,
            bool scorm2004, String name, XmlDocument doc)
        {

            XmlElement adaptationNode = null;

            // Create the root node
            adaptationNode = doc.CreateElement("adaptation");

            if (scorm12)
            {
                adaptationNode.SetAttribute("scorm12", "yes");
            }
            else
            {
                adaptationNode.SetAttribute("scorm12", "no");
            }
            if (scorm2004)
            {
                adaptationNode.SetAttribute("scorm2004", "yes");
            }
            else
            {
                adaptationNode.SetAttribute("scorm2004", "no");
            }
            adaptationNode.SetAttribute("name", name);

            // Append the initial state, when available
            if (initialState != null && !initialState.isEmpty())
            {
                XmlNode initialStateNode = doc.CreateElement("initial-state");

                // Append initial scene if available
                if (initialState.getTargetId() != null && !initialState.getTargetId().Equals(""))
                {
                    XmlElement initialScene = doc.CreateElement("initial-scene");
                    initialScene.SetAttribute("idTarget", initialState.getTargetId());
                    initialStateNode.AppendChild(initialScene);
                }

                // Append activate / deactivate flags or set value var
                XmlElement actionFlag;
                for (int i = 0; i < initialState.getFlagsVars().Count; i++)
                {
                    // check the operation's type
                    actionFlag = null;
                    if (initialState.isFlag(i))
                    {
                        if (AdaptedState.isActivateOp(initialState.getAction(i)))
                            actionFlag = doc.CreateElement("activate");
                        if (AdaptedState.isDeactivateOp(initialState.getAction(i)))
                            actionFlag = doc.CreateElement("deactivate");

                        actionFlag.SetAttribute("flag", initialState.getFlagVar(i));
                    }
                    else
                    {
                        if (AdaptedState.isSetValueOp(initialState.getAction(i)))
                            // get only the title of the operation
                            actionFlag = doc.CreateElement("set-value");
                        else if (AdaptedState.isIncrementOp(initialState.getAction(i)))
                            actionFlag = doc.CreateElement("increment");
                        else if (AdaptedState.isDecrementOp(initialState.getAction(i)))
                            actionFlag = doc.CreateElement("decrement");

                        //set the name of the current var
                        actionFlag.SetAttribute("var", initialState.getFlagVar(i));
                        // set the value
                        actionFlag.SetAttribute("value", initialState.getValueForVar(i));
                    }
                    initialStateNode.AppendChild(actionFlag);
                }

                //Append the node
                adaptationNode.AppendChild(initialStateNode);
            }

            // Append the adaptation rules
            foreach (AdaptationRule rule in rules)
            {

                //Create the rule node and set attributes
                XmlNode ruleNode = doc.CreateElement("adaptation-rule");

                //Append rule description
                XmlNode descriptionNode = doc.CreateElement("ruleDescription");
                if (rule.getDescription() != null)
                    descriptionNode.AppendChild(doc.CreateTextNode(rule.getDescription()));
                else
                    descriptionNode.AppendChild(doc.CreateTextNode(""));
                ruleNode.AppendChild(descriptionNode);

                //Append uol-state
                XmlNode uolStateNode = doc.CreateElement("uol-state");
                foreach (UOLProperty property in rule.getUOLProperties())
                {
                    XmlElement propertyElement = doc.CreateElement("property");
                    propertyElement.SetAttribute("id", property.getId());
                    propertyElement.SetAttribute("value", property.getValue());
                    uolStateNode.AppendChild(propertyElement);
                    propertyElement.SetAttribute("operation", property.getOperation());
                }
                ruleNode.AppendChild(uolStateNode);

                //Append game-state
                // Append the initial state, when available
                XmlNode gameStateNode = doc.CreateElement("game-state");
                if (rule.getAdaptedState() != null && !rule.getAdaptedState().isEmpty())
                {

                    // Append initial scene if available
                    if (rule.getAdaptedState().getTargetId() != null && !rule.getAdaptedState().getTargetId().Equals(""))
                    {
                        XmlElement initialScene = doc.CreateElement("initial-scene");
                        initialScene.SetAttribute("idTarget", rule.getAdaptedState().getTargetId());
                        gameStateNode.AppendChild(initialScene);
                    }

                    // Append activate / deactivate flags or set value var
                    XmlElement actionFlag = null;
                    for (int i = 0; i < rule.getAdaptedState().getFlagsVars().Count; i++)
                    {
                        if (rule.getAdaptedState().isFlag(i))
                        {
                            if (AdaptedState.isActivateOp(rule.getAdaptedState().getAction(i)))
                                actionFlag = doc.CreateElement("activate");
                            if (AdaptedState.isDeactivateOp(rule.getAdaptedState().getAction(i)))
                                actionFlag = doc.CreateElement("deactivate");
                            actionFlag.SetAttribute("flag", rule.getAdaptedState().getFlagVar(i));

                        }
                        else
                        {
                            // check if this operation is "set-value"
                            if (AdaptedState.isSetValueOp(rule.getAdaptedState().getAction(i)))
                                // get only the title of the operation
                                actionFlag = doc.CreateElement("set-value");
                            // check if this operation is "increment"
                            else if (AdaptedState.isIncrementOp(rule.getAdaptedState().getAction(i)))
                                // get only the title of the operation
                                actionFlag = doc.CreateElement("increment");
                            // check if this operation is "decrement"
                            else if (AdaptedState.isDecrementOp(rule.getAdaptedState().getAction(i)))
                                // get only the title of the operation
                                actionFlag = doc.CreateElement("decrement");

                            //set the name of the current var
                            actionFlag.SetAttribute("var", rule.getAdaptedState().getFlagVar(i));
                            // store the future value
                            actionFlag.SetAttribute("value", rule.getAdaptedState().getValueForVar(i));

                        }
                        gameStateNode.AppendChild(actionFlag);
                    }

                }
                //Append the node
                ruleNode.AppendChild(gameStateNode);

                //Append the rule
                adaptationNode.AppendChild(ruleNode);

            }

            return adaptationNode;
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Conditions), typeof(GlobalState))]
    public class ConditionsDOMWriter : ParametrizedDOMWriter
    {
        /**
         * Constant for "condition" tag (general case)
         */
        public const string CONDITIONS = "condition";

        /**
         * Constant for "init-condition" tag (timers)
         */
        public const string INIT_CONDITIONS = "init-condition";

        /**
         * Constant for "end-condition" tag (timers)
         */
        public const string END_CONDITIONS = "end-condition";

        /**
         * Constant for "global-state" tag
         */
        public const string GLOBAL_STATE = "global-state";
        
        public ConditionsDOMWriter()
        {
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            if (target is GlobalState)
            {
                FillNode(node, target as GlobalState, options);
            }
            else if (target is Conditions)
            {
                FillNode(node, target as Conditions, options);
            }
        }

        protected override string GetElementNameFor(object target)
        {
            string name = "";
            if (target is GlobalState)
            {
                name = GLOBAL_STATE;
            }
            else if (target is Conditions)
            {
                name = CONDITIONS;
            }
            return name;
        }

        /**
         * Builds the DOM element for a global state
         * 
         * @param globalState
         * @return
         */

        protected void FillNode(XmlNode node, GlobalState globalState, params IDOMWriterParam[] options)
        {
            XmlElement conditionsNode = node as XmlElement;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();
            conditionsNode.SetAttribute("id", globalState.getId());
            XmlNode documentationNode = doc.CreateElement("documentation");
            documentationNode.AppendChild(doc.CreateTextNode(globalState.getDocumentation()));
            conditionsNode.AppendChild(documentationNode);

            // Iterate all the condition'blocks
            for (int i = 0; i < globalState.Size(); i++)
            {
                var block = globalState.Get(i);
                // Single condition
                if (block.Count == 1)
                {
                    XmlElement conditionElement = createConditionElement(doc, block[0]);
                    doc.ImportNode(conditionElement, true);
                    conditionsNode.AppendChild(conditionElement);
                }
                else if (block.Count > 1)
                {
                    XmlNode eitherNode = createElementWithList("either", block);
                    doc.ImportNode(eitherNode, true);
                    conditionsNode.AppendChild(eitherNode);
                }
            }
        }

        protected void FillNode(XmlNode node, Conditions conditions, params IDOMWriterParam[] options)
        {
            XmlNode conditionsNode = node;

            // Create the necessary elements to create the DOM
            var doc = Writer.GetDoc();

            // Iterate all the condition'blocks
            for (int i = 0; i < conditions.Size(); i++)
            {
                IList<Condition> block = conditions.Get(i);
                // Single condition
                if (block.Count == 1)
                {
                    XmlElement conditionElement = createConditionElement(doc, block[0]);
                    doc.ImportNode(conditionElement, true);
                    conditionsNode.AppendChild(conditionElement);

                }
                else if (block.Count > 1)
                {
                    XmlNode eitherNode = createElementWithList("either", block);
                    doc.ImportNode(eitherNode, true);
                    conditionsNode.AppendChild(eitherNode);
                }
            }
        }

        private static XmlNode createElementWithList(String tagname, IList<Condition> conditions)
        {
            XmlElement conditionsListNode = null;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            conditionsListNode = doc.CreateElement(tagname);

            createElementWithList(doc, conditionsListNode, conditions);
            return conditionsListNode;
        }

        private static XmlElement createConditionElement(XmlDocument doc, Condition condition)
        {

            XmlElement conditionElement = null;

            if (condition.getType() == Condition.FLAG_CONDITION)
            {
                // Create the tag
                if (condition.getState() == FlagCondition.FLAG_ACTIVE)
                {
                    conditionElement = doc.CreateElement("active");
                }
                else if (condition.getState() == FlagCondition.FLAG_INACTIVE)
                {
                    conditionElement = doc.CreateElement("inactive");
                }

                // Set the target flag and append it
                conditionElement.SetAttribute("flag", condition.getId());
            }
            else if (condition.getType() == Condition.VAR_CONDITION)
            {
                VarCondition varCondition = (VarCondition)condition;
                // Create the tag
                switch (varCondition.getState())
                {
                    default: // VAR_EQUALS
                        conditionElement = doc.CreateElement("equals");
                        break;
                    case VarCondition.VAR_NOT_EQUALS:
                        conditionElement = doc.CreateElement("not-equals");
                        break;
                    case VarCondition.VAR_GREATER_EQUALS_THAN:
                        conditionElement = doc.CreateElement("greater-equals-than");
                        break;
                    case VarCondition.VAR_GREATER_THAN:
                        conditionElement = doc.CreateElement("greater-than");
                        break;
                    case VarCondition.VAR_LESS_EQUALS_THAN:
                        conditionElement = doc.CreateElement("less-equals-than");
                        break;
                    case VarCondition.VAR_LESS_THAN:
                        conditionElement = doc.CreateElement("less-than");
                        break;
                }

                // Set the target flag and append it
                conditionElement.SetAttribute("var", varCondition.getId());
                conditionElement.SetAttribute("value", varCondition.getValue().ToString());
            }
            else if (condition.getType() == Condition.GLOBAL_STATE_CONDITION)
            {
                GlobalStateCondition globalStateCondition = (GlobalStateCondition)condition;
                // Create the tag
                conditionElement = doc.CreateElement("global-state-ref");

                // Set the target flag and append it
                conditionElement.SetAttribute("id", globalStateCondition.getId());

                conditionElement.SetAttribute("value",
                    globalStateCondition.getState() == GlobalStateCondition.GS_SATISFIED ? "true" : "false");
            }

            return conditionElement;
        }

        private static void createElementWithList(XmlDocument doc, XmlElement conditionsListNode, IList<Condition> conditions)
        {
            // Write every condition
            foreach (Condition condition in conditions)
            {
                conditionsListNode.AppendChild(createConditionElement(doc, condition));
            }
        }


    }
}
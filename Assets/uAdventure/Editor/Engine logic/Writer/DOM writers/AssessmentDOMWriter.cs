using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AssessmentDOMWriter
    {

        /**
         * Private constructor.
         */

        private AssessmentDOMWriter()
        {

        }

        /**
         * Returns the DOM element for the chapter
         * 
         * @param chapter
         *            Chapter data to be written
         * @return DOM element with the chapter data
         */

        public static XmlElement buildDOM(AssessmentProfile profile, XmlDocument doc)
        {

            List<AssessmentRule> rules = profile.getRules();

            XmlElement assessmentNode = null;

            // Create the root node
            assessmentNode = doc.CreateElement("assessment");
            if (profile.isShowReportAtEnd())
            {
                assessmentNode.SetAttribute("show-report-at-end", "yes");
            }
            else
            {
                assessmentNode.SetAttribute("show-report-at-end", "no");
            }
            if (!profile.isShowReportAtEnd() || !profile.isSendByEmail())
                assessmentNode.SetAttribute("send-to-email", "");
            else
            {
                if (profile.getEmail() == null || !profile.getEmail().Contains("@"))
                    assessmentNode.SetAttribute("send-to-email", "");
                else
                {
                    assessmentNode.SetAttribute("send-to-email", profile.getEmail());
                }
            }
            // add scorm attributes 
            if (profile.isScorm12())
            {
                assessmentNode.SetAttribute("scorm12", "yes");
            }
            else
            {
                assessmentNode.SetAttribute("scorm12", "no");
            }
            if (profile.isScorm2004())
            {
                assessmentNode.SetAttribute("scorm2004", "yes");
            }
            else
            {
                assessmentNode.SetAttribute("scorm2004", "no");
            }
            //add the profile's name
            assessmentNode.SetAttribute("name", profile.getName());

            XmlElement smtpConfigNode = doc.CreateElement("smtp-config");
            smtpConfigNode.SetAttribute("smtp-ssl", (profile.isSmtpSSL() ? "yes" : "no"));
            smtpConfigNode.SetAttribute("smtp-server", profile.getSmtpServer());
            smtpConfigNode.SetAttribute("smtp-port", profile.getSmtpPort());
            smtpConfigNode.SetAttribute("smtp-user", profile.getSmtpUser());
            smtpConfigNode.SetAttribute("smtp-pwd", profile.getSmtpPwd());

            assessmentNode.AppendChild(smtpConfigNode);

            // Append the assessment rules
            foreach (AssessmentRule rule in rules)
            {

                if (rule is TimedAssessmentRule)
                {
                    TimedAssessmentRule tRule = (TimedAssessmentRule)rule;
                    //Create the rule node and set attributes
                    XmlElement ruleNode = doc.CreateElement("timed-assessment-rule");
                    ruleNode.SetAttribute("id", tRule.getId());
                    ruleNode.SetAttribute("importance", AssessmentRule.IMPORTANCE_VALUES[tRule.getImportance()]);
                    ruleNode.SetAttribute("usesEndConditions", (tRule.isUsesEndConditions() ? "yes" : "no"));
                    ruleNode.SetAttribute("repeatRule", (tRule.isRepeatRule() ? "yes" : "no"));

                    //Append concept
                    if (tRule.getConcept() != null && !tRule.getConcept().Equals(""))
                    {
                        XmlNode conceptNode = doc.CreateElement("concept");
                        conceptNode.AppendChild(doc.CreateTextNode(tRule.getConcept()));
                        ruleNode.AppendChild(conceptNode);
                    }

                    //Append conditions (always required at least one)
                    if (!tRule.getInitConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(ruleNode, tRule.getInitConditions(), DOMWriterUtility.Name(ConditionsDOMWriter.INIT_CONDITIONS));
                    }

                    //Append conditions (always required at least one)
                    if (!tRule.getEndConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(ruleNode, tRule.getEndConditions(), DOMWriterUtility.Name(ConditionsDOMWriter.END_CONDITIONS));
                    }

                    // Create effects
                    for (int i = 0; i < tRule.getEffectsCount(); i++)
                    {
                        //Create effect element and append it
                        XmlElement effectNode = doc.CreateElement("assessEffect");

                        // Append time attributes
                        effectNode.SetAttribute("time-min", tRule.getMinTime(i).ToString());
                        effectNode.SetAttribute("time-max", tRule.getMaxTime(i).ToString());

                        //Append set-text when appropriate
                        TimedAssessmentEffect currentEffect = tRule.getEffects()[i];
                        if (currentEffect.getText() != null && !currentEffect.getText().Equals(""))
                        {
                            XmlNode textNode = doc.CreateElement("set-text");
                            textNode.AppendChild(doc.CreateTextNode(currentEffect.getText()));
                            effectNode.AppendChild(textNode);
                        }
                        //Append properties
                        foreach (AssessmentProperty property in currentEffect.getAssessmentProperties())
                        {
                            XmlElement propertyElement = doc.CreateElement("set-property");
                            propertyElement.SetAttribute("id", property.getId());
                            propertyElement.SetAttribute("value", property.getValue().ToString());
                            effectNode.AppendChild(propertyElement);
                        }
                        //Append the effect
                        ruleNode.AppendChild(effectNode);
                    }

                    //Append the rule
                    assessmentNode.AppendChild(ruleNode);
                }
                else
                {
                    //Create the rule node and set attributes
                    XmlElement ruleNode = doc.CreateElement("assessment-rule");
                    ruleNode.SetAttribute("id", rule.getId());
                    ruleNode.SetAttribute("importance", AssessmentRule.IMPORTANCE_VALUES[rule.getImportance()]);
                    ruleNode.SetAttribute("repeatRule", (rule.isRepeatRule() ? "yes" : "no"));

                    //Append concept
                    if (rule.getConcept() != null && !rule.getConcept().Equals(""))
                    {
                        XmlNode conceptNode = doc.CreateElement("concept");
                        conceptNode.AppendChild(doc.CreateTextNode(rule.getConcept()));
                        ruleNode.AppendChild(conceptNode);
                    }

                    //Append conditions (always required at least one)
                    if (!rule.getConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(ruleNode, rule.getConditions());
                    }

                    //Create effect element and append it
                    XmlNode effectNode = doc.CreateElement("assessEffect");
                    //Append set-text when appropriate
                    if (rule.getText() != null && !rule.getText().Equals(""))
                    {
                        XmlNode textNode = doc.CreateElement("set-text");
                        textNode.AppendChild(doc.CreateTextNode(rule.getText()));
                        effectNode.AppendChild(textNode);
                    }
                    //Append properties
                    foreach (AssessmentProperty property in rule.getAssessmentProperties())
                    {
                        XmlElement propertyElement = doc.CreateElement("set-property");
                        propertyElement.SetAttribute("id", property.getId());
                        propertyElement.SetAttribute("value", property.getValue());
                        if (property.getVarName() != null)
                            propertyElement.SetAttribute("varName", property.getVarName());
                        effectNode.AppendChild(propertyElement);
                    }
                    //Append the effect
                    ruleNode.AppendChild(effectNode);

                    //Append the rule
                    assessmentNode.AppendChild(ruleNode);
                }

            }

            return assessmentNode;
        }
    }
}
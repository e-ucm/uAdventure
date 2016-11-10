using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class AssessmentSubParser_ : Subparser_
{
    /**
     * Assessment rule currently being read
     */
    private AssessmentRule currentAssessmentRule;

    /**
     * Set of conditions being read
     */
    private Conditions currentConditions;

    /**
     * Set of either conditions being read
     */
    private Conditions currentEitherCondition;

    /**
     * The assessment profile
     */
    private AssessmentProfile profile;

    public AssessmentSubParser_(Chapter chapter) : base(chapter)
    {
        profile = new AssessmentProfile();
    }

    public override void ParseElement(XmlElement element)
    {

        XmlNodeList
            smtpsconfigs = element.SelectNodes("smtp-config"),
            assessmentsrule = element.SelectNodes("assessment-rule"),
            timedsssessmentsrule = element.SelectNodes("timed-assessment-rule"),
            conditions,
            initsconditions,
            endsconditions,
            setpropertys,
            assessEffects;

        string tmpArgVal;
       
        tmpArgVal = element.GetAttribute("show-report-at-end");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            profile.setShowReportAtEnd(tmpArgVal.Equals("yes"));
        }

        tmpArgVal = element.GetAttribute("send-to-email");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            if (tmpArgVal == null || tmpArgVal.Length < 1)
            {
                profile.setEmail("");
                profile.setSendByEmail(false);
            }
            else
            {
                profile.setEmail(tmpArgVal);
                profile.setSendByEmail(true);
            }
        }

        tmpArgVal = element.GetAttribute("scorm12");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            profile.setScorm12(tmpArgVal.Equals("yes"));
        }

        tmpArgVal = element.GetAttribute("scorm2004");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            profile.setScorm2004(tmpArgVal.Equals("yes"));
        }

        tmpArgVal = element.GetAttribute("name");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            profile.setName(tmpArgVal);
        }

        foreach (XmlElement ell in smtpsconfigs)
        {
            tmpArgVal = element.GetAttribute("smtp-ssl");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                profile.setSmtpSSL(tmpArgVal.Equals("yes"));
            }
            tmpArgVal = element.GetAttribute("smtp-server");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                profile.setSmtpServer(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("smtp-port");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                profile.setSmtpPort(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("smtp-user");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                profile.setSmtpUser(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("smtp-pwd");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                profile.setSmtpPwd(tmpArgVal);
            }
        }

        foreach (XmlElement ell in assessmentsrule)
        {

            string id = null;
            int importance = 0;
            bool repeatRule = false;

            tmpArgVal = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                id = tmpArgVal;
            }
            tmpArgVal = element.GetAttribute("importance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                for (int j = 0; j < AssessmentRule.N_IMPORTANCE_VALUES; j++)
                    if (tmpArgVal.Equals(AssessmentRule.IMPORTANCE_VALUES[j]))
                        importance = j;
            }
            tmpArgVal = element.GetAttribute("repeatRule");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                repeatRule = tmpArgVal.Equals("yes");
            }

            currentAssessmentRule = new AssessmentRule(id, importance, repeatRule);

            conditions = element.SelectNodes("condition");
            foreach (XmlElement ell_ in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell_);
                currentAssessmentRule.setConditions(currentConditions);
            }

            initsconditions = element.SelectNodes("init-condition");
            foreach (XmlElement ell_ in initsconditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell_);
                ((TimedAssessmentRule)currentAssessmentRule).setInitConditions(currentConditions);
            }

            endsconditions = element.SelectNodes("end-condition");
            foreach (XmlElement ell_ in endsconditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell_);
                ((TimedAssessmentRule)currentAssessmentRule).setEndConditions(currentConditions);
            }

            if(ell.SelectSingleNode("concept")!= null)
                currentAssessmentRule.setConcept(ell.SelectSingleNode("concept").InnerText.ToString().Trim());
            if (ell.SelectSingleNode("set-text") != null)
                currentAssessmentRule.setText(ell.SelectSingleNode("set-text").InnerText.ToString().Trim());

            assessEffects = element.SelectNodes("assessEffect");
            foreach (XmlElement ell_ in assessEffects)
            {
                int timeMin = int.MinValue;
                int timeMax = int.MinValue;
                tmpArgVal = element.GetAttribute("time-min");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    timeMin = int.Parse(tmpArgVal);
                }
                tmpArgVal = element.GetAttribute("time-max");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    timeMax = int.Parse(tmpArgVal);
                }

                TimedAssessmentRule tRule = (TimedAssessmentRule)currentAssessmentRule;
                if (timeMin != int.MinValue && timeMax != int.MaxValue)
                {
                    tRule.addEffect(timeMin, timeMax);
                }
                else
                {
                    tRule.addEffect();
                }
            }

            profile.addRule(currentAssessmentRule);
        }

        foreach (XmlElement ell in timedsssessmentsrule)
        {
            string id = null;
            int importance = 0;
            bool usesEndConditions = false;
            bool has = false;
            bool repeatRule = false;

            tmpArgVal = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                id = tmpArgVal;
            }
            tmpArgVal = element.GetAttribute("importance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                for (int j = 0; j < AssessmentRule.N_IMPORTANCE_VALUES; j++)
                    if (tmpArgVal.Equals(AssessmentRule.IMPORTANCE_VALUES[j]))
                        importance = j;
            }
            tmpArgVal = element.GetAttribute("usesEndConditions");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                has = true;
                usesEndConditions = tmpArgVal.Equals("yes");
            }
            tmpArgVal = element.GetAttribute("repeatRule");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                has = true;
                repeatRule = tmpArgVal.Equals("yes");
            }
            currentAssessmentRule = new TimedAssessmentRule(id, importance, repeatRule);
            if (has)
                ((TimedAssessmentRule) currentAssessmentRule).setUsesEndConditions(usesEndConditions);

            conditions = element.SelectNodes("condition");
            foreach (XmlElement ell_ in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell_);
                currentAssessmentRule.setConditions(currentConditions);
            }

            initsconditions = element.SelectNodes("init-condition");
            foreach (XmlElement ell_ in initsconditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell_);
                ((TimedAssessmentRule)currentAssessmentRule).setInitConditions(currentConditions);
            }

            endsconditions = element.SelectNodes("end-condition");
            foreach (XmlElement ell_ in endsconditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell_);
                ((TimedAssessmentRule)currentAssessmentRule).setEndConditions(currentConditions);
            }

            if (ell.SelectSingleNode("concept") != null)
                currentAssessmentRule.setConcept(ell.SelectSingleNode("concept").InnerText.ToString().Trim());
            if (ell.SelectSingleNode("set-text") != null)
                currentAssessmentRule.setText(ell.SelectSingleNode("set-text").InnerText.ToString().Trim());

            assessEffects = element.SelectNodes("assessEffect");
            foreach (XmlElement ell_ in assessEffects)
            {
                int timeMin = int.MinValue;
                int timeMax = int.MinValue;
                tmpArgVal = element.GetAttribute("time-min");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    timeMin = int.Parse(tmpArgVal);
                }
                tmpArgVal = element.GetAttribute("time-max");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    timeMax = int.Parse(tmpArgVal);
                }

                TimedAssessmentRule tRule = (TimedAssessmentRule)currentAssessmentRule;
                if (timeMin != int.MinValue && timeMax != int.MaxValue)
                {
                    tRule.addEffect(timeMin, timeMax);
                }
                else
                {
                    tRule.addEffect();
                }
            }

            profile.addRule(currentAssessmentRule);
        }

        chapter.addAssessmentProfile(profile);
    }
}
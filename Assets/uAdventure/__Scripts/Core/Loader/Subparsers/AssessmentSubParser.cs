using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace uAdventure.Core
{
	public class AssessmentSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			AssessmentProfile profile = new AssessmentProfile ();

			profile.setShowReportAtEnd(ExString.EqualsDefault(element.GetAttribute("show-report-at-end"), "yes", false));

			profile.setName(element.GetAttribute("name"));
			profile.setEmail(element.GetAttribute ("send-to-email"));
			profile.setSendByEmail(!string.IsNullOrEmpty(profile.getEmail ()));
			profile.setScorm12(ExString.EqualsDefault(element.GetAttribute("scorm12"), "yes", false));
			profile.setScorm2004(ExString.EqualsDefault(element.GetAttribute("scorm2004"), "yes", false));

			var smtpConfig = element.SelectSingleNode("smtp-config") as XmlElement;
			if(smtpConfig != null)
			{
				profile.setSmtpSSL(ExString.EqualsDefault(element.GetAttribute("smtp-ssl"), "yes", false));
				profile.setSmtpServer(smtpConfig.GetAttribute("smtp-server"));
				profile.setSmtpPort(smtpConfig.GetAttribute("smtp-port"));
				profile.setSmtpUser(smtpConfig.GetAttribute("smtp-user"));
				profile.setSmtpPwd(smtpConfig.GetAttribute("smtp-pwd"));
            }

			// NORMAL ASSESMENT RULES
			foreach (XmlElement ell in element.SelectNodes("assessment-rule"))
            {
				var currentAssessmentRule = new AssessmentRule("", 0, false);
				fillAssesmentRule (ell, currentAssessmentRule, parameters);
                profile.addRule(currentAssessmentRule);
            }

			// TIMED ASSESMENT RULES
			foreach (XmlElement ell in element.SelectNodes("timed-assessment-rule"))
            {
				bool usesEndConditions = ExString.EqualsDefault(element.GetAttribute("usesEndConditions"), "yes", false);

				var tRule = new TimedAssessmentRule("", 0, false);
				fillAssesmentRule (ell, tRule, parameters);

				if (usesEndConditions || tRule.isRepeatRule ())
					tRule.setUsesEndConditions(usesEndConditions);
				
				tRule.setInitConditions(DOMParserUtility.DOMParse (element.SelectSingleNode("init-condition"), parameters) 			as Conditions ?? new Conditions());
				tRule.setEndConditions(DOMParserUtility.DOMParse (element.SelectSingleNode("end-condition"), parameters)			as Conditions ?? new Conditions());

				foreach (XmlElement ell_ in element.SelectNodes("assessEffect"))
				{
					int timeMin = ExParsers.ParseDefault ( ell_.GetAttribute("time-min") , int.MinValue);
					int timeMax = ExParsers.ParseDefault ( ell_.GetAttribute("time-max") , int.MinValue);

                    if (timeMin != int.MinValue && timeMax != int.MaxValue)
						tRule.addEffect(timeMin, timeMax);
                    else
						tRule.addEffect();
                }

				profile.addRule(tRule);
            }

			return profile;
        }

		/// <summary>
		/// Fills the assesment rule.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="rule">Rule.</param>
		/// <param name="parameters">Parameters.</param>
		private void fillAssesmentRule(XmlElement element, AssessmentRule rule, params object[] parameters){

			string id = element.GetAttribute("id");
			int importance = 0;
			bool repeatRule = ExString.EqualsDefault(element.GetAttribute("repeatRule"), "yes", false);

			var tmpArgVal = element.GetAttribute("importance");
			if (!string.IsNullOrEmpty(tmpArgVal))
			{
				for (int j = 0; j < AssessmentRule.N_IMPORTANCE_VALUES; j++)
					if (tmpArgVal.Equals(AssessmentRule.IMPORTANCE_VALUES[j]))
						importance = j;
			}

			rule.setId (id);
			rule.setImportance (importance);
			rule.setRepeatRule (repeatRule);

			rule.setConditions(DOMParserUtility.DOMParse (element.SelectSingleNode("condition"), parameters) 	as Conditions ?? new Conditions());

			var concept = element.SelectSingleNode ("concept");
			if (concept != null) rule.setConcept(concept.ToString().Trim());

			var setText = element.SelectSingleNode("set-text");
			if (setText != null) rule.setText(setText.InnerText.ToString().Trim());
		}
    }
}
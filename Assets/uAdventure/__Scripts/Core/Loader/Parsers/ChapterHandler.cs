using System.Collections.Generic;
using System.Xml;
using System.Linq;
using uAdventure.Runner;

namespace uAdventure.Core
{
    public class ChapterHandler : XmlHandler<Chapter>
    {
        public ChapterHandler(Chapter chapter, ResourceManager resourceManager, List<Incidence> incidences) : base(chapter, resourceManager, incidences)
        {
        }

        protected override Chapter CreateObject()
        {
            return new Chapter();
        }

        protected override Chapter ParseXml(XmlDocument doc)
        {
            XmlElement element = doc.DocumentElement;
            XmlNodeList eAdventure = element.SelectNodes("/eAdventure");

            XmlElement adaptation = (XmlElement)element.SelectSingleNode("adaptation-configuration"),
                assessment        = (XmlElement)element.SelectSingleNode("assessment-configuration"),
                title             = (XmlElement)element.SelectSingleNode("title"),
                description       = (XmlElement)element.SelectSingleNode("description");
            
            if (title != null)
            {
                content.setTitle(title.InnerText);
            }

            if (description != null)
            {
                content.setTitle(description.InnerText);
            }
            
            var restNodes = new List<XmlNode>();
            var e = element.ChildNodes.GetEnumerator();
            while (e.MoveNext())
            {
                restNodes.Add(e.Current as XmlNode);
            }

            var enumerator = eAdventure.GetEnumerator();
            while (enumerator.MoveNext())
            {
                restNodes.Remove(enumerator.Current as XmlNode);
            }

            ParseContent(restNodes);
            ParseAdaptation(eAdventure, adaptation);
            ParseAssessment(eAdventure, assessment);
            ConfigureInitialScene();

            return content;
        }

        private void ConfigureInitialScene()
        {

            // In the end of the document, if the chapter has no initial scene
            if (content.getTargetId() == null)
            {
                // Set it to the first scene
                if (content.getScenes().Count > 0)
                    content.setTargetId(content.getScenes()[0].getId());

                // Or to the first cutscene
                else if (content.getCutscenes().Count > 0)
                    content.setTargetId(content.getCutscenes()[0].getId());
            }
        }

        private void ParseContent(List<XmlNode> restNodes)
        {
            foreach (var el in restNodes)
            {
                object parsed = DOMParserUtility.DOMParse(el as XmlElement, content);
                if (parsed != null)
                {
                    var t = parsed.GetType();
                    // Gropu elements that are ITypeGroupable
                    if (parsed is ITypeGroupable)
                    {
                        t = (parsed as ITypeGroupable).GetGroupType();
                    }

                    content.getObjects(t).Add(parsed);
                }
            }
        }

        private void ParseAssessment(XmlNodeList eAdventure, XmlElement assestment)
        {
            if (assestment == null)
            {
                return;
            }

            foreach (XmlElement el in eAdventure)
            {
                if (!string.IsNullOrEmpty(el.GetAttribute("assessProfile")))
                {
                    content.setAssessmentName(el.GetAttribute("assessProfile"));
                }
            }

            var path = assestment.GetAttribute("path");
            if (!string.IsNullOrEmpty(path))
            {
                string assessmentName = path;
                // delete the path's characteristics
                assessmentName = assessmentName.Substring(assessmentName.IndexOf("/") + 1);
                assessmentName = assessmentName.Substring(0, assessmentName.IndexOf("."));
                content.setAssessmentName(assessmentName);
            }
        }

        private void ParseAdaptation(XmlNodeList eAdventure, XmlElement adaptation)
        {
            if (adaptation == null)
            {
                return;
            }

            foreach (XmlElement el in eAdventure)
            {
                if (!string.IsNullOrEmpty(el.GetAttribute("adaptProfile")))
                {
                    content.setAdaptationName(el.GetAttribute("adaptProfile"));
                }
            }

            var path = adaptation.GetAttribute("path");
            if (!string.IsNullOrEmpty(path))
            {
                string adaptationName = path;
                // delete the path's characteristics
                adaptationName = adaptationName.Substring(adaptationName.IndexOf("/") + 1);
                adaptationName = adaptationName.Substring(0, adaptationName.IndexOf("."));
                content.setAdaptationName(adaptationName);
            }
        }
    }
}
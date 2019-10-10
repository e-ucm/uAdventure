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

            ParseContent(content, restNodes);
            ParseAdaptation(content, eAdventure, adaptation);
            ParseAssessment(content, eAdventure, assessment);
            ConfigureInitialScene(content);

            return content;
        }

        private static void ConfigureInitialScene(Chapter chapter)
        {
            // In the end of the document, if the chapter has no initial scene
            if (chapter.getTargetId() == null)
            {
                // Set it to the first scene
                if (chapter.getScenes().Count > 0)
                {
                    chapter.setTargetId(chapter.getScenes()[0].getId());
                }
                // Or to the first cutscene
                else if (chapter.getCutscenes().Count > 0)
                {
                    chapter.setTargetId(chapter.getCutscenes()[0].getId());
                }
            }
        }

        private static void ParseContent(Chapter chapter, List<XmlNode> restNodes)
        {
            foreach (var el in restNodes)
            {
                object parsed = DOMParserUtility.DOMParse(el as XmlElement, chapter);
                if (parsed != null)
                {
                    var t = GroupableTypeAttribute.GetGroupType(parsed.GetType());
                    chapter.getObjects(t).Add(parsed);
                }
            }
        }

        private static void ParseAssessment(Chapter chapter, XmlNodeList eAdventure, XmlElement assestment)
        {
            if (assestment == null)
            {
                return;
            }

            foreach (XmlElement el in eAdventure)
            {
                if (!string.IsNullOrEmpty(el.GetAttribute("assessProfile")))
                {
                    chapter.setAssessmentName(el.GetAttribute("assessProfile"));
                }
            }

            var path = assestment.GetAttribute("path");
            if (!string.IsNullOrEmpty(path))
            {
                string assessmentName = GetName(path);
                chapter.setAssessmentName(assessmentName);
            }
        }

        private static void ParseAdaptation(Chapter chapter, XmlNodeList eAdventure, XmlElement adaptation)
        {
            if (adaptation == null)
            {
                return;
            }

            foreach (XmlElement el in eAdventure)
            {
                if (!string.IsNullOrEmpty(el.GetAttribute("adaptProfile")))
                {
                    chapter.setAdaptationName(el.GetAttribute("adaptProfile"));
                }
            }

            var path = adaptation.GetAttribute("path");
            if (!string.IsNullOrEmpty(path))
            {
                string adaptationName = GetName(path);
                chapter.setAdaptationName(adaptationName);
            }
        }

        private static string GetName(string path)
        {
            var name = path.Substring(path.IndexOf("/", System.StringComparison.InvariantCulture) + 1);
            name = name.Substring(0, name.IndexOf(".", System.StringComparison.InvariantCulture));

            return name;
        }
    }
}
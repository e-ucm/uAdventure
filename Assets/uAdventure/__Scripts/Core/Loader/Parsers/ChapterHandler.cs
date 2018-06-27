using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

// TODO possible unnecesary coupling
using uAdventure.Runner;

namespace uAdventure.Core
{
    public class ChapterHandler
    {
        /**
         * Chapter data
         */
        private readonly Chapter chapter;
        private readonly ResourceManager resourceManager;
        private readonly List<Incidence> incidences;

        /**
         * Default constructor.
         * 
         * @param chapter
         *            Chapter in which the data will be stored
         */
        public ChapterHandler(Chapter chapter, ResourceManager resourceManager, List<Incidence> incidences)
        {
            this.chapter = chapter;
            this.resourceManager = resourceManager;
            this.incidences = incidences;
        }

        public string getXmlContent(string path)
        {
            return resourceManager.getText(path);
        }

        public void Parse(string path)
        {
            string xml = getXmlContent(path);
            if (string.IsNullOrEmpty(xml))
                return;

            ParseXml(xml);
        }

        public void ParseXml(string xml)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(xml);

            XmlElement element = xmld.DocumentElement;
            XmlNodeList eAdventure = element.SelectNodes("/eAdventure");

            var restNodes = new List<XmlNode>();
            var e = element.ChildNodes.GetEnumerator();
            while (e.MoveNext()) restNodes.Add(e.Current as XmlNode);

            var l = new List<XmlNodeList>();
            l.Add(eAdventure);

            foreach(var xmlnodelist in l)
            {
                var enumerator = xmlnodelist.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    restNodes.Remove(enumerator.Current as XmlNode);
                }
            }

            var title = (XmlElement)element.SelectSingleNode("title");
            if (title != null)
            {
                chapter.setTitle(title.InnerText);
            }

            var description = (XmlElement)element.SelectSingleNode("description");
            if (description != null)
            {
                chapter.setTitle(description.InnerText);
            }


            foreach (XmlElement el in eAdventure)
            {
                if (!string.IsNullOrEmpty(el.GetAttribute("adaptProfile")))
                {
                    chapter.setAdaptationName(el.GetAttribute("adaptProfile"));
                }
                if (!string.IsNullOrEmpty(el.GetAttribute("assessProfile")))
                {
                    chapter.setAssessmentName(el.GetAttribute("assessProfile"));
                }
            }


            XmlElement adaptation = (XmlElement)element.SelectSingleNode("adaptation-configuration");
            if (adaptation != null)
            {
                var tmpString = adaptation.GetAttribute("path");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    string adaptationName = tmpString;
                    // delete the path's characteristics
                    adaptationName = adaptationName.Substring(adaptationName.IndexOf("/") + 1);
                    adaptationName = adaptationName.Substring(0, adaptationName.IndexOf("."));
                    chapter.setAdaptationName(adaptationName);
                }
            }

            XmlElement assestment = (XmlElement)element.SelectSingleNode("assessment-configuration");
            if (assestment != null)
            {
                var tmpString = assestment.GetAttribute("path");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    string assessmentName = tmpString;
                    // delete the path's characteristics
                    assessmentName = assessmentName.Substring(assessmentName.IndexOf("/") + 1);
                    assessmentName = assessmentName.Substring(0, assessmentName.IndexOf("."));
                    chapter.setAssessmentName(assessmentName);
                }
            }

            foreach (var el in restNodes)
            {
                object parsed = DOMParserUtility.DOMParse(el as XmlElement, chapter);
                if(parsed != null)
                {
                    var t = parsed.GetType();
                    if (parsed is ITypeGroupable)
                        t = (parsed as ITypeGroupable).GetGroupType();

                    chapter.getObjects(t).Add(parsed);
                }
            }

            // In the end of the document, if the chapter has no initial scene
            if (chapter.getTargetId() == null)
            {
                // Set it to the first scene
                if (chapter.getScenes().Count > 0)
                    chapter.setTargetId(chapter.getScenes()[0].getId());

                // Or to the first cutscene
                else if (chapter.getCutscenes().Count > 0)
                    chapter.setTargetId(chapter.getCutscenes()[0].getId());
            }
        }

    }
}
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
        private Chapter chapter;

        /**
         * Current global state being subparsed
         */
        private GlobalState currentGlobalState;

        /**
         * Current macro being subparsed
         */
        private Macro currentMacro;

        /**
         * Buffer for globalstate docs
         */
        //private string currentString;

        /* Methods */
        private ResourceManager resourceManager;

        /**
         * Default constructor.
         * 
         * @param chapter
         *            Chapter in which the data will be stored
         */
        public ChapterHandler(Chapter chapter, ResourceManager resourceManager)
        {
            this.chapter = chapter;
            this.resourceManager = resourceManager;
           // currentString = string.Empty;
        }

        public string getXmlContent(string path)
        {
            return resourceManager.getText(path);
        } 

        public void Parse(string path)
        {
            XmlDocument xmld = new XmlDocument();

            string xml = getXmlContent(path);

            xmld.LoadXml(xml);

            XmlElement element = xmld.DocumentElement;
            XmlNodeList
                eAdventure = element.SelectNodes("/eAdventure");

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

            foreach(var el in restNodes)
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
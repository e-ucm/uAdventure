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
    public class ChapterHandler_
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

        /**
         * Default constructor.
         * 
         * @param chapter
         *            Chapter in which the data will be stored
         */
        public ChapterHandler_(Chapter chapter)
        {
            this.chapter = chapter;
           // currentString = string.Empty;
        }

        public string getXmlContent(string path)
        {
            string xml = "";
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                xml = System.IO.File.ReadAllText(path);
            }
            else
#endif
                switch (ResourceManager.Instance.getLoadingType())
                {
                    case ResourceManager.LoadingType.RESOURCES_LOAD:
                        if (path.Contains(".xml"))
                        {
                            path = path.Replace(".xml", "");
                        }

                        TextAsset ta = Resources.Load(path) as TextAsset;

                        if (ta == null)
                        {
                            Debug.Log("Can't load chapter file: " + path);
                        }
                        else
                            xml = ta.text;
                        break;
                    case ResourceManager.LoadingType.SYSTEM_IO:
                        xml = System.IO.File.ReadAllText(path);
                        break;
                }

            return xml;
        }

        public void Parse(string path)
        {
            XmlDocument xmld = new XmlDocument();

            string xml = getXmlContent(path);

            xmld.LoadXml(xml);

            XmlElement element = xmld.DocumentElement;
            XmlNodeList
                eAdventure = element.SelectNodes("/eAdventure"),
                scenes = element.SelectNodes("/eAdventure/scene"),
                slidescenes = element.SelectNodes("/eAdventure/slidescene"),
                videoscenes = element.SelectNodes("/eAdventure/videoscene"),
                books = element.SelectNodes("/eAdventure/book"),
                objects = element.SelectNodes("/eAdventure/object"),
                players = element.SelectNodes("/eAdventure/player"),
                characters = element.SelectNodes("/eAdventure/character"),
                treeconversations = element.SelectNodes("/eAdventure/tree-conversation"),
                graphconversations = element.SelectNodes("/eAdventure/graph-conversation"),
                globalstates = element.SelectNodes("/eAdventure/global-state"),
                macros = element.SelectNodes("/eAdventure/macro"),
                timers = element.SelectNodes("/eAdventure/timer"),
                atrezzoobjects = element.SelectNodes("/eAdventure/atrezzoobject"),
                assessment = element.SelectNodes("/eAdventure/assessment"),
                completables = element.SelectNodes("/eAdventure/completable"),
                adaptation = element.SelectNodes("/eAdventure/adaptation");

            var restNodes = new List<XmlNode>();
            var e = element.ChildNodes.GetEnumerator();
            while (e.MoveNext()) restNodes.Add(e.Current as XmlNode);

            var l = new List<XmlNodeList>();
            l.Add(eAdventure);         l.Add(scenes);              l.Add(slidescenes);    l.Add(videoscenes);
            l.Add(books);              l.Add(objects);             l.Add(players);        l.Add(characters);
            l.Add(treeconversations);  l.Add(graphconversations);  l.Add(globalstates);   l.Add(macros);
            l.Add(timers);             l.Add(atrezzoobjects);      l.Add(assessment);     l.Add(completables);
            l.Add(adaptation);

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


            foreach (XmlElement el in scenes)
            {
                new SceneSubParser_(chapter).ParseElement(el);
            }

            foreach (XmlElement el in slidescenes)
            {
                //TODO: subparser
                new CutsceneSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in videoscenes)
            {
                //TODO: subparser
                new CutsceneSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in books)
            {
                //TODO: subparser
                new BookSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in objects)
            {
                //TODO: subparser
                new ItemSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in players)
            {
                //TODO: subparser
                new PlayerSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in characters)
            {
                //TODO: subparser
                new CharacterSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in treeconversations)
            {
                //TODO: subparser
                new TreeConversationSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in graphconversations)
            {
                //TODO: subparser
                new GraphConversationSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in globalstates)
            {
                string id = el.GetAttribute("id");
                currentGlobalState = new GlobalState(id);
                //currentString = string.Empty;
                chapter.addGlobalState(currentGlobalState);
                //TODO: subparser
                new ConditionSubParser_(currentGlobalState, chapter).ParseElement(el);
                currentGlobalState.setDocumentation(el.InnerText);
            }
            foreach (XmlElement el in macros)
            {
                string id = el.GetAttribute("id");
                currentMacro = new Macro(id);
                //currentString = string.Empty;
                chapter.addMacro(currentMacro);
                //TODO: subparser
                new EffectSubParser_(currentMacro, chapter).ParseElement(el);
                currentMacro.setDocumentation(el.InnerText);
            }
            foreach (XmlElement el in timers)
            {
                //TODO: subparser
                new TimerSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in atrezzoobjects)
            {
                //TODO: subparser
                new AtrezzoSubParser_(chapter).ParseElement(el);
            }

            foreach (XmlElement el in completables)
            {
                //TODO: subparser
                new CompletableSubParser_(chapter).ParseElement(el);
            }

            foreach (XmlElement el in assessment)
            {
                //TODO: subparser
                new AssessmentSubParser_(chapter).ParseElement(el);
            }
            foreach (XmlElement el in adaptation)
            {
                //TODO: 
                new AdaptationSubParser_(chapter).ParseElement(el);
            }

            foreach(var el in restNodes)
            {
                object parsed = DOMParserUtility.DOMParse(el as XmlElement, chapter);
                if(parsed != null)
                {
                    var t = parsed.GetType();
                    if (parsed is ITypeGroupable)
                        t = (parsed as ITypeGroupable).GetGroupType();

                    chapter.getObjects(parsed.GetType()).Add(parsed);
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
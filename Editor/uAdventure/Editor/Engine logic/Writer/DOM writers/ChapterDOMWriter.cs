using System;
using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Chapter))]
    public class ChapterDOMWriter : ParametrizedDOMWriter
    {
        public ChapterDOMWriter()
        {

        }

        protected override string GetElementNameFor(object target)
        {
            return "eAdventure";
        }

        public class ChapterTargetIDParam : IDOMWriterParam
        {
            private readonly string targetId;
            public ChapterTargetIDParam(string targetId)
            {
                this.targetId = targetId;
            }

            public string TargetId
            {
                get
                {
                    return this.targetId;
                }
            }
        }

        public static IDOMWriterParam ChapterTargetID(string id)
        {
            return new ChapterTargetIDParam(id);
        }

        /**
         * Returns the DOM element for the chapter
         * 
         * @param chapter
         *            Chapter data to be written
         * @return DOM element with the chapter data
         */

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var chapter = target as Chapter;
            var doc = Writer.GetDoc();
            XmlElement chapterNode = node as XmlElement;
            chapterNode.SetAttribute("version", "3");

            // Add the adaptation and assessment active profiles
            if (!chapter.getAdaptationName().Equals(""))
                chapterNode.SetAttribute("adaptProfile", chapter.getAdaptationName());

            // Create and append the assessment configuration
            if (!chapter.getAssessmentName().Equals(""))
            {
                chapterNode.SetAttribute("assessProfile", chapter.getAssessmentName());
            }

            var targetParam = ChapterTargetID(chapter.getTargetId());

            // Append the player element
            DOMWriterUtility.DOMWrite(chapterNode, chapter.getPlayer(), targetParam);

            foreach (var type in chapter.getObjectTypes())
            {
                foreach(var tosave in chapter.getObjects(type))
                {
                    DOMWriterUtility.DOMWrite(chapterNode, tosave, targetParam);
                }
            }


            // TODO FIX THIS and use normal domwriter

            /** ******* START WRITING THE ADAPTATION DATA ***** */
            foreach (AdaptationProfile profile in chapter.getAdaptationProfiles())
            {
                chapterNode.AppendChild(Writer.writeAdaptationData(profile, true, doc));
            }
            /** ******* END WRITING THE ADAPTATION DATA ***** */

            /** ******* START WRITING THE ASSESSMENT DATA ***** */
            foreach (AssessmentProfile profile in chapter.getAssessmentProfiles())
            {
                chapterNode.AppendChild(Writer.writeAssessmentData(profile, true, doc));
            }
            /** ******* END WRITING THE ASSESSMENT DATA ***** */
        }
    }
}
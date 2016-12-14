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
            private string targetId;
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

            //try {
            // Create the necessary elements to create the DOM
            /*DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance( );
            DocumentBuilder db = dbf.newDocumentBuilder( );
            Document doc = db.newDocument( );
            */
            // Create the root node
            chapterNode = doc.CreateElement("eAdventure");

            // Add the adaptation and assessment active profiles
            if (!chapter.getAdaptationName().Equals(""))
                chapterNode.SetAttribute("adaptProfile", chapter.getAdaptationName());

            // Create and append the assessment configuration
            if (!chapter.getAssessmentName().Equals(""))
            {
                chapterNode.SetAttribute("assessProfile", chapter.getAssessmentName());
            }

            var targetParam = ChapterTargetID(chapter.getTargetId());

            // Append the scene elements
            foreach (Scene scene in chapter.getScenes())
            {
                DOMWriterUtility.DOMWrite(chapterNode, scene, targetParam);
            }

            // Append the cutscene elements
            foreach (Cutscene cutscene in chapter.getCutscenes())
            {
                DOMWriterUtility.DOMWrite(chapterNode, cutscene, targetParam);
            }

            // Append the book elements
            foreach (Book book in chapter.getBooks())
            {
                DOMWriterUtility.DOMWrite(chapterNode, book, targetParam);
            }

            // Append the item elements
            foreach (Item item in chapter.getItems())
            {
                DOMWriterUtility.DOMWrite(chapterNode, item, targetParam);
            }

            // Append the player element
            DOMWriterUtility.DOMWrite(chapterNode, chapter.getPlayer(), targetParam);

            // Append the character element
            foreach (NPC character in chapter.getCharacters())
            {
                DOMWriterUtility.DOMWrite(chapterNode, character, targetParam);
            }

            // Append the conversation element
            foreach (Conversation conversation in chapter.getConversations())
            {
                DOMWriterUtility.DOMWrite(chapterNode, conversation, targetParam);
            }

            // Append the timers
            foreach (Timer timer in chapter.getTimers())
            {
                DOMWriterUtility.DOMWrite(chapterNode, timer, targetParam);
            }

            // Append global states
            foreach (GlobalState globalState in chapter.getGlobalStates())
            {
                DOMWriterUtility.DOMWrite(chapterNode, globalState, targetParam);
            }

            // Append macros
            foreach (Macro macro in chapter.getMacros())
            {
                DOMWriterUtility.DOMWrite(chapterNode, macro, targetParam);
            }

            // Append the atrezzo item elements
            foreach (Atrezzo atrezzo in chapter.getAtrezzo())
            {
                DOMWriterUtility.DOMWrite(chapterNode, atrezzo, targetParam);
            }

            // Append the completables
            foreach (Completable completable in chapter.getCompletabes())
            {
                DOMWriterUtility.DOMWrite(chapterNode, completable, targetParam);
            }

            /*} catch( ParserConfigurationException e ) {
                ReportDialog.GenerateErrorReport(e, true, "UNKNOWERROR");
            }*/


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
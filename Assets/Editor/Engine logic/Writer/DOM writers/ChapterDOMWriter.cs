using System;
using UnityEngine;
using System.Collections;
using System.Xml;

public class ChapterDOMWriter
{

    /**
         * Private constructor.
         */

    private ChapterDOMWriter()
    {

    }

    /**
     * Returns the DOM element for the chapter
     * 
     * @param chapter
     *            Chapter data to be written
     * @return DOM element with the chapter data
     */

    public static XmlNode buildDOM(Chapter chapter, String zipFile, XmlDocument doc)
    {

        XmlElement chapterNode = null;

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

        // Append the scene elements
        foreach (Scene scene in chapter.getScenes())
        {
            bool initialScene = chapter.getTargetId().Equals(scene.getId());
            XmlNode sceneNode = SceneDOMWriter.buildDOM(scene, initialScene);
            doc.ImportNode(sceneNode, true);
            chapterNode.AppendChild(sceneNode);
        }

        // Append the cutscene elements
        foreach (Cutscene cutscene in chapter.getCutscenes())
        {
            bool initialScene = chapter.getTargetId().Equals(cutscene.getId());
            XmlNode cutsceneNode = CutsceneDOMWriter.buildDOM(cutscene, initialScene);
            doc.ImportNode(cutsceneNode, true);
            chapterNode.AppendChild(cutsceneNode);
        }

        // Append the book elements
        foreach (Book book in chapter.getBooks())
        {
            XmlNode bookNode = BookDOMWriter.buildDOM(book);
            doc.ImportNode(bookNode, true);
            chapterNode.AppendChild(bookNode);
        }

        // Append the item elements
        foreach (Item item in chapter.getItems())
        {
            XmlNode itemNode = ItemDOMWriter.buildDOM(item);
            doc.ImportNode(itemNode, true);
            chapterNode.AppendChild(itemNode);
        }

        // Append the player element
        XmlNode playerNode = PlayerDOMWriter.buildDOM(chapter.getPlayer());
        doc.ImportNode(playerNode, true);
        chapterNode.AppendChild(playerNode);

        // Append the character element
        foreach (NPC character in chapter.getCharacters())
        {
            XmlNode characterNode = CharacterDOMWriter.buildDOM(character);
            doc.ImportNode(characterNode, true);
            chapterNode.AppendChild(characterNode);
        }

        // Append the conversation element
        foreach (Conversation conversation in chapter.getConversations())
        {
            XmlNode conversationNode = ConversationDOMWriter.buildDOM(conversation);
            doc.ImportNode(conversationNode, true);
            chapterNode.AppendChild(conversationNode);
        }

        // Append the timers
        foreach (Timer timer in chapter.getTimers())
        {
            XmlNode timerNode = TimerDOMWriter.buildDOM(timer);
            doc.ImportNode(timerNode, true);
            chapterNode.AppendChild(timerNode);
        }

        // Append global states
        foreach (GlobalState globalState in chapter.getGlobalStates())
        {
            XmlElement globalStateElement = ConditionsDOMWriter.buildDOM(globalState);
            doc.ImportNode(globalStateElement, true);
            chapterNode.AppendChild(globalStateElement);
        }

        // Append macros
        foreach (Macro macro in chapter.getMacros())
        {
            XmlElement macroElement = EffectsDOMWriter.buildDOM(macro);
            doc.ImportNode(macroElement, true);
            chapterNode.AppendChild(macroElement);
        }

        // Append the atrezzo item elements
        foreach (Atrezzo atrezzo in chapter.getAtrezzo())
        {
            XmlNode atrezzoNode = AtrezzoDOMWriter.buildDOM(atrezzo);
            doc.ImportNode(atrezzoNode, true);
            chapterNode.AppendChild(atrezzoNode);
        }

        // Append the completables
        foreach (Completable completable in chapter.getCompletabes())
        {
            XmlNode completableNode = CompletableDOMWriter.buildDOM(completable);
            doc.ImportNode(completableNode, true);
            chapterNode.AppendChild(completableNode);
        }

        /*} catch( ParserConfigurationException e ) {
        	ReportDialog.GenerateErrorReport(e, true, "UNKNOWERROR");
        }*/

        return chapterNode;
    }
}
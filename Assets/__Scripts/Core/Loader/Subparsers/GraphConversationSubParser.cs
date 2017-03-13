using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class GraphConversationSubParser_ : Subparser_
    {
        /**
       * Name of the conversation
       */
        private string conversationName;

        /**
         * Stores the current node
         */
        private ConversationNode currentNode;

        /**
         * Set of nodes for the graph
         */
        private List<ConversationNode> graphNodes;

        /**
         * Stores the current set of links (of the current node)
         */
        private List<int> currentLinks;

        /**
         * Bidimensional vector for storing the links between nodes (the first
         * dimension holds the nodes, the second one the links)
         */
        private List<List<int>> nodeLinks;

        /**
         * Current effect (of the current node)
         */
        private Effects currentEffects;

        /**
         * Name of the last non-player character read, "NPC" is no name were found
         */
        private string characterName;

        /**
         * Path of the audio track for a conversation line
         */
        private string audioPath;

        /**
         * Check if the options in option node may be random
         */
        private bool random;

        /**
         * Check if the previous line will be showed at options node
         */
        private bool keepShowing;

        /**
         * Keep showing for each conversation line
         */
        private bool keepShowingLine;

        /**
         * Check if the user's response will be showed
         */
        private bool showUserOption;

        /**
         * Check if the option node allows the pre-listening of the options
         */
        private bool preListening;

        /**
         * The position to be painted the option nodes
         */
        private int x, y;

        /**
         * v1.4 - Graphical Position of nodes in editor
         */
        private int editorX, editorY;

        /**
         * Check if each conversation line will wait until user interacts
         */
        private bool keepShowingDialogue;

        /**
         * Check if a conversation line must be synthesize
         */
        private bool synthesizerVoice;

        /**
         * Stores the current conditions being read
         */
        private Conditions currentConditions;

        /**
         * Store the current conversation line
         */
        private ConversationLine conversationLine;

        public GraphConversationSubParser_(Chapter chapter) : base(chapter)
        {
            keepShowing = false;
        }

        public override void ParseElement(XmlElement element)
        {
            XmlNodeList effects;
            /* speakschar,
                speaksplayer,
                childs,
                conditions;*/

            XmlNode end_conversation;

            string tmpArgVal;

            // Store the name
            conversationName = "";
            tmpArgVal = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                conversationName = tmpArgVal;
            }

            graphNodes = new List<ConversationNode>();
            nodeLinks = new List<List<int>>();

            foreach (XmlElement el in element)
            {
                if (el.Name == "dialogue-node")
                {
                    // Create the node depending of the tag
                    editorX = editorY = int.MinValue;

                    //If there is a "waitUserInteraction" attribute, store if the lines will wait until user interacts
                    tmpArgVal = element.GetAttribute("keepShowing");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            keepShowingDialogue = true;
                        else
                            keepShowingDialogue = false;
                    }

                    //If there is a "editor-x" and "editor-y" attributes     
                    tmpArgVal = element.GetAttribute("editor-x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        editorX = Mathf.Max(0, int.Parse(tmpArgVal));
                    }

                    tmpArgVal = element.GetAttribute("editor-y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        editorY = Mathf.Max(0, int.Parse(tmpArgVal));
                    }

                    currentNode = new DialogueConversationNode(keepShowingDialogue);
                    if (editorX > int.MinValue)
                    {
                        currentNode.setEditorX(editorX);
                    }
                    if (editorY > int.MinValue)
                    {
                        currentNode.setEditorY(editorY);
                    }

                    // Create a new vector for the links of the current node
                    currentLinks = new List<int>();
                    parseLines(currentNode, el);

                    end_conversation = el.SelectSingleNode("end-conversation");
                    if (end_conversation != null)
                        effects = end_conversation.SelectNodes("effect");
                    else
                        effects = el.SelectNodes("effect");

                    foreach (XmlElement ell in effects)
                    {
                        currentEffects = new Effects();
                        new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                        currentNode.setEffects(currentEffects);
                    }

                    // Add the current node to the node list, and the set of children references into the node links
                    graphNodes.Add(currentNode);
                    nodeLinks.Add(currentLinks);

                }
                else if (el.Name == "option-node")
                {

                    editorX = editorY = int.MinValue;

                    //If there is a "waitUserInteraction" attribute, store if the lines will wait until user interacts
                    tmpArgVal = el.GetAttribute("keepShowing");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            keepShowingDialogue = true;
                        else
                            keepShowingDialogue = false;
                    }

                    //If there is a "editor-x" and "editor-y" attributes     
                    tmpArgVal = el.GetAttribute("editor-x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        editorX = Mathf.Max(0, int.Parse(tmpArgVal));
                    }
                    tmpArgVal = el.GetAttribute("editor-y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        editorY = Mathf.Max(0, int.Parse(tmpArgVal));
                    }

                    tmpArgVal = el.GetAttribute("random");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            random = true;
                        else
                            random = false;
                    }

                    tmpArgVal = el.GetAttribute("showUserOption");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            showUserOption = true;
                        else
                            showUserOption = false;
                    }

                    tmpArgVal = el.GetAttribute("preListening");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            preListening = true;
                        else
                            preListening = false;
                    }

                    //If there is a "x" and "y" attributes with the position where the option node has to be painted
                    tmpArgVal = el.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            preListening = true;
                        else
                            preListening = false;
                    }
                    tmpArgVal = el.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("yes"))
                            preListening = true;
                        else
                            preListening = false;
                    }

                    currentNode = new OptionConversationNode(random, keepShowing, showUserOption, preListening, x, y);

                    //XAPI ELEMENTS
                    tmpArgVal = el.GetAttribute("question");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        ((OptionConversationNode)currentNode).setXApiQuestion(tmpArgVal);
                    }
                    //END OF XAPI

                    if (editorX > int.MinValue)
                    {
                        x = editorX;
                    }
                    if (editorY > int.MinValue)
                    {
                        y = editorY;
                    }
                    // Create a new vector for the links of the current node
                    currentLinks = new List<int>();
                    parseLines(currentNode, el);

                    end_conversation = el.SelectSingleNode("end-conversation");
                    if (end_conversation != null)
                        effects = end_conversation.SelectNodes("effect");
                    else
                        effects = el.SelectNodes("effect");

                    foreach (XmlElement ell in effects)
                    {
                        currentEffects = new Effects();
                        new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                        currentNode.setEffects(currentEffects);
                    }

                    // Add the current node to the node list, and the set of children references into the node links
                    graphNodes.Add(currentNode);
                    nodeLinks.Add(currentLinks);
                }
            }

            setNodeLinks();
            chapter.addConversation(new GraphConversation(conversationName, graphNodes[0]));
        }

        /**
        * Set the links between the conversational nodes, taking the indexes of
        * their children, stored in nodeLinks
        */

        private void setNodeLinks()
        {
            // The size of graphNodes and nodeLinks should be the same
            for (int i = 0; i < graphNodes.Count; i++)
            {
                // Extract the node and its links
                ConversationNode node = graphNodes[i];
                List<int> links = nodeLinks[i];

                // For each reference, insert the referenced node into the father node
                for (int j = 0; j < links.Count; j++)
                    node.addChild(graphNodes[links[j]]);
            }
        }

        private void parseLines(ConversationNode node, XmlElement lines)
        {
            string tmpArgVal = "";
            currentLinks = new List<int>();
            bool addline = true;

            foreach (XmlElement ell in lines.ChildNodes)
            {
                addline = true;
                if (ell.Name == "speak-player")
                {
                    audioPath = "";

                    tmpArgVal = ell.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        audioPath = tmpArgVal;
                    }

                    // If there is a "synthesize" attribute, store its value
                    tmpArgVal = ell.GetAttribute("synthesize");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        string response = tmpArgVal;
                        if (response.Equals("yes"))
                            synthesizerVoice = true;
                        else
                            synthesizerVoice = false;
                    }

                    // If there is a "keepShowing" attribute, store its value
                    tmpArgVal = ell.GetAttribute("keepShowing");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        string response = tmpArgVal;
                        if (response.Equals("yes"))
                            keepShowingLine = true;
                        else
                            keepShowingLine = false;
                    }

                    // Store the read string into the current node, and then delete the string. The trim is performed so we
                    // don't have to worry with indentations or leading/trailing spaces
                    conversationLine = new ConversationLine(ConversationLine.PLAYER, ell.InnerText);
                    if (audioPath != null && !this.audioPath.Equals(""))
                    {
                        conversationLine.setAudioPath(audioPath);
                    }

                    conversationLine.setSynthesizerVoice(synthesizerVoice);

                    conversationLine.setKeepShowing(keepShowingLine);

                    //XAPI ELEMENTS
                    tmpArgVal = ell.GetAttribute("correct");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        conversationLine.setXApiCorrect(tmpArgVal == "true");
                    }
                    //END OF XAPI
                }
                else if (ell.Name == "speak-char")
                {
                    // If it is a non-player character line, store the character name and audio path (if present)
                    // Set default name to "NPC"
                    characterName = "NPC";
                    audioPath = "";


                    // If there is a "idTarget" attribute, store it
                    tmpArgVal = ell.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        characterName = tmpArgVal;
                    }

                    tmpArgVal = ell.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        audioPath = tmpArgVal;
                    }

                    // If there is a "synthesize" attribute, store its value
                    tmpArgVal = ell.GetAttribute("synthesize");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        string response = tmpArgVal;
                        if (response.Equals("yes"))
                            synthesizerVoice = true;
                        else
                            synthesizerVoice = false;
                    }

                    // If there is a "keepShowing" attribute, store its value
                    tmpArgVal = ell.GetAttribute("keepShowing");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        string response = tmpArgVal;
                        if (response.Equals("yes"))
                            keepShowingLine = true;
                        else
                            keepShowingLine = false;
                    }

                    // Store the read string into the current node, and then delete the string. The trim is performed so we
                    // don't have to worry with indentations or leading/trailing spaces
                    conversationLine = new ConversationLine(characterName, ell.InnerText);
                    if (audioPath != null && !this.audioPath.Equals(""))
                    {
                        conversationLine.setAudioPath(audioPath);
                    }

                    conversationLine.setSynthesizerVoice(synthesizerVoice);

                    conversationLine.setKeepShowing(keepShowingLine);
                }
                else if (ell.Name == "condition")
                {
                    addline = false;
                    currentConditions = new Conditions();
                    new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                    currentNode.getLine(currentNode.getLineCount() - 1).setConditions(currentConditions);
                }
                else if (ell.Name == "child")
                {
                    addline = false;
                    tmpArgVal = ell.GetAttribute("nodeindex");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        // Get the child node index, and store it
                        int childIndex = int.Parse(tmpArgVal);
                        currentLinks.Add(childIndex);
                    }
                }
                else
                    addline = false;

                if (addline)
                    node.addLine(conversationLine);
            }
        }
    }
}
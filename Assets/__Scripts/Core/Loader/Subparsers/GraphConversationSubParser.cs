using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

/**
 * Class to subparse graph conversations
 */
public class GraphConversationSubParser : SubParser {


    /* Attributes */

    /**
     * Constant for subparsing nothing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing effect tag
     */
    private const int SUBPARSING_EFFECT = 1;

    /**
     * Constant for subparsing conditions
     */
    private const int SUBPARSING_CONDITION = 2;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

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
     * Subparser for the effect
     */
    private SubParser subParser;

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

    /* Methods */

    /**
     * Constructor
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public GraphConversationSubParser(Chapter chapter):base(chapter)
    {
    }

    /*
     * (non-Javadoc)
     * 
     * @see conversationaleditor.xmlparser.ConversationParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {
            // If it is a "graph-conversation" we pick the name, so we can build the conversation later
            if (qName.Equals("graph-conversation"))
            {
                // Store the name
                conversationName = "";
                foreach (KeyValuePair<string, string> entry in attrs)
                    if (entry.Key.Equals("id"))
                        conversationName = entry.Value.ToString();

                graphNodes = new List<ConversationNode>();
                nodeLinks = new List<List<int>>();
            }

            // If it is a node, create a new node
            else if (qName.Equals("dialogue-node") || qName.Equals("option-node"))
            {
                // Create the node depending of the tag
                editorX = editorY = int.MinValue;
                if (qName.Equals("dialogue-node"))
                {
                    foreach (KeyValuePair<string, string> entry in attrs)
                    {
                        //If there is a "waitUserInteraction" attribute, store if the lines will wait until user interacts
                        if (entry.Key.Equals("keepShowing"))
                        {
                            if (entry.Value.ToString().Equals("yes"))
                                keepShowingDialogue = true;
                            else
                                keepShowingDialogue = false;
                        }
                        //If there is a "editor-x" and "editor-y" attributes
                        if (entry.Key.Equals("editor-x"))
                        {
                            editorX = Mathf.Max(0, int.Parse(entry.Value.ToString()));
                        }
                        else

                        if (entry.Key.Equals("editor-y"))
                        {
                            editorY = Mathf.Max(0, int.Parse(entry.Value.ToString()));
                        }

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
                }
                if (qName.Equals("option-node"))
                {
                    foreach (KeyValuePair<string, string> entry in attrs)
                    {
                        //If there is a "random" attribute, store if the options will be random
                        if (entry.Key.Equals("random"))
                        {
                            if (entry.Value.ToString().Equals("yes"))
                                random = true;
                            else
                                random = false;
                        }
                        else
                        //If there is a "keepShowing" attribute, keep the previous conversation line showing
                        if (entry.Key.Equals("keepShowing"))
                        {
                            if (entry.Value.ToString().Equals("yes"))
                                keepShowing = true;
                            else
                                keepShowing = false;
                        }
                        else
                        //If there is a "showUserOption" attribute, identify if show the user response at option node
                        if (entry.Key.Equals("showUserOption"))
                        {
                            if (entry.Value.ToString().Equals("yes"))
                                showUserOption = true;
                            else
                                showUserOption = false;
                        }
                        else
                        //If there is a "showUserOption" attribute, identify if show the user response at option node
                        if (entry.Key.Equals("preListening"))
                        {
                            if (entry.Value.ToString().Equals("yes"))
                                preListening = true;
                            else
                                preListening = false;
                        }
                        else

                        //If there is a "x" and "y" attributes with the position where the option node has to be painted,
                        if (entry.Key.Equals("x"))
                        {
                            x = int.Parse(entry.Value.ToString());
                        }
                        else

                        if (entry.Key.Equals("y"))
                        {
                            y = int.Parse(entry.Value.ToString());
                        }
                        //If there is a "editor-x" and "editor-y" attributes
                        if (entry.Key.Equals("editor-x"))
                        {
                            editorX = Mathf.Max(0, int.Parse(entry.Value.ToString()));
                        }
                        else

                        if (entry.Key.Equals("editor-y"))
                        {
                            editorY = Mathf.Max(0, int.Parse(entry.Value.ToString()));
                        }
                    }

                    currentNode = new OptionConversationNode(random, keepShowing, showUserOption, preListening, x, y);
                    if (editorX > int.MinValue)
                    {
                        currentNode.setEditorX(editorX);
                    }
                    if (editorY > int.MinValue)
                    {
                        currentNode.setEditorY(editorY);
                    }
                }
                // Create a new vector for the links of the current node
                currentLinks = new List<int>();
            }

            // If it is a non-player character line, store the character name and audio path (if present)
            else if (qName.Equals("speak-char"))
            {
                // Set default name to "NPC"
                characterName = "NPC";
                audioPath = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    // If there is a "idTarget" attribute, store it
                    if (entry.Key.Equals("idTarget"))
                        characterName = entry.Value.ToString();

                    // If there is a "uri" attribute, store it as audio path
                    if (entry.Key.Equals("uri"))
                        audioPath = entry.Value.ToString();
                    // If there is a "synthesize" attribute, store its value
                    if (entry.Key.Equals("synthesize"))
                    {
                        string response = entry.Value.ToString();
                        if (response.Equals("yes"))
                            synthesizerVoice = true;
                        else
                            synthesizerVoice = false;
                    }
                    // If there is a "keepShowing" attribute, store its value
                    if (entry.Key.Equals("keepShowing"))
                    {
                        string response = entry.Value.ToString();
                        if (response.Equals("yes"))
                            keepShowingLine = true;
                        else
                            keepShowingLine = false;
                    }
                }
            }

            // If it is a player character line, store the audio path (if present)
            else if (qName.Equals("speak-player"))
            {
                audioPath = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {

                    // If there is a "uri" attribute, store it as audio path
                    if (entry.Key.Equals("uri"))
                        audioPath = entry.Value.ToString();
                    // If there is a "synthesize" attribute, store its value
                    if (entry.Key.Equals("synthesize"))
                    {
                        string response = entry.Value.ToString();
                        if (response.Equals("yes"))
                            synthesizerVoice = true;
                        else
                            synthesizerVoice = false;
                    }
                    // If there is a "keepShowing" attribute, store its value
                    if (entry.Key.Equals("keepShowing"))
                    {
                        string response = entry.Value.ToString();
                        if (response.Equals("yes"))
                            keepShowingLine = true;
                        else
                            keepShowingLine = false;
                    }
                }
            }

            // If it is a node to a child, store the number of the child node
            else if (qName.Equals("child"))
            {
                // Look for the index of the link, and add it
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("nodeindex"))
                    {
                        // Get the child node index, and store it
                        int childIndex = int.Parse(entry.Value.ToString());
                        currentLinks.Add(childIndex);
                    }
                }
            }

            // If it is an effect tag
            else if (qName.Equals("effect"))
            {
                // Create the new effects, and the subparser
                currentEffects = new Effects();
                subParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }// If it is a condition tag, create new conditions and switch the state
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }
        }

        // If we are subparsing an effect, spread the call
        if (subParsing == SUBPARSING_EFFECT || subParsing == SUBPARSING_CONDITION)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
            endElement(namespaceURI, sName, qName);
        }

    }

    /*
     * (non-Javadoc)
     * 
     * @see conversationaleditor.xmlparser.ConversationParser#endElement(java.lang.string, java.lang.string,
     *      java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {
            // If the tag ending is the conversation, create the graph conversation, with the first node of the list
            if (qName.Equals("graph-conversation"))
            {
                setNodeLinks();
                chapter.addConversation(new GraphConversation(conversationName, graphNodes[0]));
            }

            // If a node is closed
            else if (qName.Equals("dialogue-node") || qName.Equals("option-node"))
            {
                // Add the current node to the node list, and the set of children references into the node links
                graphNodes.Add(currentNode);
                nodeLinks.Add(currentLinks);
            }

            // If the tag is a line said by the player, add it to the current node
            else if (qName.Equals("speak-player"))
            {
                // Store the read string into the current node, and then delete the string. The trim is performed so we
                // don't
                // have to worry with indentations or leading/trailing spaces
                conversationLine = new ConversationLine(ConversationLine.PLAYER, currentstring.Trim());
                if (audioPath != null && !this.audioPath.Equals(""))
                {
                    conversationLine.setAudioPath(audioPath);
                }
                if (synthesizerVoice != null)
                    conversationLine.setSynthesizerVoice(synthesizerVoice);

                conversationLine.setKeepShowing(keepShowingLine);

                currentNode.addLine(conversationLine);
            }

            // If the tag is a line said by a non-player character, add it to the current node
            else if (qName.Equals("speak-char"))
            {
                // Store the read string into the current node, and then delete the string. The trim is performed so we
                // don't
                // have to worry with indentations or leading/trailing spaces
                conversationLine = new ConversationLine(characterName, currentstring.Trim());
                if (audioPath != null && !this.audioPath.Equals(""))
                {
                    conversationLine.setAudioPath(audioPath);
                }
                if (synthesizerVoice != null)
                    conversationLine.setSynthesizerVoice(synthesizerVoice);

                conversationLine.setKeepShowing(keepShowingLine);

                currentNode.addLine(conversationLine);
            }

            // Reset the current string
            currentstring = string.Empty;
        }

        // If we are subparsing an effect
        else if (subParsing == SUBPARSING_EFFECT || subParsing == SUBPARSING_CONDITION)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the effect is being closed, insert the effect into the current node
            if (qName.Equals("effect") && subParsing == SUBPARSING_EFFECT)
            {
                currentNode.setEffects(currentEffects);
                subParsing = SUBPARSING_NONE;
            }
            // If the effect is being closed, insert the effect into the current node
            else if (qName.Equals("condition") && subParsing == SUBPARSING_CONDITION)
            {
                conversationLine.setConditions(currentConditions);
                subParsing = SUBPARSING_NONE;
            }
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {

        // If no element is being subparsing
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If an effect is being subparsed, spread the call
        else if (subParsing == SUBPARSING_EFFECT || subParsing == SUBPARSING_CONDITION)
            subParser.characters(buf, offset, len);
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
}

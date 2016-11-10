using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

/**
 * Class to subparse tree conversations
 */
public class TreeConversationSubParser : SubParser
{

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
     * Normal state
     */
    private const int STATE_NORMAL = 0;

    /**
     * Waiting for an option state
     */
    private const int STATE_WAITING_OPTION = 1;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * State of the recognizer
     */
    private int state = STATE_NORMAL;

    /**
     * Stores the different conversations, in trees form
     */
    private Conversation conversation;

    /**
     * Stores the current node
     */
    private ConversationNode currentNode;

    /**
     * Stores the past optional nodes, for back tracking
     */
    private List<ConversationNode> pastOptionNodes;

    /**
     * Current effect (of the current node)
     */
    private Effects currentEffects;

    /**
     * The subparser for the effect
     */
    private SubParser effectSubParser;

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
     * Check if a conversation line must be synthesize
     */

    private bool synthesizerVoice;

    /* Methods */

    /**
     * Constructor
     * 
     * @param chapter
     *            Chapter data to store the readed data
     */
    public TreeConversationSubParser(Chapter chapter) : base(chapter)
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

            // If it is a "conversation" we pick the name, so we can build the tree later
            if (qName.Equals("tree-conversation"))
            {
                // Store the name
                string conversationName = "";
                foreach (KeyValuePair<string, string> entry in attrs)
                    if (entry.Key.Equals("id"))
                        conversationName = entry.Value.ToString();

                // Create a dialogue node (which will be the root node) and add it to a new tree
                // The content of the tree will be built adding nodes directly to the root
                currentNode = new DialogueConversationNode();
                conversation = new TreeConversation(conversationName, currentNode);
                pastOptionNodes = new List<ConversationNode>();
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
                }
            }

            // If it is a point with a set of possible responses, create a new OptionNode
            else if (qName.Equals("response"))
            {

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    //If there is a "random" attribute, store is the options will be random
                    if (entry.Key.Equals("random"))
                    {
                        if (entry.Value.ToString().Equals("yes"))
                            random = true;
                        else
                            random = false;
                    }
                    //If there is a "keepShowing" attribute, keep the previous conversation line showing
                    if (entry.Key.Equals("keepShowing"))
                    {
                        if (entry.Value.ToString().Equals("yes"))
                            keepShowing = true;
                        else
                            keepShowing = false;
                    }
                    //If there is a "showUserOption" attribute, identify if show the user response at option node
                    if (entry.Key.Equals("showUserOption"))
                    {
                        if (entry.Value.ToString().Equals("yes"))
                            showUserOption = true;
                        else
                            showUserOption = false;
                    }

                    //If there is a "showUserOption" attribute, identify if show the user response at option node
                    if (entry.Key.Equals("preListening"))
                    {
                        if (entry.Value.ToString().Equals("yes"))
                            preListening = true;
                        else
                            preListening = false;
                    }

                    //If there is a "x" and "y" attributes with the position where the option node has to be painted,
                    if (entry.Key.Equals("x"))
                    {
                        x = int.Parse(entry.Value.ToString());
                    }

                    if (entry.Key.Equals("y"))
                    {
                        y = int.Parse(entry.Value.ToString());
                    }
                }
                // Create a new OptionNode, and link it to the current node
                ConversationNode nuevoNodoOpcion = new OptionConversationNode(random, keepShowing, showUserOption, preListening, x, y);
                currentNode.addChild(nuevoNodoOpcion);

                // Change the actual node for the option node recently created
                currentNode = nuevoNodoOpcion;
            }

            // If we are about to read an option, change the state of the recognizer, so we can read the line of the
            // option
            else if (qName.Equals("option"))
            {
                state = STATE_WAITING_OPTION;

            }

            // If it is an effect tag, create new effect, new subparser and switch state
            else if (qName.Equals("effect"))
            {
                currentEffects = new Effects();
                effectSubParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }

            // If there is a go back, link the current node (which will be a DialogueNode) with the last OptionNode
            // stored
            else if (qName.Equals("go-back"))
            {
                currentNode.addChild(pastOptionNodes[pastOptionNodes.Count - 1]);
            }
        }

        // If an effect element is being subparsed, spread the call
        if (subParsing == SUBPARSING_EFFECT)
        {
            effectSubParser.startElement(namespaceURI, sName, qName, attrs);
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
            // If the conversation ends, store it in the game data
            if (qName.Equals("tree-conversation"))
            {
                chapter.addConversation(new GraphConversation((TreeConversation)conversation));
            }

            // If the tag is a line said by the player, add it to the current node
            else if (qName.Equals("speak-player"))
            {
                // Store the read string into the current node, and then delete the string. The trim is performed so we
                // don't
                // have to worry with indentations or leading/trailing spaces
                ConversationLine line = new ConversationLine(ConversationLine.PLAYER, currentstring.Trim());
                if (audioPath != null && !this.audioPath.Equals(""))
                {
                    line.setAudioPath(audioPath);
                }
                if (synthesizerVoice != null)
                    line.setSynthesizerVoice(synthesizerVoice);

                currentNode.addLine(line);

                // If we were waiting an option, create a new DialogueNode
                if (state == STATE_WAITING_OPTION)
                {
                    // Create a new DialogueNode, and link it to the current node (which will be a OptionNode)
                    ConversationNode newDialogueNode = new DialogueConversationNode();
                    currentNode.addChild(newDialogueNode);

                    // Add the current node (OptionNode) in the list of past option nodes, and change the current node
                    pastOptionNodes.Add(currentNode);
                    currentNode = newDialogueNode;

                    // Go back to the normal state
                    state = STATE_NORMAL;
                }

            }

            // If the tag is a line said by a non-player character, add it to the current node
            else if (qName.Equals("speak-char"))
            {
                // Store the read string into the current node, and then delete the string. The trim is performed so we
                // don't
                // have to worry with indentations or leading/trailing spaces
                ConversationLine line = new ConversationLine(characterName, currentstring.Trim());
                if (audioPath != null && !this.audioPath.Equals(""))
                {
                    line.setAudioPath(audioPath);
                }

                if (synthesizerVoice != null)
                    line.setSynthesizerVoice(synthesizerVoice);
                currentNode.addLine(line);

            }

            // If an "option" tag ends, go back to keep working on the last OptionNode
            else if (qName.Equals("option"))
            {
                // Se the current node to the last OptionNode stored
                currentNode = pastOptionNodes[pastOptionNodes.Count - 1];
                pastOptionNodes.RemoveAt(pastOptionNodes.Count - 1);
            }

            // Reset the current string
            currentstring = string.Empty;
        }

        // If an effect tag is being subparsed
        else if (subParsing == SUBPARSING_EFFECT)
        {
            // Spread the call
            effectSubParser.endElement(namespaceURI, sName, qName);

            // If the effect is being closed, insert the effect into the current node
            if (qName.Equals("effect"))
            {

                currentNode.setEffects(currentEffects);
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

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If an effect is being subparsed
        else if (subParsing == SUBPARSING_EFFECT)
            effectSubParser.characters(buf, offset, len);
    }
}
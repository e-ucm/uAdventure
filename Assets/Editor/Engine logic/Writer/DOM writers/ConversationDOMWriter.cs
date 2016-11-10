using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ConversationDOMWriter
{

    /**
     * Private constructor.
     */

    private ConversationDOMWriter()
    {

    }

    public static XmlNode buildDOM(Conversation conversation)
    {

        XmlNode conversationNode = null;

        if (conversation.getType() == Conversation.TREE)
            conversationNode = buildTreeConversationDOM((TreeConversation) conversation);
        else if (conversation.getType() == Conversation.GRAPH)
            conversationNode = buildGraphConversationDOM((GraphConversation) conversation);

        return conversationNode;
    }

    protected static XmlNode buildTreeConversationDOM(TreeConversation treeConversation)
    {

        XmlElement rootNode = null;


        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        rootNode = doc.CreateElement("tree-conversation");

        // Set the identification attribute of the new conversation, and its type
        rootNode.SetAttribute("id", treeConversation.getId());

        // Call the recursive function that will create the nodes. We pass the root of the tree, the DOM document,
        // the root DOM node that will be used to add the elements, and a depth level for indentation (2 by default)
        writeNodeInDOM(treeConversation.getRootNode(), rootNode);

        return rootNode;
    }

    /**
     * Recursive function responsible for transforming a node (and its children)
     * into a DOM structure
     * 
     * @param currentNode
     *            Node to be transformed
     * @param rootDOMNode
     *            DOM node in which the elements must be attached
     */

    private static void writeNodeInDOM(ConversationNode currentNode, XmlNode rootDOMNode)
    {

        // Extract the document
        XmlDocument document = rootDOMNode.OwnerDocument;

        // If the node is a DialogueNode write the lines one after another, and then the child (or the mark if it is no
        // child)
        if (currentNode.getType() == ConversationNodeViewEnum.DIALOGUE)
        {

            // For each line of the node
            for (int i = 0; i < currentNode.getLineCount(); i++)
            {
                // Create a phrase element, and extract the actual text line
                XmlElement phrase;
                XmlNode conditionsNode = null;
                ConversationLine line = currentNode.getLine(i);

                // If the line belongs to the player, create a "speak-player" element. Otherwise, if it belongs to a
                // NPC,
                // create a "speak-char" element, which will have an attribute "idTarget" with the name of the
                // non-playable character,
                // if there is no name the attribute won't be written
                if (line.isPlayerLine())
                {
                    phrase = document.CreateElement("speak-player");
                }
                else
                {
                    phrase = document.CreateElement("speak-char");
                    if (!line.getName().Equals("NPC"))
                        phrase.SetAttribute("idTarget", line.getName());
                }

                // Add the line text into the element
                phrase.InnerText = (line.getText());

                //If there is audio track, store it as attribute
                if (line.isValidAudio())
                    phrase.SetAttribute("uri", line.getAudioPath());
                //If there is a synthesizer valid voice, store it as attribute
                if (line.getSynthesizerVoice())
                    phrase.SetAttribute("synthesize", "yes");

                // Add the element to the DOM root
                rootDOMNode.AppendChild(phrase);

                // Create conditions for current effect
                conditionsNode = ConditionsDOMWriter.buildDOM(line.getConditions());
                document.ImportNode(conditionsNode, true);

                // Add conditions associated to that effect
                rootDOMNode.AppendChild(conditionsNode);
            }

            // Check if the node is terminal
            if (currentNode.isTerminal())
            {
                // If it is terminal add a "end-conversation" element
                XmlElement endConversation = document.CreateElement("end-conversation");

                // If the terminal node has an effect, include it into the DOM
                if (currentNode.hasEffects())
                {
                    // Extract the root node
                    XmlNode effect = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, currentNode.getEffects());

                    // Insert it into the DOM
                    document.ImportNode(effect, true);
                    endConversation.AppendChild(effect);
                }

                // Add the "end-conversation" tag into the root
                rootDOMNode.AppendChild(endConversation);
            }
            else
            {
                // If the node isn't terminal, check if it performing a "go-back" (going back to the inmediatly upper
                // OptionNode)
                if (TreeConversation.thereIsGoBackTag(currentNode))
                {
                    // If it is the case, add a "go-back" element
                    rootDOMNode.AppendChild(document.CreateElement("go-back"));
                }
                else
                {
                    // Otherwise, if the node has a child, call the recursive function with the child node, and the same
                    // DOM root node
                    writeNodeInDOM(currentNode.getChild(0), rootDOMNode);
                }
            }
        }

        // If the node is a OptionNode write a "response" element, and inside it a "option" element with its content
        else if (currentNode.getType() == ConversationNodeViewEnum.OPTION)
        {
            // Create the "response" element
            XmlElement response = document.CreateElement("response");
            // Adds a random attribute if "random" is activate in conversation node data
            if (((OptionConversationNode) currentNode).isRandom())
                response.SetAttribute("random", "yes");
            // For each line of the node (we suppose the number of line equals the number of links, or children nodes)
            for (int i = 0; i < currentNode.getLineCount(); i++)
            {
                // Create the "option" element
                XmlElement optionElement = document.CreateElement("option");
                ConversationLine line = currentNode.getLine(i);
                // Create the actual option (a "speak-player" element) and add its respective text
                XmlElement lineElement = document.CreateElement("speak-player");
                lineElement.InnerText = currentNode.getLine(i).getText();

                //If there is audio track, store it as attribute
                if (line.isValidAudio())
                    lineElement.SetAttribute("uri", line.getAudioPath());
                //If there is a synthesizer valid voice, store it as attribute
                if (line.getSynthesizerVoice())
                    lineElement.SetAttribute("synthesize", "yes");

                // Insert the text line in the option node
                optionElement.AppendChild(lineElement);

                // Call the recursive function, to write in the "option" node the appropiate elements
                // Note that the root DOM node is the "option" element
                writeNodeInDOM(currentNode.getChild(i), optionElement);

                // Add the "option" element
                response.AppendChild(optionElement);
            }
            // If the terminal node has an effect, include it into the DOM
            if (currentNode.hasEffects())
            {
                // Extract the root node
                XmlNode effect = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, currentNode.getEffects());

                // Insert it into the DOM
                document.ImportNode(effect, true);
                response.AppendChild(effect);
            }

            // Add the element
            rootDOMNode.AppendChild(response);
        }
    }

    private static XmlNode buildGraphConversationDOM(GraphConversation graphConversation)
    {

        XmlElement conversationElement = null;

        // Get the complete node list
        List<ConversationNode> nodes = graphConversation.getAllNodes();

        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        conversationElement = doc.CreateElement("graph-conversation");
        conversationElement.SetAttribute("id", graphConversation.getId());

        // For each node
        for (int i = 0; i < nodes.Count; i++)
        {
            ConversationNode node = nodes[i];

            XmlElement nodeElement = null;
            XmlNode conditionsNode = null;

            // If the node is a dialogue node
            if (node is DialogueConversationNode)
            {

                // Create the node element and set the nodeindex
                nodeElement = doc.CreateElement("dialogue-node");
                nodeElement.SetAttribute("nodeindex", i.ToString());
                // Adds a random attribute if "keepShowing" is activate in conversation node data
                if (((DialogueConversationNode) node).isKeepShowing())
                    nodeElement.SetAttribute("keepShowing", "yes");
                if (node.getEditorX() != -1)
                {
                    nodeElement.SetAttribute("editor-x", node.getEditorX().ToString());
                }
                if (node.getEditorY() != -1)
                {
                    nodeElement.SetAttribute("editor-y", node.getEditorY().ToString());
                }
                // For each line of the node
                for (int j = 0; j < node.getLineCount(); j++)
                {
                    // Create a phrase element, and extract the actual text line
                    XmlElement phrase;
                    ConversationLine line = node.getLine(j);

                    // If the line belongs to the player, create a "speak-player" element. Otherwise, if it belongs
                    // to a NPC,
                    // create a "speak-char" element, which will have an attribute "idTarget" with the name of the
                    // non-playable character,
                    // if there is no name the attribute won't be written
                    if (line.isPlayerLine())
                    {
                        phrase = doc.CreateElement("speak-player");
                    }
                    else
                    {
                        phrase = doc.CreateElement("speak-char");
                        if (!line.getName().Equals("NPC"))
                            phrase.SetAttribute("idTarget", line.getName());
                    }

                    //If there is audio track, store it as attribute
                    if (line.isValidAudio())
                        phrase.SetAttribute("uri", line.getAudioPath());
                    //If there is a synthesizer valid voice, store it as attribute
                    if (line.getSynthesizerVoice())
                        phrase.SetAttribute("synthesize", "yes");

                    // Add the line text into the element
                    phrase.InnerText = (line.getText());

                    // Add the element to the node
                    nodeElement.AppendChild(phrase);

                    // Create conditions for current effect
                    conditionsNode = ConditionsDOMWriter.buildDOM(line.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    // Add conditions associated to that effect
                    nodeElement.AppendChild(conditionsNode);

                }

                // Check if the node is terminal
                if (node.isTerminal())
                {
                    // If it is terminal add a "end-conversation" element
                    XmlElement endConversation = doc.CreateElement("end-conversation");

                    // If the terminal node has an effect, include it into the DOM
                    if (node.hasEffects())
                    {
                        // Extract the root node
                        XmlNode effect = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, node.getEffects());

                        // Insert it into the DOM
                        doc.ImportNode(effect, true);
                        endConversation.AppendChild(effect);
                    }

                    // Add the "end-conversation" tag into the node
                    nodeElement.AppendChild(endConversation);
                }
                else
                {
                    // Otherwise, if the node has a child, add the element
                    XmlElement childElement = doc.CreateElement("child");

                    // Add the number of the child node (index of the node in the structure)
                    childElement.SetAttribute("nodeindex", nodes.IndexOf(node.getChild(0)).ToString());

                    // Insert the tag into the node
                    nodeElement.AppendChild(childElement);

                    // TODO MODIFIED
                    // If the terminal node has an effect, include it into the DOM
                    if (node.hasEffects())
                    {
                        // Extract the root node
                        XmlNode effect = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, node.getEffects());

                        // Insert it into the DOM
                        doc.ImportNode(effect, true);
                        nodeElement.AppendChild(effect);
                    }

                }
            }

            // If the node is a option node
            if (node is OptionConversationNode)
            {

                // Create the node element and set the nodeindex
                nodeElement = doc.CreateElement("option-node");
                nodeElement.SetAttribute("nodeindex", i.ToString());
                // Adds a random attribute if "random" is activate in conversation node data
                if (((OptionConversationNode) node).isRandom())
                    nodeElement.SetAttribute("random", "yes");
                // Adds a random attribute if "keepShowing" is activate in conversation node data
                if (((OptionConversationNode) node).isKeepShowing())
                    nodeElement.SetAttribute("keepShowing", "yes");
                // Adds a random attribute if "showUserOption" is activate in conversation node data
                if (((OptionConversationNode) node).isShowUserOption())
                    nodeElement.SetAttribute("showUserOption", "yes");
                // Adds a random attribute if "preListening" is activate in conversation node data
                if (((OptionConversationNode) node).isPreListening())
                    nodeElement.SetAttribute("preListening", "yes");
                if (node.getEditorX() != -1)
                {
                    nodeElement.SetAttribute("editor-x", node.getEditorX().ToString());
                }
                if (node.getEditorY() != -1)
                {
                    nodeElement.SetAttribute("editor-y", node.getEditorY().ToString());
                }
                // Adds the x position of the options conversations node
                nodeElement.SetAttribute("x", ((OptionConversationNode) node).getX().ToString());
                // Adds a random attribute if "preListening" is activate in conversation node data
                nodeElement.SetAttribute("y", ((OptionConversationNode) node).getY().ToString());

                // For each line of the node
                for (int j = 0; j < node.getLineCount(); j++)
                {
                    // Take the current conversation line
                    ConversationLine line = node.getLine(j);

                    // Create the actual option (a "speak-player" element) and add its respective text
                    XmlElement lineElement = doc.CreateElement("speak-player");
                    lineElement.InnerText = (node.getLine(j).getText());

                    //If there is audio track, store it as attribute
                    if (line.isValidAudio())
                        lineElement.SetAttribute("uri", line.getAudioPath());
                    //If there is a synthesizer valid voice, store it as attribute
                    if (line.getSynthesizerVoice())
                        lineElement.SetAttribute("synthesize", "yes");

                    // Create conditions for current effect
                    conditionsNode = ConditionsDOMWriter.buildDOM(line.getConditions());
                    doc.ImportNode(conditionsNode, true);

                    // Create a child tag, and set it the index of the child
                    XmlElement childElement = doc.CreateElement("child");
                    childElement.SetAttribute("nodeindex", nodes.IndexOf(node.getChild(j)).ToString());

                    // Insert the text line in the option node
                    nodeElement.AppendChild(lineElement);
                    // Add conditions associated to that effect
                    nodeElement.AppendChild(conditionsNode);
                    // Insert child tag
                    nodeElement.AppendChild(childElement);
                }
                // If node has an effect, include it into the DOM
                if (node.hasEffects())
                {
                    // Extract the root node
                    XmlNode effect = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, node.getEffects());

                    // Insert it into the DOM
                    doc.ImportNode(effect, true);
                    nodeElement.AppendChild(effect);
                }
            }

            // Add the node to the conversation
            conversationElement.AppendChild(nodeElement);
        }


        return conversationElement;
    }
}
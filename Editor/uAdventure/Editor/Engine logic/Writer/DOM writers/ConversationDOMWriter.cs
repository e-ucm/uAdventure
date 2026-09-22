using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;
using System;
using System.Globalization;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Conversation), typeof(TreeConversation), typeof(GraphConversation))]
    public class ConversationDOMWriter : ParametrizedDOMWriter
    {
        
        public ConversationDOMWriter()
        {

        }
        
        protected override string GetElementNameFor(object target)
        {
            var conversation = target as Conversation;

            string type;
            switch (conversation.getType())
            {
                default: type = "conversation"; break;
                case Conversation.TREE: type = "tree-conversation"; break;
                case Conversation.GRAPH: type = "graph-conversation"; break;
            }

            return type;
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var conversation = target as Conversation;

            switch (conversation.getType())
            {
                default: Debug.LogError("Wrong conversation type: " + conversation.getType()); break;
                case Conversation.TREE:  FillNode(node, (TreeConversation)conversation, options);  break;
                case Conversation.GRAPH: FillNode(node, (GraphConversation)conversation, options); break;
            }
        }

        private void FillNode(XmlNode node, TreeConversation treeConversation, params IDOMWriterParam[] options)
        {

            XmlElement rootNode = node as XmlElement;

            // Set the identification attribute of the new conversation, and its type
            rootNode.SetAttribute("id", treeConversation.getId());

            // Call the recursive function that will create the nodes. We pass the root of the tree, the DOM document,
            // the root DOM node that will be used to add the elements, and a depth level for indentation (2 by default)
            WriteNodeInDom(treeConversation.getRootNode(), rootNode, options);
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

        private static void WriteNodeInDom(ConversationNode currentNode, XmlNode rootDOMNode, params IDOMWriterParam[] options)
        {

            // Extract the document
            XmlDocument document = rootDOMNode.OwnerDocument;

            // If the node is a DialogueNode write the lines one after another, and then the child (or the mark if it is no
            // child)
            if (currentNode is DialogueConversationNode)
            {

                // For each line of the node
                for (int i = 0; i < currentNode.getLineCount(); i++)
                {
                    // Create a phrase element, and extract the actual text line
                    XmlElement phrase;
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
                        {
                            phrase.SetAttribute("idTarget", line.getName());
                        }
                    }

                    // Add the line text into the element
                    var text = document.CreateElement("text");
                    text.InnerText = line.getText();
                    phrase.AppendChild(text);

                    // Append the resources
                    foreach (ResourcesUni resources in line.getResources())
                    {
                        XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CONVERSATION_LINE);
                        document.ImportNode(resourcesNode, true);
                        phrase.AppendChild(resourcesNode);
                    }

                    // Add the element to the DOM root
                    rootDOMNode.AppendChild(phrase);

                    // Create conditions for current effect
                    DOMWriterUtility.DOMWrite(rootDOMNode, line.getConditions(), options);
                }

                // Check if the node is terminal
                if (currentNode.isTerminal())
                {
                    // If it is terminal add a "end-conversation" element
                    XmlElement endConversation = document.CreateElement("end-conversation");

                    // Add the "end-conversation" tag into the root
                    rootDOMNode.AppendChild(endConversation);

                    // If the terminal node has an effect, include it into the DOM
                    if (currentNode.hasEffects())
                    {
                        DOMWriterUtility.DOMWrite(endConversation, currentNode.getEffects(), options);
                    }
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
                        WriteNodeInDom(currentNode.getChild(0), rootDOMNode);
                    }
                }
            }

            // If the node is a OptionNode write a "response" element, and inside it a "option" element with its content
            else if (currentNode is OptionConversationNode)
            {
                // Create the "response" element
                XmlElement response = document.CreateElement("response");
                // Adds a random attribute if "random" is activate in conversation node data
                if (((OptionConversationNode)currentNode).isRandom())
                {
                    response.SetAttribute("random", "yes");
                }
                // For each line of the node (we suppose the number of line equals the number of links, or children nodes)
                for (int i = 0; i < currentNode.getLineCount(); i++)
                {
                    // Create the "option" element
                    XmlElement optionElement = document.CreateElement("option");
                    ConversationLine line = currentNode.getLine(i);
                    // Create the actual option (a "speak-player" element) and add its respective text
                    XmlElement lineElement = document.CreateElement("speak-player");

                    var text = document.CreateElement("text");
                    text.InnerText = currentNode.getLine(i).getText();
                    lineElement.AppendChild(text);

                    // Append the resources
                    foreach (ResourcesUni resources in line.getResources())
                    {
                        XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CONVERSATION_LINE);
                        document.ImportNode(resourcesNode, true);
                        lineElement.AppendChild(resourcesNode);
                    }

                    // Insert the text line in the option node
                    optionElement.AppendChild(lineElement);

                    // Call the recursive function, to write in the "option" node the appropiate elements
                    // Note that the root DOM node is the "option" element
                    WriteNodeInDom(currentNode.getChild(i), optionElement);

                    // Add the "option" element
                    response.AppendChild(optionElement);
                }
                // If the terminal node has an effect, include it into the DOM
                if (currentNode.hasEffects())
                {
                    DOMWriterUtility.DOMWrite(response, currentNode.getEffects(), options);
                }

                // Add the element
                rootDOMNode.AppendChild(response);
            }
        }

        private void FillNode(XmlNode toFill, GraphConversation graphConversation, params IDOMWriterParam[] options)
        {

            XmlElement conversationElement = toFill as XmlElement;

            // Get the complete node list
            List<ConversationNode> nodes = graphConversation.getAllNodes();

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            conversationElement.SetAttribute("id", graphConversation.getId());

            // For each node
            for (int i = 0; i < nodes.Count; i++)
            {
                ConversationNode node = nodes[i];

                XmlElement nodeElement = null;
                var dialogConversationNode = node as DialogueConversationNode;

                // If the node is a dialogue node
                if (dialogConversationNode != null)
                {
                    // Create the node element and set the nodeindex
                    nodeElement = doc.CreateElement("dialogue-node");
                    nodeElement.SetAttribute("nodeindex", i.ToString());
                    // Adds a random attribute if "keepShowing" is activate in conversation node data
                    if (dialogConversationNode.isKeepShowing())
                    {
                        nodeElement.SetAttribute("keepShowing", "yes");
                    }
                    if (node.getEditorX() != -1)
                    {
                        nodeElement.SetAttribute("editor-x", node.getEditorX().ToString());
                    }
                    if (node.getEditorY() != -1)
                    {
                        nodeElement.SetAttribute("editor-y", node.getEditorY().ToString());
                    }
                    if (node.getEditorCollapsed())
                    {
                        nodeElement.SetAttribute("editor-collapsed", "yes");
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
                            {
                                phrase.SetAttribute("idTarget", line.getName());
                            }
                        }

                        // Append the resources
                        foreach (ResourcesUni resources in line.getResources())
                        {
                            XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CONVERSATION_LINE);
                            doc.ImportNode(resourcesNode, true);
                            phrase.AppendChild(resourcesNode);
                        }

                        // Add the line text into the element

                        var text = doc.CreateElement("text");
                        text.InnerText = line.getText();
                        phrase.AppendChild(text);

                        // Add the element to the node
                        nodeElement.AppendChild(phrase);

                        // Create conditions for current effect
                        DOMWriterUtility.DOMWrite(nodeElement, line.getConditions(), options);

                    }

                    // Check if the node is terminal
                    if (node.isTerminal())
                    {
                        // If it is terminal add a "end-conversation" element
                        XmlElement endConversation = doc.CreateElement("end-conversation");

                        // If the terminal node has an effect, include it into the DOM
                        if (node.hasEffects())
                        {
                            DOMWriterUtility.DOMWrite(endConversation, node.getEffects(), options);
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

                        // If the terminal node has an effect, include it into the DOM
                        if (node.hasEffects())
                        {
                            DOMWriterUtility.DOMWrite(nodeElement, node.getEffects(), options);
                        }

                    }
                }

                var optionConversationNode = node as OptionConversationNode;
                // If the node is a option node
                if (optionConversationNode != null)
                {
                    // Create the node element and set the nodeindex
                    nodeElement = doc.CreateElement("option-node");
                    nodeElement.SetAttribute("nodeindex", i.ToString());

                    if (!string.IsNullOrEmpty(optionConversationNode.getXApiQuestion()))
                    {
                        nodeElement.SetAttribute("question", optionConversationNode.getXApiQuestion());
                    }

                    // Adds a random attribute if "random" is activate in conversation node data
                    if (optionConversationNode.isRandom())
                    {
                        nodeElement.SetAttribute("random", "yes");
                    }
                    // Adds a random attribute if "keepShowing" is activate in conversation node data
                    if (optionConversationNode.isKeepShowing())
                    {
                        nodeElement.SetAttribute("keepShowing", "yes");
                    }
                    // Adds a random attribute if "showUserOption" is activate in conversation node data
                    if (optionConversationNode.isShowUserOption())
                    {
                        nodeElement.SetAttribute("showUserOption", "yes");
                    }
                    // Adds a random attribute if "preListening" is activate in conversation node data
                    if (optionConversationNode.isPreListening())
                    {
                        nodeElement.SetAttribute("preListening", "yes");
                    }
                    if (optionConversationNode.MaxElemsPerRow != -1)
                    {
                        nodeElement.SetAttribute("max-elements-per-row", optionConversationNode.MaxElemsPerRow.ToString());
                    }
                    if (optionConversationNode.Alignment != TextAnchor.UpperCenter)
                    {
                        nodeElement.SetAttribute("alignment", optionConversationNode.Alignment.ToString());
                    }
                    if (optionConversationNode.Horizontal)
                    {
                        nodeElement.SetAttribute("horizontal", "yes");
                    }
                    if (node.getEditorX() != -1)
                    {
                        nodeElement.SetAttribute("editor-x", node.getEditorX().ToString());
                    }
                    if (node.getEditorY() != -1)
                    {
                        nodeElement.SetAttribute("editor-y", node.getEditorY().ToString());
                    }
                    if (node.getEditorCollapsed())
                    {
                        nodeElement.SetAttribute("editor-collapsed", "yes");
                    }
                    // Adds the x position of the options conversations node
                    nodeElement.SetAttribute("x", optionConversationNode.getEditorX().ToString());
                    // Adds a random attribute if "preListening" is activate in conversation node data
                    nodeElement.SetAttribute("y", optionConversationNode.getEditorY().ToString());

                    // For each line of the node
                    for (int j = 0; j < node.getLineCount(); j++)
                    {
                        // Take the current conversation line
                        ConversationLine line = node.getLine(j);

                        // Create the actual option (a "speak-player" element) and add its respective text
                        XmlElement lineElement = doc.CreateElement("speak-player");

                        var text = doc.CreateElement("text");
                        text.InnerText = node.getLine(j).getText();
                        lineElement.AppendChild(text);

                        // Append the resources
                        foreach (ResourcesUni resources in line.getResources())
                        {
                            XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CONVERSATION_LINE);
                            doc.ImportNode(resourcesNode, true);
                            lineElement.AppendChild(resourcesNode);
                        }

                        if (line.getXApiCorrect())
                        {
                            lineElement.SetAttribute("correct", line.getXApiCorrect().ToString().ToLower());
                        }

                        // Create a child tag, and set it the index of the child
                        XmlElement childElement = doc.CreateElement("child");
                        childElement.SetAttribute("nodeindex", nodes.IndexOf(node.getChild(j)).ToString());


                        // Insert the text line in the option node
                        nodeElement.AppendChild(lineElement);
                        // Add conditions associated to that effect
                        DOMWriterUtility.DOMWrite(nodeElement, line.getConditions(), options);
                        // Insert child tag
                        nodeElement.AppendChild(childElement);
                    }

                    if (optionConversationNode.Timeout >= 0)
                    {
                        // Timeout
                        XmlElement timeoutElement = doc.CreateElement("timeout");
                        timeoutElement.InnerText = optionConversationNode.Timeout.ToString(CultureInfo.InvariantCulture);
                        nodeElement.AppendChild(timeoutElement);
                        // Timeout conditions
                        DOMWriterUtility.DOMWrite(nodeElement, optionConversationNode.TimeoutConditions);

                        // Create a child tag, and set it the index of the child
                        XmlElement timeoutchildElement = doc.CreateElement("child");
                        timeoutchildElement.SetAttribute("nodeindex", nodes.IndexOf(node.getChild(node.getChildCount() - 1)).ToString());
                        nodeElement.AppendChild(timeoutchildElement);
                    }

                    // If node has an effect, include it into the DOM
                    if (node.hasEffects())
                    {
                        DOMWriterUtility.DOMWrite(nodeElement, node.getEffects(), options);
                    }
                }

                // Add the node to the conversation
                conversationElement.AppendChild(nodeElement);
            }
        }
    }
}
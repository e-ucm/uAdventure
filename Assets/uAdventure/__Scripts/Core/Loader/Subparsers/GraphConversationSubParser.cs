using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace uAdventure.Core
{
	[DOMParser("graph-conversation")]
	[DOMParser(typeof(GraphConversation))]
	public class GraphConversationSubParser : IDOMParser
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
         * Name of the last non-player character read, "NPC" is no name were found
         */
        private string characterName;

        /**
         * Check if the options in option node may be random
         */
        private bool random;

        /**
         * Check if the previous line will be showed at options node
         */
        private bool keepShowing = false;

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
         * v1.4 - Graphical Position of nodes in editor
         */
        private int editorX, editorY;
        
        /**
         * uAdventure v1 the editor is now collapsible
         */
        private bool editorCollapsed;

        /**
         * Check if each conversation line will wait until user interacts
         */
        private bool keepShowingDialogue;
        /**
         * Store the current conversation line
         */
        private ConversationLine conversationLine;

		public object DOMParse(XmlElement element, params object[] parameters)
        {
            XmlNode effects;
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
				//If there is a "editor-x" and "editor-y" attributes     
				editorX = Mathf.Max(-1, ExParsers.ParseDefault(el.GetAttribute("editor-x"), -1));
				editorY = Mathf.Max(-1, ExParsers.ParseDefault(el.GetAttribute("editor-y"), -1));
                editorCollapsed = ExString.EqualsDefault(el.GetAttribute("editor-collapsed"), "yes", false);

                //If there is a "waitUserInteraction" attribute, store if the lines will wait until user interacts
                keepShowingDialogue = ExString.EqualsDefault(el.GetAttribute("waitUserInteraction"), "yes", false);

				// Node effects
				end_conversation = el.SelectSingleNode("end-conversation");
                if (end_conversation != null)
                {
                    effects = end_conversation.SelectSingleNode("effect");
                }
                else
                {
                    effects = el.SelectSingleNode("effect");
                }

				var parsedEffects = DOMParserUtility.DOMParse (effects, parameters) as Effects ?? new Effects ();

                if (el.Name == "dialogue-node")
                {
					currentNode = new DialogueConversationNode(keepShowingDialogue);

                }
                else if (el.Name == "option-node")
                {

					random = ExString.EqualsDefault(el.GetAttribute("random"), "yes", false);
					showUserOption = ExString.EqualsDefault(el.GetAttribute("showUserOption"), "yes", false);
                    keepShowing = ExString.EqualsDefault(el.GetAttribute("keepShowing"), "yes", false);
                    preListening = ExString.EqualsDefault(el.GetAttribute("preListening"), "yes", false) || editorX >= 0 || editorY >= 0;

                    var optionConversationNode = new OptionConversationNode(random, keepShowing, showUserOption, preListening)
                    {
                        Horizontal = ExString.EqualsDefault(el.GetAttribute("horizontal"), "yes", false),
                        MaxElemsPerRow = ExParsers.ParseDefault(el.GetAttribute("max-elements-per-row"), -1),
                        Alignment = el.HasAttribute("alignment") ? ExParsers.ParseEnum<TextAnchor>(el.GetAttribute("alignment")) : TextAnchor.UpperCenter
                    };
                    currentNode = optionConversationNode;

                    //XAPI ELEMENTS
                    optionConversationNode.setXApiQuestion(el.GetAttribute("question"));
                    //END OF XAPI
                }

                if (currentNode != null)
                {
                    // Node editor properties
                    currentNode.setEditorX(editorX);
                    currentNode.setEditorY(editorY);
                    currentNode.setEditorCollapsed(editorCollapsed);

                    // Create a new vector for the links of the current node
                    currentLinks = new List<int>();
                    parseLines(currentNode, el, parameters);
                    currentNode.setEffects(parsedEffects);

                    // Add the current node to the node list, and the set of children references into the node links
                    graphNodes.Add(currentNode);
                    nodeLinks.Add(currentLinks);
                }
            }

            try
            {
                setNodeLinks();
            }catch(Exception ex)
            {
                Debug.Log(conversationName);
                throw ex;
            }
            return new GraphConversation(conversationName, graphNodes[0]);
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
                {
                    node.addChild(graphNodes[links[j]]);
                }
            }
        }

		private void parseLines(ConversationNode node, XmlElement lines, params object[] parameters)
        {
            string tmpArgVal = "";
            currentLinks = new List<int>();
            bool addline = true;
            bool timeoutConditions = false;

            foreach (XmlElement ell in lines.ChildNodes)
            {
				addline = true;
				// If there is a "keepShowing" attribute, store its value
				keepShowingLine = ExString.EqualsDefault(ell.GetAttribute("keepShowing"), "yes", false);

                if (ell.Name == "speak-player")
                {
                    // Store the read string into the current node, and then delete the string. The trim is performed so we
                    // don't have to worry with indentations or leading/trailing spaces


                    conversationLine = new ConversationLine(ConversationLine.PLAYER, GetText(ell));
                    conversationLine.setKeepShowing(keepShowingLine);
                    //XAPI ELEMENTS
					conversationLine.setXApiCorrect("true".Equals (ell.GetAttribute("correct")));
                    //END OF XAPI
                    // RESOURCES
                    foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(ell.SelectNodes("resources"), parameters))
                    {
                        conversationLine.addResources(res);
                    }

                }
                else if (ell.Name == "speak-char")
                {
                    // If it is a non-player character line, store the character name and audio path (if present)
                    // Set default name to "NPC"
                    characterName = "NPC";
                    // If there is a "idTarget" attribute, store it
					characterName = ell.GetAttribute("idTarget");

                    // Store the read string into the current node, and then delete the string. The trim is performed so we
                    // don't have to worry with indentations or leading/trailing spaces
                    conversationLine = new ConversationLine(characterName, GetText(ell));
                    conversationLine.setKeepShowing(keepShowingLine);
                    // RESOURCES
                    foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(ell.SelectNodes("resources"), parameters))
                    {
                        conversationLine.addResources(res);
                    }
                }
                else if (ell.Name == "condition")
                {
                    addline = false;
					var currentConditions = DOMParserUtility.DOMParse (ell, parameters) as Conditions ?? new Conditions ();

                    if (timeoutConditions)
                    {
                        ((OptionConversationNode)currentNode).TimeoutConditions = currentConditions;
                    }
                    else
                    {
                        currentNode.getLine(currentNode.getLineCount() - 1).setConditions(currentConditions);
                    }
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
                else if(ell.Name == "timeout")
                {
                    ((OptionConversationNode)currentNode).Timeout = ExParsers.ParseDefault(GetText(ell), CultureInfo.InvariantCulture, 10f);
                    timeoutConditions = true;
                    addline = false;
                }
                else
                {
                    addline = false;
                }

                if (addline)
                {
                    node.addLine(conversationLine);
                }
            }
        }

        private static string GetText(XmlElement el)
        {
            var textNode = el.SelectSingleNode("text");
            var text = "";
            if (textNode != null)
            {
                text = textNode.InnerText;
            }

            return text;
        }
    }
}
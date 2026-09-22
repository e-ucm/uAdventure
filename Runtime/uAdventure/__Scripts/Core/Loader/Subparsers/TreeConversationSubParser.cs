using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace uAdventure.Core
{
    public class TreeConversationSubParser : Subparser
    {
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


        public TreeConversationSubParser(Chapter chapter) : base(chapter)
        {
        }

        public override void ParseElement(XmlElement element)
        {
            XmlNodeList
                speakschar = element.SelectNodes("speak-char"),
                speaksplayer = element.SelectNodes("speak-player"),
                responses = element.SelectNodes("response"),
                options = element.SelectNodes("option"),
                effects = element.SelectNodes("effect"),
                gosback = element.SelectNodes("go-back");


            string tmpArgVal;

            // Store the name
            string conversationName = "";

            tmpArgVal = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                conversationName = tmpArgVal;
            }

            // Create a dialogue node (which will be the root node) and add it to a new tree
            // The content of the tree will be built adding nodes directly to the root
            currentNode = new DialogueConversationNode();
            conversation = new TreeConversation(conversationName, currentNode);
            pastOptionNodes = new List<ConversationNode>();

            foreach (XmlElement el in speakschar)
            {
                // Set default name to "NPC"
                characterName = "NPC";

                // If there is a "idTarget" attribute, store it
                tmpArgVal = el.GetAttribute("idTarget");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    characterName = tmpArgVal;
                }

                // Store the read string into the current node, and then delete the string. The trim is performed so we
                // don't
                // have to worry with indentations or leading/trailing spaces
                ConversationLine line = new ConversationLine(characterName, GetText(el));

                // RESOURCES
                foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(el.SelectNodes("resources")))
                {
                    line.addResources(res);
                }
                currentNode.addLine(line);

            }

            foreach (XmlElement el in speaksplayer)
            {

                // Store the read string into the current node, and then delete the string. The trim is performed so we
                // don't have to worry with indentations or leading/trailing spaces
                ConversationLine line = new ConversationLine(ConversationLine.PLAYER, GetText(el));

                // RESOURCES
                foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(el.SelectNodes("resources")))
                {
                    line.addResources(res);
                }

                currentNode.addLine(line);
            }

            foreach (XmlElement el in responses)
            {

                //If there is a "random" attribute, store is the options will be random
                tmpArgVal = el.GetAttribute("random");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("yes"))
                        random = true;
                    else
                        random = false;
                }

                //If there is a "keepShowing" attribute, keep the previous conversation line showing
                tmpArgVal = el.GetAttribute("keepShowing");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("yes"))
                        keepShowing = true;
                    else
                        keepShowing = false;
                }

                //If there is a "showUserOption" attribute, identify if show the user response at option node
                tmpArgVal = el.GetAttribute("showUserOption");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("yes"))
                        showUserOption = true;
                    else
                        showUserOption = false;
                }


                //If there is a "showUserOption" attribute, identify if show the user response at option node
                tmpArgVal = el.GetAttribute("preListening");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("yes"))
                        preListening = true;
                    else
                        preListening = false;
                }

                //If there is a "x" and "y" attributes with the position where the option node has to be painted,
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
                    x = int.Parse(tmpArgVal);
                }
                tmpArgVal = el.GetAttribute("y");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    y = int.Parse(tmpArgVal);
                }

                // Create a new OptionNode, and link it to the current node
                ConversationNode nuevoNodoOpcion = new OptionConversationNode(random, keepShowing, showUserOption, preListening);
                nuevoNodoOpcion.setEditorX(x);
                nuevoNodoOpcion.setEditorY(y);
                currentNode.addChild(nuevoNodoOpcion);

                // Change the actual node for the option node recently created
                currentNode = nuevoNodoOpcion;
            }
            
            {
                var iterator = options.GetEnumerator();
                while (iterator.MoveNext())
                {
                    currentNode = pastOptionNodes[pastOptionNodes.Count - 1];
                    pastOptionNodes.RemoveAt(pastOptionNodes.Count - 1);
                }
            }

            foreach (XmlElement el in effects)
            {
                currentEffects = new Effects();
                //new EffectSubParser_(currentEffects, chapter).ParseElement(el);
                currentNode.setEffects(currentEffects);
            }
            {
                var i = gosback.GetEnumerator();
                while (i.MoveNext())
                {
                    currentNode.addChild(pastOptionNodes[pastOptionNodes.Count - 1]);
                }
            }

            chapter.addConversation(new GraphConversation((TreeConversation)conversation));
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

        //public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
        //{

        //    // If no element is being subparsed
        //    if (subParsing == SUBPARSING_NONE)
        //    {

        //        // If we are about to read an option, change the state of the recognizer, so we can read the line of the
        //        // option
        //        else if (qName.Equals("option"))
        //        {
        //            state = STATE_WAITING_OPTION;

        //        }


        //    }

        //    // If an effect element is being subparsed, spread the call
        //    if (subParsing == SUBPARSING_EFFECT)
        //    {
        //        effectSubParser.startElement(namespaceURI, sName, qName, attrs);
        //    }
        //}

        ///*
        // * (non-Javadoc)
        // * 
        // * @see conversationaleditor.xmlparser.ConversationParser#endElement(java.lang.string, java.lang.string,
        // *      java.lang.string)
        // */
        //public override void endElement(string namespaceURI, string sName, string qName)
        //{

        //    // If no element is being subparsed
        //    if (subParsing == SUBPARSING_NONE)
        //    {

        //        // If the tag is a line said by the player, add it to the current node
        //        else if (qName.Equals("speak-player"))
        //        {


        //            // If we were waiting an option, create a new DialogueNode
        //            if (state == STATE_WAITING_OPTION)
        //            {
        //                // Create a new DialogueNode, and link it to the current node (which will be a OptionNode)
        //                ConversationNode newDialogueNode = new DialogueConversationNode();
        //                currentNode.addChild(newDialogueNode);

        //                // Add the current node (OptionNode) in the list of past option nodes, and change the current node
        //                pastOptionNodes.Add(currentNode);
        //                currentNode = newDialogueNode;

        //                // Go back to the normal state
        //                state = STATE_NORMAL;
        //            }

        //        }

        //    }

        //    // If an effect tag is being subparsed
        //    else if (subParsing == SUBPARSING_EFFECT)
        //    {
        //        // Spread the call
        //        effectSubParser.endElement(namespaceURI, sName, qName);

        //        // If the effect is being closed, insert the effect into the current node
        //        if (qName.Equals("effect"))
        //        {

        //            currentNode.setEffects(currentEffects);
        //            subParsing = SUBPARSING_NONE;
        //        }
        //    }
        //}
    }
}
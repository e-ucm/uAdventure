using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    public class GraphConversationDataControl : ConversationDataControl
    {
        /**
         * Reference to the graph conversation.
         */
        private GraphConversation graphConversation;

        /**
         * Constructor.
         * 
         * @param graphConversation
         *            Contained graph conversation
         */
        public GraphConversationDataControl(GraphConversation graphConversation) : base(graphConversation)
        {
            this.graphConversation = graphConversation;
        }


        public override int getType()
        {

            return Controller.CONVERSATION_GRAPH;
        }


        public override string getId()
        {

            return graphConversation.getId();
        }


        public override ConversationNodeDataControl getRootNode()
        {
            return getNodeDataControl(graphConversation.getRootNode());
        }

        public override int getConversationLineCount()
        {
            int lineCount = 0;

            // Take all the nodes, and add the line count of each one
            List<ConversationNodeDataControl> nodes = getAllNodes();
            foreach (ConversationNodeDataControl node in nodes)
                lineCount += node.getLineCount();

            return lineCount;
        }


        public override int[] getAddableNodes(ConversationNodeDataControl node)
        {
            return node.getAddableNodes();
        }


        public override bool canAddChild(ConversationNodeDataControl node, int nodeType)
        {

            bool canAddChild = false;
            /*
            // A dialogue node only accepts nodes if it is terminal
            if (node.getType() == ConversationNodeViewEnum.DIALOGUE && node.isTerminal())
                canAddChild = true;

            // An option node only accepts dialogue nodes
            if (node.getType() == ConversationNodeViewEnum.OPTION && nodeType == (int)ConversationNodeViewEnum.DIALOGUE)
                canAddChild = true;
                */
            return canAddChild;
        }


        public override bool canDeleteNode(ConversationNodeDataControl node)
        {
            // Any node can be deleted, if it is not the start node
            return node.getContent() != graphConversation.getRootNode();
        }


        public override bool canMoveNode(ConversationNodeDataControl node)
        {

            // No node moving is allowed in graph conversations
            return false;
        }


        public override bool canMoveNodeTo(ConversationNodeDataControl node, ConversationNodeDataControl hostNodeView)
        {

            // No node moving is allowed in graph conversations
            return false;
        }


        public override bool moveNode(ConversationNodeDataControl node, ConversationNodeDataControl hostNodeView)
        {

            // No node moving is allowed in graph conversations
            return false;
        }

        /**
         * Returns a list with all the nodes in the conversation.
         * 
         * @return List with the nodes of the conversation
         */
        public override List<ConversationNodeDataControl> getAllNodes()
        {

            // Create another list
            List<ConversationNode> nodes = graphConversation.getAllNodes();
            List<ConversationNodeDataControl> dataControls = new List<ConversationNodeDataControl>();

            // Copy the data
            foreach (ConversationNode node in nodes)
                dataControls.Add(getNodeDataControl(node));

            return dataControls;
        }

        /**
         * Returns if the given father has a direct line of dialogue nodes to get to
         * the child node.
         * 
         * @param fatherView
         *            Father node
         * @param childView
         *            Child node
         * @return True if the father is related to child following only dialogue
         *         nodes, false otherwise
         */
        private bool isDirectFather(ConversationNodeDataControl fatherView, ConversationNodeDataControl childView)
        {

            bool isDirectFatherL = false;

            // Check if both nodes are dialogue nodes
            /*if (fatherView.getType() == ConversationNodeViewEnum.DIALOGUE && childView.getType() == ConversationNodeViewEnum.DIALOGUE)
            {

                // Check if the father is not a terminal node
                if (!fatherView.isTerminal())
                {

                    // If the only child of the father equals the child, there is a direct line
                    if (fatherView.getChildView(0) == childView)
                        isDirectFatherL = true;

                    // If not, keep searching with the only child of the father
                    else
                        isDirectFatherL = isDirectFather(fatherView.getChildView(0), childView);
                }
            }*/

            return isDirectFatherL;
        }


        public override System.Object getContent()
        {

            return graphConversation;
        }


        public override string renameElement(string name)
        {
            string oldConversationId = graphConversation.getId();
            string references = controller.countIdentifierReferences(oldConversationId).ToString();

            // Ask for confirmation
            if (name != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameConversationTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldConversationId, references })))
            {

                // Show a dialog asking for the new conversation id
                string newConversationId = name;
                if (name == null)
                    controller.ShowInputDialog(TC.get("Operation.RenameConversationTitle"), TC.get("Operation.RenameConversationMessage"), oldConversationId, (o,s) => performRenameElement(s));
                else
                {
                    return performRenameElement(newConversationId);
                }

            }

			return null;
        }

        private string performRenameElement(string newConversationId)
        {
            string oldConversationId = graphConversation.getId();

            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newConversationId))
                newConversationId = controller.makeElementValid(newConversationId);

            graphConversation.setId(newConversationId);
            controller.replaceIdentifierReferences(oldConversationId, newConversationId);
            controller.IdentifierSummary.deleteId<Conversation>(oldConversationId);
            controller.IdentifierSummary.addId<Conversation>(newConversationId);
            controller.DataModified();
            return newConversationId;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            getAllNodes().ForEach(n => n.updateVarFlagSummary(varFlagSummary));
        }


        public override int countAssetReferences(string assetPath)
        {
            return getAllNodes().Sum(n => n.countAssetReferences(assetPath));
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            getAllNodes().ForEach(n => n.getAssetReferences(assetPaths, assetTypes));
        }


        public override void deleteAssetReferences(string assetPath)
        {
            getAllNodes().ForEach(n => n.deleteAssetReferences(assetPath));
        }


        public override int countIdentifierReferences(string id)
        {
            return getAllNodes().Sum(n => n.countIdentifierReferences(id));
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            getAllNodes().ForEach(n => n.replaceIdentifierReferences(oldId, newId));
        }


        public override void deleteIdentifierReferences(string id)
        {
            getAllNodes().ForEach(n => n.deleteIdentifierReferences(id));
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            check(this.getId(), "ID");
            foreach (ConversationNodeDataControl cnv in this.getAllNodes())
            {
                cnv.recursiveSearch();
            }
        }


        public override Conversation getConversation()
        {

            return graphConversation;
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            List<Searchable> path = getPathFromChild(dataControl, this.getAllNodes().Cast<Searchable>().ToList());
            if (path != null)
                return path;
            if (dataControl == this)
            {
                path = new List<Searchable>();
                path.Add(this);
                return path;
            }
            return null;
        }

        public override void setRootNode()
        {
            throw new NotImplementedException();
        }

        public override bool linkNode(ConversationNodeDataControl fatherView, int nodeType, int position)
        {
            return Controller.Instance.AddTool(new ConversationNodeDataControl.LinkConversationNodeTool(this, fatherView, nodeType, position));
        }

        public override bool linkNode(ConversationNodeDataControl fatherView, ConversationNodeDataControl childView, int position)
        {
            return Controller.Instance.AddTool(new ConversationNodeDataControl.LinkConversationNodeTool(this, fatherView, childView, position));
        }

        public override void setConversation(Conversation conversation)
        {
        }

        public override void updateAllConditions()
        {
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GraphConversationDataControl : ConversationDataControl
{


    /**
     * Reference to the graph conversation.
     */
    private GraphConversation graphConversation;

    /**
     * A list with each conversation line conditions controller
     */
    private Dictionary<ConversationNodeView, System.Collections.Generic.List<ConditionsController>> allConditions;

    /**
     * Constructor.
     * 
     * @param graphConversation
     *            Contained graph conversation
     */
    public GraphConversationDataControl(GraphConversation graphConversation)
    {

        this.graphConversation = graphConversation;
        storeAllConditions();
    }

    
    public override int getType()
    {

        return Controller.CONVERSATION_GRAPH;
    }

    
    public override string getId()
    {

        return graphConversation.getId();
    }

    
    public override ConversationNodeView getRootNode()
    {

        return graphConversation.getRootNode();
    }

    
    public override void updateAllConditions()
    {

        allConditions.Clear();
        List<ConversationNodeView> nodes = getAllNodesViews();
        foreach (ConversationNodeView node in nodes)
        {
            List<ConditionsController> nodeConditions = new List<ConditionsController>();
            // add each condition for each conversation line
            for (int i = 0; i < node.getLineCount(); i++)
            {
                nodeConditions.Add(new ConditionsController(node.getLineConditions(i), (node.getType() == ConversationNodeViewEnum.DIALOGUE ? Controller.CONVERSATION_DIALOGUE_LINE : Controller.CONVERSATION_OPTION_LINE), i.ToString()));
            }
            allConditions.Add(node, nodeConditions);
        }
    }

    /**
     * Store all line conditions in allConditions
     */
    private void storeAllConditions()
    {

        allConditions = new Dictionary<ConversationNodeView, List<ConditionsController>>();
        updateAllConditions();
    }

    /**
     * Returns the conditions controller associated to the given conversation
     * line
     * 
     * @param convLine
     * @return Conditions controller
     * 
     */
    public ConditionsController getLineConditionController(ConversationNodeView node, int line)
    {

        return (allConditions[node])[line];
    }

    
    public override int getConversationLineCount()
    {

        int lineCount = 0;

        // Take all the nodes, and add the line count of each one
        List<ConversationNodeView> nodes = getAllNodesViews();
        foreach (ConversationNodeView node in nodes)
            lineCount += node.getLineCount();

        return lineCount;
    }

    
    public override int[] getAddableNodes(ConversationNodeView nodeView)
    {

        int[] addableNodes = null;

        // Dialogue nodes can add both dialogue and option nodes
        if (nodeView.getType() == ConversationNodeViewEnum.DIALOGUE)
            addableNodes = new int[] { (int) ConversationNodeViewEnum.DIALOGUE, (int)ConversationNodeViewEnum.OPTION };

        // Option nodes can only add dialogue nodes
        else if (nodeView.getType() ==ConversationNodeViewEnum.OPTION)
            addableNodes = new int[] { (int)ConversationNodeViewEnum.DIALOGUE };

        return addableNodes;
    }

    
    public override bool canAddChild(ConversationNodeView nodeView, int nodeType)
    {

        bool canAddChild = false;

        // A dialogue node only accepts nodes if it is terminal
        if (nodeView.getType() == ConversationNodeViewEnum.DIALOGUE && nodeView.isTerminal())
            canAddChild = true;

        // An option node only accepts dialogue nodes
        if (nodeView.getType() ==ConversationNodeViewEnum.OPTION && nodeType == (int)ConversationNodeViewEnum.DIALOGUE)
            canAddChild = true;

        return canAddChild;
    }

    
    public override bool canLinkNode(ConversationNodeView nodeView)
    {

        bool canLinkNode = false;

        // The node must not be the root
        if (nodeView != graphConversation.getRootNode())
        {
            // A dialogue node only can link it it is terminal
            if (nodeView.getType() == ConversationNodeViewEnum.DIALOGUE && nodeView.isTerminal())
                canLinkNode = true;

            // An option node can always link to another node
            if (nodeView.getType() ==ConversationNodeViewEnum.OPTION)
                canLinkNode = true;
        }

        return canLinkNode;
    }

    
    public override bool canDeleteLink(ConversationNodeView nodeView)
    {

        bool canLinkNode = false;

        // The node must not be the root
        if (nodeView != graphConversation.getRootNode())
        {
            // A dialogue node only can link it it is terminal
            if (nodeView.getType() == ConversationNodeViewEnum.DIALOGUE && nodeView.isTerminal())
                canLinkNode = true;

            // An option node can always link to another node
            if (nodeView.getType() ==ConversationNodeViewEnum.OPTION)
                canLinkNode = true;
        }

        return !canLinkNode && this.getAllNodesViews().Count > 1;
    }

    
    public override bool canLinkNodeTo(ConversationNodeView fatherView, ConversationNodeView childView)
    {

        bool canLinkNodeTo = false;

        // Check first if the nodes are different
        if (fatherView != childView)
        {

            // If the father is a dialogue node, it can link to another if it is terminal
            // Check also that the father is not a child of the child node, to prevent cycles
            if (fatherView.getType() == ConversationNodeViewEnum.DIALOGUE && fatherView.isTerminal() && !isDirectFather(childView, fatherView))
                canLinkNodeTo = true;

            // If the father is an option node, it can only link to a dialogue node
            if (fatherView.getType() ==ConversationNodeViewEnum.OPTION && childView.getType() == ConversationNodeViewEnum.DIALOGUE)
                canLinkNodeTo = true;
        }

        return canLinkNodeTo;
    }

    
    public override bool canDeleteNode(ConversationNodeView nodeView)
    {

        // Any node can be deleted, if it is not the start node
        return nodeView != graphConversation.getRootNode();
    }

    
    public override bool canMoveNode(ConversationNodeView nodeView)
    {

        // No node moving is allowed in graph conversations
        return false;
    }

    
    public override bool canMoveNodeTo(ConversationNodeView nodeView, ConversationNodeView hostNodeView)
    {

        // No node moving is allowed in graph conversations
        return false;
    }

    
    public override bool linkNode(ConversationNodeView fatherView, ConversationNodeView childView)
    {

        return controller.addTool(new LinkConversationNodeTool(this, fatherView, childView));
    }

    
    public override bool deleteNode(ConversationNodeView nodeView)
    {

        return controller.addTool(new DeleteConversationNodeTool(nodeView, (GraphConversation)getConversation(), allConditions));
    }

    
    public override bool moveNode(ConversationNodeView nodeView, ConversationNodeView hostNodeView)
    {

        // No node moving is allowed in graph conversations
        return false;
    }

    /**
     * Returns a list with all the nodes in the conversation.
     * 
     * @return List with the nodes of the conversation
     */
    public List<ConversationNodeView> getAllNodesViews()
    {

        // Create another list
        List<ConversationNode> nodes = graphConversation.getAllNodes();
        List<ConversationNodeView> nodeViews = new List<ConversationNodeView>();

        // Copy the data
        foreach (ConversationNode node in nodes)
            nodeViews.Add(node);

        return nodeViews;
    }

    /**
     * Returns a list with all the nodes in the conversation.
     * 
     * @return List with the nodes of the conversation
     */
    public List<SearchableNode> getAllSearchableNodes()
    {

        // Create another list
        List<ConversationNode> nodes = graphConversation.getAllNodes();
        List<SearchableNode> nodeViews = new List<SearchableNode>();

        // Copy the data
        foreach (ConversationNode node in nodes)
            nodeViews.Add(new SearchableNode(node));

        return nodeViews;
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
    private bool isDirectFather(ConversationNodeView fatherView, ConversationNodeView childView)
    {

        bool isDirectFatherL = false;

        // Check if both nodes are dialogue nodes
        if (fatherView.getType() == ConversationNodeViewEnum.DIALOGUE && childView.getType() == ConversationNodeViewEnum.DIALOGUE)
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
        }

        return isDirectFatherL;
    }

    
    public override System.Object getContent()
    {

        return graphConversation;
    }

    
    public override string renameElement(string name)
    {

        bool elementRenamed = false;
        string oldConversationId = graphConversation.getId();
        string references = controller.countIdentifierReferences(oldConversationId).ToString();

        // Ask for confirmation
        if (name != null || controller.showStrictConfirmDialog(TC.get("Operation.RenameConversationTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldConversationId, references })))
        {

            // Show a dialog asking for the new conversation id
            string newConversationId = name;
            if (name == null)
                newConversationId = controller.showInputDialog(TC.get("Operation.RenameConversationTitle"), TC.get("Operation.RenameConversationMessage"), oldConversationId);

            // If some value was typed and the identifiers are different
            if (newConversationId != null && !newConversationId.Equals(oldConversationId) && controller.isElementIdValid(newConversationId))
            {
                graphConversation.setId(newConversationId);
                controller.replaceIdentifierReferences(oldConversationId, newConversationId);
                controller.getIdentifierSummary().deleteConversationId(oldConversationId);
                controller.getIdentifierSummary().addConversationId(newConversationId);
                //controller.dataModified( );
                elementRenamed = true;
            }
        }

        if (elementRenamed)
            return oldConversationId;
        else
            return null;
    }

    
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Update the summary with the effects, if avalaible
            if (conversationNode.hasEffects())
                EffectsController.updateVarFlagSummary(varFlagSummary, conversationNode.getEffects());

            // Update the summary with the conditions of the lines
            for (int i = 0; i < conversationNode.getLineCount(); i++)
            {
                ConditionsController.updateVarFlagSummary(varFlagSummary, conversationNode.getLineConditions(i));
            }
        }
    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        return isValidNode(graphConversation.getRootNode(), currentPath, incidences, new List<ConversationNode>());
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Delete the asset references from the effects, if available
            if (conversationNode.hasEffects())
                count += EffectsController.countAssetReferences(assetPath, conversationNode.getEffects());

            // Count audio paths
            for (int i = 0; i < conversationNode.getLineCount(); i++)
            {
                if (conversationNode.hasAudioPath(i))
                {
                    string audioPath = conversationNode.getAudioPath(i);
                    if (audioPath.Equals(assetPath))
                    {
                        count++;
                    }
                }
            }

        }

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Delete the asset references from the effects, if avalaible
            if (conversationNode.hasEffects())
                EffectsController.getAssetReferences(assetPaths, assetTypes, conversationNode.getEffects());
            // Count audio paths
            for (int i = 0; i < conversationNode.getLineCount(); i++)
            {
                if (conversationNode.hasAudioPath(i))
                {
                    string audioPath = conversationNode.getAudioPath(i);
                    // Search audioPath in the list
                    bool add = true;
                    foreach (string asset in assetPaths)
                    {
                        if (asset.Equals(audioPath))
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        int last = assetPaths.Count;
                        assetPaths.Insert(last, audioPath);
                        assetTypes.Insert(last, AssetsConstants.CATEGORY_AUDIO);
                    }
                }
            }

        }
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Delete the asset references from the effects, if available
            if (conversationNode.hasEffects())
                EffectsController.deleteAssetReferences(assetPath, conversationNode.getEffects());

            // Delete audio paths
            for (int i = 0; i < conversationNode.getLineCount(); i++)
            {
                if (conversationNode.hasAudioPath(i))
                {
                    string audioPath = conversationNode.getAudioPath(i);
                    if (audioPath.Equals(assetPath))
                    {
                        conversationNode.getLine(i).setAudioPath(null);
                    }
                }
            }

        }
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Check only dialogue nodes
            if (conversationNode.getType() == ConversationNodeViewEnum.DIALOGUE)
            {
                // Check all the lines in the node
                for (int i = 0; i < conversationNode.getLineCount(); i++)
                {
                    ConversationLine conversationLine = conversationNode.getLine(i);
                    if (conversationLine.getName().Equals(id))
                        count++;
                }

                // Add the references from the effects
                if (conversationNode.hasEffects())
                    count += EffectsController.countIdentifierReferences(id, conversationNode.getEffects());

            }
        }

        // add conditions references
        foreach (List<ConditionsController> conditions in allConditions.Values)
            foreach (ConditionsController condition in conditions)
                count += condition.countIdentifierReferences(id);

        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Check only dialogue nodes
            if (conversationNode.getType() == ConversationNodeViewEnum.DIALOGUE)
            {
                // Check all the lines in the node, and replace the identifier if necessary
                for (int i = 0; i < conversationNode.getLineCount(); i++)
                {
                    ConversationLine conversationLine = conversationNode.getLine(i);
                    if (conversationLine.getName().Equals(oldId))
                        conversationLine.setName(newId);
                }

                // Replace the references from the effects
                if (conversationNode.hasEffects())
                    EffectsController.replaceIdentifierReferences(oldId, newId, conversationNode.getEffects());
            }

            // add conditions references
            foreach (List<ConditionsController> conditions in allConditions.Values)
                foreach (ConditionsController condition in conditions)
                    condition.replaceIdentifierReferences(oldId, newId);
        }
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        // Check every node on the conversation
        List<ConversationNode> conversationNodes = graphConversation.getAllNodes();
        foreach (ConversationNode conversationNode in conversationNodes)
        {
            // Check only dialogue nodes
            if (conversationNode.getType() == ConversationNodeViewEnum.DIALOGUE)
            {
                // Check all the lines in the node, and replace the identifier if necessary
                int i = 0;
                while (i < conversationNode.getLineCount())
                {
                    if (conversationNode.getLine(i).getName().Equals(id))
                        conversationNode.removeLine(i);
                    else
                        i++;
                }

                // Replace the references from the effects
                if (conversationNode.hasEffects())
                    EffectsController.deleteIdentifierReferences(id, conversationNode.getEffects());
            }

            // add conditions references
            foreach (List<ConditionsController> conditions in allConditions.Values)
                foreach (ConditionsController condition in conditions)
                    condition.deleteIdentifierReferences(id);
        }
    }

    
    public override bool canBeDuplicated()
    {

        return true;
    }

    
    public override void recursiveSearch()
    {

        check(this.getId(), "ID");
        foreach (SearchableNode cnv in this.getAllSearchableNodes())
        {
            cnv.recursiveSearch();
        }
    }

    
    public override Conversation getConversation()
    {

        return graphConversation;
    }

    
    public override void setConversation(Conversation conversation)
    {

        if (conversation is GraphConversation ) {
            graphConversation = (GraphConversation)conversation;
        }
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        List<Searchable> path = getPathFromChild(dataControl, this.getAllSearchableNodes().Cast<Searchable>().ToList());
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

    /**
     * @return the allConditions
     */
    public Dictionary<ConversationNodeView, List<ConditionsController>> getAllConditions()
    {

        return allConditions;
    }

    
    public override List<ConversationNodeView> getAllNodes()
    {

        return this.getAllNodesViews();
    }

}

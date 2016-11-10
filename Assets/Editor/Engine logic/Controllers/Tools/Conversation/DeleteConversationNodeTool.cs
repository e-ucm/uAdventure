using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteConversationNodeTool : Tool {

    public const int MODE_GRAPH = 1;

    public const int MODE_TREE = 2;

    private bool showConfirmation;

    private int mode;

    private Controller controller;

    private ConversationNodeView nodeView;

    private ConversationNode rootNode;

    private Conversation conversation;

    //// DATA TO BE GENERATED DURING DOTOOL
    private ConversationLine deletedLine;

    private ConversationNode deletedNode;

    private ConversationNode parentNode;

    /**
     * The condition controllers associated to the deleted node
     */
    private List<ConditionsController> deletedConditions;

    /**
     * The condition associated to deleted conversation line (if deleted node is
     * "option node")
     */
    private ConditionsController deletedCondition;

    private int index;

    private Dictionary<ConversationNodeView, List<ConditionsController>> allConditions;

    public DeleteConversationNodeTool(int mode, ConversationNodeView _nodeView, Conversation conversation, Dictionary<ConversationNodeView, List<ConditionsController>> allConditions)
    {

        this.mode = mode;
        this.showConfirmation = (mode == MODE_TREE);
        this.controller = Controller.getInstance();
        this.nodeView = _nodeView;
        this.conversation = conversation;
        this.rootNode = conversation.getRootNode();
        this.allConditions = allConditions;
    }

    public DeleteConversationNodeTool(ConversationNodeView _nodeView, GraphConversation conversation, Dictionary<ConversationNodeView, List<ConditionsController>> allConditions):this(MODE_GRAPH, _nodeView, conversation, allConditions)
    {}

    
    public override bool canRedo()
    {

        return true;
    }

    
    public override bool canUndo()
    {

        return deletedNode != null && mode != MODE_TREE;
    }

    
    public override bool combine(Tool other)
    {

        return false;
    }

    
    public override bool doTool()
    {

        bool nodeDeleted = false;

        // Ask for confirmation
        if (!showConfirmation || controller.showStrictConfirmDialog(TC.get("Conversation.OperationDeleteNode"), TC.get("Conversation.ConfirmDeleteNode")))
        {
            // Take the complete node
            ConversationNode node = (ConversationNode)nodeView;

            // If the node was deleted
            if (mode == MODE_TREE)
            {
                if (recursiveDeleteNode(rootNode/*treeConversation.getRootNode( )*/, node))
                {
                    // Set the data as modified
                    nodeDeleted = true;
                }
            }
            else if (mode == MODE_GRAPH)
            {
                // Get the complete node list
                List<ConversationNodeView> nodes = getAllNodes(conversation);

                // For each node
                foreach (ConversationNodeView currentNodeView in nodes)
                {
                    int j = 0;

                    // Search for the node which is being deleted among each node's children
                    while (j < currentNodeView.getChildCount())
                    {

                        // If the current child is the node we want to delete
                        if (currentNodeView.getChildView(j) == nodeView)
                        {
                            // Take the complete current node
                            parentNode = (ConversationNode)currentNodeView;

                            // Delete the child
                            deletedNode = parentNode.removeChild(j);
                            index = j;

                            // remove the conditions associated to removed node
                            deletedConditions = allConditions[deletedNode];
                            allConditions.Remove((ConversationNode)deletedNode);

                            // If the current node is an option node, delete the line too
                            if (parentNode.getType() == ConversationNodeViewEnum.OPTION)
                            {
                                deletedLine = parentNode.removeLine(j);
                                // delete the associated condition data control
                                deletedCondition = allConditions[parentNode][j];
                                allConditions[parentNode].RemoveAt(j);
                            }

                            // The node has been deleted
                            nodeDeleted = true;
                        }

                        // If it's not, go for the next child
                        else
                            j++;
                    }
                }
            }
        }

        return nodeDeleted;
    }

    
    public override bool redoTool()
    {

        parentNode.removeChild(index);
        allConditions.Remove(deletedNode);
        // If the current node is an option node, delete the line too
        if (parentNode.getType() == ConversationNodeViewEnum.OPTION)
        {
            parentNode.removeLine(index);
            allConditions[parentNode].RemoveAt(index);
        }

        controller.updatePanel();
        return true;

    }

    
    public override bool undoTool()
    {

        parentNode.addChild(index, deletedNode);
        allConditions.Add(deletedNode, deletedConditions);
        // If the current node is an option node, delete the line too
        if (parentNode.getType() == ConversationNodeViewEnum.OPTION)
        {
            parentNode.addLine(index, deletedLine);
            allConditions[deletedNode].Insert(index, deletedCondition);
        }

        controller.reloadPanel();
        return true;
    }

    /**
     * Recursive function that deletes the references of nodeToDelete in node
     * and its children.
     * 
     * @param node
     *            Node to check for references to the node being deleted
     * @param nodeToDelete
     *            Reference to the node that is being deleted
     * @return True if the node to delete was found and deleted, false otherwise
     */
    private bool recursiveDeleteNode(ConversationNode node, ConversationNode nodeToDelete)
    {

        bool isDeleted = false;

        // If it is a dialogue node
        if (node.getType() == ConversationNodeViewEnum.DIALOGUE)
        {
            // If the node has a valid child
            if (!node.isTerminal() && !TreeConversation.thereIsGoBackTag(node))
            {
                // If the child equals the node to be deleted, delete the child
                if (node.getChild(0) == nodeToDelete)
                {
                    node.removeChild(0);
                    isDeleted = true;
                }

                // If not, call the function with the child of the current node
                else
                    isDeleted = recursiveDeleteNode(node.getChild(0), nodeToDelete);
            }
        }

        // If the node is a option node
        else if (node.getType() == ConversationNodeViewEnum.OPTION)
        {
            int i = 0;

            // For each child
            while (i < node.getChildCount())
            {
                // If the child equals the node to be deleted, delete the child and its line
                if (node.getChild(i) == nodeToDelete)
                {
                    node.removeChild(i);
                    node.removeLine(i);
                    isDeleted = true;
                }

                // If not, make a recursive call with the current child, and increase i
                else {
                    isDeleted = isDeleted || recursiveDeleteNode(node.getChild(i), nodeToDelete);
                    i++;
                }
            }
        }

        return isDeleted;
    }

    /**
     * Returns a list with all the nodes in the conversation.
     * 
     * @return List with the nodes of the conversation
     */
    public List<ConversationNodeView> getAllNodes(Conversation conversation)
    {

        // Create another list
        List<ConversationNode> nodes = conversation.getAllNodes();
        List<ConversationNodeView> nodeViews = new List<ConversationNodeView>();

        // Copy the data
        foreach (ConversationNode node in nodes)
            nodeViews.Add(node);

        return nodeViews;
    }
}

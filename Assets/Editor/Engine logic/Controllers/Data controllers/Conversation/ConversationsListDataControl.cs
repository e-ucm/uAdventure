using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConversationsListDataControl : DataControl
{

    /**
     * List of conversations.
     */
    private List<Conversation> conversationsList;

    /**
     * List of conversation controllers.
     */
    private List<ConversationDataControl> conversationsDataControlList;

    /**
     * Constructor.
     * 
     * @param conversationsList
     *            List of conversations
     */
    public ConversationsListDataControl(List<Conversation> conversationsList)
    {

        this.conversationsList = conversationsList;

        // Create the subcontrollers
        conversationsDataControlList = new List<ConversationDataControl>();
        foreach (Conversation conversation in conversationsList)
        {
            if (conversation.getType() == Conversation.GRAPH)
                conversationsDataControlList.Add(new GraphConversationDataControl((GraphConversation)conversation));
        }
    }

    /**
     * Returns the list of conversation controllers.
     * 
     * @return Conversation controllers
     */
    public List<ConversationDataControl> getConversations()
    {

        return conversationsDataControlList;
    }

    /**
     * Returns the last conversation controller from the list.
     * 
     * @return Last conversation controller
     */
    public ConversationDataControl getLastConversation()
    {

        return conversationsDataControlList[conversationsDataControlList.Count - 1];
    }

    public string[] getConversationsIDs()
    {
        string [] tmp = new string[conversationsList.Count];
        for (int i = 0; i < conversationsList.Count; i++)
            tmp[i] = conversationsList[i].getId();

        return tmp;
    }

    public override System.Object getContent()
    {

        return conversationsList;
    }

    
    public override int[] getAddableElements()
    {

        return new int[] { Controller.CONVERSATION_GRAPH };
    }

    
    public override bool canAddElement(int type)
    {

        // It can always add new characters
        return type == Controller.CONVERSATION_GRAPH;
    }

    
    public override bool canBeDeleted()
    {

        return false;
    }

    
    public override bool canBeMoved()
    {

        return false;
    }

    
    public override bool canBeRenamed()
    {

        return false;
    }

    
    public override bool addElement(int type, string conversationId)
    {

        bool elementAdded = false;

        if (type == Controller.CONVERSATION_GRAPH)
        {

            // Show a dialog asking for the conversation id
            if (conversationId == null || conversationId.Equals(""))
                conversationId = controller.showInputDialog(TC.get("Operation.AddConversationTitle"), TC.get("Operation.AddConversationMessage"), TC.get("Operation.AddConversationDefaultValue"));

            // If some value was typed and the identifier is valid
            if (conversationId != null && controller.isElementIdValid(conversationId))
            {
                Conversation newConversation = new GraphConversation(conversationId);
                ConversationDataControl newConversationDataControl = new GraphConversationDataControl((GraphConversation)newConversation);

                // Add the new conversation
                conversationsList.Add(newConversation);
                conversationsDataControlList.Add(newConversationDataControl);
                controller.getIdentifierSummary().addConversationId(conversationId);
                //controller.dataModified( );
                elementAdded = true;
            }
        }

        return elementAdded;
    }

    
    public override bool duplicateElement(DataControl dataControl)
    {

        if (!(dataControl is GraphConversationDataControl ) )
            return false;

        //try
        //{
            GraphConversation newElement = (GraphConversation)(((GraphConversation)(dataControl.getContent())).Clone());
            string id = newElement.getId();
            int i = 1;
            do
            {
                id = newElement.getId() + i;
                i++;
            } while (!controller.isElementIdValid(id, false));
            newElement.setId(id);
            conversationsList.Add(newElement);
            conversationsDataControlList.Add(new GraphConversationDataControl(newElement));
            controller.getIdentifierSummary().addConversationId(id);

            string oldId = ((GraphConversation)(dataControl.getContent())).getId();
            /*bool posConfigured = ConversationConfigData.isConversationConfig( oldId );
            if (posConfigured) {
                for (int j = 0; j < newElement.getAllNodes( ).size( ); j++) {
                    int centerX = ConversationConfigData.getNodeX( oldId, j
                            );
                    int centerY = ConversationConfigData.getNodeY( oldId, j );
                    ConversationConfigData.setNodeX( id, j, centerX );
                    ConversationConfigData.setNodeY( id, j, centerY );
                }
            }*/
            GraphConversationDataControl g = (GraphConversationDataControl)dataControl;
            for (int j = 0; j < g.getAllNodes().Count; j++)
            {

                int centerX = g.getEditorX(g.getAllNodes()[j]);
                int centerY = g.getEditorY(g.getAllNodes()[j]);
                newElement.getAllNodes()[j].setEditorX(centerX);
                newElement.getAllNodes()[j].setEditorY(centerY);
            }


            return true;
    }

    
    public override string getDefaultId(int type)
    {

        return TC.get("Operation.AddConversationDefaultValue");
    }

    
    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;
        string conversationId = ((ConversationDataControl)dataControl).getId();
        string references = controller.countIdentifierReferences(conversationId).ToString();

        // Ask for confirmation
        if (!askConfirmation || controller.showStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { conversationId, references })))
        {
            if (conversationsList.Remove((Conversation)dataControl.getContent()))
            {
                conversationsDataControlList.Remove((ConversationDataControl)dataControl);
                controller.deleteIdentifierReferences(conversationId);
                controller.getIdentifierSummary().deleteConversationId(conversationId);
                //controller.dataModified( );
                elementDeleted = true;
            }
        }

        return elementDeleted;
    }

    
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = conversationsList.IndexOf((Conversation)dataControl.getContent());

        if (elementIndex > 0)
        {
            Conversation e = conversationsList[elementIndex];
            ConversationDataControl c = conversationsDataControlList[elementIndex];
            conversationsList.RemoveAt(elementIndex);
            conversationsDataControlList.RemoveAt(elementIndex);
            conversationsList.Insert(elementIndex - 1, e);
            conversationsDataControlList.Insert(elementIndex - 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

    
    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = conversationsList.IndexOf((Conversation)dataControl.getContent());

        if (elementIndex < conversationsList.Count - 1)
        {
            Conversation e = conversationsList[elementIndex];
            ConversationDataControl c = conversationsDataControlList[elementIndex];
            conversationsList.RemoveAt(elementIndex);
            conversationsDataControlList.RemoveAt(elementIndex);
            conversationsList.Insert(elementIndex + 1, e);
            conversationsDataControlList.Insert(elementIndex + 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

    
    public override string renameElement(string name)
    {

        return null;
    }

    
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

        // Iterate through each conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            conversationDataControl.updateVarFlagSummary(varFlagSummary);
    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Update the current path
        currentPath += " >> " + TC.getElement(Controller.CONVERSATIONS_LIST);

        // Iterate through the conversations
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
        {
            string conversationPath = currentPath + " >> " + conversationDataControl.getId();
            valid &= conversationDataControl.isValid(conversationPath, incidences);
        }

        return valid;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through each conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            count += conversationDataControl.countAssetReferences(assetPath);

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Iterate through each conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            conversationDataControl.getAssetReferences(assetPaths, assetTypes);
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through each conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            conversationDataControl.deleteAssetReferences(assetPath);
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Iterate through each conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            count += conversationDataControl.countIdentifierReferences(id);

        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through each conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            conversationDataControl.replaceIdentifierReferences(oldId, newId);
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        // Spread the call to every conversation
        foreach (ConversationDataControl conversationDataControl in conversationsDataControlList)
            conversationDataControl.deleteIdentifierReferences(id);
    }

    
    public override bool canBeDuplicated()
    {

        return false;
    }

    /**
     * Sets all the effects of all the conversations to notConsumed. This is
     * indispensable for the RUN option to work properly. If this is not invoked
     * before debugRun() effects might not get executed
     */
    public void resetAllConversationNodes()
    {

        foreach (Conversation convData in conversationsList)
        {
            foreach (ConversationNode node in convData.getAllNodes())
            {
                node.resetEffect();
            }

        }
    }

    
    public override void recursiveSearch()
    {

        foreach (ConversationDataControl dc in this.conversationsDataControlList)
            dc.recursiveSearch();
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return getPathFromChild(dataControl, conversationsDataControlList.Cast<Searchable>().ToList());
    }
}

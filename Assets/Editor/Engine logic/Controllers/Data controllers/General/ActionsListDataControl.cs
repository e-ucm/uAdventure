using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActionsListDataControl : DataControl
{

    /**
   * List of actions.
   */
    private List<Action> actionsList;

    /**
     * List of action controllers.
     */
    private List<ActionDataControl> actionsDataControlList;

    private DataControl parent;

    /**
     * Constructor.
     * 
     * @param actionsList
     *            List of actions
     */
    public ActionsListDataControl(List<Action> actionsList, DataControl parent)
    {

        this.actionsList = actionsList;
        this.parent = parent;

        // Create subcontrollers
        actionsDataControlList = new List<ActionDataControl>();
        foreach (Action action in actionsList)
        {
            if (action.getType() == Action.CUSTOM || action.getType() == Action.CUSTOM_INTERACT)
            {
                actionsDataControlList.Add(new CustomActionDataControl((CustomAction)action));
            }
            else if (action.getType() == Action.TALK_TO)
                actionsDataControlList.Add(new ActionDataControl(action, ((NPCDataControl)parent).getId()));
            else
                actionsDataControlList.Add(new ActionDataControl(action));
        }
    }

    /**
     * Returns the list of action controllers.
     * 
     * @return Action controllers
     */
    public List<ActionDataControl> getActions()
    {

        return actionsDataControlList;
    }

    /**
     * Returns the last action controller of the list.
     * 
     * @return Last action controller
     */
    public ActionDataControl getLastAction()
    {

        return actionsDataControlList[actionsDataControlList.Count - 1];
    }

    /**
     * Returns the info of the actions contained in the list.
     * 
     * @return Array with the information of the actions. It contains the type
     *         of the action, and information about whether they have conditions
     *         and effects
     */
    public string[][] getActionsInfo()
    {

        string[][] actionsInfo = null;

        // Create the list for the actions
        actionsInfo = new string[actionsList.Count][];
        for (int i = 0; i < actionsList.Count; i++)
            actionsInfo[i] = new string[3];

        // Fill the array with the info
        for (int i = 0; i < actionsList.Count; i++)
        {
            Action action = actionsList[i];

            if (action.getType() == Action.EXAMINE)
                actionsInfo[i][0] = TC.get("ActionsList.ExamineAction");
            else if (action.getType() == Action.GRAB)
                actionsInfo[i][0] = TC.get("ActionsList.GrabAction");
            else if (action.getType() == Action.CUSTOM)
                actionsInfo[i][0] = TC.get("ActionsList.CustomAction", ((CustomAction)action).getName());
            else if (action.getType() == Action.GIVE_TO)
                actionsInfo[i][0] = TC.get("ActionsList.GiveToAction", action.getTargetId());
            else if (action.getType() == Action.USE_WITH)
                actionsInfo[i][0] = TC.get("ActionsList.UseWithAction", action.getTargetId());
            else if (action.getType() == Action.CUSTOM_INTERACT)
                actionsInfo[i][0] = TC.get("ActionsList.CustomInteractAction", action.getTargetId());
            else if (action.getType() == Action.USE)
                actionsInfo[i][0] = TC.get("ActionsList.UseAction");
            else if (action.getType() == Action.TALK_TO)
                actionsInfo[i][0] = TC.get("ActionsList.TalkToAction");
            else if (action.getType() == Action.DRAG_TO)
                actionsInfo[i][0] = TC.get("ActionsList.DragToAction");

            if (action.getConditions().isEmpty())
                actionsInfo[i][1] = TC.get("GeneralText.No");
            else
                actionsInfo[i][1] = TC.get("GeneralText.Yes");

            if (action.getEffects().isEmpty())
                actionsInfo[i][2] = TC.get("GeneralText.No");
            else
                actionsInfo[i][2] = TC.get("GeneralText.Yes");
        }

        return actionsInfo;
    }

    
    public override System.Object getContent()
    {

        return actionsList;
    }

    
    public override int[] getAddableElements()
    {

        if (parent is ItemDataControl )
            return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_GRAB, Controller.ACTION_USE, Controller.ACTION_CUSTOM_INTERACT, Controller.ACTION_USE_WITH, Controller.ACTION_GIVE_TO, Controller.ACTION_DRAG_TO };
        if (parent is NPCDataControl )
            return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_USE, Controller.ACTION_CUSTOM, Controller.ACTION_TALK_TO, Controller.ACTION_DRAG_TO };
        if (parent is ActiveAreaDataControl )
            return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_GRAB, Controller.ACTION_USE, Controller.ACTION_CUSTOM_INTERACT };
        return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_GRAB, Controller.ACTION_USE, Controller.ACTION_CUSTOM, Controller.ACTION_USE_WITH, Controller.ACTION_GIVE_TO, Controller.ACTION_TALK_TO };
    }

    
    public override bool canAddElement(int type)
    {

        return type == Controller.ACTION_EXAMINE || type == Controller.ACTION_GRAB || type == Controller.ACTION_USE || type == Controller.ACTION_CUSTOM || type == Controller.ACTION_USE_WITH || type == Controller.ACTION_GIVE_TO || type == Controller.ACTION_CUSTOM_INTERACT || type == Controller.ACTION_TALK_TO || type == Controller.ACTION_DRAG_TO;
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

    
    public override bool addElement(int type, string id)
    {

        Action newAction = null;

        if (type == Controller.ACTION_EXAMINE)
            newAction = new Action(Action.EXAMINE);

        else if (type == Controller.ACTION_GRAB)
            newAction = new Action(Action.GRAB);

        else if (type == Controller.ACTION_USE)
            newAction = new Action(Action.USE);

        else if (type == Controller.ACTION_TALK_TO)
        {
            string[] conversations = controller.getIdentifierSummary().getConversationsIds();
            if (conversations.Length > 0)
            {
                string selectedElement = controller.showInputDialog(TC.get("Action.OperationAddAction"), TC.get("TalkToAction.MessageSelectConversation"), conversations);
                if (selectedElement != null)
                {
                    newAction = new Action(Action.TALK_TO);
                    newAction.getEffects().add(new TriggerConversationEffect(selectedElement));
                }
            }
            else
                controller.showErrorDialog(TC.get("Action.OperationAddAction"), TC.get("Action.ErrorNoItems"));

        }
        else if (type == Controller.ACTION_CUSTOM_INTERACT)
        {
            //FIX: 
            //string name = JOptionPane.showInputDialog( null, TC.get( "CustomAction.GetNameMessage" ), TC.get( "CustomAction.GetNameTitle" ), JOptionPane.QUESTION_MESSAGE );


            string name = controller.showInputDialog(TC.get("CustomAction.GetNameTitle"), TC.get("CustomAction.GetNameMessage"));
            // if user cancels the operation, finish the new action creation
            /*if( name == null || name.Equals( "" ) ) {
                name = "NONAME_" + ( new Random( ) ).nextInt( 1000 );
            }*/

            // if user do not cancel the operation
            if (name != null)
            {

                // if user press "accept" without introduce any name
                if (name.Equals(""))
                    name = "NONAME_" + (new System.Random()).Next(1000);


                string[] options = { TC.get("Element.Action"), TC.get("Element.Interaction") };
                // TODO: implementation
                //int option = JOptionPane.showOptionDialog(null, TC.get("CustomAction.SelectTypeMessage"), TC.get("CustomAction.SelectTypeTitle"), JOptionPane.DEFAULT_OPTION, JOptionPane.QUESTION_MESSAGE, null, options, 0);
                //if (option != JOptionPane.CLOSED_OPTION)
                //{
                //    if (option == 0)
                //    {
                //        newAction = new CustomAction(Action.CUSTOM);
                //        ((CustomAction)newAction).setName(name);
                //    }
                //    else {
                //        string[] items = controller.getIdentifierSummary().getItemAndActiveAreaIds();
                //        string[] npcs = controller.getIdentifierSummary().getNPCIds();
                //        string[] elements = new string[items.Length + npcs.Length];
                //        for (int i = 0; i < elements.Length; i++)
                //        {
                //            if (i < items.Length)
                //            {
                //                elements[i] = items[i];
                //            }
                //            else {
                //                elements[i] = npcs[i - items.Length];
                //            }
                //        }

                //        // If the list has elements, show the dialog with the options
                //        if (elements.Length > 0)
                //        {
                //            string selectedElement = controller.showInputDialog(TC.get("Action.OperationAddAction"), TC.get("CustomAction.MessageSelectInteraction"), elements);

                //            // If some value was selected
                //            if (selectedElement != null)
                //            {
                //                newAction = new CustomAction(Action.CUSTOM_INTERACT, selectedElement);
                //                ((CustomAction)newAction).setName(name);
                //            }
                //        }

                //        // If the list had no elements, show an error dialog
                //        else
                //            controller.showErrorDialog(TC.get("Action.OperationAddAction"), TC.get("Action.ErrorNoItems"));

                //    }
                //}// end if user cancel the action adition after introducing the name
            }// end if that controls if user
        }
        else if (type == Controller.ACTION_CUSTOM)
        {
            string name = controller.showInputDialog(TC.get("CustomAction.GetNameMessage"), TC.get("CustomAction.GetNameTitle"));
            // if name == null, the user cancel the action addition
            if (name != null)
            {
                if (name.Equals(""))
                {
                    name = "NONAME_" + (new System.Random()).Next(1000);
                }

                newAction = new CustomAction(Action.CUSTOM);
                ((CustomAction)newAction).setName(name);
            }
        }

        // If the type of action is use-with, we must ask for a second item
        else if (type == Controller.ACTION_USE_WITH)
        {
            // Take the list of the items
            string[] items = controller.getIdentifierSummary().getItemAndActiveAreaIds();

            // If the list has elements, show the dialog with the options
            if (items.Length > 0)
            {
                string selectedItem = controller.showInputDialog(TC.get("Action.OperationAddAction"), TC.get("Action.MessageSelectItem"), items);

                // If some value was selected
                if (selectedItem != null)
                    newAction = new Action(Action.USE_WITH, selectedItem);
            }

            // If the list had no elements, show an error dialog
            else
                controller.showErrorDialog(TC.get("Action.OperationAddAction"), TC.get("Action.ErrorNoItems"));
        }

        // If the type of action is drag-to, we must ask for a second item
        else if (type == Controller.ACTION_DRAG_TO)
        {
            // Take the list of the items
            string[] items = controller.getIdentifierSummary().getItemActiveAreaNPCIds();

            // If the list has elements, show the dialog with the options
            if (items.Length > 0)
            {
                string selectedItem = controller.showInputDialog(TC.get("Action.OperationAddAction"), TC.get("Action.MessageSelectItem"), items);

                // If some value was selected
                if (selectedItem != null)
                    newAction = new Action(Action.DRAG_TO, selectedItem);
            }

            // If the list had no elements, show an error dialog
            else
                controller.showErrorDialog(TC.get("Action.OperationAddAction"), TC.get("Action.ErrorNoItems"));
        }

        // If the type of action is give-to, we must ask for a character
        else if (type == Controller.ACTION_GIVE_TO)
        {
            // Take the list of the characters
            string[] npcs = controller.getIdentifierSummary().getNPCIds();

            // If the list has elements, show the dialog with the options
            if (npcs.Length > 0)
            {
                string selectedNPC = controller.showInputDialog(TC.get("Action.OperationAddAction"), TC.get("Action.MessageSelectNPC"), npcs);

                // If some value was selected
                if (selectedNPC != null)
                    newAction = new Action(Action.GIVE_TO, selectedNPC);
            }

            // If the list had no elements, show an error dialog
            else
                controller.showErrorDialog(TC.get("Action.OperationAddAction"), TC.get("Action.ErrorNoNPCs"));
        }

        // If an action was added, create a controller and store it
        if (newAction != null)
        {
            actionsList.Add(newAction);
            if (newAction.getType() == Action.CUSTOM || newAction.getType() == Action.CUSTOM_INTERACT)
                actionsDataControlList.Add(new CustomActionDataControl((CustomAction)newAction));
            else
                actionsDataControlList.Add(new ActionDataControl(newAction));
            //controller.dataModified( );
        }

        return newAction != null;
    }

    
    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;

        if (actionsList.Remove((Action)dataControl.getContent()))
        {
            actionsDataControlList.Remove((ActionDataControl)dataControl);
            //controller.dataModified( );
            elementDeleted = true;
        }

        return elementDeleted;
    }

    
    public override bool duplicateElement(DataControl dataControl)
    {

        if (!(dataControl is ActionDataControl ) )
            return false;

       
            Action newElement;
            ActionDataControl adc;
            if (((Action)(dataControl.getContent())).getType() == Action.CUSTOM || ((Action)(dataControl.getContent())).getType() == Action.CUSTOM_INTERACT)
            {
                newElement = (CustomAction)(((CustomAction)(dataControl.getContent())).Clone());
                adc = new CustomActionDataControl((CustomAction)newElement);
            }
            else {
                newElement = (Action)(((Action)(dataControl.getContent())));
                adc = new ActionDataControl(newElement);
            }
            actionsList.Add(newElement);
            actionsDataControlList.Add(adc);
            return true;
    }

    
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = actionsList.IndexOf((Action)dataControl.getContent());
        
        if (elementIndex > 0)
        {
            Action e = actionsList[elementIndex];
            ActionDataControl c = actionsDataControlList[elementIndex];
            actionsList.RemoveAt(elementIndex);
            actionsDataControlList.RemoveAt(elementIndex);
            actionsList.Insert(elementIndex - 1, e);
            actionsDataControlList.Insert(elementIndex - 1, c);
            controller.dataModified();
            elementMoved = true;

        }

        return elementMoved;
    }

    
    public override bool moveElementDown(DataControl dataControl)
    {
        bool elementMoved = false;
        int elementIndex = actionsList.IndexOf((Action)dataControl.getContent());
        
        if (elementIndex < actionsList.Count - 1)
        {
            Action e = actionsList[elementIndex];
            ActionDataControl c = actionsDataControlList[elementIndex];
            actionsList.RemoveAt(elementIndex);
            actionsDataControlList.RemoveAt(elementIndex);
            actionsList.Insert(elementIndex + 1, e);
            actionsDataControlList.Insert(elementIndex + 1, c);
            controller.dataModified();
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

        // Iterate through each action
        foreach (ActionDataControl actionDataControl in actionsDataControlList)
            actionDataControl.updateVarFlagSummary(varFlagSummary);
    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Iterate through the actions
        for (int i = 0; i < actionsDataControlList.Count; i++)
        {
            string actionPath = currentPath + " >> " + TC.get("Element.Action") + " #" + (i + 1) + " (" + TC.getElement(actionsDataControlList[i].getType()) + ")";
            valid &= actionsDataControlList[i].isValid(actionPath, incidences);
        }

        return valid;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through each action
        foreach (ActionDataControl actionDataControl in actionsDataControlList)
            count += actionDataControl.countAssetReferences(assetPath);

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Iterate through each action
        foreach (ActionDataControl actionDataControl in actionsDataControlList)
            actionDataControl.getAssetReferences(assetPaths, assetTypes);
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through each action
        foreach (ActionDataControl actionDataControl in actionsDataControlList)
            actionDataControl.deleteAssetReferences(assetPath);
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Iterate through each action
        foreach (ActionDataControl actionDataControl in actionsDataControlList)
            count += actionDataControl.countIdentifierReferences(id);

        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through each action
        foreach (ActionDataControl actionDataControl in actionsDataControlList)
            actionDataControl.replaceIdentifierReferences(oldId, newId);
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        int i = 0;

        // Check every action
        while (i < actionsList.Count)
        {
            Action action = actionsList[i];

            // If the action has a reference to the identifier, delete it
            if ((action.getType() == Action.GIVE_TO || action.getType() == Action.USE_WITH || action.getType() == Action.DRAG_TO) && action.getTargetId().Equals(id))
            {
                actionsList.RemoveAt(i);
                actionsDataControlList.RemoveAt(i);
            }
            else if ((action.getType() == Action.CUSTOM_INTERACT && action.getTargetId().Equals(id)))
            {
                actionsList.RemoveAt(i);
                actionsDataControlList.RemoveAt(i);
            }

            // If not, spread the call and increase the counter
            else {
                actionsDataControlList[i].deleteIdentifierReferences(id);
                i++;
            }
        }

    }

    
    public override bool canBeDuplicated()
    {

        return false;
    }

    
    public override void recursiveSearch()
    {

        foreach (DataControl dc in this.actionsDataControlList)
            dc.recursiveSearch();
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return getPathFromChild(dataControl, actionsDataControlList.Cast<Searchable>().ToList());
    }

    public List<Action> getActionsList()
    {

        return this.actionsList;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;
using UnityEditor;

namespace uAdventure.Editor
{
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

                if (action.getConditions().IsEmpty())
                    actionsInfo[i][1] = TC.get("GeneralText.No");
                else
                    actionsInfo[i][1] = TC.get("GeneralText.Yes");

                if (action.getEffects().IsEmpty())
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
            if (parent is ItemDataControl)
                return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_GRAB, Controller.ACTION_USE, Controller.ACTION_CUSTOM_INTERACT, Controller.ACTION_USE_WITH, Controller.ACTION_GIVE_TO, Controller.ACTION_DRAG_TO };
            if (parent is NPCDataControl)
                return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_USE, Controller.ACTION_CUSTOM, Controller.ACTION_TALK_TO, Controller.ACTION_DRAG_TO };
            if (parent is ActiveAreaDataControl)
                return new int[] { Controller.ACTION_EXAMINE, Controller.ACTION_USE, Controller.ACTION_CUSTOM };
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

            switch (type)
            {
                case Controller.ACTION_EXAMINE:
                    newAction = new Action(Action.EXAMINE);
                    break;
                case Controller.ACTION_USE:
                    newAction = new Action(Action.USE);
                    break;
                case Controller.ACTION_GRAB:
                    var item = parent as ItemDataControl;
                    newAction = new Action(Action.GRAB)
                    {
                        Effects = new Effects()
                        {
                            new RemoveElementEffect(item.getId()),
                            new GenerateObjectEffect(item.getId())
                        }
                    };
                    break;
                case Controller.ACTION_TALK_TO:

                    string[] conversations = controller.IdentifierSummary.getIds<Conversation>();
                    if (id != null && conversations.Contains(id))
                    {
                        newAction = new Action(Action.TALK_TO);
                        newAction.getEffects().Add(new TriggerConversationEffect(id));
                    }
                    else
                    {
                        var options = conversations.ToList();
                        var newConversation = "--- New ---";
                        options.Insert(0, newConversation);
                        controller.ShowInputDialog(TC.get("TalkToAction.MessageSelectConversation"), TC.get("TalkToAction.MessageSelectConversation"), options.ToArray(), (nothing, conversationId) =>
                        {
                            var action = new Action(Action.TALK_TO);
                            if (conversationId == newConversation)
                            {
                                var conversationList = controller.SelectedChapterDataControl.getConversationsList();
                                conversationList.addElement(Controller.CONVERSATION_GRAPH, null, false, this, (sender, newConversationId) =>
                                {
                                    action.getEffects().Add(new TriggerConversationEffect(newConversationId));
                                    performAddAction(action);
                                    if(controller.ShowStrictConfirmDialog("Open conversation?", "Do you want to open the new conversation?"))
                                    {
                                        controller.SelectElement(conversationList.getConversations().Last());
                                    }
                                });
                            }
                            else
                            {
                                action.getEffects().Add(new TriggerConversationEffect(conversationId));
                                performAddAction(action);
                            }
                        });
                        return true;
                    }

                    break;

                // For these tree the creation pipeline is the same
                case Controller.ACTION_USE_WITH:
                case Controller.ACTION_DRAG_TO:
                case Controller.ACTION_GIVE_TO:
                    
                    string message = "";
                    int actionType = -1;
                    System.Type[] validTypes = { };

                    // Select the elements, the action, and the popup message
                    switch (type)
                    {
                        case Controller.ACTION_DRAG_TO:
                            validTypes = new System.Type[] { typeof(Item), typeof(ActiveArea), typeof(NPC) };
                            actionType = Action.DRAG_TO;
                            message = TC.get("CustomAction.MessageSelectInteraction");
                            break;
                        case Controller.ACTION_USE_WITH:
                            validTypes = new System.Type[] { typeof(Item), typeof(ActiveArea) };
                            actionType = Action.USE_WITH;
                            message = TC.get("Action.MessageSelectItem");
                            break;
                        case Controller.ACTION_GIVE_TO:
                            validTypes = new System.Type[] { typeof(NPC) };
                            message = TC.get("Action.MessageSelectNPC");
                            actionType = Action.GIVE_TO;
                            break;
                    }

                    // If the list has elements, show the dialog with the options
                    auxCreateInteractiveAction(message, actionType, validTypes, (selectedTarget) =>
                    {
                        var action = new Action(actionType, selectedTarget);
                        var parentItem = parent as ItemDataControl;
                        if(parentItem != null && controller.ShowStrictConfirmDialog("Remove element?", "Do you want the item to be removed or consumed in the action?"))
                        {
                            if(actionType == Action.DRAG_TO)
                            {
                                action.Effects.Add(new RemoveElementEffect(parentItem.getId()));
                            }
                            else
                            {
                                action.Effects.Add(new ConsumeObjectEffect(parentItem.getId()));
                            }
                        }
                        return action;
                    });
                    break;
            }


            if (type == Controller.ACTION_CUSTOM_INTERACT)
            {
                controller.ShowInputDialog(TC.get("CustomAction.GetNameTitle"), TC.get("CustomAction.GetNameMessage"), (sender, name) =>
                {
                    if (string.IsNullOrEmpty(name))
                        name = "NONAME_" + Random.Range(0, 1000);

                    string[] options = { TC.get("Element.Action"), TC.get("Element.Interaction") };
                    controller.ShowInputDialog(TC.get("CustomAction.SelectTypeMessage"), TC.get("CustomAction.SelectTypeTitle"), options, (sender2, optionSelected) =>
                    {
                        switch(System.Array.IndexOf(options, optionSelected))
                        {
                            case 0:
                                {
                                    var customAction = new CustomAction(Action.CUSTOM);
                                    customAction.setName(name);
                                    performAddAction(customAction);
                                }
                                break;
                            case 1:
                                auxCreateInteractiveAction(TC.get("CustomAction.MessageSelectInteraction"), 
                                    Action.CUSTOM_INTERACT, new System.Type[] { typeof(Item), typeof(ActiveArea), typeof(NPC) }, 
                                    (selectedTarget) =>
                                    {
                                        var customAction = new CustomAction(Action.CUSTOM_INTERACT, selectedTarget);
                                        customAction.setName(name);
                                        return customAction;
                                    });
                                break;
                        }
                    });
                });
            }
            else if (type == Controller.ACTION_CUSTOM)
            {
                controller.ShowInputDialog(TC.get("CustomAction.GetNameTitle"), TC.get("CustomAction.GetNameMessage"), (sender, name) =>
                {
                    if (string.IsNullOrEmpty(name))
                        name = "NONAME_" + Random.Range(0, 1000);
                    
                    var customAction = new CustomAction(Action.CUSTOM);
                    customAction.setName(name);
                    performAddAction(customAction);
                });
            }

            // If an action was added, create a controller and store it
            if (newAction != null)
            {
                performAddAction(newAction);
            }

            return newAction != null;
        }

        private void auxCreateInteractiveAction(string message, int actionType, System.Type[] validTargetTypes, System.Func<string, Action> configureAction)
        {
            var elementCount = validTargetTypes.Select(t => Controller.Instance.IdentifierSummary.getIds(t).Length).Sum();

            // If the list has elements, show the dialog with the options
            if (elementCount > 0)
            {
                var elements = new string[elementCount];
                var totalCopied = 0;
                foreach (var t in validTargetTypes)
                {
                    var typeElements = controller.IdentifierSummary.getIds(t);
                    System.Array.Copy(typeElements, 0, elements, totalCopied, typeElements.Length);
                    totalCopied += typeElements.Length;
                }

                controller.ShowInputDialog(TC.get("Action.OperationAddAction"), message, elements, null,
                    new ActionsListReceiver(this)
                    {
                        ConfigureAction = configureAction
                    });
            }
            else
                controller.ShowErrorDialog(TC.get("Action.OperationAddAction"), TC.get("Action.ErrorNoItems"));
        }

        private void performAddAction(Action newAction)
        {
            getActionsList().Add(newAction);
            if (newAction.getType() == Action.CUSTOM || newAction.getType() == Action.CUSTOM_INTERACT)
                getActions().Add(new CustomActionDataControl((CustomAction)newAction));
            else
                getActions().Add(new ActionDataControl(newAction));
            controller.DataModified();
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;

            if (actionsList.Remove((Action)dataControl.getContent()))
            {
                actionsDataControlList.Remove((ActionDataControl)dataControl);
                controller.updateVarFlagSummary();
                controller.DataModified();
                elementDeleted = true;
            }

            return elementDeleted;
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is ActionDataControl))
                return false;


            Action newElement;
            ActionDataControl adc;
            if (((Action)(dataControl.getContent())).getType() == Action.CUSTOM || ((Action)(dataControl.getContent())).getType() == Action.CUSTOM_INTERACT)
            {
                newElement = (CustomAction)(((CustomAction)(dataControl.getContent())).Clone());
                adc = new CustomActionDataControl((CustomAction)newElement);
            }
            else
            {
                newElement = (Action)(((Action)(dataControl.getContent())).Clone());
                adc = new ActionDataControl(newElement);
            }
            actionsList.Add(newElement);
            actionsDataControlList.Add(adc);
            controller.updateVarFlagSummary();
            controller.DataModified();
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
                controller.DataModified();
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
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string newName)
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
                else
                {
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

        private class ActionsListReceiver : DialogReceiverInterface
        {
            private readonly ActionsListDataControl actionsListDataControl;
            public System.Func<string, Action> ConfigureAction;

            public ActionsListReceiver(ActionsListDataControl actionsListDataControl)
            {
                this.actionsListDataControl = actionsListDataControl;
            }

            public void OnDialogCanceled(object workingObject = null) { /* Nothing to do on cancel so far */ }

            public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
            {
                var newAction = ConfigureAction(message);
                // If an action was added, create a controller and store it
                if (newAction != null)
                {
                    actionsListDataControl.performAddAction(newAction);

                }
            }

        }
    }
}
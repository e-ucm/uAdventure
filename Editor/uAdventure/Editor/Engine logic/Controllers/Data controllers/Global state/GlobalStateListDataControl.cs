using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class GlobalStateListDataControl : DataControl
    {


        /**
         * List of globalStates.
         */
        private List<GlobalState> globalStatesList;

        /**
         * List of globalState controllers.
         */
        private List<GlobalStateDataControl> globalStatesDataControlList;

        /**
         * Constructor.
         * 
         * @param globalStatesList
         *            List of globalStates
         */
        public GlobalStateListDataControl(List<GlobalState> globalStatesList)
        {

            this.globalStatesList = globalStatesList;

            // Create subcontrollers
            globalStatesDataControlList = new List<GlobalStateDataControl>();
            foreach (GlobalState globalState in globalStatesList)
                globalStatesDataControlList.Add(new GlobalStateDataControl(globalState));
        }

        /**
         * Returns the list of globalState controllers.
         * 
         * @return GlobalState controllers
         */
        public List<GlobalStateDataControl> getGlobalStates()
        {

            return globalStatesDataControlList;
        }

        /**
         * Returns the last globalState controller from the list.
         * 
         * @return Last globalState controller
         */
        public GlobalStateDataControl getLastGlobalState()
        {

            return globalStatesDataControlList[globalStatesDataControlList.Count - 1];
        }

        /**
         * Returns the info of the globalStates contained in the list.
         * 
         * @return Array with the information of the globalStates. It contains the
         *         identifier of each globalState, and the number of actions
         */
        public string[][] getGlobalStatesInfo()
        {

            string[][] globalStatesInfo = null;

            // Create the list for the globalStates
            globalStatesInfo = new string[globalStatesList.Count][];
            for (int i = 0; i < globalStatesList.Count; i++)
                globalStatesInfo[i] = new string[2];

            // Fill the array with the info
            for (int i = 0; i < globalStatesList.Count; i++)
            {
                GlobalState globalState = globalStatesList[i];
                globalStatesInfo[i][0] = globalState.getId();
                globalStatesInfo[i][1] = Controller.Instance.countIdentifierReferences(globalState.getId()).ToString();
            }

            return globalStatesInfo;
        }

        public string[] getGlobalStatesIds()
        {

            string[] globalStatesInfo = null;

            // Create the list for the globalStates
            globalStatesInfo = new string[globalStatesList.Count];

            // Fill the array with the info
            for (int i = 0; i < globalStatesList.Count; i++)
            {
                globalStatesInfo[i] = globalStatesList[i].getId();
            }

            return globalStatesInfo;
        }

        public override System.Object getContent()
        {

            return globalStatesList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.GLOBAL_STATE };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new globalStates
            return type == Controller.GLOBAL_STATE;
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


        public override bool addElement(int type, string globalStateId)
        {

            bool elementAdded = false;

            if (type == Controller.GLOBAL_STATE)
            {

                // Show a dialog asking for the globalState id
                if (string.IsNullOrEmpty(globalStateId))
                    controller.ShowInputIdDialog(TC.get("Operation.AddGlobalStateTitle"), TC.get("Operation.AddGlobalStateMessage"),
                        Controller.Instance.makeElementValid(TC.get("Operation.AddGlobalStateDefaultValue")), performAddElement);
                // If some value was typed and the identifier is valid
                else
                {
                    performAddElement(null, globalStateId);
                    elementAdded = true;
                }
            }

            return elementAdded;
        }

        private void performAddElement(object sender, string globalStateId)
        {
            if (!controller.isElementIdValid(globalStateId))
                globalStateId = controller.makeElementValid(globalStateId);

            // Add thew new globalState
            GlobalState newGlobalState = new GlobalState(globalStateId);
            globalStatesList.Add(newGlobalState);
            globalStatesDataControlList.Add(new GlobalStateDataControl(newGlobalState));
            controller.IdentifierSummary.addId<GlobalState>(globalStateId);
            controller.DataModified();
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is GlobalStateDataControl))
                return false;

            GlobalState newElement = (GlobalState)(((GlobalState)(dataControl.getContent())).Clone());
            string id = newElement.getId();
            int i = 1;
            do
            {
                id = newElement.getId() + i;
                i++;
            } while (!controller.isElementIdValid(id, false));
            newElement.setId(id);
            globalStatesList.Add(newElement);
            globalStatesDataControlList.Add(new GlobalStateDataControl(newElement));
            controller.IdentifierSummary.addId<GlobalState>(id);
            controller.updateVarFlagSummary();
            controller.DataModified();
            return true;
        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddGlobalStateDefaultValue");
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;
            string globalStateId = ((GlobalStateDataControl)dataControl).getId();
            string references = controller.countIdentifierReferences(globalStateId).ToString();

            // Ask for confirmation
            if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { globalStateId, references })))
            {
                if (globalStatesList.Remove((GlobalState)dataControl.getContent()))
                {
                    globalStatesDataControlList.Remove((GlobalStateDataControl)dataControl);
                    controller.deleteIdentifierReferences(globalStateId);
                    controller.IdentifierSummary.deleteId<GlobalState>(globalStateId);
                    controller.updateVarFlagSummary();
                    controller.DataModified();
                    elementDeleted = true;
                }
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = globalStatesList.IndexOf((GlobalState)dataControl.getContent());

            if (elementIndex > 0)
            {
                GlobalState e = globalStatesList[elementIndex];
                GlobalStateDataControl c = globalStatesDataControlList[elementIndex];
                globalStatesList.RemoveAt(elementIndex);
                globalStatesDataControlList.RemoveAt(elementIndex);
                globalStatesList.Insert(elementIndex - 1, e);
                globalStatesDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = globalStatesList.IndexOf((GlobalState)dataControl.getContent());

            if (elementIndex < globalStatesList.Count - 1)
            {
                GlobalState e = globalStatesList[elementIndex];
                GlobalStateDataControl c = globalStatesDataControlList[elementIndex];
                globalStatesList.RemoveAt(elementIndex);
                globalStatesDataControlList.RemoveAt(elementIndex);
                globalStatesList.Insert(elementIndex + 1, e);
                globalStatesDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
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

            // Iterate through each globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
                globalStateDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.GLOBAL_STATE_LIST);

            // Iterate through the globalStates
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
            {
                string globalStatePath = currentPath + " >> " + globalStateDataControl.getId();
                valid &= globalStateDataControl.isValid(globalStatePath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
                count += globalStateDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
                globalStateDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
                globalStateDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
                count += globalStateDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
                globalStateDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every globalState
            foreach (GlobalStateDataControl globalStateDataControl in globalStatesDataControlList)
            {
                globalStateDataControl.deleteIdentifierReferences(id);

            }
        }


        public override bool canBeDuplicated()
        {

            return false;
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.globalStatesDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, globalStatesDataControlList.Cast<Searchable>().ToList());
        }

        public List<GlobalState> getGlobalStatesList()
        {

            return this.globalStatesList;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class GlobalStateDataControl : DataControl
    {
        private ConditionsController conditionsController;

        private GlobalState globalState;

        public GlobalStateDataControl(GlobalState conditions)
        {

            globalState = conditions;
            conditionsController = new ConditionsController(globalState, Controller.GLOBAL_STATE, globalState.getId());
        }

        public void setDocumentation(string doc)
        {

            Controller.Instance.AddTool(new ChangeDocumentationTool(globalState, doc));
        }

        public string getDocumentation()
        {

            return globalState.getDocumentation();
        }

        public string getId()
        {

            return globalState.getId();
        }

        public void setId(string val)
        {
            if (val != globalState.getId())
            {
                var oldId = globalState.getId();
                if (!controller.isElementIdValid(val))
                    val = controller.makeElementValid(val);
                globalState.setId(val);

                controller.replaceIdentifierReferences(oldId, val);
                controller.IdentifierSummary.deleteId<GlobalState>(oldId);
                controller.IdentifierSummary.addId<GlobalState>(val);
            }
        }

        /**
         * @return the controller
         */
        public ConditionsController getController()
        {

            return conditionsController;
        }


        public override bool addElement(int type, string id)
        {

            return false;
        }


        public override bool canAddElement(int type)
        {

            return false;
        }


        public override bool canBeDeleted()
        {

            // Check if no references are made to this global state
            int references = Controller.Instance.countIdentifierReferences(getId());
            return (references == 0);
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override bool canBeMoved()
        {

            return true;
        }


        public override bool canBeRenamed()
        {

            return true;
        }


        public override int countAssetReferences(string assetPath)
        {

            return 0;
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;
            count += conditionsController.countIdentifierReferences(id);
            return count;
        }


        public override void deleteAssetReferences(string assetPath)
        {

        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override void deleteIdentifierReferences(string id)
        {

            conditionsController.deleteIdentifierReferences(id);
        }


        public override int[] getAddableElements()
        {

            return new int[] { };
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

        }


        public override System.Object getContent()
        {

            return globalState;
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            return true;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override string renameElement(string name)
        {
            string oldItemId = getId();
            string references = Controller.Instance.countIdentifierReferences(oldItemId).ToString();

            // Ask for confirmation
            if (name != null || Controller.Instance.ShowStrictConfirmDialog(TC.get("Operation.RenameGlobalStateTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldItemId, references })))
            {
                // Show a dialog asking for the new item id
                if (name == null)
                    Controller.Instance.ShowInputDialog(TC.get("Operation.RenameGlobalStateTitle"), TC.get("Operation.RenameGlobalStateMessage"), oldItemId, (o,s) => performRenameElement(s));
                else
                    return performRenameElement(name);
            }

			return null;
        }

        private string performRenameElement(string newGlobalStateId)
        {
            string oldGlobalStateId = getId();
            // If some value was typed and the identifiers are different
            if (Controller.Instance.isElementIdValid(newGlobalStateId))
                newGlobalStateId = Controller.Instance.makeElementValid(newGlobalStateId);

            globalState.setId(newGlobalStateId);
            Controller.Instance.replaceIdentifierReferences(oldGlobalStateId, newGlobalStateId);
            Controller.Instance.IdentifierSummary.deleteId<GlobalState>(oldGlobalStateId);
            Controller.Instance.IdentifierSummary.addId<GlobalState>(newGlobalStateId);
            //Controller.getInstance().dataModified( );
            return newGlobalStateId;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            if (globalState.getId().Equals(oldId))
            {
                globalState.setId(newId);
                Controller.Instance.IdentifierSummary.deleteId<GlobalState>(oldId);
                Controller.Instance.IdentifierSummary.addId<GlobalState>(newId);
            }
            conditionsController.replaceIdentifierReferences(oldId, newId);
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            ConditionsController.updateVarFlagSummary(varFlagSummary, globalState);
        }


        public override void recursiveSearch()
        {
            check(this.conditionsController, TC.get("Search.Conditions"));
            check(this.getDocumentation(), TC.get("Search.Documentation"));
            check(this.getId(), "ID");
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }
    }
}
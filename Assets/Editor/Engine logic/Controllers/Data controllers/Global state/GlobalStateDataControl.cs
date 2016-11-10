using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalStateDataControl : DataControl
{
    private ConditionsController controller;

    private GlobalState globalState;

    public GlobalStateDataControl(GlobalState conditions)
    {

        globalState = conditions;
        controller = new ConditionsController(globalState, Controller.GLOBAL_STATE, globalState.getId());
    }

    public void setDocumentation(string doc)
    {

        Controller.getInstance().addTool(new ChangeDocumentationTool(globalState, doc));
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
        globalState.setId(val);
    }

    /**
     * @return the controller
     */
    public ConditionsController getController()
    {

        return controller;
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
        int references = Controller.getInstance().countIdentifierReferences(getId());
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
        count += controller.countIdentifierReferences(id);
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

        controller.deleteIdentifierReferences(id);
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

        bool elementRenamed = false;
        string oldItemId = getId();
        string references = Controller.getInstance().countIdentifierReferences(oldItemId).ToString();

        // Ask for confirmation
        if (name != null || Controller.getInstance().showStrictConfirmDialog(TC.get("Operation.RenameGlobalStateTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldItemId, references })))
        {

            // Show a dialog asking for the new item id
            string newItemId = name;
            if (name == null)
                newItemId = Controller.getInstance().showInputDialog(TC.get("Operation.RenameGlobalStateTitle"), TC.get("Operation.RenameGlobalStateMessage"), oldItemId);

            // If some value was typed and the identifiers are different
            if (newItemId != null && !newItemId.Equals(oldItemId) && Controller.getInstance().isElementIdValid(newItemId))
            {
                globalState.setId(newItemId);
                Controller.getInstance().replaceIdentifierReferences(oldItemId, newItemId);
                Controller.getInstance().getIdentifierSummary().deleteGlobalStateId(oldItemId);
                Controller.getInstance().getIdentifierSummary().addGlobalStateId(newItemId);
                //Controller.getInstance().dataModified( );
                elementRenamed = true;
            }
        }

        if (elementRenamed)
            return oldItemId;
        else
            return null;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        if (globalState.getId().Equals(oldId))
        {
            globalState.setId(newId);
            Controller.getInstance().getIdentifierSummary().deleteGlobalStateId(oldId);
            Controller.getInstance().getIdentifierSummary().addGlobalStateId(newId);
        }
        controller.replaceIdentifierReferences(oldId, newId);
    }

    
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

        ConditionsController.updateVarFlagSummary(varFlagSummary, globalState);
    }

    
    public override void recursiveSearch()
    {

        check(this.controller, TC.get("Search.Conditions"));
        check(this.getDocumentation(), TC.get("Search.Documentation"));
        check(this.getId(), "ID");
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return null;
    }
}

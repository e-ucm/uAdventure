using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MacroDataControl : DataControl
{
    private EffectsController controller;

    private Macro macro;

    public MacroDataControl(Macro conditions)
    {

        macro = conditions;
        controller = new EffectsController(macro);
    }

    public void setDocumentation(string doc)
    {

        Controller.getInstance().addTool(new ChangeDocumentationTool(macro, doc));
    }

    public string getDocumentation()
    {

        return macro.getDocumentation();
    }

    public string getId()
    {

        return macro.getId();
    }

    public void setId(string val)
    {
         macro.setId(val);
    }
    /**
     * @return the controller
     */

    public EffectsController getController()
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

        return EffectsController.countAssetReferences(assetPath, macro);
    }


    public override int countIdentifierReferences(string id)
    {

        return EffectsController.countIdentifierReferences(id, macro);
    }


    public override void deleteAssetReferences(string assetPath)
    {

        EffectsController.deleteAssetReferences(assetPath, macro);
    }


    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        return false;
    }


    public override void deleteIdentifierReferences(string id)
    {

        EffectsController.deleteIdentifierReferences(id, macro);
    }


    public override int[] getAddableElements()
    {

        return new int[] {};
    }


    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        EffectsController.getAssetReferences(assetPaths, assetTypes, macro);
    }


    public override System.Object getContent()
    {

        return macro;
    }


    public override bool isValid(string currentPath, List<string> incidences)
    {

        return EffectsController.isValid(currentPath, incidences, macro);
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
        if (name != null ||
            Controller.getInstance()
                .showStrictConfirmDialog(TC.get("Operation.RenameMacroTitle"),
                    TC.get("Operation.RenameElementWarning", new string[] {oldItemId, references})))
        {

            // Show a dialog asking for the new item id
            string newItemId = name;
            if (name == null)
                newItemId = Controller.getInstance()
                    .showInputDialog(TC.get("Operation.RenameMacroTitle"), TC.get("Operation.RenameMacroMessage"),
                        oldItemId);

            // If some value was typed and the identifiers are different
            if (newItemId != null && !newItemId.Equals(oldItemId) &&
                Controller.getInstance().isElementIdValid(newItemId))
            {
                macro.setId(newItemId);
                Controller.getInstance().replaceIdentifierReferences(oldItemId, newItemId);
                Controller.getInstance().getIdentifierSummary().deleteMacroId(oldItemId);
                Controller.getInstance().getIdentifierSummary().addMacroId(newItemId);
                //Controller.getInstance().dataModified( );
                elementRenamed = true;
            }
        }

        if (elementRenamed)
            return oldItemId;
        return null;
    }


    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        EffectsController.replaceIdentifierReferences(oldId, newId, macro);
    }


    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

        EffectsController.updateVarFlagSummary(varFlagSummary, macro);
    }


    public override void recursiveSearch()
    {

        check(this.getDocumentation(), TC.get("Search.Documentation"));
        check(this.getId(), "ID");

        for (int i = 0; i < this.getController().getEffectCount(); i++)
        {
            check(this.getController().getEffectInfo(i), TC.get("Search.Effect"));
        }
    }


    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return null;
    }
}
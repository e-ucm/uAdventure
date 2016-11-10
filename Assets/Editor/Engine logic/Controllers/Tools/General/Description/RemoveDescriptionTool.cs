using UnityEngine;
using System.Collections;

public class RemoveDescriptionTool : Tool
{
    private DescriptionsController descriptionsController;

    private DescriptionController deletedDescriptionController;

    private Description description;

    /*
     * Elements for UNDO REDO
     */
    private int lastSelectedDescription;


    public RemoveDescriptionTool(DescriptionsController descriptionsController)
    {

        this.descriptionsController = descriptionsController;

    }


    
    public override bool canRedo()
    {

        return true;
    }

    
    public override bool canUndo()
    {

        return true;
    }

    
    public override bool combine(Tool other)
    {

        return false;
    }

    
    public override bool doTool()
    {

        bool elementDeleted = false;
        lastSelectedDescription = descriptionsController.getSelectedDescriptionNumber();
        if (descriptionsController.getSelectedDescriptionNumber() > 0)
        {
            deletedDescriptionController = descriptionsController.removeSelectedDescription();
            descriptionsController.setSelectedDescription(descriptionsController.getDescriptionCount() - 1);
            elementDeleted = true;
        }



        // If it was the last one, show an error message
        else
            //TODO cambiar cadenas
            Controller.getInstance().showErrorDialog(TC.get("Operation.DeleteResourcesTitle"), TC.get("Operation.DeleteResourcesErrorLastResources"));

        return elementDeleted;

    }

    
    public override bool redoTool()
    {

        bool redone = doTool();
        if (redone)
            Controller.getInstance().updatePanel();
        return redone;
    }

    
    public override bool undoTool()
    {

        descriptionsController.addDescriptionController(deletedDescriptionController, lastSelectedDescription);
        descriptionsController.addDescription(deletedDescriptionController.getDescriptionData(), lastSelectedDescription);
        descriptionsController.setSelectedDescription(lastSelectedDescription);
        Controller.getInstance().updatePanel();
        return true;
    }

}

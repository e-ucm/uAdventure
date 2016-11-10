using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ExitsListDataControl : DataControl
{

    /**
     * Scene controller that contains this element reference.
     */
    private SceneDataControl sceneDataControl;

    /**
     * List of exits.
     */
    private List<Exit> exitsList;

    /**
     * List of exits controllers.
     */
    private List<ExitDataControl> exitsDataControlList;

    /**
     * Constructor.
     * 
     * @param sceneDataControl
     *            Link to the parent scene controller
     * @param exitsList
     *            List of exits
     */
    public ExitsListDataControl(SceneDataControl sceneDataControl, List<Exit> exitsList)
    {

        this.sceneDataControl = sceneDataControl;
        this.exitsList = exitsList;

        // Create subcontrollers
        exitsDataControlList = new List<ExitDataControl>();
        foreach (Exit exit in exitsList)
            exitsDataControlList.Add(new ExitDataControl(sceneDataControl, exit));
    }

    /**
     * Returns the list of exits controllers.
     * 
     * @return List of exits controllers
     */
    public List<ExitDataControl> getExits()
    {

        return exitsDataControlList;
    }

    /**
     * Returns the last exit controller from the list.
     * 
     * @return Last exit controller
     */
    public ExitDataControl getLastExit()
    {

        return exitsDataControlList[exitsDataControlList.Count - 1];
    }

    /**
     * Returns the id of the scene that contains this exits list.
     * 
     * @return Parent scene id
     */
    public string getParentSceneId()
    {

        return sceneDataControl.getId();
    }

 
    public override System.Object getContent()
    {

        return exitsList;
    }

 
    public override int[] getAddableElements()
    {

        return new int[] { Controller.EXIT };
    }

 
    public override bool canAddElement(int type)
    {

        // It can always add new exit
        return type == Controller.EXIT;
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

        bool elementAdded = false;

        if (type == Controller.EXIT)
        {
            string[] generalScenes = controller.getIdentifierSummary().getGeneralSceneIds();

            if (generalScenes.Length > 0)
            {
                string selectedScene = id;
                if (selectedScene != null)
                {
                    Exit newExit = new Exit(true, 240, 240, 100, 100);
                    newExit.setNextSceneId(selectedScene);
                    ExitDataControl newExitDataControl = new ExitDataControl(sceneDataControl, newExit);

                    exitsList.Add(newExit);
                    exitsDataControlList.Add(newExitDataControl);
                    elementAdded = true;
                }
            }

        }

        return elementAdded;
    }

 
    public override bool duplicateElement(DataControl dataControl)
    {

        if (!(dataControl is ExitDataControl ) )
            return false;

       
            Exit newElement = (Exit)(((Exit)(dataControl.getContent())).Clone());
            exitsList.Add(newElement);
            exitsDataControlList.Add(new ExitDataControl(sceneDataControl, newElement));
            return true;
    }

 
    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;

        if (exitsList.Remove((Exit)dataControl.getContent()))
        {
            exitsDataControlList.Remove((ExitDataControl)dataControl);
            //controller.dataModified( );
            elementDeleted = true;
        }

        return elementDeleted;
    }

 
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = exitsList.IndexOf((Exit)dataControl.getContent());

        if (elementIndex > 0)
        {
            Exit o = exitsList[elementIndex];
            ExitDataControl c = exitsDataControlList[elementIndex];
            exitsList.RemoveAt(elementIndex);
            exitsDataControlList.RemoveAt(elementIndex);
            exitsList.Insert(elementIndex - 1, o);
            exitsDataControlList.Insert(elementIndex - 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

 
    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = exitsList.IndexOf((Exit)dataControl.getContent());

        if (elementIndex < exitsList.Count - 1)
        {
            Exit o = exitsList[elementIndex];
            ExitDataControl c = exitsDataControlList[elementIndex];
            exitsList.RemoveAt(elementIndex);
            exitsDataControlList.RemoveAt(elementIndex);
            exitsList.Insert(elementIndex + 1, o);
            exitsDataControlList.Insert(elementIndex + 1, c);
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

        // Iterate through each exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            exitDataControl.updateVarFlagSummary(varFlagSummary);
    }

 
    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Iterate through the exits
        for (int i = 0; i < exitsDataControlList.Count; i++)
        {
            string exitPath = currentPath + " >> " + TC.getElement(Controller.EXIT) + " #" + (i + 1);
            valid &= exitsDataControlList[i].isValid(exitPath, incidences);
        }

        return valid;
    }

 
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through each exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            count += exitDataControl.countAssetReferences(assetPath);

        return count;
    }

 
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Iterate through each exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            exitDataControl.getAssetReferences(assetPaths, assetTypes);
    }

 
    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through each exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            exitDataControl.deleteAssetReferences(assetPath);
    }

 
    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Iterate through each exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            count += exitDataControl.countIdentifierReferences(id);

        return count;
    }

 
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through each exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            exitDataControl.replaceIdentifierReferences(oldId, newId);
    }

 
    public override void deleteIdentifierReferences(string id)
    {

        // Spread the call to every exit
        foreach (ExitDataControl exitDataControl in exitsDataControlList)
            exitDataControl.deleteIdentifierReferences(id);

        int i = 0;
        while (i < exitsList.Count)
        {
            if (exitsList[i].getNextSceneId() == null)
            {
                exitsList.RemoveAt(i);
                exitsDataControlList.RemoveAt(i);
            }
            else
                i++;
        }
    }

 
    public override bool canBeDuplicated()
    {

        return false;
    }

    /**
     * Returns the data controllers of the item references of the scene that
     * contains this element reference.
     * 
     * @return List of item references (including the one being edited)
     */
    public List<ElementReferenceDataControl> getParentSceneItemReferences()
    {

        return sceneDataControl.getReferencesList().getItemReferences();
    }

    /**
     * Returns the data controllers of the character references of the scene
     * that contains this element reference.
     * 
     * @return List of character references (including the one being edited)
     */
    public List<ElementReferenceDataControl> getParentSceneNPCReferences()
    {

        return sceneDataControl.getReferencesList().getNPCReferences();
    }

    /**
     * Returns the data controllers of the atrezzo items references of the scene
     * that contains this element reference.
     * 
     * @return List of atrezzo references (including the one being edited)
     */
    public List<ElementReferenceDataControl> getParentSceneAtrezzoReferences()
    {

        return sceneDataControl.getReferencesList().getAtrezzoReferences();
    }

    public List<BarrierDataControl> getParentSceneBarriers()
    {

        return sceneDataControl.getBarriersList().getBarriers();
    }

    public List<ActiveAreaDataControl> getParentSceneActiveAreas()
    {

        return sceneDataControl.getActiveAreasList().getActiveAreas();
    }

 
    public override void recursiveSearch()
    {

        foreach (DataControl dc in this.exitsDataControlList)
            dc.recursiveSearch();
    }

    public SceneDataControl getSceneDataControl()
    {

        return sceneDataControl;
    }

    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return getPathFromChild(dataControl, exitsDataControlList.Cast<System.Object>().ToList());
    }

    public List<Exit> getExitsList()
    {

        return this.exitsList;
    }
}

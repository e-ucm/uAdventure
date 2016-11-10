using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActiveAreasListDataControl : DataControl
{

    /**
     * Scene controller that contains this element reference.
     */
    private SceneDataControl sceneDataControl;

    /**
     * List of activeAreas.
     */
    private List<ActiveArea> activeAreasList;

    /**
     * List of activeAreas controllers.
     */
    private List<ActiveAreaDataControl> activeAreasDataControlList;

    /**
     * Constructor.
     * 
     * @param sceneDataControl
     *            Link to the parent scene controller
     * @param activeAreasList
     *            List of activeAreas
     */

    public ActiveAreasListDataControl(SceneDataControl sceneDataControl, List<ActiveArea> activeAreasList)
    {

        this.sceneDataControl = sceneDataControl;
        this.activeAreasList = activeAreasList;

        // Create subcontrollers
        activeAreasDataControlList = new List<ActiveAreaDataControl>();
        foreach (ActiveArea activeArea in activeAreasList)
            activeAreasDataControlList.Add(new ActiveAreaDataControl(sceneDataControl, activeArea));
    }

    /**
     * Returns the list of activeAreas controllers.
     * 
     * @return List of activeAreas controllers
     */

    public List<ActiveAreaDataControl> getActiveAreas()
    {

        return activeAreasDataControlList;
    }

    /**
     * Returns the last activeArea controller from the list.
     * 
     * @return Last activeArea controller
     */

    public ActiveAreaDataControl getLastActiveArea()
    {

        return activeAreasDataControlList[activeAreasDataControlList.Count - 1];
    }

    /**
     * Returns the id of the scene that contains this activeAreas list.
     * 
     * @return Parent scene id
     */

    public string getParentSceneId()
    {

        return sceneDataControl.getId();
    }


    public override System.Object getContent()
    {

        return activeAreasList;
    }


    public override int[] getAddableElements()
    {

        return new int[] {Controller.ACTIVE_AREA};
    }


    public override bool canAddElement(int type)
    {

        // It can always add new activeArea
        return type == Controller.ACTIVE_AREA;
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

        if (type == Controller.ACTIVE_AREA)
        {

            // Show a dialog asking for the item id
            string itemId = id;
            if (id == null)
                itemId = controller.showInputDialog(TC.get("Operation.AddItemTitle"), TC.get("Operation.AddItemMessage"),
                    TC.get("Operation.AddItemDefaultValue"));

            // If some value was typed and the identifier is valid
            if (itemId != null && controller.isElementIdValid(itemId))
            {
                ActiveArea newActiveArea = new ActiveArea(itemId, true, 220, 220, 100, 100);
                activeAreasList.Add(newActiveArea);
                ActiveAreaDataControl newActiveAreaDataControl = new ActiveAreaDataControl(sceneDataControl,
                    newActiveArea);
                activeAreasDataControlList.Add(newActiveAreaDataControl);
                controller.getIdentifierSummary().addActiveAreaId(itemId);
                //controller.dataModified( );
                elementAdded = true;
            }
        }

        return elementAdded;
    }


    public override string getDefaultId(int type)
    {

        return "IdObject";
    }


    public override bool duplicateElement(DataControl dataControl)
    {

        if (!(dataControl is ActiveAreaDataControl ) )
        return false;

      
            ActiveArea newElement = (ActiveArea) (((ActiveArea) (dataControl.getContent())).Clone());
            string id;
            int i = 1;
            do
            {
                id = getDefaultId(0) + i;
                i++;
            } while (!controller.isElementIdValid(id, false));
            newElement.setId(id);
            activeAreasList.Add(newElement);
            activeAreasDataControlList.Add(new ActiveAreaDataControl(sceneDataControl, newElement));
            controller.getIdentifierSummary().addActiveAreaId(id);
            return true;
       
    }


    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;
        string id = ((ActiveAreaDataControl) dataControl).getId();
        string references = controller.countIdentifierReferences(id).ToString();

        if (!askConfirmation ||
            controller.showStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"),
                TC.get("Operation.DeleteElementWarning", new string[] {id, references})))
        {
            if (activeAreasList.Remove((ActiveArea)dataControl.getContent()))
            {
                activeAreasDataControlList.Remove((ActiveAreaDataControl)dataControl);
                controller.deleteIdentifierReferences(id);
                controller.getIdentifierSummary().deleteActiveAreaId(((ActiveArea) dataControl.getContent()).getId());
                elementDeleted = true;
            }
        }

        return elementDeleted;
    }


    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = activeAreasList.IndexOf((ActiveArea)dataControl.getContent());

        if (elementIndex > 0)
        {
            ActiveArea a = activeAreasList[elementIndex];
            ActiveAreaDataControl c = activeAreasDataControlList[elementIndex];
            activeAreasList.RemoveAt(elementIndex);
            activeAreasDataControlList.RemoveAt(elementIndex);
            activeAreasList.Insert(elementIndex - 1, a);
            activeAreasDataControlList.Insert(elementIndex - 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }


    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = activeAreasList.IndexOf((ActiveArea)dataControl.getContent());

        if (elementIndex < activeAreasList.Count - 1)
        {
            ActiveArea a = activeAreasList[elementIndex];
            ActiveAreaDataControl c = activeAreasDataControlList[elementIndex];
            activeAreasList.RemoveAt(elementIndex);
            activeAreasDataControlList.RemoveAt(elementIndex);
            activeAreasList.Insert(elementIndex + 1, a);
            activeAreasDataControlList.Insert(elementIndex + 1, c);
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

        // Iterate through each activeArea
        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
            activeAreaDataControl.updateVarFlagSummary(varFlagSummary);
    }


    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Iterate through the activeAreas
        for (int i = 0; i < activeAreasDataControlList.Count; i++)
        {
            string activeAreaPath = currentPath + " >> " + TC.getElement(Controller.ACTIVE_AREA) + " #" + (i + 1);
            valid &= activeAreasDataControlList[i].isValid(activeAreaPath, incidences);
        }

        return valid;
    }


    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through each activeArea
        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
            count += activeAreaDataControl.countAssetReferences(assetPath);

        return count;
    }


    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
        activeAreaDataControl.getAssetReferences(assetPaths, assetTypes);

    }


    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through each activeArea
        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
        activeAreaDataControl.deleteAssetReferences(assetPath);
    }


    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Iterate through each activeArea
        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
        count += activeAreaDataControl.countIdentifierReferences(id);

        return count;
    }


    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through each activeArea
        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
        activeAreaDataControl.replaceIdentifierReferences(oldId, newId);
    }


    public override void deleteIdentifierReferences(string id)
    {

        // Spread the call to every activeArea
        foreach (ActiveAreaDataControl activeAreaDataControl in activeAreasDataControlList)
        activeAreaDataControl.deleteIdentifierReferences(id);

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

    public List<ExitDataControl> getParentSceneExits()
    {

        return sceneDataControl.getExitsList().getExits();
    }

    public List<BarrierDataControl> getParentSceneBarriers()
    {

        return sceneDataControl.getBarriersList().getBarriers();
    }


    public override void recursiveSearch()
    {

        foreach (DataControl dc in this.activeAreasDataControlList)
        dc.recursiveSearch();
    }

    public SceneDataControl getSceneDataControl()
    {

        return this.sceneDataControl;
    }

    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return getPathFromChild(dataControl, activeAreasDataControlList.Cast<System.Object>().ToList());
    }

    public List<ActiveArea> getActiveAreasList()
    {

        return this.activeAreasList;
    }
}
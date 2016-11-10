using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class SceneDataControl : DataControlWithResources
{
    /**
         * Contained scene data.
         */
    private Scene scene;

    /**
     * Exits list controller.
     */
    private ExitsListDataControl exitsListDataControl;

    /**
     * Element references list controller.
     */
    private ReferencesListDataControl referencesListDataControl;

    /**
     * Active areas list controller
     */
    private ActiveAreasListDataControl activeAreasListDataControl;

    /**
     * Barriers list controller
     */
    private BarriersListDataControl barriersListDataControl;

    private TrajectoryDataControl trajectoryDataControl;

    /**
     * Constructor.
     * 
     * @param scene
     *            Contained scene data
     */
    public SceneDataControl(Scene scene, string playerImagePath)
    {

        this.scene = scene;
        this.resourcesList = scene.getResources();

        selectedResources = 0;

        // Add a new resource if the list is empty
        if (resourcesList.Count == 0)
            resourcesList.Add(new ResourcesUni());

        // Create the subcontrollers
        resourcesDataControlList = new List<ResourcesDataControl>();
        foreach (ResourcesUni resources in resourcesList)
            resourcesDataControlList.Add(new ResourcesDataControl(resources, Controller.SCENE));

        exitsListDataControl = new ExitsListDataControl(this, scene.getExits());
        referencesListDataControl = new ReferencesListDataControl(playerImagePath, this, scene.getItemReferences(), scene.getAtrezzoReferences(), scene.getCharacterReferences());
        activeAreasListDataControl = new ActiveAreasListDataControl(this, scene.getActiveAreas());
        barriersListDataControl = new BarriersListDataControl(this, scene.getBarriers());
        trajectoryDataControl = new TrajectoryDataControl(this, scene.getTrajectory());
    }

    /**
     * Returns the exits list controller.
     * 
     * @return Exits list controller
     */
    public ExitsListDataControl getExitsList()
    {

        return exitsListDataControl;
    }

    /**
     * Returns the active areas list controller.
     * 
     * @return Exits list controller
     */
    public ActiveAreasListDataControl getActiveAreasList()
    {

        return this.activeAreasListDataControl;
    }

    /**
     * Returns the barriers list controller.
     * 
     * @return Barriers list controller
     */
    public BarriersListDataControl getBarriersList()
    {

        return this.barriersListDataControl;
    }

    /**
     * Returns the item references list controller.
     * 
     * @return Item references list controller
     */
    public ReferencesListDataControl getReferencesList()
    {

        return referencesListDataControl;
    }

    /**
     * Returns the path to the selected preview image.
     * 
     * @return Path to the image, null if not present
     */
    
    public string getPreviewBackground()
    {

        return resourcesDataControlList[selectedResources].getAssetPath("background");
    }

    public void setPreviewBackground(string path)
    {
        resourcesDataControlList[selectedResources].addAsset("background", path);
    }

    public string getPreviewForeground()
    {

        return resourcesDataControlList[selectedResources].getAssetPath("foreground");
    }

    public void setPreviewForeground(string path)
    {
        resourcesDataControlList[selectedResources].addAsset("foreground", path);
    }

    public string getPreviewMusic()
    {
        return resourcesDataControlList[selectedResources].getAssetPath("bgmusic");
    }

    public void setPreviewMusic(string path)
    {
        resourcesDataControlList[selectedResources].addAsset("bgmusic", path);
    }
    /**
     * Returns the id of the scene.
     * 
     * @return Scene's id
     */
    public string getId()
    {

        return scene.getId();
    }

    /**
     * Returns the documentation of the scene.
     * 
     * @return Scene's documentation
     */
    public string getDocumentation()
    {

        return scene.getDocumentation();
    }

    /**
     * Returns the name of the scene.
     * 
     * @return Scene's name
     */
    public string getName()
    {

        return scene.getName();
    }

    /**
     * Returns whether the scene has a default initial position or not.
     * 
     * @return True if the scene has an initial position, false otherwise
     */
    public bool hasDefaultInitialPosition()
    {

        return scene.hasDefaultPosition();
    }

    /**
     * Returns the X coordinate of the default initial position
     * 
     * @return X coordinate of the initial position
     */
    public int getDefaultInitialPositionX()
    {

        return scene.getPositionX();
    }

    /**
     * Returns the Y coordinate of the default initial position
     * 
     * @return Y coordinate of the initial position
     */
    public int getDefaultInitialPositionY()
    {

        return scene.getPositionY();
    }

    /**
     * Sets the new name of the scene.
     * 
     * @param name
     *            Name of the scene
     */
    public void setName(string name)
    {

        ChangeNameTool tool = new ChangeNameTool(scene, name);
        controller.addTool(tool);
    }

    public void imageChangeNotify(string imagePath)
    {

        this.referencesListDataControl.changeImagePlayerPath(imagePath);
    }

    /**
     * Sets the new documentation of the scene.
     * 
     * @param documentation
     *            Documentation of the scene
     */
    public void setDocumentation(string documentation)
    {

        ChangeDocumentationTool tool = new ChangeDocumentationTool(scene, documentation);
        controller.addTool(tool);
    }

    /**
     * Toggles the default initial position. If the scene has an initial
     * position deletes it, if it doesn't have one, set initial values for it.
     */
    public void toggleDefaultInitialPosition()
    {

        if (scene.hasDefaultPosition())
            controller.addTool(new ChangeNSDestinyPositionTool(scene, Int32.MinValue, Int32.MinValue));
        else
            controller.addTool(new ChangeNSDestinyPositionTool(scene, AssetsImageDimensions.BACKGROUND_MAX_WIDTH / 2, AssetsImageDimensions.BACKGROUND_MAX_HEIGHT / 2));
    }

    /**
     * Sets the new default initial position of the scene.
     * 
     * @param positionX
     *            X coordinate of the initial position
     * @param positionY
     *            Y coordinate of the initial position
     */
    public void setDefaultInitialPosition(int positionX, int positionY)
    {

        controller.addTool(new ChangePositionTool(scene, positionX, positionY));
    }

    public override System.Object getContent()
    {

        return scene;
    }

  
    public override int[] getAddableElements()
    {

        return new int[] { /*Controller.RESOURCES*/};
    }

  
    public override bool canAddElement(int type)
    {

        // It can always add new resources
        //return type == Controller.RESOURCES;
        return false;
    }

  
    public override bool canBeDeleted()
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

  
    public override bool addElement(int type, string id)
    {

        bool elementAdded = false;

        if (type == Controller.RESOURCES)
        {
            elementAdded = Controller.getInstance().addTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, Controller.SCENE, this));
        }

        return elementAdded;
    }

  
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

        if (elementIndex > 0)
        {
            ResourcesUni r = resourcesList[elementIndex];
            ResourcesDataControl d = resourcesDataControlList[elementIndex];
            resourcesList.RemoveAt(elementIndex);
            resourcesDataControlList.RemoveAt(elementIndex);
            resourcesList.Insert(elementIndex - 1, r);
            resourcesDataControlList.Insert(elementIndex - 1, d);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

  
    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

        if (elementIndex < resourcesList.Count - 1)
        {
            ResourcesUni r = resourcesList[elementIndex];
            ResourcesDataControl d = resourcesDataControlList[elementIndex];
            resourcesList.RemoveAt(elementIndex);
            resourcesDataControlList.RemoveAt(elementIndex);
            resourcesList.Insert(elementIndex + 1, r);
            resourcesDataControlList.Insert(elementIndex + 1, d);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

  
    public override string renameElement(string name)
    {

        bool elementRenamed = false;
        string oldSceneId = scene.getId();
        string references = controller.countIdentifierReferences(oldSceneId).ToString();

        // Ask for confirmation
        if (name != null || controller.showStrictConfirmDialog(TC.get("Operation.RenameSceneTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldSceneId, references })))
        {

            // Show a dialog asking for the new scene id
            string newSceneId = name;
            if (name == null)
                newSceneId = controller.showInputDialog(TC.get("Operation.RenameSceneTitle"), TC.get("Operation.RenameSceneMessage"), oldSceneId);

            // If some value was typed and the identifiers are different
            if (newSceneId != null && !newSceneId.Equals(oldSceneId) && controller.isElementIdValid(newSceneId))
            {
                scene.setId(newSceneId);
                controller.replaceIdentifierReferences(oldSceneId, newSceneId);
                controller.getIdentifierSummary().deleteSceneId(oldSceneId);
                controller.getIdentifierSummary().addSceneId(newSceneId);
                //controller.dataModified( );
                elementRenamed = true;
            }
        }

        if (elementRenamed)
            return oldSceneId;
        return null;
    }

  
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

        // Update the flag summary with the exits, item and character references
        exitsListDataControl.updateVarFlagSummary(varFlagSummary);
        referencesListDataControl.updateVarFlagSummary(varFlagSummary);
        activeAreasListDataControl.updateVarFlagSummary(varFlagSummary);
        barriersListDataControl.updateVarFlagSummary(varFlagSummary);
        //trajectoryDataControl.updateVarFlagSummary( varFlagSummary );
        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.updateVarFlagSummary(varFlagSummary);

    }

  
    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Iterate through the resources
        for (int i = 0; i < resourcesDataControlList.Count; i++)
        {
            string resourcesPath = currentPath + " >> " + TC.getElement(Controller.RESOURCES) + " #" + (i + 1);
            valid &= resourcesDataControlList[i].isValid(resourcesPath, incidences);
        }

        // Spread the call to the exits
        valid &= exitsListDataControl.isValid(currentPath, incidences);
        valid &= activeAreasListDataControl.isValid(currentPath, incidences);
        // valid &= trajectoryDataControl.isValid( currentPath, incidences );

        return valid;
    }

  
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            count += resourcesDataControl.countAssetReferences(assetPath);

        // Increase the counter with the references in the exits
        count += exitsListDataControl.countAssetReferences(assetPath);
        count += activeAreasListDataControl.countAssetReferences(assetPath);

        return count;
    }

  
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        exitsListDataControl.getAssetReferences(assetPaths, assetTypes);
        activeAreasListDataControl.getAssetReferences(assetPaths, assetTypes);

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.getAssetReferences(assetPaths, assetTypes);

    }

  
    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.deleteAssetReferences(assetPath);

        // Delete the references in the exits
        exitsListDataControl.deleteAssetReferences(assetPath);
        activeAreasListDataControl.deleteAssetReferences(assetPath);
    }

  
    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Increase the counter for the exits, items and characters
        count += exitsListDataControl.countIdentifierReferences(id);
        count += referencesListDataControl.countIdentifierReferences(id);
        count += activeAreasListDataControl.countIdentifierReferences(id);
        count += barriersListDataControl.countIdentifierReferences(id);
        //  count += trajectoryDataControl.countIdentifierReferences( id );

        return count;
    }

  
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        exitsListDataControl.replaceIdentifierReferences(oldId, newId);
        referencesListDataControl.replaceIdentifierReferences(oldId, newId);
        activeAreasListDataControl.replaceIdentifierReferences(oldId, newId);
        barriersListDataControl.replaceIdentifierReferences(oldId, newId);
        // trajectoryDataControl.replaceIdentifierReferences( oldId, newId );
    }

  
    public override void deleteIdentifierReferences(string id)
    {

        exitsListDataControl.deleteIdentifierReferences(id);
        referencesListDataControl.deleteIdentifierReferences(id);
        activeAreasListDataControl.deleteIdentifierReferences(id);
        barriersListDataControl.deleteIdentifierReferences(id);
        // trajectoryDataControl.deleteIdentifierReferences( id );
    }

  
    public override bool canBeDuplicated()
    {

        return true;
    }

    public TrajectoryDataControl getTrajectory()
    {

        return trajectoryDataControl;
    }

    public void setTrajectoryDataControl(TrajectoryDataControl trajectoryDataControl)
    {

        this.trajectoryDataControl = trajectoryDataControl;
    }

    public void setTrajectory(Trajectory trajectory)
    {

        scene.setTrajectory(trajectory);
    }

    public void setPlayerLayer(int layer)
    {

        scene.setPlayerLayer(layer);
    }

    public int getPlayerLayer()
    {

        return scene.getPlayerLayer();
    }

    /*   public void setAllowPlayerLayer( bool allow ) {

           scene.setAllowPlayerLayer( allow );
       }*/

    public bool isForcedPlayerLayer()
    {

        return scene.isForcedPlayerLayer();

    }

    public bool isAllowPlayer()
    {

        return scene.isAllowPlayerLayer();
    }

    public void deletePlayerInReferenceList()
    {

        referencesListDataControl.deletePlayer();
    }

    public void addPlayerInReferenceList()
    {

        referencesListDataControl.addPlayer();

    }

    public void changeAllowPlayerLayer(bool isAllowPlayerLayer /*, ScenePreviewEditionPanel scenePreviewEditionPanel^*/ )
    {

        Controller.getInstance().addTool(new ChangeForcePlayerInSceneTool(isAllowPlayerLayer, /*scenePreviewEditionPanel,*/ this));
    }

    // this method is only used in SwapPlayerModeTool
    public void insertPlayer()
    {

        deletePlayerInReferenceList();
        referencesListDataControl.restorePlayer();
    }

    // this method is only used in SwapPlayerModeTool

    public void setPlayerScale(float scale)
    {

        //scene.setPlayerScale(scale);
        Controller.getInstance().addTool(new ChangePlayerScaleTool(scene, scale));
    }

    public float getPlayerScale()
    {

        return scene.getPlayerScale();
    }

  
    public override void recursiveSearch()
    {

        this.getActiveAreasList().recursiveSearch();
        this.getBarriersList().recursiveSearch();
        check(this.getDocumentation(), TC.get("Search.Documentation"));
        check(this.getName(), TC.get("Search.Name"));
        check(this.getId(), "ID");
        this.getExitsList().recursiveSearch();
        this.getReferencesList().recursiveSearch();
    }

  
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        List<Searchable> path = getPathFromChild(dataControl, resourcesDataControlList.Cast<System.Object>().ToList());
        if (path != null)
            return path;
        path = getPathFromChild(dataControl, exitsListDataControl);
        if (path != null)
            return path;
        path = getPathFromChild(dataControl, referencesListDataControl);
        if (path != null)
            return path;
        path = getPathFromChild(dataControl, activeAreasListDataControl);
        if (path != null)
            return path;
        path = getPathFromChild(dataControl, barriersListDataControl);
        if (path != null)
            return path;
        return getPathFromChild(dataControl, trajectoryDataControl);
    }
}

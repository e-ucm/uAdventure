using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveAreaDataControl : DataControl, RectangleArea
{
    /**
         * Scene controller that contains this element reference (used to extract
         * the id of the scene).
         */
    private SceneDataControl sceneDataControl;

    /**
     * Contained activeArea.
     */
    private ActiveArea activeArea;

    /**
     * Actions list controller.
     */
    private ActionsListDataControl actionsListDataControl;

    /**
     * Conditions controller.
     */
    private ConditionsController conditionsController;

    /**
     * Controller for descriptions
     */
    private DescriptionsController descriptionsController;

    private InfluenceAreaDataControl influenceAreaDataControl;

    /**
     * Constructor.
     * 
     * @param sceneDataControl
     *            Parent scene controller
     * @param activeArea
     *            Exit of the data control structure
     */
    public ActiveAreaDataControl(SceneDataControl sceneDataControl, ActiveArea activeArea)
    {

        this.sceneDataControl = sceneDataControl;
        this.activeArea = activeArea;
        conditionsController = new ConditionsController(new Conditions());
        this.influenceAreaDataControl = new InfluenceAreaDataControl(sceneDataControl, activeArea.getInfluenceArea(), this);
        descriptionsController = new DescriptionsController(activeArea.getDescriptions());

        // Create subcontrollers
        actionsListDataControl = new ActionsListDataControl(activeArea.getActions(), this);

    }

    /**
     * Returns the actions list controller.
     * 
     * @return Actions list controller
     */
    public ActionsListDataControl getActionsList()
    {

        return actionsListDataControl;
    }

    /**
     * Returns the id of the scene that contains this element reference.
     * 
     * @return Parent scene id
     */
    public string getParentSceneId()
    {

        return sceneDataControl.getId();
    }

    /**
     * Returns the id of the item.
     * 
     * @return Item's id
     */
    public string getId()
    {

        return activeArea.getId();
    }

    /**
     * Returns the documentation of the item.
     * 
     * @return Item's documentation
     */
    public string getDocumentation()
    {

        return activeArea.getDocumentation();
    }

    /**
     * Returns the X coordinate of the upper left position of the exit.
     * 
     * @return X coordinate of the upper left point
     */
    public int getX()
    {

        return activeArea.getX();
    }

    /**
     * Returns the Y coordinate of the upper left position of the exit.
     * 
     * @return Y coordinate of the upper left point
     */
    public int getY()
    {

        return activeArea.getY();
    }

    /**
     * Returns the width of the exit.
     * 
     * @return Width of the exit
     */
    public int getWidth()
    {

        return activeArea.getWidth();
    }

    /**
     * Returns the height of the exit.
     * 
     * @return Height of the exit
     */
    public int getHeight()
    {

        return activeArea.getHeight();
    }

    /**
     * Sets the new values for the exit.
     * 
     * @param x
     *            X coordinate of the upper left point
     * @param y
     *            Y coordinate of the upper left point
     * @param width
     *            Width of the exit area
     * @param height
     *            Height of the exit area
     */
    public void setActiveArea(int x, int y, int width, int height)
    {

        controller.addTool(new ChangeRectangleValueTool(activeArea, x, y, width, height));
    }

    public override System.Object getContent()
    {

        return activeArea;
    }

    public override int[] getAddableElements()
    {

        return new int[] { };
    }

    
    public override bool canAddElement(int type)
    {

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
        return elementAdded;
    }

    
    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;
        return elementDeleted;
    }

    
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        return elementMoved;
    }

    
    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        return elementMoved;
    }

    
    public override string renameElement(string name)
    {

        bool elementRenamed = false;
        string oldSceneId = activeArea.getId();
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
                activeArea.setId(newSceneId);
                controller.replaceIdentifierReferences(oldSceneId, newSceneId);
                controller.getIdentifierSummary().deleteActiveAreaId(oldSceneId);
                controller.getIdentifierSummary().addActiveAreaId(newSceneId);
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

        actionsListDataControl.updateVarFlagSummary(varFlagSummary);
        ConditionsController.updateVarFlagSummary(varFlagSummary, activeArea.getConditions());
        descriptionsController.updateVarFlagSummary(varFlagSummary);
    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        valid &= actionsListDataControl.isValid(currentPath, incidences);
        //  valid &= descriptionsController.isValid( currentPath, incidences );

        return valid;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Add the references in the actions
        count += actionsListDataControl.countAssetReferences(assetPath);
        //v1.4
        count += descriptionsController.countAssetReferences(assetPath);

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        actionsListDataControl.getAssetReferences(assetPaths, assetTypes);
        descriptionsController.getAssetReferences(assetPaths, assetTypes);
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Delete the references from the actions
        actionsListDataControl.deleteAssetReferences(assetPath);

        //1.4
        descriptionsController.deleteAssetReferences(assetPath);
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;
        count += actionsListDataControl.countIdentifierReferences(id);
        count += conditionsController.countIdentifierReferences(id);
        //1.4
        count += descriptionsController.countIdentifierReferences(id);
        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        actionsListDataControl.replaceIdentifierReferences(oldId, newId);
        conditionsController.replaceIdentifierReferences(oldId, newId);
        //1.4
        descriptionsController.replaceIdentifierReferences(oldId, newId);
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        actionsListDataControl.deleteIdentifierReferences(id);
        conditionsController.deleteIdentifierReferences(id);
        descriptionsController.deleteIdentifierReferences(id);
    }

    /**
     * Returns the conditions of the element reference.
     * 
     * @return Conditions of the element reference
     */
    public ConditionsController getConditions()
    {

        Dictionary<string, ConditionsController.ConditionContextProperty> context1 = new Dictionary<string, ConditionsController.ConditionContextProperty>();
        ConditionsController.ConditionOwner parent = new ConditionsController.ConditionOwner(Controller.SCENE, sceneDataControl.getId());
        ConditionsController.ConditionOwner owner = new ConditionsController.ConditionOwner(Controller.ACTIVE_AREA, activeArea.getId(), parent);

        context1.Add(ConditionsController.CONDITION_OWNER, owner);
        conditionsController = new ConditionsController(activeArea.getConditions(), context1);

        return conditionsController;
    }

    
    public override bool canBeDuplicated()
    {

        return true;
    }

    
    public override void recursiveSearch()
    {

        this.getActionsList().recursiveSearch();
        this.descriptionsController.recursiveSearch();
        check(this.getConditions(), TC.get("Search.Conditions"));
        check(this.getDocumentation(), TC.get("Search.Documentation"));
        check(this.getId(), "ID");
    }

    public bool isRectangular()
    {

        return activeArea.isRectangular();
    }

    public List<Vector2> getPoints()
    {

        return activeArea.getPoints();
    }

    public void addPoint(int x, int y)
    {

        controller.addTool(new AddNewPointTool(activeArea, x, y, influenceAreaDataControl));
    }

    public Vector2 getLastPoint()
    {

        if (activeArea.getPoints().Count > 0)
            return activeArea.getPoints()[activeArea.getPoints().Count - 1];
        return Vector2.zero;
    }

    public void deletePoint(Vector2 point)
    {

        controller.addTool(new DeletePointTool(activeArea, point, influenceAreaDataControl));
    }

    public void setRectangular(bool selected)
    {

        controller.addTool(new ChangeRectangularValueTool(activeArea, selected));
    }

    public Rectangle getRectangle()
    {

        return (Rectangle)this.getContent();
    }

    public InfluenceAreaDataControl getInfluenceArea()
    {

        return influenceAreaDataControl;
    }

    public SceneDataControl getSceneDataControl()
    {

        return sceneDataControl;
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {
        List<Searchable> path = getPathFromChild(dataControl, actionsListDataControl);
        if (path != null)
            return path;
        path = getPathFromChild(dataControl, this.descriptionsController);
        return path;
    }


    public DescriptionsController getDescriptionsController()
    {

        return descriptionsController;
    }
}

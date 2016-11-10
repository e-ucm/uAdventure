using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDataControl : DataControl
{

  /* Scene controller that contains this element reference(used to extract
     * the id of the scene).
     */
    private SceneDataControl sceneDataControl;

    private Trajectory trajectory;

    /**
     * Contained node.
     */
    private Trajectory.Node node;

    private bool initial;

    /**
     * Constructor.
     * 
     * @param sceneDataControl
     *            Parent scene controller
     * @param activeArea
     *            Exit of the data control structure
     */
    public NodeDataControl(SceneDataControl sceneDataControl, Trajectory.Node node, Trajectory trajectory)
    {

        this.sceneDataControl = sceneDataControl;
        this.node = node;
        this.trajectory = trajectory;
        initial = false;
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
     * Returns the X coordinate of the upper left position of the exit.
     * 
     * @return X coordinate of the upper left point
     */
    public int getX()
    {

        return node.getX();
    }

    /**
     * Returns the Y coordinate of the upper left position of the exit.
     * 
     * @return Y coordinate of the upper left point
     */
    public int getY()
    {

        return node.getY();
    }

    /**
     * Sets the new values for the exit.
     * 
     * @param x
     *            X coordinate of the upper left point
     * @param y
     *            Y coordinate of the upper left point
     * @param scale
     *            the scale of the player on the node
     */
    public void setNode(int x, int y, float scale)
    {

        controller.addTool(new SetNodeValuesTool(node, trajectory, x, y, scale));
    }

    
    public override System.Object getContent()
    {

        return node;
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

        return false;
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

        return null;
    }

    
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        return true;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // DO nothing
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Delete the references from the actions
        // Do nothing
    }

    
    public override int countIdentifierReferences(string id)
    {

        return 0;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        //actionsListDataControl.replaceIdentifierReferences( oldId, newId );
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        //actionsListDataControl.deleteIdentifierReferences( id );
    }

    
    public override bool canBeDuplicated()
    {

        return true;
    }

    public float getScale()
    {

        return node.getScale();
    }

    public string getID()
    {

        return node.getID();
    }

    public string getPlayerImagePath()
    {

        return controller.getPlayerImagePath();
    }

    public int getPlayerLayer()
    {
        return sceneDataControl.getPlayerLayer();
    }

    public void setInitial(bool b)
    {

        initial = b;
    }

    public bool isInitial()
    {

        return initial;
    }

    
    public override void recursiveSearch()
    {

    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return null;
    }
}

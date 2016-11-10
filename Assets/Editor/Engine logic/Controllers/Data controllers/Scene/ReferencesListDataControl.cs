using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Data control for the list of references in the scene
 */
public class ReferencesListDataControl : DataControl {

    /**
     * Player image path
     */
    private string playerImagePath;

    /**
     * Scene controller that contains this element reference.
     */
    private SceneDataControl sceneDataControl;

    /**
     * List of item references.
     */
    private List<ElementReference> itemReferencesList;

    /**
     * List of atrezzo references.
     */
    private List<ElementReference> atrezzoReferencesList;

    /**
     * List of non-player character references.
     */
    private List<ElementReference> npcReferencesList;

    /**
     * List of all elements order by number of layer (or y position when they
     * have the same layer "-1")
     */
    private List<ElementContainer> allReferencesDataControl;

    /**
     * The last introduced element referenced or player (in a ElementContainer
     * object)
     */
    private ElementContainer lastElementContainer;

    /**
     * The player position in allReferencesDataControl
     */
    private int playerPositionInAllReferences;

    /**
     * The player isn't in all references
     */
    public const int NO_PLAYER = -1;

    /**
     * Tell us if the player image path has been changed
     */
    private bool imagePathHasChanged;

    /**
     * Constructor.
     * 
     * @param sceneDataControl
     *            Link to the parent scene controller
     * @param itemReferencesList
     *            List of item references
     */
    public ReferencesListDataControl(string playerImagePath, SceneDataControl sceneDataControl, List<ElementReference> itemReferencesList, List<ElementReference> atrezzoReferencesList, List<ElementReference> npcReferencesList)
    {

        this.playerImagePath = playerImagePath;
        this.sceneDataControl = sceneDataControl;
        this.itemReferencesList = itemReferencesList;
        this.atrezzoReferencesList = atrezzoReferencesList;
        this.npcReferencesList = npcReferencesList;
        this.allReferencesDataControl = new List<ElementContainer>();
        this.lastElementContainer = null;
        this.playerPositionInAllReferences = NO_PLAYER;
        this.imagePathHasChanged = false;
        // Check if one of references has layer -1: if it is true, it means that element references has no layer. 
        // Create subcontrollers

        bool hasLayerV = hasLayer();
        foreach (ElementReference itemReference in itemReferencesList)
        {
            int counter = count(itemReference);
            ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, itemReference, Controller.ITEM_REFERENCE, counter);
            insertInOrder(new ElementContainer(erdc, -1, null), hasLayerV);
        }

        foreach (ElementReference atrezzoReference in atrezzoReferencesList)
        {
            int counter = count(atrezzoReference);
            ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, atrezzoReference, Controller.ATREZZO_REFERENCE, counter);
            insertInOrder(new ElementContainer(erdc, -1, null), hasLayerV);
        }

        foreach (ElementReference npcReference in npcReferencesList)
        {
            int counter = count(npcReference);
            ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, npcReference, Controller.NPC_REFERENCE, counter);
            insertInOrder(new ElementContainer(erdc, -1, null), hasLayerV);
        }

        // insert player
        // by default, if player don´t have layer, we give it to him.
        if (playerImagePath != null && (!Controller.getInstance().isPlayTransparent()) && sceneDataControl.isForcedPlayerLayer())
        {
            int layer;
            if (sceneDataControl.getPlayerLayer() == Scene.PLAYER_WITHOUT_LAYER)
                layer = 0;
            else
                layer = sceneDataControl.getPlayerLayer();
            reassignLayerAllReferencesDataControl(insertInOrder(new ElementContainer(null, layer, AssetsController.getImage(this.playerImagePath)), true));
        }
    }

    private int count(ElementReference er)
    {
        int count = 0;
        foreach (ElementContainer e in allReferencesDataControl)
        {
            if (!e.isPlayer())
            {
                if (e.getErdc().getElementId().Equals(er.getTargetId()))
                    count++;
            }
        }
        return count;
    }

    /**
     * This method analyze the references finding if references has layer. It is
     * easy, we must only inspect one reference. If this reference has the value
     * "-1" in it "layer" attribute, it means that neither of elements has
     * layer.
     * 
     * @return true, if there are not one references with -1
     */
    private bool hasLayer()
    {

        if (itemReferencesList.Count > 0)
        {
            if (itemReferencesList[0].getLayer() == Scene.PLAYER_WITHOUT_LAYER)
                return false;
            else
                return true;
        }
        else if (atrezzoReferencesList.Count > 0)
        {
            if (atrezzoReferencesList[0].getLayer() == Scene.PLAYER_WITHOUT_LAYER)
                return false;
            else
                return true;
        }
        else if (npcReferencesList.Count > 0)
        {
            if (npcReferencesList[0].getLayer() == Scene.PLAYER_WITHOUT_LAYER)
                return false;
            else
                return true;
        }
        return false;
    }

    public Sprite getPlayerImage()
    {

        //CHANGE: Now, the image of the player must be taken from
        return AssetsController.getImage(Controller.getInstance().getPlayerImagePath());
        /*if (playerPositionInAllReferences==NO_PLAYER)
        return AssetsController.getImage(Controller.getInstance().getPlayerImagePath());
        else{
        if (imagePathHasChanged){
        	allReferencesDataControl.get(playerPositionInAllReferences).setImage(AssetsController.getImage( this.playerImagePath ));
        	imagePathHasChanged = false;
        }
        //	if (allReferences!=null)
        	return allReferencesDataControl.get(playerPositionInAllReferences).getImage();
        }*/

    }

    /**
     * Insert in order in allReferencesDataControl attribute
     * 
     * @param element
     *            The element container to be added
     * @param hasLayer
     *            Take either layer or depth value to order value
     * @param playerLayer
     *            Take the layer if player has it, or the y position if the
     *            player has not layer.
     * 
     * @return i returns the position where the element has been inserted. It
     *         will be use to reassign layer
     */
    public int insertInOrder(ElementContainer element, bool hasLayer)
    {

        bool added = false;
        int i = 0;
        bool empty = allReferencesDataControl.Count == 0;
        // While the element has not been added, and
        // we haven't checked every previous element
        while (!added && (i < allReferencesDataControl.Count || empty))
        {
            if (!empty)
            {
                if (hasLayer)
                {
                    if (element.getLayer() <= allReferencesDataControl[i].getLayer())
                    {
                        allReferencesDataControl.Insert(i, element);
                        added = true;
                    }
                }
                else {
                    if (element.getY() <= Mathf.Round(allReferencesDataControl[i].getY()))
                    {
                        allReferencesDataControl.Insert(i, element);
                        reassignLayerAllReferencesDataControl(i);
                        added = true;
                    }
                }
                i++;
            }
            else {
                allReferencesDataControl.Insert(i, element);
                if (!hasLayer)
                    reassignLayerAllReferencesDataControl(i);
                added = true;
                i++;
            }

        }

        // If the element wasn't added, add it in the last position
        if (!added)
        {
            //element.setLayer(i);
            allReferencesDataControl.Add(element);
            if (!hasLayer)
                reassignLayerAllReferencesDataControl(i - 1);

        }
        return i - 1;
    }

    /**
     * Merge all references in one list
     * 
     * @return The list that contains all references data control;
     */
    public List<ElementContainer> getAllReferencesDataControl()
    {

        return allReferencesDataControl;

    }

    /**
     * Returns the list of item reference controllers.
     * 
     * @return List of item reference controllers
     */
    public List<ElementReferenceDataControl> getItemReferences()
    {

        List<ElementReferenceDataControl> list = new List<ElementReferenceDataControl>();
        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (element.getErdc() != null && element.getErdc().getType() == Controller.ITEM_REFERENCE)
            {
                list.Add(element.getErdc());
            }
        }
        return list;
        //		return itemReferencesDataControlList;
    }

    /**
     * Returns the list of atrezzo item reference controllers.
     * 
     * @return List of atrezzo item reference controllers
     */
    public List<ElementReferenceDataControl> getAtrezzoReferences()
    {

        List<ElementReferenceDataControl> list = new List<ElementReferenceDataControl>();
        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (element.getErdc() != null && element.getErdc().getType() == Controller.ATREZZO_REFERENCE)
            {
                list.Add(element.getErdc());
            }
        }
        return list;
    }

    /**
     * Returns the list of npc reference controllers.
     * 
     * @return List of npc reference controllers
     */
    public List<ElementReferenceDataControl> getNPCReferences()
    {

        List<ElementReferenceDataControl> list = new List<ElementReferenceDataControl>();
        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (element.getErdc() != null && element.getErdc().getType() == Controller.NPC_REFERENCE)
            {
                list.Add(element.getErdc());
            }
        }
        return list;
    }

    /**
     * Returns the id of the scene that contains this item references list.
     * 
     * @return Parent scene id
     */
    public string getParentSceneId()
    {

        return sceneDataControl.getId();
    }

    //TODO ver si se puede devolver allReferences
  
    public override System.Object getContent()
    {

        return itemReferencesList;
    }

    
    public override int[] getAddableElements()
    {

        return new int[] { Controller.ITEM_REFERENCE, Controller.ATREZZO_REFERENCE, Controller.NPC_REFERENCE };
    }

    
    public override bool canAddElement(int type)
    {

        // It can always add new NPC references
        return type == Controller.ITEM_REFERENCE || type == Controller.ATREZZO_REFERENCE || type == Controller.NPC_REFERENCE;
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
        string selectedItem = id;

        if (type == Controller.ITEM_REFERENCE)
        {
            // Take the list of the items
            string[] items = controller.getIdentifierSummary().getItemIds();
            // If the list has elements, show the dialog with the options
            if (items.Length > 0)
            {

                // If some value was selected
                if (selectedItem != null)
                {
                    ElementReference newElementReference = new ElementReference(selectedItem, 50, 50);
                    int counter = count(newElementReference);
                    ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, newElementReference, type, counter);
                    itemReferencesList.Add(newElementReference);
                    ElementContainer ec = new ElementContainer(erdc, -1, null);
                    lastElementContainer = ec;
                    reassignLayerAllReferencesDataControl(insertInOrder(ec, false));
                    elementAdded = true;
                }
            }
            else
                controller.showErrorDialog(TC.get("Operation.AddItemReferenceTitle"), TC.get("Operation.AddItemReferenceErrorNoItems"));
        }

        if (type == Controller.ATREZZO_REFERENCE)
        {
            string[] items = controller.getIdentifierSummary().getAtrezzoIds();

            // If the list has elements, show the dialog with the options
            if (items.Length > 0)
            {
                if (selectedItem != null)
                {
                    ElementReference newElementReference = new ElementReference(selectedItem, 50, 50);
                    int counter = count(newElementReference);
                    ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, newElementReference, type, counter);
                    atrezzoReferencesList.Add(newElementReference);
                    ElementContainer ec = new ElementContainer(erdc, -1, null);
                    lastElementContainer = ec;
                    reassignLayerAllReferencesDataControl(insertInOrder(ec, false));
                    elementAdded = true;
                }
            }
            else
                controller.showErrorDialog(TC.get("Operation.AddAtrezzoReferenceTitle"), TC.get("Operation.AddReferenceErrorNoAtrezzo"));
        }

        if (type == Controller.NPC_REFERENCE)
        {
            string[] items = controller.getIdentifierSummary().getNPCIds();
            if (items.Length > 0)
            {
                if (selectedItem != null)
                {
                    ElementReference newElementReference = new ElementReference(selectedItem, 50, 50);
                    int counter = count(newElementReference);
                    ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, newElementReference, type, counter);
                    npcReferencesList.Add(newElementReference);
                    ElementContainer ec = new ElementContainer(erdc, -1, null);
                    lastElementContainer = ec;
                    reassignLayerAllReferencesDataControl(insertInOrder(ec, false));
                    elementAdded = true;
                }
            }
            else
                controller.showErrorDialog(TC.get("Operation.AddNPCReferenceTitle"), TC.get("Operation.AddReferenceErrorNoNPC"));
        }

        return elementAdded;
    }

    private void reassignLayerAllReferencesDataControl(int index)
    {

        for (int i = index; i < allReferencesDataControl.Count; i++)
        {
            allReferencesDataControl[i].setLayer(i);
            if (allReferencesDataControl[i].isPlayer())
                playerPositionInAllReferences = i;
        }

    }

    /**
     * Delete in allReferencesDataControl updating the layer.
     * 
     * @param dataControl
     *            the issue to delete
     */
    private void delete(DataControl dataControl)
    {

        if (dataControl != null)
        {
            int index = 0;
            for (index = 0; index < allReferencesDataControl.Count; index++)
                if (!allReferencesDataControl[index].isPlayer())
                    if (allReferencesDataControl[index].getErdc().Equals(dataControl))
                        break;
            if (index >= 0 && index < allReferencesDataControl.Count)
            {
                allReferencesDataControl.RemoveAt(index);
                reassignLayerAllReferencesDataControl(index);
            }

        }
    }

    
    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;
        if (dataControl != null)
        {
            itemReferencesList.Remove((ElementReference)dataControl.getContent());
            atrezzoReferencesList.Remove((ElementReference)dataControl.getContent());
            npcReferencesList.Remove((ElementReference)dataControl.getContent());
            delete(dataControl);
            elementDeleted = true;
        }
        return elementDeleted;
    }

    public void addElement(ElementContainer element)
    {

        if (element.getErdc().getType() == Controller.ITEM_REFERENCE)
            itemReferencesList.Add((ElementReference)element.getErdc().getContent());
        else if (element.getErdc().getType() == Controller.ATREZZO_REFERENCE)
            atrezzoReferencesList.Add((ElementReference)element.getErdc().getContent());
        else if (element.getErdc().getType() == Controller.NPC_REFERENCE)
            npcReferencesList.Add((ElementReference)element.getErdc().getContent());
        allReferencesDataControl.Insert(element.getLayer(), element);
        reassignLayerAllReferencesDataControl(element.getLayer());
    }

    private void moveUp(DataControl dataControl)
    {

        bool player;
        int index = 0;
        if (dataControl != null)
        {
            player = false;
            for (index = 0; index < allReferencesDataControl.Count; index++)
                if (!allReferencesDataControl[index].isPlayer())
                    if (allReferencesDataControl[index].getErdc().Equals(dataControl))
                        break;
        }
        else {
            index = playerPositionInAllReferences;
            player = true;
        }
        if (index > 0)
        {
            ElementContainer e = allReferencesDataControl[index];
            allReferencesDataControl.RemoveAt(index);
            allReferencesDataControl.Insert(index - 1, e);
            allReferencesDataControl[index].setLayer(index);
            allReferencesDataControl[index - 1].setLayer(index - 1);
            if (player)
                setPlayerPosition(index - 1);
            if (allReferencesDataControl[index].isPlayer())
                setPlayerPosition(index);
        }
    }

    
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        if (dataControl != null)
        {
            moveUp(dataControl);
            elementMoved = true;
        }
        else {
            moveUp(dataControl);
            elementMoved = true;
        }

        return elementMoved;
    }

    private void moveDown(DataControl dataControl)
    {

        bool player;
        int index = 0;
        if (dataControl != null)
        {
            player = false;
            for (index = 0; index < allReferencesDataControl.Count; index++)
                if (!allReferencesDataControl[index].isPlayer())
                    if (allReferencesDataControl[index].getErdc().Equals(dataControl))
                        break;
        }
        else {
            index = playerPositionInAllReferences;
            player = true;
        }
        if (index >= 0 && index < allReferencesDataControl.Count - 1)
        {
            //change the elements      
            ElementContainer e = allReferencesDataControl[index];
            allReferencesDataControl.RemoveAt(index);
            allReferencesDataControl.Insert(index + 1, e);
            //update element layer
            allReferencesDataControl[index].setLayer(index);
            allReferencesDataControl[index + 1].setLayer(index + 1);
            if (player)
                setPlayerPosition(index + 1);
            if (allReferencesDataControl[index].isPlayer())
                setPlayerPosition(index);
        }
    }

    
    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;

        if (dataControl != null)
        {
            moveDown(dataControl);
            elementMoved = true;
        }
        else {
            moveDown(dataControl);
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

        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (!element.isPlayer())
                element.getErdc().updateVarFlagSummary(varFlagSummary);
        }
    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        return true;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        return 0;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Do nothing
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Do nothing
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (!element.isPlayer())
                count += element.getErdc().countIdentifierReferences(id);
        }
        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (!element.isPlayer())
                element.getErdc().replaceIdentifierReferences(oldId, newId);
        }
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        deleteIdentifierFromReferenceList(itemReferencesList, id);
        deleteIdentifierFromReferenceList(atrezzoReferencesList, id);
        deleteIdentifierFromReferenceList(npcReferencesList, id);
        foreach (ElementContainer element in allReferencesDataControl)
        {
            if (element.getErdc() != null)
                element.getErdc().deleteIdentifierReferences(id);
        }
    }

    private void deleteIdentifierFromReferenceList(List<ElementReference> list, string id)
    {

        int i = 0;
        while (i < list.Count)
        {
            if (list[i].getTargetId().Equals(id))
            {
                deleteReferenceFromAll(list[i]);
                list.RemoveAt(i);
            }
            else
                i++;
        }
    }

    private void deleteReferenceFromAll(System.Object reference)
    {

        int i = 0;
        while (i < allReferencesDataControl.Count)
        {
            ElementContainer element = allReferencesDataControl[i];
            if (!element.isPlayer() && element.getErdc().getContent() == reference)
            {
                allReferencesDataControl.RemoveAt(i);
            }
            else
                i++;
        }
    }

    
    public override  bool canBeDuplicated()
    {

        return false;
    }

    public bool containsDataControl(ElementReferenceDataControl dataControl)
    {

        foreach (ElementContainer container in allReferencesDataControl)
        {
            if (!container.isPlayer() && container.getErdc() == dataControl)
                return true;
        }
        return false;
    }

    /**
     * Give the last introduced element container
     * 
     * @return The last introduced reference
     */
    public ElementContainer getLastElementContainer()
    {

        return lastElementContainer;
    }

    /**
     * Change the last element container
     * 
     * @param lastElementContainer
     *            the new element container
     */
    public void setLastElementContainer(ElementContainer lastElementContainer)
    {

        this.lastElementContainer = lastElementContainer;
    }

    public SceneDataControl getSceneDataControl()
    {

        return sceneDataControl;
    }

    /**
     * Put all id of the references in a string array
     * 
     * @return string[] Array of strings with the name of each element reference
     */
    public string[] getAllReferencesId()
    {

        string[] cont = new string[allReferencesDataControl.Count];
        for (int i = 0; i < cont.Length; i++)
            if (allReferencesDataControl[i].isPlayer())
                cont[i] = "Player";
            else
                cont[i] = allReferencesDataControl[i].getErdc().getElementId();
        return cont;
    }

    public int getPlayerPosition()
    {

        return playerPositionInAllReferences;
    }

    public void setPlayerPosition(int playerPosition)
    {

        this.playerPositionInAllReferences = playerPosition;
        this.sceneDataControl.setPlayerLayer(playerPosition);

    }

    public void deletePlayer()
    {

        if (playerPositionInAllReferences != NO_PLAYER)
        {
            allReferencesDataControl.RemoveAt(playerPositionInAllReferences);
            reassignLayerAllReferencesDataControl(playerPositionInAllReferences);
            playerPositionInAllReferences = NO_PLAYER;
            playerImagePath = null;
            sceneDataControl.setPlayerLayer(Scene.PLAYER_NO_ALLOWED);
        }
    }

    // this function was made to insert player in correct position in SwapPlayerModeTool
    // CAUTION!! dont check if has layer or if it is allowed, because where it is call that has been checked
    //			 dont call to setPlayerLayer() because it has been checked
    public void restorePlayer()
    {

        ElementContainer ec = new ElementContainer(null, sceneDataControl.getPlayerLayer(), AssetsController.getImage(this.playerImagePath));
        int layer = insertInOrder(ec, true);
        reassignLayerAllReferencesDataControl(layer);
    }

    public void addPlayer()
    {

        if (sceneDataControl.isForcedPlayerLayer())
        {
            playerImagePath = Controller.getInstance().getPlayerImagePath();
            ElementContainer ec = new ElementContainer(null, 0, AssetsController.getImage(this.playerImagePath));
            int layer = insertInOrder(ec, true);
            reassignLayerAllReferencesDataControl(layer);
            sceneDataControl.setPlayerLayer(layer);
        }

    }

    public void changeImagePlayerPath(string imagePath)
    {

        this.playerImagePath = imagePath;
        this.imagePathHasChanged = true;
        if (allReferencesDataControl.Count == 0)
        {
            playerPositionInAllReferences = 0;
            reassignLayerAllReferencesDataControl(insertInOrder(new ElementContainer(null, 0, AssetsController.getImage(this.playerImagePath)), true));
        }
    }

    
    public override void recursiveSearch()
    {

        if (this.getAtrezzoReferences() != null)
            foreach (DataControl dc in this.getAtrezzoReferences())
                dc.recursiveSearch();
        if (this.getItemReferences() != null)
            foreach (DataControl dc in this.getItemReferences())
                dc.recursiveSearch();
        if (this.getNPCReferences() != null)
            foreach (DataControl dc in this.getNPCReferences())
                dc.recursiveSearch();
    }

    public void setPlayerPositionInAllReferences(int playerPositionInAllReferences)
    {

        this.playerPositionInAllReferences = playerPositionInAllReferences;
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        List<Searchable> list = new List<Searchable>();
        foreach (ElementContainer container in allReferencesDataControl)
        {
            if (container.getErdc() != null)
                list.Add(container.getErdc());
        }
        return getPathFromChild(dataControl, list);
    }

    /**
     * Catch the type of the element reference control data and return the
     * associated scene preview category
     * 
     * @param type
     * @return the scene preview category
     */
    public static int transformType(int type)
    {

        int category = 0;
        // TODO: implement
        //if (type == Controller.ITEM_REFERENCE)
        //    category = ScenePreviewEditionPanel.CATEGORY_OBJECT;
        //else if (type == Controller.ATREZZO_REFERENCE)
        //    category = ScenePreviewEditionPanel.CATEGORY_ATREZZO;
        //else if (type == Controller.NPC_REFERENCE)
        //    category = ScenePreviewEditionPanel.CATEGORY_CHARACTER;
        //else if (type == -1)
        //    category = ScenePreviewEditionPanel.CATEGORY_PLAYER;
        return category;
    }
}

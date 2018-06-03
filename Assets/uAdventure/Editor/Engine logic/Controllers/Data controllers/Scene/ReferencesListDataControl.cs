using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    /**
     * Data control for the list of references in the scene
     */
    public class ReferencesListDataControl : DataControl
    {
        private struct AddRefUtil
        {
            public Type[] validElementTypes;
            public string title, message, errorMessage;
        }

        private static readonly Dictionary<int, AddRefUtil> AddRefUtils = new Dictionary<int, AddRefUtil>{
            {
                Controller.ITEM_REFERENCE,
                new AddRefUtil()
                {
                    title = "Operation.AddItemReferenceTitle",
                    message = "Operation.AddItemReferenceMessage",
                    errorMessage = "Operation.AddItemReferenceErrorNoItems",
                    validElementTypes = new Type[] { typeof(Item) }
                }
            },
            {
                Controller.ATREZZO_REFERENCE,
                new AddRefUtil()
                {
                    title = "Operation.AddAtrezzoReferenceTitle",
                    message = "Operation.AddAtrezzoReferenceMessage",
                    errorMessage = "Operation.AddReferenceErrorNoAtrezzo",
                    validElementTypes = new Type[] { typeof(Atrezzo) }
                }
            },
            {
                Controller.NPC_REFERENCE,
                new AddRefUtil()
                {
                    title = "Operation.AddNPCReferenceTitle",
                    message = "Operation.AddNPCReferenceMessage",
                    errorMessage = "Operation.AddReferenceErrorNoNPC",
                    validElementTypes = new Type[] { typeof(NPC) }
                }
            }
        };

        /**
         * Player image path
         */
        private string playerImagePath;

        /**
         * Scene controller that contains this element reference.
         */
        private SceneDataControl sceneDataControl;

        /// <summary>
        /// Refferences lists for any type
        /// </summary>
        private Dictionary<Type, List<ElementReference>> typeReferenceList;

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
        public ReferencesListDataControl(string playerImagePath, SceneDataControl sceneDataControl)
        {

            this.playerImagePath = playerImagePath;
            this.sceneDataControl = sceneDataControl;
            this.typeReferenceList = new Dictionary<Type, List<ElementReference>>();
            var content = sceneDataControl.getContent() as Scene;

            var itemReferencesList = content.getItemReferences();
            var atrezzoReferencesList = content.getAtrezzoReferences();
            var npcReferencesList = content.getCharacterReferences();

            getReferencesList(typeof(Item)).AddRange(itemReferencesList);
            getReferencesList(typeof(NPC)).AddRange(atrezzoReferencesList);
            getReferencesList(typeof(Atrezzo)).AddRange(npcReferencesList);

            this.allReferencesDataControl = new List<ElementContainer>();
            this.lastElementContainer = null;
            this.playerPositionInAllReferences = NO_PLAYER;
            this.imagePathHasChanged = false;
            // Check if one of references has layer -1: if it is true, it means that element references has no layer. 
            // Create subcontrollers

            bool hasLayerV = false;
            foreach (ElementReference itemReference in itemReferencesList)
            {
                int counter = count(itemReference);
                ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, itemReference, Controller.ITEM_REFERENCE, counter);
                insertInOrder(new ElementContainer(erdc, -1, null), hasLayerV |= (itemReference.getLayer() >= 0));
            }

            foreach (ElementReference atrezzoReference in atrezzoReferencesList)
            {
                int counter = count(atrezzoReference);
                ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, atrezzoReference, Controller.ATREZZO_REFERENCE, counter);
                insertInOrder(new ElementContainer(erdc, -1, null), hasLayerV |= (atrezzoReference.getLayer() >= 0));
            }

            foreach (ElementReference npcReference in npcReferencesList)
            {
                int counter = count(npcReference);
                ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, npcReference, Controller.NPC_REFERENCE, counter);
                insertInOrder(new ElementContainer(erdc, -1, null), hasLayerV |= (npcReference.getLayer() >= 0));
            }

            // insert player
            // by default, if player don´t have layer, we give it to him.
            if (playerImagePath != null && (!Controller.Instance.PlayTransparent) && sceneDataControl.isForcedPlayerLayer())
            {
                int layer;
                if (sceneDataControl.getPlayerLayer() == Scene.PLAYER_WITHOUT_LAYER)
                    layer = 0;
                else
                    layer = sceneDataControl.getPlayerLayer();
                reassignLayerAllReferencesDataControl(insertInOrder(new ElementContainer(null, layer, Controller.ResourceManager.getSprite(this.playerImagePath)), true));
            }
        }

        private List<ElementReference> getReferencesList(Type t)
        {
            if (!typeReferenceList.ContainsKey(t))
                typeReferenceList.Add(t, new List<ElementReference>());

            return typeReferenceList[t];
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
            foreach (var refference in allReferencesDataControl)
                return refference.getLayer() != Scene.PLAYER_WITHOUT_LAYER;

            return false;
        }

        public Sprite getPlayerImage()
        {
            //CHANGE: Now, the image of the player must be taken from
            return Controller.ResourceManager.getSprite(Controller.Instance.getPlayerImagePath());
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
            // While the element has not been added, and
            // we haven't checked every previous element
            int i = 0;
            bool added = false;
            while (!added && (i < allReferencesDataControl.Count))
            {
                if (hasLayer)
                {
                    if (element.getLayer() <= allReferencesDataControl[i].getLayer())
                    {
                        allReferencesDataControl.Insert(i, element);
                        added = true;
                    }
                }
                else
                {
                    if (element.getY() <= Mathf.Round(allReferencesDataControl[i].getY()))
                    {
                        allReferencesDataControl.Insert(i, element);
                        reassignLayerAllReferencesDataControl(i);
                        added = true;
                    }
                }
                i++;
            }

            // If the element wasn't added, add it in the last position
            if (!added)
            {
                allReferencesDataControl.Add(element);
                if (!hasLayer)
                    reassignLayerAllReferencesDataControl(Math.Max(0, i - 1));
            }
            return Math.Max(0, i - 1);
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
        public List<ElementReferenceDataControl> getRefferences()
        {
            return getRefferences(null);
        }
        public List<ElementReferenceDataControl> getRefferences<T>()
        {
            return getRefferences(typeof(T));
        }
        public List<ElementReferenceDataControl> getRefferences(Type t)
        {
            List<ElementReferenceDataControl> list = new List<ElementReferenceDataControl>();
            var ids = controller.IdentifierSummary;
            foreach (ElementContainer element in allReferencesDataControl)
            {
                if (element.getErdc() != null && ids.getType(element.getErdc().getElementId()) == t)
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
            return allReferencesDataControl;
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

            if (AddRefUtils.ContainsKey(type))
            {
                if (string.IsNullOrEmpty(id))
                {
                    List<string> elements = new List<string>();
                    var utils = AddRefUtils[type];
                    foreach (var t in utils.validElementTypes)
                        elements.AddRange(controller.IdentifierSummary.getIds(t));

                    if(elements.Count != 0)
                        controller.ShowInputDialog(TC.get(utils.title), TC.get(utils.message), elements.ToArray(), (o,s) => performAddElement(type, s));
                    else
                        controller.ShowErrorDialog(TC.get(utils.title), TC.get(utils.errorMessage));
                }
                else
                {
                    performAddElement(type, id);
                    elementAdded = true;
                }
            }

            return elementAdded;
        }

        private void performAddElement(int type, string id)
        {
            var elementType = controller.IdentifierSummary.getType(id);

            if (elementType != null)
            {
                ElementReference newElementReference = new ElementReference(id, 50, 50);
                var scene = sceneDataControl.getContent() as Scene;
                switch (type)
                {
                    case Controller.ITEM_REFERENCE: scene.addItemReference(newElementReference); break;
                    case Controller.NPC_REFERENCE: scene.addCharacterReference(newElementReference); break;
                    case Controller.ATREZZO_REFERENCE: scene.addAtrezzoReference(newElementReference); break;
                }

                int counter = count(newElementReference);
                ElementReferenceDataControl erdc = new ElementReferenceDataControl(sceneDataControl, newElementReference, type, counter);
                getReferencesList(elementType).Add(newElementReference);
                ElementContainer ec = new ElementContainer(erdc, -1, null);
                lastElementContainer = ec;
                reassignLayerAllReferencesDataControl(insertInOrder(ec, false));
            }
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
                // dataControl is ElementRefferenceDataControl
                var elementRef = (ElementReference)dataControl.getContent();
                var type = controller.IdentifierSummary.getType(elementRef.getTargetId());
                if(type != null)
                {
                    getReferencesList(type).Remove(elementRef);
                    delete(dataControl);
                    elementDeleted = true;
                }
                
            }
            return elementDeleted;
        }

        public void addElement(ElementContainer element)
        {
            var elementRef = (ElementReference)element.getErdc().getContent();
            if (elementRef == null)
                return;

            var type = controller.IdentifierSummary.getType(elementRef.getTargetId());
            if (type == null)
                return;

            getReferencesList(type).Add(elementRef);
            allReferencesDataControl.Insert(element.getLayer(), element);
            reassignLayerAllReferencesDataControl(element.getLayer());
        }

        private bool moveElement(DataControl dataControl, bool up)
        {
            bool moved = false;

            bool player;
            int index = 0;
            if (dataControl != null)
            {
                player = false;
                index = allReferencesDataControl.FindIndex(dc => !dc.isPlayer() && (dc == dataControl ||dc.getErdc() == dataControl));
            }
            else
            {
                player = true;
                index = playerPositionInAllReferences;
            }

            if (index > 0)
            {
                int toIndex = up ? index - 1 : index + 1;

                ElementContainer e = allReferencesDataControl[index];
                allReferencesDataControl.RemoveAt(index);
                allReferencesDataControl.Insert(toIndex, e);
                allReferencesDataControl[index].setLayer(index);
                allReferencesDataControl[toIndex].setLayer(toIndex);
                if (player)
                {
                    setPlayerPosition(toIndex);
                }
                if (allReferencesDataControl[index].isPlayer())
                {
                    setPlayerPosition(index);
                }
                moved = true;
            }

            return moved;
        }


        public override bool moveElementUp(DataControl dataControl)
        {
            return moveElement(dataControl, true);
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return moveElement(dataControl, false);
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
            var type = controller.IdentifierSummary.getType(id);
            if(id == null)
            {
                // If we cant find the id type, we try to remove it from them all just in case
                foreach (var kv in typeReferenceList)
                    deleteIdentifierFromReferenceList(kv.Value, id);
            }
            else
                deleteIdentifierFromReferenceList(getReferencesList(type), id);

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


        public override bool canBeDuplicated()
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

            ElementContainer ec = new ElementContainer(null, sceneDataControl.getPlayerLayer(), Controller.ResourceManager.getSprite(this.playerImagePath));
            int layer = insertInOrder(ec, true);
            reassignLayerAllReferencesDataControl(layer);
        }

        public void addPlayer()
        {

            if (sceneDataControl.isForcedPlayerLayer())
            {
                playerImagePath = Controller.Instance.getPlayerImagePath();
                ElementContainer ec = new ElementContainer(null, 0, Controller.ResourceManager.getSprite(this.playerImagePath));
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
                reassignLayerAllReferencesDataControl(insertInOrder(new ElementContainer(null, 0, Controller.ResourceManager.getSprite(this.playerImagePath)), true));
            }
        }


        public override void recursiveSearch()
        {
            foreach (var ec in getAllReferencesDataControl())
                ec.getErdc().recursiveSearch();
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
}
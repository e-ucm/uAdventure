using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    /**
     * This class can contain either an ElementReferenceDataControl or its
     * equivalent for the player
     */
    public class ElementReferenceDataControl : DataControl, IElementReference
    {

        /**
         * Scene controller that contains this element reference (used to extract
         * the id of the scene).
         */
        private SceneDataControl sceneDataControl;

        /**
         * Contained element reference.
         */
        private ElementReference elementReference;

        private InfluenceAreaDataControl influenceAreaDataControl;

        /**
         * Conditions controller.
         */
        private ConditionsController conditionsController;

        /**
         * The type of the element reference (item, npc or atrezzo)
         */
        private int type;

        private bool visible;

        public DataControl ReferencedDataControl
        {
            get { return getReferencedElementDataControl(); }
        }

        public string ReferencedId
        {
            get { return elementReference.getTargetId(); }
        }

        /**
         * Contructor.
         * 
         * @param sceneDataControl
         *            Parent scene controller
         * @param elementReference
         *            Element reference of the data control structure
         */
        public ElementReferenceDataControl(SceneDataControl sceneDataControl, ElementReference elementReference, int type, int referenceNumber)
        {

            this.sceneDataControl = sceneDataControl;
            this.elementReference = elementReference;
            this.type = type;
            this.visible = true;
            if (type == Controller.ITEM_REFERENCE || type == Controller.NPC_REFERENCE)
                this.influenceAreaDataControl = new InfluenceAreaDataControl(sceneDataControl, elementReference.getInfluenceArea(), this);

            // Create subcontrollers
            Dictionary<string, ConditionsController.ConditionContextProperty> context1 = new Dictionary<string, ConditionsController.ConditionContextProperty>();
            ConditionsController.ConditionOwner parent = new ConditionsController.ConditionOwner(Controller.SCENE, sceneDataControl.getId());
            ConditionsController.ConditionOwner owner = new ConditionsController.ConditionOwner(type, elementReference.getTargetId(), parent);
            context1.Add(ConditionsController.CONDITION_OWNER, owner);

            conditionsController = new ConditionsController(elementReference.Conditions, context1);
        }

        /**
         * Returns the conditions of the element reference.
         * 
         * @return Conditions of the element reference
         */
        public ConditionsController getConditions()
        {

            return conditionsController;
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

        public List<ExitDataControl> getParentSceneExitList()
        {

            return sceneDataControl.getExitsList().getExits();
        }

        public List<ActiveAreaDataControl> getParentSceneActiveAreaList()
        {

            return sceneDataControl.getActiveAreasList().getActiveAreas();
        }

        public List<BarrierDataControl> getParentSceneBarrierList()
        {

            return sceneDataControl.getBarriersList().getBarriers();
        }

        /**
         * Returns the id of the referenced element.
         * 
         * @return Id of the referenced element
         */
        public string getElementId()
        {

            return elementReference.getTargetId();
        }

        /**
         * Returns the x coordinate of the referenced element
         * 
         * @return X coordinate of the referenced element
         */
        public int getElementX()
        {

            return elementReference.getX();
        }

        /**
         * Returns the y coordinate of the referenced element
         * 
         * @return Y coordinate of the referenced element
         */
        public int getElementY()
        {

            return elementReference.getY();
        }

        /**
         * Returns the documentation of the element reference.
         * 
         * @return Element reference's documentation
         */
        public string getDocumentation()
        {

            return elementReference.getDocumentation();
        }

        /**
         * Sets a new next scene id.
         * 
         * @param elementId
         *            New next scene id
         */
        public void setElementId(string elementId)
        {

            // If the value is different
            controller.AddTool(new ChangeTargetIdTool(elementReference, elementId));
            //if( !elementId.equals( elementReference.getTargetId( ) ) ) {
            // Set the new element id, update the tree and modify the data
            //	elementReference.setTargetId( elementId );
            //	controller.updateTree( );
            //	controller.dataModified( );
            //}
        }

        /**
         * Sets the new position for the element reference.
         * 
         * @param x
         *            X coordinate for the element reference
         * @param y
         *            Y coordinate for the element reference
         */
        public void setElementPosition(int x, int y)
        {

            controller.AddTool(new ChangeElementReferenceTool(elementReference, x, y));
        }

        /**
         * Sets the new documentation of the element reference.
         * 
         * @param documentation
         *            Documentation of the element reference
         */
        public void setDocumentation(string documentation)
        {

            controller.AddTool(new ChangeDocumentationTool(elementReference, documentation));
        }

        /**
         * Get the scale for the element reference
         * 
         * @return the scale for the element reference
         */
        public float getElementScale()
        {

            return elementReference.Scale;
        }

        /**
         * Set the scale for the element reference
         * 
         * @param scale
         *            the scale for the element reference
         */
        public void setElementScale(float scale)
        {

            controller.AddTool(new ChangeElementReferenceTool(elementReference, scale));
        }

        /**
         * Set the position and scale for the element references
         * 
         * @param x
         *            X coordinate for the element reference
         * @param y
         *            Y coordinate for the element reference
         * @param scale
         *            the scale for the element reference
         */
        public void setElementPositionAndScale(int x, int y, float scale)
        {
            controller.AddTool(new ChangeElementReferenceTool(elementReference, x, y, scale));
        }


        public override System.Object getContent()
        {

            return elementReference;
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

            return false;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }


        public override string renameElement(string newName)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Update the flag summary with the conditions
            ConditionsController.updateVarFlagSummary(varFlagSummary, elementReference.Conditions);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            return true;
        }


        public override int countAssetReferences(string assetPath)
        {

            return 0;
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Do nothing
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;
            count += elementReference.getTargetId().Equals(id) ? 1 : 0;
            count += conditionsController.countIdentifierReferences(id);
            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            if (elementReference.getTargetId().Equals(oldId))
                elementReference.setTargetId(newId);
            conditionsController.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            conditionsController.deleteIdentifierReferences(id);
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Do nothing
        }


        public override bool canBeDuplicated()
        {

            return false;
        }

        /**
         * Return the element reference
         * 
         * @return the element reference
         */
        public ElementReference getElementReference()
        {

            return elementReference;
        }

        /**
         * 
         * @return The type of the current element reference
         */
        public int getType()
        {

            return type;
        }

        public SceneDataControl getSceneDataControl()
        {

            return sceneDataControl;
        }

        public InfluenceAreaDataControl getInfluenceArea()
        {

            return influenceAreaDataControl;
        }

        public DataControl getReferencedElementDataControl()
        {

            switch (type)
            {
                case Controller.ATREZZO_REFERENCE:
                    AtrezzoListDataControl aldc = Controller.Instance.SelectedChapterDataControl.getAtrezzoList();
                    foreach (AtrezzoDataControl adc in aldc.getAtrezzoList())
                    {
                        if (adc.getId().Equals(this.getElementId()))
                        {
                            return adc;
                        }
                    }
                    break;
                case Controller.NPC_REFERENCE:
                    NPCsListDataControl nldc = Controller.Instance.SelectedChapterDataControl.getNPCsList();
                    foreach (NPCDataControl ndc in nldc.getNPCs())
                    {
                        if (ndc.getId().Equals(this.getElementId()))
                        {
                            return ndc;
                        }
                    }
                    break;
                case Controller.ITEM_REFERENCE:
                    ItemsListDataControl ildc = Controller.Instance.SelectedChapterDataControl.getItemsList();
                    foreach (ItemDataControl idc in ildc.getItems())
                    {
                        if (idc.getId().Equals(this.getElementId()))
                        {
                            return idc;
                        }
                    }
                    break;
            }
            return null;

        }

        public override void recursiveSearch()
        {
            check(this.conditionsController, TC.get("Search.Conditions"));
            check(this.getDocumentation(), TC.get("Search.Documentation"));
            check(this.getElementId(), TC.get("Search.ElementID"));
        }

        public bool isVisible()
        {

            return visible;
        }

        public void setVisible(bool visible)
        {

            this.visible = visible;
        }

        public bool UsesOrientation
        {
            get
            {
                return type == Controller.NPC_REFERENCE || type == Controller.PLAYER;
            }
        }

        public Orientation Orientation
        {
            get { return elementReference.Orientation; }
            set { controller.AddTool(ChangeEnumValueTool.Create(elementReference, value, "Orientation")); }
        }

        public float Scale
        {
            get { return elementReference.Scale; }
            set { controller.AddTool(new ChangeFloatValueTool(elementReference, value, "Scale")); }
        }

        public bool Glow
        {
            get { return elementReference.Glow; }
            set { controller.AddTool(new ChangeBooleanValueTool(elementReference, value, "Glow")); }
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }
    }
}
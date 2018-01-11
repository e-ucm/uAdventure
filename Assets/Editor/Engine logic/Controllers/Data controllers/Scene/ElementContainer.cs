using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class ElementContainer : DataControl
    {


        private ElementReferenceDataControl erdc;

        private int playerLayer;

        private Sprite image;

        /**
         * Constructor. When erdc has value, takes the element reference data
         * control for references to atrezzo, items or non-player characters,
         * putting player layer with its non-valid value. Takes playerLayer for
         * player, when erdc is null.
         * 
         * @param erdc
         *            the element reference data control
         * @param playerLayer
         *            the layer to show the player in the correct position
         * @param image
         *            the image of the player
         */
        public ElementContainer(ElementReferenceDataControl erdc, int playerLayer, Sprite image)
        {
            if (erdc != null)
            {
                this.erdc = erdc;
                this.playerLayer = -1;
                this.image = null;
            }
            else
            {
                this.playerLayer = playerLayer;
                this.image = image;
            }
        }

        /**
         * Change the image
         * 
         * @param image
         *            the new image
         */
        public void setImage(Sprite image)
        {

            Controller.Instance.DataModified();
            this.image = image;
        }

        /**
         * Return the layer, checking if it is a player or not.
         * 
         * @return the layer.
         */
        public int getLayer()
        {

            if (erdc == null)
                return playerLayer;
            else
                return erdc.getElementReference().getLayer();
        }

        /**
         * Return the y position, checking if it is a player or not.
         * 
         * @return the y position.
         */
        public int getY()
        {

            if (erdc == null)
                return playerLayer;
            else
                return erdc.getElementReference().getY();
        }

        /**
         * Change the layer, checking if it is a player or not.
         * 
         * @param layer
         *            the new layer.
         */
        public void setLayer(int layer)
        {

            Controller.Instance.DataModified();
            if (erdc == null)
                playerLayer = layer;
            else
                erdc.getElementReference().setLayer(layer);
        }

        /**
         * Check if contains a player
         * 
         * @return true, if contains a player.
         */
        public bool isPlayer()
        {

            return erdc == null;
        }

        public int getPlayerLayer()
        {

            return playerLayer;
        }

        public ElementReferenceDataControl getErdc()
        {

            return erdc;
        }

        public Sprite getImage()
        {
            if (erdc != null)
            {
                int type = erdc.getType();
                string imagePath = string.Empty;

                if (type == Controller.ITEM_REFERENCE)
                    imagePath =
                        Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                            Controller.Instance                                .SelectedChapterDataControl                                .getItemsList()
                                .getItemIndexByID(erdc.getElementId())].getPreviewImage();
                else if (type == Controller.ATREZZO_REFERENCE)
                    imagePath =
                        Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[
                            Controller.Instance                                .SelectedChapterDataControl                                .getAtrezzoList()
                                .getAtrezzoIndexByID(erdc.getElementId())].getPreviewImage();
                else if (type == Controller.NPC_REFERENCE)
                    imagePath =
                        Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[
                            Controller.Instance                                .SelectedChapterDataControl                                .getNPCsList()
                                .getNPCIndexByID(erdc.getElementId())].getPreviewImage();

                if (!string.IsNullOrEmpty(imagePath))
                    image = AssetsController.getImage(imagePath);
            }
            return image;
        }

        public bool isVisible()
        {

            if (erdc == null)
            {
                return true;
            }
            return this.erdc.isVisible();
        }

        public void setVisible(bool visible)
        {

            if (erdc != null)
            {
                Controller.Instance.DataModified();
                this.erdc.setVisible(visible);
            }
        }


        // Embebbed erdc
        public override object getContent()
        {
            if (erdc != null)
                return erdc.getContent();
            return null;
        }

        public override int[] getAddableElements()
        {
            if (erdc != null)
                return erdc.getAddableElements();
            return null;
        }

        public override bool canAddElement(int type)
        {
            if (erdc != null)
                return erdc.canAddElement(type);
            return false;
        }

        public override bool canBeDeleted()
        {
            if (erdc != null)
                return erdc.canBeDeleted();
            return false;
        }

        public override bool canBeDuplicated()
        {
            if (erdc != null)
                return erdc.canBeDuplicated();
            return false;
        }

        public override bool canBeMoved()
        {
            if (erdc != null)
                return erdc.canBeMoved();
            return false;
        }

        public override bool canBeRenamed()
        {
            if (erdc != null)
                return erdc.canBeRenamed();
            return false;
        }

        public override bool addElement(int type, string id)
        {
            if (erdc != null)
                return erdc.addElement(type, id);
            return false;
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            if (erdc != null)
                return erdc.deleteElement(dataControl, askConfirmation);
            return false;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            if (erdc != null)
                return erdc.moveElementUp(dataControl);
            return false;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            if (erdc != null)
                return erdc.moveElementDown(dataControl);
            return false;
        }

        public override string renameElement(string newName)
        {
            if (erdc != null)
                return erdc.renameElement(newName);
            return string.Empty;
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            if (erdc != null)
                erdc.updateVarFlagSummary(varFlagSummary);
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            if (erdc != null)
                return erdc.isValid(currentPath, incidences);
            return true;
        }

        public override int countAssetReferences(string assetPath)
        {
            if (erdc != null)
                return erdc.countAssetReferences(assetPath);
            return 0;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            if (erdc != null)
                erdc.getAssetReferences(assetPaths, assetTypes);
        }

        public override void deleteAssetReferences(string assetPath)
        {
            if (erdc != null)
                erdc.deleteAssetReferences(assetPath);
        }

        public override int countIdentifierReferences(string id)
        {
            if (erdc != null)
                return erdc.countIdentifierReferences(id);
            return 0;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (erdc != null)
                erdc.replaceIdentifierReferences(oldId, newId);
        }

        public override void deleteIdentifierReferences(string id)
        {
            if (erdc != null)
                erdc.deleteIdentifierReferences(id);
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            if (erdc != null)
                return erdc.getPathToDataControl(dataControl);
            return null;
        }

        public override void recursiveSearch()
        {
            if (erdc != null)
                erdc.recursiveSearch();
        }
    }
}
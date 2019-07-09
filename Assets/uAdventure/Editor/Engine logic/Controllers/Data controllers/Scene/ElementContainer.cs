using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class ElementContainer : DataControl
    {
        private readonly ElementReferenceDataControl erdc;

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
            {
                return playerLayer;
            }
            else
            {
                return erdc.getElementReference().getLayer();
            }
        }

        /**
         * Return the y position, checking if it is a player or not.
         * 
         * @return the y position.
         */
        public int getY()
        {
            if (erdc == null)
            {
                return playerLayer;
            }
            else
            {
                return erdc.getElementReference().getY();
            }
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
            {
                playerLayer = layer;
            }
            else
            {
                erdc.getElementReference().setLayer(layer);
            }
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

                switch (type)
                {
                    default:
                        {
                            var items = Controller.Instance.SelectedChapterDataControl.getItemsList();
                            var itemIndex = items.getItemIndexByID(erdc.getElementId());
                            if(itemIndex != -1)
                            {
                                imagePath = items.getItems()[itemIndex].getPreviewImage();
                            }
                        }
                        break;
                    case Controller.ATREZZO_REFERENCE:
                        {
                            var atrezzos = Controller.Instance.SelectedChapterDataControl.getAtrezzoList();
                            var atrezzoIndex = atrezzos.getAtrezzoIndexByID(erdc.getElementId());
                            if (atrezzoIndex != -1)
                            {
                                imagePath = atrezzos.getAtrezzoList()[atrezzoIndex].getPreviewImage();
                            }
                        }
                        break;
                    case Controller.NPC_REFERENCE:
                        {
                            var npcs = Controller.Instance.SelectedChapterDataControl.getNPCsList();
                            var npcIndex = npcs.getNPCIndexByID(erdc.getElementId());
                            if (npcIndex != -1)
                            {
                                imagePath = npcs.getNPCs()[npcIndex].getPreviewImage(erdc.Orientation);
                            }
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(imagePath))
                {
                    image = Controller.ResourceManager.getSprite(imagePath);
                }
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
            return erdc != null ? erdc.getContent() : null;
        }

        public override int[] getAddableElements()
        {
            return erdc != null ? erdc.getAddableElements() : null;
        }

        public override bool canAddElement(int type)
        {
            return erdc != null && erdc.canAddElement(type);
        }

        public override bool canBeDeleted()
        {
            return erdc != null && erdc.canBeDeleted();
        }

        public override bool canBeDuplicated()
        {
            return erdc != null && erdc.canBeDuplicated();
        }

        public override bool canBeMoved()
        {
            return erdc != null && erdc.canBeMoved();
        }

        public override bool canBeRenamed()
        {
            return erdc != null && erdc.canBeRenamed();
        }

        public override bool addElement(int type, string id)
        {
            return erdc != null && erdc.addElement(type, id);
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            return erdc != null && erdc.deleteElement(dataControl, askConfirmation);
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            return erdc != null && erdc.moveElementUp(dataControl);
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return erdc != null && erdc.moveElementDown(dataControl);
        }

        public override string renameElement(string newName)
        {
            return erdc != null ? erdc.renameElement(newName) : string.Empty;
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            if (erdc != null)
            {
                erdc.updateVarFlagSummary(varFlagSummary);
            }
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return erdc == null || erdc.isValid(currentPath, incidences);
        }

        public override int countAssetReferences(string assetPath)
        {
            return erdc != null ? erdc.countAssetReferences(assetPath) : 0;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            if (erdc != null)
            {
                erdc.getAssetReferences(assetPaths, assetTypes);
            }
        }

        public override void deleteAssetReferences(string assetPath)
        {
            if (erdc != null)
            {
                erdc.deleteAssetReferences(assetPath);
            }
        }

        public override int countIdentifierReferences(string id)
        {
            return erdc != null ? erdc.countIdentifierReferences(id) : 0;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (erdc != null)
            {
                erdc.replaceIdentifierReferences(oldId, newId);
            }
        }

        public override void deleteIdentifierReferences(string id)
        {
            if (erdc != null)
            {
                erdc.deleteIdentifierReferences(id);
            }
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return erdc != null ? erdc.getPathToDataControl(dataControl) : null;
        }

        public override void recursiveSearch()
        {
            if (erdc != null)
            {
                erdc.recursiveSearch();
            }
        }
    }
}
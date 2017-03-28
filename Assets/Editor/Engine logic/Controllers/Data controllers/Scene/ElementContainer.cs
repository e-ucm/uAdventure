using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ElementContainer
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

            Controller.getInstance().dataModified();
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

            Controller.getInstance().dataModified();
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
                        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                            Controller.getInstance()
                                .getSelectedChapterDataControl()
                                .getItemsList()
                                .getItemIndexByID(erdc.getElementId())].getPreviewImage();
                else if (type == Controller.ATREZZO_REFERENCE)
                    imagePath =
                        Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList()[
                            Controller.getInstance()
                                .getSelectedChapterDataControl()
                                .getAtrezzoList()
                                .getAtrezzoIndexByID(erdc.getElementId())].getPreviewImage();
                else if (type == Controller.NPC_REFERENCE)
                    imagePath =
                        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                            Controller.getInstance()
                                .getSelectedChapterDataControl()
                                .getNPCsList()
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
                Controller.getInstance().dataModified();
                this.erdc.setVisible(visible);
            }
        }
    }
}
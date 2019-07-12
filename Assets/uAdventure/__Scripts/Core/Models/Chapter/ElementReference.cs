using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the data for a element reference in eAdventure
     */
    [System.Serializable]
    public class ElementReference : Context
    {

        /**
         * X position of the referenced element
         */
        [SerializeField]
        private int x;

        /**
         * Y position of the referenced element
         */
        [SerializeField]
        private int y;

        /**
         * The order in which will be drown this element
         */
        [SerializeField]
        private int layer;

        /**
         * The influenceArea of the object, used with trajectories
         */
        [SerializeField]
        private InfluenceArea influenceArea;

        /**
         * Creates a new ElementReference
         * 
         * @param idTarget
         *            the id of the element that is referenced
         * @param x
         *            the horizontal position of the element
         * @param y
         *            the vertical position of the element
         */

        public ElementReference(string idTarget, int x, int y) : this(idTarget, x, y, -1)
        {
        }

        /**
         * Creates a new ElementReference
         * 
         * @param idTarget
         *            the id of the element that is referenced
         * @param x
         *            the horizontal position of the element
         * @param y
         *            the vertical position of the element
         * @param layer
         *            the position where this element reference will be paint
         */

        public ElementReference(string idTarget, int x, int y, int layer) : base(idTarget)
        {
            this.x = x;
            this.y = y;
            this.layer = layer;
            influenceArea = new InfluenceArea();
        }

        /**
         * Returns the horizontal position of the element
         * 
         * @return the horizontal position of the element
         */

        public int getX()
        {

            return x;
        }

        /**
         * Returns the vertical position of the element
         * 
         * @return the vertical position of the element
         */

        public int getY()
        {

            return y;
        }
        

        /**
         * Sets the new position for the element reference.
         * 
         * @param x
         *            X coordinate of the element reference
         * @param y
         *            Y coordinate of the element reference
         */

        public void setPosition(int x, int y)
        {

            this.x = x;
            this.y = y;
        }

        /**
         * Get the layer for this element
         * 
         * @return layer
         */

        public int getLayer()
        {

            return layer;
        }

        /**
         * Changes the layer for this element
         * 
         * @param layer
         *            The new layer
         */

        public void setLayer(int layer)
        {

            this.layer = layer;
        }

        /**
         * @return the influenceArea
         */

        public InfluenceArea getInfluenceArea()
        {

            return influenceArea;
        }

        /**
         * @param influenceArea
         *            the influenceArea to set
         */

        public void setInfluenceArea(InfluenceArea influenceArea)
        {

            this.influenceArea = influenceArea;
        }

        public override object Clone()
        {
            ElementReference er = base.Clone() as ElementReference;
            er.influenceArea = (influenceArea != null ? (InfluenceArea)influenceArea.Clone() : null);
            er.layer = layer;
            er.x = x;
            er.y = y;
            return er;
        }
    }
}
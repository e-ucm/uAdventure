using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * The influence area for an item reference or active area
     */
    [System.Serializable]
    public class InfluenceArea : Rectangle, ICloneable
    {

        /**
         * True if the influence area exists (is defined)
         */
        [SerializeField]
        private bool exists = false;

        /**
         * The x axis value of the influence area, relative to the objects top left
         * corner
         */
        [SerializeField]
        private int x;

        /**
         * The y axis value of the influence area, relative to the objects top left
         * corner
         */
        [SerializeField]
        private int y;

        /**
         * The width of the active area
         */
        [SerializeField]
        private int width;

        /**
         * The height of the active area
         */
        [SerializeField]
        private int height;

        public InfluenceArea()
        {

        }

        /**
         * Creates a new influence area with the given parameters
         * 
         * @param x
         *            The x axis value
         * @param y
         *            The y axis value
         * @param width
         *            The width of the influence area
         * @param height
         *            The height of the influence area
         */
        public InfluenceArea(int x, int y, int width, int height)
        {

            exists = true;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /**
         * @return the exists
         */
        public bool isExists()
        {

            return exists;
        }

        /**
         * @param exists
         *            the exists to set
         */
        public void setExists(bool exists)
        {

            this.exists = exists;
        }

        /**
         * @return the x
         */
        public int getX()
        {

            return x;
        }

        /**
         * @param x
         *            the x to set
         */
        public void setX(int x)
        {

            if (x > 0)
                this.x = x;
        }

        /**
         * @return the y
         */
        public int getY()
        {

            return y;
        }

        /**
         * @param y
         *            the y to set
         */
        public void setY(int y)
        {

            if (y > 0)
                this.y = y;
        }

        /**
         * @return the width
         */
        public int getWidth()
        {

            return width;
        }

        /**
         * @param width
         *            the width to set
         */
        public void setWidth(int width)
        {

            if (width > 0)
                this.width = width;
        }

        /**
         * @return the height
         */
        public int getHeight()
        {

            return height;
        }

        /**
         * @param height
         *            the height to set
         */
        public void setHeight(int height)
        {

            if (height > 0)
                this.height = height;
        }

        public void setValues(int x, int y, int width, int height)
        {

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            InfluenceArea ia = (InfluenceArea) super.clone( );
            ia.exists = exists;
            ia.height = height;
            ia.width = width;
            ia.x = x;
            ia.y = y;
            return ia;
        }
        */
        public bool isRectangular()
        {

            return true;
        }

        public void setRectangular(bool rectangular)
        {

        }

        public List<Vector2> getPoints()
        {

            return null;
        }

        public object Clone()
        {
            InfluenceArea ia = (InfluenceArea)this.MemberwiseClone();
            ia.exists = exists;
            ia.height = height;
            ia.width = width;
            ia.x = x;
            ia.y = y;
            return ia;
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the data of an exit in eAdventure
     */
    public class Barrier : Element, Rectangle, ICloneable
    {
        /**
            * X position of the upper left corner of the exit
            */
        private int x;

        /**
         * Y position of the upper left corner of the exit
         */
        private int y;

        /**
         * Width of the exit
         */
        private int width;

        /**
         * Height of the exit
         */
        private int height;

        /**
         * Conditions of the active area
         */
        private Conditions conditions;

        /**
         * Creates a new Exit
         * 
         * @param x
         *            The horizontal coordinate of the upper left corner of the exit
         * @param y
         *            The vertical coordinate of the upper left corner of the exit
         * @param width
         *            The width of the exit
         * @param height
         *            The height of the exit
         */
        public Barrier(string id, int x, int y, int width, int height) : base(id)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            conditions = new Conditions();
        }

        /**
         * Returns the horizontal coordinate of the upper left corner of the exit
         * 
         * @return the horizontal coordinate of the upper left corner of the exit
         */
        public int getX()
        {

            return x;
        }

        /**
         * Returns the horizontal coordinate of the bottom right of the exit
         * 
         * @return the horizontal coordinate of the bottom right of the exit
         */
        public int getY()
        {

            return y;
        }

        /**
         * Returns the width of the exit
         * 
         * @return Width of the exit
         */
        public int getWidth()
        {

            return width;
        }

        /**
         * Returns the height of the exit
         * 
         * @return Height of the exit
         */
        public int getHeight()
        {

            return height;
        }

        /**
         * Set the values of the exit.
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
        public void setValues(int x, int y, int width, int height)
        {

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /**
         * @return the conditions
         */
        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * @param conditions
         *            the conditions to set
         */
        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Barrier b = (Barrier) super.clone( );
            b.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
            b.height = height;
            b.width = width;
            b.x = x;
            b.y = y;
            return b;
        }*/

        public override object Clone()
        {
            Barrier b = (Barrier)base.Clone();
            b.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            b.height = height;
            b.width = width;
            b.x = x;
            b.y = y;
            return b;
        }

        public List<Vector2> getPoints()
        {

            return null;
        }

        public bool isRectangular()
        {

            return true;
        }

        public void setRectangular(bool rectangular)
        {

        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the data of an active area in eAdventure
     */
    public class ActiveArea : Item, Rectangle, ICloneable
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
         * True if the active area is rectangular
         */
        private bool rectangular;

        /**
         * List of the Vector2s in the active area
         */
        private List<Vector2> Vector2s;

        /**
         * Conditions of the active area
         */
        private Conditions conditions;

        private InfluenceArea influenceArea;

        /**
         * Creates a new Exit
         * 
         * @param rectangular
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
        public ActiveArea(string id, bool rectangular, int x, int y, int width, int height) : base(id)
        {
            this.rectangular = rectangular;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            Vector2s = new List<Vector2>();
            conditions = new Conditions();
            influenceArea = new InfluenceArea();
        }

        /**
         * Returns the horizontal coordinate of the upper left corner of the exit
         * 
         * @return the horizontal coordinate of the upper left corner of the exit
         */
        public int getX()
        {

            if (rectangular)
                return x;
            else
            {
                int minX = int.MaxValue;
                foreach (Vector2 Vector2 in Vector2s)
                {
                    if (Vector2.x < minX)
                        minX = (int)Vector2.x;
                }
                return minX;
            }
        }

        /**
         * Returns the horizontal coordinate of the bottom right of the exit
         * 
         * @return the horizontal coordinate of the bottom right of the exit
         */
        public int getY()
        {

            if (rectangular)
                return y;
            else
            {
                int minY = int.MaxValue;
                foreach (Vector2 Vector2 in Vector2s)
                {
                    if (Vector2.y < minY)
                        minY = (int)Vector2.y;
                }
                return minY;
            }
        }

        /**
         * Returns the width of the exit
         * 
         * @return Width of the exit
         */
        public int getWidth()
        {

            if (rectangular)
                return width;
            else
            {
                int maxX = int.MinValue;
                int minX = int.MaxValue;
                foreach (Vector2 Vector2 in Vector2s)
                {
                    if (Vector2.x > maxX)
                        maxX = (int)Vector2.x;
                    if (Vector2.x < minX)
                        minX = (int)Vector2.x;
                }
                return maxX - minX;

            }
        }

        /**
         * Returns the height of the exit
         * 
         * @return Height of the exit
         */
        public int getHeight()
        {

            if (rectangular)
                return height;
            else
            {
                int maxY = int.MinValue;
                int minY = int.MaxValue;
                foreach (Vector2 Vector2 in Vector2s)
                {
                    if ((int)Vector2.y > maxY)
                        maxY = (int)Vector2.y;
                    if ((int)Vector2.y < minY)
                        minY = (int)Vector2.y;
                }
                return maxY - minY;
            }

        }

        public bool isRectangular()
        {

            return rectangular;
        }

        public List<Vector2> getPoints()
        {

            return Vector2s;
        }

        public void addVector2(Vector2 Vector2)
        {

            Vector2s.Add(Vector2);
        }

        /**
         * Set the values of the exit.
         * 
         * @param x
         *            X coordinate of the upper left Vector2
         * @param y
         *            Y coordinate of the upper left Vector2
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

            ActiveArea aa = (ActiveArea) super.clone( );

            //can not be two identical id
            string id = aa.getId( ) + "-" + ( new Random() ).nextInt( 1000 );
        aa.setId( id );
            aa.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
            aa.height = height;
            aa.width = width;
            aa.x = x;
            aa.y = y;
            aa.influenceArea = ( influenceArea != null ? (InfluenceArea) influenceArea.clone( ) : null );
            aa.rectangular = rectangular;
            aa.Vector2s = ( Vector2s != null ? new List<Vector2>( ) : null );
            for( Vector2 p : Vector2s )
                aa.Vector2s.add( (Vector2) p.clone( ) );
            return aa;
        }*/

        public void setRectangular(bool rectangular)
        {

            this.rectangular = rectangular;
        }

        public InfluenceArea getInfluenceArea()
        {

            return influenceArea;
        }

        public void setInfluenceArea(InfluenceArea influeceArea)
        {

            this.influenceArea = influeceArea;
        }

        public override object Clone()
        {
            ActiveArea aa = (ActiveArea)base.Clone();
            //can not be two identical id
            string id = aa.getId() + "-" + (new System.Random().Next(1000).ToString());
            aa.setId(id);
            aa.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            aa.height = height;
            aa.width = width;
            aa.x = x;
            aa.y = y;
            aa.influenceArea = (influenceArea != null ? (InfluenceArea)influenceArea.Clone() : null);
            aa.rectangular = rectangular;
            aa.Vector2s = (Vector2s != null ? new List<Vector2>() : null);
            foreach (Vector2 p in Vector2s)
                aa.Vector2s.Add(new Vector2(p.x, p.y));
            return aa;
        }
    }
}
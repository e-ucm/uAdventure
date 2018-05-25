using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * The object is a rectangle or polygon
     */
    public interface Rectangle
    {

        /**
         * Set the values of the rectangle
         * 
         * @param x
         *            The x axis value
         * @param y
         *            The y axis value
         * @param width
         *            The width of the rectangle
         * @param height
         *            The height of the rectangle
         */
        void setValues(int x, int y, int width, int height);

        /**
         * Get the x axis value
         * 
         * @return The x axis value
         */
        int getX();

        /**
         * Get the y axis value
         * 
         * @return The y axis value
         */
        int getY();

        /**
         * Get the width of the rectangle
         * 
         * @return The width of the rectangle
         */
        int getWidth();

        /**
         * Get the height of the rectangle
         * 
         * @return The height of the rectangle
         */
        int getHeight();

        /**
         * True if it is rectangular, false if it is a polygon
         * 
         * @return True if the object is rectangular
         */
        bool isRectangular();

        /**
         * Make the object rectangular (true) or a polygon (false)
         * 
         * @param rectangular
         *            The rectangular value
         */
        void setRectangular(bool rectangular);

        /**
         * Get the list of Vector2s for the polygon
         * 
         * @return The list of Vector2s of the polygon
         */
        List<Vector2> getPoints();
    }
}
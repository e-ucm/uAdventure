using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class DebugSettings
    {

        protected bool paintGrid;

        protected bool paintHotSpots;

        protected bool paintBoundingAreas;

        /**
         * Default constructor. Sets all parameters to false.
         */
        public DebugSettings() : this(false, false, false)
        {

        }

        /**
         * Constructor for "debug" mode. Sets debugMode automatically to true.
         */
        public DebugSettings(bool paintGrid, bool paintHotSpots, bool paintBoundingAreas)
        {
            this.paintGrid = paintGrid;
            this.paintHotSpots = paintHotSpots;
            this.paintBoundingAreas = paintBoundingAreas;
        }


        public bool isPaintGrid()
        {

            return paintGrid;
        }


        public void setPaintGrid(bool paintGrid)
        {

            this.paintGrid = paintGrid;
        }


        public bool isPaintHotSpots()
        {

            return paintHotSpots;
        }


        public void setPaintHotSpots(bool paintHotSpots)
        {

            this.paintHotSpots = paintHotSpots;
        }


        public bool isPaintBoundingAreas()
        {

            return paintBoundingAreas;
        }


        public void setPaintBoundingAreas(bool paintBoundingAreas)
        {

            this.paintBoundingAreas = paintBoundingAreas;
        }

    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangePositionTool : Tool
    {

        /**
         * Old position X (for undo/redo)
         */
        protected int oldX;

        /**
         * Old position Y (for undo/redo)
         */
        protected int oldY;

        /**
         * New position X
         */
        protected int x;

        /**
         * New position Y
         */
        protected int y;

        /**
         * The data which contains the (x,y) position
         */
        protected Positioned data;

        /**
         * Listeners. Useful to notify updates
         */
        protected List<ChangePositionToolListener> listeners;

        /**
         * Constructor
         * 
         * @param nextScene
         * @param newX
         * @param newY
         */
        public ChangePositionTool(Positioned data, int newX, int newY)
        {

            this.oldX = data.getPositionX();
            this.oldY = data.getPositionY();
            this.x = newX;
            this.y = newY;
            this.data = data;
            listeners = new List<ChangePositionToolListener>();
        }


        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return true;
        }


        public override bool combine(Tool other)
        {

            if (other is ChangePositionTool)
            {
                ChangePositionTool cpt = (ChangePositionTool)other;
                if (cpt.data == data)
                {
                    x = cpt.x;
                    y = cpt.y;
                    timeStamp = cpt.timeStamp;
                    return true;
                }
            }
            return false;
        }


        public override bool doTool()
        {

            bool done = false;
            // If the values are different
            if (x != data.getPositionX() || y != data.getPositionY())
            {
                // Set the new destiny position and modify the data
                data.setPositionX(x);
                data.setPositionY(y);
                done = true;
            }
            return done;
        }


        public override bool redoTool()
        {

            return undoTool();
        }


        public override bool undoTool()
        {

            data.setPositionX(oldX);
            data.setPositionY(oldY);
            int tempX = oldX;
            int tempY = oldY;
            oldX = x;
            oldY = y;
            x = tempX;
            y = tempY;
            // Notify listeners
            foreach (ChangePositionToolListener l in listeners)
            {
                l.positionUpdated(oldX, oldY);
            }
            Controller.Instance.updatePanel();
            return true;
        }

        public void addListener(ChangePositionToolListener listener)
        {

        }

        /**
         * Listener to notify changes in the tool
         * 
         */
        public interface ChangePositionToolListener
        {
            void positionUpdated(int newX, int newY);
        }
    }
}
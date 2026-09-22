using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MoveEffectInTableTool : Tool
    {

        public const int MODE_UP = 0;

        public const int MODE_DOWN = 1;

        private Effects effects;

        private int index;

        private int newIndex;

        private int mode;

        private List<ConditionsController> conditions;

        /**
         * Constructor.
         * 
         * @param list
         *            The List which contains the object to be moved
         * @param index
         *            The index of the object in the list
         * @param mode
         *            MODE_UP if the object must be moved one position up MODE_DOWN
         *            if the object must be moved one position down
         */
        public MoveEffectInTableTool(Effects effects, int index, int mode, List<ConditionsController> conditions)
        {

            this.effects = effects;
            this.index = index;
            this.mode = mode;
            this.conditions = conditions;
        }

        /**
         * Constructor.
         * 
         * @param list
         *            The List which contains the object to be moved
         * @param object
         *            The object in the list. It must be compulsorily in the list
         * @param mode
         *            MODE_UP if the object must be moved one position up MODE_DOWN
         *            if the object must be moved one position down
         */
        /*public MoveEffectInTableTool( List list, Object object, int mode ) {
            this (list, list.indexOf(object), mode);
        }*/

        public override bool canRedo()
        {

            return true;
        }

        public override bool canUndo()
        {

            return true;
        }

        public override bool doTool()
        {

            if (mode == MODE_UP)
                newIndex = moveUp();
            else if (mode == MODE_DOWN)
                newIndex = moveDown();
            return (newIndex != -1);
        }

        public override bool redoTool()
        {

            bool done = false;
            if (mode == MODE_UP)
                done = moveUp() != -1;
            else if (mode == MODE_DOWN)
                done = moveDown() != -1;

            if (done)
                Controller.Instance.updatePanel();
            return done;
        }

        public override bool undoTool()
        {

            bool done = false;
            if (mode == MODE_UP)
            {
                int temp = index;
                index = newIndex;
                done = moveDown() != -1;
                index = temp;
            }
            else if (mode == MODE_DOWN)
            {
                int temp = index;
                index = newIndex;
                done = moveUp() != -1;
                index = temp;

            }

            if (done)
                Controller.Instance.updatePanel();
            return done;

        }

        public override bool combine(Tool other)
        {

            return false;
        }

        private int moveUp()
        {

            int moved = -1;

            if (index > 0)
            {
                effects.getEffects().Insert(index - 1, effects.getEffects()[index]);
                effects.getEffects().RemoveAt(index);
                conditions.Insert(index - 1, conditions[index]);
                conditions.RemoveAt(index);
                moved = index - 1;
            }

            return moved;
        }

        private int moveDown()
        {

            int moved = -1;

            if (index < effects.getEffects().Count - 1)
            {
                effects.getEffects().Insert(index + 1, effects.getEffects()[index]);
                effects.getEffects().RemoveAt(index);
                conditions.Insert(index + 1, conditions[index]);
                conditions.RemoveAt(index);
                moved = index + 1;
            }

            return moved;
        }

    }
}
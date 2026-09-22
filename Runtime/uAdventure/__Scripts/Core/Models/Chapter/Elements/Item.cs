using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the data of an item in eAdventure
     */
    public class Item : Element, ICloneable
    {

        /**
         * The tag of the item's image
         */
        public const string RESOURCE_TYPE_IMAGE = "image";

        /**
         * The tag of the item's icon
         */
        public const string RESOURCE_TYPE_ICON = "icon";


        /**
         * The tag of the item's over image (added v1.4)
         */
        public const string RESOURCE_TYPE_IMAGEOVER = "imageover";

        /**
         * Behaviour: Added in 1.4. An item now can behave as:
         *      NORMAL: reacts to mouse overs by changing mouse cursor and text. Displays action buttons on click
         *      FIRST_ACTION: devised to simulate buttons. On click, triggers the first valid action defined (no buttons are displayed)
         *      ATREZZO: reacts to nothing. Useful to simulate atrezzos. NOTE: THIS OPTION WILL NOT BE PRESENT IN PUBLIC RELEASE
         * @author Javier Torrente
         *
         */
        public enum BehaviourType
        {
            NORMAL, FIRST_ACTION, ATREZZO
        }


        /**
         * Item's Behaviour (see {@link BehaviourTYpe}
         */
        private BehaviourType behaviour;

        /**
         * Feature added to release v1.4. When an item's appearance changes, it is possible to define a fade-in-out.
         * Very useful if combined with behaviours. Allows full implementation of buttons.
         */
        private long resourcesTransitionTime = 0; //By default transitions are instantaneous


        /**
         * Creates a new Item
         * 
         * @param id
         *            the id of the item
         */
        public Item(string id) : base(id)
        {
            this.behaviour = BehaviourType.NORMAL;
        }

        /*
         *  (non-Javadoc)
         * @see java.lang.Object#tostring()
         */
        /*
       @Override
       public string tostring()
       {

           stringBuffer sb = new stringBuffer(40);

           sb.append("\n");
           sb.append(super.tostring());
           for (Action action : actions)
               sb.append(action.tostring());

           sb.append("\n");

           return sb.tostring();
       }

       @Override
       public Object clone() throws CloneNotSupportedException
       {

           Item i = (Item) super.clone( );
           i.setBehaviour( this.behaviour );
           i.setResourcesTransitionTime( this.resourcesTransitionTime );
           return i;
       }
       */
        public override object Clone()
        {
            Item i = (Item)base.Clone();
            i.setBehaviour(this.behaviour);
            i.setResourcesTransitionTime(this.resourcesTransitionTime);
            return i;
        }
        /**
         * Returns this item's behaviour. 
         * @return Behaviour
         * see {@link BehaviourType for more info}
         */
        public BehaviourType getBehaviour()
        {
            return behaviour;
        }
        //For tools
        public int getBehaviourInteger()
        {
            return (int)behaviour;
        }

        public void setBehaviour(BehaviourType behaviour)
        {

            this.behaviour = behaviour;
        }

        // For tools
        public void setBehaviourInteger(int behaviour)
        {
            if (behaviour == (int)BehaviourType.ATREZZO)
            {
                this.behaviour = BehaviourType.ATREZZO;
            }
            else if (behaviour == (int)BehaviourType.NORMAL)
            {
                this.behaviour = BehaviourType.NORMAL;
            }
            else if (behaviour == (int)BehaviourType.FIRST_ACTION)
            {
                this.behaviour = BehaviourType.FIRST_ACTION;
            }
        }

        /**
         * Returns the transition time between changes of appearances. 
         * If it's 0, new appearance replaces the old one instantly.
         * If it's>0, a fade-in-out is performed to render this item for X miliseconds.
         * @return  X miliseconds for transition between changes of appearances.
         */
        //Long is used for tools
        public long getResourcesTransitionTime()
        {

            return resourcesTransitionTime;
        }
        //Long is used for tools
        public void setResourcesTransitionTime(long resourcesTransitionTime)
        {

            this.resourcesTransitionTime = resourcesTransitionTime;
        }

    }
}
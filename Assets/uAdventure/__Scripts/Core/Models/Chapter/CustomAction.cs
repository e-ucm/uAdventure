using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * An customizable action that can be done during the game
     * 
     */

    public class CustomAction : Action, Named, ICloneable
    {

        /**
         * Name of the action
         */
        private string name;

        /**
         * Resources used by the action (such as icons for the button)
         */
        private List<ResourcesUni> resources;

        /**
         * Default constructor for actions that only need one object
         * 
         * @param type
         *            The type of the custom action
         */

        public CustomAction(int type) : base(type)
        {
            resources = new List<ResourcesUni>();
        }

        /**
         * Constructor with id, for actions that need two objects
         * 
         * @param type
         *            the type of the custom action
         * @param idTarget
         *            the id of the other object
         */

        public CustomAction(int type, string idTarget) : base(type, idTarget)
        {
            resources = new List<ResourcesUni>();
        }

        /**
         * Constructor with conditions and effects, for actions that only need one
         * object
         * 
         * @param type
         *            the type of the action
         * @param conditions
         *            the conditions of the action
         * @param effects
         *            the effects of the action
         * @param notEffects
         *            The effects of the action when the conditions aren't OK (must
         *            not be null)
         */

        public CustomAction(int type, Conditions conditions, Effects effects, Effects notEffects)
            : base(type, conditions, effects, notEffects)
        {
            resources = new List<ResourcesUni>();
        }

        /**
         * Constructor with conditions and effects, for actions that need two
         * objects
         * 
         * @param type
         *            the type of the action
         * @param idTarget
         *            the id of the other object
         * @param conditions
         *            the conditions of the action
         * @param effects
         *            the effects of the action
         * @param notEffects
         *            The effects of the action when the conditions aren't OK (must
         *            not be null)
         */

        public CustomAction(int type, string idTarget, Conditions conditions, Effects effects, Effects notEffects,
            Effects clickEffects) : base(type, idTarget, conditions, effects, notEffects, clickEffects)
        {
            resources = new List<ResourcesUni>();
        }

        /**
         * Constructor that uses a default action
         * 
         * @param action
         *            a normal action
         */

        public CustomAction(Action action)
            : base(
                action.getType(), action.getTargetId(), action.getConditions(), action.getEffects(), action.getNotEffects(),
                action.getClickEffects())
        {
            resources = new List<ResourcesUni>();
        }

        /**
         * @param name
         *            the name to set
         */

        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * @return the name value
         */

        public string getName()
        {

            return name;
        }

        /**
         * @param resources
         *            the resources to add
         */

        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * @return the list of resources
         */

        public List<ResourcesUni> getResources()
        {

            return resources;
        }

        public override object Clone()
        {
            CustomAction ca = (CustomAction)base.Clone();
            ca.name = (name != null ? name : null);
            if (resources != null)
            {
                ca.resources = new List<ResourcesUni>();
                foreach (ResourcesUni r in resources)
                    ca.resources.Add((ResourcesUni)r.Clone());
            }
            return ca;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            CustomAction ca = (CustomAction) super.clone( );
            ca.name = ( name != null ? new string(name ) : null );
            if( resources != null ) {
                ca.resources = new List<Resources>( );
                for( Resources r : resources )
                    ca.resources.add( (Resources) r.clone( ) );
            }
            return ca;
        }*/
    }
}
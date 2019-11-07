using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the common data for any element in eAdventure. Here, element
     * means item or character
     */
    public abstract class Element : HasId, Documented, ICloneable
    {
        /**
             * The element's id
             */
        protected string id;

        /**
         * Documentation of the element.
         */
        private string documentation;

        /**
         * List of descriptions
         */
        protected List<Description> descriptions;

        /**
         * The element's set of resources
         */
        private List<ResourcesUni> resources;

        /**
         * List of actions associated with the item
         */
        protected List<Action> actions;

        /**
         * If true, the element returns to its last position when dragged
         */
        private bool returnsWhenDragged;

        /**
         * Creates a new element
         * 
         * @param id
         *            the element's id
         */
        public Element(string id)
        {

            this.id = id;

            this.returnsWhenDragged = true;
            resources = new List<ResourcesUni>();
            actions = new List<Action>();
            descriptions = new List<Description>();
            // descriptions.add( new Description() );
        }

        /**
         * Returns the element's id
         * 
         * @return the element's id
         */
        public string getId()
        {

            return id;
        }

        /**
         * Returns the documentation of the element.
         * 
         * @return the documentation of the element
         */
        public string getDocumentation()
        {

            return documentation;
        }


        /**
         * Returns the element's list of resources
         * 
         * @return the element's list of resources
         */
        public List<ResourcesUni> getResources()
        {

            return resources;
        }



        /**
         * Adds some resources to the list of resources
         * 
         * @param resources
         *            the resources to add
         */
        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * Sets the a new identifier for the element.
         * 
         * @param id
         *            New identifier
         */
        public void setId(string id)
        {

            this.id = id;
        }

        /**
         * Changes the documentation of this element.
         * 
         * @param documentation
         *            The new documentation
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Adds an action to this item
         * 
         * @param action
         *            the action to add
         */
        public void addAction(Action action)
        {

            actions.Add(action);
        }

        /**
         * Returns the list of actions of the item
         * 
         * @return the list of actions of the item
         */
        public List<Action> getActions()
        {

            return actions;
        }

        /**
         * Returns the size of the list of actions
         * 
         * @return Size (int) of the list of actions
         */
        public int getActionsCount()
        {

            if (actions == null)
                return 0;
            else
                return actions.Count;
        }

        /**
         * Returns Action object at place i
         * 
         * @param i
         * @return
         */
        public Action getAction(int i)
        {

            return actions[i];
        }

        /**
         * Changes the list of actions of the item
         * 
         * @param actions
         *            the new list of actions
         */
        public void setActions(List<Action> actions)
        {

            this.actions = actions;
        }


        /**
         * Changes the returns when dragged value
         * 
         * @param returnsWhenDragged
         *              the new value
         */
        public void setReturnsWhenDragged(bool returnsWhenDragged)
        {
            this.returnsWhenDragged = returnsWhenDragged;
        }

        /**
         * Returns if the element must return when dragged
         * 
         * @return true if the element must return when dragged
         */
        public bool isReturnsWhenDragged()
        {
            return returnsWhenDragged;
        }

        /*
         * (non-Javadoc)
         * 
         * @see java.lang.Object#tostring()
         */
        /*
       @Override
       public string tostring()
       {

           stringBuffer sb = new stringBuffer(40);

           //TODO ver que pasa ahora con  este tostring!!!!
           sb.append("Name: ");
           //sb.append( name );

           sb.append("\nDescription: ");
           //sb.append( description );

           sb.append("\nDetailed description:");
           //sb.append( detailedDescription );

           sb.append("\n");

           return sb.tostring();
       }
       */
        public List<Description> getDescriptions()
        {

            return descriptions;
        }

        public Description getDescription(int index)
        {

            return descriptions[index];

        }


        public void setDescriptions(List<Description> descriptions)
        {

            this.descriptions = descriptions;
        }

        public virtual object Clone()
        {
            Element e = (Element)this.MemberwiseClone();
            if (actions != null)
            {
                e.actions = new List<Action>();
                foreach (Action action in actions)
                {
                    e.actions.Add((Action)action.Clone());
                }
            }

            e.documentation = (documentation != null ? documentation : null);
            e.id = (id != null ? id : null);

            e.returnsWhenDragged = returnsWhenDragged;
            if (resources != null)
            {
                e.resources = new List<ResourcesUni>();
                foreach (ResourcesUni r in resources)
                {
                    e.resources.Add((ResourcesUni)r.Clone());
                }
            }
            return e;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Element e = (Element) super.clone( );
            if( actions != null ) {
                e.actions = new List<Action>();
                for (Action action : actions)
                {
                    e.actions.add((Action)action.clone());
                }
            }

            e.documentation = ( documentation != null ? new string(documentation ) : null );
            e.id = ( id != null ? new string(id ) : null );

            e.returnsWhenDragged = returnsWhenDragged;
            if( resources != null ) {
                e.resources = new List<Resources>( );
                for( Resources r : resources ) {
                    e.resources.add( (Resources) r.clone( ) );
                }
            }
            return e;
        }
        */
    }
}
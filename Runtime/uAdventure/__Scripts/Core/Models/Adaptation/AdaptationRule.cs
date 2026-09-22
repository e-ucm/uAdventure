using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class AdaptationRule : Described, HasId, ContainsAdaptedState, ICloneable
    {

        //ID
        private string id;

        // GameState
        private AdaptedState gameState;

        /**
         * List of properties to be set
         */
        private List<UOLProperty> uolState;

        //Description
        private string description;

        public AdaptationRule()
        {

            uolState = new List<UOLProperty>();
            gameState = new AdaptedState();
        }

        /**
         * Adds a new assessment property
         * 
         * @param property
         *            Assessment property to be added
         */
        public void addUOLProperty(UOLProperty property)
        {

            uolState.Add(property);
        }

        /**
         * Adds a new UOL property
         * 
         * @param id
         * @param value
         * @param op
         *            Operation of comparison between the value of var id in LMS and
         *            value
         */
        public void addUOLProperty(string id, string value, string op)
        {

            addUOLProperty(new UOLProperty(id, value, op));
        }

        public List<UOLProperty> getUOLProperties()
        {

            return uolState;
        }

        public void setInitialScene(string initialScene)
        {

            gameState.setTargetId(initialScene);

        }

        public void addActivatedFlag(string flag)
        {

            gameState.addActivatedFlag(flag);

        }

        public void addDeactivatedFlag(string flag)
        {

            gameState.addDeactivatedFlag(flag);

        }

        public void addVarValue(string var, string value)
        {

            gameState.addVarValue(var, value);
        }

        public AdaptedState getAdaptedState()
        {

            return gameState;
        }

        /**
         * @return the description
         */
        public string getDescription()
        {

            return description;
        }

        /**
         * @param description
         *            the description to set
         */
        public void setDescription(string description)
        {

            this.description = description;
        }

        public string getId()
        {

            return id;
        }

        public void setId(string generateId)
        {

            this.id = generateId;

        }

        public HashSet<string> getPropertyNames()
        {

            HashSet<string> names = new HashSet<string>();
            foreach (UOLProperty property in uolState)
            {
                names.Add(property.getId());
            }
            return names;
        }

        /**
         * Return the value of the property with specific key
         * 
         * @param key
         * @return
         */
        public string getPropertyValue(string key)
        {

            foreach (UOLProperty property in uolState)
            {
                if (property.getId().Equals(key))
                {
                    return property.getValue();
                }
            }
            return string.Empty;

        }

        /**
         * Return the operation of the property with specific key
         * 
         * @param key
         * @return
         */
        public string getPropertyOp(string key)
        {

            foreach (UOLProperty property in uolState)
            {
                if (property.getId().Equals(key))
                {
                    return property.getOperation();
                }
            }
            return string.Empty;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            AdaptationRule ar = (AdaptationRule) super.clone( );
            ar.description = ( description != null ? new string(description ) : null );
            ar.gameState = (AdaptedState) gameState.clone( );
            ar.id = ( id != null ? new string(id ) : null );
            ar.uolState = new List<UOLProperty>( );
            for( UOLProperty uolp : uolState ) {
                ar.uolState.add( (UOLProperty) uolp.clone( ) );
            }
            return ar;
        }*/

        public void setAdaptedState(AdaptedState state)
        {

            this.gameState = state;
        }

        public object Clone()
        {
            AdaptationRule ar = (AdaptationRule)this.MemberwiseClone();
            ar.description = (description != null ? description : null);
            ar.gameState = (AdaptedState)gameState.Clone();
            ar.id = (id != null ? id : null);
            ar.uolState = new List<UOLProperty>();
            foreach (UOLProperty uolp in uolState)
            {
                ar.uolState.Add((UOLProperty)uolp.Clone());
            }
            return ar;
        }
    }
}
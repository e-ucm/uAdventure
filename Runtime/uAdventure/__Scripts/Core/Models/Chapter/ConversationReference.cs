using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the data of a conversation reference in eAdventure
     */

    public class ConversationReference : Documented, HasTargetId, ICloneable
    {

        /**
         * Id of the target conversation
         */
        private string idTarget;

        /**
         * Documentation of the conversation reference.
         */
        private string documentation;

        /**
         * Conditions to trigger the conversation
         */
        private Conditions conditions;

        /**
         * Creates a new ConversationReference
         * 
         * @param idTarget
         *            the id of the conversation that is referenced
         */

        public ConversationReference(string idTarget)
        {

            this.idTarget = idTarget;

            documentation = null;
            conditions = new Conditions();
        }

        /**
         * Returns the id of the conversation that is referenced
         * 
         * @return the id of the conversation that is referenced
         */

        public string getTargetId()
        {

            return idTarget;
        }

        /**
         * Returns the documentation of the conversation.
         * 
         * @return the documentation of the conversation
         */

        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * Returns the conditions for this conversation
         * 
         * @return the conditions for this conversation
         */

        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * Sets the new conversation id target.
         * 
         * @param idTarget
         *            Id of the referenced conversation
         */

        public void setTargetId(string idTarget)
        {

            this.idTarget = idTarget;
        }

        /**
         * Changes the documentation of this conversation reference.
         * 
         * @param documentation
         *            The new documentation
         */

        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Changes the conditions for this conversation
         * 
         * @param conditions
         *            the new conditions
         */

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        public object Clone()
        {
            ConversationReference cr = (ConversationReference)this.MemberwiseClone();
            cr.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            cr.documentation = (documentation != null ? documentation : null);
            cr.idTarget = (idTarget != null ? idTarget : null);
            return cr;
        }

        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       ConversationReference cr = (ConversationReference) super.clone( );
       cr.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
       cr.documentation = ( documentation != null ? new string(documentation ) : null );
       cr.idTarget = ( idTarget != null ? new string(idTarget ) : null );
       return cr;
    }*/
    }
}
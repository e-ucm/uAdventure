using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Group of conditions named with an Id, so it can be refered to in diverse
     * points of the chapter
     * 
     */
    public class GlobalState : Conditions, Documented, HasId, ICloneable
    {

        /**
       * Id of the Conditions group
       */
        private string id;

        /**
         * Documentation (not used in game engine)
         */
        private string documentation;

        /**
         * Constructor
         */
        public GlobalState(string id) : base()
        {
            this.id = id;
            this.documentation = string.Empty;
        }

        /**
         * @return the id
         */
        public string getId()
        {

            return id;
        }

        /**
         * @param id
         *            the id to set
         */
        public void setId(string id)
        {

            this.id = id;
        }

        /**
         * @return the documentation
         */
        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * @param documentation
         *            the documentation to set
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            GlobalState gs = (GlobalState) super.clone( );
            gs.documentation = ( documentation != null ? new string(documentation ) : null );
            gs.id = ( id != null ? new string(id ) : null );
            return gs;
        }*/

        public override object Clone()
        {
            GlobalState gs = (GlobalState)base.Clone();
            gs.documentation = (documentation != null ? documentation : null);
            gs.id = (id != null ? id : null);
            return gs;
        }
    }
}
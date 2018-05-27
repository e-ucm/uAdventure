using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Group of effects named with an Id, so it can be refered to in diverse points
     * of the chapter
     * 
     */
    public class Macro : Effects, Documented, HasId
    {
        /**
             * Id of the Effects group
             */
        private string id;

        /**
         * Documentation (not used in game engine)
         */
        private string documentation;

        /**
         * Constructor
         */
        public Macro(string id) : base()
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

            Macro m = (Macro) super.clone( );
            m.documentation = ( documentation != null ? new string(documentation ) : null );
            m.id = ( id != null ? new string(id ) : null );
            return m;
        }*/
    }
}
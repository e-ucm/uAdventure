using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An based on a reference to a macro
     * 
     * @author Javier
     * 
     */
    public class MacroReferenceEffect : AbstractEffect, HasTargetId
    {
        /**
            * The id
            */
        private string macroId;

        /**
         * Constructor
         * 
         * @param flagVar
         * @param state
         */
        public MacroReferenceEffect(string id) : base()
        {
            macroId = id;
        }

        public override EffectType getType()
        {

            return EffectType.MACRO_REF;
        }

        /**
         * @return the macroId
         */
        public string getTargetId()
        {

            return macroId;
        }

        /**
         * @param macroId
         *            the macroId to set
         */
        public void setTargetId(string macroId)
        {

            this.macroId = macroId;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            MacroReferenceEffect mre = (MacroReferenceEffect) super.clone( );
            mre.macroId = ( macroId != null ? new string(macroId ) : null );
            return mre;
        }*/
    }
}
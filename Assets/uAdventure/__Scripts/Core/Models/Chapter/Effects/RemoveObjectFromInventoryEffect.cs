namespace uAdventure.Core
{
    public class RemoveObjectFromInventoryEffect : AbstractEffect, HasTargetId
    {

        /**
         * Id of the item to be consumed
         */
        private string idTarget;

        /**
         * Creates a new ConsumeObjectEffect.
         * 
         * @param idTarget
         *            the id of the object to be consumed
         */
        public RemoveObjectFromInventoryEffect(string idTarget) : base()
        {
            this.idTarget = idTarget;
        }

        public override EffectType getType()
        {

            return EffectType.CUSTOM_EFFECT;
        }

        /**
         * Returns the idTarget
         * 
         * @return string containing the idTarget
         */
        public string getTargetId()
        {

            return idTarget;
        }

        /**
         * Sets the new idTarget
         * 
         * @param idTarget
         *            New idTarget
         */
        public void setTargetId(string idTarget)
        {

            this.idTarget = idTarget;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            ConsumeObjectEffect coe = (ConsumeObjectEffect) super.clone( );
            coe.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            return coe;
        }*/
    }
}
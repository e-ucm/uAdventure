using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that cancels the standard action
     */
    public class CancelActionEffect : AbstractEffect
    {

        public CancelActionEffect() : base()
        {
        }

        public override EffectType getType()
        {

            return EffectType.CANCEL_ACTION;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            CancelActionEffect cae = (CancelActionEffect) super.clone( );
            return cae;
        }*/
    }
}
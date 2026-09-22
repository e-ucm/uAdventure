using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class TriggerLastSceneEffect : AbstractEffect
    {

        public TriggerLastSceneEffect() : base()
        {
        }

        public override EffectType getType()
        {

            return EffectType.TRIGGER_LAST_SCENE;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TriggerLastSceneEffect tlse = (TriggerLastSceneEffect) super.clone( );
            return tlse;
        }*/
    }
}
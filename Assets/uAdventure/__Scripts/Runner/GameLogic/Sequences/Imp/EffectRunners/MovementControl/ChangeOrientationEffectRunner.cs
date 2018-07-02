using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Runner
{
    [CustomEffectRunner(typeof(ChangeOrientationEffect))]
    public class ChangeOrientationEffectRunner : CustomEffectRunner
    {
        ChangeOrientationEffect effect;

        public IEffect Effect { get { return effect; } set { effect = value as ChangeOrientationEffect; } }

        public bool execute()
        {
            var target = GameObject.Find(effect.getTargetId());
            if (target)
            {
                var representable = target.GetComponent<Representable>();
                if (representable)
                {
                    representable.Orientation = effect.GetOrientation();
                }
            }
            return false;
        }
    }
}
using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;

namespace uAdventure.Geo
{
    public class TriggerZonedSceneEffect : TriggerSceneEffect
    {

        public TriggerZonedSceneEffect(string targetSceneId, string zoneId, int x, int y, float destinyScale = float.MinValue, int transitionTime = 0, int transitionType = 0) : base(targetSceneId, x, y, destinyScale, transitionTime, transitionType)
        {
            ZoneId = zoneId;
        }

        public override EffectType getType()
        {
            return EffectType.CUSTOM_EFFECT;
        }

        public string ZoneId { get; set; }
    }
}
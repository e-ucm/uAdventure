using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    public enum NavigationType { Ordered, Closeness }

    public class NavigationStep
    {
        public NavigationStep(MapElement destination = null, bool lockNavigation = false)
        {
            Destination = destination;
            LockNavigation = lockNavigation;
        }

        public bool LockNavigation { get; set; }
        public MapElement Destination { get; set; }
    }

    public class NavigateEffect : AbstractEffect
    {

        public override EffectType getType()
        {
            return EffectType.CUSTOM_EFFECT;
        }

        public MapScene MapScene { get; set; }
        public List<MapElement> ElementsPath { get; set; }

        public NavigationType NavigationType { get; set; }

    }

}
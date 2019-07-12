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
        public NavigationStep(string destination = null, bool lockNavigation = false)
        {
            Reference = destination;
            LockNavigation = lockNavigation;
        }

        public bool LockNavigation { get; set; }
        public string Reference { get; set; }
    }

    public class NavigateEffect : AbstractEffect
    {
        public NavigateEffect()
        {
            Steps = new List<NavigationStep>();
        }

        public override EffectType getType()
        {
            return EffectType.CUSTOM_EFFECT;
        }
        
        public List<NavigationStep> Steps { get; set; }
        public NavigationType NavigationType { get; set; }

    }

}
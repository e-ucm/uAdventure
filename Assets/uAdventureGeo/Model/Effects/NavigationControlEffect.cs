using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;

namespace uAdventure.Geo
{
    public class NavigationControlEffect : AbstractEffect
    {
        public enum ControlType { Next, Previous, Index, ReferenceId }

        public override EffectType getType()
        {
            return EffectType.CUSTOM_EFFECT;
        }
        
        public NavigationControlEffect()
        {
            Type = ControlType.Next;
            Index = 0;
            Reference = "";
        }

        public ControlType Type { get; set; }
        public int Index { get; set; }
        public string Reference { get; set; }
    }
}
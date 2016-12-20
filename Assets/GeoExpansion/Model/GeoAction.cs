using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;

namespace uAdventure.Geo
{
    public abstract class GeoAction : ICloneable
    {
        public GeoAction()
        {
            Conditions = new Conditions();
            Effects = new Effects();
        }

        public abstract string Name { get; }
        public Conditions Conditions { get; set; }
        public Effects Effects { get; set; }
        public virtual object Clone()
        {
            var clone = MemberwiseClone() as GeoAction;
            clone.Conditions = Conditions.Clone() as Conditions;
            clone.Effects = Effects.Clone() as Effects;
            return clone;
        }
    }
}
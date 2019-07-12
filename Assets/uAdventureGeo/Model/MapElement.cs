using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;

namespace uAdventure.Geo
{
    public abstract class MapElement : Context
    {
        public int Layer { get; set; }

        protected MapElement(string idTarget) : base(idTarget)
        {
        }

        public override object Clone()
        {
            var clone = MemberwiseClone() as MapElement;
            clone.Conditions = Conditions.Clone() as Conditions;

            return clone;
        }
    }
}



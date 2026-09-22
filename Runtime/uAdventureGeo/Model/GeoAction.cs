using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    public abstract class GeoAction : ICloneable
    {
        protected Dictionary<string, object> parameterValues;

        protected GeoAction()
        {
            Conditions = new Conditions();
            Effects = new Effects();
            parameterValues = new Dictionary<string, object>();
        }

        public abstract string Name { get; }
        public abstract string[] Parameters { get; }

        public object this[string parameter]
        {
            get { return parameterValues[parameter]; }
            set { parameterValues[parameter] = value; }
        }

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
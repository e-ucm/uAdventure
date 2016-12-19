using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;

namespace uAdventure.Geo
{
    public abstract class GeoAction : ICloneable
    {
        public abstract string Name { get; }
        public Conditions Conditions { get; set; }
        public Effects Effects { get; set; }
        public abstract object Clone();
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{
    public class ScreenDescriptor : ITransformManagerDescriptor
    {

        public ScreenDescriptor()
        {
            ParameterDescription = new Dictionary<string, Geo.ParameterDescription>
            {
                { "Position", new Geo.ParameterDescription(typeof(Vector2), Vector2.zero) },
                { "Rotation", new Geo.ParameterDescription(typeof(float), 0f) }
            };
        }

        public string Name { get { return "ScreenPositioned"; } }
        public Dictionary<string, ParameterDescription> ParameterDescription { get; private set; }
        public Type Type { get { return typeof(ScreenTransformManager); } }
    }
}

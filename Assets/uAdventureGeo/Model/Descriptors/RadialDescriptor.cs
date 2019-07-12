using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{

    public class RadialDescriptor : ITransformManagerDescriptor
    {

        public RadialDescriptor()
        {
            ParameterDescription = new Dictionary<string, ParameterDescription>
            {
                { "Degree", new ParameterDescription(typeof(float), 0f) },
                { "Distance", new ParameterDescription(typeof(float), 50f) },
                { "Rotation", new ParameterDescription(typeof(float), 0f) },
                { "RotateAround", new ParameterDescription(typeof(bool), true) }
            };
        }

        public string Name { get { return "RadialToCenterPositioned"; } }
        public Dictionary<string, ParameterDescription> ParameterDescription { get; private set; }
        public Type Type { get { return typeof(RadialTransformManager); } }
    }
}

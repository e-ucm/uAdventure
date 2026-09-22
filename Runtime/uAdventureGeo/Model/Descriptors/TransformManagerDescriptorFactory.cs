using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{

    public class TransformManagerDescriptorFactory
    {

        private readonly List<ITransformManagerDescriptor> descriptors;

        private static TransformManagerDescriptorFactory instance;
        public static TransformManagerDescriptorFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TransformManagerDescriptorFactory();
                }
                return instance;
            }
        }

        private TransformManagerDescriptorFactory()
        {
            descriptors = new List<ITransformManagerDescriptor>();
            AvaliableTransformManagers = new Dictionary<Type, string>();

            // Add descriptrs here

            descriptors.Add(new GeopositionedDescriptor());
            descriptors.Add(new ScreenDescriptor());
            descriptors.Add(new RadialDescriptor());

            // End add descriptors

            descriptors.ForEach(d => AvaliableTransformManagers.Add(d.GetType(), d.Name));
        }

        public Dictionary<Type, string> AvaliableTransformManagers { get; private set; }

        public ITransformManagerDescriptor CreateDescriptor(Type type)
        {
            return Activator.CreateInstance(type) as ITransformManagerDescriptor;
        }
    }
}

using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{

    public class TransformManagerFactory
    {
        private static TransformManagerFactory instance;
        public static TransformManagerFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TransformManagerFactory();
                }
                return instance;
            }
        }

        public ITransformManager CreateInstance(ITransformManagerDescriptor element, Dictionary<string, object> parameters)
        {
            var elem = (ITransformManager)Activator.CreateInstance(element.Type);
            elem.Configure(parameters);
            return elem;
        }
    }
}

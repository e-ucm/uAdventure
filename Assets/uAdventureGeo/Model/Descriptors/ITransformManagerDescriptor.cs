using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    public interface ITransformManagerDescriptor
    {
        /// <summary>
        /// Name to display in the interface selector
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type to utilize in the factory
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Parameter description for the editor to preconfigure
        /// </summary>
        Dictionary<string, ParameterDescription> ParameterDescription { get; }
    }
}

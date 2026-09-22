using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{

    /// <summary>
    /// GeoReferenceTransformManager interface for creating new positioning types
    /// </summary>
    public interface ITransformManager
    {
        /// <summary>
        /// Transform of the referenced element to update
        /// </summary>
        Transform ExtElemReferenceTransform { get; set; }

        /// <summary>
        /// Context for the element
        /// </summary>
        ExtElemReference Context { get; set; }

        /// <summary>
        /// Allows the ExtElemReference to reposition the element
        /// </summary>
        void Update();

        /// <summary>
        /// Configures the Transform Manager
        /// </summary>
        /// <param name="parameters">Dictionary of parameters to extract</param>
        void Configure(Dictionary<string, object> parameters);
    }
}

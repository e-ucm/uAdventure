using UnityEngine;
using System.Collections.Generic;
using System;

namespace uAdventure.Geo
{

    public class ExtElemReference : MapElement
    {
        public ExtElemReferenceTransformManagerDescriptor TransformManagerDescriptor { get; set; }
        public Dictionary<string, object> TransformManagerParameters { get; set; }
        
        public Vector3 Scale { get; set; }
        public Vector2 Position { get; set; }
    }

    /// <summary>
    /// Parameter description for GeoReferenceTransformManager Parameters
    /// </summary>
    public class ParameterDescription
    {
        public System.Type Type { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }
        public List<object> AllowedValues { get; set; }
    }


    public interface ExtElemReferenceTransformManagerDescriptor
    {
        /// <summary>
        /// Name to display in the interface selector
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type to utilize in the factory
        /// </summary>
        System.Type Type { get; }

        /// <summary>
        /// Parameter description for the editor to preconfigure
        /// </summary>
        Dictionary<string, ParameterDescription> ParameterDescription { get; }
    }

    /// <summary>
    /// GeoReferenceTransformManager interface for creating new positioning types
    /// </summary>
    public interface ExtElemReferenceTransformManager
    {
        /// <summary>
        /// Transform of the referenced element to update
        /// </summary>
        Transform ExtElemReferenceTransform { get; set; }

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

    public class ExtElemReferenceTransformManagerFactory
    {
        private static ExtElemReferenceTransformManagerFactory instance;
        public static ExtElemReferenceTransformManagerFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new ExtElemReferenceTransformManagerFactory();
                return instance;
            }
        }

        public ExtElemReferenceTransformManager CreateInstance(ExtElemReferenceTransformManagerDescriptor element, Dictionary<string, object> parameters)
        {
            var elem = (ExtElemReferenceTransformManager) Activator.CreateInstance(element.Type);
            elem.Configure(parameters);
            return elem;
        }
    }

}


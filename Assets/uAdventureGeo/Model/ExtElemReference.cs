using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Geo
{

    [Serializable]
    public class ExtElemReference : MapElement, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<string> parameters;
        [SerializeField]
        private List<string> values;
        [SerializeField]
        private List<string> types;
        [SerializeField]
        private List<GeoAction> geoActions;
        public List<GeoAction> Actions { get { return geoActions; } set { geoActions = value; } }

        public ExtElemReference(string idTarget) : base(idTarget)
        {
            TransformManagerParameters = new Dictionary<string, object>();
            var keys = TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.Keys.GetEnumerator();
            if (keys.MoveNext())
            {
                // If there is at least one descriptor, we create the first one avaliable by default
                TransformManagerDescriptor = TransformManagerDescriptorFactory.Instance
                    .CreateDescriptor(keys.Current);
            }

            geoActions = new List<GeoAction>();
        }

        public ITransformManagerDescriptor TransformManagerDescriptor { get; set; }
        public Dictionary<string, object> TransformManagerParameters { get; set; }

        public void OnAfterDeserialize()
        {
            if (parameters == null)
            {
                return;
            }

            TransformManagerParameters = new Dictionary<string, object>();
            for (int i = 0; i < parameters.Count; i++)
            {
                TransformManagerParameters.Add(parameters[i], ExParsers.Parse(values[i], Type.GetType(types[i])));
            }
        }

        public void OnBeforeSerialize()
        {
            if (TransformManagerParameters == null)
            {
                return;
            }

            parameters = TransformManagerParameters.Keys.ToList();
            values = TransformManagerParameters.Values.Select(v => v.ToString()).ToList();
        }
    }
}
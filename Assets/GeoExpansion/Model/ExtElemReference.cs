using UnityEngine;
using System.Collections.Generic;
using System;
using MapzenGo.Helpers;
using uAdventure.Runner;
using uAdventure.Core;
using AssetPackage;

namespace uAdventure.Geo
{

    public class ExtElemReference : MapElement
    {
        public ExtElemReference(string targetId) : base(targetId)
        {
            TransformManagerParameters = new Dictionary<string, object>();
            var keys = TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.Keys.GetEnumerator();
            if (keys.MoveNext())
            {
                // If there is at least one descriptor, we create the first one avaliable by default
                TransformManagerDescriptor = TransformManagerDescriptorFactory.Instance
                    .CreateDescriptor(keys.Current);
            }
        }

        public ExtElemReferenceTransformManagerDescriptor TransformManagerDescriptor { get; set; }
        public Dictionary<string, object> TransformManagerParameters { get; set; }
    }

    /// <summary>
    /// Parameter description for GeoReferenceTransformManager Parameters
    /// </summary>
    public class ParameterDescription
    {
        public ParameterDescription(System.Type type, object defaultValue) : this(type, defaultValue, null, null, null) { }
        public ParameterDescription(System.Type type, object defaultValue, object minValue, object maxValue) : this(type, defaultValue, minValue, maxValue, null) { }
        public ParameterDescription(System.Type type, object defaultValue, List<object> allowedValues) : this(type, defaultValue, null, null, allowedValues) { }
        public ParameterDescription(System.Type type, object defaultValue, object minValue, object maxValue, List<object> allowedValues)
        {
            this.Type = type;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.AllowedValues = allowedValues;
            this.DefaultValue = defaultValue;
        }

        public System.Type Type { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }
        public List<object> AllowedValues { get; set; }
        public object DefaultValue { get; set; }
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
        /// Context for the element
        /// </summary>
        ElementReference Context { get; set; }

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

    public class TransformManagerDescriptorFactory
    {

        private List<ExtElemReferenceTransformManagerDescriptor> descriptors;

        private static TransformManagerDescriptorFactory instance;
        public static TransformManagerDescriptorFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new TransformManagerDescriptorFactory();
                return instance;
            }
        }

        private TransformManagerDescriptorFactory()
        {
            descriptors = new List<ExtElemReferenceTransformManagerDescriptor>();
            AvaliableTransformManagers = new Dictionary<Type, string>();

            // Add descriptrs here

            descriptors.Add(new GeoPositionedTransformManagerDescriptor());
            descriptors.Add(new ScreenPositionedTransformManagerDescriptor());
            descriptors.Add(new RadialCenterTransformManagerDescriptor());

            // End add descriptors

            descriptors.ForEach(d => AvaliableTransformManagers.Add(d.GetType(), d.Name));
        }

        public Dictionary<Type, string> AvaliableTransformManagers { get; private set; }

        public ExtElemReferenceTransformManagerDescriptor CreateDescriptor(Type type)
        {
            return Activator.CreateInstance(type) as ExtElemReferenceTransformManagerDescriptor;
        }
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
            var elem = (ExtElemReferenceTransformManager)Activator.CreateInstance(element.Type);
            elem.Configure(parameters);
            return elem;
        }
    }



    public class GeoPositionedTransformManagerDescriptor : ExtElemReferenceTransformManagerDescriptor
    {

        public GeoPositionedTransformManagerDescriptor()
        {
            ParameterDescription = new Dictionary<string, Geo.ParameterDescription>();
            ParameterDescription.Add("Position", new Geo.ParameterDescription(typeof(Vector2d), Vector2d.zero));
            ParameterDescription.Add("Scale", new Geo.ParameterDescription(typeof(Vector3), Vector3.one));
            ParameterDescription.Add("Rotation", new Geo.ParameterDescription(typeof(float), 0f));
            ParameterDescription.Add("InteractionRange", new Geo.ParameterDescription(typeof(float), 25f)); // 25 metros
            ParameterDescription.Add("RevealOnlyOnRange", new Geo.ParameterDescription(typeof(bool), true));
        }

        public string Name { get { return "World positioned"; } }
        public Dictionary<string, ParameterDescription> ParameterDescription { get; private set; }
        public Type Type { get { return typeof(GeoPositionedTransformManager); } }
    }

    public class ScreenPositionedTransformManagerDescriptor : ExtElemReferenceTransformManagerDescriptor
    {

        public ScreenPositionedTransformManagerDescriptor()
        {
            ParameterDescription = new Dictionary<string, Geo.ParameterDescription>();
            ParameterDescription.Add("Position", new Geo.ParameterDescription(typeof(Vector2d), Vector2d.zero));
            ParameterDescription.Add("Scale", new Geo.ParameterDescription(typeof(Vector3), Vector3.one));
            ParameterDescription.Add("Rotation", new Geo.ParameterDescription(typeof(float), 0f));
        }

        public string Name { get { return "Screen positioned"; } }
        public Dictionary<string, ParameterDescription> ParameterDescription { get; private set; }
        public Type Type { get { return typeof(ScreenPositionedTransformManager); } }
    }

    public class RadialCenterTransformManagerDescriptor : ExtElemReferenceTransformManagerDescriptor
    {

        public RadialCenterTransformManagerDescriptor()
        {
            ParameterDescription = new Dictionary<string, Geo.ParameterDescription>();
            ParameterDescription.Add("Degree", new Geo.ParameterDescription(typeof(float), 0f));
            ParameterDescription.Add("Distance", new Geo.ParameterDescription(typeof(float), 50f));
            ParameterDescription.Add("Scale", new Geo.ParameterDescription(typeof(Vector3), Vector3.one));
            ParameterDescription.Add("Rotation", new Geo.ParameterDescription(typeof(float), 0f));
            ParameterDescription.Add("RotateAround", new Geo.ParameterDescription(typeof(bool), true));
        }

        public string Name { get { return "Radial to center positioned"; } }
        public Dictionary<string, ParameterDescription> ParameterDescription { get; private set; }
        public Type Type { get { return typeof(RadialCenterTransformManager); } }
    }

    public class ScreenPositionedTransformManager : ExtElemReferenceTransformManager
    {
        public Transform ExtElemReferenceTransform
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
                wrapper = transform.gameObject.GetComponent<GeoWrapper>();
                representable = transform.gameObject.GetComponentInChildren<Representable>();
                var ls = transform.lossyScale;
                transform.localScale = new Vector3(1 / ls.x, 1 / ls.y, 1 / ls.z);
                Context.setScale((scale.x + scale.y) / 2f); // Average scale
            }
        }

        public ElementReference Context { get; set; }

        private Vector2 position;
        private Vector3 scale;
        private float rotation;
        private GeoWrapper wrapper;
        private Representable representable;
        private Transform transform;
        private Interactuable interactuable;

        public void Configure(Dictionary<string, object> parameters)
        {
            position = ((Vector2d)parameters["Position"]).ToVector2();
            scale = (Vector3)parameters["Scale"];
            rotation = (float)parameters["Rotation"];
        }

        public void Update()
        {
            var ray = Camera.main.ScreenPointToRay(new Vector2(0, 0));
            transform.position = ray.origin + ray.direction * 10f;
            transform.rotation = Camera.main.transform.rotation;
            representable.setPosition(position/10);

            //transform.localScale = scale;
            //transform.localRotation = Quaternion.Euler(90, 0, 0);
            transform.GetChild(1).localRotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    public class RadialCenterTransformManager : ExtElemReferenceTransformManager
    {
        public ElementReference Context
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Transform ExtElemReferenceTransform
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Configure(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }

    public class GeoPositionedTransformManager : ExtElemReferenceTransformManager
    {
        public Transform ExtElemReferenceTransform
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
                wrapper = transform.gameObject.GetComponent<GeoWrapper>();
                particles = transform.gameObject.GetComponentInChildren<ParticleSystem>(true);
                character = GameObject.FindObjectOfType<GeoPositionedCharacter>();
                interactuable = transform.GetComponentInChildren<Interactuable>();
                childTransform = transform.GetComponentInChildren<Representable>().gameObject.transform;
                Context.setScale((scale.x + scale.y)); // Average scale
            }
        }

        public ElementReference Context { get; set; }

        private GeoPositionedCharacter character;
        private Vector2d latLon;
        private Vector3 scale;
        private float rotation;
        private float interactionRange;
        private bool revealOnRange;
        private GeoWrapper wrapper;
        private ParticleSystem particles;
        private Transform transform;
        private Transform childTransform;
        private Interactuable interactuable;

        public void Configure(Dictionary<string, object> parameters)
        {
            latLon = (Vector2d)parameters["Position"];
            scale = (Vector3)parameters["Scale"];
            rotation = (float)parameters["Rotation"];
            interactionRange = (float)parameters["InteractionRange"];
            revealOnRange = (bool)parameters["RevealOnlyOnRange"];
        }
        bool hidden = false;
        public void Update()
        {
            var pos = GM.LatLonToMeters(latLon) - wrapper.Tile.Rect.Center;
            transform.localPosition = new Vector3((float)pos.x, 10, (float)pos.y) - new Vector3(childTransform.localPosition.x, 0, childTransform.localPosition.y);
            transform.localRotation = Quaternion.Euler(90, 0, 0);
            childTransform.localRotation = Quaternion.Euler(0, rotation, 0);

            if (interactionRange <= 0 || GM.SeparationInMeters(character.LatLon, latLon) <= interactionRange)
            {
                if (hidden)
                {
                    Debug.Log("Unhidden");
                    hidden = false;
                    if (revealOnRange)
                    {
                        // TODO change this after: https://github.com/e-ucm/unity-tracker/issues/29
                        TrackerAsset.Instance.setVar("geo_element_" + wrapper.Element.getId(), 1);
                        particles.gameObject.SetActive(true);
                        particles.Play();
                        particles.transform.localPosition = childTransform.localPosition;
                        childTransform.gameObject.GetComponent<Renderer>().enabled = true;
                    }
                    if (interactuable != null) childTransform.GetComponent<Collider>().enabled = true;
                }
            }
            else if (!hidden)
            {
                hidden = true;
                particles.gameObject.SetActive(false);
                particles.time = 0;
                particles.Stop();
                if (revealOnRange) childTransform.gameObject.GetComponent<Renderer>().enabled = false;
                if (interactuable != null) childTransform.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
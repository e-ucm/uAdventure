using UnityEngine;
using System.Collections;
using uAdventure.Core;
using MapzenGo.Models;
using uAdventure.Runner;

namespace uAdventure.Geo
{
    public class GeoWrapper : MonoBehaviour
    {
        public GameObject Object_Prefab, Atrezzo_Prefab, Character_Prefab;

        public ExtElemReference Reference { get; set; }
        public Element Element { get; set; }
        public Tile Tile { get; set; }

        private ExtElemReferenceTransformManager transformManager;
        private Representable mb;

        void Start()
        {
            transformManager = ExtElemReferenceTransformManagerFactory.Instance.CreateInstance(Reference.TransformManagerDescriptor, Reference.TransformManagerParameters);
            transformManager.ExtElemReferenceTransform = this.transform;

            GameObject base_prefab = null;
            if (Element is Atrezzo) base_prefab = Atrezzo_Prefab;
            if (Element is NPC) base_prefab = Character_Prefab;
            if (Element is Item) base_prefab = Object_Prefab;

            if(base_prefab != null)
            {
                GameObject ret = GameObject.Instantiate(base_prefab);
                ret.GetComponent<Representable>().Context = new ElementReference(Reference.getTargetId(), 0, 0);
                ret.GetComponent<Representable>().Element = Element;
                ret.transform.SetParent(transform);
            }
        }

        // Update is called once per frame
        void Update()
        {
            transformManager.Update();
            if(mb is Interactuable)
            {
                var i = mb as Interactuable;
                i.setInteractuable(false);
            }
        }
    }

}

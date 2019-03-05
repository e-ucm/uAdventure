using UnityEngine;
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

        protected void Start()
        {

            GameObject base_prefab = null;
            if (Element is Atrezzo)
            {
                base_prefab = Atrezzo_Prefab;
            }

            if (Element is NPC)
            {
                base_prefab = Character_Prefab;
            }

            if (Element is Item)
            {
                base_prefab = Object_Prefab;
            }

            var context = new ElementReference(Reference.getTargetId(), 0, 0);
            if (base_prefab != null)
            {
                GameObject ret = GameObject.Instantiate(base_prefab);
                mb = ret.GetComponent<Representable>();
                mb.Context = context;
                mb.Element = Element;
                ret.transform.SetParent(transform);
                ret.transform.localPosition = Vector3.zero;
            }
            
            transformManager = ExtElemReferenceTransformManagerFactory.Instance.CreateInstance(Reference.TransformManagerDescriptor, Reference.TransformManagerParameters);
            transformManager.Context = context;
            transformManager.ExtElemReferenceTransform = this.transform;
            transformManager.Update();
        }

        // Update is called once per frame
        protected void Update()
        {
            transformManager.Update();
            if(mb is Interactuable)
            {
                var i = mb as Interactuable;
                i.setInteractuable(false);
            }
        }

        protected void OnDestroy()
        {
            var ua = uAdventurePlugin.FindObjectOfType<uAdventurePlugin>();
            if (ua)
            {
                ua.ReleaseElement(Reference);
            }
        }
    }

}

using UnityEngine;
using uAdventure.Core;
using MapzenGo.Models;
using uAdventure.Runner;

namespace uAdventure.Geo
{
    public class GeoPositioner : MonoBehaviour
    {
        public ExtElemReference Context { get; set; }
        public Element Element { get; set; }
        public Tile Tile { get; set; }
        public Representable Representable
        {
            get { return representable; }
            set
            {
                if (representable != value)
                {
                    if (representable != null)
                    {
                        representable.RepresentableChanged -= Update;
                    }

                    representable = value;
                    representable.RepresentableChanged += Update;
                }
            }
        }

        private Representable representable;
        private ITransformManager transformManager;


        protected void Start()
        {
            transformManager = TransformManagerFactory.Instance.CreateInstance(Context.TransformManagerDescriptor, Context.TransformManagerParameters);
            transformManager.Context = Context;
            transformManager.ExtElemReferenceTransform = this.transform;
            transformManager.Update();
        }

        // Update is called once per frame
        protected void Update()
        {
            transformManager.Update();
        }

        protected void OnDestroy()
        {
            var ua = uAdventurePlugin.FindObjectOfType<uAdventurePlugin>();
            if (ua)
            {
                ua.ReleaseElement(Context);
            }
        }
    }

}

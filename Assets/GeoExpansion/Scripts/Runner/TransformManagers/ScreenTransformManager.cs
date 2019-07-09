using System.Collections.Generic;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Geo
{

    public class ScreenTransformManager : ITransformManager
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
                positioner = transform.gameObject.GetComponent<GeoPositioner>();
                representable = transform.gameObject.GetComponentInChildren<Representable>();
                var ls = transform.lossyScale;
                transform.localScale = new Vector3(1 / ls.x, 1 / ls.y, 1 / ls.z);
            }
        }

        public ExtElemReference Context { get; set; }

        private Vector2 position;
        private float rotation;
        private GeoPositioner positioner;
        private Representable representable;
        private Transform transform;

        public void Configure(Dictionary<string, object> parameters)
        {
            position = (Vector2)parameters["Position"];
            rotation = (float)parameters["Rotation"];
        }

        public void Update()
        {
            if (!positioner)
            {
                GameObject.DestroyImmediate(transform.gameObject);
            }

            var ray = Camera.main.ScreenPointToRay(new Vector2(0, 0));
            transform.position = ray.origin + ray.direction * 10f;
            transform.rotation = Camera.main.transform.rotation;
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
    }
}

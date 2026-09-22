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
                representable = transform.GetComponent<Representable>() ?? transform.gameObject.GetComponentInChildren<Representable>();
                if (representable != null)
                {
                    representable.RepresentableChanged += Adapted;
                    Adapted();
                }
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

            var ray = Camera.main.ScreenPointToRay(PositionToScreenPoint(position));
            transform.position = ray.origin + ray.direction * 10f;
            transform.rotation = Camera.main.transform.rotation;
        }

        private void Adapted()
        {
            var lcs = transform.localScale;
            var lss = transform.lossyScale;
            var counterScale = new Vector3(lcs.x / lss.x, lcs.y / lss.y, lcs.z / lss.z) * (Camera.main.orthographicSize/30f);
            transform.localScale = new Vector3(lcs.x * counterScale.x, lcs.y * counterScale.y, lcs.z * counterScale.z);
        }

        private Vector2 PositionToScreenPoint(Vector2 position)
        {
            var size = representable.Size;

            var position01 = new Vector2(position.x / 800f, 1 - (position.y - size.y/2f) / 600f);
            var screenPosition = new Vector2(position01.x * Screen.width, position01.y * Screen.height);

            return screenPosition;
        }
    }
}

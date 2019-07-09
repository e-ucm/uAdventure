using AssetPackage;
using MapzenGo.Helpers;
using System.Collections.Generic;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Geo
{
    public class GeolocationTransformManager : ITransformManager
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
                particles = transform.gameObject.GetComponentInChildren<ParticleSystem>(true);
                character = GameObject.FindObjectOfType<GeoPositionedCharacter>();
                interactuable = transform.GetComponentInChildren<Interactuable>();
                childTransform = transform.GetComponentInChildren<Representable>().gameObject.transform;
            }
        }

        public ExtElemReference Context { get; set; }

        private GeoPositionedCharacter character;
        private Vector2d latLon;
        private float rotation;
        private float interactionRange;
        private bool revealOnRange;
        private GeoPositioner positioner;
        private ParticleSystem particles;
        private Transform transform;
        private Transform childTransform;
        private Interactuable interactuable;

        public void Configure(Dictionary<string, object> parameters)
        {
            latLon = (Vector2d)parameters["Position"];
            rotation = (float)parameters["Rotation"];
            interactionRange = (float)parameters["InteractionRange"];
            revealOnRange = (bool)parameters["RevealOnlyOnRange"];
        }
        bool hidden = false;
        public void Update()
        {
            var pos = GM.LatLonToMeters(latLon) - positioner.Tile.Rect.Center;
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
                        TrackerAsset.Instance.setVar("geo_element_" + positioner.Element.getId(), 1);
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

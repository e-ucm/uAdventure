using MapzenGo.Helpers;
using System.Collections.Generic;
using uAdventure.Runner;
using UnityEngine;
using Xasu;
using static uAdventure.Geo.GeoElementMB;

namespace uAdventure.Geo
{
    public class GeolocationTransformManager : ITransformManager
    {
        private static GameObject particleSystemPrefab;

        public Transform ExtElemReferenceTransform
        {
            get
            {
                return transform;
            }
            set
            {
                if (!particleSystemPrefab)
                {
                    particleSystemPrefab = Resources.Load<GameObject>("GeoParticles");
                }

                transform = value;
                collider = transform.gameObject.GetComponent<Collider>();
                renderer = transform.gameObject.GetComponent<Renderer>();
                positioner = transform.gameObject.GetComponent<GeoPositioner>();
                geoExtension = GameExtension.GetInstance<GeoExtension>();

                var particlesGo = Object.Instantiate(particleSystemPrefab,transform);
                particles = particlesGo.GetComponent<ParticleSystem>();
                material = particles.GetComponent<ParticleSystemRenderer>().material;

                character = GameObject.FindObjectOfType<GeoPositionedCharacter>();
                interactuable = transform.GetComponent<Interactuable>() ?? transform.GetComponentInChildren<Interactuable>();

                representable = transform.gameObject.GetComponent<Representable>();
                if (representable != null)
                {
                    representable.RepresentableChanged += Adapted;
                    Adapted();
                }
            }
        }

        public ExtElemReference Context { get; set; }

        private GeoPositionedCharacter character;
        private Vector2d latLon;
        private float rotation;
        private float interactionRange;
        private bool revealOnRange;
        private GeoPositioner positioner;
        private Material material;
        private Texture2D particleTexture;
        private Transform transform;
        private Interactuable interactuable;
        private Collider collider;
        private Renderer renderer;
        private ParticleSystem particles;
        private Representable representable;
        private bool shown = true;
        private bool hint;
        private List<IGeoActionManager> geoActionManagers;
        private GeoExtension geoExtension;

        public void Configure(Dictionary<string, object> parameters)
        {
            latLon = (Vector2d)parameters["Position"];
            rotation = (float)parameters["Rotation"];
            interactionRange = (float)parameters["InteractionRange"];
            revealOnRange = (bool)parameters["RevealOnlyOnRange"];
            hint = (bool)parameters["Hint"];
            particleTexture = Game.Instance.ResourceManager.getImage(parameters["RevealParticleTexture"] as string);


        }


        public void Update()
        {
            if (material && material.mainTexture != particleTexture)
            {
                material.mainTexture = particleTexture;
            }

            var pos = GM.LatLonToMeters(latLon) - positioner.Tile.Rect.Center;
            var basePosition = new Vector3((float) pos.x, 10, (float) pos.y);
            var centerVector = new Vector3(0, 0, transform.localScale.y/2f);
            transform.localPosition = basePosition + centerVector;
            transform.localRotation = Quaternion.Euler(90, rotation, 0);

            if (interactionRange <= 0 || GM.SeparationInMeters(geoExtension.Location, latLon) <= interactionRange)
            {
                SetShown(true);
                if (interactuable != null)
                {
                    interactuable.setInteractuable(true);
                }
            }
            else if (shown)
            {
                SetShown(false);
                if (interactuable != null)
                {
                    interactuable.setInteractuable(true);
                }
            }

            if (geoActionManagers == null && Context != null && Context.Actions != null)
            {
                geoActionManagers = new List<IGeoActionManager>();
                foreach (var action in Context.Actions)
                {
                    GeoActionManagerFactory.Instance.CreateFor(action);
                    var newManager = GeoActionManagerFactory.Instance.CreateFor(action);
                    newManager.Element = Context.TargetId;
                    newManager.Geometry = new GMLGeometry
                    {
                        Influence = interactionRange,
                        Points = new Vector2d[] { latLon },
                        Type = GMLGeometry.GeometryType.Point
                    };
                    newManager.Player = character;
                    newManager.Holder = positioner.gameObject;
                    geoActionManagers.Add(newManager);
                }
            }

            if(geoActionManagers != null)
            {
                geoActionManagers.ForEach(g => g.Update());
            }
        }

        private void Adapted()
        {
            // Position
            var metersSizeAt0 = representable.Size / (float) GM.GetPixelsPerMeter(0, 19);
            var pixelScaleAt = (float)GM.GetPixelsPerMeter(latLon.x, 19) / (float)GM.GetPixelsPerMeter(0, 19);
            var ns = new Vector3(metersSizeAt0.x * pixelScaleAt, metersSizeAt0.y * pixelScaleAt, 1);
            transform.localScale = ns;
            positioner.Hint.transform.localScale = new Vector3(1f/ns.x, 1f/ns.y, 1f/ns.z) * 7f;
        }

        private void SetShown(bool shown)
        {
            if (this.shown != shown)
            {
                this.shown = shown;
                if (shown)
                {
#if UNITY_ANDROID || UNITY_IOS
                    Handheld.Vibrate();
#endif
                    if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                    {
                        // TODO change this after: https://github.com/e-ucm/unity-tracker/issues/29
                        MovementTracker.Instance.Moved(Game.Instance.GameState.CurrentTarget, geoExtension.Location)
                            .WithResultExtensions(new Dictionary<string, object> { { "geo_element_" + positioner.Element.getId(), 1 } });
                    }
                }
                if (hint)
                {
                    positioner.Hint.SetActive(!shown);
                }
                if (revealOnRange)
                {
                    collider.enabled = shown;
                    renderer.enabled = shown;
                    SetParticles(shown);
                }
            }
        }

        private void SetParticles(bool enabled)
        {
            if (enabled)
            {
                particles.gameObject.SetActive(true);
                particles.Play();
            }
            else
            {
                particles.gameObject.SetActive(false);
                particles.time = 0;
                particles.Stop();
            }
        }
    }
}

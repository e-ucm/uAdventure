using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.Runner;
using System;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    [CustomEffectRunner(typeof(TriggerZonedSceneEffect))]
    public class TriggerZonedSceneRunner : CustomEffectRunner
    {
        TriggerZonedSceneEffect effect;

        public Effect Effect { get { return effect; } set { effect = value as TriggerZonedSceneEffect; } }

        public bool execute()
        {
            var geoElement = Game.Instance.GameState.FindElement<GeoElement>(effect.ZoneId);
            var go = new GameObject();
            var zc = go.AddComponent<ZoneControl>();
            zc.zone = geoElement.Geometry;
            zc.loadOnExit = Game.Instance.GameState.CurrentTarget;

            Game.Instance.renderScene(effect.getTargetId(), effect.getTransitionTime(), effect.getTransitionType());

            return false;
        }
    }

    public class ZoneControl : MonoBehaviour
    {
        public GMLGeometry zone;
        public string loadOnExit;

        public Vector2 debugLatLong;

        void Start()
        {
            debugLatLong = zone.Center.ToVector2();
            StartCoroutine(StartLocation());
        }

        IEnumerator StartLocation()
        {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
                yield break;

            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
            {
                // Access granted and location value could be retrieved
                print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            }
        }

        void Update()
        {
            if(Input.location.status == LocationServiceStatus.Running)
            {
                if (!zone.InsideInfluence(new Vector2d(Input.location.lastData.longitude, Input.location.lastData.latitude)))
                {
                    Game.Instance.renderScene(loadOnExit, 0, 0);
                    DestroyImmediate(this.gameObject);
                }
            }

            if (Input.location.status == LocationServiceStatus.Stopped)
            {
                if (!zone.InsideInfluence(debugLatLong.ToVector2d()))
                {
                    Game.Instance.renderScene(loadOnExit, 0, 0);
                    DestroyImmediate(this.gameObject);
                }
            }
        }
    }
}
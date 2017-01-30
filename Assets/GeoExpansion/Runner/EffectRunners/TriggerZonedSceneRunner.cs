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
            zc.transitionTime = effect.getTransitionTime();

            Game.Instance.RunTarget(effect.getTargetId(), effect.getTransitionTime(), effect.getTransitionType());

            return false;
        }
    }

    public class ZoneControl : MonoBehaviour
    {
        public GMLGeometry zone;
        public string loadOnExit;
        public float transitionTime;

        public Vector2 debugLatLong;

        void Start()
        {
            debugLatLong = zone.Center.ToVector2();
            if (!GPSController.Instance.IsStarted())
                GPSController.Instance.Start();

            if (GPSController.Instance.IsLocationValid())
                debugLatLong = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
        

        void Update()
        {
            if(GPSController.Instance.IsLocationValid())
            {
                if (!zone.InsideInfluence(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude),5))
                {
                    Debug.Log("No está en la influencia, pero la ubicación es válida");
                    Game.Instance.RunTarget(loadOnExit, 0, 0);
                    DestroyImmediate(this.gameObject);
                }
            }
            else if (!GPSController.Instance.IsStarted())
            {
                if (!zone.InsideInfluence(debugLatLong.ToVector2d(),5))
                {
                    Debug.Log("No está en la influencia");
                    Game.Instance.RunTarget(loadOnExit, 0, 0);
                    DestroyImmediate(this.gameObject);
                }
            }
        }
    }
}
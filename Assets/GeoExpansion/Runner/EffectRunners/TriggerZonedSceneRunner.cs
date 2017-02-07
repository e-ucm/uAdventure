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
            zc.zoneid = geoElement.Id;
            zc.Save();

            Game.Instance.RunTarget(effect.getTargetId(), effect.getTransitionTime(), effect.getTransitionType());

            return false;
        }
    }

    public class ZoneControl : MonoBehaviour
    {
        public GMLGeometry zone;
        public string zoneid;
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
            if (Game.Instance.isSomethingRunning())
                return; // We have to respect if something is running, like a conversation or an effect

            if(GPSController.Instance.IsLocationValid())
            {
                if (!zone.InsideInfluence(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude),5))
                {
                    Debug.Log("No está en la influencia, pero la ubicación es válida");
                    Game.Instance.RunTarget(loadOnExit, 0, 0);
                    RemoveKeys();
                    DestroyImmediate(this.gameObject);
                }
            }
            else if (!GPSController.Instance.IsStarted())
            {
                if (!zone.InsideInfluence(debugLatLong.ToVector2d(),5))
                {
                    Debug.Log("No está en la influencia");
                    Game.Instance.RunTarget(loadOnExit, 0, 0);
                    RemoveKeys();
                    DestroyImmediate(this.gameObject);
                }
            }
        }

        void RemoveKeys()
        {
            PlayerPrefs.DeleteKey("zone_control");
            PlayerPrefs.DeleteKey("zone_control_loadonexit");
            PlayerPrefs.DeleteKey("zone_control_id");
            PlayerPrefs.DeleteKey("zone_control_transitiontime");
            PlayerPrefs.Save();
        }

        public void Save()
        {
            PlayerPrefs.SetInt("zone_control", 1);
            PlayerPrefs.SetString("zone_control_loadonexit", loadOnExit);
            PlayerPrefs.SetString("zone_control_id", zoneid);
            PlayerPrefs.SetFloat("zone_control_transitiontime", transitionTime);
            PlayerPrefs.Save();
        }

        public void Restore()
        {
            loadOnExit = PlayerPrefs.GetString("zone_control_loadonexit");
            zone = Game.Instance.GameState.FindElement<GeoElement>(PlayerPrefs.GetString("zone_control_id")).Geometry;
            transitionTime = PlayerPrefs.GetFloat("zone_control_transitiontime");
        }
    }
}
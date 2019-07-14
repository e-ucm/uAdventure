using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.Runner;
using System;
using System.Linq;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    [CustomEffectRunner(typeof(TriggerZonedSceneEffect))]
    public class TriggerZonedSceneRunner : CustomEffectRunner
    {
        TriggerZonedSceneEffect effect;

        public IEffect Effect { get { return effect; } set { effect = value as TriggerZonedSceneEffect; } }

        public bool execute()
        {
            var geoElement = Game.Instance.GameState.FindElement<GeoElement>(effect.ZoneId);
            var go = new GameObject();
            var zc = go.AddComponent<ZoneControl>();
            zc.zone = geoElement.Geometries.Checked().FirstOrDefault();
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

        void Start()
        {
            if (!GPSController.Instance.IsStarted())
            {
                GPSController.Instance.Start();
            }
        }
        

        void Update()
        {
            if (Game.Instance.isSomethingRunning())
            {
                return; // We have to respect if something is running, like a conversation or an effect
            }

            if(GPSController.Instance.IsLocationValid())
            {
                if (!zone.InsideInfluence(GPSController.Instance.Location, 5))
                {
                    Debug.Log("No está en la influencia, pero la ubicación es válida");
                    Game.Instance.RunTarget(loadOnExit, 0, 0);
                    RemoveKeys();
                    DestroyImmediate(this.gameObject);
                }
            }
            else if (!GPSController.Instance.IsStarted() && !zone.InsideInfluence(GPSController.Instance.Location, 5))
            {
                Debug.Log("No está en la influencia");
                Game.Instance.RunTarget(loadOnExit, 0, 0);
                RemoveKeys();
                DestroyImmediate(this.gameObject);
            }
        }

        void RemoveKeys()
        {
            // TODO use PlayerPrefs in settings
            PlayerPrefs.DeleteKey("zone_control");
            PlayerPrefs.DeleteKey("zone_control_loadonexit");
            PlayerPrefs.DeleteKey("zone_control_id");
            PlayerPrefs.DeleteKey("zone_control_transitiontime");
            PlayerPrefs.Save();
        }

        public void Save()
        {
            // TODO use PlayerPrefs in settings
            PlayerPrefs.SetInt("zone_control", 1);
            PlayerPrefs.SetString("zone_control_loadonexit", loadOnExit);
            PlayerPrefs.SetString("zone_control_id", zoneid);
            PlayerPrefs.SetFloat("zone_control_transitiontime", transitionTime);
            PlayerPrefs.Save();
        }

        public void Restore()
        {
            // TODO use PlayerPrefs in settings
            loadOnExit = PlayerPrefs.GetString("zone_control_loadonexit");
            zone = Game.Instance.GameState.FindElement<GeoElement>(PlayerPrefs.GetString("zone_control_id")).Geometries.Checked().FirstOrDefault();
            transitionTime = PlayerPrefs.GetFloat("zone_control_transitiontime");
        }
    }
}
 
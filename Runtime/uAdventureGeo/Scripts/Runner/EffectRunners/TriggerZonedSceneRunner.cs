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

            if (Application.isEditor && Application.isPlaying)
            {
                GameExtension.GetInstance<GeoExtension>().Location = GameExtension.GetInstance<GeoExtension>().Location; // This will activate debug location
            }

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
            if (!GameExtension.GetInstance<GeoExtension>().IsStarted())
            {
                GameExtension.GetInstance<GeoExtension>().Start();
                Save(Game.Instance.GameState.GetMemory("geo_extension"));
            }
        }
        

        void Update()
        {
            if (Game.Instance.isSomethingRunning())
            {
                return; // We have to respect if something is running, like a conversation or an effect
            }
            var geoExtension = GameExtension.GetInstance<GeoExtension>();
            if (geoExtension.IsLocationValid())
            {
                if (!zone.InsideInfluence(geoExtension.Location, 5))
                {
                    Debug.Log("No está en la influencia, pero la ubicación es válida");
                    Game.Instance.RunTarget(loadOnExit, 0, 0);
                    Game.Instance.GameState.GetMemory("geo_extension").Set("zone_control", false);
                    DestroyImmediate(this.gameObject);
                }
            }
            else if (!geoExtension.IsStarted() && !zone.InsideInfluence(geoExtension.Location, 5))
            {
                Debug.Log("No está en la influencia");
                Game.Instance.RunTarget(loadOnExit, 0, 0);
                Game.Instance.GameState.GetMemory("geo_extension").Set("zone_control", false);
                DestroyImmediate(this.gameObject);
            }
        }

        private void Save(Memory memory)
        {
            memory.Set("zone_control", true);
            memory.Set("zone_control_loadonexit", loadOnExit);
            memory.Set("zone_control_id", zoneid);
            memory.Set("zone_control_transitiontime", transitionTime);
        }

        public void Restore(Memory memory)
        {
            loadOnExit = memory.Get<string>("zone_control_loadonexit");
            zoneid = memory.Get<string>("zone_control_id");
            zone = Game.Instance.GameState.FindElement<GeoElement>(zoneid).Geometries.Checked().FirstOrDefault();
            transitionTime = memory.Get<float>("zone_control_transitiontime");
        }
    }
}
 
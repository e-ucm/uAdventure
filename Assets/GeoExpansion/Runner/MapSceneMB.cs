using UnityEngine;
using System.Collections;

using uAdventure.Runner;
using System;

namespace uAdventure.Geo
{
    public class MapSceneMB : MonoBehaviour, IRunnerChapterTarget
    {
        private MapScene mapScene;

        public object Data
        {
            get
            {
                return mapScene;
            }

            set
            {
                mapScene = (MapScene) value;
            }
        }

        public bool canBeInteracted()
        {
            throw new NotImplementedException();
        }

        public void Destroy(float time = 0)
        {
            throw new NotImplementedException();
        }

        public InteractuableResult Interacted(RaycastHit hit = default(RaycastHit))
        {
            throw new NotImplementedException();
        }

        public void RenderScene()
        {
            throw new NotImplementedException();
        }

        public void setInteractuable(bool state)
        {
            throw new NotImplementedException();
        }
    }
}
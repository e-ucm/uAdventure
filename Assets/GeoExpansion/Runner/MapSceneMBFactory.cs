using UnityEngine;
using System.Collections;

using uAdventure.Runner;
using System;
using uAdventure.Core;

namespace uAdventure.Geo
{
    [ChapterTargetFactory(typeof(MapScene))]
    public class MapSceneMBFactory : MonoBehaviour, IChapterTargetFactory
    {
        public GameObject geoScenePrefab;

        public IRunnerChapterTarget Instantiate(IChapterTarget modelObject)
        {
            return GameObject.Instantiate(geoScenePrefab).GetComponent<MapSceneMB>();
        }
    }

}

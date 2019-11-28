using UnityEngine;

using uAdventure.Runner;
using uAdventure.Core;

namespace uAdventure.Geo
{
    [ChapterTargetFactory(typeof(MapScene))]
    public class MapSceneMBFactory : MonoBehaviour, IChapterTargetFactory
    {
        public GameObject mapScenePrefab;

        public void Awake()
        {
            mapScenePrefab = Resources.Load<GameObject>("MapScene");
        }

        public IRunnerChapterTarget Instantiate(IChapterTarget modelObject)
        {
            var ms =  GameObject.Instantiate(mapScenePrefab).GetComponent<MapSceneMB>();
            return ms;
        }
    }

}

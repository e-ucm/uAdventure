using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;

namespace uAdventure.Runner
{
    [ChapterTargetFactory(typeof(GeneralScene), typeof(Scene), typeof(Cutscene), typeof(Slidescene), typeof(Videoscene))]
    public class SceneMBFactory : MonoBehaviour, IChapterTargetFactory {

        public GameObject prefab;
        void Start()
        {
            if (prefab == null)
                prefab = Resources.Load<GameObject>("Prefabs/Scene");
        }

        public IRunnerChapterTarget Instantiate(IChapterTarget modelObject)
        {
            var s = GameObject.Instantiate(prefab).GetComponent<SceneMB>();
            s.gameObject.GetComponent<Transform>().localPosition = new Vector2(0f, 0f);
            return s;
        }
    }
}
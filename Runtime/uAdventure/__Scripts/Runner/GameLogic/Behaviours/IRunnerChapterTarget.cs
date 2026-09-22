using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public interface IRunnerChapterTarget : Interactuable
    {
        object Data { get; set; }
        void RenderScene();
        bool IsReady { get; }
        void Destroy(float time, System.Action onDestroy);
        GameObject gameObject { get; }
    }

}


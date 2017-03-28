using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public interface IRunnerChapterTarget : Interactuable
    {
        object Data { get; set; }
        void RenderScene();
        void Destroy(float time = 0);
        GameObject gameObject { get; }
    }

}


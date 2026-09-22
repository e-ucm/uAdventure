using Simva;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Simva
{
    public class SimvaSceneWrapper : MonoBehaviour, IRunnerChapterTarget
    {
        private SimvaScene simvaScene;

        public object Data { get { return simvaScene; } set { simvaScene = (SimvaScene)value; } }

        public bool IsReady { get { return SimvaSceneController.Ready; } }

        public SimvaSceneController SimvaSceneController { get; set; }

        public void RenderScene()
        {
            InventoryManager.Instance.Show = false;
            SimvaSceneController.Render();
        }

        public void Destroy(float time, Action onDestroy)
        {
            SimvaSceneController.Destroy();
            onDestroy();
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void setInteractuable(bool state)
        {
        }
    }
}

using UnityEngine;
using UniRx;
using UnityFx.Async.Promises;
using uAdventure.Runner;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Simva;
using UnityFx.Async;
using uAdventure.Analytics;
using System.Collections;
using AssetPackage;

namespace uAdventure.Simva
{
    // Manager for "Simva.End"
    public class FlushAllController : MonoBehaviour, IRunnerChapterTarget
    {
        private bool ready;
        public Text trackerStatusText;

        public object Data { get; set; }

        public bool IsReady { get { return ready; } }

        protected void OnApplicationResume()
        {
        }

        public void RenderScene()
        {
            SimvaExtension.Instance.NotifyLoading(true);
            this.transform.GetChild(0).gameObject.SetActive(true);
            InventoryManager.Instance.Show = false;
            if((Data as FlushAllScene).onlyFlushAndBackup)
            {
                StartCoroutine(FlushTracker());
            }
            else
            {
                StartCoroutine(FinishTracker());
            }
            ready = true;
        }

        private IEnumerator FinishTracker()
        {
            yield return AnalyticsExtension.Instance.OnGameFinished();
            SimvaExtension.Instance.AfterFlush();
        } 

        private IEnumerator FlushTracker()
        {
            var flushed = false;
            TrackerAsset.Instance.ForceCompleteTraces();
            TrackerAsset.Instance.FlushAll(() => flushed = true);
            yield return new WaitUntil(() => flushed);
            SimvaExtension.Instance.AfterFlush();
        }

        public void Destroy(float time, Action onDestroy)
        {
            GameObject.DestroyImmediate(this.gameObject);
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

        private void Update()
        {
            if (!trackerStatusText || !trackerStatusText.isActiveAndEnabled)
            {
                return;
            }

            string text = "";
            foreach(var kv in TrackerAsset.Instance.GetTrackerStatus())
            {
                text += kv.Key + ": " + kv.Value + " \n "; 
            }
            trackerStatusText.text = text;
        }
    }
}


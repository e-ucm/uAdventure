using UnityEngine;
using UnityFx.Async.Promises;
using uAdventure.Runner;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Simva;
using UnityFx.Async;
using AssetPackage;
using System.IO;
using System.Text;

namespace uAdventure.Simva
{
    // Manager for "Simva.Backup"
    public class BackupController : MonoBehaviour, IRunnerChapterTarget
    {
        public Scrollbar progressBar;
        public Text successText;
        public Text errorText;
        private bool progressCallbackAdded;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return true; } }

        protected void OnApplicationResume()
        {
        }

        public void Update()
        {
            if(SimvaExtension.Instance.backupOperation != null)
            {
                if (!progressCallbackAdded)
                {
                    ((AsyncCompletionSource)SimvaExtension.Instance.backupOperation).AddProgressCallback((p) =>
                    {
                        if (progressBar)
                        {
                            var scaledProgress = (Mathf.Clamp(p, 0f, 0.5f) / 0.5f) * 0.9f
                                + (Mathf.Clamp(p - 0.5f, 0f, 0.5f) / 0.5f) * 0.1f;
                            progressBar.size = scaledProgress;
                        }
                    });
                    progressCallbackAdded = true;
                }

                if (SimvaExtension.Instance.backupOperation.IsCompletedSuccessfully)
                {
                    progressBar.transform.parent.parent.gameObject.SetActive(false);
                    successText.gameObject.SetActive(true);
                    errorText.gameObject.SetActive(false);
                }
                else if (SimvaExtension.Instance.backupOperation.IsFaulted)
                {
                    progressBar.transform.parent.parent.gameObject.SetActive(false);
                    successText.gameObject.SetActive(false);
                    errorText.gameObject.SetActive(true);
                    var errorMessage = errorText.transform.GetChild(0).GetComponent<Text>();
                    errorMessage.text = SimvaExtension.Instance.backupOperation.Exception.Message;
                }
                else
                {
                    progressBar.transform.parent.parent.gameObject.SetActive(true);
                    successText.gameObject.SetActive(false);
                    errorText.gameObject.SetActive(false);
                }
            }
        }

        public void Retry()
        {
            progressCallbackAdded = false;

            var se = SimvaExtension.Instance;
            string traces = se.SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
            se.backupOperation = se.SaveActivity(se.backupActivity.Id, traces, true);
            se.backupOperation.Then(() =>
            {
                se.AfterBackup();
            });
        }

        public void Share()
        {
            string username = SimvaExtension.Instance.API.AuthorizationInfo.Username;
            string traces = SimvaExtension.Instance.SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
            string filePath = Path.Combine(Application.temporaryCachePath, "traces_backup_" + username + ".log");
            File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(traces));

            new NativeShare().AddFile(filePath)
                .SetSubject("Backup de " + username).SetText("Backup adjunto")
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void RenderScene()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            foreach(var backupPopup in FindObjectsOfType<BackupPopupController>())
            {
                DestroyImmediate(backupPopup.gameObject);
            }
            InventoryManager.Instance.Show = false;
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
    }
}


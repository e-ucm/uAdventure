using UnityEngine;
using uAdventure.Runner;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Simva;
using System.IO;
using System.Text;
using Xasu;
using Xasu.Util;
using System.Threading.Tasks;

namespace uAdventure.Simva
{
    // Manager for "Simva.Finalize"
    public class FinalizeController : MonoBehaviour, IRunnerChapterTarget
    {
        public Scrollbar progressBar;
        public Text successText;
        public Text errorText;

        public object Data { get { return null; } set { } }

        public bool IsReady { get { return true; } }
        
        public async void Start()
        {
            if (XasuTracker.Instance.Status.State == TrackerState.NetworkRequired || XasuTracker.Instance.Status.State == TrackerState.Fallback)
            {
                // TODO Remember the player to connect in order to flush everything
            }

            if (XasuTracker.Instance.Status.State == TrackerState.Normal)
            {
                await DoFinalize();
            }
        }

        private async Task<bool> DoFinalize()
        {
            // Configure visual elements
            progressBar.transform.parent.parent.gameObject.SetActive(true);
            successText.gameObject.SetActive(false);
            errorText.gameObject.SetActive(false);

            // Finalize
            PartialStatements.CompleteAllStatements();

            try
            {
                Debug.Log("Finalize!");
                await XasuTracker.Instance.Finalize(new Progress<float>(p => progressBar.size = p));

                progressBar.transform.parent.parent.gameObject.SetActive(false);
                successText.gameObject.SetActive(true);
                errorText.gameObject.SetActive(false);

                // Continue the Simva process
                GameExtension.GetInstance<SimvaExtension>().AfterFinalize();
                return true;
            }
            catch(Exception ex)
            {
                // Failed
                progressBar.transform.parent.parent.gameObject.SetActive(false);
                successText.gameObject.SetActive(false);
                errorText.gameObject.SetActive(true);
                var errorMessage = errorText.transform.GetChild(0).GetComponent<Text>();
                errorMessage.text = ex.Message;
                return false;
            }
        }

        public async void Retry()
        {
            await XasuTracker.Instance.ResetState();
            await DoFinalize();
        }

        public void Share()
        {
            if (!XasuTracker.Instance.TrackerConfig.Backup)
            {
                return;
            }

            string username = GameExtension.GetInstance<SimvaExtension>().API.Authorization.Agent.name;
            string traces = File.ReadAllText(Application.temporaryCachePath + "/" + XasuTracker.Instance.TrackerConfig.BackupFileName);
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
            foreach(var backupPopup in FindObjectsOfType<FinalizePopupController>())
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


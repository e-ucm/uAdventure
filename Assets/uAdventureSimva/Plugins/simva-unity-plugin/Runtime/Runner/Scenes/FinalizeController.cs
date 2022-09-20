using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;
using Xasu;
using Xasu.Util;
using System.Threading.Tasks;

namespace Simva
{
    // Manager for "Simva.Finalize"
    public class FinalizeController : SimvaSceneController
    {
        public Scrollbar progressBar;
        public Text successText;
        public Text errorText;
        public GameObject networkPopup;
        
        public async void Start()
        {
            await DoFinalize();
        }

        private async Task<bool> DoFinalize()
        {
            // Await for connection
            do
            {
                networkPopup.SetActive(XasuTracker.Instance.Status.IsNetworkRequired);
                await Task.Yield();

            } while (networkPopup.activeSelf);

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
                SimvaManager.Instance.AfterFinalize();
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

            string username = SimvaManager.Instance.API.Authorization.Agent.name;
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

        public override void Destroy()
        {
            GameObject.DestroyImmediate(this.gameObject);
        }

        public override void Render()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            foreach (var backupPopup in FindObjectsOfType<FinalizePopupController>())
            {
                DestroyImmediate(backupPopup.gameObject);
            }
            Ready = true;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using uAdventure.Simva;
using UnityEngine;
using UnityEngine.UI;

namespace Simva
{
    public class BackupPopupController : MonoBehaviour
    {
        private enum States { Appearing, Showing, Hiding }
        private States state;

        public float fadeInTime = 0.5f;
        public float fadeOutTime = 3f;
        public CanvasGroup backupPopup;
        public Scrollbar progressBar;
        public Text successText;
        public Text errorText;

        private void Start()
        {
            backupPopup.alpha = 0;
            state = States.Appearing;
        }

        // Update is called once per frame
        private void Update()
        {
            if (state == States.Appearing)
            {
                backupPopup.alpha += Mathf.Clamp01(Time.deltaTime / fadeInTime);
                if(backupPopup.alpha == 1)
                {
                    state = States.Showing;
                }
            }

            if (SimvaExtension.Instance.backupOperation != null)
            {
                Debug.Log("Last progress: " + SimvaExtension.Instance.backupOperation.Progress);
                /*if (!progressCallbackAdded)
                {
                    ((AsyncCompletionSource)SimvaExtension.Instance.backupOperation).AddProgressCallback((p) =>
                    {
                        progressBar.size = p;
                    });
                    progressCallbackAdded = true;
                }*/
                var scaledProgress = (Mathf.Clamp(SimvaExtension.Instance.backupOperation.Progress, 0f, 0.5f)/0.5f) * 0.9f
                    + (Mathf.Clamp(SimvaExtension.Instance.backupOperation.Progress - 0.5f, 0f, 0.5f)/0.5f) * 0.1f;
                progressBar.size = scaledProgress;

                if (SimvaExtension.Instance.backupOperation.IsCompletedSuccessfully)
                {
                    progressBar.transform.parent.gameObject.SetActive(false);
                    successText.gameObject.SetActive(true);
                    errorText.gameObject.SetActive(false);
                    state = States.Hiding;
                }
                else if (SimvaExtension.Instance.backupOperation.IsFaulted)
                {
                    progressBar.transform.parent.gameObject.SetActive(false);
                    successText.gameObject.SetActive(false);
                    errorText.gameObject.SetActive(true);
                    var errorMessage = errorText.transform.GetChild(0).GetComponent<Text>();
                    errorMessage.text = SimvaExtension.Instance.backupOperation.Exception.Message;
                }
                else
                {
                    progressBar.transform.parent.gameObject.SetActive(true);
                    successText.gameObject.SetActive(false);
                    errorText.gameObject.SetActive(false);
                }
            }

            if (state == States.Hiding)
            {
                backupPopup.alpha -= Mathf.Clamp01(Time.deltaTime / fadeOutTime);
                if (backupPopup.alpha == 0)
                {
                    DestroyImmediate(this.gameObject);
                }
            }
        }
    }
}


using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZXing;

namespace dZine4D.Misc.QR
{
    /// <summary>
    /// Detects qr codes using the webcam/phone camera
    /// </summary>
    public class QRReader : MonoBehaviour
    {
        // .. ATTRIBUTES

        public string LastResult { get; private set; }


        [SerializeField]
        [Tooltip("An optional renderer component to display the camera feed.")]
        private Renderer OutputRenderer = null;
        [SerializeField]
        [Tooltip("An optional RawImage component to display the camera feed.")]
        private RawImage OutputImage = null;
        [SerializeField]
        [Tooltip("The Scaler for the RawImage")]
        private CanvasScaler OutputImageScaler = null;
        [SerializeField]
        [Tooltip("An optional text component to display the last qr decoding result.")]
        private Text OutputText = null;
        [SerializeField]
        [Tooltip("Should we start decoding on awake?")]
        private bool EnableOnAwake = true;

        private WebCamTexture camTexture;
        private Thread qrThread;

        private int W = 512;
        private int H = 512;

        private Color32[] cameraFeedGrab;
        private bool isQuit;
        private bool isReaderEnabled;

        private string prevResult;
        private int previousWebCamRotation = -1;

        // .. EVENTS

        [Serializable]
        public class QrCodeDetectedEvent : UnityEvent<string> { }
        public QrCodeDetectedEvent OnQrCodeDetected;


        // .. INITIALIZATION

        void Awake()
        {
            LastResult = string.Empty;

            camTexture = new WebCamTexture(W, H);

            if (OutputRenderer != null)
                OutputRenderer.material.mainTexture = camTexture;
            if (OutputImage != null)
                OutputImage.texture = camTexture;

            if (EnableOnAwake)
                EnableReader();

            qrThread = new Thread(DecodeQR);
            qrThread.Start();
        }


        // .. OPERATIONS

        public void EnableReader()
        {
            StopCoroutine(EnableReaderRoutine());
            StartCoroutine(EnableReaderRoutine());
        }

        public void DisableReader()
        {
            if (!isReaderEnabled)
                return;
            isReaderEnabled = false;

            LastResult = string.Empty;
            prevResult = string.Empty;
            cameraFeedGrab = null;

            camTexture.Pause();
        }

        public void SetOutputImage(RawImage image)
        {
            OutputImage = image;
            OutputImage.texture = camTexture;
        }

        public void SetOutputRenderer(Renderer renderer)
        {
            OutputRenderer = renderer;
            OutputRenderer.material.mainTexture = camTexture;
        }



        void Update()
        {
            if (!isReaderEnabled)
                return;

            if (cameraFeedGrab == null)
            {
                cameraFeedGrab = camTexture.GetPixels32();
            }

            if(previousWebCamRotation != camTexture.videoRotationAngle)
            {
                if(camTexture.videoRotationAngle == 90 || camTexture.videoRotationAngle == 270)
                {
                    OutputImageScaler.referenceResolution = new Vector2(camTexture.height, camTexture.width);
                }
                else
                {
                    OutputImageScaler.referenceResolution = new Vector2(camTexture.width, camTexture.height);
                }

                OutputImage.rectTransform.sizeDelta = new Vector2((float) camTexture.width, (float) camTexture.height);
                OutputImage.rectTransform.rotation = Quaternion.Euler(0, 0, 360 - camTexture.videoRotationAngle);
                previousWebCamRotation = camTexture.videoRotationAngle;
            }

            if (!string.IsNullOrEmpty(LastResult) && LastResult != prevResult)
            {
                prevResult = LastResult;
                if (OnQrCodeDetected != null)
                    OnQrCodeDetected.Invoke(prevResult);

                if (OutputText != null)
                    OutputText.text = LastResult;
            }
        }

        void OnDestroy()
        {
            qrThread.Abort();
            camTexture.Stop();
        }

        // It's better to stop the thread by itself rather than abort it.
        void OnApplicationQuit()
        {
            isQuit = true;
        }

        void DecodeQR()
        {
            // create a reader with a custom luminance source
            var barcodeReader = new BarcodeReader { AutoRotate = false };

            while (true)
            {
                if (isQuit)
                    break;

                if (!isReaderEnabled)
                {
                    Thread.Sleep(200);
                    continue;
                }

                try
                {
                    // decode the current frame
                    var result = barcodeReader.Decode(cameraFeedGrab, W, H);
                    if (result != null)
                    {
                        LastResult = result.Text;
                        print(result.Text);
                    }

                    // Sleep a little bit and set the signal to get the next frame
                    Thread.Sleep(200);
                    cameraFeedGrab = null;
                }
                catch
                {
                }
            }
        }


        private int tries = 0;
        public void SwitchDevice()
        {
            camTexture.Stop();
            var currentDeviceIndex = WebCamTexture.devices
                .Select((t, index) => new { t, index })
                .Where(a => a.t.name == camTexture.deviceName)
                .Select(a => a.index)
                .DefaultIfEmpty(-1)
                .First();
            
            camTexture.deviceName = WebCamTexture.devices[(currentDeviceIndex +1) % WebCamTexture.devices.Length].name;
            camTexture.Play();
        }

        // .. COROUTINES

        IEnumerator EnableReaderRoutine()
        {
            if (isReaderEnabled)
                yield break;

            LastResult = string.Empty;
            prevResult = string.Empty;
            cameraFeedGrab = null;

            camTexture.Play();
            W = camTexture.width;
            H = camTexture.height;

            yield return new WaitForSeconds(0.5f);

            isReaderEnabled = true;
        }



    }
}

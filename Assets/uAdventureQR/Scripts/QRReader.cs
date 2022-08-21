using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;
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
        private Color32[] auxFlipArray;
        private bool isQuit;
        private bool isReaderEnabled;
        private bool isFeedFlipped;

        private string prevResult;
        private int previousWebCamRotation = -1;
        private bool previousWebCamVerticallyMirrored = false;

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

#if UNITY_IOS
            StartCoroutine(DecodeQRCoroutine());
#else
            qrThread = new Thread(DecodeQR);
            qrThread.Start();
#endif
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
            isFeedFlipped = false;

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
                isFeedFlipped = false;
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

            if (previousWebCamVerticallyMirrored != camTexture.videoVerticallyMirrored)
            {
                if (camTexture.videoVerticallyMirrored)
                {
                    OutputImage.rectTransform.localScale = new Vector3(1, -1, 1);
                }
                else
                {
                    OutputImage.rectTransform.localScale = new Vector3(1, 1, 1);
                }
                previousWebCamVerticallyMirrored = camTexture.videoVerticallyMirrored;
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
#if UNITY_IOS
            StopCoroutine(DecodeQRCoroutine());
#else
            qrThread.Abort();
#endif
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

                if (!isReaderEnabled || cameraFeedGrab == null)
                {
                    Thread.Sleep(200);
                    continue;
                }

                try
                {
                    FlipCameraFeedIfNeeded();
                    DecodeCurrentFrame(barcodeReader);

                    // Sleep a little bit and set the signal to get the next frame
                    Thread.Sleep(200);
                    cameraFeedGrab = null;
                }
                catch
                {
                }
            }
        }

        IEnumerator DecodeQRCoroutine()
        {
            // create a reader with a custom luminance source
            var barcodeReader = new BarcodeReader { AutoRotate = false };

            while (true)
            {
                if (isQuit)
                    yield break;

                if (!isReaderEnabled || cameraFeedGrab == null)
                {
                    yield return new WaitForSeconds(0.2f);
                    continue;
                }

                try
                {
                    FlipCameraFeedIfNeeded();
                    DecodeCurrentFrame(barcodeReader);
                }
                catch
                {
                }

                // Sleep a little bit and set the signal to get the next frame
                yield return new WaitForSeconds(0.2f);
                cameraFeedGrab = null;
            }
        }

        private void DecodeCurrentFrame(BarcodeReader barcodeReader)
        {
            // decode the current frame
            var result = barcodeReader.Decode(cameraFeedGrab, W, H);
            if (result != null)
            {
                LastResult = result.Text;
                print(result.Text);
            }
        }

        private void FlipCameraFeedIfNeeded()
        {
            if (previousWebCamVerticallyMirrored && !isFeedFlipped)
            {
                if (auxFlipArray == null || auxFlipArray.Length != cameraFeedGrab.Length)
                {
                    auxFlipArray = new Color32[cameraFeedGrab.Length];
                }

                for (int i = 0; i < H; i++)
                {
                    Array.Copy(cameraFeedGrab, i * W, auxFlipArray, (H - i - 1) * W, W);
                }
                cameraFeedGrab = auxFlipArray;
                isFeedFlipped = true;
            }
        }

        public void SwitchDevice()
        {
            StartCoroutine(SwitchDeviceRoutine());
        }

        private IEnumerator SwitchDeviceRoutine()
        {
            isReaderEnabled = false;
            var switched = 0;
            while (!isReaderEnabled && switched < WebCamTexture.devices.Length - 2)
            {
                camTexture.Stop();
                var currentDeviceIndex = WebCamTexture.devices
                    .Select((t, index) => new { t, index })
                    .Where(a => a.t.name == camTexture.deviceName)
                    .Select(a => a.index)
                    .DefaultIfEmpty(-1)
                    .First();

                camTexture.deviceName = WebCamTexture.devices[(currentDeviceIndex + 1) % WebCamTexture.devices.Length].name;
                camTexture.Play();
                W = camTexture.width;
                H = camTexture.height;

                yield return new WaitForSeconds(0.5f);
                if (camTexture.isPlaying)
                {
                    isReaderEnabled = true;
                }
                switched++;
            }
        }

        // .. COROUTINES

        IEnumerator EnableReaderRoutine()
        {
            if (isReaderEnabled)
                yield break;

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);

                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
            }

#elif UNITY_IOS
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.Log("Webcam authorized found");
            }
            else
            {
                Debug.Log("Webcam not authorized");
            }
#endif

            LastResult = string.Empty;
            prevResult = string.Empty;
            cameraFeedGrab = null;
            isFeedFlipped = false;

            camTexture.Play();
            W = camTexture.width;
            H = camTexture.height;

            yield return new WaitForSeconds(0.5f);

            isReaderEnabled = true;
        }



    }
}

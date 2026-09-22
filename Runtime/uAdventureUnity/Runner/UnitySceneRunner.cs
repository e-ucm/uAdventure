using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uAdventure.Runner;
using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;

namespace uAdventure.Unity
{
    [ChapterTargetFactory(typeof(UnityScene))]
    public class UnitySceneRunnerFactory : MonoBehaviour, IChapterTargetFactory
    {
        public IRunnerChapterTarget Instantiate(IChapterTarget modelObject)
        {
            var go = new GameObject("UnitySceneRunner", typeof(UnitySceneRunner));
            return go.GetComponent<UnitySceneRunner>();
        }
    }

    public class UnitySceneRunner : MonoBehaviour, IRunnerChapterTarget
    {
        // uAdventure Unity Scene Data Transfer
        private UnityScene unityScene;
        // OnDestroy holer for after the scenes are loaded
        private System.Action onDestroy;
        // previousCamera stores the camera used prior to any load
        private GameObject previousCamera;
        // previousTrasitionManager stores the transitionManager used prior to any load
        private TransitionManager previousTransitionManager;
        // previousRaycasterOverride stores the uAdventureRaycaster override value prior to any load
        private GameObject previousRaycasterOverride;

        public object Data
        {
            get { return unityScene; }
            set { unityScene = value as UnityScene; }
        }

        public bool IsReady
        {
            get; private set;
        }


        public bool canBeInteracted()
        {
            return false;
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        public void RenderScene()
        {
            // Back up the camera to differenciate it in case there is another one after loading
            previousCamera = Camera.main.gameObject;
            // Save the transition manager to restore its rendertexture and transition in the new TransitionManager if there's a new Camera
            previousTransitionManager = previousCamera.GetComponent<TransitionManager>();
            // Save also the override
            previousRaycasterOverride = previousCamera.GetComponent<uAdventureRaycaster>().Override;
            // Add the AfterLoadingScene to get the callback right after the scene is loaded
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += AfterLoadingScene; 
            // Load the scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(unityScene.Scene);
        }

        private void AfterLoadingScene(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            // Remove the callback of the scene loaded
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= AfterLoadingScene;

            // Look for a new camera different than the previous
            var newCamera = FindObjectsOfType<Camera>().Where(c => c.gameObject != previousCamera.gameObject).FirstOrDefault();

            // If we find it, we have to check some of its configurations, otherwise we keep it
            if (newCamera)
            {
                // We configure the new transition manager
                var newTransitionManager = newCamera.GetComponent<TransitionManager>();
                if (!newTransitionManager)
                {
                    newTransitionManager = newCamera.gameObject.AddComponent<TransitionManager>();
                }
                // This step restores the previous transition and RenderTexture
                newTransitionManager.CopyFrom(previousTransitionManager);
                // And then we forget about the previous transition manager
                previousTransitionManager = newTransitionManager;

                // If there's camera in the new scene loaded we destroy the old camera
                DestroyImmediate(previousCamera.gameObject);
                // We check if it has a Raycaster and if not, we add the uA raycaster
                if (!newCamera.GetComponent<BaseRaycaster>())
                {
                    var newRaycaster = newCamera.gameObject.AddComponent<uAdventureRaycaster>();
                    // Also we restore the override
                    newRaycaster.Override = previousRaycasterOverride;
                }
            }

            // Our render is ready
            IsReady = true;
        }

        public void Destroy(float time, System.Action onDestroy)
        {
            // We save the onDestroy for calling it in AfterGoingBack
            this.onDestroy = onDestroy;
            // Save the current camera in case its marked as DontDestroyOnLoad
            this.previousCamera = Camera.main.gameObject;
            // Save the uAdventureRaycaster Override
            var newRaycaster = previousCamera.GetComponent<uAdventureRaycaster>();
            previousRaycasterOverride = newRaycaster ? newRaycaster.Override : null;
            // Set the AfterGoingBack Callback to get the callback after loading _Scene1
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += AfterGoingBack;
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Scene1");
            DestroyImmediate(this.gameObject);
        }

        private void AfterGoingBack(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            // We remove the callback
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= AfterGoingBack;

            // We look for the new camera and restore the transition manager and the raycaster override
            var newCamera = FindObjectsOfType<Camera>().Where(c => c.gameObject != previousCamera.gameObject).FirstOrDefault();
            newCamera.GetComponent<TransitionManager>().CopyFrom(previousTransitionManager);
            newCamera.GetComponent<uAdventureRaycaster>().Override = previousRaycasterOverride;
            DontDestroyOnLoad(newCamera);

            // If we still have a previous camera (in case its marked as dont destroy on load, we destroy it
            if (previousCamera)
            {
                DestroyImmediate(previousCamera);
            }

            onDestroy();
        }

        public void setInteractuable(bool state)
        {
        }
    }
}

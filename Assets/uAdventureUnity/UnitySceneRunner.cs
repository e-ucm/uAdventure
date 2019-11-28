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
        private UnityScene unityScene;

        public object Data
        {
            get { return unityScene; }
            set { unityScene = value as UnityScene; }
        }

        public bool IsReady
        {
            get; private set;
        }

        public GameObject gameObject
        {
            get; set;
        }

        public void Start()
        {
            RenderScene();
        }



        public bool canBeInteracted()
        {
            return false;
        }

        public void Destroy(float time = 0)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(unityScene.Scene);
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Scene1");
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        private Camera previousCamera;
        public void RenderScene()
        {
            previousCamera = FindObjectOfType<Camera>();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded; 
            UnityEngine.SceneManagement.SceneManager.LoadScene(unityScene.Scene);
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            var newCamera = FindObjectsOfType<Camera>().Where(c => c != previousCamera).FirstOrDefault();

            // If there's no camera in the new scene loaded we clone the old camera
            if (!newCamera)
            {
                newCamera = Instantiate(previousCamera);
            }
            // Otherwise, we check if it has a Raycaster and if not, we add the uA raycaster
            else if (!newCamera.GetComponent<BaseRaycaster>())
            {
                newCamera.gameObject.AddComponent<uAdventureRaycaster>();
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            IsReady = true;
        }

        public void setInteractuable(bool state)
        {
            throw new System.NotImplementedException();
        }
    }
}

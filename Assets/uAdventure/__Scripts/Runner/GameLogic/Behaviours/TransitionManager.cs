using System;
using System.Collections;
using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Runner
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private Material transitionMaterial;
        private Texture transitionTexture;
        private RenderTexture renderTexture;
        private bool transitioning;
        private Transition transition;
        private Coroutine transitionRoutine;
        private float endTime;
        private System.Action<Transition, Texture> onFinish;
        private bool _shuttingDown = false;

        void Awake()
        {
            /*renderTexture = new RenderTexture(Screen.width/2, Screen.height/2, 24);
            renderTexture.Create();
            Update();*/
        }

        void Update()
        {
            if(this.gameObject == Camera.main.gameObject && !transitioning && (renderTexture == null || renderTexture.width != Screen.width/2 || renderTexture.height != Screen.height/2))
            {
                if (renderTexture)
                {
                    renderTexture.Release();
                    Destroy(renderTexture);
                }

                renderTexture = new RenderTexture(Screen.width/2, Screen.height/2, 24);
                renderTexture.Create();
            }

            if (transitioning && transitionRoutine != null && Time.time > endTime)
            {
                FinalizeTransition(transition, transitionTexture, onFinish);
            }
        }

        private void OnDisable()
        {
            if (renderTexture)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }
        }

        public void UseMaterial(Material material)
        {
            this.transitionMaterial = material;
        }

        public void PrepareTransition(Transition transition)
        {
            if (transitionRoutine != null)
            {
                Debug.LogError("The transition manager was already transitioning!");
                return;
            }

            CreateTransitionTexture();
            PrepareTransition(transition, renderTexture);
        }

        public void PrepareTransition(Transition transition, Texture transitionTexture)
        {
            if (transitionRoutine != null)
            {
                Debug.LogError("The transition manager was already transitioning!");
                return;
            }

            this.transitioning = true;
            this.transitionTexture = transitionTexture;
            this.transition = transition;
        }

        public void DoTransition(System.Action<Transition, Texture> onFinish)
        {
            if (transitionRoutine != null)
            {
                Debug.LogError("The transition was already started!");
                FinalizeTransition(transition, transitionTexture, onFinish);
                return;
            }

            if (transition == null || transition.getType() == TransitionType.NoTransition || transition.getTime() == 0)
            {
                FinalizeTransition(transition, transitionTexture, onFinish);
                return;
            }

            this.endTime = Time.time + transition.getTime() / 1000f;
            this.onFinish = onFinish;
            this.transitionRoutine = StartCoroutine(TransitionRoutine(transition));
        }

        public void CopyFrom(TransitionManager otherTransitionManager)
        {
            if (transitionRoutine != null || otherTransitionManager.transitionRoutine != null)
            {
                Debug.LogError("The other transition manager is already doing a transition!");
                return;
            }

            this.transitionMaterial = otherTransitionManager.transitionMaterial;
            this.transitionTexture  = otherTransitionManager.transitionTexture;
            this.renderTexture      = otherTransitionManager.renderTexture;
            this.transitioning      = otherTransitionManager.transitioning;
            this.transition         = otherTransitionManager.transition;
        }

        private void OnDestroy()
        {
            _shuttingDown = true;
            if (renderTexture)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }
        }

        private void ResetMaterial()
        {
            if (_shuttingDown)
            {
                return;
            }

            transitionMaterial.SetFloat("_DirectionX", 0);
            transitionMaterial.SetFloat("_DirectionY", 0);
            transitionMaterial.SetFloat("_Progress", 0);
            transitionMaterial.SetFloat("_Blend", 0);
        }

        private IEnumerator TransitionRoutine(Transition transition)
        {
            var timeLeft = transition.getTime()/1000f;
            var totalTime = timeLeft;
            var fade = false;
            ResetMaterial();
            transitionMaterial.SetTexture("_TransitionTex", transitionTexture);

            switch (transition.getType())
            {
                case TransitionType.FadeIn:
                    fade = true;
                    break;
                case TransitionType.BottomToTop:
                    transitionMaterial.SetFloat("_DirectionY", -1);
                    break;
                case TransitionType.TopToBottom:
                    transitionMaterial.SetFloat("_DirectionY", 1);
                    break;
                case TransitionType.RightToLeft:
                    transitionMaterial.SetFloat("_DirectionX", 1);
                    break;
                case TransitionType.LeftToRight:
                    transitionMaterial.SetFloat("_DirectionX", -1);
                    break;
            }

            while (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                transitionMaterial.SetFloat(fade ? "_Blend" : "_Progress", Mathf.Clamp01(1 - (timeLeft / totalTime)));
                yield return null;
            }
        }

        private void FinalizeTransition(Transition transition, Texture transitionTexture, System.Action<Transition, Texture> onFinish)
        {
            transitioning = false;
            this.transition = null;
            ResetMaterial();
            transitionRoutine = null;
            transitionMaterial.SetTexture("_MainTex", transitionTexture);
            transitionMaterial.SetTexture("_TransitionTex", null);
            onFinish(transition, transitionTexture);
        }


        private void CreateTransitionTexture()
        {
            var mainCamera = Camera.main;
            mainCamera.targetTexture = renderTexture;

            RenderTexture.active = renderTexture;
            mainCamera.Render();

            RenderTexture.active = null;
            mainCamera.targetTexture = null;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (transitioning)
            {
                transitionMaterial.SetTexture("_TransitionTex", source);
                Graphics.Blit(transitionTexture, destination, transitionMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Runner
{

    [RequireComponent(typeof(TransitionManager))]
    public class Representable : MonoBehaviour
    {
#region Attributes

        public enum ResourceType { ANIMATION, TEXTURE };

        private TransitionManager transitionManager;
        private AutoGlower autoGlower;
        private Element element;
        private Context context;
        private Renderer rend;
        private ResourcesUni resource;
        protected float deformation;
        protected bool mirror;

        // Texture
        private Texture2D texture;

        private ResourceType resourceType = ResourceType.TEXTURE;
        private string animationUri;
        private eAnim eanim;
        private string then;
        private int currentFrame;
        private float currentFrameDuration = 0.5f;
        private float timeElapsedInCurrentFrame = 0;
        private bool isTransitioning;

        private Orientation orientation;

        public delegate void RepresentableChangedDelegate();

        public RepresentableChangedDelegate RepresentableChanged;

        #endregion Attributes

        #region Properties
        public string Animation {
            get { return animationUri; }
            set
            {
                if (animationUri != value)
                {
                    animationUri = value;
                    resourceType = ResourceType.ANIMATION;
                    eAnim = GetAnimationInPriority(animationUri, Orientation, ref mirror);
                }
            }
        }

        public eAnim eAnim { get { return eanim; } set
            {
                if (value != eanim)
                {
                    eanim = value;
                    resourceType = ResourceType.ANIMATION;
                    StartAnimation();
                }
            }
        }


        public Context Context
        {
            get { return context; }
            set
            {
                context = value;
                Orientation = context.Orientation;
            }
        }

        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    if (resourceType == ResourceType.ANIMATION)
                    {
                        eAnim = GetAnimationInPriority(animationUri, orientation, ref mirror);
                    }
                }

                if (context != null)
                {
                    context.Orientation  = orientation;
                }
            }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (value != texture)
                {
                    resourceType = ResourceType.TEXTURE;
                    texture = value;
                    Adaptate();
                }
            }
        }

        public Vector2 Size
        {
            get
            {
                Vector2 size = new Vector2(50, 50);
                if (Texture)
                {
                    size = new Vector2(texture.width, texture.height);
                }
                return size * Context.Scale;

            }
        }


        public Element Element
        {
            get { return element; }
            set
            {
                element = value;
                if (element != null)
                {
                    this.gameObject.name = element.getId();
                }
            }
        }

        public ResourceType ResourceMode
        {
            get { return resourceType; }
            set
            {
                if(resourceType != value)
                {
                    resourceType = value;
                    if (resourceType == ResourceType.ANIMATION)
                    {
                        eAnim = GetAnimationInPriority(animationUri, orientation, ref mirror);
                    }
                }
            }
        }

        #endregion Properties

        protected virtual void Start()
        {
            rend = this.GetComponent<Renderer>();
            transitionManager = this.GetComponent<TransitionManager>();
            transitionManager.UseMaterial(rend.material);
            autoGlower = this.GetComponent<AutoGlower>();
            Game.Instance.GameState.OnConditionChanged += OnConditionChanged;
            OnConditionChanged(null, 0);
        }

        protected virtual void OnDestroy()
        {
            if (Game.Instance)
            {
                Game.Instance.GameState.OnConditionChanged -= OnConditionChanged;
            }
        }
        
        private void OnConditionChanged(string condition, int value)
        {
            checkResources();
            if (this && gameObject)
            {
                gameObject.SetActive(!Context.IsRemoved() && ConditionChecker.check(Context.Conditions)); 
            }
        }

        public void checkResources()
        {
            if(element == null || element.getResources() == null)
            {
                return;
            }
            var prevResource = this.resource;

            this.resource = element.getResources()
                .Find(res => ConditionChecker.check(res.getConditions()));

            if (prevResource != resource)
            {
                if (ResourceMode == ResourceType.ANIMATION)
                {
                    var toPlay = animationUri;
                    animationUri = null;
                    this.Play(toPlay);
                }
                else
                {
                    this.Adaptate();
                }
            }
        }

        public void Adaptate()
        {
            if(rend == null)
            {
                rend = this.GetComponent<Renderer>();
            }

            if (texture)
            {
                rend.material.mainTexture = texture;
            }
            var worldSize = SceneMB.ToWorldSize(Size);
            // Mirror
            worldSize.Scale(new Vector3((mirror ? -1 : 1), 1, 1));
            // Set
            transform.localScale = worldSize;

            if(this.autoGlower)
                this.autoGlower.enabled = context.Glow;
            if (RepresentableChanged != null)
            {
                RepresentableChanged();
            }
        }

        //##############################################
        //################ TEXTURE PART ################
        //##############################################

        protected Texture2D LoadTexture(string uri)
        {
            return Game.Instance.ResourceManager.getImage(resource.getAssetPath(uri));
        }

        public void SetTexture(string uri)
        {
            ResourceMode = ResourceType.TEXTURE;
            texture = LoadTexture(uri);
            Adaptate();
        }

        private static string OrientationToText(Orientation orientation)
        {
            switch (orientation)
            {
                default:
                case Orientation.S: return "down";
                case Orientation.N: return "up";
                case Orientation.E: return "right";
                case Orientation.O: return "left";
            }

        }

        //#################################################
        //################ ANIMATIONS PART ################
        //#################################################

        public bool Play(string animation, string then = null)
        {
            ResourceMode = ResourceType.ANIMATION;
            this.then = then;
            this.Animation = animation;

            return true;
        }

        protected void StartAnimation()
        {
            if(eanim != null)
            {
                timeElapsedInCurrentFrame = 0;
                SetFrame(0);
            }
        }

        protected static string FullAnimationUri(string animationName, Orientation orientation)
        {
            string uri = animationName;
            switch (animationName)
            {
                case "speak": uri = "speak" + OrientationToText(orientation); break;
                case "walk" : uri = "walk"  + OrientationToText(orientation); break;
                case "use"  : uri = "use"   + OrientationToText(orientation); break;
                case "stand": uri = "stand" + OrientationToText(orientation); break;
                case "actionAnimation":
                    if (orientation == Orientation.O)
                    {
                        uri = "actionAnimation" + OrientationToText(orientation);
                    }
                    break;
            }
            return uri;
        }

        protected eAnim LoadAnimation(string uri)
        {
            if (resource == null)
            {
                checkResources();
                if (resource == null)
                {
                    return null;
                }
            }

            var animationPath = resource.getAssetPath(uri);

            if (!string.IsNullOrEmpty(animationPath) && !animationPath.EndsWith(SpecialAssetPaths.ASSET_EMPTY_ANIMATION))
            {
                return Game.Instance.ResourceManager.getAnimation(animationPath);
            }

            return null;
        }

        protected Texture2D GetFrameTexture(int framenumber)
        {
            currentFrame = framenumber % eAnim.frames.Count;
            return eAnim.frames[currentFrame].Image;
        }

        protected void SetFrame(int framenumber)
        {
            if(eAnim != null)
            {
                currentFrame = framenumber % eAnim.frames.Count;
                texture = eAnim.frames[currentFrame].Image;
                currentFrameDuration = eAnim.frames[currentFrame].Duration / 1000f;
                Adaptate();
            }
        }

        private void NextFrame()
        {
            if (eAnim != null)
            {
                timeElapsedInCurrentFrame -= currentFrameDuration;
                Transition transition = null;
                if (eAnim.Animation.isUseTransitions())
                {
                    transition = eAnim.Animation.getTranstionForFrame(currentFrame);
                }
                currentFrame++;
                if (!string.IsNullOrEmpty(then) && currentFrame == eAnim.frames.Count)
                {
                    Play(then);
                }
                else if (transition != null)
                {
                    isTransitioning = true;
                    transitionManager.PrepareTransition(transition, GetFrameTexture(currentFrame));
                    transitionManager.DoTransition((_, __) =>
                    {
                        isTransitioning = false;
                        SetFrame(currentFrame);
                    });
                }
                else
                {
                    SetFrame(currentFrame);
                }
            }
        }

        protected static Orientation GetNextOrientationInPriority(Orientation orientation, ref bool isMirror)
        {
            Orientation next;
            switch (orientation)
            {
                default:
                    next     = Orientation.S;
                    isMirror = false;
                    break;
                case Orientation.E:
                    next     = isMirror ? Orientation.S : Orientation.O;
                    isMirror = !isMirror;
                    break;
                case Orientation.O:
                    next     = isMirror ? Orientation.S : Orientation.E;
                    isMirror = !isMirror;
                    break;
            }

            return next;
        }

        protected static string GetNextAnimationInPriority(string animation)
        {
            return "stand";
        }

        protected static bool NextAnimation(Orientation originalOrientation, ref string animation, ref Orientation orientation, ref bool isMirror)
        {
            var previousOrientation = orientation;
            var previousMirror = isMirror;

            orientation = GetNextOrientationInPriority(orientation, ref isMirror);

            if (previousOrientation == orientation && previousMirror == isMirror)
            {
                orientation = originalOrientation;
                isMirror = false;
                var prevAnimation = animation;

                animation = GetNextAnimationInPriority(animation);

                if(animation == prevAnimation)
                {
                    return false;
                }
            }

            return true;
        }

        protected eAnim GetAnimationInPriority(string animation, Orientation orientation, ref bool isMirror)
        {
            if (string.IsNullOrEmpty(animation))
            {
                return null;
            }

            eAnim loadedAnimation = null;
            bool loaded = false;
            isMirror = false;
            var originalOrientation = orientation;

            do
            {
                var animationToLoad = FullAnimationUri(animation, orientation);
                loadedAnimation = LoadAnimation(animationToLoad);
                loaded = loadedAnimation != null && loadedAnimation.Animation != null;
            } while (!loaded && NextAnimation(originalOrientation, ref animation, ref orientation, ref isMirror));

            return loadedAnimation;
        }

        protected virtual void Update()
        {
            if (resourceType == ResourceType.ANIMATION && eAnim != null && !isTransitioning)
            {
                timeElapsedInCurrentFrame += Time.deltaTime;
                
                if (timeElapsedInCurrentFrame >= currentFrameDuration)
                {
                    this.NextFrame();
                }
            }

            // Callback to update when zooming (scroll and pinch cases)
            if ((Input.mouseScrollDelta != Vector2.zero)
                || (Input.touchCount >= 2 && Input.touches[0].deltaPosition != Vector2.zero && Input.touches[1].deltaPosition != Vector2.zero))
            {
                this.Adaptate();
            }
        }
    }
}
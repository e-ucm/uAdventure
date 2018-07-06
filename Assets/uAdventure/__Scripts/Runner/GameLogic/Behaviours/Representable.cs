using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Runner
{
    public enum Orientation
    {
        N, E, S, O
    }

    public class Representable : MonoBehaviour, Movable
    {
        public static readonly Vector2 RepresentablePivot = new Vector2(0.5f, 0f);

#region Attributes

        public enum ResourceType { ANIMATION, TEXTURE };

        private Element element;
        private Renderer rend;
        private ResourcesUni resource;
        protected float deformation;
        protected bool mirror;

        // Texture
        private int hasovertex = -1;
        private Texture2D texture;

        private ResourceType resourceType = ResourceType.TEXTURE;
        private string animationUri;
        private eAnim eanim = null;
        private string then = null;
        private int currentFrame;
        private float currentFrameDuration = 0.5f;
        private float timeElapsedInCurrentFrame = 0;
        private float z = 0;

        #endregion Attributes

        #region Properties
        public string Animation {
            get { return animationUri; }
            set
            {
                if(animationUri != value)
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
        public SceneMB Scene { get; set; }

        private ElementReference context;


        public ElementReference Context
        {
            get { return context; }
            set
            {
                context = value;
                Orientation = context.GetOrientation();
            }
        }

        private Orientation orientation;
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                
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
                    context.SetOrientation(orientation);
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
                return size * Context.getScale();

            }
        }

        public float Z
        {
            get { return z; }
            set
            {
                if (value != z)
                {
                    z = value;
                    this.Positionate();
                }
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
                    if(resourceType == Representable.ResourceType.ANIMATION)
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
            checkResources();
            Orientation = Orientation.E;
        }

        protected void checkResources()
        {
            foreach (ResourcesUni resource in element.getResources())
            {
                if (ConditionChecker.check(resource.getConditions()))
                {
                    this.resource = resource;
                    break;
                }
            }
        }

        public void Adaptate()
        {
            if (texture)
            {
                rend.material.mainTexture = texture;
            }
            var worldSize = Scene.ToWorldSize(Size);
            // Mirror
            worldSize.Scale(new Vector3((mirror ? -1 : 1), 1, 1));
            // Set
            transform.localScale = worldSize;
        }

        public void Positionate()
        {
            if (!rend)
            {
                rend = this.GetComponent<Renderer>();
            }

            transform.localPosition = Scene.ToWorldPosition(getPosition(), Size, RepresentablePivot, z);
        }

        public void setPosition(Vector2 position)
        {
            Context.setPosition((int) position.x, (int) position.y);
            Positionate();
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
            Positionate();
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

        /*  ## ANIMATION METHOD ##
         * 
         * private Animation current_anim;
         * 
         * Texture2D tmp = current_anim.getFrame(0).getImage(false,false,0).texture;
            update_ratio = current_anim.getFrame(0).getTime();//Duration/1000f;
         * 
         * return (current_anim.getFrame(current_frame).getImage(false,false,0).texture.height) * context.getScale();
         * 
         * current_frame = (current_frame + 1) % current_anim.getFrames().Count;
            Texture2D tmp = current_anim.getFrame(current_frame).getImage(false,false,0).texture;
            update_ratio = current_anim.getFrame(current_frame).getTime();
         * 
         */

        public bool Play(string animation, string then = null)
        {
            ResourceMode = ResourceType.TEXTURE;
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
                Positionate();
            }
        }

        protected static string FullAnimationUri(string animationName, Orientation orientation)
        {
            string uri = animationName;
            switch (animationName)
            {
                case "speak": uri = "speak" + OrientationToText(orientation); break;
                case "walk": uri = "walk" + OrientationToText(orientation); break;
                case "use": uri = "use" + OrientationToText(orientation); break;
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
            var animationPath = resource.getAssetPath(uri);

            if (!string.IsNullOrEmpty(animationPath) && !animationPath.EndsWith(SpecialAssetPaths.ASSET_EMPTY_ANIMATION))
            {
                return Game.Instance.ResourceManager.getAnimation(animationPath);
            }

            return null;
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
                currentFrame++;
                if (!string.IsNullOrEmpty(then) && currentFrame == eAnim.frames.Count)
                {
                    Play(then);
                }
                SetFrame(currentFrame);
            }
        }

        protected Orientation GetNextOrientationInPriority(Orientation orientation, ref bool isMirror)
        {
            Orientation next;
            switch (orientation)
            {
                default:
                    next = Orientation.S;
                    isMirror = false;
                    break;
                case Orientation.E:
                    next = isMirror ? Orientation.S : Orientation.O;
                    isMirror = !isMirror;
                    break;
                case Orientation.O:
                    next = isMirror ? Orientation.S : Orientation.E;
                    isMirror = !isMirror;
                    break;
            }

            return next;
        }

        protected string GetNextAnimationInPriority(string animation)
        {
            return "stand";
        }

        protected bool NextAnimation(Orientation originalOrientation, ref string animation, ref Orientation orientation, ref bool isMirror)
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
            eAnim loadedAnimation = null;
            bool loaded = false;
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
            if (resourceType == ResourceType.ANIMATION && eAnim != null)
            {
                timeElapsedInCurrentFrame += Time.deltaTime;

                if (timeElapsedInCurrentFrame >= currentFrameDuration)
                {
                    this.NextFrame();
                }
            }
        }

        public Vector2 getPosition()
        {
            return new Vector2(Context.getX(), Context.getY());
        }
    }
}
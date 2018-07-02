using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;

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

        private string then = null;
        private int currentFrame;
        private float currentFrameDuration = 0.5f;
        private float timeElapsedInCurrentFrame = 0;
        private float z = 0;

#endregion Attributes

#region Properties
        public eAnim Animation { get; set; }
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



        #endregion Properties

        protected void Awake()
        {
            Orientation = Orientation.O;
        }

        protected virtual void Start()
        {
            rend = this.GetComponent<Renderer>();
            checkResources();
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
            texture = LoadTexture(uri);
            Adaptate();
            Positionate();
        }

        private string OrientationToText(Orientation orientation)
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
            string uri = animation;
            switch (animation)
            {
                case "speak": uri = "speak" + OrientationToText(Orientation); break;
                case "walk":  uri = "walk"  + OrientationToText(Orientation); break;
                case "use":   uri = "use"   + OrientationToText(Orientation); break;
                case "stand": uri = "stand" + OrientationToText(Orientation); break;
                case "actionAnimation":
                    if(Orientation == Orientation.O)
                    {
                        uri = "actionAnimation" + OrientationToText(Orientation);
                    }
                    break;
            }
            this.then = then;

            return SetAnimation(uri);
        }
        
        private Orientation getAnimationOrientation(string uri)
        {
            Orientation orientation = Orientation.E;
            switch (uri)
            {
                // UP = N
                case NPC.RESOURCE_TYPE_STAND_UP:
                case NPC.RESOURCE_TYPE_WALK_UP:
                case NPC.RESOURCE_TYPE_SPEAK_UP:
                    orientation = Orientation.N;
                    break;
                // DOWN = S
                case NPC.RESOURCE_TYPE_STAND_DOWN:
                case NPC.RESOURCE_TYPE_WALK_DOWN:
                case NPC.RESOURCE_TYPE_SPEAK_DOWN:
                    orientation = Orientation.S;
                    break;
                // RIGHT = E
                case "actionAnimation":
                case NPC.RESOURCE_TYPE_STAND_RIGHT:
                case NPC.RESOURCE_TYPE_WALK_RIGHT:
                case NPC.RESOURCE_TYPE_USE_RIGHT:
                case NPC.RESOURCE_TYPE_SPEAK_RIGHT:
                default:
                    orientation = Orientation.E;
                    break;
                // LEFT = W
                case "actionAnimationLeft":
                case NPC.RESOURCE_TYPE_STAND_LEFT:
                case NPC.RESOURCE_TYPE_WALK_LEFT:
                case NPC.RESOURCE_TYPE_USE_LEFT:
                case NPC.RESOURCE_TYPE_SPEAK_LEFT:
                    orientation = Orientation.O;
                    break;
            }

            return orientation;
        }

        private string GetMirrorURI(string uri)
        {
            string mirror = uri;
            switch (uri)
            {
                // STAND
                case NPC.RESOURCE_TYPE_STAND_UP:    mirror = null;                          break;
                case NPC.RESOURCE_TYPE_STAND_DOWN:  mirror = null;                          break;
                case NPC.RESOURCE_TYPE_STAND_LEFT:  mirror = NPC.RESOURCE_TYPE_STAND_RIGHT; break;
                case NPC.RESOURCE_TYPE_STAND_RIGHT: mirror = NPC.RESOURCE_TYPE_STAND_LEFT;  break;
                // WALK
                case NPC.RESOURCE_TYPE_WALK_UP:     mirror = null;                          break;
                case NPC.RESOURCE_TYPE_WALK_DOWN:   mirror = null;                          break;
                case NPC.RESOURCE_TYPE_WALK_LEFT:   mirror = NPC.RESOURCE_TYPE_WALK_RIGHT;  break;
                case NPC.RESOURCE_TYPE_WALK_RIGHT:  mirror = NPC.RESOURCE_TYPE_WALK_LEFT;   break;
                // USING
                case NPC.RESOURCE_TYPE_USE_LEFT:    mirror = NPC.RESOURCE_TYPE_USE_RIGHT;   break;
                case NPC.RESOURCE_TYPE_USE_RIGHT:   mirror = NPC.RESOURCE_TYPE_USE_LEFT;    break;
                // SPEAK
                case NPC.RESOURCE_TYPE_SPEAK_UP:    mirror = null;                          break;
                case NPC.RESOURCE_TYPE_SPEAK_DOWN:  mirror = null;                          break;
                case NPC.RESOURCE_TYPE_SPEAK_LEFT:  mirror = NPC.RESOURCE_TYPE_SPEAK_RIGHT; break;
                case NPC.RESOURCE_TYPE_SPEAK_RIGHT: mirror = NPC.RESOURCE_TYPE_SPEAK_LEFT;  break;
                // CUSTOM ACTION
                case "actionAnimation":             mirror = "actionAnimationLeft";         break;
                case "actionAnimationLeft":         mirror = "actionAnimation";             break;
            }

            return mirror;
        }

        private bool IsMirrorable(string uri)
        {
            return GetMirrorURI(uri) != null;
        }

        protected eAnim LoadAnimation(string uri)
        {
            var anim = Game.Instance.ResourceManager.getAnimation(resource.getAssetPath(uri));
            mirror = false;

            if ((anim == null || anim.Animation == null || anim.Animation.isEmptyAnimation()) && IsMirrorable(uri))
            {
                anim = Game.Instance.ResourceManager.getAnimation(resource.getAssetPath(GetMirrorURI(uri)));
                mirror = true;

                if(anim == null)
                {
                    Debug.LogWarning("Couldn't load animation: " + uri);
                }
            }

            return anim;
        }

        protected bool SetAnimation(string uri)
        {
            timeElapsedInCurrentFrame = 0;
            var animationLoaded = LoadAnimation(uri);
            if (animationLoaded != null)
            {
                Animation = animationLoaded;
                SetFrame(0);
                Positionate();
            }
            return animationLoaded != null;
        }

        protected void SetFrame(int framenumber)
        {
            if(Animation != null)
            {
                currentFrame = framenumber % Animation.frames.Count;
                texture = Animation.frames[currentFrame].Image;
                currentFrameDuration = Animation.frames[currentFrame].Duration / 1000f;
                Adaptate();
            }
        }

        private void NextFrame()
        {
            if (Animation != null)
            {
                timeElapsedInCurrentFrame -= currentFrameDuration;
                currentFrame++;
                if (!string.IsNullOrEmpty(then) && currentFrame == Animation.frames.Count)
                {
                    Play(then);
                }
                SetFrame(currentFrame);
            }
        }

        protected virtual void Update()
        {
            if(Animation != null)
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
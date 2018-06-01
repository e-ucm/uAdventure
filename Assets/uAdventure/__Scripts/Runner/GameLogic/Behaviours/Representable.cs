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
        private ElementReference context;
        private ResourcesUni resource;
        protected float deformation;
        protected bool mirror;
        protected Orientation orientation = Orientation.O;

        private SceneMB scene;

        // Texture
        private int hasovertex = -1;
        private Texture2D texture;

        private string then = null;
        private eAnim anim;
        private int currentFrame;
        private float currentFrameDuration = 0.5f;
        private float timeElapsedInCurrentFrame = 0;
        private float z = 0;

#endregion Attributes

#region Properties

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
        public eAnim Animation
        {
            get { return anim; }
            set { anim = value; }
        }

        public SceneMB Scene
        {
            get { return scene; }
            set { scene = value; }
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
                //deformation = -0.01f * FindObjectsOfType(this.GetType()).Length;
            }
        }
        public ElementReference Context
        {
            get { return context; }
            set { context = value; }
        }

        public Orientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }


        #endregion Properties

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
            rend.material.mainTexture = texture;
            Vector2 tmpSize = texture ? new Vector2(texture.width, texture.height) : new Vector2(50,50);
            var worldSize = scene.ToWorldSize(tmpSize * context.getScale());
            // Mirror
            worldSize.Scale(new Vector3((mirror ? -1 : 1), 1, 1));
            // Set
            transform.localScale = worldSize;
        }

        public void Positionate()
        {
            if(!rend)
                rend = this.GetComponent<Renderer>();

            var texture = rend.material.mainTexture;
            Vector2 tmpSize = texture ? new Vector2(texture.width, texture.height) * context.getScale(): new Vector2(50,50);
            Vector2 tmpPos = new Vector2(context.getX(), context.getY());

            transform.localPosition = scene.ToWorldPosition(tmpPos, tmpSize, RepresentablePivot, z);
        }

        public float getHeight()
        {
            return this.GetComponent<Renderer>().material.mainTexture.height * context.getScale();
        }

        public Texture2D getTexture()
        {
            return (Texture2D)rend.material.mainTexture;
        }

        public void setPosition(Vector2 position)
        {
            this.context.setPosition((int) position.x, (int) position.y);
            Positionate();
        }

        //##############################################
        //################ TEXTURE PART ################
        //##############################################

        protected void LoadTexture(string uri)
        {
            texture = Game.Instance.ResourceManager.getImage(resource.getAssetPath(uri));
        }

        public void SetTexture(string uri)
        {
            texture = Game.Instance.ResourceManager.getImage(resource.getAssetPath(uri));
            Adaptate();
            Positionate();
        }

        protected bool hasOverSprite()
        {
            if (hasovertex == -1)
                hasovertex = resource.getAssetPath(Item.RESOURCE_TYPE_IMAGEOVER) != null ? 1 : 0;

            return hasovertex == 1;
        }

        private string orientationToText(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.N:
                    return "up";
                case Orientation.E:
                    return "right";
                case Orientation.O:
                    return "left";
                default:
                case Orientation.S:
                    return "down";
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
                case "speak": uri = "speak" + orientationToText(orientation); break;
                case "walk":  uri = "walk"  + orientationToText(orientation); break;
                case "use":   uri = "use"   + orientationToText(orientation); break;
                case "stand": uri = "stand" + orientationToText(orientation); break;
                case "actionAnimation":
                    {
                        if(orientation == Orientation.O)
                            uri = "actionAnimation" + orientationToText(orientation);
                    }
                    break;
            }
            this.then = then;

            return setAnimation(uri);
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

        private string getMirrorUri(string uri)
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

        private bool isMirrorable(string uri)
        {
            return getMirrorUri(uri) != null;
        }

        protected bool LoadAnimation(string uri)
        {
            anim = Game.Instance.ResourceManager.getAnimation(resource.getAssetPath(uri));
            mirror = false;

            if ((anim == null || anim.Animation == null || anim.Animation.isEmptyAnimation()) && isMirrorable(uri))
            {
                anim = Game.Instance.ResourceManager.getAnimation(resource.getAssetPath(getMirrorUri(uri)));
                mirror = true;

                if(anim == null)
                {
                    Debug.LogWarning("Couldn't load animation: " + uri);
                }
            }

            return anim != null;
        }

        protected bool setAnimation(string uri)
        {
            timeElapsedInCurrentFrame = 0;
            var loaded = LoadAnimation(uri);
            if (loaded)
            {
                setFrame(0);
                Positionate();
            }
            return loaded;
        }

        protected void setFrame(int framenumber)
        {
            if(anim != null)
            {
                currentFrame = framenumber % anim.frames.Count;
                texture = anim.frames[currentFrame].Image;
                currentFrameDuration = anim.frames[currentFrame].Duration / 1000f;
                Adaptate();
            }
        }

        private void nextFrame()
        {
            timeElapsedInCurrentFrame -= currentFrameDuration;
            ++currentFrame;
            if (!string.IsNullOrEmpty(then) && currentFrame == anim.frames.Count)
            {
                Play(then);
            }
            setFrame(currentFrame);
        }

        protected virtual void Update()
        {
            if(anim != null)
            {
                timeElapsedInCurrentFrame += Time.deltaTime;

                if (timeElapsedInCurrentFrame >= currentFrameDuration)
                {
                    this.nextFrame();
                }
            }
        }

        public Vector2 getPosition()
        {
            return new Vector2(context.getX(), context.getY());
        }

        public void Traslate(Vector2 position)
        {
            this.context.setPosition((int)position.x, (int)position.y);
            Positionate();
        }
    }
}
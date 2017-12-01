using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public abstract class Representable : MonoBehaviour, Movable
    {

        public static int DIVISOR = 10, WIDTH = 80, HEIGHT = 60;

        public enum ResourceType { ANIMATION, TEXTURE };

        private Element element;
        private Renderer rend;
        private ElementReference context;
        private ResourcesUni resource;
        protected float deformation;

        public static Vector2 TransformPoint(Vector2 point)
        {
            return new Vector2(point.x / DIVISOR, 60 - (point.y / DIVISOR));
        }

        public Element Element
        {
            get { return element; }
            set
            {
                element = value;
                //deformation = -0.01f * FindObjectsOfType(this.GetType()).Length;
            }
        }
        public ElementReference Context
        {
            get { return context; }
            set { context = value; }
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

        protected void Adaptate()
        {
            rend.material.mainTexture = texture;
            this.transform.localScale = new Vector3(texture.width / DIVISOR, texture.height / DIVISOR, 1) * context.getScale();
        }

        protected void Positionate()
        {
            Vector2 tmppos = new Vector2(context.getX(), context.getY()) / DIVISOR + (new Vector2(0, -transform.localScale.y)) / 2;

            transform.localPosition = new Vector3(tmppos.x, HEIGHT - tmppos.y, -context.getLayer() + deformation);
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
            this.context.setPosition((int)(position.x * DIVISOR), (int)((HEIGHT - position.y) * DIVISOR));
            Positionate();
        }

        //##############################################
        //################ TEXTURE PART ################
        //##############################################

        int hasovertex = -1;
        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        protected void LoadTexture(string uri)
        {
            texture = ResourceManager.Instance.getImage(resource.getAssetPath(uri));
        }

        protected void setTexture(string uri)
        {
            texture = ResourceManager.Instance.getImage(resource.getAssetPath(uri));
            Adaptate();
            Positionate();
        }

        protected bool hasOverSprite()
        {
            if (hasovertex == -1)
                hasovertex = resource.getAssetPath(Item.RESOURCE_TYPE_IMAGEOVER) != null ? 1 : 0;

            return hasovertex == 1;
        }

        //#################################################
        //################ ANIMATIONS PART ################
        //#################################################

        private eAnim anim;
        private int current_frame;
        private float update_ratio = 0.5f;
        private float current_time = 0;

        public eAnim Animation
        {
            get { return anim; }
            set { anim = value; }
        }

        protected void LoadAnimation(string uri)
        {
            anim = ResourceManager.Instance.getAnimation(resource.getAssetPath(uri));
        }

        protected void setAnimation(string uri)
        {
            LoadAnimation(uri);
            setFrame(0);
            Positionate();
        }

        protected void setFrame(int framenumber)
        {
            current_frame = framenumber % anim.frames.Count;
            texture = anim.frames[current_frame].Image;
            update_ratio = anim.frames[current_frame].Duration / 1000f;

            Adaptate();
        }

        private void nextFrame()
        {
            current_time = 0;
            setFrame(current_frame + 1);
        }

        protected virtual void Update()
        {
            current_time += Time.deltaTime;

            if (current_time >= update_ratio)
            {
                this.nextFrame();
            }
        }

        public Vector2 getPosition()
        {
            Vector2 ret = new Vector2();

            if(this.anim != null)
                ret = new Vector2(this.transform.localPosition.x, this.transform.localPosition.y - (anim.frames[current_frame].Image.height * this.Context.getScale() / DIVISOR) / 2);
            else
                ret = new Vector2(this.transform.localPosition.x, this.transform.localPosition.y - (this.texture.height * this.Context.getScale() / DIVISOR) / 2);


            return ret;
        }

        public void Move(Vector2 position)
        {
            this.transform.localPosition = new Vector3(position.x, position.y, this.transform.localPosition.z);
            this.context.setPosition((int)position.x * DIVISOR, (int)(HEIGHT - position.y) * DIVISOR);
        }
    }
}
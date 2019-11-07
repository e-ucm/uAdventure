using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uAdventure.Runner
{
    public class Bubble : MonoBehaviour
    {
        public static readonly Vector2 Margin = new Vector2(20,20);

        private enum BubbleState { SHOWING, DESTROYING, NOTHING };

        BubbleState state = BubbleState.NOTHING;

        BubbleData data;
        public BubbleData Data
        {
            get { return data; }
            set { data = value; }
        }

        Transform text, image;
        RectTransform rectTransform;
        // Use this for initialization
        void Start()
        {
            //Image
            if (data.Image)
            {
                image = this.transform.Find("RawImage");
                image.GetComponent<RawImage>().texture = data.Image;
                var layoutElement = image.GetComponent<LayoutElement>();

                var ratio = data.Image.width / (float) data.Image.height;

                var height = Mathf.Min(240f, Mathf.Min(240f, data.Image.width) / ratio);
                var width = height * ratio;

                layoutElement.preferredHeight = height;
                layoutElement.preferredWidth = width;
            }
            //Audio
            if (data.Audio)
            {
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = data.Audio;
                audioSource.Play();

            }
            //resize ();
            text = this.transform.Find("Text");
            text.GetComponent<Text>().text = data.Line;
            rectTransform = this.GetComponent<RectTransform>();

            /*float guiscale = Screen.height/600f;

            text.GetComponent<Text>().fontSize = Mathf.RoundToInt(guiscale * 20);*/

            this.rectTransform.anchoredPosition = data.origin;
            this.moveTo(data.destiny);
            if(this.state != BubbleState.DESTROYING)
            {
                this.state = BubbleState.SHOWING;
            }
        }

        //######################################################################
        //############################## MOVEMENT ##############################
        //######################################################################
        private Vector2 finalPosition;
        private float distance;
        public float easing = 0.1f;
        public bool _____________________________;
        // fields set dynamically
        public float camZ; // The desired Z pos of the camera
        void Awake()
        {
            camZ = this.transform.position.z;
        }


        // TODO why destroy time is unused?
        //private float destroytime = 0.2f;
        private float currenttime = 0f;
        void FixedUpdate()
        {
            float completed = 0f;
            switch (state)
            {
                case BubbleState.NOTHING:
                    break;
                case BubbleState.DESTROYING:
                    currenttime += Time.deltaTime;
                    completed = 1f - currenttime / 0.2f;

                    setAlpha(completed);
                    setScale(completed);

                    if (completed <= 0)
                    {
                        GameObject.Destroy(this.gameObject);
                    }
                    break;
                case BubbleState.SHOWING:
                    Vector3 destination = finalPosition;

                    destination = Vector3.Lerp(this.rectTransform.anchoredPosition, destination, easing);
                    destination.z = camZ;


                    completed = 1f - (Vector2.Distance(destination, finalPosition) / distance);

                    if (float.IsNaN(completed))
                        completed = 1f;

                    setAlpha(completed);
                    setScale(completed);

                    this.rectTransform.anchoredPosition = destination;

                    if (completed >= 1f)
                    {
                        this.state = BubbleState.NOTHING;
                    }
                    break;
            }
        }

        private Vector2 getMaxSize()
        {
            setScale(1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var size = rectTransform.sizeDelta;
            setScale(0);
            return size;
        }

        private Vector2 encapsulateInParent(Vector2 position)
        {
            var canvasRectTransform = transform.parent.GetComponent<RectTransform>();
            // Calculate the destination rect
            var myRect = new Rect();
            // Calculate my size in the end
            myRect.size = getMaxSize();
            // Set the destination position
            myRect.center = position;
            // And trap the rect inside of the canvas rect
            return myRect.TrapInside(new Rect(Margin, canvasRectTransform.sizeDelta - 2 * Margin)).center;
        }

        public void moveTo(Vector2 position)
        {
            this.finalPosition = encapsulateInParent(position);
            // If the position on the Y axis has changed we try to move it below
            if (finalPosition.y != position.y)
            {
                var maxSize = getMaxSize();
                var inversePosition = 2 * rectTransform.anchoredPosition - position - new Vector2(0, maxSize.y * 0.5f);
                var encapsulatedInverse = encapsulateInParent(inversePosition);
                // If the inverse position is moved less than the original position then we go with the inverse
                if (encapsulatedInverse.y - inversePosition.y < position.y - finalPosition.y)
                {
                    this.finalPosition = encapsulatedInverse;
                }
            }
            
            this.distance = Vector2.Distance(rectTransform.anchoredPosition, position);
        }

        public void destroy()
        {
            this.state = BubbleState.DESTROYING;
        }

        float min_alpha = 0f, max_alpha = 1f;
        public void setAlpha(float percent)
        {
            float alpha = (max_alpha - min_alpha) * percent + min_alpha;
            this.GetComponent<Image>().color = new Color(data.BaseColor.r, data.BaseColor.g, data.BaseColor.b, alpha);
            this.GetComponent<Outline>().effectColor = new Color(data.OutlineColor.r, data.OutlineColor.g, data.OutlineColor.b, alpha);

            text.GetComponent<Text>().color = new Color(data.TextColor.r, data.TextColor.g, data.TextColor.b, alpha);
            text.GetComponent<Outline>().effectColor = new Color(data.TextOutlineColor.r, data.TextOutlineColor.g, data.TextOutlineColor.b, alpha);

            if (image)
            {
                image.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, alpha);
            }

        }

        float min_scale = 0.7f, max_scale = 1f;
        public void setScale(float percent)
        {
            float scale = (max_scale - min_scale) * percent + min_scale;
            this.transform.localScale = new Vector3(scale, scale, 1);
        }

        private float width = 200f;
        public void resize()
        {
            float newwidth = (Screen.width / 600f) * width;

            this.rectTransform.sizeDelta = new Vector2(newwidth, 0);
        }
    }
}
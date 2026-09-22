using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(RectTransform), typeof(Image), typeof(Outline))]
    public class Bubble : MonoBehaviour
    {
        private enum BubbleState { FADING, SHOWING, DESTROYING, NOTHING };

        public static readonly Vector2 Margin = new Vector2(20, 20);

        // Speed of the transition
        public float easing = 0.1f; 
        // Desired Z pos of the camera
        public float camZ;
        // Image component for the line image
        public RawImage image;
        // Audio source for the line sound
        public AudioSource audioSource;
        // Text component to put the line into
        public Text text;
        // Text displaying speed (characters per second)
        private int charactersPerSecond = 50;

        private Outline textBorder;
        private Image background;
        private Outline border;
        private RectTransform rectTransform, canvasRectTransform;
        private float completed;
        private float textCompleted;
        private Vector3 currentVelocity;

        // Scale constants for the bubble size transition
        private const float MinScale = 0.7f, MaxScale = 1f;
        // Alpha costants for the bubble alpha transition
        private const float MinAlpha = 0f, MaxAlpha = 1f;
        // Max height for the image in the bubble
        private const float Height = 240f;

        private BubbleState state = BubbleState.FADING;
        private Vector3 finalPosition;
        private float distance;
        private float currenttime = 0f;

        public BubbleData Data { get; set; }

        public InteractuableResult Interacted()
        {
            if(this.state == BubbleState.FADING)
            { 
                this.rectTransform.anchoredPosition = finalPosition;
                currentVelocity = Vector3.zero;
                completed = 1;
                Update();
            }
            if(this.state == BubbleState.SHOWING)
            {
                textCompleted = Data.Line.Length;
                Update();
                return InteractuableResult.REQUIRES_MORE_INTERACTION;
            }
            if(this.state == BubbleState.NOTHING || this.state == BubbleState.DESTROYING)
            {
                Destroy();
                return InteractuableResult.IGNORES;
            }
            return InteractuableResult.REQUIRES_MORE_INTERACTION;
        }

        public void Destroy()
        {
            this.state = BubbleState.DESTROYING;
        }

        /// <summary>
        /// Constructor method.
        /// Gather all the components and set up the initial Z.
        /// </summary>
        protected void Awake()
        {
            // Gather all the components
            rectTransform = GetComponent<RectTransform>();
            background = GetComponent<Image>();
            border = GetComponent<Outline>();
            textBorder = text.GetComponent<Outline>();

            camZ = this.transform.position.z;
        }

        /// <summary>
        /// Initialization method.
        /// Set up the image, text and audio of the bubble.
        /// </summary>
        protected void Start()
        {
            currentVelocity = Vector3.zero;
            canvasRectTransform = transform.parent.GetComponent<RectTransform>();
            // Image set up
            if (Data.Image)
            {
                image.texture = Data.Image;
                    
                // In case of a LayoutElement is present in the image, we set up its sizes
                var layoutElement = image.GetComponent<LayoutElement>();
                if (layoutElement)
                {
                    // The height and width of the element will be fixed to the height, but the image has to adjust 
                    // its width based on the image ratio
                    var ratio = Data.Image.width / (float)Data.Image.height;
                    var height = Mathf.Min(Height, Mathf.Min(Height, Data.Image.width) / ratio);
                    var width = height * ratio;
                    layoutElement.preferredHeight = height;
                    layoutElement.preferredWidth = width;
                }
            }

            // Audio set up
            if (Data.Audio)
            {
                audioSource.clip = Data.Audio;
                audioSource.Play();
            }

            // Text set up
            if (!string.IsNullOrEmpty(Data.Line))
            {
                SetTextProgress(0);
            }

            // We set up the screen position (anchored position) using the origin screen position to make the bubble 
            // pop up from the character of specified location
            this.rectTransform.anchoredPosition = Data.origin;
            this.MoveTo(Data.destiny);

            // Make sure the state is showing when started unless it's being destroyed (due to fast clicking for example)
            if (this.state != BubbleState.DESTROYING)
            {
                this.state = BubbleState.FADING;
                completed = 0;
            }
        }

        /// <summary>
        /// Update method used to animate and manage the current state flow.
        /// </summary>
        protected void Update()
        {
            switch (state)
            {
                case BubbleState.NOTHING:
                    {
                        // In the nothing state the bubble is just idle.
                    }
                    break;

                case BubbleState.DESTROYING:
                    {
                        // In the destroying state the bubble is fading out and finally destroyed.
                        currenttime += Time.deltaTime;
                        completed = 1f - currenttime / 0.2f;

                        SetAlpha(completed);
                        SetScale(completed);

                        if (completed <= 0)
                        {
                            GameObject.Destroy(this.gameObject);
                        }
                    }
                    break;
                case BubbleState.FADING:
                    {
                        finalPosition.z = camZ;
                        // In the showing state the bubble is fading in and finally moves to nothing (idle) state.

                        rectTransform.anchoredPosition = Vector3.SmoothDamp(rectTransform.anchoredPosition, finalPosition, ref currentVelocity, easing);

                        var ratioLeft = Vector2.Distance(finalPosition, rectTransform.anchoredPosition) / distance;
                        if(ratioLeft < 0.001)
                        {
                            ratioLeft = 0;
                        }
                        completed = Mathf.Clamp01(1f - ratioLeft);
                        textCompleted += Time.deltaTime * charactersPerSecond;

                        SetTextProgress(textCompleted / Data.Line.Length);
                        SetAlpha(completed);
                        SetScale(completed);


                        if (Mathf.Approximately(completed,1f))
                        {
                            completed = 0;
                            this.state = BubbleState.SHOWING;
                        }
                    }
                    break;
                case BubbleState.SHOWING:
                    {
                        // In the showing state the bubble is fading in and finally moves to nothing (idle) state.

                        textCompleted += Time.deltaTime * charactersPerSecond;
                        SetTextProgress(textCompleted / Data.Line.Length);

                        if (textCompleted >= Data.Line.Length)
                        {
                            this.state = BubbleState.NOTHING;
                            completed = 0;
                            textCompleted = 0;
                        }
                    }
                    break;
            }
        }

        private Vector2 GetMaxSize()
        {
            SetScale(1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var size = rectTransform.sizeDelta;
            SetScale(0);
            return size;
        }

        protected Vector2 EncapsulateInParent(Vector2 position)
        {
            // Calculate the destination rect
            var myRect = new Rect
            {
                // Calculate my size in the end
                size = GetMaxSize(),
                // Set the destination position
                center = position
            };
            // And trap the rect inside of the canvas rect
            return myRect.TrapInside(new Rect(Margin, canvasRectTransform.sizeDelta - 2 * Margin)).center;
        }

        protected void MoveTo(Vector2 position)
        {
            this.finalPosition = EncapsulateInParent(position);
            // If the position on the Y axis has changed we try to move it below
            if (finalPosition.y != position.y)
            {
                var maxSize = GetMaxSize();
                var inversePosition = 2 * rectTransform.anchoredPosition - position - new Vector2(0, maxSize.y * 0.5f);
                var encapsulatedInverse = EncapsulateInParent(inversePosition);
                // If the inverse position is moved less than the original position then we go with the inverse
                if (encapsulatedInverse.y - inversePosition.y < position.y - finalPosition.y)
                {
                    this.finalPosition = encapsulatedInverse;
                }
            }
            
            this.distance = Vector2.Distance(rectTransform.anchoredPosition, position);
        }

        protected void SetAlpha(float percent)
        {
            float alpha = (MaxAlpha - MinAlpha) * percent + MinAlpha;
            background.color = new Color(Data.BaseColor.r, Data.BaseColor.g, Data.BaseColor.b, alpha);
            border.effectColor = new Color(Data.OutlineColor.r, Data.OutlineColor.g, Data.OutlineColor.b, alpha);

            text.color = new Color(Data.TextColor.r, Data.TextColor.g, Data.TextColor.b, alpha);
            textBorder.effectColor = new Color(Data.TextOutlineColor.r, Data.TextOutlineColor.g, Data.TextOutlineColor.b, alpha);

            if (image)
            {
                image.color = new Color(1f, 1f, 1f, alpha);
            }
        }
        protected void SetScale(float percent)
        {
            float scale = (MaxScale - MinScale) * percent + MinScale;
            this.transform.localScale = new Vector3(scale, scale, 1);
        }

        protected void SetTextProgress(float percent)
        {
            var charactersShown = (int)(Data.Line.Length * Mathf.Clamp01(percent));
            string cutted = CutWithSymbols(Data.Line, charactersShown);
            var openedSymbols = FindOpenedSimbols(cutted);
            text.text = cutted + CloseOpenedSymbols(openedSymbols) + "<color=#00000000>" + RemoveSymbols(Data.Line.Substring(cutted.Length)) + "</color>";
        }

        private string CutWithSymbols(string line, int charactersShown)
        {
            var regex = @"<(\/?[A-z]+)(=#?[A-z0-9\.]+)*(\s[A-z]+=#?[A-z0-9\.]+)*>";

            string cutted = null;
            foreach (Match match in Regex.Matches(line, regex))
            {
                if(match.Index <= charactersShown && match.Index+match.Length > charactersShown)
                {
                    if (IsValidSymbol(match.Groups[1].Value))
                    {
                        charactersShown = match.Index;
                    }
                    break;
                }
            }

            if (cutted == null)
            {
                cutted = Data.Line.Substring(0, charactersShown);
            }

            return cutted;
        }

        private List<string> FindOpenedSimbols(string v)
        {
            var openedSymbols = new List<string>();
            var regex = @"<(\/?[A-z]+)(=#?[A-z0-9\.]+)*(\s[A-z]+=#?[A-z0-9\.]+)*>";

            foreach(Match match in Regex.Matches(v, regex))
            {
                var openedSymbol = match.Groups[1].Value;
                if (!IsValidSymbol(openedSymbol))
                {
                    continue;
                }

                var rest = v.Substring(match.Index);
                rest.IndexOf("</" + openedSymbol + ">");

                if (!openedSymbol.StartsWith("/"))
                {
                    openedSymbols.Add(match.Groups[1].Value);
                }
                else if (openedSymbols.Count > 0 && openedSymbol.Substring(1) == openedSymbols[openedSymbols.Count - 1])
                {
                    openedSymbols.RemoveAt(openedSymbols.Count - 1);
                }
                else
                {
                    throw new Exception("Malformed html!");
                }
            }

            return openedSymbols;
        }

        private string RemoveSymbols(string v)
        {
            var regex = @"<(\/?[A-z]+)(=#?[A-z0-9\.]+)*(\s[A-z]+=#?[A-z0-9\.]+)*>";
            return Regex.Replace(v, regex, t => IsValidSymbol(t.Groups[1].Value) ? "" : t.Value);
        }

        private string CloseOpenedSymbols(List<string> openedSymbols)
        {
            var r = "";
            foreach (var symbol in openedSymbols)
            {
                r += "</" + symbol + ">";
            }
            return r;
        }

        private bool IsValidSymbol(string openedSymbol)
        {
            if (openedSymbol.StartsWith("/"))
            {
                openedSymbol = openedSymbol.Substring(1);
            }

            switch (openedSymbol)
            {
                case "b":
                case "i":
                case "size":
                case "color":
                case "material":
                case "quad":
                    return true;
                default:
                    return false;
            }
        }
    }
}
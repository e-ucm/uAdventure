using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public class BubbleData
    {

        public static readonly Color BLACK = new Color(0, 0, 0, 1);

        private string line = "";
        public string Line
        {
            get { return line; }
            set { this.line = value; }
        }

        private Color baseColor = Color.white;
        public Color BaseColor
        {
            get { return baseColor; }
            set { this.baseColor = value; }
        }

        private Color outlineColor = Color.black;
        public Color OutlineColor
        {
            get { return outlineColor; }
            set { this.outlineColor = value; }
        }

        private Color textColor = Color.white;
        public Color TextColor
        {
            get { return textColor; }
            set { this.textColor = value; }
        }
        private Color textOutlineColor = Color.black;
        public Color TextOutlineColor
        {
            get { return textOutlineColor; }
            set { this.textOutlineColor = value; }
        }

        public Vector3 origin;
        public Vector3 Origin
        {
            get { return origin; }
            set { this.origin = value; }
        }

        public Vector3 destiny;
        public Vector3 Destiny
        {
            get { return destiny; }
            set { this.destiny = value; }
        }

        private GameObject talker;
        public GameObject Talker
        {
            get { return talker; }
            set { talker = value; }
        }

        public Texture2D Image { get; set; }
        public AudioClip Audio { get; set; }
        public bool TTS { get; set; }

        public BubbleData(string line, Vector3 origin, Vector3 destiny, GameObject talker = null)
        {
            initialize(line, origin, destiny, Color.white, Color.black, Color.white, Color.black, talker);
        }

        public BubbleData(string line, Vector3 origin, Vector3 destiny, Color baseColor, Color outlineColor, Color textColor, Color textOutlineColor, GameObject talker = null)
        {
            initialize(line, origin, destiny, baseColor, outlineColor, textColor, textOutlineColor, talker);
        }


        private void initialize(string line, Vector3 origin, Vector3 destiny, Color baseColor, Color outlineColor, Color textColor, Color textOutlineColor, GameObject talker = null)
        {
            this.line = line;

            this.origin = origin;
            this.destiny = destiny;

            this.baseColor = baseColor;
            this.outlineColor = outlineColor;
            this.textColor = textColor;
            this.textOutlineColor = textOutlineColor;
            this.talker = talker;
        }
    }
}
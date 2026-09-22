using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Simva
{
    public class FinalizeScene : SimvaScene
    {
        private string endText;
        private string backgroundPath;
        private Color endColor;
        private Color endBackgroundColor;
        private Color endBorder;

        public FinalizeScene() : base("Simva.Finalize")
        {
        }

        public string EndText
        {
            get
            {
                return endText;
            }

            set
            {
                endText = value;
            }
        }

        public string BackgroundPath
        {
            get
            {
                return backgroundPath;
            }

            set
            {
                backgroundPath = value;
            }
        }

        public Color EndColor
        {
            get
            {
                return endColor;
            }

            set
            {
                endColor = value;
            }
        }

        public Color EndBackgroundColor
        {
            get
            {
                return endBackgroundColor;
            }

            set
            {
                endBackgroundColor = value;
            }
        }

        public Color EndBorder
        {
            get
            {
                return endBorder;
            }

            set
            {
                endBorder = value;
            }
        }
    }
}

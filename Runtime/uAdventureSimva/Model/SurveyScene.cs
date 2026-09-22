using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Simva
{
    public class SurveyScene : SimvaScene
    {
        private string surveyTitle;
        private string continueText;
        private string openText;

        private string backgroundPath;
        private Color backgroundBorder;
        private Color backgroundColor;
        private Color continueColor;
        private Color continueBorder;
        private Color openColor;
        private Color openBorder;

        public SurveyScene() : base("Simva.Survey")
        {
        }

        public string SurveyTitle
        {
            get
            {
                return surveyTitle;
            }

            set
            {
                surveyTitle = value;
            }
        }

        public string ContinueText
        {
            get
            {
                return continueText;
            }

            set
            {
                continueText = value;
            }
        }

        public string OpenText
        {
            get
            {
                return openText;
            }

            set
            {
                openText = value;
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

        public Color BackgroundBorder
        {
            get
            {
                return backgroundBorder;
            }

            set
            {
                backgroundBorder = value;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }

            set
            {
                backgroundColor = value;
            }
        }

        public Color ContinueColor
        {
            get
            {
                return continueColor;
            }

            set
            {
                continueColor = value;
            }
        }

        public Color ContinueBorder
        {
            get
            {
                return continueBorder;
            }

            set
            {
                continueBorder = value;
            }
        }

        public Color OpenColor
        {
            get
            {
                return openColor;
            }

            set
            {
                openColor = value;
            }
        }

        public Color OpenBorder
        {
            get
            {
                return openBorder;
            }

            set
            {
                openBorder = value;
            }
        }
    }
}

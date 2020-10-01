using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Simva
{
    public class LoginScene : SimvaScene
    {
        private string loginTitle;
        private string tokenText;
        private string tokenPlaceholder;

        private string backgroundPath;
        private Color backgroundBorder;
        private Color backgroundColor;

        private Color loginColor;
        private Color loginBorder;
        private Color openColor;
        private Color openBorder;

        public LoginScene() : base("Simva.Login")
        {
        }

        public string LoginTitle
        {
            get
            {
                return loginTitle;
            }

            set
            {
                loginTitle = value;
            }
        }

        public string TokenText
        {
            get
            {
                return tokenText;
            }

            set
            {
                tokenText = value;
            }
        }

        public string TokenPlaceholder
        {
            get
            {
                return tokenPlaceholder;
            }

            set
            {
                tokenPlaceholder = value;
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

        public Color LoginColor
        {
            get
            {
                return loginColor;
            }

            set
            {
                loginColor = value;
            }
        }

        public Color LoginBorder
        {
            get
            {
                return loginBorder;
            }

            set
            {
                loginBorder = value;
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

using System.Xml;
using uAdventure.Core;

namespace uAdventure.Simva
{
    [DOMParser("login-scene", "survey-scene")]
    public class SimvaSceneDOMParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            switch (element.Name)
            {
                case "login-scene":
                    return ParseLoginScene(element, parameters);
                case "survey-scene":
                    return ParseSurveyScene(element, parameters);
            }
            return null;
        }

        private SurveyScene ParseSurveyScene(XmlElement element, params object[] parameters)
        {
            var background = (XmlElement)element.SelectSingleNode("background");
            var open = (XmlElement)element.SelectSingleNode("open");
            var cont = (XmlElement)element.SelectSingleNode("continue");

            return new SurveyScene()
            {
                // Background
                BackgroundBorder = ExParsers.ParseDefault(background.GetAttribute("border"), UnityEngine.Color.black),
                BackgroundColor = ExParsers.ParseDefault(background.GetAttribute("color"), new UnityEngine.Color(1, 1, 1, 0.6f)),
                BackgroundPath = background.GetAttribute("path"),

                // Open
                OpenBorder = ExParsers.ParseDefault(open.GetAttribute("border"), UnityEngine.Color.black),
                OpenColor = ExParsers.ParseDefault(open.GetAttribute("color"), new UnityEngine.Color(1, 1, 1, 0.6f)),
                OpenText = open.GetAttribute("text"),

                // Continue
                ContinueBorder = ExParsers.ParseDefault(cont.GetAttribute("border"), UnityEngine.Color.black),
                ContinueColor = ExParsers.ParseDefault(cont.GetAttribute("color"), new UnityEngine.Color(1, 1, 1, 0.6f)),
                ContinueText = cont.GetAttribute("text"),
            };
        }
        private LoginScene ParseLoginScene(XmlElement element, params object[] parameters)
        {
            var background = (XmlElement)element.SelectSingleNode("background");
            var login = (XmlElement)element.SelectSingleNode("login");
            var token = (XmlElement)element.SelectSingleNode("token");

            return new LoginScene()
            {
                // Background
                BackgroundBorder = ExParsers.ParseDefault(background.GetAttribute("border"), UnityEngine.Color.black),
                BackgroundColor = ExParsers.ParseDefault(background.GetAttribute("color"), new UnityEngine.Color(1, 1, 1, 0.6f)),
                BackgroundPath = background.GetAttribute("path"),

                // Login
                LoginTitle = login.GetAttribute("title"),
                LoginColor = ExParsers.ParseDefault(login.GetAttribute("color"), new UnityEngine.Color(1, 1, 1, 0.6f)),

                // Continue
                TokenText = token.GetAttribute("token"),
                TokenPlaceholder = token.GetAttribute("placeholder"),
            };
        }
    }
}

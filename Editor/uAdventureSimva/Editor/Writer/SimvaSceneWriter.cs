using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using uAdventure.Editor;

namespace uAdventure.Simva
{
    [DOMWriter(typeof(SimvaScene), typeof(LoginScene), typeof(SurveyScene))]
    public class SimvaSceneWriter : ParametrizedDOMWriter
    {
        protected override string GetElementNameFor(object target)
        {
            return (target is SurveyScene) ? "survey-scene" : "login-scene";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            if(target is LoginScene)
            {
                FillLoginScene(node, target as LoginScene);
            }
            else if (target is SurveyScene)
            {
                FillSurveyScene(node, target as SurveyScene);
            }
        }

        protected void FillLoginScene(XmlNode node, LoginScene loginScene)
        {
            var background = node.OwnerDocument.CreateElement("background");
            background.SetAttribute("color", loginScene.BackgroundColor.ToString());
            background.SetAttribute("path", loginScene.BackgroundPath.ToString());
            background.SetAttribute("border", loginScene.BackgroundBorder.ToString());
            node.AppendChild(background);

            var login = node.OwnerDocument.CreateElement("login");
            login.SetAttribute("text", loginScene.LoginTitle.ToString());
            login.SetAttribute("color", loginScene.LoginColor.ToString());
            login.SetAttribute("border", loginScene.LoginBorder.ToString());
            node.AppendChild(login);

            var token = node.OwnerDocument.CreateElement("token");
            token.SetAttribute("text", loginScene.TokenPlaceholder.ToString());
            token.SetAttribute("placeholder", loginScene.TokenText.ToString());
            node.AppendChild(token);

        }

        protected void FillSurveyScene(XmlNode node, SurveyScene surveyScene)
        {
            var background = node.OwnerDocument.CreateElement("background");
            background.SetAttribute("color", surveyScene.BackgroundColor.ToString());
            background.SetAttribute("path", surveyScene.BackgroundPath.ToString());
            background.SetAttribute("border", surveyScene.BackgroundBorder.ToString());
            node.AppendChild(background);

            var cont = node.OwnerDocument.CreateElement("continue");
            cont.SetAttribute("color", surveyScene.ContinueBorder.ToString());
            cont.SetAttribute("path", surveyScene.ContinueColor.ToString());
            cont.SetAttribute("border", surveyScene.ContinueText.ToString());
            node.AppendChild(cont);

            var open = node.OwnerDocument.CreateElement("open");
            open.SetAttribute("color", surveyScene.OpenBorder.ToString());
            open.SetAttribute("path", surveyScene.OpenColor.ToString());
            open.SetAttribute("border", surveyScene.OpenBorder.ToString());
            node.AppendChild(open);
        }

    }
}

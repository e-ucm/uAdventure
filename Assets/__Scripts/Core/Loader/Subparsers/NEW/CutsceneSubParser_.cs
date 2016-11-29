using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
    public class CutsceneSubParser_ : Subparser_
    {

        /**
        * Stores the current slidescene being parsed
        */
        private Cutscene cutscene;

        /**
         * Stores the current resources being parsed
         */
        private ResourcesUni currentResources;

        /**
         * Stores the current next-scene being used
         */
        private NextScene currentNextScene;

        /**
         * Stores the current conditions being parsed
         */
        private Conditions currentConditions;

        /**
         * Stores the current effects being parsed
         */
        private Effects currentEffects;

        public CutsceneSubParser_(Chapter chapter) : base(chapter)
        {
        }

        public override void ParseElement(XmlElement element)
        {
            XmlNodeList
                endsgame = element.SelectNodes("end-game"),
                nextsscene = element.SelectNodes("next-scene"),
                resourcess = element.SelectNodes("resources"),
                //descriptionss = element.SelectNodes("description"),
                assets,
                //conditions = element.SelectNodes("condition"),
                effects = element.SelectNodes("effect");
                //postseffects = element.SelectNodes("post-effect");

            string tmpArgVal;

            string slidesceneId = "";
            bool initialScene = false;
            string idTarget = "";
            int x = int.MinValue, y = int.MinValue;
            int transitionType = 0, transitionTime = 0;
            string next = "go-back";
            bool canSkip = true;


            tmpArgVal = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                slidesceneId = tmpArgVal;
            }
            tmpArgVal = element.GetAttribute("start");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                initialScene = tmpArgVal.Equals("yes");
            }
            tmpArgVal = element.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }
            tmpArgVal = element.GetAttribute("destinyX");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("destinyY");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("transitionType");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                transitionType = int.Parse(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("transitionTime");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                transitionTime = int.Parse(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("next");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                next = tmpArgVal;
            }
            tmpArgVal = element.GetAttribute("canSkip");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                canSkip = tmpArgVal.Equals("yes");
            }

            if (element.Name.Equals("slidescene"))
                cutscene = new Slidescene(slidesceneId);
            else
                cutscene = new Videoscene(slidesceneId);
            if (initialScene)
                chapter.setTargetId(slidesceneId);

            //XAPI ELEMENTS
            tmpArgVal = element.GetAttribute("class");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                cutscene.setXApiClass(tmpArgVal);
            }
            tmpArgVal = element.GetAttribute("type");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                cutscene.setXApiType(tmpArgVal);
            }
            //END OF XAPI

            cutscene.setTargetId(idTarget);
            cutscene.setPositionX(x);
            cutscene.setPositionY(y);
            cutscene.setTransitionType((NextSceneEnumTransitionType)transitionType);
            cutscene.setTransitionTime(transitionTime);

            if (element.SelectSingleNode("name") != null)
                cutscene.setName(element.SelectSingleNode("name").InnerText);
            if (element.SelectSingleNode("documentation") != null)
                cutscene.setDocumentation(element.SelectSingleNode("documentation").InnerText);

            foreach (XmlElement ell in effects)
            {
                currentEffects = new Effects();
                new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                cutscene.setEffects(currentEffects);
            }


            if (cutscene is Videoscene)
                ((Videoscene)cutscene).setCanSkip(canSkip);

            if (next.Equals("go-back"))
            {
                cutscene.setNext(Cutscene.GOBACK);
            }
            else if (next.Equals("new-scene"))
            {
                cutscene.setNext(Cutscene.NEWSCENE);
            }
            else if (next.Equals("end-chapter"))
            {
                cutscene.setNext(Cutscene.ENDCHAPTER);
            }

            foreach (XmlElement el in resourcess)
            {
                currentResources = new ResourcesUni();
                tmpArgVal = el.GetAttribute("name");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    currentResources.setName(el.GetAttribute(tmpArgVal));
                }

                assets = el.SelectNodes("asset");
                foreach (XmlElement ell in assets)
                {
                    string type = "";
                    string path = "";

                    tmpArgVal = ell.GetAttribute("type");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        type = tmpArgVal;
                    }
                    tmpArgVal = ell.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        path = tmpArgVal;
                    }
                    currentResources.addAsset(type, path);
                }

                XmlNodeList conditonss = el.SelectNodes("condition");
                foreach (XmlElement ell in conditonss)
                {
                    currentConditions = new Conditions();
                    new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                    currentResources.setConditions(currentConditions);
                }

                cutscene.addResources(currentResources);
            }

            for(int i = 0; i < endsgame.Count; i++)
            {
                cutscene.setNext(Cutscene.ENDCHAPTER);
            }

            foreach (XmlElement el in nextsscene)
            {
                string idTarget_ = "";
                int x_ = int.MinValue, y_ = int.MinValue;
                int transitionType_ = 0, transitionTime_ = 0;

                tmpArgVal = el.GetAttribute("idTarget");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    idTarget_ = tmpArgVal;
                }
                tmpArgVal = el.GetAttribute("x");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    x_ = int.Parse(tmpArgVal);
                }
                tmpArgVal = el.GetAttribute("y");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    y_ = int.Parse(tmpArgVal);
                }
                tmpArgVal = el.GetAttribute("transitionType");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    transitionType_ = int.Parse(tmpArgVal);
                }
                tmpArgVal = el.GetAttribute("transitionTime");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    transitionTime_ = int.Parse(tmpArgVal);
                }

                currentNextScene = new NextScene(idTarget_, x_, y_);
                currentNextScene.setTransitionType((NextSceneEnumTransitionType)transitionType_);
                currentNextScene.setTransitionTime(transitionTime_);
                XmlNodeList conditionss = el.SelectNodes("condition");
                foreach (XmlElement ell in conditionss)
                {
                    currentConditions = new Conditions();
                    new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                    currentNextScene.setConditions(currentConditions);
                }

                XmlNodeList effectsss = el.SelectNodes("effect");
                foreach (XmlElement ell in effectsss)
                {
                    currentEffects = new Effects();
                    new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                    currentNextScene.setEffects(currentEffects);
                }

                XmlNodeList postseffectsss = el.SelectNodes("post-effect");
                foreach (XmlElement ell in postseffectsss)
                {
                    currentEffects = new Effects();
                    new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                    currentNextScene.setPostEffects(currentEffects);
                }

                cutscene.addNextScene(currentNextScene);
            }

            chapter.addCutscene(cutscene);
        }

    }
}
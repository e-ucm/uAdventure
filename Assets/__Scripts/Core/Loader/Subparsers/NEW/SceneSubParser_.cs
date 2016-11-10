using UnityEngine;
using System.Collections;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

public class SceneSubParser_ : Subparser_
{
    /**
     * Stores the element being parsed
     */
    private Scene scene;

    /**
     * Stores the current resources being parsed
     */
    private ResourcesUni currentResources;

    /**
     * Stores the current exit being used
     */
    private Exit currentExit;

    /**
     * Stores the current exit look being used
     */
    private ExitLook currentExitLook;

    /**
     * Stores the current next-scene being used
     */
    private NextScene currentNextScene;

    private Vector2 currentPoint;

    /**
     * Stores the current element reference being used
     */
    private ElementReference currentElementReference;

    /**
     * Stores the current conditions being parsed
     */
    private Conditions currentConditions;

    /**
     * Stores the current effects being parsed
     */
    private Effects currentEffects;

    public SceneSubParser_(Chapter chapter) : base(chapter)
    {
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            resourcess = element.SelectNodes("resources"),
            assets,
            conditions,
            effects,
            postseffects,
            notseffect,
            defaultsinitialsposition = element.SelectNodes("default-initial-position"),
            exits = element.SelectNodes("exits/exit"),
            exitslook,
            nextsscene = element.SelectNodes("next-scene"),
            points,
            objectsrefs = element.SelectNodes("objects/object-ref"),
            charactersrefs = element.SelectNodes("characters/character-ref"),
            atrezzosrefs = element.SelectNodes("atrezzo/atrezzo-ref"),
            activesareas = element.SelectNodes("active-areas/active-area"),
            barriers = element.SelectNodes("barrier"),
            trajectorys = element.SelectNodes("trajectory");

        string tmpArgVal;

        string sceneId = "";
        bool initialScene = false;
        int playerLayer = -1;
        float playerScale = 1.0f;

        tmpArgVal = element.GetAttribute("id");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            sceneId = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("start");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            initialScene = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("playerLayer");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            playerLayer = int.Parse(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("playerScale");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            playerScale = float.Parse(tmpArgVal, CultureInfo.InvariantCulture);
        }

        scene = new Scene(sceneId);
        scene.setPlayerLayer(playerLayer);
        scene.setPlayerScale(playerScale);
        if (initialScene)
            chapter.setTargetId(sceneId);

        if (element.SelectSingleNode("name") != null)
            scene.setName(element.SelectSingleNode("name").InnerText);
        if (element.SelectSingleNode("documentation") != null)
            scene.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        //XAPI ELEMENTS
        tmpArgVal = element.GetAttribute("class");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            scene.setXApiClass(tmpArgVal);
        }
        tmpArgVal = element.GetAttribute("type");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            scene.setXApiType(tmpArgVal);
        }
        //END OF XAPI

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

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentResources.setConditions(currentConditions);
            }

            scene.addResources(currentResources);
        }

        foreach (XmlElement el in defaultsinitialsposition)
        {
            int x = int.MinValue, y = int.MinValue;

            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }

            scene.setDefaultPosition(x, y);
        }

        foreach (XmlElement el in exits)
        {
            int x = 0, y = 0, width = 0, height = 0;
            bool rectangular = true;
            int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
            bool hasInfluence = false;
            string idTarget = "";
            int destinyX = int.MinValue, destinyY = int.MinValue;
            int transitionType = 0, transitionTime = 0;
            bool notEffects = false;

            tmpArgVal = el.GetAttribute("rectangular");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                rectangular = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("width");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                width = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("height");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                height = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("hasInfluenceArea");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                hasInfluence = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("influenceX");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceX = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceY");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceY = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceWidth");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceWidth = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceHeight");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceHeight = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("destinyX");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                destinyX = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("destinyY");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                destinyY = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("transitionType");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                transitionType = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("transitionTime");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                transitionTime = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                notEffects = tmpArgVal.Equals("yes");
            }

            currentExit = new Exit(rectangular, x, y, width, height);
            currentExit.setNextSceneId(idTarget);
            currentExit.setDestinyX(destinyX);
            currentExit.setDestinyY(destinyY);
            currentExit.setTransitionTime(transitionTime);
            currentExit.setTransitionType(transitionType);
            currentExit.setHasNotEffects(notEffects);
            if (hasInfluence)
            {
                InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                currentExit.setInfluenceArea(influenceArea);
            }

            exitslook = el.SelectNodes("exit-look");
            foreach (XmlElement ell in exitslook)
            {
                currentExitLook = new ExitLook();
                string text = null;
                string cursorPath = null;
                string soundPath = null;

                tmpArgVal = ell.GetAttribute("text");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    text = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("cursor-path");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    cursorPath = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("sound-path");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    soundPath = tmpArgVal;
                }

                currentExitLook.setCursorPath(cursorPath);
                currentExitLook.setExitText(text);
                if (soundPath != null)
                {
                    currentExitLook.setSoundPath(soundPath);
                }
                currentExit.setDefaultExitLook(currentExitLook);
            }

            if (el.SelectSingleNode("documentation") != null)
                currentExit.setDocumentation(el.SelectSingleNode("documentation").InnerText);

            points = el.SelectNodes("point");
            foreach (XmlElement ell in points)
            {
                int x_ = 0;
                int y_ = 0;

                tmpArgVal = ell.GetAttribute("x");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    x_ = int.Parse(tmpArgVal);
                }
                tmpArgVal = ell.GetAttribute("y");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    y_ = int.Parse(tmpArgVal);
                }
                currentPoint = new Vector2(x_, y_);
                currentExit.addPoint(currentPoint);
            }

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentExit.setConditions(currentConditions);
            }


            effects = el.SelectNodes("effect");
            foreach (XmlElement ell in effects)
            {
                currentEffects = new Effects();
                new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                currentExit.setEffects(currentEffects);
            }

            notseffect = el.SelectNodes("not-effect");
            foreach (XmlElement ell in notseffect)
            {
                currentEffects = new Effects();
                new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                currentExit.setNotEffects(currentEffects);
            }

            postseffects = el.SelectNodes("post-effect");
            foreach (XmlElement ell in postseffects)
            {
                currentEffects = new Effects();
                new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                currentExit.setPostEffects(currentEffects);
            }


            if (currentExit.getNextScenes().Count > 0)
            {
                foreach (NextScene nextScene in currentExit.getNextScenes())
                {

                    Exit exit = (Exit) currentExit;
                    exit.setNextScenes(new List<NextScene>());
                    exit.setDestinyX(nextScene.getPositionX());
                    exit.setDestinyY(nextScene.getPositionY());
                    exit.setEffects(nextScene.getEffects());
                    exit.setPostEffects(nextScene.getPostEffects());
                    if (exit.getDefaultExitLook() == null)
                        exit.setDefaultExitLook(nextScene.getExitLook());
                    else
                    {
                        if (nextScene.getExitLook() != null)
                        {
                            if (nextScene.getExitLook().getExitText() != null &&
                                !nextScene.getExitLook().getExitText().Equals(""))
                                exit.getDefaultExitLook().setExitText(nextScene.getExitLook().getExitText());
                            if (nextScene.getExitLook().getCursorPath() != null &&
                                !nextScene.getExitLook().getCursorPath().Equals(""))
                                exit.getDefaultExitLook().setCursorPath(nextScene.getExitLook().getCursorPath());
                        }
                    }
                    exit.setHasNotEffects(false);
                    exit.setConditions(nextScene.getConditions());
                    exit.setNextSceneId(nextScene.getTargetId());
                    scene.addExit(exit);
                }
            }
            else
            {
                scene.addExit(currentExit);
            }
        }


        foreach (XmlElement el in nextsscene)
        {
            string idTarget = "";
            int x = int.MinValue, y = int.MinValue;
            int transitionType = 0, transitionTime = 0;


            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("transitionType");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                transitionType = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("transitionTime");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                transitionTime = int.Parse(tmpArgVal);
            }

            currentNextScene = new NextScene(idTarget, x, y);
            currentNextScene.setTransitionType((NextSceneEnumTransitionType) transitionType);
            currentNextScene.setTransitionTime(transitionTime);

            currentNextScene.setExitLook(currentExitLook);


            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentNextScene.setConditions(currentConditions);
            }

            effects = el.SelectNodes("effect");
            foreach (XmlElement ell in effects)
            {
                currentEffects = new Effects();
                new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                currentNextScene.setEffects(currentEffects);
            }
            postseffects = el.SelectNodes("post-effect");
            foreach (XmlElement ell in effects)
            {
                currentEffects = new Effects();
                new EffectSubParser_(currentEffects, chapter).ParseElement(ell);
                currentNextScene.setPostEffects(currentEffects);
            }

        }


        foreach (XmlElement el in objectsrefs)
        {
            string idTarget = "";
            int x = 0, y = 0;
            float scale = 0;
            int layer = 0;
            int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
            bool hasInfluence = false;

            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("scale");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                scale = float.Parse(tmpArgVal, CultureInfo.InvariantCulture);
            }
            tmpArgVal = el.GetAttribute("layer");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                layer = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("hasInfluenceArea");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                hasInfluence = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("layer");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                layer = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceX");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceX = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceY");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceY = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceWidth");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceWidth = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceHeight");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceHeight = int.Parse(tmpArgVal);
            }

            // This is for maintain the back-compatibility: in previous dtd versions layer has -1 as default value and this is
            // an erroneous value. This reason, if this value is -1, it will be changed to 0. Now in dtd there are not default value
            // for layer
            if (layer == -1)
                layer = 0;

            currentElementReference = new ElementReference(idTarget, x, y, layer);
            if (hasInfluence)
            {
                InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                currentElementReference.setInfluenceArea(influenceArea);
            }
            if (scale > 0.001 || scale < -0.001)
                currentElementReference.setScale(scale);

            if (el.SelectSingleNode("documentation") != null)
                currentElementReference.setDocumentation(el.SelectSingleNode("documentation").InnerText);

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentElementReference.setConditions(currentConditions);
            }

            scene.addItemReference(currentElementReference);
        }

        foreach (XmlElement el in charactersrefs)
        {
            string idTarget = "";
            int x = 0, y = 0;
            float scale = 0;
            int layer = 0;
            int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
            bool hasInfluence = false;

            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("scale");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                scale = float.Parse(tmpArgVal, CultureInfo.InvariantCulture);
            }
            tmpArgVal = el.GetAttribute("layer");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                layer = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("hasInfluenceArea");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                hasInfluence = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("layer");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                layer = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceX");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceX = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceY");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceY = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceWidth");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceWidth = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceHeight");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceHeight = int.Parse(tmpArgVal);
            }

            // This is for maintain the back-compatibility: in previous dtd versions layer has -1 as default value and this is
            // an erroneous value. This reason, if this value is -1, it will be changed to 0. Now in dtd there are not default value
            // for layer
            if (layer == -1)
                layer = 0;

            currentElementReference = new ElementReference(idTarget, x, y, layer);
            if (hasInfluence)
            {
                InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                currentElementReference.setInfluenceArea(influenceArea);
            }
            if (scale > 0.001 || scale < -0.001)
                currentElementReference.setScale(scale);

            if (el.SelectSingleNode("documentation") != null)
                currentElementReference.setDocumentation(el.SelectSingleNode("documentation").InnerText);

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentElementReference.setConditions(currentConditions);
            }

            scene.addCharacterReference(currentElementReference);
        }

        foreach (XmlElement el in atrezzosrefs)
        {
            string idTarget = "";
            int x = 0, y = 0;
            float scale = 0;
            int layer = 0;
            int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
            bool hasInfluence = false;

            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("x");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                x = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("y");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                y = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("scale");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                scale = float.Parse(tmpArgVal, CultureInfo.InvariantCulture);
            }
            tmpArgVal = el.GetAttribute("layer");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                layer = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("hasInfluenceArea");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                hasInfluence = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("layer");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                layer = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceX");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceX = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceY");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceY = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceWidth");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceWidth = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("influenceHeight");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                influenceHeight = int.Parse(tmpArgVal);
            }


            // This is for maintain the back-compatibility: in previous dtd versions layer has -1 as default value and this is
            // an erroneous value. This reason, if this value is -1, it will be changed to 0. Now in dtd there are not default value
            // for layer
            if (layer == -1)
                layer = 0;

            currentElementReference = new ElementReference(idTarget, x, y, layer);
            if (hasInfluence)
            {
                InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                currentElementReference.setInfluenceArea(influenceArea);
            }
            if (scale > 0.001 || scale < -0.001)
                currentElementReference.setScale(scale);

            if (el.SelectSingleNode("documentation") != null)
                currentElementReference.setDocumentation(el.SelectSingleNode("documentation").InnerText);

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentElementReference.setConditions(currentConditions);
            }

            scene.addAtrezzoReference(currentElementReference);
        }

        foreach (XmlElement el in activesareas)
        {
            new ActiveAreaSubParser_(chapter, scene, scene.getActiveAreas().Count).ParseElement(el);
        }

        foreach (XmlElement el in barriers)
        {
            new BarrierSubParser_(chapter, scene, scene.getBarriers().Count).ParseElement(el);
        }

        foreach (XmlElement el in trajectorys)
        {
            new TrajectorySubParser_(chapter, scene).ParseElement(el);
        }



        if (scene != null)
        {
            TrajectoryFixer.fixTrajectory(scene);
        }
        chapter.addScene(scene);
    }
}
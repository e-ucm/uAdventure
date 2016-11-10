using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;

/**
 * Class to subparse scenes
 */
public class SceneSubParser : SubParser
{


    /* Attributes */

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading resources tag
     */
    private const int READING_RESOURCES = 1;

    /**
     * Constant for reading exit tag
     */
    private const int READING_EXIT = 2;

    /**
     * Constant for reading next-scene tag
     */
    private const int READING_NEXT_SCENE = 3;

    /**
     * Constant for reading element reference tag
     */
    private const int READING_ELEMENT_REFERENCE = 4;

    /**
     * Constant for subparsing nothing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing condition tag
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Constant for subparsing effect tag
     */
    private const int SUBPARSING_EFFECT = 2;

    /**
     * Constant for subparsing active area
     */
    private const int SUBPARSING_ACTIVE_AREA = 3;

    /**
     * Constant for subparsing active area
     */
    private const int SUBPARSING_BARRIER = 4;

    private const int SUBPARSING_TRAJECTORY = 5;

    /**
     * Stores the current element being parsed
     */
    private int reading = READING_NONE;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

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

    /**
     * The subparser for the condition or effect tags
     */
    private SubParser subParser;

    /* Methods */

    /**
     * Constructor
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public SceneSubParser(Chapter chapter) : base(chapter)
    {
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.cargador.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {
        Debug.Log("START: " + sName + " " + qName + " sub:" + subParsing + ", reading: " + reading);
        // If no element is being parsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a scene tag, create a new scene with its id
            if (qName.Equals("scene"))
            {
                string sceneId = "";
                bool initialScene = false;
                int playerLayer = -1;
                float playerScale = 1.0f;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        sceneId = entry.Value.ToString();
                    if (entry.Key.Equals("start"))
                        initialScene = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("playerLayer"))
                        playerLayer = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("playerScale"))
                        playerScale = float.Parse(entry.Value.ToString(), CultureInfo.InvariantCulture);
                }

                scene = new Scene(sceneId);
                scene.setPlayerLayer(playerLayer);
                scene.setPlayerScale(playerScale);
                if (initialScene)
                    chapter.setTargetId(sceneId);
            }

            // If it is a resources tag, create the new resources
            else if (qName.Equals("resources"))
            {
                currentResources = new ResourcesUni();

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("name"))
                        currentResources.setName(entry.Value.ToString());
                }

                reading = READING_RESOURCES;
            }

            // If it is an asset tag, read it and add it to the current resources
            else if (qName.Equals("asset"))
            {
                string type = "";
                string path = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("type"))
                        type = entry.Value.ToString();
                    if (entry.Key.Equals("uri"))
                        path = entry.Value.ToString();
                }

                currentResources.addAsset(type, path);
            }

            // If it is a default-initial-position tag, store it in the scene
            else if (qName.Equals("default-initial-position"))
            {
                int x = int.MinValue, y = int.MinValue;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                }

                scene.setDefaultPosition(x, y);
            }

            // If it is an exit tag, create the new exit
            else if (qName.Equals("exit"))
            {
                int x = 0, y = 0, width = 0, height = 0;
                bool rectangular = true;
                int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
                bool hasInfluence = false;
                string idTarget = "";
                int destinyX = int.MinValue, destinyY = int.MinValue;
                int transitionType = 0, transitionTime = 0;
                bool notEffects = false;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("rectangular"))
                        rectangular = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("width"))
                        width = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("height"))
                        height = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("hasInfluenceArea"))
                        hasInfluence = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("influenceX"))
                        influenceX = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceY"))
                        influenceY = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceWidth"))
                        influenceWidth = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceHeight"))
                        influenceHeight = int.Parse(entry.Value.ToString());

                    if (entry.Key.Equals("idTarget"))
                        idTarget = entry.Value.ToString();
                    if (entry.Key.Equals("destinyX"))
                        destinyX = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("destinyY"))
                        destinyY = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("transitionType"))
                        transitionType = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("transitionTime"))
                        transitionTime = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("not-effects"))
                        notEffects = entry.Value.ToString().Equals("yes");
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
                reading = READING_EXIT;
            }

            else if (qName.Equals("exit-look"))
            {
                currentExitLook = new ExitLook();
                string text = null;
                string cursorPath = null;
                string soundPath = null;
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("text"))
                    {
                        text = entry.Value.ToString();
                    }
                    if (entry.Key.Equals("cursor-path"))
                        cursorPath = entry.Value.ToString();
                    if (entry.Key.Equals("sound-path"))
                        soundPath = entry.Value.ToString();
                }
                currentExitLook.setCursorPath(cursorPath);
                currentExitLook.setExitText(text);
                if (soundPath != null)
                {
                    currentExitLook.setSoundPath(soundPath);
                }
                //  Debug.Log("311" + currentExitLook.getExitText());
            }

            // If it is a next-scene tag, create the new next scene
            else if (qName.Equals("next-scene"))
            {
                string idTarget = "";
                int x = int.MinValue, y = int.MinValue;
                int transitionType = 0, transitionTime = 0;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("idTarget"))
                        idTarget = entry.Value.ToString();
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("transitionType"))
                        transitionType = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("transitionTime"))
                        transitionTime = int.Parse(entry.Value.ToString());
                }

                currentNextScene = new NextScene(idTarget, x, y);
                currentNextScene.setTransitionType((NextSceneEnumTransitionType)transitionType);
                currentNextScene.setTransitionTime(transitionTime);
                reading = READING_NEXT_SCENE;
            }

            else if (qName.Equals("point"))
            {

                int x = 0;
                int y = 0;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                }

                currentPoint = new Vector2(x, y);
            }

            // If it is a object-ref or character-ref, create the new element reference
            else if (qName.Equals("object-ref") || qName.Equals("character-ref") || qName.Equals("atrezzo-ref"))
            {
                Debug.Log("SceneReference Start");
                string idTarget = "";
                int x = 0, y = 0;
                float scale = 0;
                int layer = 0;
                int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
                bool hasInfluence = false;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("idTarget"))
                        idTarget = entry.Value.ToString();
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("scale"))
                        scale = float.Parse(entry.Value.ToString(), CultureInfo.InvariantCulture);
                    if (entry.Key.Equals("layer"))
                        layer = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("hasInfluenceArea"))
                        hasInfluence = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("influenceX"))
                        influenceX = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceY"))
                        influenceY = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceWidth"))
                        influenceWidth = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceHeight"))
                        influenceHeight = int.Parse(entry.Value.ToString());
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
                reading = READING_ELEMENT_REFERENCE;
            }

            // If it is a condition tag, create the new condition, the subparser and switch the state
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }

            // If it is a effect tag, create the new effect, the subparser and switch the state
            else if (qName.Equals("effect"))
            {
                currentEffects = new Effects();
                subParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }

            // If it is a post-effect tag, create the new effect, the subparser and switch the state
            else if (qName.Equals("post-effect"))
            {
                currentEffects = new Effects();
                subParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }

            // If it is a post-effect tag, create the new effect, the subparser and switch the state
            else if (qName.Equals("not-effect"))
            {
                currentEffects = new Effects();
                subParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }

            // If it is a post-effect tag, create the new effect, the subparser and switch the state
            else if (qName.Equals("active-area"))
            {
                subParsing = SUBPARSING_ACTIVE_AREA;
                subParser = new ActiveAreaSubParser(chapter, scene, scene.getActiveAreas().Count);
            }

            // If it is a post-effect tag, create the new effect, the subparser and switch the state
            else if (qName.Equals("barrier"))
            {
                subParsing = SUBPARSING_BARRIER;
                subParser = new BarrierSubParser(chapter, scene, scene.getBarriers().Count);
            }

            else if (qName.Equals("trajectory"))
            {
                subParsing = SUBPARSING_TRAJECTORY;
                subParser = new TrajectorySubParser(chapter, scene);
            }

        }

        // If it is subparsing an effect or condition, spread the call
        if (subParsing != SUBPARSING_NONE)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.cargador.subparsers.SubParser#endElement(java.lang.string, java.lang.string,
     *      java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {
        Debug.Log("END: " + sName + " " + qName + " sub:" + subParsing + ", reading: " + reading);
        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a scene tag, store the scene in the game data
            if (qName.Equals("scene"))
            {
                if (scene != null)
                {
                    TrajectoryFixer.fixTrajectory(scene);
                }
                chapter.addScene(scene);
            }

            // If it is a resources tag, add the resources to the scene
            else if (qName.Equals("resources"))
            {
                scene.addResources(currentResources);
                reading = READING_NONE;
            }

            // If it is a name tag, store the name in the scene
            else if (qName.Equals("name"))
            {
                scene.setName(currentstring.ToString().Trim());
            }

            // If it is a documentation tag, hold the documentation in the current element
            else if (qName.Equals("documentation"))
            {
                if (reading == READING_NONE)
                    scene.setDocumentation(currentstring.ToString().Trim());
                else if (reading == READING_EXIT)
                    currentExit.setDocumentation(currentstring.ToString().Trim());
                else if (reading == READING_ELEMENT_REFERENCE)
                    currentElementReference.setDocumentation(currentstring.ToString().Trim());
            }

            // If it is an exit tag, store the exit in the scene
            else if (qName.Equals("exit"))
            {
                if (currentExit.getNextScenes().Count > 0)
                {
                    foreach (NextScene nextScene in currentExit.getNextScenes())
                    {

                        Exit exit = (Exit)currentExit;
                        exit.setNextScenes(new List<NextScene>());
                        exit.setDestinyX(nextScene.getPositionX());
                        exit.setDestinyY(nextScene.getPositionY());
                        exit.setEffects(nextScene.getEffects());
                        exit.setPostEffects(nextScene.getPostEffects());
                        if (exit.getDefaultExitLook() == null)
                            exit.setDefaultExitLook(nextScene.getExitLook());
                        else {
                            if (nextScene.getExitLook() != null)
                            {
                                if (nextScene.getExitLook().getExitText() != null && !nextScene.getExitLook().getExitText().Equals(""))
                                    exit.getDefaultExitLook().setExitText(nextScene.getExitLook().getExitText());
                                if (nextScene.getExitLook().getCursorPath() != null && !nextScene.getExitLook().getCursorPath().Equals(""))
                                    exit.getDefaultExitLook().setCursorPath(nextScene.getExitLook().getCursorPath());
                            }
                        }
                        exit.setHasNotEffects(false);
                        exit.setConditions(nextScene.getConditions());
                        exit.setNextSceneId(nextScene.getTargetId());
                        scene.addExit(exit);

                    }
                }
                else {
                    scene.addExit(currentExit);
                }
                //scene.addExit( currentExit );
                reading = READING_NONE;
            }

            // If it is an exit look tag, store the look in the exit
            else if (qName.Equals("exit-look"))
            {
                Debug.Log("559" + currentExitLook.getExitText());
                if (reading == READING_NEXT_SCENE)
                    currentNextScene.setExitLook(currentExitLook);
                else if (reading == READING_EXIT)
                {
                    currentExit.setDefaultExitLook(currentExitLook);
                }
            }

            // If it is a next-scene tag, store the next scene in the current exit
            else if (qName.Equals("next-scene"))
            {
                currentExit.addNextScene(currentNextScene);
                reading = READING_NONE;
            }

            else if (qName.Equals("point"))
            {
                currentExit.addPoint(currentPoint);
            }

            // If it is a object-ref tag, store the reference in the scene
            else if (qName.Equals("object-ref"))
            {
                Debug.Log("SceneReference End");
                scene.addItemReference(currentElementReference);
                reading = READING_NONE;
            }

            // If it is a character-ref tag, store the reference in the scene
            else if (qName.Equals("character-ref"))
            {
                Debug.Log("CharRef End");
                scene.addCharacterReference(currentElementReference);
                reading = READING_NONE;
            }
            // If it is a atrezzo-ref tag, store the reference in the scene
            else if (qName.Equals("atrezzo-ref"))
            {
                scene.addAtrezzoReference(currentElementReference);
                reading = READING_NONE;
            }

            // Reset the current string
            currentstring = string.Empty;
        }

        // If a condition is being subparsed
        else if (subParsing == SUBPARSING_CONDITION)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the condition tag is being closed
            if (qName.Equals("condition"))
            {

                // If we are parsing resources, add the condition to the current resources
                if (reading == READING_RESOURCES)
                    currentResources.setConditions(currentConditions);

                // If we are parsing a next-scene, add the condition to the current next scene
                if (reading == READING_NEXT_SCENE)
                    currentNextScene.setConditions(currentConditions);

                // If we are parsing an element reference, add the condition to the current element reference
                if (reading == READING_ELEMENT_REFERENCE)
                    currentElementReference.setConditions(currentConditions);

                if (reading == READING_EXIT)
                    currentExit.setConditions(currentConditions);

                // Switch the state
                subParsing = SUBPARSING_NONE;

            }
        }

        // If an effect is being subparsed
        else if (subParsing == SUBPARSING_EFFECT)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the effect tag is being closed, add the effects to the current next scene and switch the state
            if (qName.Equals("effect"))
            {
                if (reading == READING_NEXT_SCENE)
                    currentNextScene.setEffects(currentEffects);
                if (reading == READING_EXIT)
                    currentExit.setEffects(currentEffects);
                subParsing = SUBPARSING_NONE;
            }

            // If the effect tag is being closed, add the post-effects to the current next scene and switch the state
            if (qName.Equals("post-effect"))
            {
                if (reading == READING_NEXT_SCENE)
                    currentNextScene.setPostEffects(currentEffects);
                if (reading == READING_EXIT)
                    currentExit.setPostEffects(currentEffects);
                subParsing = SUBPARSING_NONE;
            }

            if (qName.Equals("not-effect"))
            {
                currentExit.setNotEffects(currentEffects);
                subParsing = SUBPARSING_NONE;
            }
        }

        // If an active area is being subparsed
        else if (subParsing == SUBPARSING_ACTIVE_AREA)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            if (qName.Equals("active-area"))
            {
                subParsing = SUBPARSING_NONE;
            }
        }

        // If a barrier is being subparsed
        else if (subParsing == SUBPARSING_BARRIER)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            if (qName.Equals("barrier"))
            {
                subParsing = SUBPARSING_NONE;
            }
        }

        else if (subParsing == SUBPARSING_TRAJECTORY)
        {
            subParser.endElement(namespaceURI, sName, qName);
            if (qName.Equals("trajectory"))
            {
                subParsing = SUBPARSING_NONE;
                // next line is moved to TrayectorySubParser
                //scene.getTrajectory().deleteUnconnectedNodes();
            }
        }

    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If it is reading an effect or a condition
        else
            subParser.characters(buf, offset, len);
    }
}
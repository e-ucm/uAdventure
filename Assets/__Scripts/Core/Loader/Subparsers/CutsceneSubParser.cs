using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse slidescenes
 */
public class CutsceneSubParser : SubParser
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
     * Constant for reading next-scene tag
     */
    private const int READING_NEXT_SCENE = 2;

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
     * Stores the current element being parsed
     */
    private int reading = READING_NONE;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

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
    public CutsceneSubParser(Chapter chapter):base(chapter)
    {
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a slidescene tag, create a new slidescene with its id
            if (qName.Equals("slidescene") || qName.Equals("videoscene"))
            {
                string slidesceneId = "";
                bool initialScene = false;
                string idTarget = "";
                int x = int.MinValue, y = int.MinValue;
                int transitionType = 0, transitionTime = 0;
                string next = "go-back";
                bool canSkip = true;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        slidesceneId = entry.Value.ToString();
                    if (entry.Key.Equals("start"))
                        initialScene = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("idTarget"))
                        idTarget = entry.Value.ToString();
                    if (entry.Key.Equals("destinyX"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("destinyY"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("transitionType"))
                        transitionType = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("transitionTime"))
                        transitionTime = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("next"))
                        next = entry.Value.ToString();
                    if (entry.Key.Equals("canSkip"))
                        canSkip = entry.Value.ToString().Equals("yes");
                }

                if (qName.Equals("slidescene"))
                    cutscene = new Slidescene(slidesceneId);
                else
                    cutscene = new Videoscene(slidesceneId);
                if (initialScene)
                    chapter.setTargetId(slidesceneId);

                cutscene.setTargetId(idTarget);
                cutscene.setPositionX(x);
                cutscene.setPositionY(y);
                cutscene.setTransitionType((NextSceneEnumTransitionType) transitionType);
                cutscene.setTransitionTime(transitionTime);
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
            }

            // If it is a resources tag, create new resources
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

            // If it is an end-game tag, store it in the slidescene
            else if (qName.Equals("end-game"))
            {
                cutscene.setNext(Cutscene.ENDCHAPTER);
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
        }

        // If it is reading an effect or a condition, spread the call
        if (subParsing != SUBPARSING_NONE)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#endElement(java.lang.string, java.lang.string,
     *      java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a slidescene tag, add it to the game data
            if (qName.Equals("slidescene") || qName.Equals("videoscene"))
            {
                chapter.addCutscene(cutscene);
            }

            // If it is a resources tag, add it to the slidescene
            else if (qName.Equals("resources"))
            {
                cutscene.addResources(currentResources);
                reading = READING_NONE;
            }

            // If it is a name tag, add the name to the slidescene
            else if (qName.Equals("name"))
            {
                cutscene.setName(currentstring.ToString().Trim());
            }

            // If it is a documentation tag, hold the documentation in the slidescene
            else if (qName.Equals("documentation"))
            {
                cutscene.setDocumentation(currentstring.ToString().Trim());
            }

            // If it is a next-scene tag, add the next scene to the slidescene
            else if (qName.Equals("next-scene"))
            {
                cutscene.addNextScene(currentNextScene);
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
                // If we are parsing a resources tag, add the conditions to the current resources
                if (reading == READING_RESOURCES)
                    currentResources.setConditions(currentConditions);

                // If we are parsing a next-scene tag, add the conditions to the current next scene
                if (reading == READING_NEXT_SCENE)
                    currentNextScene.setConditions(currentConditions);

                // Switch the state
                subParsing = SUBPARSING_NONE;
            }
        }

        // If an effect is being subparsed
        else if (subParsing == SUBPARSING_EFFECT)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the effect tag is being closed, store the effect in the next scene and switch the state
            if (qName.Equals("effect"))
            {
                if (currentNextScene != null)
                    currentNextScene.setEffects(currentEffects);
                else {
                    Effects effects = cutscene.getEffects();
                    foreach (AbstractEffect effect in currentEffects.getEffects())
                    {
                        effects.add(effect);
                    }
                }
                subParsing = SUBPARSING_NONE;
            }

            // If the effect tag is being closed, add the post-effects to the current next scene and switch the state
            if (qName.Equals("post-effect"))
            {
                if (currentNextScene != null)
                    currentNextScene.setPostEffects(currentEffects);
                else {
                    Effects effects = cutscene.getEffects();
                    foreach (AbstractEffect effect in currentEffects.getEffects())
                    {
                        effects.add(effect);
                    }
                }
                //currentNextScene.setPostEffects( currentEffects );

                subParsing = SUBPARSING_NONE;
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

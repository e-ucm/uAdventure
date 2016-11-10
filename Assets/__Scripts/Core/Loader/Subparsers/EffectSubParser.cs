using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

/**
 * Class to subparse effects
 */
public class EffectSubParser : SubParser
{

    /* Constants */
    /**
     * Constant for no subparsing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing conditions
     */
    private const int SUBPARSING_CONDITION = 1;

    /* Attributes */

    /**
     * The current subparser being used
     */
    private SubParser subParser;

    /**
     * Indicates the current element being subparsed
     */
    private int subParsing;

    /**
     * Stores the current id target
     */
    private string currentCharIdTarget;

    /**
     * Stores the effects being parsed
     */
    private Effects effects;

    /**
     * Atributes for show-text effects
     */

    int x = 0;

    int y = 0;

    int frontColor = 0;

    int borderColor = 0;

    /**
     * Constants for reading random-effect
     */
    private bool positiveBlockRead = false;

    private bool readingRandomEffect = false;

    private RandomEffect randomEffect;

    /**
     * Stores the current conditions being read
     */
    private Conditions currentConditions;

    /**
     * CurrentEffect. Stores the last created effect to add it later the
     * conditions
     */
    private AbstractEffect currentEffect;

    /**
     * New effects
     */
    private AbstractEffect newEffect;

    /**
     * Audio path for speak player and character
     */
    private string audioPath;

    /* Methods */

    /**
     * Constructor
     * 
     * @param effects
     *            Structure in which the effects will be placed
     * @param chapter
     *            Chapter data to store the read data
     */
    public EffectSubParser(Effects effects, Chapter chapter) : base(chapter)
    {
        this.effects = effects;
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {
        //Debug.Log("Start: " + sName + " " + qName + "\nAttr: \n " +CollectionPrinter.PrintCollection(attrs));
        newEffect = null;
        audioPath = string.Empty;

        //Debug.Log(sName + " " + qName + "\nAttrs: \n" + CollectionPrinter.PrintCollection(attrs));
        // If it is a cancel-action tag
        if (qName.Equals("cancel-action"))
        {
            newEffect = new CancelActionEffect();
        }

        // If it is a activate tag
        else if (qName.Equals("activate"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("flag"))
                {
                    newEffect = new ActivateEffect(entry.Value.ToString());
                    chapter.addFlag(entry.Value.ToString());
                }
        }

        // If it is a deactivate tag
        else if (qName.Equals("deactivate"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("flag"))
                {
                    newEffect = new DeactivateEffect(entry.Value.ToString());
                    chapter.addFlag(entry.Value.ToString());
                }
        }

        // If it is a set-value tag
        else if (qName.Equals("set-value"))
        {
            string var = null;
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {

                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            newEffect = new SetValueEffect(var, value);
            chapter.addVar(var);
        }

        // If it is a set-value tag
        else if (qName.Equals("increment"))
        {
            string var = null;
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {

                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            newEffect = new IncrementVarEffect(var, value);
            chapter.addVar(var);
        }

        // If it is a decrement tag
        else if (qName.Equals("decrement"))
        {
            string var = null;
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {

                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            newEffect = new DecrementVarEffect(var, value);
            chapter.addVar(var);
        }

        // If it is a macro-reference tag
        else if (qName.Equals("macro-ref"))
        {
            // Id
            string id = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                {
                    id = entry.Value.ToString();
                }
            }
            // Store the inactive flag in the conditions or either conditions
            newEffect = new MacroReferenceEffect(id);
        }

        // If it is a consume-object tag
        else if (qName.Equals("consume-object"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("idTarget"))
                    newEffect = new ConsumeObjectEffect(entry.Value.ToString());
        }

        // If it is a generate-object tag
        else if (qName.Equals("generate-object"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("idTarget"))
                    newEffect = new GenerateObjectEffect(entry.Value.ToString());
        }

        // If it is a speak-char tag
        else if (qName.Equals("speak-char"))
        {

            audioPath = "";
            // Store the idTarget, to store the effect when the tag is closed
            currentCharIdTarget = null;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("idTarget"))
                    currentCharIdTarget = entry.Value.ToString();
                // If there is a "uri" attribute, store it as audio path
                if (entry.Key.Equals("uri"))
                    audioPath = entry.Value.ToString();
            }

        }

        // If it is a trigger-book tag
        else if (qName.Equals("trigger-book"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("idTarget"))
                    newEffect = new TriggerBookEffect(entry.Value.ToString());
        }

        // If it is a trigger-last-scene tag
        else if (qName.Equals("trigger-last-scene"))
        {
            newEffect = new TriggerLastSceneEffect();
        }

        // If it is a play-sound tag
        else if (qName.Equals("play-sound"))
        {
            // Store the path and background
            string path = "";
            bool background = true;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("background"))
                    background = entry.Value.ToString().Equals("yes");
                else if (entry.Key.Equals("uri"))
                    path = entry.Value.ToString();
            }

            // Add the new play sound effect
            newEffect = new PlaySoundEffect(background, path);
        }

        // If it is a trigger-conversation tag
        else if (qName.Equals("trigger-conversation"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("idTarget"))
                    newEffect = new TriggerConversationEffect(entry.Value.ToString());
        }

        // If it is a trigger-cutscene tag
        else if (qName.Equals("trigger-cutscene"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("idTarget"))
                    newEffect = new TriggerCutsceneEffect(entry.Value.ToString());
        }

        // If it is a trigger-scene tag
        else if (qName.Equals("trigger-scene"))
        {
            string scene = "";
            int x = 0;
            int y = 0;
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("idTarget"))
                    scene = entry.Value.ToString();
                else if (entry.Key.Equals("x"))
                    x = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("y"))
                    y = int.Parse(entry.Value.ToString());

            newEffect = new TriggerSceneEffect(scene, x, y);
        }

        // If it is a play-animation tag
        else if (qName.Equals("play-animation"))
        {
            string path = "";
            int x = 0;
            int y = 0;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("uri"))
                    path = entry.Value.ToString();
                else if (entry.Key.Equals("x"))
                    x = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("y"))
                    y = int.Parse(entry.Value.ToString());
            }

            // Add the new play sound effect
            newEffect = new PlayAnimationEffect(path, x, y);
        }

        // If it is a move-player tag
        else if (qName.Equals("move-player"))
        {
            int x = 0;
            int y = 0;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("x"))
                    x = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("y"))
                    y = int.Parse(entry.Value.ToString());
            }

            // Add the new move player effect
            newEffect = new MovePlayerEffect(x, y);
        }

        // If it is a move-npc tag
        else if (qName.Equals("move-npc"))
        {
            string npcTarget = "";
            int x = 0;
            int y = 0;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("idTarget"))
                    npcTarget = entry.Value.ToString();
                else if (entry.Key.Equals("x"))
                    x = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("y"))
                    y = int.Parse(entry.Value.ToString());
            }

            // Add the new move NPC effect
            newEffect = new MoveNPCEffect(npcTarget, x, y);
        }

        // Random effect tag
        else if (qName.Equals("random-effect"))
        {
            int probability = 0;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("probability"))
                    probability = int.Parse(entry.Value.ToString());
            }

            // Add the new random effect
            randomEffect = new RandomEffect(probability);
            newEffect = randomEffect;
            readingRandomEffect = true;
            positiveBlockRead = false;
        }
        // wait-time effect
        else if (qName.Equals("wait-time"))
        {
            int time = 0;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("time"))
                    time = int.Parse(entry.Value.ToString());
            }

            // Add the new move NPC effect
            newEffect = new WaitTimeEffect(time);
        }

        // show-text effect
        else if (qName.Equals("show-text"))
        {
            x = 0;
            y = 0;
            frontColor = 0;
            borderColor = 0;
            audioPath = "";

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("x"))
                    x = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("y"))
                    y = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("frontColor"))
                    frontColor = int.Parse(entry.Value.ToString());
                else if (entry.Key.Equals("borderColor"))
                    borderColor = int.Parse(entry.Value.ToString());
                // If there is a "uri" attribute, store it as audio path
                else if (entry.Key.Equals("uri"))
                    audioPath = entry.Value.ToString();
            }

        }

        else if (qName.Equals("highlight-item"))
        {
            int type = 0;
            bool animated = false;
            string id = "";
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("idTarget"))
                    id = entry.Value.ToString();
                if (entry.Key.Equals("animated"))
                    animated = (entry.Value.ToString().Equals("yes") ? true : false);
                if (entry.Key.Equals("type"))
                {
                    if (entry.Value.ToString().Equals("none"))
                        type = HighlightItemEffect.NO_HIGHLIGHT;
                    if (entry.Value.ToString().Equals("green"))
                        type = HighlightItemEffect.HIGHLIGHT_GREEN;
                    if (entry.Value.ToString().Equals("red"))
                        type = HighlightItemEffect.HIGHLIGHT_RED;
                    if (entry.Value.ToString().Equals("blue"))
                        type = HighlightItemEffect.HIGHLIGHT_BLUE;
                    if (entry.Value.ToString().Equals("border"))
                        type = HighlightItemEffect.HIGHLIGHT_BORDER;
                }
            }
            newEffect = new HighlightItemEffect(id, type, animated);
        }
        else if (qName.Equals("move-object"))
        {
            bool animated = false;
            string id = "";
            int x = 0;
            int y = 0;
            float scale = 1.0f;
            int translateSpeed = 20;
            int scaleSpeed = 20;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("idTarget"))
                    id = entry.Value.ToString();
                if (entry.Key.Equals("animated"))
                    animated = (entry.Value.ToString().Equals("yes") ? true : false);
                if (entry.Key.Equals("x"))
                    x = int.Parse(entry.Value.ToString());
                if (entry.Key.Equals("y"))
                    y = int.Parse(entry.Value.ToString());
                if (entry.Key.Equals("scale"))
                    scale = float.Parse(entry.Value.ToString(), CultureInfo.InvariantCulture);
                if (entry.Key.Equals("translateSpeed"))
                    translateSpeed = int.Parse(entry.Value.ToString());
                if (entry.Key.Equals("scaleSpeed"))
                    scaleSpeed = int.Parse(entry.Value.ToString());
            }
            newEffect = new MoveObjectEffect(id, x, y, scale, animated, translateSpeed, scaleSpeed);
        }


        else if (qName.Equals("speak-player"))
        {
            audioPath = "";

            foreach (KeyValuePair<string, string> entry in attrs)
            {

                // If there is a "uri" attribute, store it as audio path
                if (entry.Key.Equals("uri"))
                    audioPath = entry.Value.ToString();
            }
        }
        // If it is a condition tag, create new conditions and switch the state
        else if (qName.Equals("condition"))
        {
            currentConditions = new Conditions();
            subParser = new ConditionSubParser(currentConditions, chapter);
            subParsing = SUBPARSING_CONDITION;
        }

        // Not reading Random effect: Add the new Effect if not null
        if (!readingRandomEffect && newEffect != null)
        {
            effects.add(newEffect);
            // Store current effect
            currentEffect = newEffect;

        }

        // Reading random effect
        if (readingRandomEffect)
        {
            // When we have just created the effect, add it
            if (newEffect != null && newEffect == randomEffect)
            {
                effects.add(newEffect);
            }
            // Otherwise, determine if it is positive or negative effect 
            else if (newEffect != null && !positiveBlockRead)
            {
                randomEffect.setPositiveEffect(newEffect);
                positiveBlockRead = true;
            }
            // Negative effect 
            else if (newEffect != null && positiveBlockRead)
            {
                randomEffect.setNegativeEffect(newEffect);
                positiveBlockRead = false;
                readingRandomEffect = false;
                newEffect = randomEffect;
                randomEffect = null;

            }
            // Store current effect
            currentEffect = newEffect;

        }

        // If it is reading an effect or a condition, spread the call
        if (subParsing != SUBPARSING_NONE)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
            endElement(namespaceURI, sName, qName);
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

        // Debug.Log("END: " + sName + " " + qName );
        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {
            newEffect = null;

            // If it is a speak-player
            if (qName.Equals("speak-player"))
            {
                // Add the effect and clear the current string
                newEffect = new SpeakPlayerEffect(currentstring.ToString().Trim());
                ((SpeakPlayerEffect)newEffect).setAudioPath(audioPath);
            }

            // If it is a speak-char
            else if (qName.Equals("speak-char"))
            {
                // Add the effect and clear the current string
                newEffect = new SpeakCharEffect(currentCharIdTarget, currentstring.ToString().Trim());
                ((SpeakCharEffect)newEffect).setAudioPath(audioPath);
            }// If it is a show-text
            else if (qName.Equals("show-text"))
            {
                // Add the new ShowTextEffect
                newEffect = new ShowTextEffect(currentstring.ToString().Trim(), x, y, frontColor.ToString(), borderColor.ToString());
                ((ShowTextEffect)newEffect).setAudioPath(audioPath);
            }

            // Not reading Random effect: Add the new Effect if not null
            if (!readingRandomEffect && newEffect != null)
            {
                effects.add(newEffect);
                // Store current effect
                currentEffect = newEffect;

            }

            // Reading random effect
            if (readingRandomEffect)
            {
                // When we have just created the effect, add it
                if (newEffect != null && newEffect == randomEffect)
                {
                    effects.add(newEffect);
                }
                // Otherwise, determine if it is positive or negative effect 
                else if (newEffect != null && !positiveBlockRead)
                {
                    randomEffect.setPositiveEffect(newEffect);
                    positiveBlockRead = true;
                }
                // Negative effect 
                else if (newEffect != null && positiveBlockRead)
                {
                    randomEffect.setNegativeEffect(newEffect);
                    positiveBlockRead = false;
                    readingRandomEffect = false;
                    newEffect = randomEffect;
                    randomEffect = null;
                }
                // Store current effect
                currentEffect = newEffect;

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
                //Debug.Log(currentEffect);
                //Debug.Log(currentConditions);
                // Store the conditions in the effect
                currentEffect.setConditions(currentConditions);

                // Switch state
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

        // If it is reading an effect or a condition, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}

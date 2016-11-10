using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

public class EffectSubParser_ : Subparser_
{
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

    public EffectSubParser_(Effects effects, Chapter chapter) : base(chapter)
    {
        this.effects = effects;

    }

    public override void ParseElement(XmlElement element)
    {

        string tmpArgVal;
        int x = 0;
        int y = 0;
        string path = "";
        string id = "";
        bool animated = false, addeffect = true;
        List<AbstractEffect> effectlist;

        foreach (XmlElement effect in element.ChildNodes)
        {
            addeffect = true;

            switch (effect.Name)
            {
                case "cancel-action": currentEffect = new CancelActionEffect(); break;
                case "activate":
                case "deactivate":
                    tmpArgVal = effect.GetAttribute("flag");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        chapter.addFlag(tmpArgVal);
                    }

                    if (effect.Name == "activate")
                        currentEffect = new ActivateEffect(tmpArgVal);
                    else
                        currentEffect = new DeactivateEffect(tmpArgVal);
                    break;
                case "set-value":
                case "increment":
                case "decrement":
                    string var = null;
                    int value = 0;

                    tmpArgVal = effect.GetAttribute("var");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        var = tmpArgVal;
                    }
                    tmpArgVal = effect.GetAttribute("value");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        value = int.Parse(tmpArgVal);
                    }
                    if (effect.Name == "set-value")
                        currentEffect = new SetValueEffect(var, value);
                    else if (effect.Name == "increment")
                        currentEffect = new IncrementVarEffect(var, value);
                    else
                        currentEffect = new DecrementVarEffect(var, value);

                    chapter.addVar(var);
                    break;
                case "macro-ref":
                    id = "";
                    tmpArgVal = effect.GetAttribute("id");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        id = tmpArgVal;
                    }
                    currentEffect = new MacroReferenceEffect(id);
                    break;
                case "speak-char":
                    audioPath = "";
                    currentCharIdTarget = null;

                    tmpArgVal = effect.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        currentCharIdTarget = tmpArgVal;
                    }

                    tmpArgVal = effect.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        audioPath = tmpArgVal;
                    }

                    // Add the effect and clear the current string
                    currentEffect = new SpeakCharEffect(currentCharIdTarget, effect.InnerText.ToString().Trim());
                    ((SpeakCharEffect)currentEffect).setAudioPath(audioPath);
                    break;
                case "trigger-last-scene":
                    currentEffect = new TriggerLastSceneEffect();
                    break;
                case "play-sound":
                    // Store the path and background
                    bool background = true;
                    path = "";

                    tmpArgVal = effect.GetAttribute("background");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        background = tmpArgVal.Equals("yes");
                    }
                    tmpArgVal = effect.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        path = tmpArgVal;
                    }

                    // Add the new play sound effect
                    currentEffect = new PlaySoundEffect(background, path);
                    break;

                case "consume-object":
                case "generate-object":
                case "trigger-book":
                case "trigger-conversation":
                case "trigger-cutscene":
                    tmpArgVal = effect.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                        switch (effect.Name)
                        {
                            case "consume-object": currentEffect = new ConsumeObjectEffect(tmpArgVal); break;
                            case "generate-object": currentEffect = new GenerateObjectEffect(tmpArgVal); break;
                            case "trigger-book": currentEffect = new TriggerBookEffect(tmpArgVal); break;
                            case "trigger-conversation": currentEffect = new TriggerConversationEffect(tmpArgVal); break;
                            case "trigger-cutscene": currentEffect = new TriggerCutsceneEffect(tmpArgVal); break;
                        }
                    break;
                case "trigger-scene":
                    x = 0;
                    y = 0;
                    string scene = "";
                    tmpArgVal = effect.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        scene = tmpArgVal;
                    }
                    tmpArgVal = effect.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        x = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        y = int.Parse(tmpArgVal);
                    }

                    currentEffect = new TriggerSceneEffect(scene, x, y);
                    break;
                case "play-animation":
                    x = 0;
                    y = 0;
                    path = "";

                    tmpArgVal = effect.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        path = tmpArgVal;
                    }
                    tmpArgVal = effect.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        x = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        y = int.Parse(tmpArgVal);
                    }

                    // Add the new play sound effect
                    currentEffect = new PlayAnimationEffect(path, x, y);
                    break;
                case "move-player":
                    x = 0;
                    y = 0;
                    tmpArgVal = effect.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        x = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        y = int.Parse(tmpArgVal);
                    }

                    // Add the new move player effect
                    currentEffect = new MovePlayerEffect(x, y);
                    break;
                case "move-npc":
                    x = 0;
                    y = 0;
                    string npcTarget = "";
                    tmpArgVal = effect.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        npcTarget = tmpArgVal;
                    }
                    tmpArgVal = effect.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        x = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        y = int.Parse(tmpArgVal);
                    }

                    // Add the new move NPC effect
                    currentEffect = new MoveNPCEffect(npcTarget, x, y);
                    break;
                case "random-effect":
                    int probability = 0;

                    tmpArgVal = effect.GetAttribute("probability");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        probability = int.Parse(tmpArgVal);
                    }

                    // Add the new random effect
                    randomEffect = new RandomEffect(probability);

                    Effects randomEffectList = new Effects();

                    new EffectSubParser_(randomEffectList, this.chapter).ParseElement(effect);

                    randomEffect.setPositiveEffect(randomEffectList.getEffects()[0]);
                    randomEffect.setNegativeEffect(randomEffectList.getEffects()[1]);

                    currentEffect = randomEffect;
                    break;
                case "wait-time":
                    int time = 0;

                    tmpArgVal = effect.GetAttribute("time");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        time = int.Parse(tmpArgVal);
                    }

                    // Add the new move NPC effect
                    currentEffect = new WaitTimeEffect(time);
                    break;
                case "show-text":
                    x = 0;
                    y = 0;
                    frontColor = 0;
                    borderColor = 0;
                    audioPath = "";
                    tmpArgVal = effect.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        x = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        y = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("frontColor");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        frontColor = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("borderColor");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        borderColor = int.Parse(tmpArgVal);
                    }
                    tmpArgVal = effect.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        audioPath = tmpArgVal;
                    }

                    // Add the new ShowTextEffect
                    currentEffect = new ShowTextEffect(effect.InnerText.ToString().Trim(), x, y, frontColor.ToString(), borderColor.ToString());
                    ((ShowTextEffect)currentEffect).setAudioPath(audioPath);
                    break;
                case "highlight-item":
                    int type = 0;
                    id = "";
                    animated = false;

                    tmpArgVal = effect.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        id = tmpArgVal;
                    }

                    tmpArgVal = effect.GetAttribute("animated");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        animated = (tmpArgVal.Equals("yes") ? true : false);
                    }

                    tmpArgVal = effect.GetAttribute("type");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        if (tmpArgVal.Equals("none"))
                            type = HighlightItemEffect.NO_HIGHLIGHT;
                        if (tmpArgVal.Equals("green"))
                            type = HighlightItemEffect.HIGHLIGHT_GREEN;
                        if (tmpArgVal.Equals("red"))
                            type = HighlightItemEffect.HIGHLIGHT_RED;
                        if (tmpArgVal.Equals("blue"))
                            type = HighlightItemEffect.HIGHLIGHT_BLUE;
                        if (tmpArgVal.Equals("border"))
                            type = HighlightItemEffect.HIGHLIGHT_BORDER;
                    }
                    currentEffect = new HighlightItemEffect(id, type, animated);
                    break;
                case "move-object":
                    float scale = 1.0f;
                    int translateSpeed = 20;
                    int scaleSpeed = 20;
                    x = 0;
                    y = 0;
                    id = "";
                    animated = false;

                    tmpArgVal = effect.GetAttribute("idTarget");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        id = tmpArgVal;
                    }
                    tmpArgVal = effect.GetAttribute("animated");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        animated = (tmpArgVal.Equals("yes") ? true : false);
                    }

                    tmpArgVal = effect.GetAttribute("x");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        x = int.Parse(tmpArgVal);
                    }

                    tmpArgVal = effect.GetAttribute("y");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        y = int.Parse(tmpArgVal);
                    }

                    tmpArgVal = effect.GetAttribute("scale");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        scale = float.Parse(tmpArgVal, CultureInfo.InvariantCulture);
                    }

                    tmpArgVal = effect.GetAttribute("translateSpeed");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        translateSpeed = int.Parse(tmpArgVal);
                    }

                    tmpArgVal = effect.GetAttribute("scaleSpeed");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        scaleSpeed = int.Parse(tmpArgVal);
                    }
                    currentEffect = new MoveObjectEffect(id, x, y, scale, animated, translateSpeed, scaleSpeed);
                    break;
                case "speak-player":
                    audioPath = "";

                    tmpArgVal = effect.GetAttribute("uri");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        audioPath = tmpArgVal;
                    }

                    // Add the effect and clear the current string
                    currentEffect = new SpeakPlayerEffect(effect.InnerText.ToString().Trim());
                    break;
                case "condition":
                    addeffect = false;
                    currentConditions = new Conditions();
                    new ConditionSubParser_(currentConditions, chapter).ParseElement(effect);
                    currentEffect.setConditions(currentConditions);

                    effectlist = effects.getEffects();
                    effectlist[effectlist.Count - 1].setConditions(currentConditions);
                    break;
                case "documentation":
                    addeffect = false;
                    break;
                default:
                    addeffect = false;
                    Debug.LogWarning("EFFECT NOT SUPPORTED: " + effect.Name);
                    break;
            }

            if (addeffect)
                effects.add(currentEffect);
        }
    }

}
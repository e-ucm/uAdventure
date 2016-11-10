using UnityEngine;
using System.Collections;
using System.Xml;

public class EffectsDOMWriter{
    /**
             * Constant for the effect block.
             */
    public const string EFFECTS = "effect";

    /**
     * Constant for the post effect block.
     */
    public const string POST_EFFECTS = "post-effect";

    /**
     * Constant for the not effect block.
     */
    public const string NOT_EFFECTS = "not-effect";

    /**
     * Constant for the macro block.
     */
    public const string MACRO = "macro";

    /**
     * Private constructor.
     */
    private EffectsDOMWriter()
    {

    }

    public static XmlNode buildDOM(string type, Effects effects)
    {
        XmlNode effectsNode = null;

       
            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            effectsNode = doc.CreateElement(type);
            appendEffects(doc, effectsNode, effects);

        return effectsNode;

    }

    public static XmlElement buildDOM(Macro macro)
    {

        XmlElement effectsNode = null;

       
            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            effectsNode = doc.CreateElement(MACRO);
            effectsNode.SetAttribute("id", macro.getId());
            XmlNode documentationNode = doc.CreateElement("documentation");
            documentationNode.AppendChild(doc.CreateTextNode(macro.getDocumentation()));
            effectsNode.AppendChild(documentationNode);
            appendEffects(doc, effectsNode, macro);

        return effectsNode;

    }

    public static void appendEffects(XmlDocument doc, XmlNode effectsNode, Effects effects)
    {
        // Add every effect
        foreach (AbstractEffect effect in effects.getEffects())
        {

            XmlElement effectElement = null;
            XmlNode conditionsNode = null;

            if (effect.getType() != EffectType.RANDOM_EFFECT)
                effectElement = buildEffectNode(effect, doc);
            else {
                RandomEffect randomEffect = (RandomEffect)effect;
                effectElement = doc.CreateElement("random-effect");
                effectElement.SetAttribute("probability", randomEffect.getProbability().ToString());

                XmlElement posEfElement = null;
                XmlElement negEfElement = null;

                if (randomEffect.getPositiveEffect() != null)
                {
                    posEfElement = buildEffectNode(randomEffect.getPositiveEffect(), doc);
                    effectElement.AppendChild(posEfElement);
                    if (randomEffect.getNegativeEffect() != null)
                    {
                        negEfElement = buildEffectNode(randomEffect.getNegativeEffect(), doc);
                        effectElement.AppendChild(negEfElement);
                    }
                }

            }
            // Create conditions for current effect
            conditionsNode = ConditionsDOMWriter.buildDOM(effect.getConditions());
            doc.ImportNode(conditionsNode, true);
            // Add the effect
            effectsNode.AppendChild(effectElement);

            // Add conditions associated to that effect
            effectsNode.AppendChild(conditionsNode);

        }

    }

    private static XmlElement buildEffectNode(AbstractEffect effect, XmlDocument doc)
    {
        XmlElement effectElement = null;

        switch (effect.getType())
        {
            case EffectType.ACTIVATE:
                ActivateEffect activateEffect = (ActivateEffect)effect;
                effectElement = doc.CreateElement("activate");
                effectElement.SetAttribute("flag", activateEffect.getTargetId());
                break;
            case EffectType.DEACTIVATE:
                DeactivateEffect deactivateEffect = (DeactivateEffect)effect;
                effectElement = doc.CreateElement("deactivate");
                effectElement.SetAttribute("flag", deactivateEffect.getTargetId());
                break;
            case EffectType.SET_VALUE:
                SetValueEffect setValueEffect = (SetValueEffect)effect;
                effectElement = doc.CreateElement("set-value");
                effectElement.SetAttribute("var", setValueEffect.getTargetId());
                effectElement.SetAttribute("value", setValueEffect.getValue().ToString());
                break;
            case EffectType.INCREMENT_VAR:
                IncrementVarEffect incrementVarEffect = (IncrementVarEffect)effect;
                effectElement = doc.CreateElement("increment");
                effectElement.SetAttribute("var", incrementVarEffect.getTargetId());
                effectElement.SetAttribute("value", incrementVarEffect.getIncrement().ToString());
                break;
            case EffectType.DECREMENT_VAR:
                DecrementVarEffect decrementVarEffect = (DecrementVarEffect)effect;
                effectElement = doc.CreateElement("decrement");
                effectElement.SetAttribute("var", decrementVarEffect.getTargetId());
                effectElement.SetAttribute("value", decrementVarEffect.getDecrement().ToString());
                break;
            case EffectType.MACRO_REF:
                MacroReferenceEffect macroRefEffect = (MacroReferenceEffect)effect;
                effectElement = doc.CreateElement("macro-ref");
                effectElement.SetAttribute("id", macroRefEffect.getTargetId());
                break;
            case EffectType.CONSUME_OBJECT:
                ConsumeObjectEffect consumeObjectEffect = (ConsumeObjectEffect)effect;
                effectElement = doc.CreateElement("consume-object");
                effectElement.SetAttribute("idTarget", consumeObjectEffect.getTargetId());
                break;
            case EffectType.GENERATE_OBJECT:
                GenerateObjectEffect generateObjectEffect = (GenerateObjectEffect)effect;
                effectElement = doc.CreateElement("generate-object");
                effectElement.SetAttribute("idTarget", generateObjectEffect.getTargetId());
                break;
            case EffectType.CANCEL_ACTION:
                effectElement = doc.CreateElement("cancel-action");
                break;
            case EffectType.SPEAK_PLAYER:
                SpeakPlayerEffect speakPlayerEffect = (SpeakPlayerEffect)effect;
                effectElement = doc.CreateElement("speak-player");
                if (speakPlayerEffect.getAudioPath() != null && !speakPlayerEffect.getAudioPath().Equals(""))
                    effectElement.SetAttribute("uri", speakPlayerEffect.getAudioPath());
                effectElement.AppendChild(doc.CreateTextNode(speakPlayerEffect.getLine()));
                break;
            case EffectType.SPEAK_CHAR:
                SpeakCharEffect speakCharEffect = (SpeakCharEffect)effect;
                effectElement = doc.CreateElement("speak-char");
                effectElement.SetAttribute("idTarget", speakCharEffect.getTargetId());
                if (speakCharEffect.getAudioPath() != null && !speakCharEffect.getAudioPath().Equals(""))
                    effectElement.SetAttribute("uri", speakCharEffect.getAudioPath());
                effectElement.AppendChild(doc.CreateTextNode(speakCharEffect.getLine()));
                break;
            case EffectType.TRIGGER_BOOK:
                TriggerBookEffect triggerBookEffect = (TriggerBookEffect)effect;
                effectElement = doc.CreateElement("trigger-book");
                effectElement.SetAttribute("idTarget", triggerBookEffect.getTargetId());
                break;
            case EffectType.PLAY_SOUND:
                PlaySoundEffect playSoundEffect = (PlaySoundEffect)effect;
                effectElement = doc.CreateElement("play-sound");
                if (!playSoundEffect.isBackground())
                    effectElement.SetAttribute("background", "no");
                effectElement.SetAttribute("uri", playSoundEffect.getPath());
                break;
            case EffectType.PLAY_ANIMATION:
                PlayAnimationEffect playAnimationEffect = (PlayAnimationEffect)effect;
                effectElement = doc.CreateElement("play-animation");
                effectElement.SetAttribute("uri", playAnimationEffect.getPath());
                effectElement.SetAttribute("x", playAnimationEffect.getX().ToString());
                effectElement.SetAttribute("y", playAnimationEffect.getY().ToString());
                break;
            case EffectType.MOVE_PLAYER:
                MovePlayerEffect movePlayerEffect = (MovePlayerEffect)effect;
                effectElement = doc.CreateElement("move-player");
                effectElement.SetAttribute("x", movePlayerEffect.getX().ToString());
                effectElement.SetAttribute("y", movePlayerEffect.getY().ToString());
                break;
            case EffectType.MOVE_NPC:
                MoveNPCEffect moveNPCEffect = (MoveNPCEffect)effect;
                effectElement = doc.CreateElement("move-npc");
                effectElement.SetAttribute("idTarget", moveNPCEffect.getTargetId());
                effectElement.SetAttribute("x", moveNPCEffect.getX().ToString());
                effectElement.SetAttribute("y", moveNPCEffect.getY().ToString());
                break;
            case EffectType.TRIGGER_CONVERSATION:
                TriggerConversationEffect triggerConversationEffect = (TriggerConversationEffect)effect;
                effectElement = doc.CreateElement("trigger-conversation");
                effectElement.SetAttribute("idTarget", triggerConversationEffect.getTargetId());
                break;
            case EffectType.TRIGGER_CUTSCENE:
                TriggerCutsceneEffect triggerCutsceneEffect = (TriggerCutsceneEffect)effect;
                effectElement = doc.CreateElement("trigger-cutscene");
                effectElement.SetAttribute("idTarget", triggerCutsceneEffect.getTargetId());
                break;
            case EffectType.TRIGGER_LAST_SCENE:
                effectElement = doc.CreateElement("trigger-last-scene");
                break;
            case EffectType.TRIGGER_SCENE:
                TriggerSceneEffect triggerSceneEffect = (TriggerSceneEffect)effect;
                effectElement = doc.CreateElement("trigger-scene");
                effectElement.SetAttribute("idTarget", triggerSceneEffect.getTargetId());
                effectElement.SetAttribute("x", triggerSceneEffect.getX().ToString());
                effectElement.SetAttribute("y", triggerSceneEffect.getY().ToString());
                break;
            case EffectType.WAIT_TIME:
                WaitTimeEffect waitTimeEffect = (WaitTimeEffect)effect;
                effectElement = doc.CreateElement("wait-time");
                effectElement.SetAttribute("time", waitTimeEffect.getTime().ToString());
                break;
            case EffectType.SHOW_TEXT:
                ShowTextEffect showTextEffect = (ShowTextEffect)effect;
                effectElement = doc.CreateElement("show-text");
                effectElement.SetAttribute("x", showTextEffect.getX().ToString());
                effectElement.SetAttribute("y", showTextEffect.getY().ToString());
                effectElement.SetAttribute("frontColor", showTextEffect.getRgbFrontColor().ToString());
                effectElement.SetAttribute("borderColor", showTextEffect.getRgbBorderColor().ToString());
                effectElement.SetAttribute("uri", showTextEffect.getAudioPath());
                effectElement.AppendChild(doc.CreateTextNode(showTextEffect.getText()));
                break;
            case EffectType.MOVE_OBJECT:
                MoveObjectEffect moveObjectEffect = (MoveObjectEffect)effect;
                effectElement = doc.CreateElement("move-object");
                effectElement.SetAttribute("idTarget", moveObjectEffect.getTargetId());
                effectElement.SetAttribute("x", moveObjectEffect.getX().ToString());
                effectElement.SetAttribute("y", moveObjectEffect.getY().ToString());
                effectElement.SetAttribute("scale", moveObjectEffect.getScale().ToString());
                effectElement.SetAttribute("animated", (moveObjectEffect.isAnimated() ? "yes" : "no"));
                effectElement.SetAttribute("translateSpeed", moveObjectEffect.getTranslateSpeed().ToString());
                effectElement.SetAttribute("scaleSpeed", moveObjectEffect.getScaleSpeed().ToString());
                break;
            case EffectType.HIGHLIGHT_ITEM:
                HighlightItemEffect highlightItemEffect = (HighlightItemEffect)effect;
                effectElement = doc.CreateElement("highlight-item");
                effectElement.SetAttribute("idTarget", highlightItemEffect.getTargetId());
                effectElement.SetAttribute("animated", (highlightItemEffect.isHighlightAnimated() ? "yes" : "no"));
                switch (highlightItemEffect.getHighlightType())
                {
                    case HighlightItemEffect.HIGHLIGHT_BLUE:
                        effectElement.SetAttribute("type", "blue");
                        break;
                    case HighlightItemEffect.HIGHLIGHT_BORDER:
                        effectElement.SetAttribute("type", "border");
                        break;
                    case HighlightItemEffect.HIGHLIGHT_RED:
                        effectElement.SetAttribute("type", "red");
                        break;
                    case HighlightItemEffect.HIGHLIGHT_GREEN:
                        effectElement.SetAttribute("type", "green");
                        break;
                    case HighlightItemEffect.NO_HIGHLIGHT:
                        effectElement.SetAttribute("type", "none");
                        break;
                }
                break;

        }
        return effectElement;
    }
}

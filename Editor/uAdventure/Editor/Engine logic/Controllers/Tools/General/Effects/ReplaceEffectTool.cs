using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ReplaceEffectTool : Tool
    {

        protected Effects effects;

        protected IEffect effect;

        protected Dictionary<int, System.Object> newProperties;

        protected IEffect oldEffect;

        protected IEffect pos;

        protected IEffect neg;

        public ReplaceEffectTool(Effects effects, IEffect effect, Dictionary<int, System.Object> newProperties) : this(effects, effect, newProperties, null, null)
        { }

        public ReplaceEffectTool(Effects effects, IEffect effect, Dictionary<int, System.Object> newProperties, IEffect pos, IEffect neg)
        {

            this.effects = effects;
            this.effect = effect;
            this.pos = pos;
            this.neg = neg;
            this.newProperties = newProperties;
        }

        public override bool canRedo()
        {

            return true;
        }

        public override bool canUndo()
        {

            return (oldEffect != null);
        }

        public override bool combine(Tool other)
        {

            return false;
        }

        public override bool doTool()
        {

            bool effectEdited = false;

            EffectType effectType = effect.getType();

            // If a change has been made
            if (newProperties != null)
            {
                effectEdited = true;
                oldEffect = effect;

                switch (effectType)
                {
                    case EffectType.ACTIVATE:
                        ActivateEffect activateEffect = (ActivateEffect)effect;
                        activateEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        Controller.Instance.updateVarFlagSummary();
                        break;
                    case EffectType.DEACTIVATE:
                        DeactivateEffect deactivateEffect = (DeactivateEffect)effect;
                        deactivateEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        Controller.Instance.updateVarFlagSummary();
                        break;
                    case EffectType.SET_VALUE:
                        SetValueEffect setValueEffect = (SetValueEffect)effect;
                        setValueEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        setValueEffect.setValue(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_VALUE]));
                        Controller.Instance.updateVarFlagSummary();
                        break;
                    case EffectType.INCREMENT_VAR:
                        IncrementVarEffect incrementVarEffect = (IncrementVarEffect)effect;
                        incrementVarEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        incrementVarEffect.setIncrement(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_VALUE]));
                        Controller.Instance.updateVarFlagSummary();
                        break;
                    case EffectType.DECREMENT_VAR:
                        DecrementVarEffect decrementVarEffect = (DecrementVarEffect)effect;
                        decrementVarEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        decrementVarEffect.setDecrement(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_VALUE]));
                        Controller.Instance.updateVarFlagSummary();
                        break;
                    case EffectType.MACRO_REF:
                        MacroReferenceEffect macroEffect = (MacroReferenceEffect)effect;
                        macroEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        break;
                    case EffectType.CONSUME_OBJECT:
                        ConsumeObjectEffect consumeObjectEffect = (ConsumeObjectEffect)effect;
                        consumeObjectEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        break;
                    case EffectType.GENERATE_OBJECT:
                        GenerateObjectEffect generateObjectEffect = (GenerateObjectEffect)effect;
                        generateObjectEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        break;
                    case EffectType.SPEAK_PLAYER:
                        SpeakPlayerEffect speakPlayerEffect = (SpeakPlayerEffect)effect;
                        speakPlayerEffect.setLine((string)newProperties[EffectsController.EFFECT_PROPERTY_TEXT]);
                        speakPlayerEffect.setAudioPath((string)newProperties[EffectsController.EFFECT_PROPERTY_PATH]);
                        break;
                    case EffectType.SPEAK_CHAR:
                        SpeakCharEffect speakCharEffect = (SpeakCharEffect)effect;
                        speakCharEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        speakCharEffect.setLine((string)newProperties[EffectsController.EFFECT_PROPERTY_TEXT]);
                        speakCharEffect.setAudioPath((string)newProperties[EffectsController.EFFECT_PROPERTY_PATH]);
                        break;
                    case EffectType.TRIGGER_BOOK:
                        TriggerBookEffect triggerBookEffect = (TriggerBookEffect)effect;
                        triggerBookEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        break;
                    case EffectType.PLAY_SOUND:
                        PlaySoundEffect playSoundEffect = (PlaySoundEffect)effect;
                        playSoundEffect.setPath((string)newProperties[EffectsController.EFFECT_PROPERTY_PATH]);
                        playSoundEffect.setBackground(bool.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_BACKGROUND]));
                        break;
                    case EffectType.PLAY_ANIMATION:
                        PlayAnimationEffect playAnimationEffect = (PlayAnimationEffect)effect;
                        playAnimationEffect.setPath((string)newProperties[EffectsController.EFFECT_PROPERTY_PATH]);
                        playAnimationEffect.setDestiny(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_X]), int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_Y]));
                        break;
                    case EffectType.MOVE_PLAYER:
                        MovePlayerEffect movePlayerEffect = (MovePlayerEffect)effect;
                        movePlayerEffect.setDestiny(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_X]), int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_Y]));
                        break;
                    case EffectType.MOVE_NPC:
                        MoveNPCEffect moveNPCEffect = (MoveNPCEffect)effect;
                        moveNPCEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        moveNPCEffect.setDestiny(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_X]), int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_Y]));
                        break;
                    case EffectType.TRIGGER_CONVERSATION:
                        TriggerConversationEffect triggerConversationEffect = (TriggerConversationEffect)effect;
                        triggerConversationEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        break;
                    case EffectType.TRIGGER_CUTSCENE:
                        TriggerCutsceneEffect triggerCutsceneEffect = (TriggerCutsceneEffect)effect;
                        triggerCutsceneEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        break;
                    case EffectType.TRIGGER_SCENE:
                        TriggerSceneEffect triggerSceneEffect = (TriggerSceneEffect)effect;
                        triggerSceneEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        triggerSceneEffect.setPosition(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_X]), int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_Y]));
                        break;
                    case EffectType.RANDOM_EFFECT:
                        RandomEffect randomEffect = (RandomEffect)effect;
                        randomEffect.setProbability(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_PROBABILITY]));
                        randomEffect.setPositiveEffect(pos);
                        randomEffect.setNegativeEffect(neg);
                        break;
                    case EffectType.WAIT_TIME:
                        WaitTimeEffect waitTimeEffect = (WaitTimeEffect)effect;
                        waitTimeEffect.setTime(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_TIME]));
                        break;
                    case EffectType.SHOW_TEXT:
                        ShowTextEffect showTextEffect = (ShowTextEffect)effect;
                        showTextEffect.setText((string)newProperties[EffectsController.EFFECT_PROPERTY_TEXT]);
                        showTextEffect.setTextPosition(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_X]), int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_Y]));

                        Color frontColor;
                        ColorUtility.TryParseHtmlString((string)newProperties[EffectsController.EFFECT_PROPERTY_FRONT_COLOR], out frontColor);
                        showTextEffect.setRgbFrontColor(frontColor);
                        Color borderColor;
                        ColorUtility.TryParseHtmlString((string)newProperties[EffectsController.EFFECT_PROPERTY_BORDER_COLOR], out borderColor);
                        showTextEffect.setRgbFrontColor(borderColor);
                        showTextEffect.setAudioPath((string)newProperties[EffectsController.EFFECT_PROPERTY_PATH]);
                        break;
                    case EffectType.HIGHLIGHT_ITEM:
                        HighlightItemEffect highlightItemEffect = (HighlightItemEffect)effect;
                        highlightItemEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        highlightItemEffect.setHighlightAnimated((bool)newProperties[EffectsController.EFFECT_PROPERTY_ANIMATED]);
                        highlightItemEffect.setHighlightType((int)newProperties[EffectsController.EFFECT_PROPERTY_HIGHLIGHT_TYPE]);
                        break;
                    case EffectType.MOVE_OBJECT:
                        MoveObjectEffect moveObjectEffect = (MoveObjectEffect)effect;
                        moveObjectEffect.setTargetId((string)newProperties[EffectsController.EFFECT_PROPERTY_TARGET]);
                        moveObjectEffect.setX(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_X]));
                        moveObjectEffect.setY(int.Parse((string)newProperties[EffectsController.EFFECT_PROPERTY_Y]));
                        moveObjectEffect.setScale((float)newProperties[EffectsController.EFFECT_PROPERTY_SCALE]);
                        moveObjectEffect.setAnimated((bool)newProperties[EffectsController.EFFECT_PROPERTY_ANIMATED]);
                        moveObjectEffect.setScaleSpeed((int)newProperties[EffectsController.EFFECT_PROPERTY_SCALE_SPEED]);
                        moveObjectEffect.setTranslateSpeed((int)newProperties[EffectsController.EFFECT_PROPERTY_TRANSLATION_SPEED]);
                        break;
                }
                effectEdited = true;
                Controller.Instance.updatePanel();
            }

            return effectEdited;
        }

        public override bool redoTool()
        {
            int index = effects.getEffects().IndexOf(oldEffect);
            effects.getEffects().Remove(oldEffect);
            effects.getEffects().Insert(index, effect);
            Controller.Instance.updateVarFlagSummary();
            Controller.Instance.updatePanel();
            return true;
        }

        public override bool undoTool()
        {
            int index = effects.getEffects().IndexOf(effect);
            effects.getEffects().Remove(effect);
            effects.getEffects().Insert(index, oldEffect);
            Controller.Instance.updateVarFlagSummary();
            Controller.Instance.updatePanel();
            return true;
        }

    }
}
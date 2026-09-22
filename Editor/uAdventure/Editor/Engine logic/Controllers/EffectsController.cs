using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * This class is the controller of the effects blocks. It manages the insertion
     * and modification of the effects lists.
     */

    public class EffectsController
    {

        /**
        * Constant for effect property. Refers to target elements.
        */

        public const
            int EFFECT_PROPERTY_TARGET = 0;

        /**
         * Constant for effect property. Refers to an asset path.
         */

        public const
            int EFFECT_PROPERTY_PATH = 1;

        /**
         * Constant for effect property. Refers to text content.
         */

        public const
            int EFFECT_PROPERTY_TEXT = 2;

        /**
         * Constant for effect property. Refers to X coordinate.
         */

        public const
            int EFFECT_PROPERTY_X = 3;

        /**
         * Constant for effect property. Refers to Y coordinate.
         */

        public const
            int EFFECT_PROPERTY_Y = 4;

        /**
         * Constant for effect property. Refers to "Play in background" flag.
         */

        public const
            int EFFECT_PROPERTY_BACKGROUND = 5;

        /**
         * Constant for effect property. Refers to "Play in background" flag.
         */

        public const
            int EFFECT_PROPERTY_PROBABILITY = 6;

        /**
         * Constant for effect property. Refers to "Value" flag.
         */

        public const
            int EFFECT_PROPERTY_VALUE = 7;

        /**
         * Constant for effect property. Refers to time value for WaitTimeEffect.
         */

        public const
            int EFFECT_PROPERTY_TIME = 8;

        /**
         * Constant for effect property. Refers to text front color .
         */

        public const
            int EFFECT_PROPERTY_FRONT_COLOR = 9;

        /**
         * Constant for effect property. Refers to text border color.
         */

        public const
            int EFFECT_PROPERTY_BORDER_COLOR = 10;

        /**
         * Constant for effect property. Refers to type (ACTIVATE | DEACTIVATE |
         * MOVE-NPC...).
         */

        public const
            int EFFECT_PROPERTY_TYPE = 11;

        /**
         * Constant for effect property. Refers to first effect (RandomEffect).
         */

        public const
            int EFFECT_PROPERTY_FIRST_EFFECT = 12;

        /**
         * Constant for effect property. Refers to second effect (RandomEffect).
         */

        public const
            int EFFECT_PROPERTY_SECOND_EFFECT = 13;

        /**
         * Constant for effect property. Refers to the type of the highlight (none, blue, green, ...)
         */

        public const
            int EFFECT_PROPERTY_HIGHLIGHT_TYPE = 14;

        /**
         * Constant for effect property. Refers to "animated" flag of the highlight
         */

        public const
            int EFFECT_PROPERTY_ANIMATED = 15;

        public const
            int EFFECT_PROPERTY_SCALE = 16;

        public const
            int EFFECT_PROPERTY_TRANSLATION_SPEED = 17;

        public const
            int EFFECT_PROPERTY_SCALE_SPEED = 18;


        /**
         * Constant to filter the selection of an asset. Used for animations.
         */

        public const
            int ASSET_ANIMATION = 0;

        /**
         * Constant to filter the selection of an asset. Used for sounds.
         */

        public const
            int ASSET_SOUND = 1;

        /**
         * Link to the main controller.
         */
        protected Controller controller;

        /**
         * Contained block of effects.
         */
        protected Effects effects;

        protected List<ConditionsController> conditionsList;

        protected bool waitingForEffectSelection = false;

        /**
         * Constructor.
         * 
         * @param effects
         *            Contained block of effects
         */

        public EffectsController(Effects effects)
        {

            this.effects = effects;
            controller = Controller.Instance;
            conditionsList = new List<ConditionsController>();
            // create the list of effects controllers
            foreach (var effect in
                effects.getEffects())
            {
                if(effect is AbstractEffect)
                {
                    conditionsList.Add(new ConditionsController((effect as AbstractEffect).getConditions(), Controller.EFFECT, getEffectInfo(effect)));
                }
                else
                {
                    conditionsList.Add(null);
                }
            }
        }

        /**
         * Return the conditions controller in the given position.
         * 
         * @param index
         * @return
         */

        public ConditionsController getConditionController(int index)
        {

            return conditionsList[index];
        }

        /**
         * Returns the number of effects contained.
         * 
         * @return Number of effects
         */

        public int getEffectCount()
        {
            return effects.getEffects().Count;
        }

        /**
         * Returns the info of the effect in the given position.
         * 
         * @param index
         *            Position of the effect
         * @return Information about the effect
         */

        public string getEffectInfo(int index)
        {
            if (getEffectCount() > 0)
            {
                IEffect effect = effects.getEffects()[index];
                return getEffectInfo(effect);
            }
            else
                return null;
        }

        /**
         * Returns the icon of the effect in the given position.
         * 
         * @param index
         *            Position of the effect
         * @return Icon of the effect
         */

        public Sprite getEffectIcon(int index)
        {

            Sprite icon = null;
            if (index >= 0 && index < effects.Count)
            {
                IEffect effect = effects[index];
                switch (effect.getType())
                {
                    case EffectType.ACTIVATE:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/activate.png", typeof(Sprite));
                        break;
                    case EffectType.DEACTIVATE:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/deactivate.png", typeof(Sprite));
                        break;
                    case EffectType.SET_VALUE:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/set-value.png", typeof(Sprite));
                        break;
                    case EffectType.INCREMENT_VAR:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/increment.png", typeof(Sprite));
                        break;
                    case EffectType.DECREMENT_VAR:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/decrement.png", typeof(Sprite));
                        break;
                    case EffectType.MACRO_REF:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/macro.png", typeof(Sprite));
                        break;
                    case EffectType.CONSUME_OBJECT:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/consume-object.png", typeof(Sprite));
                        break;
                    case EffectType.GENERATE_OBJECT:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/generate-object.png", typeof(Sprite));
                        break;
                    case EffectType.CANCEL_ACTION:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/cancel-action.png", typeof(Sprite));
                        break;
                    case EffectType.SPEAK_PLAYER:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/activate.png", typeof(Sprite));
                        if (((SpeakPlayerEffect)effect).getAudioPath() != null &&
                            !((SpeakPlayerEffect)effect).getAudioPath().Equals(""))
                            icon =
                                (Sprite)
                                    Resources.Load("img/icons/effects/16x16/speak-player-withsound.png", typeof(Sprite));
                        else
                            icon = (Sprite)Resources.Load("img/icons/effects/16x16/speak-player.png", typeof(Sprite));
                        break;
                    case EffectType.SPEAK_CHAR:
                        if (((SpeakCharEffect)effect).getAudioPath() != null &&
                            !((SpeakCharEffect)effect).getAudioPath().Equals(""))
                            icon =
                                (Sprite)Resources.Load("img/icons/effects/16x16/speak-npc-withsound.png", typeof(Sprite));
                        else
                            icon = (Sprite)Resources.Load("img/icons/effects/16x16/speak-npc.png", typeof(Sprite));
                        break;
                    case EffectType.TRIGGER_BOOK:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/trigger-book.png", typeof(Sprite));
                        break;
                    case EffectType.PLAY_SOUND:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/play-sound.png", typeof(Sprite));
                        break;
                    case EffectType.PLAY_ANIMATION:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/play-animation.png", typeof(Sprite));
                        break;
                    case EffectType.MOVE_PLAYER:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/move-player.png", typeof(Sprite));
                        break;
                    case EffectType.MOVE_NPC:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/move-npc.png", typeof(Sprite));
                        break;
                    case EffectType.TRIGGER_CONVERSATION:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/trigger-conversation.png", typeof(Sprite));
                        break;
                    case EffectType.TRIGGER_CUTSCENE:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/trigger-cutscene.png", typeof(Sprite));
                        break;
                    case EffectType.TRIGGER_SCENE:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/trigger-scene.png", typeof(Sprite));
                        break;
                    case EffectType.TRIGGER_LAST_SCENE:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/trigger-last-scene.png", typeof(Sprite));
                        break;
                    case EffectType.RANDOM_EFFECT:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/random-effect.png", typeof(Sprite));
                        break;
                    case EffectType.WAIT_TIME:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/wait.png", typeof(Sprite));
                        break;
                    case EffectType.SHOW_TEXT:
                        if (((ShowTextEffect)effect).getAudioPath() != null &&
                            !((ShowTextEffect)effect).getAudioPath().Equals(""))
                            icon =
                                (Sprite)Resources.Load("img/icons/effects/16x16/show-text-withsound.png", typeof(Sprite));
                        else
                            icon = (Sprite)Resources.Load("img/icons/effects/16x16/show-text.png", typeof(Sprite));
                        break;
                    case EffectType.HIGHLIGHT_ITEM:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/highlight-item.png", typeof(Sprite));
                        break;
                    case EffectType.MOVE_OBJECT:
                        icon = (Sprite)Resources.Load("img/icons/effects/16x16/move-object.png", typeof(Sprite));
                        break;
                }

            }
            return icon;
        }

        public static string getEffectInfo(IEffect effect)
        {

            string effectInfo = null;

            switch (effect.getType())
            {
                case EffectType.ACTIVATE:
                    ActivateEffect activateEffect = (ActivateEffect)effect;
                    effectInfo = "Effect.ActivateInfo" + activateEffect.getTargetId();
                    break;
                case EffectType.DEACTIVATE:
                    DeactivateEffect deactivateEffect = (DeactivateEffect)effect;
                    effectInfo = "Effect.DeactivateInfo" + deactivateEffect.getTargetId();
                    break;
                case EffectType.SET_VALUE:
                    SetValueEffect setValueEffect = (SetValueEffect)effect;
                    effectInfo = "Effect.SetValueInfo" + setValueEffect.getTargetId() + setValueEffect.getValue().ToString();
                    break;
                case EffectType.INCREMENT_VAR:
                    IncrementVarEffect incrementEffect = (IncrementVarEffect)effect;
                    effectInfo = "Effect.IncrementVarInfo" + incrementEffect.getTargetId() + incrementEffect.getIncrement();
                    break;
                case EffectType.DECREMENT_VAR:
                    DecrementVarEffect decrementEffect = (DecrementVarEffect)effect;
                    effectInfo = "Effect.DecrementVarInfo" + decrementEffect.getTargetId() + decrementEffect.getDecrement();
                    break;
                case EffectType.MACRO_REF:
                    MacroReferenceEffect macroReferenceEffect = (MacroReferenceEffect)effect;
                    effectInfo = "Effect.MacroRefInfo" + macroReferenceEffect.getTargetId();
                    break;
                case EffectType.CONSUME_OBJECT:
                    ConsumeObjectEffect consumeObjectEffect = (ConsumeObjectEffect)effect;
                    effectInfo = "Effect.ConsumeObjectInfo" + consumeObjectEffect.getTargetId();
                    break;
                case EffectType.GENERATE_OBJECT:
                    GenerateObjectEffect generateObjectEffect = (GenerateObjectEffect)effect;
                    effectInfo = "Effect.GenerateObjectInfo" + generateObjectEffect.getTargetId();
                    break;
                case EffectType.CANCEL_ACTION:
                    effectInfo = "Effect.CancelActionInfo";
                    break;
                case EffectType.SPEAK_PLAYER:
                    SpeakPlayerEffect speakPlayerEffect = (SpeakPlayerEffect)effect;
                    effectInfo = "Effect.SpeakPlayerInfo" + speakPlayerEffect.getLine();
                    break;
                case EffectType.SPEAK_CHAR:
                    SpeakCharEffect speakCharEffect = (SpeakCharEffect)effect;
                    effectInfo = "Effect.SpeakCharacterInfo" + speakCharEffect.getTargetId() + speakCharEffect.getLine();
                    break;
                case EffectType.TRIGGER_BOOK:
                    TriggerBookEffect triggerBookEffect = (TriggerBookEffect)effect;
                    effectInfo = "Effect.TriggerBookInfo" + triggerBookEffect.getTargetId();
                    break;
                case EffectType.PLAY_SOUND:
                    PlaySoundEffect playSoundEffect = (PlaySoundEffect)effect;
                    effectInfo = "Effect.PlaySoundInfo" + playSoundEffect.getPath();
                    break;
                case EffectType.PLAY_ANIMATION:
                    PlayAnimationEffect playAnimationEffect = (PlayAnimationEffect)effect;
                    effectInfo = "Effect.PlayAnimationInfo" + playAnimationEffect.getPath();
                    break;
                case EffectType.MOVE_PLAYER:
                    MovePlayerEffect movePlayerEffect = (MovePlayerEffect)effect;
                    effectInfo = "Effect.MovePlayerInfo" + movePlayerEffect.getX().ToString() +
                                 movePlayerEffect.getY().ToString();
                    break;
                case EffectType.MOVE_NPC:
                    MoveNPCEffect moveNPCEffect = (MoveNPCEffect)effect;
                    effectInfo = "Effect.MoveCharacterInfo" + moveNPCEffect.getTargetId() + moveNPCEffect.getX().ToString() +
                                 " " + moveNPCEffect.getY();
                    break;
                case EffectType.TRIGGER_CONVERSATION:
                    TriggerConversationEffect triggerConversationEffect = (TriggerConversationEffect)effect;
                    effectInfo = "Effect.TriggerConversationInfo" + triggerConversationEffect.getTargetId();
                    break;
                case EffectType.TRIGGER_CUTSCENE:
                    TriggerCutsceneEffect triggerCutsceneEffect = (TriggerCutsceneEffect)effect;
                    effectInfo = "Effect.TriggerCutsceneInfo" + triggerCutsceneEffect.getTargetId();
                    break;
                case EffectType.TRIGGER_SCENE:
                    TriggerSceneEffect triggerSceneEffect = (TriggerSceneEffect)effect;
                    effectInfo = "Effect.TriggerSceneInfo" + triggerSceneEffect.getTargetId();
                    break;
                case EffectType.TRIGGER_LAST_SCENE:
                    effectInfo = "Effect.TriggerLastSceneInfo";
                    break;
                case EffectType.RANDOM_EFFECT:
                    RandomEffect randomEffect = (RandomEffect)effect;
                    string posInfo = "";
                    string negInfo = "";
                    if (randomEffect.getPositiveEffect() != null)
                        posInfo = getEffectInfo(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        negInfo = getEffectInfo(randomEffect.getNegativeEffect());
                    effectInfo = "Effect.RandomInfo" + randomEffect.getProbability() + " " +
                                 (100 - randomEffect.getProbability()) + " " + posInfo + " " + negInfo;
                    break;
                case EffectType.WAIT_TIME:
                    WaitTimeEffect waitTimeEffect = (WaitTimeEffect)effect;
                    effectInfo = "Effect.WaitTimeInfo" + waitTimeEffect.getTime().ToString();
                    break;
                case EffectType.SHOW_TEXT:
                    ShowTextEffect showTextInfo = (ShowTextEffect)effect;
                    effectInfo = "Effect.ShowTextInfo" + showTextInfo.getText() + " " + showTextInfo.getX() + " " +
                                 showTextInfo.getY();
                    break;
                case EffectType.HIGHLIGHT_ITEM:
                    HighlightItemEffect highlightItemEffect = (HighlightItemEffect)effect;
                    if (highlightItemEffect.getHighlightType() == HighlightItemEffect.NO_HIGHLIGHT)
                        effectInfo = "Effect.NoHighlightItemInfo" + " " + highlightItemEffect.getTargetId();
                    if (highlightItemEffect.getHighlightType() == HighlightItemEffect.HIGHLIGHT_BLUE)
                        effectInfo = "Effect.BlueHighlightItemInfo" + " " + highlightItemEffect.getTargetId();
                    if (highlightItemEffect.getHighlightType() == HighlightItemEffect.HIGHLIGHT_GREEN)
                        effectInfo = "Effect.GreenHighlightItemInfo" + " " + highlightItemEffect.getTargetId();
                    if (highlightItemEffect.getHighlightType() == HighlightItemEffect.HIGHLIGHT_RED)
                        effectInfo = "Effect.RedHighlightItemInfo" + " " + highlightItemEffect.getTargetId();
                    if (highlightItemEffect.getHighlightType() == HighlightItemEffect.HIGHLIGHT_BORDER)
                        effectInfo = "Effect.BorderHighlightItemInfo" + " " + highlightItemEffect.getTargetId();
                    break;
                case EffectType.MOVE_OBJECT:
                    MoveObjectEffect moveObjectEffect = (MoveObjectEffect)effect;
                    effectInfo = "Effect.MoveObjectInfo" + " " + moveObjectEffect.getTargetId();
                    break;
            }


            return effectInfo;
        }

        public List<IEffect> getEffects()
        {

            return effects.getEffects();
        }

        public Effects getEffectsDirectly()
        {
            return effects;
        }

        /**
             * Starts Adding a new condition to the block.
             * 
             * @return True if an effect was added, false otherwise
             */

        public virtual bool addEffect(IEffect newEffect)
        {

            /*bool effectAdded = false;
            // TODO: visual version of effect checker
            //Dictionary<int, System.Object> effectProperties = SelectEffectsDialog.getNewEffectProperties(this);
            Dictionary<int, System.Object> effectProperties = new Dictionary<int, System.Object>();


            if (effectProperties != null)
            {
                int selectedType = 0;
                if (effectProperties.ContainsKey(EFFECT_PROPERTY_TYPE))
                {
                    selectedType = int.Parse((string)effectProperties[EFFECT_PROPERTY_TYPE]);
                }

                IEffect newEffect = createNewEffect(effectProperties);

                if (selectedType == (int)EffectType.RANDOM_EFFECT)
                {
                    IEffect firstEffect = null;
                    IEffect secondEffect = null;
                    if (effectProperties.ContainsKey(EFFECT_PROPERTY_FIRST_EFFECT))
                        firstEffect = (IEffect)effectProperties[EFFECT_PROPERTY_FIRST_EFFECT];
                    if (effectProperties.ContainsKey(EFFECT_PROPERTY_SECOND_EFFECT))
                        secondEffect = (IEffect)effectProperties[EFFECT_PROPERTY_SECOND_EFFECT];

                    RandomEffect randomEffect = new RandomEffect(50);
                    if (effectProperties.ContainsKey(EffectsController.EFFECT_PROPERTY_PROBABILITY))
                    {
                        randomEffect.setProbability(
                            int.Parse((string)effectProperties[EFFECT_PROPERTY_PROBABILITY]));
                    }
                    if (firstEffect != null)
                        randomEffect.setPositiveEffect(firstEffect);
                    if (secondEffect != null)
                        randomEffect.setNegativeEffect(secondEffect);
                    newEffect = randomEffect;
                }
                effectAdded = controller.AddTool(new AddEffectTool(effects, newEffect, conditionsList));
            }*/

            switch (newEffect.getType())
            {
                case EffectType.ACTIVATE:
                    controller.VarFlagSummary.addFlagReference(((ActivateEffect)newEffect).getTargetId());
                    break;
                case EffectType.DEACTIVATE:
                    controller.VarFlagSummary.addFlagReference(((DeactivateEffect)newEffect).getTargetId());
                    break;
                case EffectType.SET_VALUE:
                    controller.VarFlagSummary.addVarReference(((SetValueEffect)newEffect).getTargetId());
                    break;
                case EffectType.INCREMENT_VAR:
                    controller.VarFlagSummary.addVarReference(((IncrementVarEffect)newEffect).getTargetId());
                    break;
                case EffectType.DECREMENT_VAR:
                    controller.VarFlagSummary.addVarReference(((DecrementVarEffect)newEffect).getTargetId());
                    break;
            }

            return controller.AddTool(new AddEffectTool(effects, newEffect, conditionsList));
        }

        /**
         * Creates a new effect of the appropriate type except for the "RANDOM" type
         * 
         * @param effectProperties
         * @return
         */

        protected IEffect createNewEffect(Dictionary<int, System.Object> effectProperties)
        {
            EffectType selectedType = 0;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_TYPE))
            {
                selectedType = (EffectType)int.Parse((string)effectProperties[EFFECT_PROPERTY_TYPE]);
            }

            IEffect newEffect = null;

            // Take all the values from the set
            string target = (string)effectProperties[EFFECT_PROPERTY_TARGET];
            string path = (string)effectProperties[EFFECT_PROPERTY_PATH];
            string text = (string)effectProperties[EFFECT_PROPERTY_TEXT];
            int value = 0;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_VALUE))
                value = int.Parse((string)effectProperties[EFFECT_PROPERTY_VALUE]);

            int x = 0;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_X))
                x = int.Parse((string)effectProperties[EFFECT_PROPERTY_X]);

            int y = 0;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_Y))
                y = int.Parse((string)effectProperties[EFFECT_PROPERTY_Y]);

            bool background = false;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_BACKGROUND))
                background = bool.Parse((string)effectProperties[EFFECT_PROPERTY_BACKGROUND]);

            int time = 0;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_TIME))
                time = int.Parse((string)effectProperties[EFFECT_PROPERTY_TIME]);

            Color frontColor = default(Color);
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_FRONT_COLOR))
                ColorUtility.TryParseHtmlString((string)effectProperties[EFFECT_PROPERTY_FRONT_COLOR], out frontColor);

            Color borderColor = default(Color);
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_BORDER_COLOR))
                ColorUtility.TryParseHtmlString((string)effectProperties[EFFECT_PROPERTY_BORDER_COLOR], out borderColor);

            int type = 0;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_HIGHLIGHT_TYPE))
                type = (int)effectProperties[EFFECT_PROPERTY_HIGHLIGHT_TYPE];

            bool animated = false;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_ANIMATED))
                animated = (bool)effectProperties[EFFECT_PROPERTY_ANIMATED];

            float scale = 1.0f;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_SCALE))
                scale = (float)effectProperties[EFFECT_PROPERTY_SCALE];

            int translationSpeed = 20;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_TRANSLATION_SPEED))
                translationSpeed = (int)effectProperties[EFFECT_PROPERTY_TRANSLATION_SPEED];

            int scaleSpeed = 20;
            if (effectProperties.ContainsKey(EFFECT_PROPERTY_SCALE_SPEED))
                scaleSpeed = (int)effectProperties[EFFECT_PROPERTY_SCALE_SPEED];

            switch (selectedType)
            {
                case EffectType.ACTIVATE:
                    newEffect = new ActivateEffect(target);
                    controller.VarFlagSummary.addFlagReference(target);
                    break;
                case EffectType.DEACTIVATE:
                    newEffect = new DeactivateEffect(target);
                    controller.VarFlagSummary.addFlagReference(target);
                    break;
                case EffectType.SET_VALUE:
                    newEffect = new SetValueEffect(target, value);
                    controller.VarFlagSummary.addVarReference(target);
                    break;
                case EffectType.INCREMENT_VAR:
                    newEffect = new IncrementVarEffect(target, value);
                    controller.VarFlagSummary.addVarReference(target);
                    break;
                case EffectType.DECREMENT_VAR:
                    newEffect = new DecrementVarEffect(target, value);
                    controller.VarFlagSummary.addVarReference(target);
                    break;
                case EffectType.MACRO_REF:
                    newEffect = new MacroReferenceEffect(target);
                    break;
                case EffectType.CONSUME_OBJECT:
                    newEffect = new ConsumeObjectEffect(target);
                    break;
                case EffectType.GENERATE_OBJECT:
                    newEffect = new GenerateObjectEffect(target);
                    break;
                case EffectType.TRIGGER_LAST_SCENE:
                    newEffect = new TriggerLastSceneEffect();
                    break;
                case EffectType.SPEAK_PLAYER:
                    newEffect = new SpeakPlayerEffect(text);
                    ((SpeakPlayerEffect)newEffect).setAudioPath(path);
                    break;
                case EffectType.SPEAK_CHAR:
                    newEffect = new SpeakCharEffect(target, text);
                    ((SpeakCharEffect)newEffect).setAudioPath(path);
                    break;
                case EffectType.TRIGGER_BOOK:
                    newEffect = new TriggerBookEffect(target);
                    break;
                case EffectType.PLAY_SOUND:
                    newEffect = new PlaySoundEffect(background, path);
                    break;
                case EffectType.PLAY_ANIMATION:
                    newEffect = new PlayAnimationEffect(path, x, y);
                    break;
                case EffectType.MOVE_PLAYER:
                    newEffect = new MovePlayerEffect(x, y);
                    break;
                case EffectType.MOVE_NPC:
                    newEffect = new MoveNPCEffect(target, x, y);
                    break;
                case EffectType.TRIGGER_CONVERSATION:
                    newEffect = new TriggerConversationEffect(target);
                    break;
                case EffectType.TRIGGER_CUTSCENE:
                    newEffect = new TriggerCutsceneEffect(target);
                    break;
                case EffectType.TRIGGER_SCENE:
                    newEffect = new TriggerSceneEffect(target, x, y);
                    break;
                case EffectType.WAIT_TIME:
                    newEffect = new WaitTimeEffect(time);
                    break;
                case EffectType.SHOW_TEXT:
                    newEffect = new ShowTextEffect(text, x, y, frontColor, borderColor);
                    ((ShowTextEffect)newEffect).setAudioPath(path);
                    break;
                case EffectType.HIGHLIGHT_ITEM:
                    newEffect = new HighlightItemEffect(target, type, animated);
                    break;
                case EffectType.MOVE_OBJECT:
                    newEffect = new MoveObjectEffect(target, x, y, scale, animated, translationSpeed, scaleSpeed);
                    break;
                case EffectType.CANCEL_ACTION:
                    newEffect = new CancelActionEffect();
                    break;
            }

            return newEffect;
        }

        /**
         * Deletes the effect in the given position.
         * 
         * @param index
         *            Index of the effect
         */

        public void deleteEffect(int index)
        {
            controller.AddTool(new DeleteEffectTool(effects, index, conditionsList));
        }

        /**
         * Moves up the effect in the given position.
         * 
         * @param index
         *            Index of the effect to move
         * @return True if the effect was moved, false otherwise
         */

        public bool moveUpEffect(int index)
        {
            return controller.AddTool(new MoveEffectInTableTool(effects, index, MoveObjectTool.MODE_UP, conditionsList));
        }

        /**
         * Moves down the effect in the given position.
         * 
         * @param index
         *            Index of the effect to move
         * @return True if the effect was moved, false otherwise
         */

        public bool moveDownEffect(int index)
        {
            return controller.AddTool(new MoveEffectInTableTool(effects, index, MoveObjectTool.MODE_DOWN, conditionsList));
        }

        /**
         * Edits the effect in the given position.
         * 
         * @param index
         *            Index of the effect
         * @return True if the effect was moved, false otherwise
         */

        public bool editEffect(int index)
        {

            bool effectEdited = false;

            // Take the effect and its type
            IEffect effect = effects[index];
            EffectType effectType = effect.getType();

            // Create the hashmap to store the current values
            Dictionary<int, System.Object> currentValues = new Dictionary<int, System.Object>();

            if (effect is HasTargetId)
            {
                currentValues.Add(EFFECT_PROPERTY_TARGET, ((HasTargetId)effect).getTargetId());
            }

            switch (effectType)
            {
                case EffectType.ACTIVATE:
                case EffectType.DEACTIVATE:
                case EffectType.MACRO_REF:
                case EffectType.CONSUME_OBJECT:
                case EffectType.GENERATE_OBJECT:
                case EffectType.TRIGGER_BOOK:
                case EffectType.TRIGGER_CONVERSATION:
                case EffectType.TRIGGER_CUTSCENE:
                    break;
                case EffectType.SET_VALUE:
                    SetValueEffect setValueEffect = (SetValueEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_VALUE, setValueEffect.getValue());
                    break;
                case EffectType.INCREMENT_VAR:
                    IncrementVarEffect incrementVarEffect = (IncrementVarEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_VALUE, incrementVarEffect.getIncrement());
                    break;
                case EffectType.DECREMENT_VAR:
                    DecrementVarEffect decrementVarEffect = (DecrementVarEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_VALUE, decrementVarEffect.getDecrement());
                    break;
                case EffectType.SPEAK_PLAYER:
                    SpeakPlayerEffect speakPlayerEffect = (SpeakPlayerEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_PATH, speakPlayerEffect.getAudioPath());
                    currentValues.Add(EFFECT_PROPERTY_TEXT, speakPlayerEffect.getLine());
                    break;
                case EffectType.SPEAK_CHAR:
                    SpeakCharEffect speakCharEffect = (SpeakCharEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_PATH, speakCharEffect.getAudioPath());
                    currentValues.Add(EFFECT_PROPERTY_TEXT, speakCharEffect.getLine());
                    break;
                case EffectType.PLAY_SOUND:
                    PlaySoundEffect playSoundEffect = (PlaySoundEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_PATH, playSoundEffect.getPath());
                    currentValues.Add(EFFECT_PROPERTY_BACKGROUND, playSoundEffect.isBackground());
                    break;
                case EffectType.PLAY_ANIMATION:
                    PlayAnimationEffect playAnimationEffect = (PlayAnimationEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_PATH, playAnimationEffect.getPath());
                    currentValues.Add(EFFECT_PROPERTY_X, playAnimationEffect.getX());
                    currentValues.Add(EFFECT_PROPERTY_Y, playAnimationEffect.getY());
                    break;
                case EffectType.MOVE_PLAYER:
                    MovePlayerEffect movePlayerEffect = (MovePlayerEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_X, movePlayerEffect.getX());
                    currentValues.Add(EFFECT_PROPERTY_Y, movePlayerEffect.getY());
                    break;
                case EffectType.MOVE_NPC:
                    MoveNPCEffect moveNPCEffect = (MoveNPCEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_X, moveNPCEffect.getX());
                    currentValues.Add(EFFECT_PROPERTY_Y, moveNPCEffect.getY());
                    break;
                case EffectType.TRIGGER_SCENE:
                    TriggerSceneEffect triggerSceneEffect = (TriggerSceneEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_X, triggerSceneEffect.getX());
                    currentValues.Add(EFFECT_PROPERTY_Y, triggerSceneEffect.getY());
                    break;
                case EffectType.WAIT_TIME:
                    WaitTimeEffect waitTimeEffect = (WaitTimeEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_TIME, waitTimeEffect.getTime());
                    break;
                case EffectType.SHOW_TEXT:
                    ShowTextEffect showTextEffect = (ShowTextEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_TEXT, showTextEffect.getText());
                    currentValues.Add(EFFECT_PROPERTY_X, showTextEffect.getX());
                    currentValues.Add(EFFECT_PROPERTY_Y, showTextEffect.getY());
                    currentValues.Add(EFFECT_PROPERTY_FRONT_COLOR, showTextEffect.getRgbFrontColor());
                    currentValues.Add(EFFECT_PROPERTY_BORDER_COLOR, showTextEffect.getRgbBorderColor());
                    currentValues.Add(EFFECT_PROPERTY_PATH, showTextEffect.getAudioPath());
                    break;
                case EffectType.HIGHLIGHT_ITEM:
                    HighlightItemEffect highlightItem = (HighlightItemEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_HIGHLIGHT_TYPE, highlightItem.getHighlightType());
                    currentValues.Add(EFFECT_PROPERTY_ANIMATED, highlightItem.isHighlightAnimated());
                    break;
                case EffectType.MOVE_OBJECT:
                    MoveObjectEffect moveObject = (MoveObjectEffect)effect;
                    currentValues.Add(EFFECT_PROPERTY_X, moveObject.getX());
                    currentValues.Add(EFFECT_PROPERTY_Y, moveObject.getY());
                    currentValues.Add(EFFECT_PROPERTY_SCALE, moveObject.getScale());
                    currentValues.Add(EFFECT_PROPERTY_ANIMATED, moveObject.isAnimated());
                    currentValues.Add(EFFECT_PROPERTY_TRANSLATION_SPEED, moveObject.getTranslateSpeed());
                    currentValues.Add(EFFECT_PROPERTY_SCALE_SPEED, moveObject.getScaleSpeed());
                    break;
            }

            // Show the editing dialog
            Dictionary<int, System.Object> newProperties = null;
            SingleEffectController pos = null;
            SingleEffectController neg = null;

            if (effectType != EffectType.RANDOM_EFFECT)
                // TODO: implement visual version of effect checker
                //newProperties = EffectDialog.showEditEffectDialog(this, effectType, currentValues);
                newProperties = null;
            else
            {
                RandomEffect randomEffect = (RandomEffect)effect;
                pos = new SingleEffectController(randomEffect.getPositiveEffect());
                neg = new SingleEffectController(randomEffect.getNegativeEffect());

                // TODO: implement visual version of effect checker
                //newProperties = EffectDialog.showEditRandomEffectDialog(randomEffect.getProbability(), pos, neg);
            }

            // If a change has been made
            if (newProperties != null)
            {

                if (effectType != EffectType.RANDOM_EFFECT)
                {
                    effectEdited = controller.AddTool(new ReplaceEffectTool(effects, effect, newProperties));
                }
                else
                {
                    effectEdited =
                        controller.AddTool(new ReplaceEffectTool(effects, effect, newProperties, pos.getEffect(),
                            neg.getEffect()));
                }
            }

            return effectEdited;
        }

        /**
         * This method allows the user to select an asset to include it as an
         * animation or a sound to be played in the effects block.
         * 
         * @param assetType
         *            Type of the asset
         * @return The path of the asset if it was selected, or null if no asset was
         *         selected
         */

        public string selectAsset(int assetType)
        {
            int assetCategory = -1;
            int assetFilter = AssetsController.FILTER_NONE;
            if (assetType == ASSET_ANIMATION)
            {
                assetCategory = AssetsConstants.CATEGORY_ANIMATION;
                assetFilter = AssetsController.FILTER_PNG;

            }
            else if (assetType == ASSET_SOUND)
            {
                assetCategory = AssetsConstants.CATEGORY_AUDIO;
            }

            string assetPath = SelectResourceTool.selectAssetPathUsingChooser(assetCategory, assetFilter);

            return assetPath;
        }

        /**
         * Updates the given flag summary, adding the flag references contained in
         * the given effects.
         * 
         * @param varFlagSummary
         *            Flag summary to update
         * @param effects
         *            Set of effects to search in
         */

        public static void updateVarFlagSummary(VarFlagSummary varFlagSummary, Effects effects)
        {

            // Search every effect
            foreach (var effect in effects)
            {

                updateVarFlagSummary(varFlagSummary, effect);

            }
        }

        /**
         * Udaptes a flag summary according to a single Effect
         * 
         * @param varFlagSummary
         * @param effect
         */

        private static void updateVarFlagSummary(VarFlagSummary varFlagSummary, IEffect effect)
        {

            if (effect.getType() == EffectType.ACTIVATE)
            {
                ActivateEffect activateEffect = (ActivateEffect)effect;
                varFlagSummary.addFlagReference(activateEffect.getTargetId());
            }
            else if (effect.getType() == EffectType.DEACTIVATE)
            {
                DeactivateEffect deactivateEffect = (DeactivateEffect)effect;
                varFlagSummary.addFlagReference(deactivateEffect.getTargetId());
            }
            else if (effect.getType() == EffectType.SET_VALUE)
            {
                SetValueEffect setValueEffect = (SetValueEffect)effect;
                varFlagSummary.addVarReference(setValueEffect.getTargetId());
            }
            else if (effect.getType() == EffectType.INCREMENT_VAR)
            {
                IncrementVarEffect incrementEffect = (IncrementVarEffect)effect;
                varFlagSummary.addVarReference(incrementEffect.getTargetId());
            }
            else if (effect.getType() == EffectType.DECREMENT_VAR)
            {
                DecrementVarEffect decrementEffect = (DecrementVarEffect)effect;
                varFlagSummary.addVarReference(decrementEffect.getTargetId());
            }
            else if (effect.getType() == EffectType.RANDOM_EFFECT)
            {
                RandomEffect randomEffect = (RandomEffect)effect;
                if (randomEffect.getNegativeEffect() != null)
                {
                    updateVarFlagSummary(varFlagSummary, randomEffect.getNegativeEffect());
                }

                if (randomEffect.getPositiveEffect() != null)
                {
                    updateVarFlagSummary(varFlagSummary, randomEffect.getPositiveEffect());
                }

            }

            // UPdate conditions
            var abstractEffect = effect as AbstractEffect;
            if (abstractEffect != null)
            {
                ConditionsController.updateVarFlagSummary(varFlagSummary, abstractEffect.getConditions());
            }
        }

        /**
         * Returns if the effects block is valid or not.
         * 
         * @param currentPath
         *            string with the path to the given element (including the
         *            element)
         * @param incidences
         *            List to store the incidences in the elements. Null if no
         *            incidences track must be stored
         * @param effects
         *            Block of effects
         * @return True if the data structure pending from the element is valid,
         *         false otherwise
         */

        public static bool isValid(string currentPath, List<string> incidences, Effects effects)
        {

            bool valid = true;

            // Search every effect
            foreach (var effect in effects)
            {
                EffectType type = effect.getType();

                // If the effect is an animation without asset, set as invalid
                if (type == EffectType.PLAY_ANIMATION && ((PlayAnimationEffect)effect).getPath().Length == 0)
                {
                    valid = false;

                    // Store the incidence
                    if (incidences != null)
                        incidences.Add(currentPath);
                }

                // If the effect is a sound without asset, set as invalid
                else if (type == EffectType.PLAY_SOUND && ((PlaySoundEffect)effect).getPath().Length == 0)
                {
                    valid = false;

                    // Store the incidence
                    if (incidences != null)
                        incidences.Add(currentPath);
                }

                // If random effect
                else if (type == EffectType.RANDOM_EFFECT)
                {

                    RandomEffect randomEffect = (RandomEffect)effect;
                    Effects e = new Effects();
                    if (randomEffect.getPositiveEffect() != null)
                        e.Add(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        e.Add(randomEffect.getNegativeEffect());
                    EffectsController.isValid(currentPath, incidences, e);

                }
            }

            return valid;
        }

        /**
         * Returns the count of references to the given asset path in the block of
         * effects.
         * 
         * @param assetPath
         *            Path to the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         * @param effects
         *            Block of effects
         * @return Number of references to the asset path in the block of effects
         */

        public static int countAssetReferences(string assetPath, Effects effects)
        {

            int count = 0;

            // Search every effect
            foreach (var effect in effects)
            {
                EffectType type = effect.getType();

                // If the asset appears in an animation or sound, add it
                if ((type == EffectType.PLAY_ANIMATION && ((PlayAnimationEffect)effect).getPath().Equals(assetPath)) ||
                    (type == EffectType.PLAY_SOUND && ((PlaySoundEffect)effect).getPath().Equals(assetPath))
                    || type == EffectType.SPEAK_CHAR && ((SpeakCharEffect)effect).getAudioPath().Equals(assetPath)
                    || type == EffectType.SPEAK_PLAYER && ((SpeakPlayerEffect)effect).getAudioPath().Equals(assetPath))
                    count++;

                // If random effect
                else if (type == EffectType.RANDOM_EFFECT)
                {

                    RandomEffect randomEffect = (RandomEffect)effect;
                    Effects e = new Effects();
                    if (randomEffect.getPositiveEffect() != null)
                        e.Add(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        e.Add(randomEffect.getNegativeEffect());
                    count += EffectsController.countAssetReferences(assetPath, e);

                }

            }

            return count;
        }

        /**
         * Produces a list with all the referenced assets in the data control. This
         * method works recursively.
         * 
         * @param assetPaths
         *            The list with all the asset references. The list will only
         *            contain each asset path once, even if it is referenced more
         *            than once.
         * @param assetTypes
         *            The types of the assets contained in assetPaths.
         */

        public static void getAssetReferences(List<string> assetPaths, List<int> assetTypes, Effects effects)
        {

            // Search every effect
            foreach (var effect in effects.getEffects())
            {
                EffectType type = effect.getType();

                int assetType = -1;
                string assetPath = null;
                // If the asset appears in an animation or sound, add it
                if (type == EffectType.PLAY_ANIMATION)
                {
                    PlayAnimationEffect animationEffect = (PlayAnimationEffect)effect;
                    assetPath = animationEffect.getPath();
                    assetType = AssetsConstants.CATEGORY_ANIMATION;
                    //( (PlayAnimationEffect) effect ).getPath( ).Equals( assetPath ) ) || ( type == Effect.PLAY_SOUND && ( (PlaySoundEffect) effect ).getPath( ).Equals( assetPath ) ) )
                }
                else if (type == EffectType.PLAY_SOUND)
                {
                    PlaySoundEffect soundEffect = (PlaySoundEffect)effect;
                    assetPath = soundEffect.getPath();
                    assetType = AssetsConstants.CATEGORY_AUDIO;
                }

                else if (type == EffectType.SPEAK_CHAR)
                {
                    SpeakCharEffect speakCharEffect = (SpeakCharEffect)effect;
                    if (speakCharEffect.getAudioPath() != null && !speakCharEffect.getAudioPath().Equals(""))
                    {
                        assetPath = speakCharEffect.getAudioPath();
                        assetType = AssetsConstants.CATEGORY_AUDIO;
                    }
                }

                else if (type == EffectType.SPEAK_PLAYER)
                {
                    SpeakPlayerEffect speakPlayerEffect = (SpeakPlayerEffect)effect;
                    if (speakPlayerEffect.getAudioPath() != null && !speakPlayerEffect.getAudioPath().Equals(""))
                    {
                        assetPath = speakPlayerEffect.getAudioPath();
                        assetType = AssetsConstants.CATEGORY_AUDIO;
                    }
                }

                // If random effect
                else if (type == EffectType.RANDOM_EFFECT)
                {

                    RandomEffect randomEffect = (RandomEffect)effect;
                    Effects e = new Effects();
                    if (randomEffect.getPositiveEffect() != null)
                        e.Add(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        e.Add(randomEffect.getNegativeEffect());

                    EffectsController.getAssetReferences(assetPaths, assetTypes, e);
                }

                if (assetPath != null)
                {
                    // Search the list. If the asset is not in it, add it
                    bool add = true;
                    foreach (string asset in assetPaths)
                    {
                        if (asset.Equals(assetPath))
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        int last = assetPaths.Count;
                        assetPaths.Insert(last, assetPath);
                        assetTypes.Insert(last, assetType);
                    }
                }
            }
        }

        /**
         * Deletes all the references to the asset path in the given block of
         * effects.
         * 
         * @param assetPath
         *            Path to the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         * @param effects
         *            Block of effects
         */

        public static void deleteAssetReferences(string assetPath, Effects effects)
        {

            // Search every effect
            foreach (var effect in effects.getEffects())
            {
                EffectType type = effect.getType();

                // If the effect is a play animation or play sound effect
                if (type == EffectType.PLAY_ANIMATION)
                {
                    // If the asset is the same, delete it
                    PlayAnimationEffect playAnimationEffect = (PlayAnimationEffect)effect;
                    if (playAnimationEffect.getPath().Equals(assetPath))
                    {
                        playAnimationEffect.setPath("");
                    }
                }
                else if (type == EffectType.PLAY_SOUND)
                {
                    // If the asset is the same, delete it
                    PlaySoundEffect playSoundEffect = (PlaySoundEffect)effect;
                    if (playSoundEffect.getPath().Equals(assetPath))
                    {
                        playSoundEffect.setPath("");
                    }
                }

                else if (type == EffectType.SPEAK_CHAR)
                {
                    // If the asset is the same, delete it
                    SpeakCharEffect speakCharEffect = (SpeakCharEffect)effect;
                    if (speakCharEffect.getAudioPath().Equals(assetPath))
                    {
                        speakCharEffect.setAudioPath("");
                    }
                }

                else if (type == EffectType.SPEAK_PLAYER)
                {
                    // If the asset is the same, delete it
                    SpeakPlayerEffect speakPlayerEffect = (SpeakPlayerEffect)effect;
                    if (speakPlayerEffect.getAudioPath().Equals(assetPath))
                    {
                        speakPlayerEffect.setAudioPath("");
                    }
                }

                // If random effect
                else if (type == EffectType.RANDOM_EFFECT)
                {
                    RandomEffect randomEffect = (RandomEffect)effect;
                    Effects e = new Effects();
                    if (randomEffect.getPositiveEffect() != null)
                        e.Add(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        e.Add(randomEffect.getNegativeEffect());
                    EffectsController.deleteAssetReferences(assetPath, e);
                }
            }
        }

        /**
         * Returns the count of references to the given identifier in the block of
         * effects.
         * 
         * @param id
         *            Identifier to search
         * @param effects
         *            Block of effects
         * @return Number of references to the identifier in the block of effects
         */

        public static int countIdentifierReferences(string id, Effects effects)
        {

            int count = 0;

            // Search every effect
            foreach (var effect in effects.getEffects())
            {
                EffectType type = effect.getType();

                // If random effect
                if (type == EffectType.RANDOM_EFFECT)
                {

                    RandomEffect randomEffect = (RandomEffect)effect;
                    Effects e = new Effects();
                    if (randomEffect.getPositiveEffect() != null)
                        e.Add(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        e.Add(randomEffect.getNegativeEffect());
                    EffectsController.countIdentifierReferences(id, e);
                }
                else if (effect is HasTargetId && ((HasTargetId)effect).getTargetId().Equals(id))
                {
                    count++;
                }

                var hasExtraIds = effect as HasExtraIds;
                if (hasExtraIds != null)
                {
                    count+= hasExtraIds.getIds().Count(id.Equals);
                }

                if (effect is AbstractEffect)
                {
                    ConditionsController conditionsController =
                        new ConditionsController(((AbstractEffect)effect).getConditions());
                    count += conditionsController.countIdentifierReferences(id);
                }
            }

            return count;
        }

        /**
         * Replaces all the references to the given identifier to references to a
         * new one in the block of effects.
         * 
         * @param oldId
         *            Identifier which references must be replaced
         * @param newId
         *            New identifier to replace the old references
         * @param effects
         *            Block of effects
         */

        public static void replaceIdentifierReferences(string oldId, string newId, Effects effects)
        {
            foreach (var effect in effects.getEffects())
            {
                if (effect is HasTargetId)
                {
                    if (((HasTargetId)effect).getTargetId().Equals(oldId))
                        ((HasTargetId)effect).setTargetId(newId);
                }
                else if (effect.getType() == EffectType.RANDOM_EFFECT)
                {
                    RandomEffect randomEffect = (RandomEffect)effect;
                    Effects e = new Effects();
                    if (randomEffect.getPositiveEffect() != null)
                        e.Add(randomEffect.getPositiveEffect());
                    if (randomEffect.getNegativeEffect() != null)
                        e.Add(randomEffect.getNegativeEffect());
                    EffectsController.replaceIdentifierReferences(oldId, newId, e);
                }

                var hasExtraIds = effect as HasExtraIds;
                if (hasExtraIds != null)
                {
                    var ids = hasExtraIds.getIds();
                    for (int i = 0, l = ids.Length; i < l; i++)
                    {
                        if (ids[i].Equals(oldId))
                        {
                            ids[i] = newId;
                        }
                    }
                    hasExtraIds.setIds(ids);
                }

                if (effect is AbstractEffect)
                {
                    ConditionsController conditionsController =
                        new ConditionsController(((AbstractEffect)effect).getConditions());
                    conditionsController.replaceIdentifierReferences(oldId, newId);
                }
            }
        }

        /**
         * Deletes all the references to the given identifier in the block of
         * effects. All the effects in the block that reference to the given
         * identifier will be deleted.
         * 
         * @param id
         *            Identifier which references must be deleted
         * @param effects
         *            Block of effects
         */

        public static void deleteIdentifierReferences(string id, Effects effects)
        {

            int i = 0;

            // For earch effect
            while (i < effects.getEffects().Count)
            {

                // Get the effect and the type
                IEffect effect = effects.getEffects()[i];
                EffectType type = effect.getType();
                bool deleteEffect = false;

                // check identifier references in conditions
                if(effect is AbstractEffect)
                {
                    ConditionsController conditionsController =
                        new ConditionsController(((AbstractEffect)effect).getConditions());
                    conditionsController.deleteIdentifierReferences(id);
                }
                // If random effect
                if (type == EffectType.RANDOM_EFFECT)
                {
                    RandomEffect randomEffect = (RandomEffect)effect;

                    if (randomEffect.getPositiveEffect() != null)
                    {
                        if (deleteSingleEffect(id, randomEffect.getPositiveEffect()))
                        {
                            randomEffect.setPositiveEffect(null);
                            deleteEffect = true;
                        }
                    }
                    if (randomEffect.getNegativeEffect() != null)
                    {
                        if (deleteSingleEffect(id, randomEffect.getNegativeEffect()))
                        {
                            randomEffect.setNegativeEffect(null);

                        }
                    }

                }
                else
                {
                    deleteEffect = deleteSingleEffect(id, effect);
                }



                var hasExtraIds = effect as HasExtraIds;
                if (hasExtraIds != null)
                {
                    var ids = hasExtraIds.getIds().ToList();
                    for (int j = ids.Count - 1; j >= 0; j--)
                    {
                        if (ids[j].Equals(id))
                        {
                            ids.RemoveAt(j);
                        }
                    }
                    hasExtraIds.setIds(ids.ToArray());
                }

                // Delete the effect, or increase the counter
                if (deleteEffect)
                    effects.getEffects().RemoveAt(i);
                else
                    i++;

            }
        }

        private static bool deleteSingleEffect(string id, IEffect effect)
        {
            bool deleteEffect = false;

            if (effect is HasTargetId)
            {
                deleteEffect = ((HasTargetId)effect).getTargetId().Equals(id);
            }


            return deleteEffect;
        }
    }
}
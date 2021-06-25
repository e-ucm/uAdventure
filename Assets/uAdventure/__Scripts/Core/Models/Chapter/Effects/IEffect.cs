using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Core
{
    public enum EffectType
    {

        /**
        * Constant for activate effect.
        */
        ACTIVATE = 0,

        /**
         * Constant for deactivate effect.
         */
        DEACTIVATE = 1,

        /**
         * Constant for consume-object effect.
         */
        CONSUME_OBJECT = 2,

        /**
         * Constant for generate-object effect.
         */
        GENERATE_OBJECT = 3,

        /**
         * Constant for cancel-action effect.
         */
        CANCEL_ACTION = 4,

        /**
         * Constant for speak-player effect.
         */
        SPEAK_PLAYER = 5,

        /**
         * Constant for speak-char effect.
         */
        SPEAK_CHAR = 6,

        /**
         * Constant for trigger-book effect.
         */
        TRIGGER_BOOK = 7,

        /**
         * Constant for play-sound effect.
         */
        PLAY_SOUND = 8,

        /**
         * Constant for play-animation effect.
         */
        PLAY_ANIMATION = 9,

        /**
         * Constant for move-player effect.
         */
        MOVE_PLAYER = 10,

        /**
         * Constant for move-npc effect.
         */
        MOVE_NPC = 11,

        /**
         * Constant for trigger-conversation effect.
         */
        TRIGGER_CONVERSATION = 12,

        /**
         * Constant for trigger-cutscene effect.
         */
        TRIGGER_CUTSCENE = 13,

        /**
         * Constant for trigger-scene effect.
         */
        TRIGGER_SCENE = 14,

        /**
         * Constant for trigger-last-scene effect.
         */
        TRIGGER_LAST_SCENE = 15,

        /**
         * Constant for random-effect.
         */
        RANDOM_EFFECT = 16,

        /**
         * Constant for set-value effect.
         */
        SET_VALUE = 17,

        /**
         * Constant for increment var effect.
         */
        INCREMENT_VAR = 18,

        /**
         * Constant for decrement var effect.
         */
        DECREMENT_VAR = 19,

        /**
         * Constant for macro-ref effect.
         */
        MACRO_REF = 20,

        /**
         * Constant for wait-time effect
         */
        WAIT_TIME = 21,

        /**
         * Constant for show-text effect
         */
        SHOW_TEXT = 22,

        /**
         * Constant for highlight element effect
         */
        HIGHLIGHT_ITEM = 23,

        /**
         * Constant for move object effect
         */
        MOVE_OBJECT = 24,

        /// <summary>
        /// use this for any other custom effect customly executed
        /// </summary>
        CUSTOM_EFFECT = 25
    }

    /**
     * This interface defines any individual effect that can be triggered by a
     * player's action during the game.
     */
    public interface IEffect : ICloneable
    {
        string GUID { get; set; }
        /**
         * Returns the type of the effect.
         * 
         * @return Type of the effect
         */
        EffectType getType();

    }
}
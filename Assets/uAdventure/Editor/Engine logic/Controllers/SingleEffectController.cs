using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * This class is the controller of the effects blocks. It manages the insertion
     * and modification of the effects lists.
     */
    public class SingleEffectController : EffectsController
    {
        private static Effects createEffectsStructure(IEffect effect)
        {

            Effects effects = new Effects();
            if (effect != null)
                effects.Add(effect);
            return effects;
        }

        /**
         * Constructor.
         * 
         * @param effects
         *            Contained block of effects
         */
        public SingleEffectController(IEffect effect) : base(createEffectsStructure(effect))
        {
        }

        public SingleEffectController() : base(new Effects())
        {

        }

        /**
         * Returns the info of the effect in the given position.
         * 
         * @param index
         *            Position of the effect
         * @return Information about the effect
         */
        public string getEffectInfo()
        {

            if (getEffectCount() > 0)
                return getEffectInfo(0);
            return null;
        }

        /**
         * Adds a new condition to the block.
         * 
         * @return True if an effect was added, false otherwise
         */
        public override bool addEffect(IEffect effect)
        {

            effects.Clear();

            bool effectAdded = false;

            // Create a list with the names of the effects (in the same order as the next)
            string[] effectNames = new string[] { "EffectType.Activate", "EffectType.Deactivate", "EffectType.SetValue", "EffectType.ConsumeObject", "EffectType.GenerateObject", "EffectType.SpeakPlayer", "EffectType.SpeakCharacter", "EffectType.TriggerBook", "EffectType.PlaySound", "EffectType.PlayAnimation", "EffectType.MovePlayer", "EffectType.MoveCharacter", "EffectType.TriggerConversation", "EffectType.TriggerCutscene", "EffectType.TriggerScene", "EffectType.TriggerLastScene", "EffectType.ShowText", "EffectType.WaitTime", "EffectType.MoveObject" };

            // Create a list with the types of the effects (in the same order as the previous)
            EffectType[] effectTypes = new EffectType[] { EffectType.ACTIVATE, EffectType.DEACTIVATE, EffectType.SET_VALUE, EffectType.CONSUME_OBJECT, EffectType.GENERATE_OBJECT, EffectType.SPEAK_PLAYER, EffectType.SPEAK_CHAR, EffectType.TRIGGER_BOOK, EffectType.PLAY_SOUND, EffectType.PLAY_ANIMATION, EffectType.MOVE_PLAYER, EffectType.MOVE_NPC, EffectType.TRIGGER_CONVERSATION, EffectType.TRIGGER_CUTSCENE, EffectType.TRIGGER_SCENE, EffectType.TRIGGER_LAST_SCENE, EffectType.SHOW_TEXT, EffectType.WAIT_TIME, EffectType.MOVE_OBJECT };

            //TODO: ADDING EFFECTS UI
            // Show a dialog to select the type of the effect
            //string selectedValue = controller.showInputDialog("Effects.OperationAddEffect"), "Effects.SelectEffectType"), effectNames);

            //// If some effect was selected
            //if (selectedValue != null)
            //{
            //    // Store the type of the effect selected
            //    int selectedType = 0;
            //    for (int i = 0; i < effectNames.length; i++)
            //        if (effectNames[i].equals(selectedValue))
            //            selectedType = effectTypes[i];

            //    HashMap<Integer, Object> effectProperties = null;
            //    if (selectedType == EffectType.MOVE_PLAYER && Controller.getInstance().isPlayTransparent())
            //    {
            //        Controller.getInstance().showErrorDialog("Error.EffectMovePlayerNotAllowed.Title"), "Error.EffectMovePlayerNotAllowed.Message"));
            //    }
            //    else {

            //        effectProperties = EffectDialog.showAddEffectDialog(this, selectedType);

            //    }

            //    if (effectProperties != null)
            //    {
            //        AbstractEffect newEffect = null;
            //        effectProperties.put(EffectsController.EFFECT_PROPERTY_TYPE, Integer.tostring(selectedType));
            //        newEffect = createNewEffect(effectProperties);


            //        effectAdded = controller.addTool(new AddEffectTool(effects, newEffect, null));
            //    }
            //}

            return effectAdded;
        }

        /**
         * Deletes the effect in the given position.
         * 
         * @param index
         *            Index of the effect
         */
        public void deleteEffect()
        {
            if (getEffectCount() > 0)
                deleteEffect(0);
        }

        /**
         * Edits the effect in the given position.
         * 
         * @param index
         *            Index of the effect
         * @return True if the effect was moved, false otherwise
         */
        public bool editEffect()
        {

            if (getEffectCount() > 0)
                return editEffect(0);
            else
                return addEffect(null); // TODO null was put there just to silence the compiler after IEffect was added as a parameter in the effectsController add effect
        }

        public IEffect getEffect()
        {

            if (getEffectCount() > 0)
                return effects.getEffects()[0];
            else
                return null;
        }
    }
}
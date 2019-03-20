using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * A list of effects that can be triggered by an unique player's action during
     * the game.
     */

    public class Effects : List<IEffect>, ICloneable
    {

        /**
         * Creates a new list of Effects.
         */

        public Effects() : base()
        {
        }

        /**
         * Returns whether the effects block is empty or not.
         * 
         * @return True if the block has no effects, false otherwise
         */

        public bool IsEmpty()
        {
            return Count == 0;
        }

        /**
         * Adds a new effect to the list.
         * 
         * @param effect
         *            the effect to be added
         */

        public new void Add(IEffect effect)
        {
            base.Add(effect);

            //Check if the effect has resources, to add it in the AllAssetsPaths
            if (effect.getType() == EffectType.PLAY_ANIMATION || effect.getType() == EffectType.PLAY_SOUND ||
                effect.getType() == EffectType.SPEAK_CHAR || effect.getType() == EffectType.SPEAK_PLAYER ||
                effect.getType() == EffectType.SHOW_TEXT)
            {
                AllElementsWithAssets.addAsset(effect);
            }
            else if (effect.getType() == EffectType.RANDOM_EFFECT)
            {
                if (((RandomEffect)effect).getPositiveEffect() != null)
                {
                    EffectType peType = ((RandomEffect)effect).getPositiveEffect().getType();
                    if (peType == EffectType.PLAY_ANIMATION || peType == EffectType.PLAY_SOUND ||
                        peType == EffectType.SPEAK_CHAR || peType == EffectType.SPEAK_PLAYER ||
                        peType == EffectType.SHOW_TEXT)
                    {
                        AllElementsWithAssets.addAsset(((RandomEffect)effect).getPositiveEffect());
                    }
                }
                if (((RandomEffect)effect).getNegativeEffect() != null)
                {
                    EffectType neType = ((RandomEffect)effect).getNegativeEffect().getType();
                    if (neType == EffectType.PLAY_ANIMATION || neType == EffectType.PLAY_SOUND ||
                        neType == EffectType.SPEAK_CHAR || neType == EffectType.SPEAK_PLAYER ||
                        neType == EffectType.SHOW_TEXT)
                    {
                        AllElementsWithAssets.addAsset(((RandomEffect)effect).getNegativeEffect());
                    }
                }
            }

        }

        /**
         * Returns the contained list of effects
         * 
         * @return List of effects
         */

        public List<IEffect> getEffects()
        {
            return this;
        }

        /**
         * Checks if there is any cancel action effect in the list
         */

        public bool hasCancelAction()
        {
            bool hasCancelAction = false;
            foreach (IEffect effect in this)
            {
                if (effect.getType() == EffectType.CANCEL_ACTION)
                {
                    hasCancelAction = true;
                    break;
                }
            }
            return hasCancelAction;
        }

        public virtual object Clone()
        {
            var e = new Effects();
            foreach (var ef in this)
            {
                e.Add((IEffect) ef.Clone());
            }
            return e;
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A list of effects that can be triggered by an unique player's action during
 * the game.
 */

public class Effects : ICloneable
{

    /**
     * List of effects to be triggered
     */
    private List<AbstractEffect> effects;

    /**
     * Creates a new list of Effects.
     */

    public Effects()
    {

        effects = new List<AbstractEffect>();
    }

    /**
     * Returns whether the effects block is empty or not.
     * 
     * @return True if the block has no effects, false otherwise
     */

    public bool isEmpty()
    {

        return effects.Count == 0;
    }

    /**
     * Clear the list of effects.
     */

    public void clear()
    {

        effects.Clear();
    }

    /**
     * Adds a new effect to the list.
     * 
     * @param effect
     *            the effect to be added
     */

    public void add(AbstractEffect effect)
    {
        effects.Add(effect);

        //Check if the effect has resources, to add it in the AllAssetsPaths
        if (effect.getType() == EffectType.PLAY_ANIMATION || effect.getType() == EffectType.PLAY_SOUND ||
            effect.getType() == EffectType.SPEAK_CHAR || effect.getType() == EffectType.SPEAK_PLAYER ||
            effect.getType() == EffectType.SHOW_TEXT)
        {
            AllElementsWithAssets.addAsset(effect);
        }
        else if (effect.getType() == EffectType.RANDOM_EFFECT)
        {
            if (((RandomEffect) effect).getPositiveEffect() != null)
            {
                EffectType peType = ((RandomEffect) effect).getPositiveEffect().getType();
                if (peType == EffectType.PLAY_ANIMATION || peType == EffectType.PLAY_SOUND ||
                    peType == EffectType.SPEAK_CHAR || peType == EffectType.SPEAK_PLAYER ||
                    peType == EffectType.SHOW_TEXT)
                {
                    AllElementsWithAssets.addAsset(((RandomEffect) effect).getPositiveEffect());
                }
            }
            if (((RandomEffect) effect).getNegativeEffect() != null)
            {
                EffectType neType = ((RandomEffect) effect).getNegativeEffect().getType();
                if (neType == EffectType.PLAY_ANIMATION || neType == EffectType.PLAY_SOUND ||
                    neType == EffectType.SPEAK_CHAR || neType == EffectType.SPEAK_PLAYER ||
                    neType == EffectType.SHOW_TEXT)
                {
                    AllElementsWithAssets.addAsset(((RandomEffect) effect).getNegativeEffect());
                }
            }
        }

    }

    /**
     * Returns the contained list of effects
     * 
     * @return List of effects
     */

    public List<AbstractEffect> getEffects()
    {

        return effects;
    }

    /**
     * Checks if there is any cancel action effect in the list
     */

    public bool hasCancelAction()
    {

        bool hasCancelAction = false;
        foreach (Effect effect in effects)
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

        Effects e = (Effects) this.MemberwiseClone();
        if (effects != null)
        {
            e.effects = new List<AbstractEffect>();
            //TODO: check if its working
            foreach (AbstractEffect ef in effects)
                e.effects.Add((AbstractEffect) ef.Clone());
        }
        return e;
    }

    /*
@Override
public Object clone() throws CloneNotSupportedException
{

   Effects e = (Effects) super.clone( );
   if( effects != null ) {
       e.effects = new List<AbstractEffect>();
       for (Effect ef : effects)
           e.effects.add((AbstractEffect)ef.clone());
   }
   return e;
}*/
}
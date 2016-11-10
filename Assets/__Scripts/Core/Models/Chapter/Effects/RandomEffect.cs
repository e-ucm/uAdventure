using UnityEngine;
using System.Collections;

/**
 * Data representation of a random effect. According to a probability defined by
 * user, the system chooses between two effects which is to be launched So the
 * behaviour is: PROBABILITY times POSITIVE EFFECT is triggered 100-PROBABILITY
 * times NEGATIVE EFFECT is triggered
 */
public class RandomEffect : AbstractEffect
{

    /**
         * Effect to be triggered PROBABILITY% of the times
         */
    private AbstractEffect positiveEffect;

    /**
     * Effect to be triggered 100-PROBABILITY% of the times
     */
    private AbstractEffect negativeEffect;

    /**
     * Probability in range 0%-100%
     */
    private int probability;

    /**
     * Constructor
     * 
     * @param probability
     */
    public RandomEffect(int probability):base()
    {
        this.probability = probability;
    }

    /**
     * Default constructor. Sets probability to 50%
     */
    public RandomEffect() : this (50)
    {
    }

    public override EffectType getType()
    {

        return EffectType.RANDOM_EFFECT;
    }

    /**
     * @param positiveEffect
     *            the positiveEffect to set
     */
    public void setPositiveEffect(AbstractEffect positiveEffect)
    {

        this.positiveEffect = positiveEffect;
    }

    /**
     * @param negativeEffect
     *            the negativeEffect to set
     */
    public void setNegativeEffect(AbstractEffect negativeEffect)
    {

        this.negativeEffect = negativeEffect;
    }

    /**
     * @return the probability
     */
    public int getProbability()
    {

        return probability;
    }

    /**
     * @param probability
     *            the probability to set
     */
    public void setProbability(int probability)
    {

        this.probability = probability;
    }

    /**
     * @return the positiveEffect
     */
    public AbstractEffect getPositiveEffect()
    {

        return positiveEffect;
    }

    /**
     * @return the negativeEffect
     */
    public AbstractEffect getNegativeEffect()
    {

        return negativeEffect;
    }
    /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

        RandomEffect re = (RandomEffect) super.clone( );
        re.negativeEffect = ( negativeEffect != null ? (AbstractEffect) negativeEffect.clone( ) : null );
        re.positiveEffect = ( positiveEffect != null ? (AbstractEffect) positiveEffect.clone( ) : null );
        re.probability = probability;
        return re;
    }*/
}

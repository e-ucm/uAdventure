using System;
using UnityEngine;
using System.Collections;

public abstract class AbstractEffect: Effect, ICloneable
{

    private Conditions conditions;

    public AbstractEffect()
    {

        conditions = new Conditions();
    }

    /**
     * @return the conditions
     */
    public Conditions getConditions()
    {

        return conditions;
    }

    /**
     * @param conditions
     *            the conditions to set
     */
    public void setConditions(Conditions conditions)
    {

        this.conditions = conditions;
    }

    /**
     * Returns the type of the effect.
     * 
     * @return Type of the effect
     */
    public abstract EffectType getType();

    public virtual object Clone()
    {
        AbstractEffect absEf = (AbstractEffect)this.MemberwiseClone();
        absEf.conditions = (Conditions)conditions.Clone();
        return absEf;
    }
    /*
@Override
public Object clone() throws CloneNotSupportedException
{

   AbstractEffect absEf = (AbstractEffect) super.clone( );
   absEf.conditions = (Conditions) conditions.clone( );
  return absEf;
}*/
}

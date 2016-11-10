using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class holds a list of conditions
 */
public class Conditions:ICloneable
{


    /**
     * The list of conditions
     */
    private List<List<Condition>> conditionsList;

    /**
     * Create a new Conditions
     */
    public Conditions()
    {

        conditionsList = new List<List<Condition>>();
    }

    /**
     * Adds a new condition to the list
     * 
     * @param condition
     *            the condition to add
     */
    public void add(int index, Condition condition)
    {

        List<Condition> newBlock = new List<Condition>();
        newBlock.Add(condition);
        conditionsList.Insert(index, newBlock);
    }

    /**
     * Adds a new condition to the list
     * 
     * @param condition
     *            the condition to add
     */
    public void add(Condition condition)
    {

        List<Condition> newBlock = new List<Condition>();
        newBlock.Add(condition);
        conditionsList.Add(newBlock);
    }

    /**
     * Adds a list of conditions, such that at least one of these must be ok
     * 
     * @param conditions
     *            the conditions to add
     */
    public void add(Conditions conditions)
    {

        conditionsList.Add(conditions.getSimpleConditions());
    }

    /**
     * Inserts a list of conditions in the given position
     * 
     * @param conditions
     *            the conditions to add
     * @param index
     *            the index where conditions must be inserted
     */
    public void add(int index, Conditions conditions)
    {

        conditionsList.Insert(index, conditions.getSimpleConditions());
    }

    /**
     * Inserts a list of conditions in the given position
     * 
     * @param conditions
     *            the conditions to add
     * @param index
     *            the index where conditions must be inserted
     */
    public void add(int index, List<Condition> conditions)
    {

        conditionsList.Insert(index, conditions);
    }

    /**
     * Inserts a list of conditions in the given position
     * 
     * @param conditions
     *            the conditions to add
     * @param index
     *            the index where conditions must be inserted
     */
    public void add(List<Condition> conditions)
    {

        conditionsList.Add(conditions);
    }

    /**
     * Returns whether the conditions block is empty or not.
     * 
     * @return True if the block has no conditions, false otherwise
     */
    public bool isEmpty()
    {

        return conditionsList.Count == 0;
    }

    /**
     * Deletes the given either conditions block.
     * 
     * @param index
     *            Index of the either conditions block
     */
    public List<Condition> delete(int index)
    {
        List<Condition> item = conditionsList[index];
        conditionsList.RemoveAt(index);
        return item;
    }

    /**
     * Returns a list with all the simple conditions of the block. All these
     * conditions must be evaluated with AND.
     * 
     * @return List of conditions
     */
    public List<Condition> getSimpleConditions()
    {

        List<Condition> conditions = new List<Condition>();
        foreach (List<Condition> wrapper in conditionsList)
        {
            if (wrapper.Count == 1)
            {
                conditions.Add(wrapper[0]);
            }
        }
        return conditions;
    }

    /**
     * Returns a list with all the either condition blocks. This method is only
     * held for past compatibility
     * 
     * @return List of conditions
     */
    private List<Conditions> getEitherConditions()
    {

        List<Conditions> conditions = new List<Conditions>();
        foreach (List<Condition> wrapper in conditionsList)
        {
            if (wrapper.Count > 1)
            {
                Conditions eitherBlock = new Conditions();
                foreach (Condition condition in wrapper)
                {
                    eitherBlock.add(condition);
                }
                conditions.Add(eitherBlock);
            }
        }
        return conditions;
    }

    /**
     * @return the conditionsList
     */
    public List<List<Condition>> getConditionsList()
    {

        return conditionsList;
    }

    /**
     * Return all global state ids for this controller
     * 
     * @return
     */
    public List<string> getGloblaStateIds()
    {

        List<string> conditions = new List<string>();
        foreach (List<Condition> wrapper in conditionsList)
        {
            foreach (Condition condition in wrapper)
            {
                if (condition.getType() == Condition.GLOBAL_STATE_CONDITION)
                    conditions.Add(condition.getId());
            }
        }
        return conditions;
    }

    /**
     * Returns the number of either conditions blocks present.
     * 
     * @return Count of either conditions blocks
     */
    public int getEitherConditionsBlockCount()
    {

        return getEitherConditions().Count;
    }

    /**
     * Returns the either block of conditions specified.
     * 
     * @param index
     *            Index of the either block of conditions
     * @return List of conditions
     */
    public List<Condition> getEitherConditions(int index)
    {

        return getEitherConditions()[index].getSimpleConditions();
    }

    /**
     * Returns the either block of conditions specified.
     * 
     * @param index
     *            Index of the either block of conditions
     * @return List of conditions
     */
    public Conditions getEitherBlock(int index)
    {

        return getEitherConditions()[index];
    }

    public List<Condition> get(int index)
    {

        if (index >= 0 && index < this.conditionsList.Count)
        {
            return conditionsList[index];
        }
        return null;
    }

    /* (non-Javadoc)
     * @see java.lang.Object#clone()
     */
    /*
   @Override
   public Object clone() throws CloneNotSupportedException
   {

       Conditions clone = (Conditions) super.clone( );
       clone.conditionsList = new List<List<Condition>>( );
       for( List<Condition> wrapper : this.conditionsList ) {
           List<Condition> wrapperClone = new List<Condition>();
   clone.add( wrapperClone );
           for( Condition condition : wrapper ) {
               wrapperClone.add( (Condition) condition.clone( ) );
           }
       }
       return clone;
   }*/

    public int size()
    {

        return conditionsList.Count;
    }

    public virtual object Clone()
    {
        Conditions clone = (Conditions)this.MemberwiseClone();
        clone.conditionsList = new List<List<Condition>>();
        foreach (List<Condition> wrapper in this.conditionsList)
        {
            List<Condition> wrapperClone = new List<Condition>();
            clone.add(wrapperClone);
            foreach (Condition condition in wrapper)
            {
                wrapperClone.Add((Condition)condition.Clone());
            }
        }
        return clone;
    }
}

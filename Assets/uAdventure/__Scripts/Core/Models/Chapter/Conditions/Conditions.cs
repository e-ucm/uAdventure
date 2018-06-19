using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace uAdventure.Core
{
    /**
     * This class holds a list of conditions
     */
    [Serializable]
    public class Conditions : ICloneable
    {
        [Serializable]
        protected class ConditionBlock : IList<Condition>
        {
            [SerializeField]
            protected List<Condition> conditions = new List<Condition>();

            public Condition this[int index] { get { return ((IList<Condition>)conditions)[index]; } set { ((IList<Condition>) conditions)[index] = value; } }

            public List<Condition> Conditions { get { return conditions; } set { conditions = value; } }

            public int Count { get { return ((IList<Condition>)conditions).Count; } }

            public bool IsReadOnly { get { return ((IList<Condition>)conditions).IsReadOnly; } }

            public void Add(Condition item)
            {
                ((IList<Condition>)conditions).Add(item);
            }

            public void Clear()
            {
                ((IList<Condition>)conditions).Clear();
            }

            public bool Contains(Condition item)
            {
                return ((IList<Condition>)conditions).Contains(item);
            }

            public void CopyTo(Condition[] array, int arrayIndex)
            {
                ((IList<Condition>)conditions).CopyTo(array, arrayIndex);
            }

            public IEnumerator<Condition> GetEnumerator()
            {
                return ((IList<Condition>)conditions).GetEnumerator();
            }

            public int IndexOf(Condition item)
            {
                return ((IList<Condition>)conditions).IndexOf(item);
            }

            public void Insert(int index, Condition item)
            {
                ((IList<Condition>)conditions).Insert(index, item);
            }

            public bool Remove(Condition item)
            {
                return ((IList<Condition>)conditions).Remove(item);
            }

            public void RemoveAt(int index)
            {
                ((IList<Condition>)conditions).RemoveAt(index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IList<Condition>)conditions).GetEnumerator();
            }
        }

        /**
         * The list of conditions
         */

        [SerializeField]
        private List<ConditionBlock> conditionsList;

        /**
         * Create a new Conditions
         */
        public Conditions()
        {
            conditionsList = new List<ConditionBlock>();
        }

        /**
         * Adds a new condition to the list
         * 
         * @param condition
         *            the condition to add
         */
        public void Add(int index, Condition condition)
        {
            conditionsList.Insert(index, new ConditionBlock { condition });
        }

        /**
         * Adds a new condition to the list
         * 
         * @param condition
         *            the condition to add
         */
        public void Add(Condition condition)
        {
            conditionsList.Add(new ConditionBlock { condition });
        }

        /**
         * Adds a list of conditions, such that at least one of these must be ok
         * 
         * @param conditions
         *            the conditions to add
         */
        public void Add(Conditions conditions)
        {
            conditionsList.Add(new ConditionBlock()
            {
                Conditions = conditions.GetSimpleConditions()
            });
        }

        /**
         * Inserts a list of conditions in the given position
         * 
         * @param conditions
         *            the conditions to add
         * @param index
         *            the index where conditions must be inserted
         */
        public void Add(int index, Conditions conditions)
        {
            conditionsList.Insert(index, new ConditionBlock()
            {
                Conditions = conditions.GetSimpleConditions()
            });
        }

        /**
         * Inserts a list of conditions in the given position
         * 
         * @param conditions
         *            the conditions to add
         * @param index
         *            the index where conditions must be inserted
         */
        public void Add(int index, List<Condition> conditions)
        {
            conditionsList.Insert(index, new ConditionBlock()
            {
                Conditions = conditions
            });
        }

        /**
         * Inserts a list of conditions in the given position
         * 
         * @param conditions
         *            the conditions to add
         * @param index
         *            the index where conditions must be inserted
         */
        public void Add(List<Condition> conditions)
        {
            conditionsList.Add(new ConditionBlock()
            {
                Conditions = conditions
            });
        }

        /**
         * Returns whether the conditions block is empty or not.
         * 
         * @return True if the block has no conditions, false otherwise
         */
        public bool IsEmpty()
        {
            return conditionsList.Count == 0;
        }

        /**
         * Deletes the given either conditions block.
         * 
         * @param index
         *            Index of the either conditions block
         */
        public List<Condition> Delete(int index)
        {
            ConditionBlock item = conditionsList[index];
            conditionsList.RemoveAt(index);
            return item.Conditions;
        }

        /**
         * Returns a list with all the simple conditions of the block. All these
         * conditions must be evaluated with AND.
         * 
         * @return List of conditions
         */
        public List<Condition> GetSimpleConditions()
        {
            List<Condition> conditions = new List<Condition>();
            foreach (ConditionBlock conditionBlock in conditionsList)
            {
                if (conditionBlock.Count == 1)
                {
                    conditions.Add(conditionBlock[0]);
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
        private List<Conditions> GetEitherConditions()
        {

            List<Conditions> conditions = new List<Conditions>();
            foreach (ConditionBlock conditionBlock in conditionsList)
            {
                if (conditionBlock.Count > 1)
                {
                    Conditions eitherBlock = new Conditions();
                    foreach (Condition condition in conditionBlock)
                    {
                        eitherBlock.Add(condition);
                    }
                    conditions.Add(eitherBlock);
                }
            }
            return conditions;
        }

        /**
         * @return the conditionsList
         */
        public List<List<Condition>> GetConditionsList()
        {
            return conditionsList.ConvertAll(conditionBlock => new List<Condition>(conditionBlock));
        }

        /**
         * Return all global state ids for this controller
         * 
         * @return
         */
        public List<string> GetGloblalStateIds()
        {
            List<string> conditions = new List<string>();
            foreach (ConditionBlock conditionBlock in conditionsList)
            {
                foreach (Condition condition in conditionBlock)
                {
                    if (condition.getType() == Condition.GLOBAL_STATE_CONDITION)
                    {
                        conditions.Add(condition.getId());
                    }
                }
            }
            return conditions;
        }

        /**
         * Returns the number of either conditions blocks present.
         * 
         * @return Count of either conditions blocks
         */
        public int GetEitherConditionsBlockCount()
        {
            return GetEitherConditions().Count;
        }

        /**
         * Returns the either block of conditions specified.
         * 
         * @param index
         *            Index of the either block of conditions
         * @return List of conditions
         */
        public List<Condition> GetEitherConditions(int index)
        {
            return GetEitherConditions()[index].GetSimpleConditions();
        }

        /**
         * Returns the either block of conditions specified.
         * 
         * @param index
         *            Index of the either block of conditions
         * @return List of conditions
         */
        public Conditions GetEitherBlock(int index)
        {
            return GetEitherConditions()[index];
        }

        public IList<Condition> Get(int index)
        {
            if (index >= 0 && index < this.conditionsList.Count)
            {
                return conditionsList[index];
            }
            return null;
        }

        public int Size()
        {
            return conditionsList.Count;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<string> andList = new List<string>();
            List<string> orList = new List<string>();
            
            foreach(var block in conditionsList)
            {
                if (block.Count > 1) sb.Append("(");
                orList.Clear();
                foreach (var c in block)
                {
                    orList.Add(c.ToString());
                }
                sb.Append(string.Join(" or ", orList.ToArray()));
                if (block.Count > 1)
                {
                    sb.Append(")");
                }
                andList.Add(sb.ToString());
                sb.Length = 0;
            }
            return sb.Append("if ").Append(string.Join(" and ", andList.ToArray())).Append(" then").ToString();
        }

        public virtual object Clone()
        {
            Conditions clone = (Conditions)this.MemberwiseClone();
            clone.conditionsList = new List<ConditionBlock>();
            foreach (var conditionBlock in this.conditionsList)
            {
                List<Condition> wrapperClone = new List<Condition>();
                clone.Add(wrapperClone);
                foreach (Condition condition in conditionBlock)
                {
                    wrapperClone.Add((Condition)condition.Clone());
                }
            }
            return clone;
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Text;

namespace uAdventure.Core
{
    /**
     * Specific class for a Var-based Condition
     * 
     * @author Javier
     * 
     */
    [Serializable]
    public class VarCondition : Condition, ICloneable
    {

        /**
        * Constant for greater-than var.
        */
        public const int VAR_GREATER_THAN = 2;
        public static int GetVAR_GREATER_THAN() { return VAR_GREATER_THAN; }

        /**
         * Constant for greater-than or equals var.
         */
        public const int VAR_GREATER_EQUALS_THAN = 3;
        public static int GetVAR_GREATER_EQUALS_THAN() { return VAR_GREATER_EQUALS_THAN; }

        /**
         * Constant for equals var.
         */
        public const int VAR_EQUALS = 4;
        public static int GetVAR_EQUALS() { return VAR_EQUALS; }

        /**
         * Constant for less than or equals var.
         */
        public const int VAR_LESS_EQUALS_THAN = 5;
        public static int GetVAR_LESS_EQUALS_THAN() { return VAR_LESS_EQUALS_THAN; }

        /**
         * Constant for less-than var.
         */
        public const int VAR_LESS_THAN = 6;
        public static int GetVAR_LESS_THAN() { return VAR_LESS_THAN; }

        /**
         * Constant for not-equals var.
         */
        public const int VAR_NOT_EQUALS = 7;
        public static int GetVAR_NOT_EQUALS() { return VAR_NOT_EQUALS; }

        /**
         * MIN VALUE
         */
        public const int MIN_VALUE = 0;
        public static int GetMIN_VALUE() { return MIN_VALUE; }

        /**
         * MAX VALUE
         */
        public const int MAX_VALUE = int.MaxValue;
        public static int GetMAX_VALUE() { return MAX_VALUE; }

        /**
         * The value of the var-condition
         */
        [SerializeField]
        private int value;

        /**
         * Constructor
         * 
         * @param flagVar
         * @param state
         */

        public VarCondition(string flagVar, int state, int value) : base(VAR_CONDITION, flagVar, state)
        {
            this.value = value;
        }

        /**
         * @return the value
         */
        public int getValue()
        {

            return value;
        }

        /**
         * @param value
         *            the value to set
         */
        public void setValue(int value)
        {

            this.value = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(getId());
            switch (this.state)
            {
                case VAR_GREATER_THAN:
                    sb.Append(" > ");
                    break;
                case VAR_GREATER_EQUALS_THAN:
                    sb.Append(" >= ");
                    break;
                case VAR_EQUALS:
                    sb.Append(" == ");
                    break;
                case VAR_LESS_EQUALS_THAN:
                    sb.Append(" <= ");
                    break;
                case VAR_LESS_THAN:
                    sb.Append(" < ");
                    break;
                case VAR_NOT_EQUALS:
                    sb.Append(" != ");
                    break;
            }

            return sb.Append(value).ToString();
        }
        public override object Clone()
        {
            VarCondition vc = (VarCondition)base.Clone();
            vc.id = (id != null ? id : null);
            vc.state = state;
            vc.type = type;
            vc.value = value;
            return vc;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            VarCondition vc = (VarCondition) super.clone( );
            vc.id = ( id != null ? new String(id ) : null );
            vc.state = state;
            vc.type = type;
            vc.value = value;
            return vc;
        }*/
    }
}
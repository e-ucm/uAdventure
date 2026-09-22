using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
    * Stores the adaptation data, which includes the flag activation and
    * deactivation values, along with the initial scene of the game
*/

    public class AdaptedState : ICloneable
    {


        /**
         * Id of the initial scene
         */
        private string initialScene;

        /**
         * Flags values
         */
        public const string ACTIVATE = "activate";

        public const string DEACTIVATE = "deactivate";

        /**
         * Vars values
         */
        public const string INCREMENT = "increment";

        public const string DECREMENT = "decrement";

        public const string VALUE = "set-value";

        /**
         * List of all flags and vars (in order)
         */
        private List<string> allFlagsVars;

        /**
         * List of deactivate/activate for flags (and value for vars)
         */
        private List<string> actionsValues;

        /**
         * Constructor
         */

        public AdaptedState()
        {

            initialScene = null;
            allFlagsVars = new List<string>();
            actionsValues = new List<string>();

        }

        /**
         * Returns the id of the initial scene
         * 
         * @return Id of the initial scene, null if none
         */

        public string getTargetId()
        {

            return initialScene;
        }

        /**
         * Returns the list of flags and vars
         * 
         * @return List of the deactivated flags
         */

        public List<string> getFlagsVars()
        {

            return allFlagsVars;
        }

        /**
         * Sets the initial scene id
         * 
         * @param initialScene
         *            Id of the initial scene
         */

        public void setTargetId(string initialScene)
        {

            this.initialScene = initialScene;
        }

        /**
         * Adds a new flag to be activated
         * 
         * @param flag
         *            Name of the flag
         */

        public void addActivatedFlag(string flag)
        {

            allFlagsVars.Add(flag);
            actionsValues.Add(ACTIVATE);

        }

        /**
         * Adds a new flag to be deactivated
         * 
         * @param flag
         *            Name of the flag
         */

        public void addDeactivatedFlag(string flag)
        {

            allFlagsVars.Add(flag);
            actionsValues.Add(DEACTIVATE);

        }

        /**
         * Adds a new var
         * 
         * @param var
         * @param value
         */

        public void addVarValue(string var, string value)
        {

            allFlagsVars.Add(var);
            actionsValues.Add(value);
        }

        public void removeFlagVar(int row)
        {

            allFlagsVars.RemoveAt(row);
            actionsValues.RemoveAt(row);
        }

        public void changeFlag(int row, string flag)
        {

            int nFlags = actionsValues.Count;
            allFlagsVars.RemoveAt(row);
            if (row < nFlags - 1)
                allFlagsVars.Insert(row, flag);
            else
                allFlagsVars.Add(flag);

        }

        public void changeAction(int row)
        {

            int nFlags = actionsValues.Count;
            if (actionsValues[row].Equals(ACTIVATE))
            {
                actionsValues.RemoveAt(row);

                if (row < nFlags - 1)
                    actionsValues.Insert(row, DEACTIVATE);
                else
                    actionsValues.Add(DEACTIVATE);
            }

            else if (actionsValues[row].Equals(DEACTIVATE))
            {
                actionsValues.RemoveAt(row);

                if (row < nFlags - 1)
                    actionsValues.Insert(row, ACTIVATE);
                else
                    actionsValues.Add(ACTIVATE);
            }

        }

        /**
         * Change the type of "index" position. If in that position there was a
         * flag, change it to var and vice versa. It change and set the new flag or
         * var to default value.
         * 
         * @param index
         *            the position where the change will take place
         * @param name
         *            the name of the new flag or var
         */

        public void change(int index, string name)
        {

            if (isFlag(index))
            {
                actionsValues[index] = INCREMENT + " 0";
                allFlagsVars[index] = name;
            }
            else
            {
                actionsValues[index] = ACTIVATE;
                allFlagsVars[index] = name;
            }
        }

        public void changeAction(int row, string newValue)
        {

            if (row >= 0 && row <= actionsValues.Count)
            {
                actionsValues.RemoveAt(row);
                actionsValues.Insert(row, newValue);
            }
        }

        public string getAction(int i)
        {

            return actionsValues[i];
        }

        public string getValueForVar(int i)
        {

            //string val = actionsValues[i];
            //if (val.contains(VALUE)){
            return getValueToSet(i).ToString();
            //}else {
            //  return null;
            //}
        }

        /**
         * Returns the value for "VALUE" action (the value which will be set to
         * associated var)
         * 
         * @param index
         * @return
         */

        public int getValueToSet(int index)
        {

            string val = actionsValues[index];
            int subIndex = val.IndexOf(" ");
            if (subIndex != -1)
            {
                val = val.Substring(subIndex + 1);
                return int.Parse(val);
            }
            else
            {
                return int.MinValue;
            }

        }

        public int getValue(int i)
        {

            return int.Parse(actionsValues[i]);
        }

        public string getFlagVar(int i)
        {

            return allFlagsVars[i];
        }

        public bool isEmpty()
        {

            return allFlagsVars.Count == 0 && (initialScene == null || initialScene.Equals(""));
        }

        /**
         * Returns the list of the activated flags
         * 
         * @return List of the activated flags
         */

        public List<string> getActivatedFlags()
        {

            List<string> activatedFlags = new List<string>();
            for (int i = 0; i < actionsValues.Count; i++)
            {
                if (actionsValues[i].Equals(ACTIVATE))
                {
                    activatedFlags.Add(allFlagsVars[i]);
                }
            }
            return activatedFlags;
        }

        /**
         * Returns the list of the deactivated flags
         * 
         * @return List of the deactivated flags
         */

        public List<string> getDeactivatedFlags()
        {

            List<string> deactivatedFlags = new List<string>();
            for (int i = 0; i < actionsValues.Count; i++)
            {
                if (actionsValues[i].Equals(DEACTIVATE))
                {
                    deactivatedFlags.Add(allFlagsVars[i]);
                }
            }
            return deactivatedFlags;
        }

        /**
         * Fills the argumented structures with the names of the vars and the values
         * they must be set with
         * 
         * @param vars
         * @param values
         */

        public void getVarsValues(List<string> vars, List<string> values)
        {

            for (int i = 0; i < actionsValues.Count; i++)
            {
                if (!actionsValues[i].Equals(ACTIVATE) && !actionsValues[i].Equals(DEACTIVATE))
                {
                    vars.Add(allFlagsVars[i]);
                    values.Add(actionsValues[i]);
                }
            }
        }

        /**
         * Joins two Adapted States. The new resulting adapted state has a merge of
         * active/inactive flags of both states, and the initial scene will be set
         * as the initial scene of the parameter state. With the vars, its do the
         * same action. The new flags/vars will be add at the end of the data
         * structure;
         * 
         * @param AdaptedState
         *            mergeState The state which will be merged with the current
         *            object
         * 
         */

        public void merge(AdaptedState mergeState)
        {

            if (mergeState.initialScene != null)
                this.initialScene = mergeState.initialScene;
            if (this.allFlagsVars.Count == 0)
            {
                this.allFlagsVars = mergeState.allFlagsVars;
                this.actionsValues = mergeState.actionsValues;
            }
            else
            {
                for (int i = 0; i < mergeState.allFlagsVars.Count; i++)
                {
                    this.allFlagsVars.Add(mergeState.allFlagsVars[i]);
                    this.actionsValues.Add(mergeState.allFlagsVars[i]);
                }
            }
        }

        /**
         * Check if the value in the given position is flag or variable.
         * 
         * @param index
         *            the position in allFlagsVar
         * @return true if the value in the given position has "flag" value.
         */

        public bool isFlag(int index)
        {

            string value = actionsValues[index];
            if (value.Equals(ACTIVATE) || value.Equals(DEACTIVATE))
                return true;
            else
                return false;
        }

        /**
         * Check if the value with the given name is flag or variable.
         * 
         * @param name
         *            the name of the var or flag
         * @return true, if name is flag
         */

        public bool isFlag(string name)
        {

            bool isFlag = false;
            for (int i = 0; i < allFlagsVars.Count; i++)
            {
                if (allFlagsVars[i].Equals(name))
                {
                    if (this.isFlag(i))
                        return true;
                    else
                        return false;
                }
            }
            return isFlag;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            AdaptedState as = (AdaptedState) super.clone( );
            as.actionsValues = new List<string>( );
            for( string s : actionsValues )
                as.actionsValues.add( ( s != null ? new string(s ) : null ) );
            as.allFlagsVars = new List<string>( );
            for( string s : allFlagsVars )
                as.allFlagsVars.add( ( s != null ? new string(s ) : null ) );
            as.initialScene = ( initialScene != null ? new string(initialScene ) : null );
            return as;
        }*/

        /**
         * @return the actionsValues
         */

        public List<string> getActionsValues()
        {

            return actionsValues;
        }


        /**
         * Check if the given string is a "activate" operation
         * 
         * @param op
         * @return
         */

        public static bool isActivateOp(string op)
        {

            return op.Contains(ACTIVATE);
        }

        /**
         * Check if the given string is a "deactivate" operation
         * 
         * @param op
         * @return
         */

        public static bool isDeactivateOp(string op)
        {

            return op.Contains(DEACTIVATE);
        }

        /**
         * Check if the given string is a "set-value" operation
         * 
         * @param op
         * @return
         */

        public static bool isSetValueOp(string op)
        {

            return op.Contains(VALUE);
        }

        /**
         * Check if the given string is a "increment" operation
         * 
         * @param op
         * @return
         */

        public static bool isIncrementOp(string op)
        {

            return op.Contains(INCREMENT);
        }

        /**
         * Check if the given string is a "decrement" operation
         * 
         * @param op
         * @return
         */

        public static bool isDecrementOp(string op)
        {

            return op.Contains(DECREMENT);
        }

        /**
         * Take the numeric value from a "action value" that will be set as var
         * value
         * 
         * @param value
         * @return
         */

        public static string getSetValueData(string value)
        {

            int subIndex = value.IndexOf(" ");
            if (subIndex != -1)
            {
                value = value.Substring(subIndex + 1);
                return value;
            }
            else
            {
                return null;
            }
        }

        public virtual object Clone()
        {
            AdaptedState ass = (AdaptedState)this.MemberwiseClone();
            ass.actionsValues = new List<string>();
            foreach (string s in actionsValues)
                ass.actionsValues.Add((s != null ? s : null));
            ass.allFlagsVars = new List<string>();
            foreach (string s in allFlagsVars)
                ass.allFlagsVars.Add((s != null ? s : null));
            ass.initialScene = (initialScene != null ? initialScene : null);
            return ass;
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the name of all the active flags in the script.
     */

    public class VarFlagSummary
    {

        /**
         * List of flags.
         */
        private List<string> flags;

        /**
         * List of flag references.
         */
        private List<int> flagReferences;

        /**
         * List of vars.
         */
        private List<string> vars;

        /**
         * List of var references.
         */
        private List<int> varReferences;

        /**
         * Constructor.
         */

        public VarFlagSummary()
        {

            // Create the lists
            flags = new List<string>();
            flagReferences = new List<int>();
            vars = new List<string>();
            varReferences = new List<int>();
        }

        /**
         * Clears the summary, deleting all references.
         */

        public void clearReferences()
        {

            // Clear both lists
            //flags.clear( );
            flagReferences.Clear();
            for (int i = 0; i < flags.Count; i++)
            {
                flagReferences.Add(0);
            }
            //vars.clear( );
            varReferences.Clear();
            for (int i = 0; i < vars.Count; i++)
            {
                varReferences.Add(0);
            }
        }

        /**
         * Deletes all var and flags that have 0 references
         */

        public void clean()
        {
            for (int i = 0; i < flagReferences.Count; i++)
            {
                if (flagReferences[i] == 0)
                {
                    flags.RemoveAt(i);
                }
            }

            while (flagReferences.Contains(0))
            {
                flagReferences.RemoveAt(0);
            }

            for (int i = 0; i < varReferences.Count; i++)
            {
                if (varReferences[i] == 0)
                {
                    vars.RemoveAt(i);
                }
            }

            while (varReferences.Contains(0))
            {
                varReferences.RemoveAt(0);
            }
        }

        /**
         * Adds a new flag to the list (with zero references).
         * 
         * @param flag
         *            New flag
         * @return True if the flag was added, false otherwise
         */

        public bool addFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
            {
                return false;
            }

            bool addedFlag = false;

            // Add it only if it doesn't exist
            if (!existsFlag(flag))
            {
                flags.Add(flag);
                flagReferences.Add(0);
                addedFlag = true;

                // Sort the list
                sortList(flags, flagReferences);
            }

            return addedFlag;
        }

        /**
         * Deletes the given flag from the list.
         * 
         * @param flag
         *            Flag to be deleted
         * @return True if the flag was deleted, false otherwise
         */

        public bool deleteFlag(string flag)
        {

            bool deletedFlag = false;

            // Get the index of the flag
            int flagIndex = flags.IndexOf(flag);

            // If the flag exists, delete the info
            if (flagIndex >= 0)
            {
                flags.RemoveAt(flagIndex);
                flagReferences.RemoveAt(flagIndex);
                deletedFlag = true;
            }

            return deletedFlag;
        }

        /**
         * Adds a new var to the list (with zero references).
         * 
         * @param flag
         *            New var
         * @return True if the var was added, false otherwise
         */

        public bool addVar(string var)
        {
            if (string.IsNullOrEmpty(var))
            {
                return false;
            }

            bool addedVar = false;

            // Add it only if it doesn't exist
            if (!existsFlag(var))
            {
                vars.Add(var);
                varReferences.Add(0);
                addedVar = true;

                // Sort the list
                sortList(vars, varReferences);
            }

            return addedVar;
        }

        /**
         * Deletes the given var from the list.
         * 
         * @param var
         *            Var to be deleted
         * @return True if the var was deleted, false otherwise
         */

        public bool deleteVar(string var)
        {

            bool deletedVar = false;

            // Get the index of the flag
            int varIndex = vars.IndexOf(var);

            // If the var exists, delete the info
            if (varIndex >= 0)
            {
                vars.RemoveAt(varIndex);
                varReferences.RemoveAt(varIndex);
                deletedVar = true;
            }

            return deletedVar;
        }

        /**
         * Adds a new reference (if the id provided is a flag addFlagReference is
         * invoked, if the id provided is a var addVarReference is called).
         * 
         * @param id
         *            New ref id
         */

        public void addReference(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (flags.Contains(id))
            {
                addFlagReference(id);
            }
            else if (vars.Contains(id))
            {
                addVarReference(id);
            }
        }

        /**
         * Adds a new flag reference (creates the flag with one reference, or
         * updates the references).
         * 
         * @param flag
         *            New flag
         */

        public void addFlagReference(string flag)
        {
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            // Get the index of the flag
            int flagIndex = flags.IndexOf(flag);

            // If the flag was on the list, update the references
            if (flagIndex >= 0)
            {
                int references = flagReferences[flagIndex] + 1;
                flagReferences[flagIndex] = references;
            }

            // If the flag wasn't on the list, add it
            else
            {
                flags.Add(flag);
                flagReferences.Add(1);

                // Sort the list
                sortList(flags, flagReferences);
            }
        }

        /**
         * Deletes the given flag from the list
         * 
         * @param flag
         *            Flag to be deleted
         */

        public void deleteFlagReference(string flag)
        {
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            // Get the index of the flag
            int flagIndex = flags.IndexOf(flag);

            // If the flag is on the list
            if (flagIndex >= 0)
            {
                // Get the number of references, decrease it and update
                int references = flagReferences[flagIndex] - 1;
                flagReferences[flagIndex] = references;
            }

            // If it is not, show an error message
            else
                Debug.LogError("Error: Trying to delete a nonexistent flag");
        }

        /**
         * Deletes the given if (either flag or var) from the list
         * 
         * @param id
         *            Id to be deleted
         */

        public void deleteReference(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (flags.Contains(id))
            {
                deleteFlagReference(id);
            }
            else if (vars.Contains(id))
            {
                deleteVarReference(id);
            }
        }

        /**
         * Adds a new var reference (creates the var with one reference, or updates
         * the references).
         * 
         * @param var
         *            New var
         */

        public void addVarReference(string var)
        {
            if (string.IsNullOrEmpty(var))
            {
                return;
            }

            // Get the index of the var
            int varIndex = vars.IndexOf(var);

            // If the var was on the list, update the references
            if (varIndex >= 0)
            {
                int references = varReferences[varIndex] + 1;
                varReferences[varIndex] = references;
            }

            // If the var wasn't on the list, add it
            else
            {
                vars.Add(var);
                varReferences.Add(1);

                // Sort the list
                sortList(vars, varReferences);
            }
        }

        /**
         * Deletes the given var from the list
         * 
         * @param var
         *            Var to be deleted
         */

        public void deleteVarReference(string var)
        {
            if (string.IsNullOrEmpty(var))
            {
                return;
            }

            // Get the index of the var
            int varIndex = vars.IndexOf(var);

            // If the var is on the list
            if (varIndex >= 0)
            {
                // Get the number of references, decrease it and update
                int references = varReferences[varIndex] - 1;
                varReferences[varIndex] = references;
            }

            // If it is not, show an error message
            else
                Debug.LogError("Error: Trying to delete a nonexistent var");
        }

        /**
         * Returns if the flag summary contains the given flag.
         * 
         * @param flag
         *            Flag to be checked
         * @return True if the list contains the flag, false otherwise
         */

        public bool existsFlag(string flag)
        {

            return flags.Contains(flag);
        }

        /**
         * Returns if the var summary contains the given var.
         * 
         * @param var
         *            Var to be checked
         * @return True if the list contains the var, false otherwise
         */

        public bool existsVar(string var)
        {

            return vars.Contains(var);
        }

        /**
         * Returns if the summary contains the given id (both flags & vars are
         * checked).
         * 
         * @param id
         *            Id to be checked
         * @return True if some of the lists contain the id, false otherwise
         */

        public bool existsId(string id)
        {

            return existsFlag(id) || existsVar(id);
        }

        /**
         * Returns the number of flags present in the summary.
         * 
         * @return Number of flags
         */

        public int getFlagCount()
        {

            return flags.Count;
        }

        /**
         * Returns the number of varss present in the summary.
         * 
         * @return Number of vars
         */

        public int getVarCount()
        {

            return vars.Count;
        }

        /**
         * Returns the flag name in the given position.
         * 
         * @param index
         *            Index for the flag
         * @return Flag name
         */

        public string getFlag(int index)
        {

            return flags[index];
        }

        /**
         * Returns the var name in the given position.
         * 
         * @param index
         *            Index for the var
         * @return Var name
         */

        public string getVar(int index)
        {

            return vars[index];
        }

        /**
         * Returns the flag references number in the given position.
         * 
         * @param index
         *            Index for the flag
         * @return Number of references of the flag
         */

        public int getFlagReferences(int index)
        {

            return flagReferences[index];
        }

        /**
         * Returns the var references number in the given position.
         * 
         * @param index
         *            Index for the var
         * @return Number of references of the var
         */

        public int getVarReferences(int index)
        {

            return varReferences[index];
        }

        /**
         * Returns an array with all the flags of the list
         * 
         * @return Array with all the flags
         */

        public string[] getFlags()
        {

            return flags.ToArray();
        }

        /**
         * Returns an array with all the vars of the list
         * 
         * @return Array with all the vars
         */

        public string[] getVars()
        {

            return vars.ToArray();
        }

        //It is only used to show all the flags and vars in assessment profiles
        public string[] getVarsAndFlags()
        {
            List<string> mix = new List<string>();
            mix.AddRange(vars);
            mix.AddRange(flags);
            mix.Add("report");
            return mix.ToArray();
        }

        /**
         * Sorts the lists of flags and resources, by the name of the flags.
         */

        private void sortList(List<string> list, List<int> refsList)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < (list.Count - 1) - i; j++)
                {
                    // If the current flag is greater than the next one, swap values (flag and references)
                    if (list[j].CompareTo(list[j + 1]) > 0)
                    {
                        string s1 = list[j];
                        int s2 = refsList[j];
                        list.RemoveAt(j);
                        refsList.RemoveAt(j);
                        list.Insert(j + 1, s1);
                        refsList.Insert(j + 1, s2);
                    }
                }
            }

        }
    }
}
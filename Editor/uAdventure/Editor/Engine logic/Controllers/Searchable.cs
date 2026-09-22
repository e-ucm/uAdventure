using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class Searchable
    {


        protected static Dictionary<Searchable, List<string>> resultSet = new Dictionary<Searchable, List<string>>();

        protected static string searchedText;

        private static bool caseSensitive;

        private static bool fullMatch;

        public Dictionary<Searchable, List<string>> search(string text, bool caseSensitive, bool fullMatch)
        {

            resultSet.Clear();
            if (caseSensitive)
                Searchable.searchedText = text;
            else
                Searchable.searchedText = text.ToLower();
            Searchable.caseSensitive = caseSensitive;
            Searchable.fullMatch = fullMatch;
            this.recursiveSearch();
            return Searchable.resultSet;
        }

        public abstract void recursiveSearch();

        protected void addResult(string where)
        {

            List<string> places = resultSet[this];
            if (places == null)
            {
                places = new List<string>();
                resultSet.Add(this, places);
            }
            if (!places.Contains(where))
                places.Add(where);
        }

        protected void check(string value, string desc)
        {

            if (value != null)
            {
                if (!fullMatch)
                {
                    if (!caseSensitive && value.ToLower().Contains(searchedText))
                        addResult(desc);
                    else if (caseSensitive && value.Contains(searchedText))
                        addResult(desc);
                }
                else
                {
                    if (!caseSensitive && value.ToLower().Equals(searchedText))
                        addResult(desc);
                    else if (caseSensitive && value.Equals(searchedText))
                        addResult(desc);
                }
            }
        }

        protected void check(ConditionsController conditions, string desc)
        {

            for (int i = 0; i < conditions.getBlocksCount(); i++)
            {
                for (int j = 0; j < conditions.getConditionCount(i); j++)
                {
                    Dictionary<string, string> properties = conditions.getCondition(i, j);
                    if (properties.ContainsKey(ConditionsController.CONDITION_ID))
                        check(properties[ConditionsController.CONDITION_ID], desc + " (ID)");
                    if (properties.ContainsKey(ConditionsController.CONDITION_STATE))
                        // CHECK!!!
                        check(properties[ConditionsController.CONDITION_STATE], desc + " ()");
                    if (properties.ContainsKey(ConditionsController.CONDITION_VALUE))
                        // CHECK!!!
                        check(properties[ConditionsController.CONDITION_VALUE], desc + " ()");
                }
            }
        }

        protected abstract List<Searchable> getPath(Searchable dataControl);

        protected List<Searchable> getPathFromChild(Searchable dataControl, DataControl child)
        {

            if (child != null)
            {
                List<Searchable> path = child.getPath(dataControl);
                if (path != null)
                {
                    path.Add(this);
                    return path;
                }
            }
            return null;
        }

        protected List<Searchable> getPathFromChild(Searchable dataControl, List<System.Object> list)
        {

            foreach (System.Object temp in list)
            {
                List<Searchable> path = ((Searchable)temp).getPath(dataControl);
                if (path != null)
                {
                    path.Add(this);
                    return path;
                }
            }
            return null;
        }

        protected List<Searchable> getPathFromChild(Searchable dataControl, List<Searchable> list)
        {

            foreach (Searchable temp in list)
            {
                List<Searchable> path = temp.getPath(dataControl);
                if (path != null)
                {
                    path.Add(this);
                    return path;
                }
            }
            return null;
        }

        protected List<Searchable> getPathFromSearchableChild(Searchable dataControl, Searchable child)
        {

            if (child != null)
            {
                List<Searchable> path = child.getPath(dataControl);
                if (path != null)
                {
                    path.Add(this);
                    return path;
                }
            }
            return null;
        }

    }
}
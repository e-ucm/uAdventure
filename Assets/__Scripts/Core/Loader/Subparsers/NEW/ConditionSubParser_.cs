using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ConditionSubParser_ : Subparser_
{
    /**
   * Stores the conditions
   */
    private Conditions conditions;

    /**
     * Stores the current either conditions
     */
    private Conditions currentEitherCondition;


    public ConditionSubParser_(Conditions conditions, Chapter chapter) : base(chapter)
    {
        this.conditions = conditions;
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            eithers = element.SelectNodes("either"),
            actives = element.SelectNodes("active"),
            inactives = element.SelectNodes("inactive"),
            greatersthan = element.SelectNodes("greater-than"),
            greatersequalssthan = element.SelectNodes("greater-equals-than"),
            lesssthan = element.SelectNodes("less-than"),
            lesssequalssthan = element.SelectNodes("less-equals-than"),
            equalss = element.SelectNodes("equals"),
            notsequals = element.SelectNodes("not-equals"),
            globalsstatesref = element.SelectNodes("global-state-ref");

        string tmpArgVal;

        foreach (XmlElement el in eithers)
        {
            currentEitherCondition = new Conditions();

            //Embeded inside eithers
            XmlNodeList
                actives_e = el.SelectNodes("active"),
                inactives_e = el.SelectNodes("inactive"),
                greatersthan_e = el.SelectNodes("greater-than"),
                greatersequalssthan_e = el.SelectNodes("greater-equals-than"),
                lesssthan_e = el.SelectNodes("less-than"),
                lesssequalssthan_e = el.SelectNodes("less-equals-than"),
                equalss_e = el.SelectNodes("equals"),
                notsequals_e = el.SelectNodes("not-equals"),
                globalsstatesref_e = el.SelectNodes("global-state-ref");

            foreach (XmlElement ell in actives_e)
            {
                tmpArgVal = ell.GetAttribute("flag");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    currentEitherCondition.add(new FlagCondition(tmpArgVal, FlagCondition.FLAG_ACTIVE));
                    chapter.addFlag(tmpArgVal);
                }
            }

            foreach (XmlElement ell in inactives_e)
            {
                tmpArgVal = ell.GetAttribute("flag");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    currentEitherCondition.add(new FlagCondition(tmpArgVal, FlagCondition.FLAG_INACTIVE));
                    chapter.addFlag(tmpArgVal);
                }
            }

            foreach (XmlElement ell in greatersthan_e)
            {
                // The var
                string var = null;
                // The value
                int value = 0;

                tmpArgVal = ell.GetAttribute("var");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    var = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = int.Parse(tmpArgVal);
                }
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_GREATER_THAN, value));
                chapter.addVar(var);
            }

            foreach (XmlElement ell in greatersequalssthan_e)
            {
                // The var
                string var = null;
                // The value
                int value = 0;

                tmpArgVal = ell.GetAttribute("var");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    var = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = int.Parse(tmpArgVal);
                }
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_GREATER_EQUALS_THAN, value));
                chapter.addVar(var);
            }

            foreach (XmlElement ell in lesssthan_e)
            {
                // The var
                string var = null;
                // The value
                int value = 0;

                tmpArgVal = ell.GetAttribute("var");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    var = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = int.Parse(tmpArgVal);
                }
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_LESS_THAN, value));
                chapter.addVar(var);
            }

            foreach (XmlElement ell in lesssequalssthan_e)
            {
                // The var
                string var = null;
                // The value
                int value = 0;

                tmpArgVal = ell.GetAttribute("var");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    var = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = int.Parse(tmpArgVal);
                }
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_LESS_EQUALS_THAN, value));
                chapter.addVar(var);
            }

            foreach (XmlElement ell in equalss_e)
            {
                // The var
                string var = null;
                // The value
                int value = 0;

                tmpArgVal = ell.GetAttribute("var");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    var = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = int.Parse(tmpArgVal);
                }
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_EQUALS, value));
                chapter.addVar(var);
            }

            foreach (XmlElement ell in notsequals_e)
            {
                // The var
                string var = null;
                // The value
                int value = 0;

                tmpArgVal = ell.GetAttribute("var");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    var = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = int.Parse(tmpArgVal);
                }
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_NOT_EQUALS, value));
                chapter.addVar(var);
            }

            foreach (XmlElement ell in globalsstatesref_e)
            {
                // Id
                string id = null;
                // VALUE WAS ADDED IN EAD1.4 - It allows negating a global state
                bool value = true;

                tmpArgVal = ell.GetAttribute("id");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    id = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("value");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    value = tmpArgVal.ToLower().Equals("true");
                }
                currentEitherCondition.add(new GlobalStateCondition(id,
                    value ? GlobalStateCondition.GS_SATISFIED : GlobalStateCondition.GS_NOT_SATISFIED));
            }

            conditions.add(currentEitherCondition);
        }


        foreach (XmlElement el in actives)
        {
            tmpArgVal = el.GetAttribute("flag");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                conditions.add(new FlagCondition(tmpArgVal, FlagCondition.FLAG_ACTIVE));
                chapter.addFlag(tmpArgVal);
            }
        }

        foreach (XmlElement el in inactives)
        {
            tmpArgVal = el.GetAttribute("flag");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                conditions.add(new FlagCondition(tmpArgVal, FlagCondition.FLAG_INACTIVE));
                chapter.addFlag(tmpArgVal);
            }
        }

        foreach (XmlElement el in greatersthan)
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            tmpArgVal = el.GetAttribute("var");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                var = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = int.Parse(tmpArgVal);
            }
            conditions.add(new VarCondition(var, VarCondition.VAR_GREATER_THAN, value));
            chapter.addVar(var);
        }

        foreach (XmlElement el in greatersequalssthan)
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            tmpArgVal = el.GetAttribute("var");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                var = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = int.Parse(tmpArgVal);
            }
            conditions.add(new VarCondition(var, VarCondition.VAR_GREATER_EQUALS_THAN, value));
            chapter.addVar(var);
        }

        foreach (XmlElement el in lesssthan)
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            tmpArgVal = el.GetAttribute("var");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                var = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = int.Parse(tmpArgVal);
            }
            conditions.add(new VarCondition(var, VarCondition.VAR_LESS_THAN, value));
            chapter.addVar(var);
        }

        foreach (XmlElement el in lesssequalssthan)
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            tmpArgVal = el.GetAttribute("var");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                var = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = int.Parse(tmpArgVal);
            }
            conditions.add(new VarCondition(var, VarCondition.VAR_LESS_EQUALS_THAN, value));
            chapter.addVar(var);
        }

        foreach (XmlElement el in equalss)
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            tmpArgVal = el.GetAttribute("var");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                var = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = int.Parse(tmpArgVal);
            }
            conditions.add(new VarCondition(var, VarCondition.VAR_EQUALS, value));
            chapter.addVar(var);
        }

        foreach (XmlElement el in notsequals)
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            tmpArgVal = el.GetAttribute("var");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                var = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = int.Parse(tmpArgVal);
            }
            conditions.add(new VarCondition(var, VarCondition.VAR_NOT_EQUALS, value));
            chapter.addVar(var);
        }

        foreach (XmlElement el in globalsstatesref)
        {
            // Id
            string id = null;
            // VALUE WAS ADDED IN EAD1.4 - It allows negating a global state
            bool value = true;

            tmpArgVal = el.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                id = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("value");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                value = tmpArgVal.ToLower().Equals("true");
            }
            conditions.add(new GlobalStateCondition(id,
                value ? GlobalStateCondition.GS_SATISFIED : GlobalStateCondition.GS_NOT_SATISFIED));
        }
    }
}
using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Geo
{
    public class ExitAction : GeoAction
    {

        private readonly string[] parameters = new[] { "OnlyFromInside" };
        public ExitAction() : base()
        {
            OnlyFromInside = true;
        }

        public override string Name
        {
            get
            {
                return "Exit";
            }
        }
        public override string[] Parameters { get { return parameters; } }

        public bool OnlyFromInside
        {
            get { return (bool)this["OnlyFromInside"]; }
            set { this["OnlyFromInside"] = value; }
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}
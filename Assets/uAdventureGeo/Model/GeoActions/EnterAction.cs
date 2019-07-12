using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Geo
{
    public class EnterAction : GeoAction
    {
        private readonly string[] parameters = new[] { "OnlyFromOutside" };

        public EnterAction() : base()
        {
            OnlyFromOutside = true;
        }

        public override string Name
        {
            get
            {
                return "Enter";
            }
        }

        public bool OnlyFromOutside
        {
            get { return (bool)this["OnlyFromOutside"]; }
            set { this["OnlyFromOutside"] = value; }
        }

        public override string[] Parameters { get { return parameters; } }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}

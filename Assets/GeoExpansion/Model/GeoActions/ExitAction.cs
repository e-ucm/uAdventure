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

        public bool OnlyFromInside { get; set; }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}
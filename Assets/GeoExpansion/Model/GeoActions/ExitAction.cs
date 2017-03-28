using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Geo
{
    public class ExitAction : GeoAction
    {
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

        public bool OnlyFromInside { get; set; }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}
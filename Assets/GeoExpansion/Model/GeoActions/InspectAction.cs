using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Geo
{
    public class InspectAction : GeoAction
    {
        public InspectAction() : base()
        {
            Inside = true;
        }

        public override string Name
        {
            get
            {
                return "Inspect";
            }
        }

        public bool Inside { get; set; }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}


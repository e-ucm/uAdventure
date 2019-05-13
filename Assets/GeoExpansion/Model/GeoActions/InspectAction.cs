using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Geo
{
    public class InspectAction : GeoAction
    {
        private readonly string[] parameters = {"Inside"};

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

        public override string[] Parameters
        {
            get { return parameters; }
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}


using UnityEngine;
using System.Collections;

namespace uAdventure.Geo
{
    public class EnterAction : GeoAction
    {
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

        public bool OnlyFromOutside { get; set; }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}

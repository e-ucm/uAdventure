using UnityEngine;
using System.Collections;

namespace uAdventure.Geo
{

    public class GeoReference : MapElement
    {
        public GeoReference(string idTarget) : base(idTarget)
        {
        }

        bool ShowDirection { get; set; }
        GeoElementDrawer OverrideDrawer { get; set; }
    }
}
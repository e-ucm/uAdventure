using UnityEngine;
using System.Collections;

namespace uAdventure.Geo
{

    public class GeoReference : MapElement
    {
        public GeoReference(string targetId) : base(targetId)
        {
        }

        bool ShowDirection { get; set; }
        GeoElementDrawer OverrideDrawer { get; set; }
    }
}
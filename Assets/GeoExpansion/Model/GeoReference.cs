using UnityEngine;
using System.Collections;

namespace uAdventure.Geo
{

    public class GeoReference : MapElement
    {
        bool ShowDirection { get; set; }
        GeoElementDrawer OverrideDrawer { get; set; }
    }
}
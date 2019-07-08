using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{
    public class OsmLargeMeta : SimpleTileMeta
    {
        public OsmLargeMeta() : base(
            "OSMTileLarge",
            "Geo.TileMeta.Name.OSMTileLarge",
            "Geo.TileMeta.Description.OSMTileLarge",
            new Dictionary<string, object>
            {
                {"content-type", "image/png"},
                {"resolution", new Vector2Int(512,512)},
                {"url-template", "http://b.tile.openstreetmap.us/usgs_large_scale/{0}/{1}/{2}.png" }
            })
        { }
    }
}

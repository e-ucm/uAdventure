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
                {"url-template", "{0}://b.tile.openstreetmap.us/usgs_large_scale/{1}/{2}/{3}.png" }
            })
        { }
    }
}

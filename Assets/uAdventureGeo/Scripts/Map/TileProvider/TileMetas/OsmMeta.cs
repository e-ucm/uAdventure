using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{
    public class OsmMeta : SimpleTileMeta
    {
        public OsmMeta() : base (
            "OSMTile", 
            "Geo.TileMeta.Name.OSMTile", 
            "Geo.TileMeta.Description.OSMTile", 
            new Dictionary<string, object>
            {
                {"content-type", "image/png"},
                {"resolution", new Vector2Int(512,512)},
                {"url-template", "{0}://b.tile.openstreetmap.org/{1}/{2}/{3}.png" }
            })
        { }
    }
}

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
                {"url-template", "http://b.tile.openstreetmap.org/{0}/{1}/{2}.png" }
            })
        { }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{
    public class StamenTerrainMeta : SimpleTileMeta
    {
        public StamenTerrainMeta() : base(
            "StamenTerrain",
            "Geo.TileMeta.Name.StamenTerrain",
            "Geo.TileMeta.Description.StamenTerrain",
            new Dictionary<string, object>
            {
                {"content-type", "image/png"},
                {"resolution", new Vector2Int(512,512)},
                {"url-template", "http://tile.stamen.com/terrain-background/{0}/{1}/{2}.png" }
            })
        { }
    }
}
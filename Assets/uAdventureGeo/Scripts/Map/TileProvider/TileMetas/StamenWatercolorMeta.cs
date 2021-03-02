using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{
    public class StamenWatercolorMeta : SimpleTileMeta
    {
        public StamenWatercolorMeta() : base(
            "StamenWatercolor",
            "Geo.TileMeta.Name.StamenWatercolor",
            "Geo.TileMeta.Description.StamenWatercolor",
            new Dictionary<string, object>
            {
                {"content-type", "image/png"},
                {"resolution", new Vector2Int(512,512)},
                {"url-template", "{0}://c.tile.stamen.com/watercolor/{1}/{2}/{3}.png" }
            })
        { }
    }
}
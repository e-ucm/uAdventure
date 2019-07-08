using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Geo
{
    public class StamenTonerMeta : SimpleTileMeta
    {
        public StamenTonerMeta() : base(
            "StamenToner",
            "Geo.TileMeta.Name.StamenToner",
            "Geo.TileMeta.Description.StamenToner",
            new Dictionary<string, object>
            {
                {"content-type", "image/png"},
                {"resolution", new Vector2Int(512,512)},
                {"url-template", "http://a.tile.stamen.com/toner/{0}/{1}/{2}.png" }
            })
        { }
    }
}
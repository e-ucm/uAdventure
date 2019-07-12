using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Geo
{
    public static class TileUtil
    {
        public static IEnumerable<Vector2d> Tiles(RectD limits)
        {
            for (int i = (int)limits.Min.x, iEnd = (int)(limits.Min.x + limits.Width); i < iEnd; i++)
            {
                for (int j = (int)limits.Min.y, jEnd = (int)(limits.Min.y + limits.Height); j < jEnd; j++)
                {
                    yield return new Vector2d(i, j);
                }
            }
        }

        public static IEnumerable<Vector2d> NonCollidingTiles(AreaMeta area, IEnumerable<AreaMeta> allAreas, AreaMeta extraArea, bool useExtraArea)
        {
            // But not everything has to be removed so we get the limits relevant to the old gameplayArea considering
            var possibleCollidingAreas = allAreas;

            // If we use the extra area we add it to the set
            if (useExtraArea)
            {
                possibleCollidingAreas = possibleCollidingAreas.Union(new[] { extraArea });
            }

            // And finally we get the relevant limits (only overlapping limits)
            var limitsToConsiderBeforeRemoving = GetLimitsRelevantTo(possibleCollidingAreas, area).ToArray();

            // And finally we iterate over the limits where the tile is not contained in any limit
            foreach (var tile in TileUtil.Tiles(area.Limits).Where(t => !limitsToConsiderBeforeRemoving.Any(l => l.Contains(t))))
            {
                yield return tile;
            }
        }

        public static IEnumerable<RectD> GetLimitsRelevantTo(IEnumerable<AreaMeta> areas, AreaMeta area)
        {
            // First we get the tile limits of all the other areas that use the same tiles
            return areas
                .Where(a => a.Meta == area.Meta)
                .Select(a => a.Limits)
                // And then we optimize the check by only preserving the regions that intersect with new region we want to remove
                .Where(l => l.Intersects(area.Limits));
        }
    }
}

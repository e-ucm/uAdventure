
using UnityEngine;

namespace uAdventure.Geo
{
    public interface ITileStorer
    {
        bool CanStoreTile(Vector3d tile, ITileMeta meta);

        bool StoreTile(Vector3d tile, ITileMeta meta, ITilePromise tilePromise);

        bool DeleteTile(Vector3d tile, ITileMeta meta);

    }
}

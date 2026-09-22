using System;
using UnityEngine;

namespace uAdventure.Geo
{

    public interface ITileLoader
    {
        bool CanLoadTile(Vector3d tile, ITileMeta tileMeta);

        ITilePromise LoadTile(Vector3d tile, ITileMeta tileMeta, Action<ITilePromise> callback);
    }
}

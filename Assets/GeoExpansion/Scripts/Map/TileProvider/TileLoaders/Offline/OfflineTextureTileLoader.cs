
using System;
using UnityEngine;

namespace uAdventure.Geo
{
    public class OfflineTextureTileLoader : ITileLoader
    {
        public bool CanLoadTile(Vector3d tile, ITileMeta tileMeta)
        {
            return false;
        }

        public ITilePromise LoadTile(Vector3d tile, ITileMeta tileMeta, Action<ITilePromise> callback)
        {
            throw new NotImplementedException();
        }
    }
}

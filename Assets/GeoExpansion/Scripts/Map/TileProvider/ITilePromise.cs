using UnityEngine;

namespace uAdventure.Geo
{
    
    /// Tile promise class is used to provide a tile to draw that will be download 
    /// later on.
    public interface ITilePromise
    {
        Vector3d Tile { get; }

        ITileMeta Meta { get; }

        bool Loaded { get; }

        object Data { get; }

        bool Update();
    }
}

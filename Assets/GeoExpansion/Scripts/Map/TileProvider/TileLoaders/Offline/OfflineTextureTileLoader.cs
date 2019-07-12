using System;
using System.Linq;
using MapzenGo.Helpers;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Geo
{
    public class OfflineTextureTileLoader : ITileLoader
    {
        public static readonly string ResourcesTilePath = "uAdventure/Geo/Tiles/{0}_{1}_{2}_{3}";
        public static readonly string LocalTilePath = "Assets/Resources/uAdventure/Geo/Tiles/{0}_{1}_{2}_{3}.png";
        public static readonly string FullTilePath = Application.dataPath + "/Resources/uAdventure/Geo/Tiles/{0}_{1}_{2}_{3}.png";

        public bool CanLoadTile(Vector3d tile, ITileMeta tileMeta)
        {
            if (Application.isPlaying)
            {
                var mapScenes = Game.Instance.GameState.GetObjects<MapScene>();
                var tileBounds = GM.TileBounds(tile.ToVector2d(), (int) tile.z)
                                    .ToPoints()
                                    .Select(p => GM.MetersToLatLon(p))
                                    .ToArray()
                                    .ToRectD();
                var tileIsCached = mapScenes.Any(m =>
                    m.UsesGameplayArea && 
                    m.TileMetaIdentifier == tileMeta.Identifier &&
                    m.GameplayArea.Intersects(tileBounds));

                return tileIsCached && Resources.Load<Texture2D>(GetResourcesTilePath(tile, tileMeta));
            }
            return false;
        }

        public ITilePromise LoadTile(Vector3d tile, ITileMeta tileMeta, Action<ITilePromise> callback)
        {
            var resourcesTilePromise =
                new ResourcesTilePromise(tile, tileMeta, GetResourcesTilePath(tile, tileMeta), callback);
            resourcesTilePromise.Load();
            return resourcesTilePromise;
        }

        public static string GetResourcesTilePath(Vector3d tile, ITileMeta tileMeta)
        {
            return string.Format(ResourcesTilePath, tileMeta.Identifier, tile.z, tile.x, tile.y);
        }

        public static string GetTilePath(Vector3d tile, ITileMeta tileMeta)
        {
            return string.Format(LocalTilePath, tileMeta.Identifier, tile.z, tile.x, tile.y);
        }

        public static string GetFullTilePath(Vector3d tile, ITileMeta tileMeta)
        {
            return string.Format(FullTilePath, tileMeta.Identifier, tile.z, tile.x, tile.y);
        }
    }
}

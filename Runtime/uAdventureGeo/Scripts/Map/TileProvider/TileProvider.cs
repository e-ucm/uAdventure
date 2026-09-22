using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Geo
{
    public class TileProvider
    {
        #region Singleton
        // ##################################

        private static TileProvider instance;

        public static TileProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TileProvider();
                    instance.InitLoaders();
                }

                return instance;
            }
        }

        // ##################################
        #endregion

        #region Variables
        // ##################################

        private readonly List<ITileLoader> tileLoaders;
        private readonly List<ITileMeta> publicMeta;
        private ITileMeta[] publicMetaArray;
        private static Dictionary<ITileMeta, Dictionary<Vector3d, ITilePromise>> tileCache;
        private bool previousPlayingState = false;

        // ##################################
        #endregion

        #region Constructor
        // ##################################

        private TileProvider()
        {
            // Init the variables
            tileCache = new Dictionary<ITileMeta, Dictionary<Vector3d, ITilePromise>>();
            publicMeta = new List<ITileMeta>();
            tileLoaders = new List<ITileLoader>();
        }

        private void InitLoaders()
        {
            // Register the tile loaders
            tileLoaders.Add(new OfflineTextureTileLoader());
            tileLoaders.Add(new SimpleCachedOnlineTextureTileLoader(new OsmMeta(), 604800));
            tileLoaders.Add(new SimpleCachedOnlineTextureTileLoader(new OsmLargeMeta(), 604800));
            tileLoaders.Add(new SimpleCachedOnlineTextureTileLoader(new StamenTerrainMeta(), 604800));
            tileLoaders.Add(new SimpleCachedOnlineTextureTileLoader(new StamenTonerMeta(), 604800));
            tileLoaders.Add(new SimpleCachedOnlineTextureTileLoader(new StamenWatercolorMeta(), 604800));
        }

        // ##################################
        #endregion

        #region Metas
        // ##################################

        public void PublishMeta(ITileMeta meta)
        {
            var newMetaType = meta.GetType();
            if (publicMeta.All(m => m.GetType() != newMetaType))
            {
                publicMeta.Add(meta);
                publicMetaArray = publicMeta.ToArray();
            }   
        }

        public ITileMeta[] PublicMetas
        {
            get { return publicMetaArray; }
        }

        public ITileMeta GetTileMeta(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            return PublicMetas.First(m => m.Identifier == identifier);
        }

        // ##################################
        #endregion

        #region Loaders
        // ##################################


        public ITilePromise GetTile(Vector3d tile, Action<ITilePromise> callback)
        {
            var defaultTileMeta = publicMeta.Count > 0 ? publicMeta[0] : null;
            return defaultTileMeta != null ? GetTile(tile, defaultTileMeta, callback) : null;
        }


        public ITilePromise GetTile(Vector3d tile, ITileMeta tileMeta, Action<ITilePromise> callback)
        {
            if(previousPlayingState != Application.isPlaying)
            {
                tileCache.Clear();
                previousPlayingState = Application.isPlaying;
            }

            if (!tileCache.ContainsKey(tileMeta))
            {
                tileCache[tileMeta] = new Dictionary<Vector3d, ITilePromise>();
            }

            if (tileCache[tileMeta].ContainsKey(tile))
            {
                callback(tileCache[tileMeta][tile]);
                return tileCache[tileMeta][tile];
            }

            foreach (var tileLoader in tileLoaders)
            {
                if (tileLoader.CanLoadTile(tile, tileMeta))
                {
                    var tilePromise = tileLoader.LoadTile(tile, tileMeta, (tp) =>
                    {
                        if (tp.Data == null)
                        {
                            tileCache[tp.Meta].Remove(tp.Tile);
                        }
                        callback(tp);
                    });
                    tileCache[tileMeta][tile] = tilePromise;
                    return tilePromise;
                }
            }

            Debug.LogWarning("Couldn't load tile " + tile + " with metatype " + tileMeta.GetType().ToString());

            return null; 
        }

        // ##################################
        #endregion
    }

}

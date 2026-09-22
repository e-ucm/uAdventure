
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class PngTextureStorer : ITileStorer
    {
        private static readonly string ContentTypeAttr = "content-type";
        private static readonly string ContentTypeValue = "image/png";


        public bool CanStoreTile(Vector3d tile, ITileMeta meta)
        {
            return meta.Attributes.Contains(ContentTypeAttr) && string.Equals(ContentTypeValue, 
                       meta[ContentTypeAttr] as string, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool StoreTile(Vector3d tile, ITileMeta meta, ITilePromise tilePromise)
        {
            if (tilePromise.Loaded && tilePromise.Data != null)
            {
                SimpleCachedOnlineTextureTileLoader.SavePNG(tilePromise.Data as Texture2D, OfflineTextureTileLoader.GetFullTilePath(tile, meta));
                AssetDatabase.ImportAsset(OfflineTextureTileLoader.GetTilePath(tile, meta));
                return true;
            }

            return false;
        }

        public bool DeleteTile(Vector3d tile, ITileMeta meta)
        {
            return AssetDatabase.DeleteAsset(OfflineTextureTileLoader.GetTilePath(tile, meta));
        }
    }
}

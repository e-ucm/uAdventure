
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

        private static readonly string LocalTilePath = "Assets/Resources/uAdventure/Geo/Tiles/{0}_{1}_{2}_{3}.png";
        private static readonly string FullTilePath = Application.dataPath + "/Resources/uAdventure/Geo/Tiles/{0}_{1}_{2}_{3}.png";

        public bool CanStoreTile(Vector3d tile, ITileMeta meta)
        {
            return meta.Attributes.Contains(ContentTypeAttr) && string.Equals(ContentTypeValue, 
                       meta[ContentTypeAttr] as string, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool StoreTile(Vector3d tile, ITileMeta meta, ITilePromise tilePromise)
        {
            if (tilePromise.Loaded && tilePromise.Data != null)
            {
                SimpleCachedOnlineTextureTileLoader.SavePNG(tilePromise.Data as Texture2D, GetFullTilePath(tile, meta));
                AssetDatabase.ImportAsset(GetTilePath(tile, meta));
                return true;
            }

            return false;
        }

        public bool DeleteTile(Vector3d tile, ITileMeta meta)
        {
            return AssetDatabase.DeleteAsset(GetTilePath(tile, meta));
        }

        private string GetTilePath(Vector3d tile, ITileMeta tileMeta)
        {
            return string.Format(LocalTilePath, tileMeta.Identifier, tile.z, tile.x, tile.y);
        }

        private string GetFullTilePath(Vector3d tile, ITileMeta tileMeta)
        {
            return string.Format(FullTilePath, tileMeta.Identifier, tile.z, tile.x, tile.y);
        }
    }
}

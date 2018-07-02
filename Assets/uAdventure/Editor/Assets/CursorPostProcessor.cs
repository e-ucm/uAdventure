using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class CursorPostProcessor : AssetPostprocessor
    {
        private static readonly string[] categories = { "/cursors/" };

        void OnPreprocessTexture()
        {
            string lowerCaseAssetPath = assetPath.ToLower();
            bool isuAdventureAsset = lowerCaseAssetPath.Contains("/uadventure/");
            bool shouldMakeItCursor = isuAdventureAsset && categories.Any(c => lowerCaseAssetPath.Contains(c));

            if (shouldMakeItCursor)
            {
                var textureImporter = assetImporter as TextureImporter;
                textureImporter.textureType = TextureImporterType.Cursor;
            }
        }
    }
}


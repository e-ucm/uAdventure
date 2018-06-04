using UnityEngine;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    public class ReadWritePostProcessor : AssetPostprocessor
    {
        private static readonly string[] categories = { "/animation/", "/arrows/", "/background/", "/foreground/", "/defaultassets/", "/hud/", "/image/", "/options/", "/special/" };

        void OnPreprocessTexture()
        {
            string lowerCaseAssetPath = assetPath.ToLower();
            bool isuAdventureAsset = lowerCaseAssetPath.Contains("/uadventure/");
            bool shouldActiveReadWrite = isuAdventureAsset && categories.Any(c => lowerCaseAssetPath.Contains(c));

            if (shouldActiveReadWrite)
            {
                var textureImporter = assetImporter as TextureImporter;
                textureImporter.isReadable = true;
            }
        }
    }
}
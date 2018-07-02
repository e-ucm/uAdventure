using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SpritePostProcessor : AssetPostprocessor
    {
        private static readonly string[] categories = { "/animation/", "/arrows/", "/background/", "/defaultassets/", "/hud/", "/icon/", "/image/", "/options/", "/special/" };

        void OnPreprocessTexture()
        {
            string lowerCaseAssetPath = assetPath.ToLower();
            bool isuAdventureAsset = lowerCaseAssetPath.Contains("/uadventure/");
            bool shouldMakeItSprite = isuAdventureAsset && categories.Any(c => lowerCaseAssetPath.Contains(c));

            if (shouldMakeItSprite)
            {
                var textureImporter = assetImporter as TextureImporter;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
            }
        }
    }
}


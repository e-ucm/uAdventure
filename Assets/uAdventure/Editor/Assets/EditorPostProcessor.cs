using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class EditorPostProcessor : AssetPostprocessor
    {
        private static readonly string[] categories = { "/icons/", "/help/", "/gui/", "/build-icons/" };

        void OnPreprocessTexture()
        {
            string lowerCaseAssetPath = assetPath.ToLower();
            bool isuAdventureAsset = lowerCaseAssetPath.Contains("/uadventure/editor/");
            bool shouldMakeItGUI = isuAdventureAsset && categories.Any(c => lowerCaseAssetPath.Contains(c));

            if (shouldMakeItGUI)
            {
                var textureImporter = assetImporter as TextureImporter;
                textureImporter.textureType = TextureImporterType.GUI;

                // Effect icons sizes
                if (lowerCaseAssetPath.Contains("/16x16/"))
                {
                    textureImporter.maxTextureSize = 16;
                }
                if (lowerCaseAssetPath.Contains("/32x32/") || lowerCaseAssetPath.Contains("/build-icons/"))
                {
                    textureImporter.maxTextureSize = 32;
                }
                if (lowerCaseAssetPath.Contains("/64x64/"))
                {
                    textureImporter.maxTextureSize = 64;
                }
                if (lowerCaseAssetPath.Contains("/64x64-hot/"))
                {
                    textureImporter.maxTextureSize = 64;
                }
            }
        }
    }
}


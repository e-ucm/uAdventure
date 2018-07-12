using UnityEngine;
using System.Collections;
using System.IO;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Tool for editing Resources Blocks ("Edit" button). It cannot be combined as
     * well.
     */

    public class EditResourceTool : ResourcesTool
    {
        /**
         * The filename to edit
         */
        protected string filename;

        // Added v.15
        protected string destinyAssetName;

        public EditResourceTool(ResourcesUni resources, AssetInformation[] assetsInformation, int index, string filename,
            string destinyAssetName) : base(resources, assetsInformation, -1, index)
        {
            this.filename = filename;
            this.destinyAssetName = destinyAssetName;
        }

        public override bool doTool()
        {

            bool done = false;

            AssetsController.AddSingleAsset(assetsInformation[index].category, filename, destinyAssetName, true);
            // Dirty fix?
            string selectedAsset = destinyAssetName == null ? (new FileInfo(filename)).FullName : destinyAssetName;
            // If a file was selected
            if (selectedAsset != null)
            {

                // Take the index of the selected asset
                string[] assetFilenames = AssetsController.GetAssetFilenames(assetsInformation[index].category,
                    assetsInformation[index].filter);
                string[] assetPaths = AssetsController.GetAssetsList(assetsInformation[index].category,
                    assetsInformation[index].filter);
                int assetIndex = -1;
                for (int i = 0; i < assetFilenames.Length; i++)
                    if (assetFilenames[i].Equals(selectedAsset))
                        assetIndex = i;

                // check if the asset is "standright" or "standleft" in order to modify the attr assetNecessary
                // for the assetInformation
                if (assetsInformation[index].name.Equals("standright"))
                {
                    // if "standright" asset is necessary, set the "standleft" as not necessary
                    if (assetsInformation[index].assetNecessary)
                    {
                        for (int i = 0; i < assetsInformation.Length; i++)
                        {
                            if (assetsInformation[i].name.Equals("standleft"))
                                assetsInformation[i].assetNecessary = false;
                        }
                    }
                    //if is not art necessary and is 3rd person game, look for "standleft", if this asset is 
                    // not necessary, set "standright as necessary"
                    else if (!Controller.Instance.PlayTransparent)
                    {
                        for (int i = 0; i < assetsInformation.Length; i++)
                        {
                            if (assetsInformation[i].name.Equals("standleft"))
                            {
                                assetsInformation[index].assetNecessary = true;
                                assetsInformation[i].assetNecessary = false;
                            }
                        }
                    }
                }
                else if (assetsInformation[index].name.Equals("standleft"))
                {
                    // if "standleft" asset is necessary, set the "standright" as not necessary
                    if (assetsInformation[index].assetNecessary)
                    {
                        for (int i = 0; i < assetsInformation.Length; i++)
                        {
                            assetsInformation[i].assetNecessary = false;
                        }
                    } //if is not art necessary and is 3rd person game, look for "standright", if this asset is 
                      // not necessary, set "standright as necessary"
                    else if (!Controller.Instance.PlayTransparent)
                    {
                        for (int i = 0; i < assetsInformation.Length; i++)
                        {
                            if (assetsInformation[i].name.Equals("standright"))
                            {
                                assetsInformation[index].assetNecessary = true;
                                assetsInformation[i].assetNecessary = false;
                            }
                        }
                    }
                }
                //The empty animation is, in fact, a special asset. When this asset is in an animation, it is considered as animation asset.
                // For this reason,at this point, assetIndex is = -1. So, if animation is emptyAnimation, change the path in addAsset method
                bool changeFilter = false;
                string specialPath = AssetsController.CATEGORY_SPECIAL_ASSETS + "/" + "EmptyAnimation.eaa";
                if (filename.Contains("EmptyAnimation"))
                    changeFilter = true;

                resources.addAsset(assetsInformation[index].name, changeFilter ? specialPath : assetPaths[assetIndex]);
                done = true;


            }
            return done;

        }
    }
}
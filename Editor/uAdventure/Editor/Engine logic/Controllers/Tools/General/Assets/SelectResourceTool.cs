using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Edition tool for selecting a resource. It supports undo, redo but not
     * combine.
     * 
     */
    public class SelectResourceTool : ResourcesTool
    {

        public SelectResourceTool(ResourcesUni resources, AssetInformation[] assetsInformation, int resourcesType, int index) : base(resources, assetsInformation, resourcesType, index)
        {
        }

        public override bool doTool()
        {

            bool done = false;
            //TODO: implement this part
            //string selectedAsset = null;
            //AssetChooser chooser = AssetsController.getAssetChooser(assetsInformation[index].category, assetsInformation[index].filter);
            //int option = chooser.showAssetChooser(controller.peekWindow());
            ////In case the asset was selected from the zip file
            //if (option == AssetChooser.ASSET_FROM_ZIP)
            //{
            //    selectedAsset = chooser.getSelectedAsset();
            //}

            ////In case the asset was not in the zip file: first add it
            //else if (option == AssetChooser.ASSET_FROM_OUTSIDE)
            //{
            //    bool added = AssetsController.addSingleAsset(assetsInformation[index].category, chooser.getSelectedFile().getAbsolutePath());
            //    if (added)
            //    {
            //        selectedAsset = chooser.getSelectedFile().getName();
            //    }
            //}

            //// If a file was selected
            //if (selectedAsset != null)
            //{
            //    // Take the index of the selected asset
            //    string[] assetFilenames = AssetsController.getAssetFilenames(assetsInformation[index].category, assetsInformation[index].filter);
            //    string[] assetPaths = AssetsController.getAssetsList(assetsInformation[index].category, assetsInformation[index].filter);
            //    int assetIndex = -1;
            //    for (int i = 0; i < assetFilenames.Length; i++)
            //        if (assetFilenames[i].Equals(selectedAsset))
            //            assetIndex = i;

            //    // Store the data in the resources block (removing the suffix if necessary)
            //    if (assetsInformation[index].category == AssetsConstants.CATEGORY_ANIMATION)
            //    {
            //        done = resources.addAsset(assetsInformation[index].name, AssetsController.removeSuffix(assetPaths[assetIndex]));

            //        // For player and character resources block, check if the other animations are set. When any are set, ask the user to set them automatically
            //        if (resourcesType == Controller.PLAYER || resourcesType == Controller.NPC)
            //        {
            //            bool someAnimationSet = false;
            //            for (int i = 0; i < assetsInformation.Length; i++)
            //            {
            //                if (i != index && resources.getAssetPath(assetsInformation[i].name) != null && !resources.getAssetPath(assetsInformation[i].name).Equals(""))
            //                {
            //                    someAnimationSet = true;
            //                    break;
            //                }
            //            }

            //            //THE SAME CODE IS IN EDITRESOURCETOOL!!
            //            // check if the asset is "standright" or "standleft" in order to modify the attr assetNecessary
            //            // for the assetInformation
            //            if (assetsInformation[index].name.Equals("standright"))
            //            {
            //                // if "standright" asset is necessary, set the "standleft" as not necessary
            //                if (assetsInformation[index].assetNecessary)
            //                {
            //                    for (int i = 0; i < assetsInformation.Length; i++)
            //                    {
            //                        if (assetsInformation[i].name.Equals("standleft"))
            //                            assetsInformation[i].assetNecessary = false;
            //                    }
            //                }
            //                //if is not art necessary and is 3rd person game, look for "standleft", if this asset is 
            //                // not necessary, set "standright as necessary"
            //                else if (!Controller.getInstance().isPlayTransparent())
            //                {
            //                    for (int i = 0; i < assetsInformation.Length; i++)
            //                    {
            //                        if (assetsInformation[i].name.Equals("standleft"))
            //                        {
            //                            assetsInformation[index].assetNecessary = true;
            //                            assetsInformation[i].assetNecessary = false;
            //                        }
            //                    }
            //                }
            //            }
            //            else if (assetsInformation[index].name.Equals("standleft"))
            //            {
            //                // if "standleft" asset is necessary, set the "standright" as not necessary
            //                if (assetsInformation[index].assetNecessary)
            //                {
            //                    for (int i = 0; i < assetsInformation.Length; i++)
            //                    {
            //                        assetsInformation[i].assetNecessary = false;
            //                    }
            //                } //if is not art necessary and is 3rd person game, look for "standright", if this asset is 
            //                // not necessary, set "standright as necessary"
            //                else if (!Controller.getInstance().isPlayTransparent())
            //                {
            //                    for (int i = 0; i < assetsInformation.Length; i++)
            //                    {
            //                        if (assetsInformation[i].name.Equals("standright"))
            //                        {
            //                            assetsInformation[index].assetNecessary = true;
            //                            assetsInformation[i].assetNecessary = false;
            //                        }
            //                    }
            //                }
            //            }

            //            if (!someAnimationSet && controller.showStrictConfirmDialog(Language.GetText("Operation.SetAllAnimations.Title"), Language.GetText("Operation.SetAllAnimations.Message")))
            //            {
            //                for (int i = 0; i < assetsInformation.Length; i++)
            //                {
            //                    if (i != index)
            //                    {
            //                        done |= resources.addAsset(assetsInformation[i].name, AssetsController.removeSuffix(assetPaths[assetIndex]));
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else {
            //        done = resources.addAsset(assetsInformation[index].name, assetPaths[assetIndex]);
            //    }
            //}
            return done;
        }

        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (done)
            {
                Controller.Instance.updatePanel();
            }
            return done;
        }

        public override bool redoTool()
        {

            bool done = base.redoTool();
            if (done)
            {
                Controller.Instance.updatePanel();
            }
            return done;
        }

        /**
         * Uses a SelectResourceTool to get the assetPath of a resource belonging to
         * the given category using an asset chooser with the given filter
         * 
         * @param category
         * @param filter
         * @return
         */
        public static string selectAssetPathUsingChooser(int category, int filter)
        {

            string assetPath = null;

            ResourcesUni resources = new ResourcesUni();
            AssetInformation[] assetsInformation = new AssetInformation[1];
            assetsInformation[0] = new AssetInformation("", "marihuanhell", false, category, filter);
            int resourcesType = -10;
            int index = 0;
            //try
            //{
            SelectResourceTool tool = new SelectResourceTool(resources, assetsInformation, resourcesType, index);
            tool.doTool();
            if (tool.resources.existAsset("marihuanhell"))
                assetPath = tool.resources.getAssetPath("marihuanhell");
            //}
            //catch (CloneNotSupportedException e)
            //{
            //    ReportDialog.GenerateErrorReport(e, true, "Error selecting asset path");
            //}
            return assetPath;
        }
    }
}
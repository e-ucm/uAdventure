using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SelectExitCursorPathTool : SelectResourceTool
    {
        protected const string CURSOR_STR = "cursor";

        protected ExitLook exitLook;

        protected static AssetInformation[] createAssetInfoArray()
        {

            AssetInformation[] array = new AssetInformation[1];
            array[0] = new AssetInformation("", CURSOR_STR, true, AssetsConstants.CATEGORY_CURSOR,
                AssetsController.FILTER_NONE);
            return array;
        }

        protected static ResourcesUni createResources(ExitLook exitLook)
        {

            ResourcesUni resources = new ResourcesUni();
            resources.addAsset(CURSOR_STR, exitLook.getCursorPath());
            return resources;
        }

        public SelectExitCursorPathTool(ExitLook exitLook)
            : base(createResources(exitLook), createAssetInfoArray(), Controller.EXIT, 0)
        {
            this.exitLook = exitLook;
        }

        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (!done)
                return false;
            else
            {
                exitLook.setCursorPath(resources.getAssetPath(CURSOR_STR));
                controller.updatePanel();
                return true;
            }

        }

        public override bool redoTool()
        {

            bool done = base.redoTool();
            if (!done)
                return false;
            else
            {
                exitLook.setCursorPath(resources.getAssetPath(CURSOR_STR));
                controller.updatePanel();
                return true;
            }
        }


        public override bool doTool()
        {

            bool done = base.doTool();
            if (!done)
                return false;
            else
            {
                exitLook.setCursorPath(resources.getAssetPath(CURSOR_STR));
                return true;
            }
        }
    }
}
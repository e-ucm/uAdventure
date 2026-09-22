using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteResourceTool : ResourcesTool
    {

        public DeleteResourceTool(ResourcesUni resources, AssetInformation[] assetsInformation, int index)
            : base(resources, assetsInformation, -1, index)
        {
        }


        public override bool doTool()
        {

            bool done = false;
            // If the given asset is not empty, delete it
            if (resources.getAssetPath(assetsInformation[index].name) != null)
            {
                resources.deleteAsset(assetsInformation[index].name);
                done = true;
            }
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
    }
}
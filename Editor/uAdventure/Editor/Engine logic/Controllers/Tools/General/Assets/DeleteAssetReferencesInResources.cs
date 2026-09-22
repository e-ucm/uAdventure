using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Edition tool for deleting the given asset path
     */
    public class DeleteAssetReferencesInResources : ResourcesTool
    {

        /**
         * The assetPath to be deleted
         */
        protected string assetPath;

        public DeleteAssetReferencesInResources(ResourcesUni resources, string assetPath) : base(resources, null, -1, -1)
        {
            this.assetPath = assetPath;
        }


        public override bool doTool()
        {

            bool done = false;
            // Search in the types of the resources
            foreach (string type in resources.getAssetTypes())
            {
                if (resources.getAssetPath(type).Equals(assetPath))
                {
                    resources.deleteAsset(type);
                    done = true;
                }
            }
            return done;
        }
    }
}
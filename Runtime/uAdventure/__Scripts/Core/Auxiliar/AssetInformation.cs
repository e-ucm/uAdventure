using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the information about an asset. It stores the description of
     * the asset, the identifier (name) of the asset, and its type.
     */
    public class AssetInformation
    {
        /**
             * Textual description of the asset.
             */
        public string description;

        /**
         * Name of the asset.
         */
        public string name;

        /**
         * True if the asset is necessary for the resources block to be valid.
         */
        public bool assetNecessary;

        /**
         * Category of the asset.
         */
        public int category;

        /**
         * Specific filter for the category
         */
        public int filter;

        /**
         * Constructor.
         * 
         * @param description
         *            Description of the asset
         * @param name
         *            Name of the asset
         * @param assetNecessary
         *            True if the asset is necessary for the resources to be valid
         * @param category
         *            Category of the asset
         * @param filter
         *            Specific filter for the category
         */
        public AssetInformation(string description, string name, bool assetNecessary, int category, int filter)
        {

            this.description = description;
            this.name = name;
            this.assetNecessary = assetNecessary;
            this.category = category;
            this.filter = filter;
        }
    }
}
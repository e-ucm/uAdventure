using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Set of constants which declare maximum sizes permitted for some type of assets.
     * AssetController (editor) must implement it.
     */
    public static class AssetsImageDimensions
    {

        /**
         * Dimensions for background
         */
        public const int BACKGROUND_MAX_WIDTH = 800;
        public const int BACKGROUND_MAX_HEIGHT = 600;

        /**
         * Dimensions for icons
         */
        public const int ICON_MAX_WIDTH = 80;
        public const int ICON_MAX_HEIGHT = 48;

        /**
         * Dimensionsn for book arrows
         */
        public const int ARROW_BOOK_MAX_WIDTH = 300;
        public const int ARROW_BOOK_MAX_HEIGHT = 200;

    }
}
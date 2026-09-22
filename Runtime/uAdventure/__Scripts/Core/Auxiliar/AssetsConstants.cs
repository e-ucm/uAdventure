namespace uAdventure.Core
{

    /**
     * Set of constants for identifying types of Asssets. AssetsController (editor)
     * must implement it.
     * 
     */
    public static class AssetsConstants
    {

        /**
         * Background category.
         */
        public const int CATEGORY_BACKGROUND = 0;

        /**
         * Animation category.
         */
        public const int CATEGORY_ANIMATION = 1;

        /**
         * Image category.
         */
        public const int CATEGORY_IMAGE = 2;

        /**
         * Icon category.
         */
        public const int CATEGORY_ICON = 3;

        /**
         * Audio category.
         */
        public const int CATEGORY_AUDIO = 4;

        /**
         * Video category.
         */
        public const int CATEGORY_VIDEO = 5;

        /**
         * Cursor category.
         */
        public const int CATEGORY_CURSOR = 6;

        /**
         * Cursor category.
         */
        public const int CATEGORY_STYLED_TEXT = 7;

        /**
         * Animation Image category
         */
        public const int CATEGORY_ANIMATION_IMAGE = 8;

        /**
         * Customized button category
         */
        public const int CATEGORY_BUTTON = 9;

        /**
         * Animation sound category
         */
        public const int CATEGORY_ANIMATION_AUDIO = 10;

        /**
         * Arrows for books
         */
        public const int CATEGORY_ARROW_BOOK = 11;

        /**
         * Number of categories.
         */
        public const int CATEGORIES_COUNT = 12;

        /**
         * Extra categories that are not stored are placed ahead of categories count
         */
        public const int CATEGORY_BOOL = 13;


    }

}
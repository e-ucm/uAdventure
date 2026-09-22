namespace uAdventure.Core
{
    /**
     * Centralizes the paths used to locate uAdventure content after the engine moved
     * to a UPM package layout. Engine and editor default content live inside the
     * package (Packages/es.e-ucm.uadventure) while user-generated content
     * (CurrentGame, config, exported reports) lives in the project's Assets folder.
     */
    public static class UAdventurePaths
    {
        public const string PACKAGE_NAME = "es.e-ucm.uadventure";

        /** AssetDatabase / filesystem path to the package root (editor only). */
        public static readonly string PACKAGE_ASSET_PATH = "Packages/" + PACKAGE_NAME;

        /** Read-only engine content shipped with the package. */
        public static readonly string ENGINE_RESOURCES_PATH = PACKAGE_ASSET_PATH + "/Runtime/uAdventure/Resources/";

        /** Read-only editor content shipped with the package. */
        public static readonly string EDITOR_RESOURCES_PATH = PACKAGE_ASSET_PATH + "/Editor/uAdventure/Editor/Resources/";

        /** Writable project location for user generated content. */
        public static readonly string PROJECT_RESOURCES_PATH = "Assets/Resources/";

        /** Writable project location for the current game data. */
        public static readonly string CURRENT_GAME_PATH = PROJECT_RESOURCES_PATH + "CurrentGame/";
    }
}
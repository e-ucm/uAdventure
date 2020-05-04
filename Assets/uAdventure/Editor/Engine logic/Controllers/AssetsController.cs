using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uAdventure.Core;
using UnityEditor;
using System.Linq;
using System.Globalization;
using System.Text;

namespace uAdventure.Editor
{
    /**
     * This class is responsible for managing the multimedia data stored in the
     * adventure ZIP files. It is also responsible for the assessment and adaptation
     * files of the adventures.
     */

    public class AssetsController
    {
        protected const string UA_RESOURCES = "Assets/uAdventure/Resources/";
        protected const string UA_EDITOR_RESOURCES = "Assets/uAdventure/Editor/Resources/";
        /**
         * Assessment files category.
         */
        //public const int CATEGORY_ASSESSMENT = 0;
        /**
         * Assessment files category.
         */
        //public const int CATEGORY_ADAPTATION = 1;

        /**
         * Void filter.
         */
        public const int FILTER_NONE = 0;

        /**
         * JPG files filter.
         */
        public const int FILTER_JPG = 1;

        /**
         * PNG files filter.
         */
        public const int FILTER_PNG = 2;

        /**
         * MP3 files filter.
         */
        public const int FILTER_MP3 = 3;

        //TODO: VIDEO FILTERS

        /**
         * Path for the background assets.
         */
        public static readonly string CATEGORY_BACKGROUND_FOLDER = "assets/background";

        /**
         * Path for the animation assets.
         */
        public static readonly string CATEGORY_ANIMATION_FOLDER = "assets/animation";

        /**
         * Path for the image assets.
         */
        public static readonly string CATEGORY_IMAGE_FOLDER = "assets/image";

        /**
         * Path for the icon assets.
         */
        public static readonly string CATEGORY_ICON_FOLDER = "assets/icon";

        /**
         * Path for the audio assets.
         */
        public static readonly string CATEGORY_AUDIO_PATH = "assets/audio";

        /**
         * Path for the video assets.
         */
        public static readonly string CATEGORY_VIDEO_PATH = "assets/video";

        /**
         * Path for the video assets.
         */
        public static readonly string CATEGORY_CURSOR_PATH = "gui/cursors";

        /**
         * Path for the video assets.
         */
        public static readonly string CATEGORY_STYLED_TEXT_PATH = "assets/styledtext";

        /**
         * Path for the custom button assets.
         */
        public static readonly string CATEGORY_BUTTON_PATH = "gui/buttons";

        /**
         * Path for the arrows of books
         */
        public static readonly string CATEGORY_ARROW_BOOK_PATH = "assets/arrows";

        public const string CATEGORY_SPECIAL_ASSETS = "assets/special";
        public const string EADVETURE_CONTENT_FOLDER = "EAdventureData";

        public const string BASE_DIR = "CurrentGame/";
        
        private static List<string> pathsToReimport;

        public static string EditorResourcesPath { get { return UA_EDITOR_RESOURCES; } }

        /**
         * Static class. Private constructor.
         */

        private AssetsController()
        {

        }

        public static void ResetCache()
        {
        }

        public static string[] CategoryFolders()
        {

            string[] folders = new string[AssetsConstants.CATEGORIES_COUNT];
            for (int i = 0; i < AssetsConstants.CATEGORIES_COUNT; i++)
            {
                folders[i] = AssetsController.getCategoryFolder(i);
            }
            return folders;
        }

        /**
         * Creates the initial structure of asset folders
         */

        public static void CreateFolderStructure()
        {
            DirectoryInfo projectDir = new DirectoryInfo(Controller.Instance.ProjectFolder);
            Debug.Log("CREATE: " + projectDir.FullName);
            var folders = new List<string>();
            folders.AddRange(CategoryFolders());
            folders.Add("assets/special/");
            foreach(var folder in folders)
            {
                var subDirPath = Path.Combine(projectDir.FullName, folder);
                if (!Directory.Exists(subDirPath))
                {
                    projectDir.CreateSubdirectory(folder);
                }
            }

            var dtds = new string[] { "descriptor.dtd", "eadventure.dtd", "assessment.dtd", "adaptation.dtd", "animation.dtd" };
            var uAdventureContentFolder = UA_EDITOR_RESOURCES + EADVETURE_CONTENT_FOLDER;

            foreach(var dtd in dtds)
            {
                // Copy eadventure.dtd, descriptor.dtd, assessment.dtd, adaptation.dtd
                FileInfo descriptorDTD = new FileInfo(Path.Combine(uAdventureContentFolder, dtd));
                if (descriptorDTD.Exists)
                {
                    descriptorDTD.CopyTo(Path.Combine(projectDir.FullName, dtd), true);
                }
                else
                {
                    Controller.Instance.ShowErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " " + dtd);
                }
            }
        }

        /**
         * Returns the name of the files in the given category. This method must be
         * used only to display the data, as it doesn't contain complete paths.
         * 
         * @param assetsCategory
         *            Category of the assets
         * @return List of assets of the given category
         */

        public static string[] GetAssetFilenames(int assetsCategory)
        {

            return GetAssetFilenames(assetsCategory, FILTER_NONE);
        }

        /**
         * Returns the name of the files in the given category. This method must be
         * used only to display the data, as it doesn't contain complete paths.
         * 
         * @param assetsCategory
         *            Category of the assets
         * @param filter
         *            Specific filter for the files
         * @return List of assets of the given category
         */

        public static string[] GetAssetFilenames(int assetsCategory, int filter)
        {

            string[] assetsList = new string[] { };

            // Take the category folder, from the ZIP file name
            DirectoryInfo categoryFolder =
                new DirectoryInfo(Path.Combine(Controller.Instance.ProjectFolder,
                    getCategoryFolder(assetsCategory)));

            // Take the file list and create the array
            FileInfo[] fileList = categoryFolder.GetFiles(getAssetsFileFilter(assetsCategory, filter));

            // If the array is not empty
            if (fileList != null && assetsCategory != AssetsConstants.CATEGORY_STYLED_TEXT)
            {
                // Copy the relative paths to the string array
                // If is styled text, remove referenced files (folder) when present
                assetsList = new string[fileList.Length];
                for (int i = 0; i < fileList.Length; i++)
                {
                    assetsList[i] = fileList[i].Name;
                }
            }

            return assetsList;
        }

        /**
         * Returns the files in the given category, using a relative path to the ZIP
         * file.
         * 
         * @param assetsCategory
         *            Category of the assets
         * @return List of assets of the given category, using paths relative to the
         *         opened ZIP file
         */

        public static string[] GetAssetsList(int assetsCategory)
        {

            return GetAssetsList(assetsCategory, FILTER_NONE);
        }

        /**
         * Returns the files for the given category and filter, using a relative
         * path to the ZIP file.
         * 
         * @param assetsCategory
         *            Category of the assets
         * @param filter
         *            Specific filter for the files
         * @return List of assets for the given category and filter, using paths
         *         relative to the opened ZIP file
         */

        public static string[] GetAssetsList(int assetsCategory, int filter)
        {

            string[] assetsList = new string[] { };

            // Take the category folder, from the ZIP file name
            DirectoryInfo categoryFolder =
                new DirectoryInfo(Path.Combine(Controller.Instance.ProjectFolder,
                    getCategoryFolder(assetsCategory)));

            // Take the file list and create the array
            FileInfo[] fileList = categoryFolder.GetFiles(getAssetsFileFilter(assetsCategory, filter));

            // If the array is not empty
            if (fileList != null)
            {
                // If is styled text, remove referenced files (folder) when present
                if (assetsCategory != AssetsConstants.CATEGORY_STYLED_TEXT)
                {
                    // Copy the relative paths to the string array
                    assetsList = new string[fileList.Length];
                    for (int i = 0; i < fileList.Length; i++)
                        //assetsList[i] = fileList[i].getEnclEntryName( );
                        assetsList[i] = getCategoryFolder(assetsCategory) + "/" + fileList[i].Name;
                }
                else
                {
                    //Remove those files which are directories
                    //List<FileInfo> filteredFileList = new List<FileInfo>();
                    //for (int i = 0; i < fileList.length; i++)
                    //{
                    //    if (!fileList[i].isDirectory())
                    //        filteredFileList.add(fileList[i]);
                    //}

                    //assetsList = new string[filteredFileList.size()];
                    //for (int i = 0; i < filteredFileList.size(); i++)
                    //    //assetsList[i] = filteredFileList.get( i ).getEnclEntryName( );
                    //    assetsList[i] = getCategoryFolder(assetsCategory) + "/" + filteredFileList[i].getName();

                }
            }

            return assetsList;
        }

        public static string AddSingleAsset(int assetsCategory, string assetPath)
        {
            return AddSingleAsset(assetsCategory, assetPath, true);
        }

        public static string AddSingleAsset(int assetsCategory, string assetPath, bool checkIfAssetExists)
        {
            return AddSingleAsset(assetsCategory, assetPath, null, checkIfAssetExists);
        }

        public static string AddSingleAsset(int assetsCategory, string assetPath, string destinyAssetName,
            bool checkIfAssetExists)
        {
            string assetTypeDir = getCategoryFolder(assetsCategory);

            var localPath = UA_RESOURCES + BASE_DIR + assetTypeDir;
            DirectoryInfo path = new DirectoryInfo(localPath);
            if (!Directory.Exists(path.FullName))
            {
                Directory.CreateDirectory(path.FullName);
            }

            string nameOnly = !string.IsNullOrEmpty(destinyAssetName) ? destinyAssetName : Path.GetFileName(assetPath);
            string returnPath = null;
            string destinationPath = Path.Combine(path.FullName, nameOnly);

            var isSameAsDestiny = assetPath.Equals(destinationPath, StringComparison.InvariantCultureIgnoreCase);
            var isLocalFile = assetPath.StartsWith(UA_RESOURCES + BASE_DIR);

            if (!isSameAsDestiny && !isLocalFile) // Avoid to copy the same origin to same destination files
            {
                var destination = destinationPath;
                File.Copy(assetPath, destination, true);
                File.SetAttributes(destination, FileAttributes.Normal);
                
                // File doesnt exist
                returnPath = assetTypeDir + "/" + nameOnly;
                var localAsset = localPath + "/" + nameOnly;

                AssetDatabase.ImportAsset(localAsset, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.Refresh();
                if(assetsCategory == AssetsConstants.CATEGORY_IMAGE)
                {
                    // Force unload 
                    var image = Controller.ResourceManager.getImage(returnPath);
                    Resources.UnloadAsset(image);
                }

                if (assetsCategory == AssetsConstants.CATEGORY_ANIMATION)
                {
                    var animation = Loader.LoadAnimation(returnPath, Controller.ResourceManager, new List<Incidence>());
                    if(animation != null)
                    {
                        var regularBasePath = assetPath.Replace("\\", "/");
                        var sourceAnimationBasePath = regularBasePath.Substring(0, regularBasePath.LastIndexOf("assets/animation"));
                        foreach (var frame in animation.getFrames())
                        {
                            var frameName = frame.getUri().Substring(frame.getUri().LastIndexOf("/") + 1);
                            var framePath = (sourceAnimationBasePath + frame.getUri()).Replace("/", "\\");
                            var destinationFramePath = Path.Combine(path.FullName, frameName);
                            File.Copy(framePath, destinationFramePath, true);
                            File.SetAttributes(destinationFramePath, FileAttributes.Normal);
                            AssetDatabase.ImportAsset(destinationFramePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                            AssetDatabase.Refresh();
                        }
                    }
                    else
                    {
                        Debug.LogError("The selected animation couldn't be loaded so the frames couldn't be copied.");
                    }
                }
            }
            else
            {
                // File already exists
                returnPath = assetTypeDir + "/" + nameOnly;
            }
            return returnPath;

        }
        

        /**
         * Deletes the given asset from the ZIP, asking for confirmation to the
         * user.
         * 
         * @param assetPath
         *            Path to the asset file to delete, relative to the ZIP file.
         * @return True if the file was deleted, false otherwise
         */
        public static bool DeleteAsset(string assetPath)
        {

            // Delete the asset and store if it has been deleted
            bool assetDeleted = DeleteAsset(assetPath, true);

            // If the asset was deleted, delete the references in the adventure
            Controller controller = Controller.Instance;
            if (assetDeleted)
            {
                // Delete the references to the asset
                controller.deleteAssetReferences(assetPath);
            }

            return assetDeleted;
        }

        /**
         * Deletes the given asset from the ZIP.
         * 
         * @param assetPath
         *            Path to the asset file to delete, relative to the ZIP file
         * @param askForConfirmation
         *            If true, asks the user for confirmation to delete
         * @return True if the asset was deleted, false otherwise
         */
        public static bool DeleteAsset(string assetPath, bool askForConfirmation)
        {

            bool assetDeleted = false;

            // Count the references, if it is an animation, remove the suffix to do the search
            string references = Controller.Instance.countAssetReferences(assetPath).ToString();

            var fileInfo = new FileInfo(assetPath);

            // If the asset must be deleted (when the user is not asked, or is asked and answers "Yes")
            if (!askForConfirmation || Controller.Instance.ShowStrictConfirmDialog( TC.get("Assets.DeleteAsset"),  TC.get("Assets.DeleteAssetWarning", new string[] { fileInfo.Name, references })))
            {

                // If it is an animation, delete all the files
                if (assetPath.StartsWith(CATEGORY_ANIMATION_FOLDER))
                {
                    // Set "assetDeleted" to true, to perform an AND operation with each result
                    assetDeleted = true;

                    // Prepare the root of the animation path and the suffix
                    assetPath = assetPath.RemoveFromEnd(fileInfo.Extension);

                    // While the last image has not been read
                    var animation = Loader.LoadAnimation(assetPath, Controller.ResourceManager, new List<Incidence>());
                    if(animation != null)
                    {
                        foreach (var frame in animation.getFrames())
                        {
                            var framePath = frame.getUri();
                            if (!string.IsNullOrEmpty(framePath))
                            {
                                DeleteAsset(framePath);
                            }
                            var soundPath = frame.getSoundUri();
                            if (!string.IsNullOrEmpty(soundPath))
                            {
                                DeleteAsset(soundPath);
                            }
                        }
                    }
                }

                // If it is not an animation, just delete the file
                else
                {
                    var path = UA_RESOURCES + BASE_DIR + assetPath;
                    assetDeleted = AssetDatabase.DeleteAsset(path);
                }
            }

            return assetDeleted;
        }

        private static void ImportAssets(string[] paths)
        {
            foreach (string path in paths)
                AssetDatabase.WriteImportSettingsIfDirty(path);
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (string path in paths)
                    AssetDatabase.ImportAsset(path);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        /**
         * Copies all the asset files from one ZIP to another.
         * 
         * @param sourceFile
         *            Source ZIP file
         * @param destinyFile
         *            Destiny ZIP file
         */



        public static void copyAssets(string sourceFile, string destinyFile)
        {
            string[] directoriesToCopy = new string[]
            {
                "assets", "gui"
            };
            
            foreach (var directory in directoriesToCopy)
            {
                var dest = Path.Combine(destinyFile, directory);
                var origin = Path.Combine(sourceFile, directory);

                if (!Directory.Exists(origin))
                {
                    Debug.LogWarning("Copying expected directory to exist: " + origin);
                    continue;
                }

                FileUtil.ReplaceDirectory(origin, dest);
            }

            ImportDirectory(destinyFile);
            EditorUtility.DisplayProgressBar("Importing project", "Removing special characters...", 0.7f);
            FixImportSpecialCharacters(destinyFile);
            EditorUtility.DisplayProgressBar("Importing project", "Modifying importer config...", 0.8f);
            ModifyImportSpecialCases(destinyFile);
        }

        public static void copyAllFiles(string sourceFile, string destinyFile)
        {
            EditorUtility.DisplayProgressBar("Importing project", "Copying files...", 0.4f);
            if (!Directory.Exists(sourceFile))
            {
                Debug.LogWarning("Copying expected directory to exist: " + sourceFile);
                return;
            }

            FileUtil.ReplaceDirectory(sourceFile, destinyFile);
            EditorUtility.DisplayProgressBar("Importing project", "Importing files...", 0.6f);

            ImportDirectory(destinyFile);
            EditorUtility.DisplayProgressBar("Importing project", "Removing special characters...", 0.7f);
            FixImportSpecialCharacters(destinyFile);
            EditorUtility.DisplayProgressBar("Importing project", "Modifying importer config...", 0.8f);
            ModifyImportSpecialCases(destinyFile);
        }


        private static bool addSpecialAsset(string uri, string destination)
        {
            // Add the defaultBook
            var finalDest = Path.Combine(Controller.Instance.ProjectFolder, destination);
            if (AssetDatabase.CopyAsset(UA_EDITOR_RESOURCES + uri, finalDest))
            {
                pathsToReimport.Add(finalDest);
                return true;
            }
            return false;
        }

        public static void addSpecialAssets()
        {
            pathsToReimport = new List<string>();

            if (!addSpecialAsset(SpecialAssetPaths.FILE_DEFAULT_BOOK_IMAGE, SpecialAssetPaths.ASSET_DEFAULT_BOOK_IMAGE))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_DEFAULT_BOOK_IMAGE));
            }

            if (!addSpecialAsset(SpecialAssetPaths.FILE_DEFAULT_ARROW_NORMAL, SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_DEFAULT_ARROW_NORMAL));
            }

            if (!addSpecialAsset(SpecialAssetPaths.FILE_DEFAULT_ARROW_OVER, SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_DEFAULT_ARROW_OVER));
            }

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_IMAGE, SpecialAssetPaths.ASSET_EMPTY_IMAGE))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_IMAGE));
            }

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_BACKGROUND, SpecialAssetPaths.ASSET_EMPTY_BACKGROUND))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_BACKGROUND));
            }

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_ICON, SpecialAssetPaths.ASSET_EMPTY_ICON))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_ICON));
            }

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_ANIMATION, SpecialAssetPaths.ASSET_EMPTY_ANIMATION + "_01.png"))
            {
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_ANIMATION));
            }

            ImportAssets(pathsToReimport.ToArray());
        }

        /**
         * Returns whether the given asset is a special one or not.
         * 
         * @param assetPath
         *            Asset path
         * @return True if the asset is special, false otherwise
         */
        public static bool isAssetSpecial(string assetPath)
        {

            // The three empty assets are considered as special
            return assetPath.Equals(SpecialAssetPaths.ASSET_EMPTY_IMAGE) ||
                   assetPath.Equals(SpecialAssetPaths.ASSET_EMPTY_ICON) ||
                   assetPath.Equals(SpecialAssetPaths.ASSET_EMPTY_ANIMATION);
        }
        

        ////    /**
        ////     * Checks the given asset to see if it fits the category. If the asset has
        ////     * some problem, a message is displayed to the user. If the asset already
        ////     * exists in the ZIP file, the user is prompted to overwrite it.
        ////     * 
        ////     * @param assetPath
        ////     *            Absolute path to the asset
        ////     * @param assetCategory
        ////     *            Category for the asset
        ////     * @return True if the asset can be added to the set, false otherwise
        ////     */
        ////    private static bool checkAsset(string assetPath, int assetCategory)
        ////    {

        ////        bool assetValid = true;

        ////        // For images, only those who have restricted dimension are checked
        ////        if (isImageWithRestrictedDimension(assetCategory))
        ////        {
        ////            // Take the instance of the controller, and the filename of the asset
        ////            Controller controller = Controller.getInstance();
        ////            string assetFilename = getFilename(assetPath);

        ////            // Take the data from the file
        ////            // Image image = new ImageIcon( assetPath ).getImage( );
        ////            Image image = getImage(assetPath);
        ////            int width = image.getWidth(null);
        ////            int height = image.getHeight(null);

        ////            // Prepare the string array for the error message
        ////            string[] fileInformation = new string[] { assetFilename, string.valueOf(width), string.valueOf(height) };

        ////            // Restrict dimensions for the asset category
        ////            Dimension d = getRestrictedDimension(assetCategory);
        ////            int res_width = (int)d.getWidth();
        ////            int res_height = (int)d.getHeight();

        ////            // Icon must be exactly restricted dimensions
        ////            if (assetCategory == CATEGORY_ICON)
        ////            {
        ////                if (width != res_width || height != res_height)
        ////                {
        ////                    controller.showErrorDialog( TC.get("IconAssets.Title"),  TC.get("IconAssets.ErrorIconSize", fileInformation));
        ////                    assetValid = false;
        ////                }
        ////            }
        ////            // Backgrond must be bigger than restricted dimensions
        ////            else if (assetCategory == AssetsConstants.CATEGORY_BACKGROUND)
        ////            {
        ////                if (width < res_width || height < res_height)
        ////                {
        ////                   // controller.showErrorDialog( TC.get("BackgroundAssets.Title"),  TC.get("BackgroundAssets.ErrorBackgroundSize", fileInformation));
        ////                    assetValid = false;
        ////                }
        ////            }
        ////            // Arrow book must be smaller than restricted dimensions
        ////            else if (assetCategory == AssetsConstants.CATEGORY_ARROW_BOOK)
        ////            {
        ////                if (width > res_width || height > res_height)
        ////                {
        ////                    //controller.showErrorDialog( TC.get("ArrowAssets.Title"),  TC.get("ArrowAssets.ErrorArrowSize", fileInformation));
        ////                    assetValid = false;
        ////                }
        ////            }
        ////        }

        ////        return assetValid;
        ////    }
        
        

        public static void checkAssetFilesConsistency(List<Incidence> incidences)
        {

            List<string> assetPaths = new List<string>();
            List<int> assetTypes = new List<int>();
            Controller controller = Controller.Instance;
            controller.getAssetReferences(assetPaths, assetTypes);

            for (int i = 0; i < assetPaths.Count; i++)
            {
                bool assetValid = true;
                string assetPath = assetPaths[i];
                int assetCategory = assetTypes[i];
                string message = "";
                bool notPresent = true;

                // Take the instance of the controller, and the filename of the asset
                FileInfo file = new FileInfo(Path.Combine(controller.ProjectFolder, assetPath));
                if (assetCategory == AssetsConstants.CATEGORY_ANIMATION)
                {
                    file = new FileInfo(Path.Combine(controller.ProjectFolder, assetPath + "_01.png"));
                    if (!file.Exists)
                    {
                        file = new FileInfo(Path.Combine(controller.ProjectFolder, assetPath + "_01.jpg"));
                    }
                    if (!file.Exists)
                    {
                        file = new FileInfo(Path.Combine(controller.ProjectFolder, assetPath));
                    }
                }
                assetValid = file.Exists && file.Length > 0;
                if (!assetValid)
                {
                    message = TC.get("Error.AssetNotFound" + assetCategory, assetPath);
                }

                // For images, only background and icon are checked
                if (assetValid && (assetCategory == AssetsConstants.CATEGORY_BACKGROUND || assetCategory == AssetsConstants.CATEGORY_ICON))
                {
                    // Take the data from the file
                    Sprite image = Controller.ResourceManager.getSprite(assetPath);
                    int width = (int)image.rect.width;
                    int height = (int)image.rect.height;

                    // Prepare the string array for the error message
                    string[] fileInformation = new string[] { assetPath, width.ToString(), height.ToString() };

                    // The background files must have a size of at least 800x600
                    if (assetCategory == AssetsConstants.CATEGORY_BACKGROUND && (width < AssetsImageDimensions.BACKGROUND_MAX_WIDTH || height < AssetsImageDimensions.BACKGROUND_MAX_HEIGHT))
                    {
                        message =  TC.get("BackgroundAssets.ErrorBackgroundSize", fileInformation);
                        assetValid = false;
                        notPresent = false;
                    }

                    // The icon files must have a size of 80x48
                    else if (assetCategory == AssetsConstants.CATEGORY_ICON && (width != AssetsImageDimensions.ICON_MAX_WIDTH || height != AssetsImageDimensions.ICON_MAX_HEIGHT))
                    {
                        message =  TC.get("IconAssets.ErrorIconSize", fileInformation);
                        assetValid = false;
                        notPresent = false;
                    }
                }

                if (!assetValid)
                {
                    incidences.Add(Incidence.createAssetIncidence(notPresent, assetCategory, message, assetPath, null));
                }
            }
        }

        ////    /**
        ////     * Returns the folder associated with the given category.
        ////     * 
        ////     * @param assetsCategory
        ////     *            Category for the folder
        ////     * @return Folder for the given category, null if the category was not
        ////     *         recognized
        ////     */
        public static string getCategoryFolder(int assetsCategory)
        {

            string folder = null;

            switch (assetsCategory)
            {
                case AssetsConstants.CATEGORY_BACKGROUND:
                    folder = CATEGORY_BACKGROUND_FOLDER;
                    break;
                case AssetsConstants.CATEGORY_ANIMATION:
                case AssetsConstants.CATEGORY_ANIMATION_IMAGE:
                case AssetsConstants.CATEGORY_ANIMATION_AUDIO:
                    folder = CATEGORY_ANIMATION_FOLDER;
                    break;
                case AssetsConstants.CATEGORY_IMAGE:
                    folder = CATEGORY_IMAGE_FOLDER;
                    break;
                case AssetsConstants.CATEGORY_ICON:
                    folder = CATEGORY_ICON_FOLDER;
                    break;
                case AssetsConstants.CATEGORY_AUDIO:
                    folder = CATEGORY_AUDIO_PATH;
                    break;
                case AssetsConstants.CATEGORY_VIDEO:
                    folder = CATEGORY_VIDEO_PATH;
                    break;
                case AssetsConstants.CATEGORY_CURSOR:
                    folder = CATEGORY_CURSOR_PATH;
                    break;
                case AssetsConstants.CATEGORY_STYLED_TEXT:
                    folder = CATEGORY_STYLED_TEXT_PATH;
                    break;
                case AssetsConstants.CATEGORY_BUTTON:
                    folder = CATEGORY_BUTTON_PATH;
                    break;
                case AssetsConstants.CATEGORY_ARROW_BOOK:
                    folder = CATEGORY_ARROW_BOOK_PATH;
                    break;

            }

            return folder;
        }

        ////    /**
        ////     * Returns the file filter associated with the given category and filter.
        ////     * 
        ////     * @param assetsCategory
        ////     *            Category of the asset
        ////     * @param filter
        ////     *            Specific filter for the category
        ////     * @return File filter for the category and filter
        ////     */
        public static string getAssetsFileFilter(int assetsCategory, int filter)
        {

            string fileFilter = null;

            //switch (assetsCategory)
            //{
            //    /*case CATEGORY_ASSESSMENT:
            //    case CATEGORY_ADAPTATION:
            //        fileFilter = new XMLFileFilter( );
            //        break;*/
            //    case AssetsConstants.CATEGORY_BACKGROUND:
            //        // NOTE: In this category, subfilters are now ignored
            //        //if( filter == FILTER_NONE )
            //        fileFilter = new ImageFileFilter();
            //        //if( filter == FILTER_JPG )
            //        //	fileFilter = new JPGFileFilter( );
            //        //if( filter == FILTER_PNG )
            //        //	fileFilter = new PNGFileFilter( );
            //        break;
            //    case AssetsConstants.CATEGORY_ANIMATION:
            //        if (filter == FILTER_NONE)
            //            fileFilter = new AnimationFileFilter();
            //        if (filter == FILTER_JPG)
            //            fileFilter = new JPGSlidesFileFilter();
            //        if (filter == FILTER_PNG)
            //            fileFilter = new PNGAnimationFileFilter();
            //        break;
            //    case AssetsConstants.CATEGORY_IMAGE:
            //    case AssetsConstants.CATEGORY_CURSOR:
            //    case AssetsConstants.CATEGORY_ICON:
            //    case AssetsConstants.CATEGORY_BUTTON:
            //    case AssetsConstants.CATEGORY_ARROW_BOOK:
            //        fileFilter = new ImageFileFilter();
            //        break;
            //    case AssetsConstants.CATEGORY_AUDIO:
            //        if (filter == FILTER_NONE)
            //            fileFilter = new AudioFileFilter();
            //        if (filter == FILTER_MP3)
            //            fileFilter = new MP3FileFilter();
            //        break;
            //    case AssetsConstants.CATEGORY_VIDEO:
            //        fileFilter = new VideoFileFilter();
            //        break;
            //    case AssetsConstants.CATEGORY_STYLED_TEXT:
            //        fileFilter = new FormattedTextFileFilter();
            //        break;
            //}

            return fileFilter;
        }

        ////    public static AssetChooser getAssetChooser(int category, int filter)
        ////    {

        ////        AssetChooser assetChooser = null;
        ////        switch (category)
        ////        {
        ////            case AssetsConstants.CATEGORY_ANIMATION:
        ////                assetChooser = new AnimationChooser(filter);
        ////                break;
        ////            case AssetsConstants.CATEGORY_ICON:
        ////                assetChooser = new IconChooser(filter);
        ////                break;
        ////            case AssetsConstants.CATEGORY_IMAGE:
        ////            case AssetsConstants.CATEGORY_ARROW_BOOK:
        ////                assetChooser = new ImageChooser(filter, category);
        ////                break;
        ////            case AssetsConstants.CATEGORY_BACKGROUND:
        ////                assetChooser = new BackgroundChooser(filter);
        ////                break;
        ////            case AssetsConstants.CATEGORY_AUDIO:
        ////                assetChooser = new AudioChooser(filter);
        ////                break;
        ////            case AssetsConstants.CATEGORY_VIDEO:
        ////                assetChooser = new VideoChooser();
        ////                break;
        ////            case AssetsConstants.CATEGORY_CURSOR:
        ////                assetChooser = new CursorChooser();
        ////                break;
        ////            case AssetsConstants.CATEGORY_STYLED_TEXT:
        ////                assetChooser = new FormatedTextChooser();
        ////                break;
        ////            case AssetsConstants.CATEGORY_BUTTON:
        ////                assetChooser = new ButtonChooser();
        ////                break;

        ////        }
        ////        return assetChooser;
        ////    }

        internal static void FixImportSpecialCharacters(string assetFolder)
        {
            pathsToReimport = new List<string>();

            foreach (var assetPath in AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith(assetFolder)))
            {
                // Remove all accents from the file
                var goodName = RemoveDiacritics(assetPath);
                if (goodName != assetPath)
                {
                    File.Move(assetPath, goodName);
                    pathsToReimport.Add(goodName);
                }

                // In case of animation
                if (goodName.EndsWith(".eaa", StringComparison.InvariantCultureIgnoreCase))
                {
                    var newName = goodName + ".xml";
                    File.Move(goodName, newName);
                    pathsToReimport.Add(newName);
                }
            }

            ImportAssets(pathsToReimport.ToArray());
        }

        internal static void ModifyImportSpecialCases(string assetFolder)
        {
            pathsToReimport = new List<string>();

            foreach(var assetPath in AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith(assetFolder) && path.EndsWith(".xml") && File.Exists(path)))
            {
                var text = File.ReadAllText(assetPath);
                text = RemoveDiacritics(text);
                text = text.Replace(".eaa\"", ".eaa.xml\"");
                WriteAllTextWithFlush(assetPath, text);
                pathsToReimport.Add(assetPath);
            }

            ImportAssets(pathsToReimport.ToArray());
        }

        public static void WriteAllTextWithFlush(string path, string contents)
        {
            // generate a temp filename
            var tempPath = Path.GetTempFileName();

            // create the backup name
            var backup = path + ".tmp";

            // delete any existing backups
            if (File.Exists(backup))
                File.Delete(backup);

            // get the bytes
            var data = Encoding.UTF8.GetBytes(contents);

            // write the data to a temp file
            using (var tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough))
                tempFile.Write(data, 0, data.Length);

            // replace the contents
            File.Replace(tempPath, path, backup);
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        internal static void ImportDirectory(string DestinationPath)
        {
            if (!DestinationPath.StartsWith("Assets/"))
            {
                Debug.LogError("Cannot import an external path");
            }

            AssetDatabase.ImportAsset(DestinationPath, ImportAssetOptions.ImportRecursive);
        }


        [Obsolete]
        public static void copyAssetsWithoutDeleteCurrentFiles(string sourceFile, string destinyFile)
        {
            pathsToReimport = new List<string>();
            DirectoryInfo assets = new DirectoryInfo(sourceFile + "/assets");
            if (assets.Exists)
            {
                DirectoryInfo destinyAssets = new DirectoryInfo(destinyFile + "/assets");
                DirectoryCopy(assets.FullName, destinyAssets.FullName, true);
            }
            DirectoryInfo gui = new DirectoryInfo(sourceFile + "/gui");
            if (gui.Exists)
            {
                DirectoryInfo destinyGui = new DirectoryInfo(destinyFile + "/gui");
                DirectoryCopy(gui.FullName, destinyGui.FullName, true);
            }
            ImportAssets(pathsToReimport.ToArray());
        }

        [Obsolete]
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.IsuAdventureRelevant())
                    continue;

                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);

                // In case of animation
                if (file.Extension.ToLowerInvariant() == ".eaa")
                {
                    File.Move(temppath, temppath + ".xml");
                }

                // In case of animation
                if (file.Extension.ToLowerInvariant() == ".xml")
                {
                    string text = File.ReadAllText(temppath);
                    text = text.Replace(".eaa\"", ".eaa.xml\"");
                    File.WriteAllText(temppath, text);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uAdventure.Core;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    /**
     * This class is responsible for managing the multimedia data stored in the
     * adventure ZIP files. It is also responsible for the assessment and adaptation
     * files of the adventures.
     */

    public class AssetsController
    {
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

        /**
         * Static class. Private constructor.
         */

        private AssetsController()
        {

        }

        //private static VideoCache videoCache = new AssetsController.VideoCache( );

        private static Dictionary<string, FileInfo> tempFiles = new Dictionary<string, FileInfo>();

        public static void resetCache()
        {

            //Reset video cache
            /*videoCache.clean( );
            videoCache.reset( );
            string[] videoAssets = getAssetsList( CATEGORY_VIDEO );
            for( string videoAsset : videoAssets ) {

                // Add the file
                videoCache.cacheVideo( videoAsset );
            }*/

            //Reset tempFiles
            tempFiles = new Dictionary<string, FileInfo>();
        }

        /*public static void cleanVideoCache( ) {
            videoCache.clean( );
        }*/

        public static string[] categoryFolders()
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

        public static void createFolderStructure()
        {
            DirectoryInfo projectDir = new DirectoryInfo(Controller.Instance.ProjectFolder);
            Debug.Log("CREATE: " + projectDir.FullName);
            string[] folders = categoryFolders();
            for (int i = 0; i < folders.Length; i++)
            {
                DirectoryInfo category = projectDir.CreateSubdirectory(folders[i]);
            }
            projectDir.CreateSubdirectory("assets/special/");

            // Copy eadventure.dtd, descriptor.dtd, assessment.dtd, adaptation.dtd
            FileInfo descriptorDTD = new FileInfo(Path.Combine("Assets/Resources/" + EADVETURE_CONTENT_FOLDER, "descriptor.dtd"));
            if (descriptorDTD.Exists)
            {
                // TODO: Check if its working
                descriptorDTD.CopyTo(Path.Combine(projectDir.FullName, "descriptor.dtd"), true);
            }
            else
            {
                Controller.Instance.ShowErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " descriptor.dtd");
            }

            // eadventure.dtd
            FileInfo eadventureDTD = new FileInfo(Path.Combine("Assets/Resources/" + EADVETURE_CONTENT_FOLDER, "eadventure.dtd"));
            if (eadventureDTD.Exists)
            {
                // TODO: Check if its working
                eadventureDTD.CopyTo(Path.Combine(projectDir.FullName, "eadventure.dtd"), true);
            }
            else
            {
                Controller.Instance.ShowErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " eadventure.dtd");
            }

            // assessment.dtd
            FileInfo assessmentDTD = new FileInfo("Assets/Resources/" + Path.Combine(EADVETURE_CONTENT_FOLDER, "assessment.dtd"));
            if (assessmentDTD.Exists)
            {
                // TODO: Check if its working
                assessmentDTD.CopyTo(Path.Combine(projectDir.FullName, "assessment.dtd"), true);
            }
            else
            {
                Controller.Instance.ShowErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " assessment.dtd");
            }


            // adaptation.dtd
            FileInfo adaptationDTD = new FileInfo(Path.Combine("Assets/Resources/" + EADVETURE_CONTENT_FOLDER, "adaptation.dtd"));
            if (adaptationDTD.Exists)
            {
                // TODO: Check if its working
                adaptationDTD.CopyTo(Path.Combine(projectDir.FullName, "adaptation.dtd"), true);
            }
            else
            {
                Controller.Instance.ShowErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " adaptation.dtd");
            }

            // animation.dtd
            FileInfo animationDTD = new FileInfo(Path.Combine("Assets/Resources/" + EADVETURE_CONTENT_FOLDER, "animation.dtd"));
            if (animationDTD.Exists)
            {
                // TODO: Check if its working
                animationDTD.CopyTo(Path.Combine(projectDir.FullName, "animation.dtd"), true);
            }
            else
            {
                Controller.Instance.ShowErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " animation.dtd");
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

        public static string[] getAssetFilenames(int assetsCategory)
        {

            return getAssetFilenames(assetsCategory, FILTER_NONE);
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

        public static string[] getAssetFilenames(int assetsCategory, int filter)
        {

            string[] assetsList = new string[] { };

            // Take the category folder, from the ZIP file name
            DirectoryInfo categoryFolder =
                new DirectoryInfo(Path.Combine(Controller.Instance.ProjectFolder,
                    getCategoryFolder(assetsCategory)));

            // Take the file list and create the array
            //FileInfo[] fileList = categoryFolder.listFiles( getAssetsFileFilter( assetsCategory, filter ), categoryFolder.getArchiveDetector( ) );
            FileInfo[] fileList = categoryFolder.GetFiles(getAssetsFileFilter(assetsCategory, filter));

            // If the array is not empty
            if (fileList != null)
            {
                // Copy the relative paths to the string array

                // If is styled text, remove referenced files (folder) when present
                if (assetsCategory != AssetsConstants.CATEGORY_STYLED_TEXT)
                {
                    assetsList = new string[fileList.Length];
                    for (int i = 0; i < fileList.Length; i++)
                        assetsList[i] = fileList[i].Name;
                }
                else
                {
                    ////Remove those files which are directories
                    //List<FileInfo> filteredFileList = new List<FileInfo>();
                    //for (int i = 0; i < fileList.Length; i++)
                    //{
                    //    if (!fileList[i].isDirectory())
                    //        filteredFileList.add(fileList[i]);
                    //}

                    //assetsList = new string[filteredFileList.size()];
                    //for (int i = 0; i < filteredFileList.size(); i++)
                    //    assetsList[i] = filteredFileList[i].getName();
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

        public static string[] getAssetsList(int assetsCategory)
        {

            return getAssetsList(assetsCategory, FILTER_NONE);
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

        public static string[] getAssetsList(int assetsCategory, int filter)
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

        /**
         * Returns an image corresponding to the given file path (relative to the
         * ZIP).
         * 
         * @param imagePath
         *            Path to the image, relative to the ZIP file
         * @return Image for the given file
         */

        [Obsolete("Use ResourceManager instead")]
        public static Sprite getImage(string imagePath)
        {
            return Controller.ResourceManager.getSprite(imagePath);
        }

        /**
         * Returns an image corresponding to the given file path (relative to the
         * ZIP).
         * 
         * @param imagePath
         *            Path to the image, relative to the ZIP file
         * @return Image for the given file
         */

        [Obsolete("Use ResourceManager instead")]
        public static Texture2D getImageTexture(string imagePath)
        {
            return Controller.ResourceManager.getImage(imagePath);
        }


        public static bool addSingleAsset(int assetsCategory, string assetPath)
        {
            return addSingleAsset(assetsCategory, assetPath, true);
        }

        public static bool addSingleAsset(int assetsCategory, string assetPath, bool checkIfAssetExists)
        {
            return addSingleAsset(assetsCategory, assetPath, null, checkIfAssetExists);
        }

        public static bool addSingleAsset(int assetsCategory, string assetPath, string destinyAssetName,
            bool checkIfAssetExists)
        {

            bool assetsAdded = false;
            // Take the category folder, from the ZIP file name
            //File categoryFolder = new File(Controller.getInstance().getProjectFolder(), getCategoryFolder(assetsCategory));

            //// Check if the file is correct, and is going to be added
            //if (checkAsset(assetPath, assetsCategory))
            //{

            //    // If it is an animation asset, add all the images of the animation
            //    if (assetsCategory == AssetsConstants.CATEGORY_ANIMATION && !assetPath.EndsWith(".eaa"))
            //    {
            //        // Prepare the root of the animation path and the extension
            //        string extension = getExtension(assetPath);
            //        assetPath = removeSuffix(assetPath);

            //        // While the last image has not been read
            //        bool end = false;
            //        for (int i = 1; i < 100 && !end; i++)
            //        {
            //            // Open source file
            //            File sourceFile = new File(assetPath + string.format("_%02d.", i) + extension);

            //            // If the file exists, create the destiny file and do the copy
            //            if (sourceFile.Exists)
            //            {
            //                File destinyFile = new File(categoryFolder, destinyAssetName == null ? sourceFile.getName() : destinyAssetName);

            //                // Check those are not the same file
            //                if (!sourceFile.getAbsolutePath().ToLower().Equals(destinyFile.getAbsolutePath().ToLower()))
            //                {

            //                    //If is directory, copy all contents
            //                    if (sourceFile.isDirectory())
            //                        assetsAdded = sourceFile.copyAllTo(destinyFile);
            //                    else
            //                        assetsAdded = sourceFile.CopyTo(destinyFile);
            //                }
            //                else {
            //                    assetsAdded = false;
            //                    end = true;
            //                }
            //            }

            //            // If it doesn't exist, stop loading data
            //            else
            //                end = true;
            //        }
            //    }
            //    else if (assetsCategory == AssetsConstants.CATEGORY_ANIMATION)
            //    {

            //        Animation animation = Loader.loadAnimation(AssetsController.getInputStreamCreator(), assetPath, new EditorImageLoader());
            //        animation.setAbsolutePath(assetPath);
            //        File sourceFile = new File(assetPath);
            //        File destinyFile;
            //        // empty animation always goes to assets/special folder
            //        if (sourceFile.getName().contains("EmptyAnimation"))
            //        {
            //            destinyFile = new File(Controller.getInstance().getProjectFolder() + "/" + CATEGORY_SPECIAL_ASSETS + "/" + destinyAssetName == null ? sourceFile.getName() : destinyAssetName);
            //        }
            //        else
            //            destinyFile = new File(categoryFolder, destinyAssetName == null ? sourceFile.getName() : destinyAssetName);

            //        if (!sourceFile.getAbsolutePath().ToLower().Equals(destinyFile.getAbsolutePath().ToLower()))
            //        {
            //            if (destinyFile.Exists && !Controller.getInstance().showStrictConfirmDialog( TC.get("Assets.AddAsset"),  TC.get("Assets.WarningAssetFound", destinyFile.getName())))
            //            {
            //                deleteAsset(assetPath, false);
            //            }
            //            assetsAdded = sourceFile.CopyTo(destinyFile);
            //        }
            //        else {
            //            assetsAdded = true;
            //        }

            //        if (assetsAdded)
            //        {
            //            for (Frame frame : animation.getFrames())
            //            {
            //                string image = frame.getImageAbsolutePath();
            //                sourceFile = new File(image);
            //                destinyFile = new File(categoryFolder, sourceFile.getName());
            //                sourceFile.CopyTo(destinyFile);

            //                string sound = frame.getSoundAbsolutePath();
            //                if (sound != null)
            //                {
            //                    sourceFile = new File(sound);
            //                    destinyFile = new File(categoryFolder, sourceFile.getName());
            //                    sourceFile.CopyTo(destinyFile);
            //                }
            //            }
            //        }

            //    }

            //    // If it's a styled text, images associated with it have to be imported too
            //    else if (assetsCategory == CATEGORY_STYLED_TEXT && (assetPath.EndsWith(".html") || assetPath.EndsWith(".htm")))
            //    {
            //        assetsAdded = addStyledText(assetPath, destinyAssetName, categoryFolder);
            //    }
            //    // If it is not an animation asset, just add the file
            //    else {
            //        // Open source file, and create destiny file in the ZIP
            //        File sourceFile = new File(assetPath);
            //        File destinyFile = new File(categoryFolder, destinyAssetName == null ? sourceFile.getName() : destinyAssetName);

            //        // Check those are not the same file
            //        if (!sourceFile.getAbsolutePath().ToLower().Equals(destinyFile.getAbsolutePath().ToLower()))
            //        {

            //            // Check if the asset is being overwritten, if so prompt the user for action
            //            if (checkIfAssetExists && destinyFile.Exists && !Controller.getInstance().showStrictConfirmDialog( TC.get("Assets.AddAsset"),  TC.get("Assets.WarningAssetFound", destinyFile.getName())))
            //            {
            //                // If the user accepts to overwrite the asset, delete it first
            //                deleteAsset(assetPath, false);
            //            }

            //            // Copy the data
            //            //If is directory, copy all contents
            //            if (sourceFile.isDirectory())
            //                assetsAdded = sourceFile.copyAllTo(destinyFile);
            //            else
            //                assetsAdded = sourceFile.CopyTo(destinyFile);
            //        }
            //        else {
            //            assetsAdded = true;
            //        }

            //        //If it is a video, cache it 
            //        /*if( assetsCategory == CATEGORY_VIDEO ) {
            //            if( !videoCache.isVideoCachedZip( getCategoryFolder( assetsCategory ) + "/" + sourceFile.getName( ) ) )
            //                videoCache.cacheVideo( getCategoryFolder( assetsCategory ) + "/" + sourceFile.getName( ), sourceFile.getAbsolutePath( ) );
            //        }*/
            //    }
            //}
            return assetsAdded;

        }

        ////    private static bool addStyledText(string assetPath, string destinyAssetName, File categoryFolder)
        ////    {

        ////        bool assetsAdded = true;

        ////        try
        ////        {
        ////            File sourceFile = new File(assetPath);
        ////            File destinyFile = new File(categoryFolder, destinyAssetName == null ? sourceFile.getName() : destinyAssetName);

        ////            // Read sourceFile content
        ////            BufferedReader r = new BufferedReader(new FileReader(sourceFile));
        ////            string html = "";
        ////            string line = null;
        ////            while ((line = r.readLine()) != null)
        ////            {
        ////                html += line + "\n";
        ////            }
        ////            r.close();

        ////            // Look for css sheet
        ////            if (html.indexOf("<link rel=\"stylesheet\"") != -1)
        ////            {
        ////                string cssFile = html.Substring(html.indexOf("href=\"") + 6, html.indexOf('"', html.indexOf("href=\"") + 8));
        ////                File sourceCssFile = new File(sourceFile.getParent(), cssFile);
        ////                File destinyCssFile = new File(categoryFolder, cssFile);
        ////                assetsAdded = sourceCssFile.CopyTo(destinyCssFile);

        ////            }

        ////            // Look for images
        ////            string htmlProcessed = new string(html);
        ////            while (assetsAdded && htmlProcessed.indexOf("src=\"") != -1)
        ////            {
        ////                htmlProcessed = htmlProcessed.Substring(htmlProcessed.indexOf("src=\"") + 5, htmlProcessed.Length - 1);
        ////                string imgName = htmlProcessed.Substring(0, htmlProcessed.indexOf('"'));
        ////                // Copy image to project folder
        ////                File sourceImgFile = new File(sourceFile.getParent(), imgName);
        ////                File destinyImgFile = new File(categoryFolder, imgName);
        ////                assetsAdded = sourceImgFile.CopyTo(destinyImgFile);
        ////            }

        ////            assetsAdded = sourceFile.CopyTo(destinyFile);
        ////            /*BufferedWriter w = new BufferedWriter( new FileWriter( destinyFile ) );
        ////            w.write( html.replaceAll( "src=\"", "src=\"" + folderName ) );
        ////            w.close( );*/

        ////        }
        ////        catch (FileNotFoundException e)
        ////        {
        ////            assetsAdded = false;
        ////        }
        ////        catch (IOException e)
        ////        {
        ////            assetsAdded = false;
        ////        }

        ////        return assetsAdded;

        ////    }

        ////    /**
        ////     * Deletes the given asset from the ZIP, asking for confirmation to the
        ////     * user.
        ////     * 
        ////     * @param assetPath
        ////     *            Path to the asset file to delete, relative to the ZIP file.
        ////     * @return True if the file was deleted, false otherwise
        ////     */
        ////    public static bool deleteAsset(string assetPath)
        ////    {

        ////        // Delete the asset and store if it has been deleted
        ////        bool assetDeleted = deleteAsset(assetPath, true);

        ////        // If the asset was deleted, delete the references in the adventure
        ////        Controller controller = Controller.getInstance();
        ////        if (assetDeleted)
        ////        {
        ////            // Delete the references to the asset
        ////            if (assetPath.StartsWith(CATEGORY_ANIMATION_FOLDER))
        ////                controller.deleteAssetReferences(removeSuffix(assetPath));
        ////            else
        ////                controller.deleteAssetReferences(assetPath);
        ////        }

        ////        return assetDeleted;
        ////    }

        ////    /**
        ////     * Deletes the given asset from the ZIP.
        ////     * 
        ////     * @param assetPath
        ////     *            Path to the asset file to delete, relative to the ZIP file
        ////     * @param askForConfirmation
        ////     *            If true, asks the user for confirmation to delete
        ////     * @return True if the asset was deleted, false otherwise
        ////     */
        ////    public static bool deleteAsset(string assetPath, bool askForConfirmation)
        ////    {

        ////        bool assetDeleted = false;

        ////        // Count the references, if it is an animation, remove the suffix to do the search
        ////        string references = null;
        ////        if (assetPath.StartsWith(CATEGORY_ANIMATION_FOLDER))
        ////            references = string.valueOf(Controller.getInstance().countAssetReferences(removeSuffix(assetPath)));
        ////        else
        ////            references = string.valueOf(Controller.getInstance().countAssetReferences(assetPath));

        ////        // If the asset must be deleted (when the user is not asked, or is asked and answers "Yes")
        ////        if (!askForConfirmation || Controller.getInstance().showStrictConfirmDialog( TC.get("Assets.DeleteAsset"),  TC.get("Assets.DeleteAssetWarning", new string[] { getFilename(assetPath), references })))
        ////        {

        ////            // If it is an animation, delete all the files
        ////            if (assetPath.StartsWith(CATEGORY_ANIMATION_FOLDER))
        ////            {
        ////                // Set "assetDeleted" to true, to perform an AND operation with each result
        ////                assetDeleted = true;

        ////                // Prepare the root of the animation path and the suffix
        ////                string extension = getExtension(assetPath);
        ////                assetPath = removeSuffix(assetPath);

        ////                // While the last image has not been read
        ////                bool end = false;
        ////                for (int i = 1; i < 100 && !end; i++)
        ////                {
        ////                    // Open the file to be deleted
        ////                    File animationFrameFile = new File(Controller.getInstance().getProjectFolder(), assetPath + string.format("_%02d.", i) + extension);

        ////                    // If the file exists, delete it
        ////                    if (animationFrameFile.Exists)
        ////                        assetDeleted &= animationFrameFile.delete();

        ////                    // If it doesn't exist, stop deleting data
        ////                    else
        ////                        end = true;
        ////                }
        ////            }

        ////            // If it is not an animation, just delete the file
        ////            else
        ////                assetDeleted = new File(Controller.getInstance().getProjectFolder(), assetPath).delete();
        ////        }

        ////        return assetDeleted;
        ////    }

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
            EditorUtility.DisplayProgressBar("Importing project", "Modifying importer config...", 0.8f);
            ModifyImportSpecialCases(destinyFile);
        }


        private static bool addSpecialAsset(string uri, string destination)
        {
            // Add the defaultBook
            FileInfo sourceFile = new FileInfo("Assets/Resources/" + uri);
            if (sourceFile.Exists)
            {
                var finalFile = Path.Combine(Controller.Instance.ProjectFolder, destination);
                sourceFile.CopyTo(finalFile, true);
                InitImporterConfig(finalFile);
                return true;
            }
            // If the source file doesn't exist, show an error message
            else
                return false;
        }

        public static void addSpecialAssets()
        {
            pathsToReimport = new List<string>();

            if (!addSpecialAsset(SpecialAssetPaths.FILE_DEFAULT_BOOK_IMAGE, SpecialAssetPaths.ASSET_DEFAULT_BOOK_IMAGE))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"),TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_DEFAULT_BOOK_IMAGE));

            if (!addSpecialAsset(SpecialAssetPaths.FILE_DEFAULT_ARROW_NORMAL, SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_DEFAULT_ARROW_NORMAL));

            if (!addSpecialAsset(SpecialAssetPaths.FILE_DEFAULT_ARROW_OVER, SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_DEFAULT_ARROW_OVER));

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_IMAGE, SpecialAssetPaths.ASSET_EMPTY_IMAGE))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_IMAGE));

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_BACKGROUND, SpecialAssetPaths.ASSET_EMPTY_BACKGROUND))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_BACKGROUND));

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_ICON, SpecialAssetPaths.ASSET_EMPTY_ICON))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_ICON));

            if (!addSpecialAsset(SpecialAssetPaths.FILE_EMPTY_ANIMATION, SpecialAssetPaths.ASSET_EMPTY_ANIMATION + "_01.png"))
                Controller.Instance.ShowErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", SpecialAssetPaths.FILE_EMPTY_ANIMATION));

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
                //Debug.Log(controller.getProjectFolder() + " | " + assetPath + " | " + file.FullName);
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
                    Sprite image = getImage(assetPath);
                    int width = (int)image.rect.width;
                    int height = (int)image.rect.height;

                    // Prepare the string array for the error message
                    string[] fileInformation = new string[] { assetPath, width.ToString(), height.ToString() };

                    // The background files must have a size of at least 800x600
                    if (assetCategory == AssetsConstants.CATEGORY_BACKGROUND && (width < AssetsImageDimensions.BACKGROUND_MAX_WIDTH || height < AssetsImageDimensions.BACKGROUND_MAX_HEIGHT))
                    {
                        // message =  TC.get("BackgroundAssets.ErrorBackgroundSize", fileInformation);
                        assetValid = false;
                        notPresent = false;
                    }

                    // The icon files must have a size of 80x48
                    else if (assetCategory == AssetsConstants.CATEGORY_ICON && (width != AssetsImageDimensions.ICON_MAX_WIDTH || height != AssetsImageDimensions.ICON_MAX_HEIGHT))
                    {
                        //message =  TC.get("IconAssets.ErrorIconSize", fileInformation);
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
        

        // TODO: TMP - delte iT
        public class FileFilter
        {
            public string getAssetsFileFilter(int assetsCategory, int filter)
            {
                return string.Empty;
            }
        }

        private static List<string> pathsToReimport;

        private static bool InitTextureImporter(TextureImporter textureImporter, bool saveInstant = false)
        {
            if (!textureImporter)
                return false;
            
            textureImporter.isReadable = true;
            textureImporter.npotScale = TextureImporterNPOTScale.None;
            if (saveInstant || pathsToReimport == null)
            {
                textureImporter.SaveAndReimport();
            }
            else
            {
                pathsToReimport.Add(textureImporter.assetPath);
            }

            return true;
        }
        private static bool InitVideoClipImporter(VideoClipImporter videoClipImporter, bool saveInstant = false)
        {
            if (!videoClipImporter)
                return false;

            var width = videoClipImporter.GetResizeWidth(VideoResizeMode.OriginalSize);
            var height = videoClipImporter.GetResizeHeight(VideoResizeMode.OriginalSize);
            var ratio = width / (float)height;
            
            // Default for standalone
            videoClipImporter.defaultTargetSettings = new VideoImporterTargetSettings()
            {
                enableTranscoding = true,
                resizeMode = width <= 1920 || height <= 1080 ? VideoResizeMode.OriginalSize : VideoResizeMode.CustomSize,
                customWidth = ratio > 1 ? 1920 : Mathf.FloorToInt(1080 * ratio),
                customHeight = ratio < 1 ? 1080 : Mathf.FloorToInt(1920 / ratio),
                aspectRatio = VideoEncodeAspectRatio.NoScaling,
                codec = VideoCodec.Auto,
                spatialQuality = VideoSpatialQuality.HighSpatialQuality,
                bitrateMode = VideoBitrateMode.High
            };


            // Valid names: 'Default', 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'XBox360', 'XboxOne', 'WP8', or 'WSA'
            // From: https://github.com/Unity-Technologies/UnityCsReference/blob/11bcfd801fccd2a52b09bb6fd636c1ddcc9f1705/artifacts/generated/bindings_old/common/Editor/VideoImporterBindings.gen.cs#L224
            
            // Android
            videoClipImporter.SetTargetSettings("Android", new VideoImporterTargetSettings()
            {
                enableTranscoding = true,
                resizeMode = width <= 1280 || height <= 720 ? VideoResizeMode.OriginalSize : VideoResizeMode.CustomSize,
                customWidth = ratio > 1 ? 1280 : Mathf.FloorToInt(720 * ratio),
                customHeight = ratio < 1 ? 720 : Mathf.FloorToInt(1280 / ratio),
                aspectRatio = VideoEncodeAspectRatio.NoScaling,
                codec = VideoCodec.Auto,
                spatialQuality = VideoSpatialQuality.LowSpatialQuality,
                bitrateMode = VideoBitrateMode.Low
            });

            // iOS
            videoClipImporter.SetTargetSettings("iOS", new VideoImporterTargetSettings()
            {
                enableTranscoding = true,
                resizeMode = width <= 1280 || height <= 720 ? VideoResizeMode.OriginalSize : VideoResizeMode.CustomSize,
                customWidth = ratio > 1 ? 1280 : Mathf.FloorToInt(720 * ratio),
                customHeight = ratio < 1 ? 720 : Mathf.FloorToInt(1280 / ratio),
                aspectRatio = VideoEncodeAspectRatio.NoScaling,
                codec = VideoCodec.Auto,
                spatialQuality = VideoSpatialQuality.MediumSpatialQuality,
                bitrateMode = VideoBitrateMode.Medium
            });

            if (saveInstant || pathsToReimport == null)
            {
                videoClipImporter.SaveAndReimport();
            }
            else
            {
                pathsToReimport.Add(videoClipImporter.assetPath);
            }

            return true;
        }

        public static bool InitImporterConfig(string path)
        {
            string currentDir = Directory.GetCurrentDirectory();
            // Clean path
            path = path.Replace(currentDir, "").Replace("\\", "/");
            if (path.Length > 0 && path[0] == '/') path = path.Remove(0, 1);
            if (!path.StartsWith("Assets/"))
            {
                Debug.LogWarning("Initing importer config for a route not starting with Assets: \"" + path + '"');
            }

            var file = new FileInfo(path);
            if (!file.Exists)
            {
                Debug.LogWarning("File not found while importing: " + path);
            }

            var importer = AssetImporter.GetAtPath(path);
            if (!importer) // If the importer doesn't exist, the asset might haven't been imported yet
            {
                // We import it
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate); 
                // And then the importer will be accesible
                importer = AssetImporter.GetAtPath(path);
                if (!importer) // But if we don't have importer yet it might be failing to import
                {
                    Debug.LogWarning("Failed to import: \"" + path + "\"\nPlease check the file and change the import settings manually");
                    return false;
                }
            }
            
            if (importer is TextureImporter)
                InitTextureImporter(importer as TextureImporter);

            if (importer is VideoClipImporter)
                InitVideoClipImporter(importer as VideoClipImporter);

            return importer != null;
        }

        internal static void ModifyImportSpecialCases(string assetFolder)
        {
            pathsToReimport = new List<string>();

            foreach(var assetPath in AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith(assetFolder)))
            {
                InitImporterConfig(assetPath); 
                if (assetPath.Remove(0, assetFolder.Length).StartsWith("/gui/cursors/"))
                {
                    Debug.Log("Cursor modified!"); 
                    // The images in the cursors folder are cursors, hence we have to change their types
                    var cursorImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    cursorImporter.textureType = TextureImporterType.Cursor;
                    pathsToReimport.Add(assetPath);
                }

                // In case of animation
                if (assetPath.EndsWith(".eaa", StringComparison.InvariantCultureIgnoreCase))
                {
                    var newName = assetPath + ".xml";
                    File.Copy(assetPath, newName);
                    AssetDatabase.DeleteAsset(assetPath);
                    pathsToReimport.Add(newName);
                }

                // In case of animation
                if (assetPath.EndsWith(".xml"))
                {
                    string text = File.ReadAllText(assetPath);
                    text = text.Replace(".eaa", ".eaa.xml");
                    File.WriteAllText(assetPath, text);
                }
            }

            ImportAssets(pathsToReimport.ToArray());
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
            //Debug.Log(sourceDirName + " ||| " + destDirName);
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
                DirectoryInfo i = Directory.CreateDirectory(destDirName);
                // Debug.Log("Create: " + destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.IsuAdventureRelevant())
                    continue;

                string temppath = Path.Combine(destDirName, file.Name);
                //Debug.Log("CopyTo: " + temppath);
                file.CopyTo(temppath, false);
                
                if (!InitImporterConfig(temppath))
                {
                    Debug.LogWarning("Cannot init importer for: " + temppath);
                }

                // In case of animation
                if (file.Extension.ToLowerInvariant() == ".eaa")
                {
                    File.Move(temppath, temppath + ".xml");
                }

                // In case of animation
                if (file.Extension.ToLowerInvariant() == ".xml")
                {
                    string text = File.ReadAllText(temppath);
                    text = text.Replace(".eaa", ".eaa.xml");
                    File.WriteAllText(temppath, text);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    //Debug.Log("temppath: " + temppath);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uAdventure.Core;

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
                Controller.Instance.showErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " descriptor.dtd");
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
                Controller.Instance.showErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " eadventure.dtd");
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
                Controller.Instance.showErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " assessment.dtd");
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
                Controller.Instance.showErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " adaptation.dtd");
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
                Controller.Instance.showErrorDialog("Error.DTD.NotFound.Title", "Error.DTD.NotFound.Message" + " animation.dtd");
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
         * Returns an input stream corresponding to the given file path (relative to
         * the ZIP).
         * 
         * @param assetPath
         *            Path of the file, relative to the ZIP file
         * @return Input stream for the given file
         */
        ////    public static InputStream getInputStream(string assetPath)
        ////    {

        ////        InputStream inputStream = null;

        ////        if (assetPath == null)
        ////            return inputStream;

        ////        try
        ////        {
        ////            // Load the input stream from the file (if it exists)
        ////            File temp = new File(assetPath);
        ////            if (temp.Exists && !temp.isDirectory())
        ////                inputStream = new FileInputStream(assetPath);
        ////            else if (new File(Controller.getInstance().getProjectFolder(), assetPath).Exists)
        ////                inputStream = new FileInputStream(Controller.getInstance().getProjectFolder() + "/" + assetPath);

        ////        }
        ////        catch (FileNotFoundException e)
        ////        {
        ////            ReportDialog.GenerateErrorReport(e, true, "UNKNOWERROR");
        ////        }

        ////        return inputStream;
        ////    }

        ////    /*public static MediaLocator getVideo( string assetPath ) {
        ////    	return videoCache.fetchVideo( assetPath );
        ////    }*/

        ////    public static MediaLocator getVideo(string assetPath)
        ////    {

        ////        string absolutePath = null;

        ////        string[] assetsList = AssetsController.getAssetsList(CATEGORY_VIDEO);
        ////        int position = -1;
        ////        //If it is in the zip
        ////        for (int i = 0; i < assetsList.length; i++)
        ////        {
        ////            if (assetsList[i].Equals(assetPath))
        ////            {
        ////                position = i;
        ////            }
        ////        }
        ////        if (position != -1)
        ////        {
        ////            absolutePath = new File(Controller.getInstance().getProjectFolder(), assetPath).getAbsolutePath();
        ////        }

        ////        File destinyFile = new File(absolutePath);
        ////        try
        ////        {
        ////            return new MediaLocator(destinyFile.toURI().toURL());
        ////        }
        ////        catch (MalformedURLException e)
        ////        {
        ////            ReportDialog.GenerateErrorReport(e, true, "UNKNOWERROR");
        ////            return null;
        ////        }

        ////    }

        ////    public static URL getResourceAsURLFromZip(string path)
        ////    {

        ////        try
        ////        {
        ////            return es.eucm.eadventure.common.auxiliar.zipurl.ZipURL.createAssetURL(Controller.getInstance().getProjectFolder(), path);
        ////        }
        ////        catch (MalformedURLException e)
        ////        {
        ////            return null;
        ////        }
        ////    }

        /**
         * Returns an image corresponding to the given file path (relative to the
         * ZIP).
         * 
         * @param imagePath
         *            Path to the image, relative to the ZIP file
         * @return Image for the given file
         */

        public static Sprite getImage(string imagePath)
        {
            Sprite image = null;

            if (!string.IsNullOrEmpty(imagePath))
                image = (Sprite)Resources.Load(BASE_DIR + imagePath.Substring(0, imagePath.LastIndexOf('.')), typeof(Sprite));

            return image;
        }

        //image = (Sprite)Resources.Load(path, typeof(Sprite));

        ////    /**
        ////     * Returns an array of images, corresponding to the given animation path
        ////     * (relative to the ZIP).
        ////     * 
        ////     * @param animationPath
        ////     *            Animation path relative to the ZIP, including suffix
        ////     *            ("_01.png" or "_01.jpg")
        ////     * @return Array of images, each one containing a frame of the animation
        ////     */
        ////    public static Image[] getAnimation(string animationPath)
        ////    {

        ////        // Create a list of images
        ////        List<Image> framesList = new List<Image>();

        ////        // Prepare the root of the animation path and the extension
        ////        string extension = getExtension(animationPath);
        ////        animationPath = removeSuffix(animationPath);

        ////        // While the last image has not been read
        ////        bool end = false;
        ////        for (int i = 1; i < 100 && !end; i++)
        ////        {

        ////            // Load the current image file
        ////            Image frame = AssetsController.getImage(animationPath + string.format("_%02d.", i) + extension);

        ////            // If it exists, add it to the list
        ////            if (frame != null)
        ////                framesList.add(frame);

        ////            // If it doesn't exist, exit the bucle
        ////            else
        ////                end = true;
        ////        }

        ////        return framesList.toArray(new Image[] { });
        ////    }

        ////    /**
        ////     * Shows a dialog to the user for selecting files which will be added into
        ////     * the given category.
        ////     * 
        ////     * @param assetsCategory
        ////     *            Destiny asset category
        ////     * @return True if some asset was added, false otherwise
        ////     */
        ////    public static bool addAssets(int assetsCategory)
        ////    {

        ////        bool assetsAdded = false;

        ////        // Ask the user for the files to include
        ////        string[] selectedFiles = Controller.getInstance().showMultipleSelectionLoadDialog(getAssetsFileFilter(assetsCategory, FILTER_NONE));

        ////        // If the set is not empty
        ////        if (selectedFiles != null)
        ////        {
        ////            //try {

        ////            assetsAdded = true;

        ////            // For each asset
        ////            for (string assetPath : selectedFiles)
        ////            {
        ////                assetsAdded &= addSingleAsset(assetsCategory, assetPath);
        ////            }

        ////            // Umount the ZIP files
        ////            //File.umount( );
        ////            //} catch( ArchiveException e ) {
        ////            //	e.printStackTrace( );
        ////            //}
        ////        }

        ////        return assetsAdded;
        ////    }

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
            Debug.Log("KOPIUJĘ: " + sourceFile + " | " + destinyFile);
            DirectoryInfo assets = new DirectoryInfo(sourceFile + "\\assets");
            if (assets.Exists)
            {
                DirectoryInfo destinyAssets = new DirectoryInfo(destinyFile + "\\assets");
                if (destinyAssets.Exists)
                {
                    Directory.Delete(destinyFile + "\\assets", true);
                }
                DirectoryCopy(assets.FullName, destinyAssets.FullName, true);
            }
            DirectoryInfo gui = new DirectoryInfo(sourceFile + "\\gui");
            if (gui.Exists)
            {
                DirectoryInfo destinyGui = new DirectoryInfo(destinyFile + "\\gui");
                if (destinyGui.Exists)
                {
                    Directory.Delete(destinyFile + "\\gui", true);
                }
                DirectoryCopy(gui.FullName, destinyGui.FullName, true);
            }
        }

        public static void copyAllFiles(string sourceFile, string destinyFile)
        {
            DirectoryInfo dest = new DirectoryInfo(destinyFile);
            if (dest.Exists)
            {
                Directory.Delete(destinyFile, true);
                // dest.Delete(true);
            }
            DirectoryCopy(sourceFile, destinyFile, true);
        }

        public static void copyAssetsWithoutDeleteCurrentFiles(string sourceFile, string destinyFile)
        {

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
        }
        /**
         * Adds the special assets to the current adventure.
         */

        public static void addSpecialAssets()
        {

            // Get the zip file
            string zipFile = Controller.Instance.ProjectFolder;

            // Add the defaultBook
            FileInfo sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_DEFAULT_BOOK_IMAGE);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_DEFAULT_BOOK_IMAGE), true);
            // If the source file doesn't exist, show an error message
            else
                Controller.Instance                    .showErrorDialog(TC.get("Error.Title"),
                        TC.get("Error.SpecialAssetNotFound", "img/assets/DefaultBook.jpg"));

            sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_DEFAULT_ARROW_NORMAL);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL), true);
            // If the source file doesn't exist, show an error message
            else
                Controller.Instance                    .showErrorDialog(TC.get("Error.Title"),
                        TC.get("Error.SpecialAssetNotFound", "img/assets/DefaultLeftNormalArrow.png"));

            sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_DEFAULT_ARROW_OVER);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER), true);
            // If the source file doesn't exist, show an error message
            else
                Controller.Instance                    .showErrorDialog(TC.get("Error.Title"),
                        TC.get("Error.SpecialAssetNotFound", "img/assets/DefaultLeftOverArrow.png"));

            // Add the empty image
            sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_EMPTY_IMAGE);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_EMPTY_IMAGE), true);

            // Add the empty backlground image
            sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_EMPTY_BACKGROUND);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_EMPTY_BACKGROUND), true);

            // If the source file doesn't exist, show an error message
            else
                Controller.Instance                    .showErrorDialog(TC.get("Error.Title"),
                        TC.get("Error.SpecialAssetNotFound", "img/assets/EmptyImage.png"));

            // Add the empty icon
            sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_EMPTY_ICON);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_EMPTY_ICON), true);

            // If the source file doesn't exist, show an error message
            else
                Controller.Instance                    .showErrorDialog(TC.get("Error.Title"), TC.get("Error.SpecialAssetNotFound", "img/assets/EmptyIcon.png"));

            // Add the empty animation
            sourceFile = new FileInfo("Assets/Resources/" + SpecialAssetPaths.FILE_EMPTY_ANIMATION);
            if (sourceFile.Exists)
                sourceFile.CopyTo(Path.Combine(zipFile, SpecialAssetPaths.ASSET_EMPTY_ANIMATION + "_01.png"), true);

            // If the source file doesn't exist, show an error message
            else
                Controller.Instance                    .showErrorDialog(TC.get("Error.Title"),
                        TC.get("Error.SpecialAssetNotFound", "img/assets/EmptyAnimation_01.png"));
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
        ////     * Returns the filename of the given asset.
        ////     * 
        ////     * @param assetPath
        ////     *            Path to the asset
        ////     * @return Name of the file representing the asset
        ////     */
        ////    public static string getFilename(string assetPath)
        ////    {

        ////        int lastSlashIndex = Math.Max(assetPath.LastIndexOf('/') + 1, assetPath.LastIndexOf('\\') + 1);
        ////        return assetPath.Substring(lastSlashIndex, assetPath.Length);
        ////    }

        ////    /**
        ////     * Removes the suffix "_01.png" or "_01.jpg" from the given asset path.
        ////     * 
        ////     * @param assetPath
        ////     *            Source asset path
        ////     * @return Asset path without the suffix
        ////     */
        ////    public static string removeSuffix(string assetPath)
        ////    {

        ////        // Remove the suffix in the PNG animations
        ////        if (assetPath.ToLower().EndsWith("_01.png"))
        ////            assetPath = assetPath.Substring(0, assetPath.Length - 7);
        ////        // Remove the suffix in the JPG slides
        ////        else if (assetPath.ToLower().EndsWith("_01.jpg"))
        ////        {
        ////            assetPath = assetPath.Substring(0, assetPath.Length - 7);
        ////        }

        ////        return assetPath;
        ////    }

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

        ////    private static bool isImageWithRestrictedDimension(int assetCategory)
        ////    {

        ////        return (assetCategory == CATEGORY_BACKGROUND || assetCategory == CATEGORY_ICON || assetCategory == CATEGORY_ARROW_BOOK);
        ////    }

        ////    /**
        ////     * 
        ////     * @param assetCategory
        ////     *            The asset category
        ////     * @return Return the maximum dimensions for an asset category
        ////     */
        ////    private static Dimension getRestrictedDimension(int assetCategory)
        ////    {

        ////        int rest_width = 0;
        ////        int rest_height = 0;

        ////        switch (assetCategory)
        ////        {
        ////            case AssetsConstants.CATEGORY_BACKGROUND:
        ////                rest_width = AssetsImageDimensions.BACKGROUND_MAX_WIDTH;
        ////                rest_height = AssetsImageDimensions.BACKGROUND_MAX_HEIGHT;
        ////                break;
        ////            case AssetsConstants.CATEGORY_ICON:
        ////                rest_width = AssetsImageDimensions.ICON_MAX_WIDTH;
        ////                rest_height = AssetsImageDimensions.ICON_MAX_HEIGHT;
        ////                break;
        ////            case AssetsConstants.CATEGORY_ARROW_BOOK:
        ////                rest_width = AssetsImageDimensions.ARROW_BOOK_MAX_WIDTH;
        ////                rest_height = AssetsImageDimensions.ARROW_BOOK_MAX_HEIGHT;
        ////                break;
        ////        }

        ////        return new Dimension(rest_width, rest_height);
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
        ////     * Returns the extension of the given asset.
        ////     * 
        ////     * @param assetPath
        ////     *            Path to the asset
        ////     * @return Extension of the file
        ////     */
        ////    private static string getExtension(string assetPath)
        ////    {

        ////        return assetPath.Substring(assetPath.LastIndexOf('.') + 1, assetPath.Length);
        ////    }

        ////    /**
        ////     * Returns the folder associated with the given category.
        ////     * 
        ////     * @param assetsCategory
        ////     *            Category for the folder
        ////     * @return Folder for the given category, null if the category was not
        ////     *         recognized
        ////     */
        ////    public static string getCategoryAbsoluteFolder(int assetsCategory)
        ////    {

        ////        return new File(Controller.getInstance().getProjectFolder(), getCategoryFolder(assetsCategory)).getAbsolutePath();
        ////    }

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
                /*case CATEGORY_ASSESSMENT:
                    folder = CATEGORY_ASSESSMENT_FOLDER;
                    break;
                case CATEGORY_ADAPTATION:
                    folder = CATEGORY_ADAPTATION_FOLDER;
                    break;*/
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

        ////    /*public static class TempFileGenerator {

        ////        private static Random random = new Random( );

        ////        private static int MAX_RANDOM = 100000;

        ////        public static readonly  string TEMP_FILE_NAME = "eadventure_";

        ////        public TempFileGenerator( ) {

        ////            random = new Random( );
        ////        }

        ////        public static string generateTempFileAbsolutePath( string extension ) {

        ////            return generateTempFileAbsolutePath( TEMP_FILE_NAME, extension );
        ////        }

        ////        public static string generateTempFileAbsolutePath( string name, string extension ) {

        ////            string tempDirectory = null;
        ////            if( System.getenv( "TEMP" ) != null && !System.getenv( "TEMP" ).Equals( "" ) ) {
        ////                tempDirectory = System.getenv( "TEMP" );
        ////            }
        ////            else if( System.getenv( "HOME" ) != null && !System.getenv( "HOME" ).Equals( "" ) ) {
        ////                tempDirectory = System.getenv( "HOME" );
        ////            }
        ////            else if( System.getenv( "ROOT" ) != null && !System.getenv( "ROOT" ).Equals( "" ) ) {
        ////                tempDirectory = System.getenv( "ROOT" );
        ////            }
        ////            else {
        ////                tempDirectory = "";
        ////            }

        ////            string fileName = name + "." + extension;
        ////            File file = new File( tempDirectory + "/" + fileName );
        ////            while( file.exists( ) ) {
        ////                fileName = name + random.nextInt( MAX_RANDOM ) + "." + extension;
        ////                file = new File( tempDirectory + "/" + fileName );
        ////            }
        ////            return tempDirectory + "/" + fileName;
        ////        }

        ////        public static string generateTempFileOverwriteExisting( string name, string extension ) {

        ////            string tempDirectory = null;
        ////            if( System.getenv( "TEMP" ) != null && !System.getenv( "TEMP" ).Equals( "" ) ) {
        ////                tempDirectory = System.getenv( "TEMP" );
        ////            }
        ////            else if( System.getenv( "HOME" ) != null && !System.getenv( "HOME" ).Equals( "" ) ) {
        ////                tempDirectory = System.getenv( "HOME" );
        ////            }
        ////            else if( System.getenv( "ROOT" ) != null && !System.getenv( "ROOT" ).Equals( "" ) ) {
        ////                tempDirectory = System.getenv( "ROOT" );
        ////            }
        ////            else {
        ////                tempDirectory = "";
        ////            }

        ////            string fileName = name + "." + extension;
        ////            File file = new File( tempDirectory + "/" + fileName );
        ////            if( file.exists( ) ) {
        ////                file.delete( );
        ////            }
        ////            return tempDirectory + "/" + fileName;
        ////        }

        ////    }*/

        ////    /**
        ////     * Extracts the resource and get it copied to a file in the local system.
        ////     * Required when an asset cannot be loaded directly from zip
        ////     * 
        ////     * @param assetPath
        ////     * @return The absolute path of the destiny file where the asset was copied
        ////     */
        ////    public static string extractResource(string assetPath)
        ////    {

        ////        string toReturn = null;
        ////        try
        ////        {
        ////            //Check if the file has already been extracted
        ////            if (!tempFiles.ContainsKey(assetPath))
        ////            {

        ////                //string filePath = VideoCache.generateTempFileAbsolutePath (getExtension(assetPath));
        ////                FileInfo destinyFile = File.createTempFile("ead-resource", "." + getExtension(assetPath));
        ////                //string filePath = TempFileGenerator.generateTempFileAbsolutePath( getExtension( assetPath ) );
        ////                string filePath = destinyFile.getAbsolutePath();
        ////                File sourceFile = new File(Controller.getInstance().getProjectFolder(), assetPath);
        ////                //File destinyFile = new File( filePath );
        ////                if (sourceFile.CopyTo(destinyFile))
        ////                {
        ////                    tempFiles.put(assetPath, destinyFile);
        ////                    toReturn = filePath;
        ////                }
        ////                else
        ////                    toReturn = null;
        ////            }
        ////            else
        ////                toReturn = tempFiles.get(assetPath).getAbsolutePath();
        ////        }
        ////        catch (Exception e)
        ////        {
        ////            toReturn = null;
        ////        }

        ////        return toReturn;
        ////    }

        public class InputStreamCreatorEditor : InputStreamCreator
        {

            private string absolutePath;

            public InputStreamCreatorEditor()
            {

                absolutePath = "";
            }

            public InputStreamCreatorEditor(string absolutePath)
            {

                this.absolutePath = absolutePath;
            }

            public string buildInputStream(string filePath)
            {
                //TODO: implementation
                //string iss = "";
                //if (absolutePath == null)
                //{
                //     if( filePath.StartsWith( "/" ) || filePath.StartsWith( "\\" ) ) {
                //    FIXME: Somehow, these is not needed anymore (check more, might still be needed outside eclipse)
                //    string os = System.getProperty( "os.name" ).toLowerCase( );
                //     if ( !filePath.StartsWith( "/User" ) )
                //       filePath = filePath.Substring( 1, filePath.length( ) );
                //    }

                //    iss = getInputStream(filePath);
                //    if (iss == null && filePath.Length > 1 && !filePath.StartsWith("/User"))
                //    {
                //        iss = getInputStream(filePath.Substring(1, filePath.Length));
                //    }

                //}
                //else {
                //    try
                //    {
                //        iss = new FileInputStream(new File(absolutePath, filePath));
                //    }
                //    catch (FileNotFoundException e)
                //    {
                //        return null;
                //    }
                //}

                //return iss;
                Debug.Log(absolutePath + "\n" + filePath);
                return Path.Combine(absolutePath, filePath);
            }

            public string[] listNames(string filePath)
            {
                string[] fileEntries = null;
                string targetPath = Path.Combine(absolutePath, filePath);
                if (Directory.Exists(targetPath))
                    fileEntries = Directory.GetFiles(targetPath);
                else
                    fileEntries = new string[0];
                return fileEntries;
            }

            ////    public MediaLocator buildMediaLocator(string file)
            ////    {

            ////        return getVideo(file);
            ////    }

            ////    public URL buildURL(string path)
            ////    {

            ////        try
            ////        {
            ////            return new File(Controller.getInstance().getProjectFolder(), path).toURI().toURL();
            ////        }
            ////        catch (MalformedURLException e)
            ////        {
            ////            return null;
            ////        }
            ////    }

            ////}

            public static InputStreamCreator getInputStreamCreator()
            {

                return new InputStreamCreatorEditor();
            }

            public static InputStreamCreator getInputStreamCreator(string absolutePath)
            {

                return new InputStreamCreatorEditor(absolutePath);
            }
        }

        // TODO: TMP - delte iT
        public class FileFilter
        {
            public string getAssetsFileFilter(int assetsCategory, int filter)
            {
                return string.Empty;
            }
        }

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
                string temppath = Path.Combine(destDirName, file.Name);
                //Debug.Log("CopyTo: " + temppath);
                file.CopyTo(temppath, false);
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
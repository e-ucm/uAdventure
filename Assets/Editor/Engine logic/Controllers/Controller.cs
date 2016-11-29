using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using uAdventure.Core;

using Animation = uAdventure.Core.Animation;

namespace uAdventure.Editor
{
    /**
     * This class is the main controller of the application. It holds the main
     * operations and data to control the editor.
     */

    public class Controller
    {
        [DllImport("user32.dll")]
        private static extern void SaveFileDialog();

        private System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();

        /**
         * Id for the complete chapter data element.
         */
        public const int CHAPTER = 0;

        /**
         * Id for the scenes list element.
         */
        public const int SCENES_LIST = 1;

        /**
         * Id for the scene element.
         */
        public const int SCENE = 2;

        /**
         * Id for the exits list element.
         */
        public const int EXITS_LIST = 3;

        /**
         * Id for the exit element.
         */
        public const int EXIT = 4;

        /**
         * Id for the item references list element.
         */
        public const int ITEM_REFERENCES_LIST = 5;

        /**
         * Id for the item reference element.
         */
        public const int ITEM_REFERENCE = 6;

        /**
         * Id for the NPC references list element.
         */
        public const int NPC_REFERENCES_LIST = 7;

        /**
         * Id for the NPC reference element.
         */
        public const int NPC_REFERENCE = 8;

        /**
         * Id for the cutscenes list element.
         */
        public const int CUTSCENES_LIST = 9;

        /**
         * Id for the slidescene element.
         */
        public const int CUTSCENE_SLIDES = 10;

        public const int CUTSCENE = 910;

        public const int CUTSCENE_VIDEO = 37;

        /**
         * Id for the books list element.
         */
        public const int BOOKS_LIST = 11;

        /**
         * Id for the book element.
         */
        public const int BOOK = 12;

        /**
         * Id for the book paragraphs list element.
         */
        public const int BOOK_PARAGRAPHS_LIST = 13;

        /**
         * Id for the title paragraph element.
         */
        public const int BOOK_TITLE_PARAGRAPH = 14;

        /**
         * Id for the text paragraph element.
         */
        public const int BOOK_TEXT_PARAGRAPH = 15;

        /**
         * Id for the bullet paragraph element.
         */
        public const int BOOK_BULLET_PARAGRAPH = 16;

        /**
         * Id for the image paragraph element.
         */
        public const int BOOK_IMAGE_PARAGRAPH = 17;

        /**
         * Id for the items list element.
         */
        public const int ITEMS_LIST = 18;

        /**
         * Id for the item element.
         */
        public const int ITEM = 19;

        /**
         * Id for the actions list element.
         */
        public const int ACTIONS_LIST = 20;

        /**
         * Id for the "Examine" action element.
         */
        public const int ACTION_EXAMINE = 21;

        /**
         * Id for the "Grab" action element.
         */
        public const int ACTION_GRAB = 22;

        /**
         * Id for the "Use" action element.
         */
        public const int ACTION_USE = 23;

        /**
         * Id for the "Custom" action element.
         */
        public const int ACTION_CUSTOM = 230;

        /**
         * Id for the "Talk to" action element.
         */
        public const int ACTION_TALK_TO = 231;

        /**
         * Id for the "Use with" action element.
         */
        public const int ACTION_USE_WITH = 24;

        /**
         * Id for the "Give to" action element.
         */
        public const int ACTION_GIVE_TO = 25;

        /**
         * Id for the "Drag-to" action element.
         */
        public const int ACTION_DRAG_TO = 251;

        /**
         * Id for the "Custom interact" action element.
         */
        public const int ACTION_CUSTOM_INTERACT = 250;

        /**
         * Id for the player element.
         */
        public const int PLAYER = 26;

        /**
         * Id for the NPCs list element.
         */
        public const int NPCS_LIST = 27;

        /**
         * Id for the NPC element.
         */
        public const int NPC = 28;

        /**
         * Id for the conversation references list element.
         */
        public const int CONVERSATION_REFERENCES_LIST = 29;

        /**
         * Id for the conversation reference element.
         */
        public const int CONVERSATION_REFERENCE = 30;

        /**
         * Id for the conversations list element.
         */
        public const int CONVERSATIONS_LIST = 31;

        /**
         * Id for the tree conversation element.
         */
        public const int CONVERSATION_TREE = 32;

        /**
         * Id for the graph conversation element.
         */
        public const int CONVERSATION_GRAPH = 33;

        /**
         * Id for the graph conversation element.
         */
        public const int CONVERSATION_DIALOGUE_LINE = 330;

        /**
         * Id for the graph conversation element.
         */
        public const int CONVERSATION_OPTION_LINE = 331;

        /**
         * Id for the resources element.
         */
        public const int RESOURCES = 34;

        /**
         * Id for the next scene element.
         */
        public const int NEXT_SCENE = 35;

        /**
         * If for the end scene element.
         */
        public const int END_SCENE = 36;

        /**
         * Id for Assessment Rule
         */
        public const int ASSESSMENT_RULE = 38;

        /**
         * Id for Adaptation Rule
         */
        public const int ADAPTATION_RULE = 39;

        /**
         * Id for Assessment Rules
         */
        public const int ASSESSMENT_PROFILE = 40;

        /**
         * Id for Adaptation Rules
         */
        public const int ADAPTATION_PROFILE = 41;

        /**
         * Id for the styled book element.
         */
        public const int STYLED_BOOK = 42;

        /**
         * Id for the page of a STYLED_BOK.
         */
        public const int BOOK_PAGE = 43;

        /**
         * Id for timers.
         */
        public const int TIMER = 44;

        /**
         * Id for the list of timers.
         */
        public const int TIMERS_LIST = 45;

        /**
         * Id for the advanced features node.
         */
        public const int ADVANCED_FEATURES = 46;

        /**
         * Id for the assessment profiles node.
         */
        public const int ASSESSSMENT_PROFILES = 47;

        /**
         * Id for the adaptation profiles node.
         */
        public const int ADAPTATION_PROFILES = 48;

        /**
         * Id for timed assessment rules
         */
        public const int TIMED_ASSESSMENT_RULE = 49;

        /**
         * Id for active areas list.
         */
        public const int ACTIVE_AREAS_LIST = 50;

        /**
         * Id for active area
         */
        public const int ACTIVE_AREA = 51;

        /**
         * Id for barriers list.
         */
        public const int BARRIERS_LIST = 52;

        /**
         * Id for barrier
         */
        public const int BARRIER = 53;

        /**
         * Id for global state
         */
        public const int GLOBAL_STATE = 54;

        /**
         * Id for global state list
         */
        public const int GLOBAL_STATE_LIST = 55;

        /**
         * Id for macro
         */
        public const int MACRO = 56;

        /**
         * Id for macro list
         */
        public const int MACRO_LIST = 57;

        /**
         * Id for atrezzo item element
         */
        public const int ATREZZO = 58;

        /**
         * Id for atrezzo list element
         */
        public const int ATREZZO_LIST = 59;

        /**
         * Id for atrezzo reference
         */
        public const int ATREZZO_REFERENCE = 60;

        /**
         * Id for atrezzo references list
         */
        public const int ATREZZO_REFERENCES_LIST = 61;

        public const int NODE = 62;

        public const int SIDE = 63;

        public const int TRAJECTORY = 64;

        public const int ANIMATION = 65;

        public const int EFFECT = 66;

        //TYPES OF EAD FILES
        public const int FILE_ADVENTURE_1STPERSON_PLAYER = 0;

        public const int FILE_ADVENTURE_3RDPERSON_PLAYER = 1;

        public const int FILE_ASSESSMENT = 2;

        public const int FILE_ADAPTATION = 3;

        /**
         * Identifiers for differents scorm profiles
         */
        public const int SCORM12 = 0;

        public const int SCORM2004 = 1;

        public const int AGREGA = 2;

        /**
         * Singleton instance.
         */
        private static Controller controllerInstance = null;

        /**
         * The complete path to the current open ZIP file.
         */
        //TODO: implement init method
        private string currentZipFile = "";

        /**
         * The path to the folder that holds the open file.
         */
        private string currentZipPath;

        /**
         * The name of the file that is being currently edited. Used only to display
         * info.
         */
        private string currentZipName;

        /**
         * The name of the current project
         * info.
         */
        private string currentProjectName;

        /**
         * The data of the adventure being edited.
         */
        private AdventureDataControl adventureDataControl;

        /**
         * Stores if the data has been modified since the last save.
         */
        private bool dataModified_F;

        /**
         * Stores the file that contains the GUI strings.
         */
        private string languageFile;

        // private LoadingScreen loadingScreen;

        private string lastDialogDirectory;

        /*private bool isTempFile = false;

        public bool isTempFile( ) {
            return isTempFile;
        }*/

        private ChapterListDataControl chaptersController = new ChapterListDataControl();

        private MainWindowWrapper mainWindow;
        //   // private AutoSave autoSave;

        //   // private Timer autoSaveTimer;

        //    //private bool isLomEs = false;

        //    /**
        //     * Store all effects selection. Connects the type of effect with the number
        //     * of times that has been used
        //     */
        //    // private SelectedEffectsController selectedEffects;
        //    /**
        //     * Void and private constructor.
        //     */
        private Controller()
        {

            chaptersController = new ChapterListDataControl();
        }

        // ABSOLUTE?
        private string getCurrentExportSaveFolder()
        {

            return ReleaseFolders.EXPORTS_FOLDER;
        }

        // ABSOLUTE?
        public string getCurrentLoadFolder()
        {

            return ReleaseFolders.PROJECTS_FOLDER;
        }

        //    public void setLastDirectory(string directory)
        //    {

        //        this.lastDialogDirectory = directory;
        //    }

        //    public string getLastDirectory()
        //    {

        //        if (lastDialogDirectory != null)
        //        {
        //            return lastDialogDirectory;
        //        }
        //        else
        //            return ReleaseFolders.PROJECTS_FOLDER.getAbsolutePath();
        //    }

        /**
         * Returns the instance of the controller.
         * 
         * @return The instance of the controller
         */

        public static Controller getInstance()
        {

            if (controllerInstance == null)
                controllerInstance = new Controller();

            return controllerInstance;
        }

        public static void resetInstance()
        {
            controllerInstance = null;
            GameRources.GetInstance().Reset();
        }

        public int playerMode()
        {

            return adventureDataControl.getPlayerMode();
        }

        //    /**
        //     * Initializing function.
        //     */
        public void init(string loadProjectPath = null)
        {
            ConfigData.loadFromXML(ReleaseFolders.configFileEditorRelativePath());
            // Load the configuration
            //ProjectConfigData.init();
            //SCORMConfigData.init();

            // Create necessary folders if no created befor
            DirectoryInfo projectsFolder = new DirectoryInfo(ReleaseFolders.PROJECTS_FOLDER);
            if (!projectsFolder.Exists)
            {
                projectsFolder.Create();
            }
            DirectoryInfo tempFolder = new DirectoryInfo(ReleaseFolders.WEB_TEMP_FOLDER);
            if (!tempFolder.Exists)
            {
                projectsFolder.Create();
            }
            DirectoryInfo exportsFolder = new DirectoryInfo(ReleaseFolders.EXPORTS_FOLDER);
            if (!exportsFolder.Exists)
            {
                exportsFolder.Create();
            }

            languageFile = ReleaseFolders.LANGUAGE_ENGLISH;
            setLanguage(ReleaseFolders.getLanguageFromPath(ConfigData.getLanguangeFile()), false);

            // Create a list for the chapters
            chaptersController = new ChapterListDataControl();

            // Inits the controller with empty data
            currentZipFile = null;
            currentZipPath = null;
            currentZipName = null;
            currentProjectName = null;

            dataModified_F = false;

            ////Create main window and hide it
            mainWindow = new MainWindowWrapper();

            if (loadProjectPath != null)
            {
                FileInfo projectFile = new FileInfo(loadProjectPath);
                if (projectFile.Exists)
                {
                    if (projectFile.FullName.ToLower().EndsWith(".eap"))
                    {
                        string absolutePath = projectFile.FullName;
                        loadFile(absolutePath.Substring(0, absolutePath.Length - 4), true);
                    }
                    else if (projectFile.Exists)
                        loadFile(projectFile.FullName, true);
                }
            }
            else
            {
                loadFile();
            }
        }

        public bool Initialized()
        {
            return adventureDataControl != null;
        }

        //    /*public void addSelectedEffect(string name){
        //        selectedEffects.addSelectedEffect(name);
        //    }

        //    public SelectedEffectsController getSelectedEffectsController(){
        //        return selectedEffects;
        //    }*/

        //    //public void startAutoSave(int minutes)
        //    //{

        //    //    stopAutoSave();

        //    //    if ((ProjectConfigData.existsKey("autosave") && ProjectConfigData.getProperty("autosave").Equals("yes")) || !ProjectConfigData.existsKey("autosave"))
        //    //    {
        //    //        /*			autoSaveTimer = new Timer();
        //    //        			autoSave = new AutoSave();
        //    //        			autoSaveTimer.schedule(autoSave, 10000, minutes * 60 * 1000);
        //    //        */
        //    //    }
        //    //    if (!ProjectConfigData.existsKey("autosave"))
        //    //        ProjectConfigData.setProperty("autosave", "yes");
        //    //}

        //    //public void stopAutoSave()
        //    //{

        //    //    if (autoSaveTimer != null)
        //    //    {
        //    //        autoSaveTimer.cancel();
        //    //        autoSave.stop();
        //    //        autoSaveTimer = null;
        //    //    }
        //    //    autoSave = null;
        //    //}

        //    //private ToolSystemDebugger tsd;

        //    // ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //    // General data functions of the aplication

        /**
         * Returns the complete path to the currently open Project.
         * 
         * @return The complete path to the ZIP file, null if none is open
         */

        public string getProjectFolder()
        {
            return currentZipFile;
        }

        /**
         * Returns the File object representing the currently open Project.
         * 
         * @return The complete path to the ZIP file, null if none is open
         */

        public FileInfo getProjectFolderFile()
        {

            return new FileInfo(currentZipFile);
        }

        //    /**
        //     * Returns the name of the file currently open.
        //     * 
        //     * @return The name of the current file
        //     */
        //    public string getFileName()
        //    {

        //        string filename;

        //        // Show "New" if no current file is currently open
        //        if (currentZipName != null)
        //            filename = currentZipName;
        //        else
        //            filename = "http://e-adventure.e-ucm.es";

        //        return filename;
        //    }

        //    /**
        //     * Returns the parent path of the file being currently edited.
        //     * 
        //     * @return Parent path of the current file
        //     */
        //    public string getFilePath()
        //    {

        //        return currentZipPath;
        //    }

        //    /**
        //     * Returns an array with the chapter titles.
        //     * 
        //     * @return Array with the chapter titles
        //     */
        //    public string[] getChapterTitles()
        //    {

        //        return chaptersController.getChapterTitles();
        //    }

        //    /**
        //     * Returns the index of the chapter currently selected.
        //     * 
        //     * @return Index of the selected chapter
        //     */
        //    public int getSelectedChapter()
        //    {

        //        return chaptersController.getSelectedChapter();
        //    }

        /**
         * Returns the selected chapter data controller.
         * 
         * @return The selected chapter data controller
         */

        public ChapterDataControl getSelectedChapterDataControl()
        {

            return chaptersController.getSelectedChapterDataControl();
        }

        /**
         * Returns the identifier summary.
         * 
         * @return The identifier summary
         */
        public IdentifierSummary getIdentifierSummary()
        {

            return chaptersController.getIdentifierSummary();
        }

        //    /**
        //     * Returns the varFlag summary.
        //     * 
        //     * @return The varFlag summary
        //     */
        public VarFlagSummary getVarFlagSummary()
        {

            return chaptersController.getVarFlagSummary();
        }

        public ChapterListDataControl getCharapterList()
        {

            return chaptersController;
        }

        /**
         * Returns whether the data has been modified since the last save.
         * 
         * @return True if the data has been modified, false otherwise
         */

        public bool isDataModified()
        {

            return dataModified_F;
        }

        //    ///**
        //    // * Called when the data has been modified, it sets the value to true.
        //    // */
        public void dataModified()
        {

            // If the data were not modified, change the value and set the new title of the window
            if (!dataModified_F)
            {
                dataModified_F = true;
                // mainWindow.updateTitle();
            }
        }

        public bool isPlayTransparent()
        {

            if (adventureDataControl == null)
            {
                return false;
            }
            return adventureDataControl.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON;

        }

        //    public void swapPlayerMode(bool showConfirmation)
        //    {

        //        addTool(new SwapPlayerModeTool(showConfirmation, adventureDataControl, chaptersController));
        //    }

        //    // ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Functions that perform usual application actions

        /**
         * This method creates a new file with it's respective data.
         * 
         * @return True if the new data was created successfully, false otherwise
         */

        public bool newFile(int fileType)
        {

            bool fileCreated = false;

            if (fileType == Controller.FILE_ADVENTURE_1STPERSON_PLAYER ||
                fileType == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER)
                fileCreated = newAdventureFile(fileType);
            else if (fileType == Controller.FILE_ASSESSMENT)
            {
                //fileCreated = newAssessmentFile();
            }
            else if (fileType == Controller.FILE_ADAPTATION)
            {
                //fileCreated = newAdaptationFile();
            }

            if (fileCreated)
                AssetsController.resetCache();

            return fileCreated;
        }

        public void save()
        {
            Writer.writeData("Assets\\Resources\\CurrentGame", adventureDataControl, true);
            UnityEditor.AssetDatabase.Refresh();
        }

        //    public bool newFile()
        //    {

        //        bool createNewFile = true;

        //        if (dataModified)
        //        {
        //            int option = mainWindow.showConfirmDialog(TC.get("Operation.NewFileTitle"), TC.get("Operation.NewFileMessage"));

        //            // If the data must be saved, create the new file only if the save was successful
        //            if (option == JOptionPane.YES_OPTION)
        //                createNewFile = saveFile(false);

        //            // If the data must not be saved, create the new data directly
        //            else if (option == JOptionPane.NO_OPTION)
        //            {
        //                createNewFile = true;
        //                dataModified = false;
        //                mainWindow.updateTitle();
        //            }

        //            // Cancel the action if selected
        //            else if (option == JOptionPane.CANCEL_OPTION)
        //            {
        //                createNewFile = false;
        //            }

        //        }

        //        if (createNewFile)
        //        {
        //            stopAutoSave();
        //            ConfigData.storeToXML();
        //            ProjectConfigData.storeToXML();
        //            ConfigData.loadFromXML(ReleaseFolders.configFileEditorRelativePath());
        //            ProjectConfigData.init();

        //            // Show dialog
        //            //StartDialog start = new StartDialog( StartDialog.NEW_TAB );
        //            FrameForInitialDialogs start = new FrameForInitialDialogs(StartDialog.NEW_TAB);

        //            //mainWindow.setEnabled( false );
        //            //mainWindow.setExtendedState(JFrame.ICONIFIED | mainWindow.getExtendedState());
        //            mainWindow.setVisible(false);

        //            //int op = start.showOpenDialog( mainWindow );
        //            int op = start.showStartDialog();
        //            //start.end();
        //            if (op == StartDialog.NEW_FILE_OPTION)
        //            {
        //                newFile(start.getFileType());
        //            }
        //            else if (op == StartDialog.OPEN_FILE_OPTION)
        //            {
        //                java.io.File selectedFile = start.getSelectedFile();
        //                if (selectedFile.getAbsolutePath().toLowerCase().endsWith(".eap"))
        //                {
        //                    string absolutePath = selectedFile.getPath();
        //                    loadFile(absolutePath.substring(0, absolutePath.length() - 4), true);
        //                }
        //                else if (selectedFile.isDirectory() && selectedFile.exists())
        //                    loadFile(start.getSelectedFile().getAbsolutePath(), true);
        //                else {
        //                    this.importGame(selectedFile.getAbsolutePath());
        //                }
        //            }
        //            else if (op == StartDialog.RECENT_FILE_OPTION)
        //            {
        //                loadFile(start.getRecentFile().getAbsolutePath(), true);
        //            }
        //            else if (op == StartDialog.CANCEL_OPTION)
        //            {
        //                //exit( );
        //            }

        //            start.remove();

        //            if (currentZipFile == null)
        //            {
        //                mainWindow.reloadData();
        //            }

        //            mainWindow.setResizable(true);
        //            mainWindow.setEnabled(true);
        //            mainWindow.setVisible(true);
        //            //DEBUGGING
        //            //tsd = new ToolSystemDebugger( chaptersController );
        //        }

        //        Controller.gc();

        //        return createNewFile;
        //    }

        private bool newAdventureFile(int fileType)
        {

            bool fileCreated = false;

            // Decide the directory of the temp file and give a name for it

            // If there is a valid temp directory in the system, use it
            //string tempDir = System.getenv( "TEMP" );
            //string completeFilePath = "";

            //isTempFile = true;
            //if( tempDir != null ) {
            //	completeFilePath = tempDir + "/" + TEMP_NAME;
            //}

            // If the temp directory is not valid, use the home directory
            //else {

            //	completeFilePath = FileSystemView.getFileSystemView( ).getHomeDirectory( ) + "/" + TEMP_NAME;
            //}

            bool create = false;
            DirectoryInfo selectedDir = null;
            FileInfo selectedFile = null;
            // Prompt main folder of the project
            //ProjectFolderChooser folderSelector = new ProjectFolderChooser( false, false );
            //FrameForInitialDialogs start = new FrameForInitialDialogs(false);
            //int op = start.showStartDialog();
            // If some folder is selected, check all characters are correct  
            // if( folderSelector.showOpenDialog( mainWindow ) == JFileChooser.APPROVE_OPTION ) {

            Stream myStream = null;
            sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.InitialDirectory = "c:\\";
            sfd.Filter = "ead files (*.ead) | *.ead |eap files (*.eap) | *.eap";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {

                if ((myStream = sfd.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        DirectoryInfo selectedFolder = new DirectoryInfo(sfd.FileName);
                        selectedFile = new FileInfo(sfd.FileName);
                        if (selectedFile.FullName.EndsWith(".eap"))
                        {
                            string absolutePath = selectedFolder.FullName;
                            selectedFolder = new DirectoryInfo(absolutePath.Substring(0, absolutePath.Length - 4));
                        }
                        else
                        {
                            selectedFile = new FileInfo(selectedFile.FullName + ".eap");
                        }
                        selectedDir = selectedFolder;

                        // Check the parent folder is not forbidden
                        if (isValidTargetProject(selectedFile))
                        {

                            //if (FolderFileFilter.checkCharacters(selectedFolder.getName()))
                            //{
                            // Folder can be created/used
                            // Does the folder exist?
                            if (selectedFolder.Exists)
                            {
                                //Is the folder empty?
                                if (selectedFolder.GetFiles().Length > 0)
                                {
                                    // Delete content?
                                    if (this.showStrictConfirmDialog(TC.get("Operation.NewProject.FolderNotEmptyTitle"),
                                        TC.get("Operation.NewProject.FolderNotEmptyMessage")))
                                    {
                                        DirectoryInfo directory = new DirectoryInfo(selectedFolder.FullName);
                                        directory.Delete(true);
                                    }
                                }
                                create = true;
                            }
                            else
                            {
                                // Create new folder?
                                if (this.showStrictConfirmDialog(TC.get("Operation.NewProject.FolderNotCreatedTitle"),
                                    TC.get("Operation.NewProject.FolderNotCreatedMessage")))
                                {
                                    DirectoryInfo directory = new DirectoryInfo(selectedFolder.FullName);
                                    directory.Create();
                                    if (directory.Exists)
                                    {
                                        create = true;
                                    }
                                    else
                                    {
                                        this.showStrictConfirmDialog(TC.get("Error.Title"), TC.get("Error.CreatingFolder"));
                                    }

                                }
                                else
                                {
                                    create = false;
                                }
                            }
                            //}
                            //else {
                            //    // Display error message
                            //    this.showErrorDialog(TC.get("Error.Title"), TC.get("Error.ProjectFolderName", FolderFileFilter.getAllowedChars()));
                            //}
                        }
                        else
                        {
                            // Show error: The target dir cannot be contained 
                            Debug.LogError(TC.get("Operation.NewProject.ForbiddenParent.Title") + " \n " +
                                           TC.get("Operation.NewProject.ForbiddenParent.Message"));
                            create = false;
                        }
                    }
                    myStream.Dispose();
                }
            }

            // Create the new project?
            //LoadingScreen loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.CreateProject" ), getLoadingImage( ), mainWindow);

            if (create)
            {
                //loadingScreen.setVisible( true );
                //loadingScreen.setMessage(TC.get("Operation.CreateProject"));
                //loadingScreen.setVisible(true);

                // Set the new file, path and create the new adventure
                currentZipFile = selectedDir.FullName;
                currentZipPath = selectedDir.Parent.FullName;
                currentZipName = selectedDir.Name;
                int playerMode = -1;
                if (fileType == FILE_ADVENTURE_3RDPERSON_PLAYER)
                    playerMode = DescriptorData.MODE_PLAYER_3RDPERSON;
                else if (fileType == FILE_ADVENTURE_1STPERSON_PLAYER)
                    playerMode = DescriptorData.MODE_PLAYER_1STPERSON;
                //adventureDataControl = new AdventureDataControl(TC.get("DefaultValue.AdventureTitle"),
                //    TC.get("DefaultValue.ChapterTitle"), TC.get("DefaultValue.SceneId"), playerMode);
                adventureDataControl = new AdventureDataControl(TC.get("DefaultValue.AdventureTitle"), "ChapterTitle", TC.get("DefaultValue.SceneId"), playerMode);
                // Clear the list of data controllers and refill it
                chaptersController = new ChapterListDataControl(adventureDataControl.getChapters());

                // Init project properties (empty)
                ProjectConfigData.init();
                //SCORMConfigData.init();

                AssetsController.createFolderStructure();
                AssetsController.addSpecialAssets();
                AssetsController.copyAssets(currentZipFile, new DirectoryInfo("Assets\\Resources").FullName);

                // Check the consistency of the chapters
                bool valid = chaptersController.isValid(null, null);

                // Save the data
                if (Writer.writeData(currentZipFile, adventureDataControl, valid))
                {
                    // Set modified to false and update the window title
                    dataModified_F = false;

                    Thread.Sleep(1);

                    try
                    {
                        if (selectedFile != null && !selectedFile.Exists)
                            selectedFile.Create().Close();
                    }
                    catch (IOException e)
                    {
                    }

                    // The file was saved
                    fileCreated = true;

                }
                else
                    fileCreated = false;
            }

            if (fileCreated)
            {
                ConfigData.fileLoaded(currentZipFile);
                // Feedback
                //mainWindow.showInformationDialog(TC.get("Operation.FileLoadedTitle"), TC.get("Operation.FileLoadedMessage"));
            }
            //else {
            //    // Feedback
            //    mainWindow.showInformationDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.FileNotLoadedMessage"));
            //}
            //loadingScreen.setVisible(false);

            //Controller.gc();

            return fileCreated;

        }

        //    //public void showLoadingScreen(string message)
        //    //{

        //    //    loadingScreen.setMessage(message);
        //    //    loadingScreen.setVisible(true);
        //    //}

        //    //public void hideLoadingScreen()
        //    //{

        //    //    loadingScreen.setVisible(false);
        //    //}

        //    public bool fixIncidences(List<Incidence> incidences)
        //    {

        //        bool abort = false;
        //        List<Chapter> chapters = this.adventureDataControl.getChapters();

        //        for (int i = 0; i < incidences.size(); i++)
        //        {
        //            Incidence current = incidences.get(i);
        //            // Critical importance: abort operation, the game could not be loaded
        //            if (current.getImportance() == Incidence.IMPORTANCE_CRITICAL)
        //            {
        //                if (current.getException() != null)
        //                    ReportDialog.GenerateErrorReport(current.getException(), true, TC.get("GeneralText.LoadError"));
        //                abort = true;
        //                break;
        //            }
        //            // High importance: the game is partially unreadable, but it is possible to continue.
        //            else if (current.getImportance() == Incidence.IMPORTANCE_HIGH)
        //            {
        //                // An error occurred relating to the load of a chapter which is unreadable.
        //                // When this happens the chapter returned in the adventure data structure is corrupted.
        //                // Options: 1) Delete chapter. 2) Select chapter from other file. 3) Abort
        //                if (current.getAffectedArea() == Incidence.CHAPTER_INCIDENCE && current.getType() == Incidence.XML_INCIDENCE)
        //                {
        //                    string dialogTitle = TC.get("ErrorSolving.Chapter.Title") + " - Error " + (i + 1) + "/" + incidences.size();
        //                    string dialogMessage = TC.get("ErrorSolving.Chapter.Message", new string[] { current.getMessage(), current.getAffectedResource() });
        //                    string[] options = { TC.get("GeneralText.Delete"), TC.get("GeneralText.Replace"), TC.get("GeneralText.Abort"), TC.get("GeneralText.ReportError") };

        //                    int option = showOptionDialog(dialogTitle, dialogMessage, options);
        //                    // Delete chapter
        //                    if (option == 0)
        //                    {
        //                        string chapterName = current.getAffectedResource();
        //                        for (int j = 0; j < chapters.size(); j++)
        //                        {
        //                            if (chapters.get(j).getChapterPath().Equals(chapterName))
        //                            {
        //                                chapters.remove(j);
        //                                //this.chapterDataControlList.remove( j );
        //                                // Update selected chapter if necessary
        //                                if (chapters.size() == 0)
        //                                {
        //                                    // When there are no more chapters, add a new, blank one
        //                                    Chapter newChapter = new Chapter(TC.get("DefaultValue.ChapterTitle"), TC.get("DefaultValue.SceneId"));
        //                                    chapters.add(newChapter);
        //                                    //chapterDataControlList.add( new ChapterDataControl (newChapter) );
        //                                }
        //                                chaptersController = new ChapterListDataControl(chapters);
        //                                dataModified();
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    // Replace chapter
        //                    else if (option == 1)
        //                    {
        //                        bool replaced = false;
        //                        JFileChooser xmlChooser = new JFileChooser();
        //                        xmlChooser.setDialogTitle(TC.get("GeneralText.Select"));
        //                        xmlChooser.setFileFilter(new XMLFileFilter());
        //                        xmlChooser.setMultiSelectionEnabled(false);
        //                        // A file is selected
        //                        if (xmlChooser.showOpenDialog(mainWindow) == JFileChooser.APPROVE_OPTION)
        //                        {
        //                            // Get absolute path
        //                            string absolutePath = xmlChooser.getSelectedFile().getAbsolutePath();
        //                            // Try to load chapter with it
        //                            List<Incidence> newChapterIncidences = new ArrayList<Incidence>();
        //                            Chapter chapter = Loader.loadChapterData(AssetsController.getInputStreamCreator(), absolutePath, incidences);
        //                            // IF no incidences occurred
        //                            if (chapter != null && newChapterIncidences.size() == 0)
        //                            {
        //                                // Try comparing names

        //                                int found = -1;
        //                                for (int j = 0; found == -1 && j < chapters.size(); j++)
        //                                {
        //                                    if (chapters.get(j).getChapterPath().Equals(current.getAffectedResource()))
        //                                    {
        //                                        found = j;
        //                                    }
        //                                }
        //                                // Replace it if found
        //                                if (found >= 0)
        //                                {
        //                                    //this.chapterDataControlList.remove( found );
        //                                    chapters.set(found, chapter);
        //                                    chaptersController = new ChapterListDataControl(chapters);
        //                                    //chapterDataControlList.add( found, new ChapterDataControl(chapter) );

        //                                    // Copy original file to project
        //                                    File destinyFile = new File(this.getProjectFolder(), chapter.getChapterPath());
        //                                    if (destinyFile.exists())
        //                                        destinyFile.delete();
        //                                    File sourceFile = new File(absolutePath);
        //                                    sourceFile.copyTo(destinyFile);
        //                                    replaced = true;
        //                                    dataModified();
        //                                }

        //                            }
        //                        }
        //                        // The chapter was not replaced: inform
        //                        if (!replaced)
        //                        {
        //                            mainWindow.showWarningDialog(TC.get("ErrorSolving.Chapter.NotReplaced.Title"), TC.get("ErrorSolving.Chapter.NotReplaced.Message"));
        //                        }
        //                    }
        //                    // Report Dialog
        //                    else if (option == 3)
        //                    {
        //                        if (current.getException() != null)
        //                            ReportDialog.GenerateErrorReport(current.getException(), true, TC.get("GeneralText.LoadError"));
        //                        abort = true;

        //                    }
        //                    // Other case: abort
        //                    else {
        //                        abort = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            // Medium importance: the game might be slightly affected
        //            else if (current.getImportance() == Incidence.IMPORTANCE_MEDIUM)
        //            {
        //                // If an asset is missing or damaged. Delete references
        //                if (current.getType() == Incidence.ASSET_INCIDENCE)
        //                {
        //                    this.deleteAssetReferences(current.getAffectedResource());
        //                    //  if (current.getAffectedArea( ) == AssetsController.CATEGORY_ICON||current.getAffectedArea( ) == AssetsController.CATEGORY_BACKGROUND){
        //                    //    mainWindow.showInformationDialog( TC.get( "ErrorSolving.Asset.Deleted.Title" ) + " - Error " + ( i + 1 ) + "/" + incidences.size( ), current.getMessage( ) );
        //                    //}else
        //                    mainWindow.showInformationDialog(TC.get("ErrorSolving.Asset.Deleted.Title") + " - Error " + (i + 1) + "/" + incidences.size(), TC.get("ErrorSolving.Asset.Deleted.Message", current.getAffectedResource()));
        //                    if (current.getException() != null)
        //                        ReportDialog.GenerateErrorReport(current.getException(), true, TC.get("GeneralText.LoadError"));

        //                }
        //                // If it was an assessment profile (referenced) delete the assessment configuration of the chapter
        //                else if (current.getAffectedArea() == Incidence.ASSESSMENT_INCIDENCE)
        //                {
        //                    mainWindow.showInformationDialog(TC.get("ErrorSolving.AssessmentReferenced.Deleted.Title") + " - Error " + (i + 1) + "/" + incidences.size(), TC.get("ErrorSolving.AssessmentReferenced.Deleted.Message", current.getAffectedResource()));
        //                    for (int j = 0; j < chapters.size(); j++)
        //                    {
        //                        if (chapters.get(j).getAssessmentName().Equals(current.getAffectedResource()))
        //                        {
        //                            chapters.get(j).setAssessmentName("");
        //                            dataModified();
        //                        }
        //                    }
        //                    if (current.getException() != null)
        //                        ReportDialog.GenerateErrorReport(current.getException(), true, TC.get("GeneralText.LoadError"));
        //                    //	adventureData.getAssessmentRulesListDataControl( ).deleteIdentifierReferences( current.getAffectedResource( ) );
        //                }
        //                // If it was an assessment profile (referenced) delete the assessment configuration of the chapter
        //                else if (current.getAffectedArea() == Incidence.ADAPTATION_INCIDENCE)
        //                {
        //                    mainWindow.showInformationDialog(TC.get("ErrorSolving.AdaptationReferenced.Deleted.Title") + " - Error " + (i + 1) + "/" + incidences.size(), TC.get("ErrorSolving.AdaptationReferenced.Deleted.Message", current.getAffectedResource()));
        //                    for (int j = 0; j < chapters.size(); j++)
        //                    {
        //                        if (chapters.get(j).getAdaptationName().Equals(current.getAffectedResource()))
        //                        {
        //                            chapters.get(j).setAdaptationName("");
        //                            dataModified();
        //                        }
        //                    }
        //                    if (current.getException() != null)
        //                        ReportDialog.GenerateErrorReport(current.getException(), true, TC.get("GeneralText.LoadError"));
        //                    //adventureData.getAdaptationRulesListDataControl( ).deleteIdentifierReferences( current.getAffectedResource( ) );
        //                }

        //                // Abort
        //                else {
        //                    abort = true;
        //                    break;
        //                }
        //            }
        //            // Low importance: the game will not be affected
        //            else if (current.getImportance() == Incidence.IMPORTANCE_LOW)
        //            {
        //                if (current.getAffectedArea() == Incidence.ADAPTATION_INCIDENCE)
        //                {
        //                    //adventureData.getAdaptationRulesListDataControl( ).deleteIdentifierReferences( current.getAffectedResource( ) );
        //                    dataModified();
        //                }
        //                if (current.getAffectedArea() == Incidence.ASSESSMENT_INCIDENCE)
        //                {
        //                    //adventureData.getAssessmentRulesListDataControl( ).deleteIdentifierReferences( current.getAffectedResource( ) );
        //                    dataModified();
        //                }
        //            }

        //        }
        //        return abort;
        //    }

        /**
         * Called when the user wants to load data from a file.
         * 
         * @return True if a file was loaded successfully, false otherwise
         */

        public bool loadFile()
        {

            return loadFile(null, true);
        }

        //    public bool replaceSelectedChapter(Chapter newChapter)
        //    {

        //        chaptersController.replaceSelectedChapter(newChapter);
        //        //mainWindow.updateTree();
        //        mainWindow.reloadData();
        //        return true;
        //    }

        public NPC getNPC(string npcId)
        {

            return this.getSelectedChapterDataControl().getNPCsList().getNPC(npcId);
        }

        private bool loadFile(string completeFilePath, bool loadingImage)
        {

            bool fileLoaded = false;
            bool hasIncedence = false;
            try
            {
                bool loadFile = true;
                // If the data was not saved, ask for an action (save, discard changes...)
                //if (dataModified_F)
                //{
                //    int option = mainWindow.showConfirmDialog(TC.get("Operation.LoadFileTitle"), TC.get("Operation.LoadFileMessage"));

                //    // If the data must be saved, load the new file only if the save was succesful
                //    if (option == JOptionPane.YES_OPTION)
                //        loadFile = saveFile(false);

                //    // If the data must not be saved, load the new data directly
                //    else if (option == JOptionPane.NO_OPTION)
                //    {
                //        loadFile = true;
                //        dataModified = false;
                //        mainWindow.updateTitle();
                //    }

                //    // Cancel the action if selected
                //    else if (option == JOptionPane.CANCEL_OPTION)
                //    {
                //        loadFile = false;
                //    }

                //}
                //TODO: implement
                //if (loadFile && completeFilePath == null)
                //{
                //    //TODO: implement
                //    //this.stopAutoSave();
                //    //ConfigData.loadFromXML(ReleaseFolders.configFileEditorRelativePath());
                //    //ProjectConfigData.loadFromXML();

                //    // Show dialog
                //    // StartDialog start = new StartDialog( StartDialog.OPEN_TAB );
                //   // FrameForInitialDialogs start = new FrameForInitialDialogs(StartDialog.OPEN_TAB);
                //    //start.askForProject();
                //    //mainWindow.setEnabled( false );
                //    //mainWindow.setExtendedState(JFrame.ICONIFIED | mainWindow.getExtendedState());
                //   // mainWindow.setVisible(false);

                //    //int op = start.showOpenDialog( null );
                //    int op = start.showStartDialog();
                //    //start.end();
                //    if (op == StartDialog.NEW_FILE_OPTION)
                //    {
                //        newFile(start.getFileType());
                //    }
                //    else if (op == StartDialog.OPEN_FILE_OPTION)
                //    {
                //        java.io.File selectedFile = start.getSelectedFile();
                //        string absPath = selectedFile.getAbsolutePath().toLowerCase();
                //        if (absPath.endsWith(".eap"))
                //        {
                //            string absolutePath = selectedFile.getPath();
                //            loadFile(absolutePath.substring(0, absolutePath.length() - 4), true);
                //        }
                //        else if (selectedFile.isDirectory() && selectedFile.exists())
                //            loadFile(start.getSelectedFile().getAbsolutePath(), true);
                //        else
                //            // importGame is the same method for .ead, .jar and .zip (LO) import
                //            this.importGame(selectedFile.getAbsolutePath());

                //    }
                //    else if (op == StartDialog.RECENT_FILE_OPTION)
                //    {
                //        loadFile(start.getRecentFile().getAbsolutePath(), true);
                //    }
                //    else if (op == StartDialog.CANCEL_OPTION)
                //    {
                //        //exit( );
                //    }

                //    start.remove();

                //    // if( currentZipFile == null ) {
                //    //   mainWindow.reloadData( );
                //    //}

                //    mainWindow.setResizable(true);
                //    mainWindow.setEnabled(true);
                //    mainWindow.setVisible(true);

                //    return true;
                //}

                //LoadingScreen loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.LoadProject" ), getLoadingImage( ), mainWindow);
                // If some file was selected
                if (completeFilePath != null)
                {
                    //if (loadingImage)
                    //{
                    //    loadingScreen.setMessage(TC.get("Operation.LoadProject"));
                    //    this.loadingScreen.setVisible(true);
                    //    loadingImage = true;
                    //}
                    // Create a file to extract the name and path
                    FileInfo newFile = new FileInfo(completeFilePath);

                    // Load the data from the file, and update the info
                    List<Incidence> incidences = new List<Incidence>();
                    //ls.start( );
                    /*AdventureData loadedAdventureData = Loader.loadAdventureData( AssetsController.getInputStreamCreator(completeFilePath), 
                            AssetsController.getCategoryFolder(AssetsController.CATEGORY_ASSESSMENT),
                            AssetsController.getCategoryFolder(AssetsController.CATEGORY_ADAPTATION),incidences );
                     */
                    AdventureData loadedAdventureData = Loader.loadAdventureData(completeFilePath, incidences);

                    //mainWindow.setNormalState( );

                    // If the adventure was loaded without problems, update the data
                    if (loadedAdventureData != null)
                    {
                        // Update the values of the controller
                        currentZipFile = newFile.FullName;
                        currentZipPath = newFile.DirectoryName;
                        currentZipName = newFile.Name;
                        System.IO.File.WriteAllText("Assets\\Resources\\CurrentGame.eap", completeFilePath);
                        loadedAdventureData.setProjectName(currentZipName);
                        AssetsController.copyAllFiles(currentZipFile, new DirectoryInfo("Assets\\Resources\\CurrentGame\\").FullName);
                        adventureDataControl = new AdventureDataControl(loadedAdventureData);
                        chaptersController = new ChapterListDataControl(adventureDataControl.getChapters());

                        // Check asset files
                        AssetsController.checkAssetFilesConsistency(incidences);
                        Incidence.sortIncidences(incidences);
                        //TODO: implement
                        // If there is any incidence
                        //if (incidences.size() > 0)
                        //{
                        //    bool abort = fixIncidences(incidences);
                        //    if (abort)
                        //    {
                        //        mainWindow.showInformationDialog(TC.get("Error.LoadAborted.Title"), TC.get("Error.LoadAborted.Message"));
                        //        hasIncedence = true;
                        //    }
                        //}

                        ProjectConfigData.loadFromXML();
                        AssetsController.createFolderStructure();
                        AssetsController.addSpecialAssets();

                        dataModified_F = false;

                        // The file was loaded
                        fileLoaded = true;

                        // Reloads the view of the window
                        //mainWindow.reloadData();
                    }
                }
                else if (System.IO.File.Exists("Assets\\Resources\\CurrentGame.eap"))
                {
                    completeFilePath = System.IO.File.ReadAllText("Assets\\Resources\\CurrentGame.eap");
                    FileInfo newFile = new FileInfo(completeFilePath);
                    List<Incidence> incidences = new List<Incidence>();
                    AdventureData loadedAdventureData = Loader.loadAdventureData("Assets\\Resources\\CurrentGame", incidences);
                    if (loadedAdventureData != null)
                    {
                        currentZipFile = newFile.FullName;
                        currentZipPath = newFile.DirectoryName;
                        currentZipName = newFile.Name;

                        System.IO.File.WriteAllText("Assets\\Resources\\CurrentGame.eap", completeFilePath);
                        loadedAdventureData.setProjectName(currentZipName);

                        adventureDataControl = new AdventureDataControl(loadedAdventureData);
                        chaptersController = new ChapterListDataControl(adventureDataControl.getChapters());
                        ProjectConfigData.loadFromXML();
                        /*AssetsController.checkAssetFilesConsistency(incidences);
                        Incidence.sortIncidences(incidences);

                        AssetsController.createFolderStructure();
                        AssetsController.addSpecialAssets();*/

                        dataModified_F = false;
                        fileLoaded = true;
                    }
                }

                //TODO: implement
                //if the file was loaded, update the RecentFiles list:
                if (fileLoaded)
                {
                    ConfigData.fileLoaded(currentZipFile);
                    AssetsController.resetCache();
                    // Load project config file
                    ProjectConfigData.loadFromXML();

                    //startAutoSave(15);

                    //// Feedback
                    ////loadingScreen.close( );
                    //if (!hasIncedence)
                    //    mainWindow.showInformationDialog(TC.get("Operation.FileLoadedTitle"), TC.get("Operation.FileLoadedMessage"));
                    //else
                    //    mainWindow.showInformationDialog(TC.get("Operation.FileLoadedWithErrorTitle"), TC.get("Operation.FileLoadedWithErrorMessage"));

                }
                //else {
                //    // Feedback
                //    //loadingScreen.close( );
                //    mainWindow.showInformationDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.FileNotLoadedMessage"));
                //}

                //if (loadingImage)
                //    //ls.close( );
                //    loadingScreen.setVisible(false);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                fileLoaded = false;
                //if (loadingImage)
                //    loadingScreen.setVisible(false);
                //mainWindow.showInformationDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.FileNotLoadedMessage"));
            }

            //Controller.gc();

            return fileLoaded;
        }

        /**
         * Called when the user wants to save data to a file.
         * 
         * @param saveAs
         *            True if the destiny file must be chosen inconditionally
         * @return True if a file was saved successfully, false otherwise
         */

        public bool saveFile(bool saveAs)
        {

            bool fileSaved = false;
            //try
            //{
            bool saveFile = true;

            // Select a new file if it is a "Save as" action
            //if (saveAs)
            //{
            //    //loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.SaveProjectAs" ), getLoadingImage( ), mainWindow);
            //    //loadingScreen.setVisible( true );
            //    string completeFilePath = null;
            //    //completeFilePath = mainWindow.showSaveDialog(getCurrentLoadFolder(), new FolderFileFilter(false, false, null));
            //    //TODO: implementation
            //    completeFilePath = "C:\\Users\\dijks\\Desktop\\Export" + "\\test1";
            //    // If some file was selected set the new file
            //    if (completeFilePath != null)
            //    {
            //        // Create a file to extract the name and path
            //        DirectoryInfo newFolder;
            //        FileInfo newFile;
            //        if (completeFilePath.EndsWith(".eap"))
            //        {
            //            newFile = new FileInfo(completeFilePath);
            //            newFolder = new DirectoryInfo(completeFilePath.Substring(0, completeFilePath.Length - 4));
            //        }
            //        else
            //        {
            //            newFile = new FileInfo(completeFilePath + ".eap");
            //            newFolder = new DirectoryInfo(completeFilePath);
            //        }
            //        // Check the selectedFolder is not inside a forbidden one

            //        if (isValidTargetProject(newFile))
            //        {

            //            // Debug.Log("TEST newFile" + this.currentZipFile + " " + newFolder.FullName + " " + newFile.FullName + " ");// + newFolder.GetFiles().Length);
            //            //if (FolderFileFilter.checkCharacters(newFolder.getName()))
            //            //{

            //            // If the file doesn't exist, or if the user confirms the writing in the file and the file it is not the current path of the project
            //if ((this.currentZipFile == null ||
            //     !newFolder.FullName.ToLower().Equals(this.currentZipFile.ToLower())) &&
            //    ((!newFile.Exists && !newFolder.Exists) || !newFolder.Exists ||
            //     newFolder.GetFiles().Length == 0))
            //                //|| mainWindow.showStrictConfirmDialog(TC.get("Operation.SaveFileTitle"), TC.get("Operation.NewProject.FolderNotEmptyMessage", newFolder.getName()))))
            //            {
            //                // If the file exists, delete it so it's clean in the first save
            //                //if( newFile.exists( ) )
            //                //	newFile.delete( );

            //                Debug.Log("TEST newFile 2");
            //                if (!newFile.Exists)
            //                    newFile.Create();

            //                // If this is a "Save as" operation, copy the assets from the old file to the new one
            //                if (saveAs)
            //                {
            //                    //loadingScreen.setMessage(TC.get("Operation.SaveProjectAs"));
            //                    //loadingScreen.setVisible(true);
            //                    AssetsController.copyAssets(currentZipFile, newFolder.FullName);
            //                }

            //                // Set the new file and path
            //                currentZipFile = newFolder.FullName;
            //                currentZipPath = newFolder.Parent.FullName;
            //                currentZipName = newFolder.Name;

            //                AssetsController.createFolderStructure();
            //            }

            //            // If the file was not overwritten, don't save the data
            //            else
            //                saveFile = false;

            //            // In case the selected folder is the same that the previous one, report an error
            //            if (!saveFile && this.currentZipFile != null &&
            //                newFolder.FullName.ToLower().Equals(this.currentZipFile.ToLower()))
            //            {
            //                this.showErrorDialog(TC.get("Operation.SaveProjectAs.TargetFolderInUse.Title"),
            //                    TC.get("Operation.SaveProjectAs.TargetFolderInUse.Message"));
            //            }
            //            // }
            //            else
            //            {
            //                Debug.Log("TEST newFile 3");
            //                //this.showErrorDialog(TC.get("Error.Title"), TC.get("Error.ProjectFolderName", FolderFileFilter.getAllowedChars()));
            //                saveFile = false;
            //            }
            //        }
            //        else
            //        {
            //            Debug.Log("TEST newFile 4");
            //            // Show error: The target dir cannot be contained 
            //            // mainWindow.showErrorDialog(TC.get("Operation.NewProject.ForbiddenParent.Title"), TC.get("Operation.NewProject.ForbiddenParent.Message"));
            //            saveFile = false;
            //        }
            //    }

            //    // If no file was selected, don't save the data
            //    else
            //        saveFile = false;
            //}

            if (saveAs)
            {
                //loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.SaveProjectAs" ), getLoadingImage( ), mainWindow);
                //loadingScreen.setVisible( true );
                string completeFilePath = null;
                //completeFilePath = mainWindow.showSaveDialog(getCurrentLoadFolder(), new FolderFileFilter(false, false, null));
                completeFilePath = currentZipName;

                Debug.Log("SAVE AS " + completeFilePath);
                // If some file was selected set the new file
                if (completeFilePath != null)
                {
                    // Create a file to extract the name and path
                    DirectoryInfo newFolder;
                    FileInfo newFile;
                    if (completeFilePath.EndsWith(".eap"))
                    {
                        newFile = new FileInfo(completeFilePath);
                        newFolder = new DirectoryInfo(completeFilePath.Substring(0, completeFilePath.Length - 4));
                    }
                    else
                    {
                        newFile = new FileInfo(completeFilePath + ".eap");
                        newFolder = new DirectoryInfo(completeFilePath);
                    }
                    // Check the selectedFolder is not inside a forbidden one
                    if (isValidTargetProject(newFile))
                    {
                        Debug.Log("TEST0");
                        //if (FolderFileFilter.checkCharacters(newFolder.getName()))
                        //{

                        // If the file doesn't exist, or if the user confirms the writing in the file and the file it is not the current path of the project
                        //if ((this.currentZipFile == null ||
                        //     !newFolder.FullName.ToLower().Equals(this.currentZipFile.ToLower())) &&
                        //    ((!newFile.Exists && !newFolder.Exists) || !newFolder.Exists ||
                        //     newFolder.GetFiles().Length == 0))
                        //{
                        Debug.Log("TEST1");
                        // If the file exists, delete it so it's clean in the first save
                        //if( newFile.exists( ) )
                        //	newFile.delete( );

                        if (!newFile.Exists)
                            System.IO.File.Create(newFile.FullName).Close();

                        // If this is a "Save as" operation, copy the assets from the old file to the new one
                        if (saveAs)
                        {
                            //loadingScreen.setMessage(TC.get("Operation.SaveProjectAs"));
                            //loadingScreen.setVisible(true);
                            AssetsController.copyAssets(new DirectoryInfo("Assets\\Resources").FullName, currentZipFile);
                            AssetsController.copyAssets(currentZipFile, newFolder.FullName);
                            Debug.Log("TEST2");
                        }

                        // Set the new file and path
                        //currentZipFile = newFolder.FullName;
                        //currentZipPath = newFolder.Parent.FullName;
                        //currentZipName = newFolder.Name;

                        AssetsController.createFolderStructure();
                        //}

                        // If the file was not overwritten, don't save the data
                        //TODO: test if it is working
                        //else
                        //    saveFile = false;

                        //}
                        //else {
                        //    //this.showErrorDialog(TC.get("Error.Title"), TC.get("Error.ProjectFolderName", FolderFileFilter.getAllowedChars()));
                        //    saveFile = false;
                        //}
                    }
                    else
                    {
                        // Show error: The target dir cannot be contained 
                        //mainWindow.showErrorDialog(TC.get("Operation.NewProject.ForbiddenParent.Title"), TC.get("Operation.NewProject.ForbiddenParent.Message"));
                        saveFile = false;
                    }
                }

                // If no file was selected, don't save the data
                else
                    saveFile = false;
            }
            else
            {
                //loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.SaveProject" ), getLoadingImage( ), mainWindow);

                //loadingScreen.setVisible( true );
                //loadingScreen.setMessage(TC.get("Operation.SaveProject"));
                //loadingScreen.setVisible(true);
            }

            // If the data must be saved
            if (saveFile)
            {
                //TODO: implement
                ConfigData.storeToXML();
                ProjectConfigData.storeToXML();



                // If the zip was temp file, delete it
                //if( isTempFile( ) ) {
                //	File file = new File( oldZipFile );
                //	file.deleteOnExit( );
                //	isTempFile = false;
                //}

                // Check the consistency of the chapters
                bool valid = chaptersController.isValid(null, null);

                // If the data is not valid, show an error message
                if (!valid)
                    Debug.LogError(TC.get("Operation.AdventureConsistencyTitle") + "\n" +
                                   TC.get("Operation.AdventurInconsistentWarning"));

                // Control the version number
                string newValue = increaseVersionNumber(adventureDataControl.getAdventureData().getVersionNumber());
                adventureDataControl.getAdventureData().setVersionNumber(newValue);

                // Save the data

                if (Writer.writeData(currentZipFile, adventureDataControl, valid))
                {
                    FileInfo eapFile = new FileInfo(currentZipFile + ".eap");
                    if (!eapFile.Exists)
                        File.Create(eapFile.FullName).Close();

                    // Set modified to false and update the window title
                    dataModified_F = false;
                    //mainWindow.updateTitle();

                    // The file was saved
                    fileSaved = true;
                }
            }

            //If the file was saved, update the recent files list:
            if (fileSaved)
            {
                //TODO: implement
                ConfigData.fileLoaded(currentZipFile);
                Debug.Log(currentZipFile);
                ProjectConfigData.storeToXML();

                AssetsController.resetCache();
                // also, look for adaptation and assessment folder, and delete them
                DirectoryInfo currentAssessFolder = new DirectoryInfo(Path.Combine(currentZipFile, "assessment"));
                if (currentAssessFolder.Exists)
                {
                    FileInfo[] files = currentAssessFolder.GetFiles();
                    for (int x = 0; x < files.Length; x++)
                        files[x].Delete();
                    currentAssessFolder.Delete(true);
                }
                DirectoryInfo currentAdaptFolder = new DirectoryInfo(Path.Combine(currentZipFile, "adaptation"));
                if (currentAdaptFolder.Exists)
                {
                    FileInfo[] files = currentAdaptFolder.GetFiles();
                    for (int x = 0; x < files.Length; x++)
                        files[x].Delete();
                    currentAdaptFolder.Delete();
                }
            }
            //}
            //catch (Exception e)
            //{
            //    fileSaved = false;
            //    Debug.LogError(TC.get("Operation.FileNotSavedTitle") + TC.get("Operation.FileNotSavedMessage"));

            //}

            //Controller.gc();

            // loadingScreen.setVisible(false);

            return fileSaved;
        }

        /**
         * Increase the game version number
         * 
         * @param digits
         * @param index
         * @return the version number after increase it
         */

        private string increaseVersionNumber(char[] digits, int index)
        {

            if (digits[index] != '9')
            {
                // increase in "1" the ASCII code 
                digits[index]++;
                return new string(digits);
            }
            else if (index == 0)
            {
                char[] aux = new char[digits.Length + 1];
                aux[0] = '1';
                aux[1] = '0';
                for (int i = 2; i < aux.Length; i++)
                    aux[i] = digits[i - 1];
                return new string(aux);

            }
            else
            {
                digits[index] = '0';
                return increaseVersionNumber(digits, --index);
            }

        }

        private string increaseVersionNumber(string versionNumber)
        {

            char[] digits = versionNumber.ToCharArray();
            return increaseVersionNumber(digits, digits.Length - 1);
        }

        //    public void importGame()
        //    {

        //        importGame(null);
        //    }

        //    public void importGame(string eadPath)
        //    {

        //        bool importGame = true;
        //        java.io.File selectedFile = null;
        //        try
        //        {
        //            if (dataModified)
        //            {
        //                int option = mainWindow.showConfirmDialog(TC.get("Operation.SaveChangesTitle"), TC.get("Operation.SaveChangesMessage"));
        //                // If the data must be saved, load the new file only if the save was succesful
        //                if (option == JOptionPane.YES_OPTION)
        //                    importGame = saveFile(false);

        //                // If the data must not be saved, load the new data directly
        //                else if (option == JOptionPane.NO_OPTION)
        //                {
        //                    importGame = true;
        //                    dataModified = false;
        //                    mainWindow.updateTitle();
        //                }

        //                // Cancel the action if selected
        //                else if (option == JOptionPane.CANCEL_OPTION)
        //                {
        //                    importGame = false;
        //                }

        //            }

        //            if (importGame)
        //            {
        //                if (eadPath.endsWith(".zip"))
        //                    mainWindow.showInformationDialog(TC.get("Operation.ImportProject"), TC.get("Operation.ImportLO.InfoMessage"));
        //                else if (eadPath.endsWith(".jar"))
        //                    mainWindow.showInformationDialog(TC.get("Operation.ImportProject"), TC.get("Operation.ImportJAR.InfoMessage"));

        //                // Ask origin file
        //                JFileChooser chooser = new JFileChooser();
        //                chooser.setFileFilter(new EADFileFilter());
        //                chooser.setMultiSelectionEnabled(false);
        //                chooser.setCurrentDirectory(new File(getCurrentExportSaveFolder()));
        //                int option = JFileChooser.APPROVE_OPTION;
        //                if (eadPath == null)
        //                    option = chooser.showOpenDialog(mainWindow);
        //                if (option == JFileChooser.APPROVE_OPTION)
        //                {
        //                    java.io.File originFile = null;
        //                    if (eadPath == null)
        //                        originFile = chooser.getSelectedFile();
        //                    else
        //                        originFile = new File(eadPath);

        //                    // if( !originFile.getAbsolutePath( ).endsWith( ".ead" ) )
        //                    //   originFile = new java.io.File( originFile.getAbsolutePath( ) + ".ead" );

        //                    // If the file not exists display error
        //                    if (!originFile.exists())
        //                        mainWindow.showErrorDialog(TC.get("Error.Import.FileNotFound.Title"), TC.get("Error.Import.FileNotFound.Title", originFile.getName()));
        //                    // Otherwise ask folder for the new project
        //                    else {
        //                        bool create = false;
        //                        java.io.File selectedDir = null;
        //                        // Prompt main folder of the project
        //                        // ProjectFolderChooser folderSelector = new ProjectFolderChooser( false, false );
        //                        FrameForInitialDialogs start = new FrameForInitialDialogs(false);
        //                        // If some folder is selected, check all characters are correct  
        //                        int op = start.showStartDialog();
        //                        if (op == StartDialog.APROVE_SELECTION)
        //                        {
        //                            java.io.File selectedFolder = start.getSelectedFile();
        //                            selectedFile = selectedFolder;
        //                            if (selectedFolder.getAbsolutePath().endsWith(".eap"))
        //                            {
        //                                string absolutePath = selectedFolder.getAbsolutePath();
        //                                selectedFolder = new java.io.File(absolutePath.substring(0, absolutePath.length() - 4));
        //                            }
        //                            else {
        //                                selectedFile = new java.io.File(selectedFolder.getAbsolutePath() + ".eap");
        //                            }

        //                            selectedDir = selectedFolder;

        //                            // Check the selectedFolder is not inside a forbidden one
        //                            if (isValidTargetProject(selectedFolder))
        //                            {
        //                                if (FolderFileFilter.checkCharacters(selectedFolder.getName()))
        //                                {
        //                                    // Folder can be created/used
        //                                    // Does the folder exist?
        //                                    if (selectedFolder.exists())
        //                                    {
        //                                        //Is the folder empty?
        //                                        if (selectedFolder.list().length > 0)
        //                                        {
        //                                            // Delete content?
        //                                            if (this.showStrictConfirmDialog(TC.get("Operation.NewProject.FolderNotEmptyTitle"), TC.get("Operation.NewProject.FolderNotEmptyMessage")))
        //                                            {
        //                                                File directory = new File(selectedFolder.getAbsolutePath());
        //                                                if (directory.deleteAll())
        //                                                {
        //                                                    create = true;
        //                                                }
        //                                                else {
        //                                                    this.showStrictConfirmDialog(TC.get("Error.Title"), TC.get("Error.DeletingFolderContents"));
        //                                                }

        //                                            } // FIXME: else branch to return to previous dialog when the user tries to assign an existing name to his project
        //                                              // and select "no" in re-writing confirmation panel

        //                                        }
        //                                        else {
        //                                            create = true;
        //                                        }
        //                                    }
        //                                    else {
        //                                        // Create new folder?
        //                                        File directory = new File(selectedFolder.getAbsolutePath());
        //                                        if (directory.mkdirs())
        //                                        {
        //                                            create = true;
        //                                        }
        //                                        else {
        //                                            this.showStrictConfirmDialog(TC.get("Error.Title"), TC.get("Error.CreatingFolder"));
        //                                        }
        //                                    }
        //                                }
        //                                else {
        //                                    // Display error message
        //                                    this.showErrorDialog(TC.get("Error.Title"), TC.get("Error.ProjectFolderName", FolderFileFilter.getAllowedChars()));
        //                                }
        //                            }
        //                            else {
        //                                // Show error: The target dir cannot be contained 
        //                                mainWindow.showErrorDialog(TC.get("Operation.NewProject.ForbiddenParent.Title"), TC.get("Operation.NewProject.ForbiddenParent.Message"));
        //                                create = false;
        //                            }
        //                        }

        //                        start.remove();
        //                        // Create the new project?
        //                        if (create)
        //                        {
        //                            //LoadingScreen loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.ImportProject" ), getLoadingImage( ), mainWindow);
        //                            loadingScreen.setMessage(TC.get("Operation.ImportProject"));
        //                            loadingScreen.setVisible(true);
        //                            //AssetsController.createFolderStructure();
        //                            if (!selectedDir.exists())
        //                                selectedDir.mkdirs();

        //                            if (selectedFile != null && !selectedFile.exists())
        //                                selectedFile.createNewFile();

        //                            bool correctFile = true;
        //                            // Unzip directory

        //                            if (eadPath != null && eadPath.endsWith(".ead"))
        //                            {
        //                                bool newerVersionRequired = Loader.requiresNewVersion(new java.io.File(eadPath));
        //                                if (newerVersionRequired)
        //                                {
        //                                    // FIXME Internationalize
        //                                    mainWindow.showErrorDialog(TC.get("Error.LoadAborted.Title"), "Newer version required");
        //                                    loadingScreen.setVisible(false);
        //                                    correctFile = false;
        //                                }
        //                                else {
        //                                    File.unzipDir(originFile.getAbsolutePath(), selectedDir.getAbsolutePath());
        //                                }
        //                            }
        //                            else if (eadPath.endsWith(".zip"))
        //                            {

        //                                // import EadJAR returns false when selected jar is not a eadventure jar
        //                                if (!File.importEadventureLO(originFile.getAbsolutePath(), selectedDir.getAbsolutePath()))
        //                                {
        //                                    loadingScreen.setVisible(false);
        //                                    mainWindow.showErrorDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.ImportLO.FileNotLoadedMessage"));
        //                                    correctFile = false;
        //                                }
        //                                else {
        //                                    // Remove gui/options/**/*.png
        //                                    cleanImportedDefaultGUIImages(selectedDir);
        //                                }
        //                            }
        //                            else if (eadPath.endsWith(".jar"))
        //                            {
        //                                // import EadLO returns false when selected zip is not a eadventure LO

        //                                if (!File.importEadventureJar(originFile.getAbsolutePath(), selectedDir.getAbsolutePath()))
        //                                {
        //                                    loadingScreen.setVisible(false);
        //                                    mainWindow.showErrorDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.ImportJAR.FileNotLoaded"));
        //                                    correctFile = false;
        //                                }
        //                                else {
        //                                    // Remove gui/options/**/*.png
        //                                    cleanImportedDefaultGUIImages(selectedDir);
        //                                }


        //                            }
        //                            //ProjectConfigData.loadFromXML( );

        //                            // Load new project
        //                            if (correctFile)
        //                            {
        //                                loadFile(selectedDir.getAbsolutePath(), false);
        //                                //loadingScreen.close( );
        //                                loadingScreen.setVisible(false);
        //                            }
        //                            else {
        //                                //remove .eapFile
        //                                selectedFile.delete();
        //                                selectedDir.delete();

        //                            }
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            loadingScreen.setVisible(false);
        //            mainWindow.showErrorDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.FileNotLoadedMessage"));

        //        }
        //    }

        public bool exportGame()
        {
            return exportGame("Games\\" + currentZipName);
        }

        //    /**
        //     * When a game is imported from Jar or Zip, default GUI images like those for game menu are imported as well.
        //     * This method deletes default images under gui/options.
        //     * @param selectedDir
        //     */
        //    private void cleanImportedDefaultGUIImages(java.io.File selectedDir)
        //    {
        //        File options = null;
        //        if (selectedDir.getAbsolutePath().endsWith("/") ||
        //                selectedDir.getAbsolutePath().endsWith("\\"))
        //        {
        //            options = new File(selectedDir.getAbsolutePath() + "gui/options/");
        //        }
        //        else {
        //            options = new File(selectedDir.getAbsolutePath() + "/gui/options/");
        //        }
        //        for (File subDir: options.listFiles())
        //        {
        //            if (subDir.isDirectory())
        //            {
        //                for (File image: subDir.listFiles())
        //                {
        //                    if (image.getAbsolutePath().toLowerCase().endsWith("png"))
        //                        image.delete();
        //                }
        //                subDir.delete();
        //            }
        //            else if (subDir.getAbsolutePath().toLowerCase().endsWith("png"))
        //            {
        //                subDir.delete();
        //            }
        //        }

        //    }

        public bool exportGame(string targetFilePath)
        {

            bool exportGame = true;
            bool exported = false;
            exportGame = saveFile(true);
            //TODO: testing
            //if (dataModified_F)
            //{

            //    //int option = mainWindow.showConfirmDialog(TC.get("Operation.SaveChangesTitle"), TC.get("Operation.SaveChangesMessage"));

            //    //// If the data must be saved, load the new file only if the save was succesful
            //    //if (option == JOptionPane.YES_OPTION)
            //    exportGame = saveFile(true);

            //    //// If the data must not be saved, load the new data directly
            //    //else if (option == JOptionPane.NO_OPTION)
            //    //    exportGame = true;

            //    //// Cancel the action if selected
            //    //else if (option == JOptionPane.CANCEL_OPTION)
            //    //    exportGame = false;
            //}

            if (exportGame)
            {
                string selectedPath = targetFilePath;
                //TODO: implementation
                //if (selectedPath == null)
                //    selectedPath = mainWindow.showSaveDialog(getCurrentExportSaveFolder(), new EADFileFilter());
                if (selectedPath != null)
                {
                    if (!selectedPath.ToLower().EndsWith(".eap"))
                        selectedPath = selectedPath + ".eap";
                    AssetsController.copyAllFiles(currentZipFile, new DirectoryInfo(targetFilePath).FullName);
                    FileInfo destinyFile = new FileInfo(selectedPath);

                    // Check the destinyFile is not in the project folder
                    if (targetFilePath != null || isValidTargetFile(destinyFile))
                    {

                        // If the file exists, ask to overwrite
                        if (!destinyFile.Exists || targetFilePath != null)
                        //|| mainWindow.showStrictConfirmDialog(TC.get("Operation.SaveFileTitle"), TC.get("Operation.OverwriteExistingFile", destinyFile.getName())))
                        {
                            destinyFile.Delete();

                            // Finally, export it
                            //LoadingScreen loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.ExportProject.AsEAD" ), getLoadingImage( ), mainWindow);
                            //if (targetFilePath == null)
                            //{
                            //    loadingScreen.setMessage(TC.get("Operation.ExportProject.AsEAD"));
                            //    loadingScreen.setVisible(true);
                            //}
                            if (Writer.export(getProjectFolder(), destinyFile.FullName))
                            {
                                exported = true;
                                if (targetFilePath == null)
                                    Debug.Log(TC.get("Operation.ExportT.Success.Title") +
                                              TC.get("Operation.ExportT.Success.Message"));
                            }
                            else
                            {
                                Debug.LogError(TC.get("Operation.ExportT.NotSuccess.Title") +
                                               TC.get("Operation.ExportT.NotSuccess.Message"));
                            }
                            //loadingScreen.close( );
                            //if (targetFilePath == null)
                            //    loadingScreen.setVisible(false);
                        }
                    }
                    else
                    {
                        // Show error: The target dir cannot be contained 
                        Debug.LogError(TC.get("Operation.ExportT.TargetInProjectDir.Title") +
                                       TC.get("Operation.ExportT.TargetInProjectDir.Message"));
                    }
                }
            }

            return exported;
        }

        //    public bool createBackup(string targetFilePath)
        //    {

        //        bool fileSaved = false;
        //        if (targetFilePath == null)
        //            targetFilePath = currentZipFile + ".tmp";
        //        File category = new File(currentZipFile, "backup");
        //        try
        //        {
        //            bool valid = chaptersController.isValid(null, null);

        //            category.create();

        //            if (Writer.writeData(currentZipFile + File.separatorChar + "backup", adventureDataControl, valid))
        //            {
        //                fileSaved = true;
        //            }

        //            if (fileSaved)
        //            {
        //                string selectedPath = targetFilePath;

        //                if (selectedPath != null)
        //                {

        //                    java.io.File destinyFile = new File(selectedPath);

        //                    if (targetFilePath != null || isValidTargetFile(destinyFile))
        //                    {
        //                        if (!destinyFile.exists() || targetFilePath != null)
        //                        {
        //                            destinyFile.delete();
        //                            if (Writer.export(getProjectFolder(), destinyFile.getAbsolutePath()))
        //                                fileSaved = true;
        //                        }
        //                    }
        //                    else
        //                        fileSaved = false;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            fileSaved = false;
        //        }

        //        if (category.exists())
        //        {
        //            category.deleteAll();
        //        }

        //        return fileSaved;
        //    }

        //    public void exportStandaloneGame()
        //    {

        //        bool exportGame = true;
        //        try
        //        {
        //            if (dataModified)
        //            {
        //                int option = mainWindow.showConfirmDialog(TC.get("Operation.SaveChangesTitle"), TC.get("Operation.SaveChangesMessage"));
        //                // If the data must be saved, load the new file only if the save was succesful
        //                if (option == JOptionPane.YES_OPTION)
        //                    exportGame = saveFile(false);

        //                // If the data must not be saved, load the new data directly
        //                else if (option == JOptionPane.NO_OPTION)
        //                    exportGame = true;

        //                // Cancel the action if selected
        //                else if (option == JOptionPane.CANCEL_OPTION)
        //                    exportGame = false;

        //            }

        //            if (exportGame)
        //            {
        //                string completeFilePath = null;
        //                completeFilePath = mainWindow.showSaveDialog(getCurrentExportSaveFolder(), new JARFileFilter());

        //                if (completeFilePath != null)
        //                {

        //                    if (!completeFilePath.toLowerCase().endsWith(".jar"))
        //                        completeFilePath = completeFilePath + ".jar";
        //                    // If the file exists, ask to overwrite
        //                    java.io.File destinyFile = new File(completeFilePath);

        //                    // Check the destinyFile is not in the project folder
        //                    if (isValidTargetFile(destinyFile))
        //                    {

        //                        if (!destinyFile.exists() || mainWindow.showStrictConfirmDialog(TC.get("Operation.SaveFileTitle"), TC.get("Operation.OverwriteExistingFile", destinyFile.getName())))
        //                        {
        //                            destinyFile.delete();

        //                            // Finally, export it
        //                            loadingScreen.setMessage(TC.get("Operation.ExportProject.AsJAR"));
        //                            loadingScreen.setVisible(true);
        //                            if (Writer.exportStandalone(getProjectFolder(), destinyFile.getAbsolutePath()))
        //                            {
        //                                mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        //                            }
        //                            else {
        //                                mainWindow.showInformationDialog(TC.get("Operation.ExportT.NotSuccess.Title"), TC.get("Operation.ExportT.NotSuccess.Message"));
        //                            }
        //                            loadingScreen.setVisible(false);

        //                        }
        //                    }
        //                    else {
        //                        // Show error: The target dir cannot be contained 
        //                        mainWindow.showErrorDialog(TC.get("Operation.ExportT.TargetInProjectDir.Title"), TC.get("Operation.ExportT.TargetInProjectDir.Message"));
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            loadingScreen.setVisible(false);
        //            mainWindow.showErrorDialog(TC.get("Operation.FileNotSavedTitle"), TC.get("Operation.FileNotSavedMessage"));
        //        }

        //    }

        ////    public void exportToLOM()
        ////    {

        ////        bool exportFile = true;
        ////        try
        ////        {
        ////            if (dataModified)
        ////            {
        ////                int option = mainWindow.showConfirmDialog(TC.get("Operation.SaveChangesTitle"), TC.get("Operation.SaveChangesMessage"));
        ////                // If the data must be saved, load the new file only if the save was succesful
        ////                if (option == JOptionPane.YES_OPTION)
        ////                    exportFile = saveFile(false);

        ////                // If the data must not be saved, load the new data directly
        ////                else if (option == JOptionPane.NO_OPTION)
        ////                    exportFile = true;

        ////                // Cancel the action if selected
        ////                else if (option == JOptionPane.CANCEL_OPTION)
        ////                    exportFile = false;

        ////            }

        ////            if (exportFile)
        ////            {
        ////                // Ask the data of the Learning Object:
        ////                ExportToLOMDialog dialog = new ExportToLOMDialog(TC.get("Operation.ExportToLOM.DefaultValue"));
        ////                string loName = dialog.getLomName();
        ////                string authorName = dialog.getAuthorName();
        ////                string organization = dialog.getOrganizationName();
        ////                bool windowed = dialog.getWindowed();
        ////                int type = dialog.getType1();

        ////                // For GAMETEL
        ////                string testUserId = dialog.getTestUserID();
        ////                string testReturnURI = dialog.getTestReturnURI();

        ////                bool validated = dialog.isValidated();

        ////                if ((type == 2 || type == 6) && !hasScormProfiles(SCORM12))
        ////                {
        ////                    // error situation: both profiles must be scorm 1.2 if they exist
        ////                    mainWindow.showErrorDialog(TC.get("Operation.ExportSCORM12.BadProfiles.Title"), TC.get("Operation.ExportSCORM12.BadProfiles.Message"));
        ////                }
        ////                else if ((type == 3 || type == 7) && !hasScormProfiles(SCORM2004))
        ////                {
        ////                    // error situation: both profiles must be scorm 2004 if they exist
        ////                    mainWindow.showErrorDialog(TC.get("Operation.ExportSCORM2004.BadProfiles.Title"), TC.get("Operation.ExportSCORM2004.BadProfiles.Message"));
        ////                }
        ////                else if (type == 4 && !hasScormProfiles(AGREGA))
        ////                {
        ////                    // error situation: both profiles must be scorm 2004 if they exist to export to AGREGA
        ////                    mainWindow.showErrorDialog(TC.get("Operation.ExportSCORM2004AGREGA.BadProfiles.Title"), TC.get("Operation.ExportSCORM2004AGREGA.BadProfiles.Message"));
        ////                }
        ////                //TODO comprobaciones de perfiles
        ////                // else if( type == 5 ) {
        ////                // error situation: both profiles must be scorm 2004 if they exist to export to AGREGA
        ////                //   mainWindow.showErrorDialog( TC.get( "Operation.ExportSCORM2004AGREGA.BadProfiles.Title" ), TC.get( "Operation.ExportSCORM2004AGREGA.BadProfiles.Message" ) );

        ////                if (validated)
        ////                {
        ////                    //string loName = this.showInputDialog( TextConstants.getText( "Operation.ExportToLOM.Title" ), TextConstants.getText( "Operation.ExportToLOM.Message" ), TextConstants.getText( "Operation.ExportToLOM.DefaultValue" ));
        ////                    if (loName != null && !loName.Equals("") && !loName.Contains(" ") && !loName.Contains("ďż˝") && !loName.Contains("ďż˝"))
        ////                    {
        ////                        //Check authorName & organization
        ////                        if (authorName != null && authorName.length() > 5 && organization != null && organization.length() > 5)
        ////                        {

        ////                            //Ask for the name of the zip
        ////                            string completeFilePath = null;
        ////                            completeFilePath = mainWindow.showSaveDialog(getCurrentExportSaveFolder(), new FileFilter() {

        ////                                @Override
        ////                                public bool accept(java.io.File arg0)
        ////    {

        ////        return arg0.getAbsolutePath().toLowerCase().endsWith(".zip") || arg0.isDirectory();
        ////    }

        ////    @Override
        ////                                public string getDescription()
        ////    {

        ////        return "Zip files (*.zip)";
        ////    }
        ////} );

        ////                            // If some file was selected set the new file
        ////                            if( completeFilePath != null ) {
        ////                                // Add the ".zip" if it is not present in the name
        ////                                if( !completeFilePath.toLowerCase( ).endsWith( ".zip" ) )
        ////                                    completeFilePath += ".zip";

        ////                                // Create a file to extract the name and path
        ////                                File newFile = new File(completeFilePath);

        ////                                // Check the selected file is contained in a valid folder
        ////                                if( isValidTargetFile(newFile ) ) {

        ////                                    // If the file doesn't exist, or if the user confirms the writing in the file
        ////                                    if( !newFile.exists( ) || mainWindow.showStrictConfirmDialog( TC.get( "Operation.SaveFileTitle" ), TC.get( "Operation.OverwriteExistingFile", newFile.getName( ) ) ) ) {
        ////                                        // If the file exists, delete it so it's clean in the first save

        ////                                        try {
        ////                                            if( newFile.exists( ) )
        ////                                                newFile.delete( );

        ////                                            //change the old animations to eaa animations, this method force to save game
        ////                                            changeAllAnimationFormats();
        ////                                            saveFile( false );

        //////LoadingScreen loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.ExportProject.AsJAR" ), getLoadingImage( ), mainWindow);
        ////loadingScreen.setMessage( TC.get( "Operation.ExportProject.AsLO" ) );
        ////                                            loadingScreen.setVisible( true );
        ////                                            this.updateLOMLanguage( );

        ////                                            if( type == 0 && Writer.exportAsLearningObject( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////}
        ////                                            else if( type == 1 && Writer.exportAsWebCTObject( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////}
        ////                                            else if( type == 2 && Writer.exportAsSCORM( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl, false ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));

        ////}
        ////                                            else if( type == 3 && Writer.exportAsSCORM2004( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl, false ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////}
        ////                                            else if( type == 4 && Writer.exportAsAGREGA( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////}
        ////                                            else if( type == 5 && Writer.exportAsLAMSLearningObject( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////}
        ////                                            else if( type == 8 && Writer.exportAsGAMETELLearningObject( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, testReturnURI, testUserId, adventureDataControl ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////} else if( type == 6 && Writer.exportAsSCORM( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl, true ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////} else if( type == 7 && Writer.exportAsSCORM2004( completeFilePath, loName, authorName, organization, windowed, this.currentZipFile, adventureDataControl, true ) ) {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.Success.Title"), TC.get("Operation.ExportT.Success.Message"));
        ////}
        ////                                            else {
        ////    mainWindow.showInformationDialog(TC.get("Operation.ExportT.NotSuccess.Title"), TC.get("Operation.ExportT.NotSuccess.Message"));
        ////}

        //////loadingScreen.close( );
        ////loadingScreen.setVisible( false );

        ////}
        ////                                        catch( Exception e ) {
        ////                                            this.showErrorDialog( TC.get( "Operation.ExportToLOM.LONameNotValid.Title" ), TC.get("Operation.ExportToLOM.LONameNotValid.Title") );
        ////ReportDialog.GenerateErrorReport(e, true, TC.get("Operation.ExportToLOM.LONameNotValid.Title"));
        ////hideLoadingScreen();
        ////}

        ////}
        ////                                }
        ////                                else {
        ////                                    // Show error: The target dir cannot be contained 
        ////                                    mainWindow.showErrorDialog( TC.get( "Operation.ExportT.TargetInProjectDir.Title" ), TC.get( "Operation.ExportT.TargetInProjectDir.Message" ) );
        ////                                    hideLoadingScreen();
        ////                                }

        ////                            }
        ////                        }
        ////                        else {
        ////                            this.showErrorDialog( TC.get( "Operation.ExportToLOM.AuthorNameOrganizationNotValid.Title" ), TC.get("Operation.ExportToLOM.AuthorNameOrganizationNotValid.Message") );
        ////}
        ////}
        ////                    else {
        ////                        this.showErrorDialog( TC.get( "Operation.ExportToLOM.LONameNotValid.Title" ), TC.get("Operation.ExportToLOM.LONameNotValid.Message") );
        ////}
        ////}
        ////            }
        ////        }
        ////        catch( Exception e ) {
        ////            loadingScreen.setVisible( false );
        ////            mainWindow.showErrorDialog( "Operation.FileNotSavedTitle", "Operation.FileNotSavedMessage" );
        ////        }

        ////    }

        //    /**
        //     * Check if assessment and adaptation profiles are both scorm 1.2 or scorm
        //     * 2004
        //     * 
        //     * @param scormType
        //     *            the scorm type, 1.2 or 2004
        //     * @return
        //     */
        //    private bool hasScormProfiles(int scormType)
        //{

        //    if (scormType == SCORM12)
        //    {
        //        // check that adaptation and assessment profiles are scorm 1.2 profiles
        //        return chaptersController.hasScorm12Profiles(adventureDataControl);

        //    }
        //    else if (scormType == SCORM2004 || scormType == AGREGA)
        //    {
        //        // check that adaptation and assessment profiles are scorm 2004 profiles
        //        return chaptersController.hasScorm2004Profiles(adventureDataControl);

        //    }

        //    return false;
        //}

        ///**
        // * Executes the current project. Firstly, it checks that the game does not
        // * present any consistency errors. Then exports the project to the web dir
        // * as a temp .ead file and gets it running
        // */
        ////public void run()
        ////{

        ////    stopAutoSave();

        ////    // Check adventure consistency
        ////    if (checkAdventureConsistency(false))
        ////    {
        ////        this.getSelectedChapterDataControl().getConversationsList().resetAllConversationNodes();
        ////        new Timer().schedule(new TimerTask() {

        ////                @Override
        ////                public void run()
        ////{

        ////    if (canBeRun())
        ////    {
        ////        mainWindow.setNormalRunAvailable(false);
        ////        // First update flags
        ////        chaptersController.updateVarsFlagsForRunning();
        ////        EAdventureDebug.runOrDebug(Controller.getInstance().adventureDataControl.getAdventureData(), AssetsController.getInputStreamCreator(), buildRunAndDebugSettings(false));
        ////        Controller.getInstance().startAutoSave(15);
        ////        mainWindow.setNormalRunAvailable(true);
        ////    }

        ////}

        ////            }, 1000 );
        ////        }
        ////    }

        ////    /**
        ////     * Executes the current project. Firstly, it checks that the game does not
        ////     * present any consistency errors. Then exports the project to the web dir
        ////     * as a temp .ead file and gets it running
        ////     */
        ////    public void debugRun()
        ////{

        ////    stopAutoSave();

        ////    // Check adventure consistency
        ////    if (checkAdventureConsistency(false))
        ////    {
        ////        this.getSelectedChapterDataControl().getConversationsList().resetAllConversationNodes();
        ////        new Timer().schedule(new TimerTask() {

        ////                @Override
        ////                public void run()
        ////{

        ////    if (canBeRun())
        ////    {
        ////        mainWindow.setNormalRunAvailable(false);
        ////        chaptersController.updateVarsFlagsForRunning();
        ////        EAdventureDebug.runOrDebug(Controller.getInstance().adventureDataControl.getAdventureData(), AssetsController.getInputStreamCreator(), buildRunAndDebugSettings(true));
        ////        Controller.getInstance().startAutoSave(15);
        ////        mainWindow.setNormalRunAvailable(true);
        ////    }
        ////}

        ////            }, 1000 );
        ////        }
        ////    }

        ////    /**
        ////     * Check if the current project is saved before run. If not, ask user to
        ////     * save it.
        ////     * 
        ////     * @return if false is returned, the game will not be launched
        ////     */
        ////    private bool canBeRun()
        ////{

        ////    if (dataModified)
        ////    {
        ////        if (mainWindow.showStrictConfirmDialog(TC.get("Run.CanBeRun.Title"), TC.get("Run.CanBeRun.Text")))
        ////        {
        ////            this.saveFile(false);
        ////            return true;
        ////        }
        ////        else
        ////            return false;
        ////    }
        ////    else
        ////        return true;
        ////}

        /**
         * Determines if the target file of an exportation process is valid. The
         * file cannot be located neither inside the project folder, nor inside the
         * web folder
         * 
         * @param targetFile
         * @return
         */
        private bool isValidTargetFile(FileInfo targetFile)
        {

            FileInfo[] forbiddenParents = new FileInfo[]
            {new FileInfo(ReleaseFolders.WEB_FOLDER), new FileInfo(ReleaseFolders.WEB_TEMP_FOLDER)};
            //, getProjectFolderFile()};
            bool isValid = true;
            foreach (FileInfo forbiddenParent in forbiddenParents)
            {
                Debug.Log(targetFile.FullName + " vs " + forbiddenParent.FullName);
                if (targetFile.FullName.ToLower().StartsWith(forbiddenParent.FullName.ToLower()))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        /**
         * Determines if the target folder for a new project is valid. The folder
         * cannot be located inside the web folder
         * 
         * @param targetFile
         * @return
         */

        private bool isValidTargetProject(FileInfo targetFile)
        {

            FileInfo[] forbiddenParents = new FileInfo[]
            {new FileInfo(ReleaseFolders.WEB_FOLDER), new FileInfo(ReleaseFolders.WEB_TEMP_FOLDER)};
            bool isValid = true;
            foreach (FileInfo forbiddenParent in forbiddenParents)
            {
                if (targetFile.FullName.ToLower().StartsWith(forbiddenParent.FullName.ToLower()))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        ///**
        // * Confirm user that the conditions associated to description block will be
        // * deleted
        // * 
        // * @return
        // */
        ////public bool askDeleteConditionsDescriptionSet()
        ////{

        ////    int option = mainWindow.showConfirmDialog(TC.get("DescriptionSet.deleteOnlyOneDescription.title"), TC.get("DescriptionSet.deleteOnlyOneDescription.message"));

        ////    if (option == JOptionPane.YES_OPTION)
        ////        return true;

        ////    else if (option == JOptionPane.NO_OPTION || option == JOptionPane.CANCEL_OPTION)
        ////        return false;

        ////    return false;

        ////}

        ///**
        // * Confirm user that the conditions associated to resource block will be
        // * deleted
        // * 
        // * @return
        // */
        ////public bool askDeleteConditionsResourceBlock()
        ////{

        ////    int option = mainWindow.showConfirmDialog(TC.get("ResourceBlock.deleteOnlyOneBlock.title"), TC.get("ResourceBlock.deleteOnlyOneBlock.message"));

        ////    if (option == JOptionPane.YES_OPTION)
        ////        return true;

        ////    else if (option == JOptionPane.NO_OPTION || option == JOptionPane.CANCEL_OPTION)
        ////        return false;

        ////    return false;

        ////}

        ///**
        // * Exits from the aplication.
        // */
        ////public void exit()
        ////{

        ////    bool exit = true;

        ////    // If the data was not saved, ask for an action (save, discard changes...)
        ////    if (dataModified)
        ////    {
        ////        int option = mainWindow.showConfirmDialog(TC.get("Operation.ExitTitle"), TC.get("Operation.ExitMessage"));

        ////        // If the data must be saved, lexit only if the save was succesful
        ////        if (option == JOptionPane.YES_OPTION)
        ////            exit = saveFile(false);

        ////        // If the data must not be saved, exit directly
        ////        else if (option == JOptionPane.NO_OPTION)
        ////            exit = true;

        ////        // Cancel the action if selected
        ////        else if (option == JOptionPane.CANCEL_OPTION)
        ////            exit = false;

        ////        //if( isTempFile( ) ) {
        ////        //	File file = new File( oldZipFile );
        ////        //	file.deleteOnExit( );
        ////        //	isTempFile = false;
        ////        //}
        ////    }

        ////    // Exit the aplication
        ////    if (exit)
        ////    {
        ////        ConfigData.storeToXML();
        ////        ProjectConfigData.storeToXML();
        ////        //AssetsController.cleanVideoCache( );
        ////        System.exit(0);
        ////    }
        ////}

        ///**
        // * Checks if the adventure is valid or not. It shows information to the
        // * user, whether the data is valid or not.
        // */
        //public bool checkAdventureConsistency()
        //{

        //    return checkAdventureConsistency(true);
        //}

        //public bool checkAdventureConsistency(bool showSuccessFeedback)
        //{

        //    // Create a list to store the incidences
        //    List<string> incidences = new ArrayList<string>();

        //    // Check all the chapters
        //    bool valid = chaptersController.isValid(null, incidences);

        //    // If the data is valid, show a dialog with the information
        //    if (valid)
        //    {
        //        if (showSuccessFeedback)
        //            mainWindow.showInformationDialog(TC.get("Operation.AdventureConsistencyTitle"), TC.get("Operation.AdventureConsistentReport"));

        //        // If it is not valid, show a dialog with the problems
        //    }
        //    else
        //        new InvalidReportDialog(incidences, TC.get("Operation.AdventureInconsistentReport"));

        //    return valid;
        //}

        //public void checkFileConsistency()
        //{

        //}

        ///**
        // * Shows the adventure data dialog editor.
        // */
        //public void showAdventureDataDialog()
        //{

        //    new AdventureDataDialog();
        //}

        ///**
        // * Shows the LOM data dialog editor.
        // */
        ////public void showLOMDataDialog()
        ////{

        ////    isLomEs = false;
        ////    new LOMDialog(adventureDataControl.getLomController());
        ////}

        ///**
        // * Shows the LOM for SCORM packages data dialog editor.
        // */
        ////public void showLOMSCORMDataDialog()
        ////{

        ////    isLomEs = false;
        ////    new IMSDialog(adventureDataControl.getImsController());
        ////}

        ///**
        // * Shows the LOMES for AGREGA packages data dialog editor.
        // */
        ////public void showLOMESDataDialog()
        ////{

        ////    isLomEs = true;
        ////    new LOMESDialog(adventureDataControl.getLOMESController());
        ////}

        ///**
        // * Shows the GUI style selection dialog.
        // */
        //public void showGUIStylesDialog()
        //{

        //    adventureDataControl.showGUIStylesDialog();
        //}

        //public void changeToolGUIStyleDialog(int optionSelected)
        //{

        //    if (optionSelected != 1)
        //    {

        //        adventureDataControl.setGUIStyleDialog(optionSelected);
        //    }

        //}

        ///**
        // * Asks for confirmation and then deletes all unreferenced assets. Checks
        // * for animations indirectly referenced assets.
        // */
        ////public void deleteUnsuedAssets()
        ////{

        ////    if (!this.showStrictConfirmDialog(TC.get("DeleteUnusedAssets.Title"), TC.get("DeleteUnusedAssets.Warning")))
        ////        return;

        ////    int deletedAssetCount = 0;
        ////    ArrayList<string> assets = new ArrayList<string>();
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_IMAGE))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_BACKGROUND))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_VIDEO))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_AUDIO))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_CURSOR))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_BUTTON))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_ICON))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_STYLED_TEXT))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_ARROW_BOOK))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);

        ////    /*  assets.remove( "gui/cursors/arrow_left.png" );
        ////      assets.remove( "gui/cursors/arrow_right.png" ); */

        ////    for (string temp : assets)
        ////    {
        ////        int references = 0;
        ////        references = countAssetReferences(temp);
        ////        if (references == 0)
        ////        {
        ////            new File(Controller.getInstance().getProjectFolder(), temp).delete();
        ////            deletedAssetCount++;
        ////        }
        ////    }

        ////    assets.clear();
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_ANIMATION_AUDIO))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_ANIMATION_IMAGE))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);
        ////    for (string temp : AssetsController.getAssetsList(AssetsController.CATEGORY_ANIMATION))
        ////        if (!assets.Contains(temp))
        ////            assets.add(temp);

        ////    int i = 0;
        ////    while (i < assets.size())
        ////    {
        ////        string temp = assets.get(i);
        ////        if (countAssetReferences(AssetsController.removeSuffix(temp)) != 0)
        ////        {
        ////            assets.remove(temp);
        ////            if (temp.endsWith("eaa"))
        ////            {
        ////                Animation a = Loader.loadAnimation(AssetsController.getInputStreamCreator(), temp, new EditorImageLoader());
        ////                for (Frame f : a.getFrames())
        ////                {
        ////                    if (f.getUri() != null && assets.Contains(f.getUri()))
        ////                    {
        ////                        for (int j = 0; j < assets.size(); j++)
        ////                        {
        ////                            if (assets.get(j).Equals(f.getUri()))
        ////                            {
        ////                                if (j < i)
        ////                                    i--;
        ////                                assets.remove(j);
        ////                            }
        ////                        }
        ////                    }
        ////                    if (f.getSoundUri() != null && assets.Contains(f.getSoundUri()))
        ////                    {
        ////                        for (int j = 0; j < assets.size(); j++)
        ////                        {
        ////                            if (assets.get(j).Equals(f.getSoundUri()))
        ////                            {
        ////                                if (j < i)
        ////                                    i--;
        ////                                assets.remove(j);
        ////                            }
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////            else {
        ////                int j = 0;
        ////                while (j < assets.size())
        ////                {
        ////                    if (assets.get(j).startsWith(AssetsController.removeSuffix(temp)))
        ////                    {
        ////                        if (j < i)
        ////                            i--;
        ////                        assets.remove(j);
        ////                    }
        ////                    else
        ////                        j++;
        ////                }
        ////            }
        ////        }
        ////        else {
        ////            i++;
        ////        }
        ////    }

        ////    for (string temp2 : assets)
        ////    {
        ////        new File(Controller.getInstance().getProjectFolder(), temp2).delete();
        ////        deletedAssetCount++;
        ////    }

        ////    if (deletedAssetCount != 0)
        ////        mainWindow.showInformationDialog(TC.get("DeleteUnusedAssets.Title"), TC.get("DeleteUnusedAssets.AssetsDeleted", new string[] { string.valueOf(deletedAssetCount) }));
        ////    else
        ////        mainWindow.showInformationDialog(TC.get("DeleteUnusedAssets.Title"), TC.get("DeleteUnusedAssets.NoUnsuedAssetsFound"));
        ////}

        ////public java.io.File selectXMLChapterFile()
        ////{

        ////    JFileChooser chooser = new JFileChooser();
        ////    chooser.setFileFilter(new XMLFileFilter());
        ////    chooser.setMultiSelectionEnabled(false);
        ////    chooser.setCurrentDirectory(new File(getCurrentLoadFolder()));
        ////    int option = chooser.showOpenDialog(mainWindow);
        ////    if (option == JFileChooser.APPROVE_OPTION)
        ////        return chooser.getSelectedFile();
        ////    else
        ////        return null;
        ////}

        /////**
        //// * Shows the flags dialog.
        //// */
        ////public void showEditFlagDialog()
        ////{

        ////    new VarsFlagsDialog(new VarFlagsController(getVarFlagSummary()));
        ////}

        ///**
        //* Sets a new selected chapter with the given index.
        //* 
        //* @param selectedChapter
        //*            Index of the new selected chapter
        //*/
        public void setSelectedChapter(int selectedChapter)
        {

            chaptersController.setSelectedChapterInternal(selectedChapter);
            //mainWindow.reloadData();
        }

        public void updateVarFlagSummary()
        {

            chaptersController.updateVarFlagSummary();
        }

        /**
         * Adds a new chapter to the adventure. This method asks for the title of
         * the chapter to the user, and updates the view of the application if a new
         * chapter was added.
         */
        public void addChapter(string val)
        {
            addTool(new AddChapterTool(chaptersController, val));
        }

        ///**
        // * Imports a new chapter to the adventure. This method open a window to
        // * select the xml chapter.
        // */
        //public void importChapter()
        //{

        //    try
        //    {
        //        addTool(new ImportChapterTool(chaptersController));
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.getStackTrace();
        //        loadingScreen.setVisible(false);
        //        showInformationDialog(TC.get("ImportChapter.UnknowProblems.Title"), TC.get("ImportChapter.UnknowProblems.Message"));
        //    }

        //}

        /**
         * Deletes the selected chapter from the adventure. This method asks the
         * user for confirmation, and updates the view if needed.
         */
        public void deleteChapter()
        {

            addTool(new DeleteChapterTool(chaptersController));
        }

        ///**
        // * Moves the selected chapter to the previous position of the chapter's
        // * list.
        // */
        //public void moveChapterUp()
        //{

        //    addTool(new MoveChapterTool(MoveChapterTool.MODE_UP, chaptersController));
        //}

        ///**
        // * Moves the selected chapter to the next position of the chapter's list.
        // * 
        // */
        //public void moveChapterDown()
        //{

        //    addTool(new MoveChapterTool(MoveChapterTool.MODE_DOWN, chaptersController));
        //}

        //// ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //// Methods to edit and get the adventure general data (title and description)

        ///**
        // * Returns the title of the adventure.
        // * 
        // * @return Adventure's title
        // */
        //public string getAdventureTitle()
        //{

        //    return adventureDataControl.getTitle();
        //}

        //public string getVersionNumber()
        //{
        //    return adventureDataControl.getAdventureData().getVersionNumber();
        //}

        ///**
        // * Returns the description of the adventure.
        // * 
        // * @return Adventure's description
        // */
        //public string getAdventureDescription()
        //{

        //    return adventureDataControl.getDescription();
        //}

        ///**
        // * Returns the LOM controller.
        // * 
        // * @return Adventure LOM controller.
        // * 
        // */
        ////public LOMDataControl getLOMDataControl()
        ////{

        ////    return adventureDataControl.getLomController();
        ////}

        ///**
        // * Sets the new title of the adventure.
        // * 
        // * @param title
        // *            Title of the adventure
        // */
        //public void setAdventureTitle(string title)
        //{

        //    // If the value is different
        //    if (!title.Equals(adventureDataControl.getTitle()))
        //    {
        //        // Set the new title and modify the data
        //        adventureDataControl.setTitle(title);
        //    }
        //}

        ///**
        // * Sets the new description of the adventure.
        // * 
        // * @param description
        // *            Description of the adventure
        // */
        //public void setAdventureDescription(string description)
        //{

        //    // If the value is different
        //    if (!description.Equals(adventureDataControl.getDescription()))
        //    {
        //        // Set the new description and modify the data
        //        adventureDataControl.setDescription(description);
        //    }
        //}

        //public void setKeyboardNavigation(bool enabled)
        //{

        //    if (enabled != adventureDataControl.isKeyboardNavigationEnabled())
        //    {
        //        adventureDataControl.setKeyboardNavigation(enabled);
        //    }
        //}

        //public bool isKeyboardNavigationEnabled()
        //{

        //    return adventureDataControl.isKeyboardNavigationEnabled();
        //}

        //// ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //// Methods that perform specific tasks for the microcontrollers

        /**
         * Returns whether the given identifier is valid or not. If the element
         * identifier is not valid, this method shows an error message to the user.
         * 
         * @param elementId
         *            Element identifier to be checked
         * @return True if the identifier is valid, false otherwise
         */
        public bool isElementIdValid(string elementId)
        {

            return isElementIdValid(elementId, true);
        }

        ///**
        // * Returns whether the given identifier is valid or not. If the element
        // * identifier is not valid, this method shows an error message to the user
        // * if showError is true
        // * 
        // * @param elementId
        // *            Element identifier to be checked
        // * @param showError
        // *            True if the error message must be shown
        // * @return True if the identifier is valid, false otherwise
        // */
        public bool isElementIdValid(string elementId, bool showError)
        {
            bool elementIdValid = false;
            //TODO: implement
            // Check if the identifier has no spaces
            if (!elementId.Contains(" ") && !elementId.Contains("'"))
            {

                //If the identifier doesn't exist already
                if (!getIdentifierSummary().existsId(elementId))
                {

                    //If the identifier is not a reserved identifier
                    if (!elementId.Equals(Player.IDENTIFIER) && !elementId.Equals(TC.get("ConversationLine.PlayerName")))
                    {

                        //If the first character is a letter
                        if (elementId.Length > 0 && char.IsLetter(elementId[0]))
                        {
                            elementIdValid = isCharacterValid(elementId);
                            if (!elementIdValid)
                                mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorCharacter"));
                        }
                        //Show non-letter first character error
                        else if (showError)
                            mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorFirstCharacter"));
                    }

                    //Show invalid identifier error
                    else if (showError)
                        mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorReservedIdentifier", elementId));
                }

                // Show repeated identifier error
                else if (showError)
                    mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorAlreadyUsed"));
            }

            //Show blank spaces error
            else if (showError)
                mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorBlankSpaces"));

            return elementIdValid;
        }

        public bool isCharacterValid(string elementId)
        {

            char chId;
            bool isValid = true;
            int i = 1;
            while (i < elementId.Length && isValid)
            {
                chId = elementId[i];
                if (!char.IsLetterOrDigit(chId) && chId != '-' && chId != '_' && chId != ':' && chId != '.')
                {
                    isValid = false;
                }
                i++;
            }

            return isValid;

        }

        /**
         * This method returns the absolute path of the background image of the
         * given scene.
         * 
         * @param sceneId
         *            Scene id
         * @return Path to the background image, null if it was not found
         */
        public string getSceneImagePath(string sceneId)
        {

            string sceneImagePath = null;

            // Search for the image in the list, comparing the identifiers
            foreach (SceneDataControl scene in getSelectedChapterDataControl().getScenesList().getScenes())
                if (sceneId.Equals(scene.getId()))
                    sceneImagePath = scene.getPreviewBackground();

            return sceneImagePath;
        }

        /**
         * This method returns the trajectory of a scene from its id.
         * 
         * @param sceneId
         *            Scene id
         * @return Trajectory of the scene, null if it was not found
         */
        public Trajectory getSceneTrajectory(string sceneId)
        {

            Trajectory trajectory = null;

            // Search for the image in the list, comparing the identifiers
            foreach (SceneDataControl scene in getSelectedChapterDataControl().getScenesList().getScenes())
                if (sceneId.Equals(scene.getId()) && scene.getTrajectory().hasTrajectory())
                    trajectory = (Trajectory)scene.getTrajectory().getContent();

            return trajectory;
        }

        ///**
        // * This method returns the absolute path of the default image of the player.
        // * 
        // * @return Default image of the player
        // */
        public string getPlayerImagePath()
        {

            if (getSelectedChapterDataControl() != null)
                return getSelectedChapterDataControl().getPlayer().getPreviewImage();
            else
                return null;
        }

        /**
         * Returns the player
         */
        public Player getPlayer()
        {

            return (Player)getSelectedChapterDataControl().getPlayer().getContent();
        }

        ///**
        // * Change the player resources when the mode is changed.
        // * 
        // * @param assetNecessary
        // * 
        // */
        ////public void changePlayerNecessaryResources(bool assetNecessary)
        ////{

        ////    int right, left;
        ////    //TODO no se comprueba el caso en el que el player tenga uno de los dos.... al realizar 2 veces el cambio de 
        ////    // tipo de juego, se setean ambos como necesarios y solo aparece uno...
        ////    for (ResourcesDataControl rdc : getSelectedChapterDataControl().getPlayer().getResources())
        ////    {
        ////        for (int i = 0; i < rdc.getAssetsInformation().length; i++)
        ////        {
        ////            if (rdc.getAssetsInformation()[i].name.Equals("standleft"))
        ////            {
        ////                rdc.getAssetsInformation()[i].assetNecessary = assetNecessary;
        ////                left = i;
        ////            }
        ////            else if (rdc.getAssetsInformation()[i].name.Equals("standright"))
        ////            {
        ////                rdc.getAssetsInformation()[i].assetNecessary = assetNecessary;
        ////                right = i;
        ////            }
        ////        }

        ////    }
        ////}

        /**
         * This method returns the absolute path of the default image of the given
         * element (item or character).
         * 
         * @param elementId
         *            Id of the element
         * @return Default image of the requested element
         */
        public string getElementImagePath(string elementId)
        {

            string elementImage = null;

            // Search for the image in the items, comparing the identifiers
            foreach (ItemDataControl item in getSelectedChapterDataControl().getItemsList().getItems())
                if (elementId.Equals(item.getId()))
                    elementImage = item.getPreviewImage();

            // Search for the image in the characters, comparing the identifiers
            foreach (NPCDataControl npc in getSelectedChapterDataControl().getNPCsList().getNPCs())
                if (elementId.Equals(npc.getId()))
                    elementImage = npc.getPreviewImage();
            // Search for the image in the items, comparing the identifiers
            foreach (AtrezzoDataControl atrezzo in getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList())
                if (elementId.Equals(atrezzo.getId()))
                    elementImage = atrezzo.getPreviewImage();

            return elementImage;
        }

        /**
         * Counts all the references to a given asset in the entire script.
         * 
         * @param assetPath
         *            Path of the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         * @return Number of references to the given asset
         */
        public int countAssetReferences(string assetPath)
        {

            int refs = adventureDataControl.countAssetReferences(assetPath) + chaptersController.countAssetReferences(assetPath);

            // Add references in images and sounds in eaa files
            List<string> assetPaths = new List<string>();
            List<int> assetTypes = new List<int>();
            getAssetReferences(assetPaths, assetTypes);

            for (int i = 0; i < assetPaths.Count; i++)
            {
                if (assetPaths[i].ToLower().EndsWith(".eaa"))
                {
                    refs += countAssetReferencesInEAA(assetPaths[i], assetPath);
                }
            }

            return refs;
        }

        /**
         * ead1.4 - used to check references to images and sounds in eaa files
         */
        private int countAssetReferencesInEAA(string eaaFilePath, string assetPath)
        {
            int refs = 0;
            Animation animation = Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator(), eaaFilePath, new EditorImageLoader());
            foreach (Frame frame in animation.getFrames())
            {
                if (frame != null)
                {
                    if (frame.getUri() != null && frame.getUri().Equals(assetPath))
                    {
                        refs++;
                    }
                    if (frame.getSoundUri() != null && frame.getSoundUri().Equals(assetPath))
                    {
                        refs++;
                    }
                }
            }
            return refs;
        }


        /**
         * Gets a list with all the assets referenced in the chapter along with the
         * types of those assets
         * 
         * @param assetPaths
         * @param assetTypes
         */
        public void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            adventureDataControl.getAssetReferences(assetPaths, assetTypes);
            chaptersController.getAssetReferences(assetPaths, assetTypes);

            // Add references in images and sounds in eaa files
            for (int i = 0; i < assetPaths.Count; i++)
            {
                if (assetPaths[i].ToLower().EndsWith(".eaa"))
                {
                    getAssetReferencesInEAA(assetPaths[i], assetPaths, assetTypes);
                }
            }
        }

        /**
         * ead1.4 - used to check references to images and sounds in eaa files
         */

        private void getAssetReferencesInEAA(string eaaFilePath, List<string> assetPaths, List<int> assetTypes)
        {
            Animation animation = Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator(),
                eaaFilePath, new EditorImageLoader());
            foreach (Frame frame in animation.getFrames())
            {
                if (frame != null)
                {
                    if (frame.getUri() != null && !frame.getUri().Equals("") && !assetPaths.Contains(frame.getUri()))
                    {
                        assetPaths.Add(frame.getUri());
                        assetTypes.Add(AssetsConstants.CATEGORY_ANIMATION_IMAGE);
                    }
                    if (frame.getSoundUri() != null && !frame.getSoundUri().Equals("") &&
                        !assetPaths.Contains(frame.getSoundUri()))
                    {
                        assetPaths.Add(frame.getSoundUri());
                        assetTypes.Add(AssetsConstants.CATEGORY_ANIMATION_AUDIO);
                    }
                }
            }
        }

        ///**
        // * Deletes a given asset from the script, removing all occurrences.
        // * 
        // * @param assetPath
        // *            Path of the asset (relative to the ZIP), without suffix in
        // *            case of an animation or set of slides
        // */
        //public void deleteAssetReferences(string assetPath)
        //{

        //    adventureDataControl.deleteAssetReferences(assetPath);
        //    chaptersController.deleteAssetReferences(assetPath);

        //    // Add references in images and sounds in eaa files
        //    List<string> assetPaths = new ArrayList<string>();
        //    List<Integer> assetTypes = new ArrayList<Integer>();
        //    getAssetReferences(assetPaths, assetTypes);

        //    for (int i = 0; i < assetPaths.size(); i++)
        //    {
        //        if (assetPaths.get(i).toLowerCase().endsWith(".eaa"))
        //        {
        //            deleteAssetReferencesInEAA(assetPaths.get(i), assetPath);
        //        }
        //    }

        //}

        ///**
        // * ead1.4 - used to check references to images and sounds in eaa files
        // */
        //private void deleteAssetReferencesInEAA(string eaaFilePath, string assetPath)
        //{
        //    Animation animation = Loader.loadAnimation(AssetsController.getInputStreamCreator(), eaaFilePath, new EditorImageLoader());
        //    bool changed = false;
        //    for (int i = 0; i < animation.getFrames().size(); i++)
        //    {
        //        Frame frame = animation.getFrame(i);
        //        if (frame != null)
        //        {
        //            if (frame.getSoundUri() != null && frame.getSoundUri().Equals(assetPath))
        //            {
        //                frame.setSoundUri(null);
        //                changed = true;
        //            }
        //            if (frame.getUri() != null && frame.getUri().Equals(assetPath))
        //            {
        //                frame.setUri(null);
        //                changed = true;
        //            }
        //        }
        //    }
        //    if (changed)
        //    {
        //        AnimationWriter.writeAnimation(eaaFilePath, animation);
        //    }
        //}


        ///**
        // * Counts all the references to a given identifier in the entire script.
        // * 
        // * @param id
        // *            Identifier to which the references must be found
        // * @return Number of references to the given identifier
        // */
        public int countIdentifierReferences(string id)
        {

            return getSelectedChapterDataControl().countIdentifierReferences(id);
        }

        /**
         * Deletes a given identifier from the script, removing all occurrences.
         * 
         * @param id
         *            Identifier to be deleted
         */

        public void deleteIdentifierReferences(string id)
        {

            chaptersController.deleteIdentifierReferences(id);
        }

        ///**
        // * Replaces a given identifier with another one, in all the occurrences in
        // * the script.
        // * 
        // * @param oldId
        // *            Old identifier to be replaced
        // * @param newId
        // *            New identifier to replace the old one
        // */
        public void replaceIdentifierReferences(string oldId, string newId)
        {

            getSelectedChapterDataControl().replaceIdentifierReferences(oldId, newId);
        }

        //// ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //// Methods linked with the GUI

        ///**
        // * Updates the chapter menu with the new names of the chapters.
        // */
        public void updateChapterMenu()
        {

            //mainWindow.updateChapterMenu();
        }

        ///**
        // * Updates the tree of the main window.
        // */
        public void updateStructure()
        {

            // mainWindow.updateStructure();
        }

        ///**
        // * Reloads the panel of the main window currently being used.
        // */
        public void reloadPanel()
        {

            //mainWindow.reloadPanel();
        }

        public void updatePanel()
        {

            // mainWindow.updatePanel();
        }

        ///**
        // * Reloads the panel of the main window currently being used.
        // */
        //public void reloadData()
        //{

        //    //mainWindow.reloadData();
        //}

        ///**
        // * Returns the last window opened by the application.
        // * 
        // * @return Last window opened
        // */
        ////public Window peekWindow()
        ////{

        ////    return mainWindow.peekWindow();
        ////}

        ///**
        // * Pushes a new window in the windows stack.
        // * 
        // * @param window
        // *            Window to push
        // */
        ////public void pushWindow(Window window)
        ////{

        ////    mainWindow.pushWindow(window);
        ////}

        ///**
        // * Pops the last window pushed into the stack.
        // */
        ////public void popWindow()
        ////{

        ////    mainWindow.popWindow();
        ////}

        ///**
        // * Shows a dialog showing information and with the button "Ok"
        // * 
        // * @param title
        // *            Title of the dialog
        // * @param message
        // *            Message of the dialog
        // */
        //public void showInformationDialog(string title, string message)
        //{

        //    //mainWindow.showInformationDialog(title, message);
        //}

        ///**
        // * Shows a load dialog to select multiple files.
        // * 
        // * @param filter
        // *            File filter for the dialog
        // * @return Full path of the selected files, null if no files were selected
        // */
        ////public string[] showMultipleSelectionLoadDialog(FileFilter filter)
        ////{

        ////    return mainWindow.showMultipleSelectionLoadDialog(currentZipPath, filter);
        ////}

        /**
         * Shows a dialog with the options "Yes" and "No", with the given title and
         * text.
         * 
         * @param title
         *            Title of the dialog
         * @param message
         *            Message of the dialog
         * @return True if the "Yes" button was pressed, false otherwise
         */
        public bool showStrictConfirmDialog(string title, string message)
        {
            //TODO: implementation
            return true;
            // return mainWindow.showStrictConfirmDialog(title, message);
        }

        ///**
        // * Shows a dialog with the given set of options.
        // * 
        // * @param title
        // *            Title of the dialog
        // * @param message
        // *            Message of the dialog
        // * @param options
        // *            Array of strings containing the options of the dialog
        // * @return The index of the option selected, JOptionPane.CLOSED_OPTION if
        // *         the dialog was closed.
        // */
        ////public int showOptionDialog(string title, string message, string[] options)
        ////{

        ////    return mainWindow.showOptionDialog(title, message, options);
        ////}

        ///**
        // * Uses the GUI to show an input dialog.
        // * 
        // * @param title
        // *            Title of the dialog
        // * @param message
        // *            Message of the dialog
        // * @param defaultValue
        // *            Default value of the dialog
        // * @return string typed in the dialog, null if the cancel button was pressed
        // */
        public string showInputDialog(string title, string message, string defaultValue)
        {
            //TODO: Implementation
            string s;
            System.Random random = new System.Random((int)DateTime.Now.Ticks);
            s = "defaultValue" + random.Next(100);

            return s;
        }

        public string showInputDialog(string title, string message)
        {

            //TODO: Implementation
            return "";
            // return mainWindow.showInputDialog(title, message);
        }

        /**
         * Uses the GUI to show an input dialog.
         * 
         * @param title
         *            Title of the dialog
         * @param message
         *            Message of the dialog
         * @param selectionValues
         *            Possible selection values of the dialog
         * @return Option selected in the dialog, null if the cancel button was
         *         pressed
         */

        public string showInputDialog(string title, string message, System.Object[] selectionValues)
        {
            //TODO: Implementation
            return "";

            //return mainWindow.showInputDialog(title, message, selectionValues);
        }

        ///**
        // * Uses the GUI to show an error dialog.
        // * 
        // * @param title
        // *            Title of the dialog
        // * @param message
        // *            Message of the dialog
        // */
        public void showErrorDialog(string title, string message)
        {

            Debug.LogError(title + "\n" + message);
        }

        ////public void showCustomizeGUIDialog()
        ////{

        ////    new CustomizeGUIDialog(this.adventureDataControl);
        ////}

        //public bool isFolderLoaded()
        //{

        //    return chaptersController.isAnyChapterSelected();
        //}

        //public string getEditorMinVersion()
        //{

        //    return "1.3";
        //}

        //public string getEditorVersion()
        //{

        //    return "1.3";
        //}

        //public void updateLOMLanguage()
        //{

        //    this.adventureDataControl.getLomController().updateLanguage();

        //}

        //public void updateIMSLanguage()
        //{

        //    this.adventureDataControl.getImsController().updateLanguage();
        //}

        //public void showAboutDialog()
        //{

        //    try
        //    {
        //        JDialog dialog = new JPositionedDialog(Controller.getInstance().peekWindow(), TC.get("About"), Dialog.ModalityType.TOOLKIT_MODAL);
        //        dialog.getContentPane().setLayout(new BorderLayout());

        //        JPanel panel = new JPanel();
        //        panel.setLayout(new BorderLayout());

        //        File file = new File(ConfigData.getAboutFile());
        //        if (file.exists())
        //        {
        //            JEditorPane pane = new JEditorPane();
        //            pane.setPage(file.toURI().toURL());
        //            pane.setEditable(false);
        //            panel.add(pane, BorderLayout.CENTER);

        //        }

        //        dialog.getContentPane().add(panel, BorderLayout.CENTER);

        //        dialog.setSize(275, 560);
        //        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        //        dialog.setLocation((screenSize.width - dialog.getWidth()) / 2, (screenSize.height - dialog.getHeight()) / 2);
        //        dialog.setVisible(true);

        //    }
        //    catch (IOException e)
        //    {
        //        ReportDialog.GenerateErrorReport(e, true, "UNKNOWERROR");
        //    }
        //}

        //public AssessmentProfilesDataControl getAssessmentController()
        //{

        //    return this.chaptersController.getSelectedChapterDataControl().getAssessmentProfilesDataControl();
        //}

        //public AdaptationProfilesDataControl getAdaptationController()
        //{

        //    return this.chaptersController.getSelectedChapterDataControl().getAdaptationProfilesDataControl();
        //}

        //public bool isCommentaries()
        //{

        //    return this.adventureDataControl.isCommentaries();
        //}

        //public void setCommentaries(bool b)
        //{

        //    this.adventureDataControl.setCommentaries(b);

        //}

        //public bool isKeepShowing()
        //{

        //    return this.adventureDataControl.isKeepShowing();
        //}

        //public void setKeepShowing(bool b)
        //{

        //    this.adventureDataControl.setKeepShowing(b);

        //}

        ///**
        // * Returns an int value representing the current language used to display
        // * the editor
        // * 
        // * @return
        // */
        public string getLanguage()
        {

            return this.languageFile;
        }

        /**
         * Get the default lenguage
         * 
         * @return name of default language in standard internationalization
         */

        public string getDefaultLanguage()
        {

            return ReleaseFolders.LANGUAGE_DEFAULT;
        }

        /**
         * Sets the current language of the editor. Accepted values are
         * {@value #LANGUAGE_ENGLISH} & {@value #LANGUAGE_ENGLISH}. This method
         * automatically updates the about, language strings, and loading image
         * parameters.
         * 
         * The method will reload the main window always
         * 
         * @param language
         */

        public void setLanguage(string language)
        {

            setLanguage(language, true);
        }

        /**
         * Sets the current language of the editor. Accepted values are
         * {@value #LANGUAGE_ENGLISH} & {@value #LANGUAGE_ENGLISH}. This method
         * automatically updates the about, language strings, and loading image
         * parameters.
         * 
         * The method will reload the main window if reloadData is true
         * 
         * @param language
         */

        public void setLanguage(string language, bool reloadData)
        {

            // image loading route
            string dirImageLoading = ReleaseFolders.IMAGE_LOADING_DIR + "/" + language + "/Editor2D-Loading.png";
            // if there isn't file, load the default file
            FileInfo fichero = new FileInfo(dirImageLoading);
            if (!fichero.Exists)
                dirImageLoading = ReleaseFolders.IMAGE_LOADING_DIR + "/" + getDefaultLanguage() + "/Editor2D-Loading.png";

            //about file route
            string dirAboutFile = ReleaseFolders.LANGUAGE_DIR_EDITOR + "/" + ReleaseFolders.getAboutFilePath(language);
            FileInfo fichero2 = new FileInfo(dirAboutFile);
            if (!fichero2.Exists)
                dirAboutFile = ReleaseFolders.LANGUAGE_DIR_EDITOR + "/" + ReleaseFolders.getDefaultAboutFilePath();

            ConfigData.setLanguangeFile(ReleaseFolders.getLanguageFilePath(language), dirAboutFile, dirImageLoading);

            languageFile = language;
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, languageFile));
            TC.appendstrings(ReleaseFolders.getLanguageFilePath4Editor(false, languageFile));
            //loadingScreen.setImage(getLoadingImage());
            //if (reloadData)
            //    mainWindow.reloadData();
        }

        //public string getLoadingImage()
        //{

        //    return ConfigData.getLoadingImage();
        //}

        //public void showGraphicConfigDialog()
        //{

        //    // Show the dialog
        //    //   GraphicConfigDialog guiStylesDialog = new GraphicConfigDialog( adventureDataControl.getGraphicConfig( ) );
        //    new GraphicConfigDialog(adventureDataControl.getGraphicConfig());

        //    // If the new GUI style is different from the current, and valid, change the value
        //    /*    int optionSelected = guiStylesDialog.getOptionSelected( );
        //        if( optionSelected != -1 && this.adventureDataControl.getGraphicConfig( ) != optionSelected ) {
        //            adventureDataControl.setGraphicConfig( optionSelected );
        //        }*/
        //}

        //public void changeToolGraphicConfig(int optionSelected)
        //{

        //    if (optionSelected != -1 && this.adventureDataControl.getGraphicConfig() != optionSelected)
        //    {
        //        //  this.grafhicDialog.cambiarCheckBox( );
        //        adventureDataControl.setGraphicConfig(optionSelected);
        //    }

        //}

        //// METHODS TO MANAGE UNDO/REDO

        public bool addTool(Tool tool)
        {

            bool added = chaptersController.addTool(tool);
            //tsd.update();
            return added;
        }

        public void undoTool()
        {

            chaptersController.undoTool();
            //tsd.update();
        }

        public void redoTool()
        {

            chaptersController.redoTool();
            //tsd.update();
        }

        //public void pushLocalToolManager()
        //{

        //    chaptersController.pushLocalToolManager();
        //}

        //public void popLocalToolManager()
        //{

        //    chaptersController.popLocalToolManager();
        //}

        //public void search()
        //{

        //    new SearchDialog();
        //}

        //public bool getAutoSaveEnabled()
        //{

        //    if (ProjectConfigData.existsKey("autosave"))
        //    {
        //        string temp = ProjectConfigData.getProperty("autosave");
        //        if (temp.Equals("yes"))
        //        {
        //            return true;
        //        }
        //        else {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public void setAutoSaveEnabled(bool selected)
        //{

        //    if (selected != getAutoSaveEnabled())
        //    {
        //        ProjectConfigData.setProperty("autosave", (selected ? "yes" : "no"));
        //        startAutoSave(15);
        //    }
        //}

        ///**
        // * @return the isLomEs
        // */
        //public bool isLomEs()
        //{

        //    return isLomEs;
        //}

        //public int getGUIConfigConfiguration()
        //{

        //    return this.adventureDataControl.getGraphicConfig();
        //}

        //public string getDefaultExitCursorPath()
        //{

        //    string temp = this.adventureDataControl.getCursorPath("exit");
        //    if (temp != null && temp.length() > 0)
        //        return temp;
        //    else
        //        return "gui/cursors/exit.png";
        //}

        public AdvancedFeaturesDataControl getAdvancedFeaturesController()
        {

            return this.chaptersController.getSelectedChapterDataControl().getAdvancedFeaturesController();
        }

        //public static Color generateColor(int i)
        //{

        //    int r = (i * 180) % 256;
        //    int g = ((i + 4) * 130) % 256;
        //    int b = ((i + 2) * 155) % 256;

        //    if (r > 250 && g > 250 && b > 250)
        //    {
        //        r = 0;
        //        g = 0;
        //        b = 0;
        //    }

        //    return new Color(r, g, b);
        //}

        //private const Runnable gc = new Runnable()
        //{

        //        public void run()
        //{

        //    System.gc();
        //}
        //    };

        //    /**
        //     * Public method to perform garbage collection on a different thread.
        //     */
        //    public static void gc()
        //{

        //    new Thread(gc).start();
        //}

        //public static java.io.File createTempDirectory() throws IOException
        //{

        //    final java.io.File temp;

        //    temp = java.io.File.createTempFile( "temp", Long.tostring( System.nanoTime( ) ) );

        //        if( !( temp.delete( ) ) )
        //            throw new IOException( "Could not delete temp file: " + temp.getAbsolutePath( ) );

        //        if( !( temp.mkdir( ) ) )
        //            throw new IOException( "Could not create temp directory: " + temp.getAbsolutePath( ) );

        //        return ( temp );
        //    }

        //    public DefaultClickAction getDefaultCursorAction()
        //{

        //    return this.adventureDataControl.getDefaultClickAction();
        //}

        //public void setDefaultCursorAction(DefaultClickAction defaultClickAction)
        //{

        //    this.adventureDataControl.setDefaultClickAction(defaultClickAction);
        //}

        //public Perspective getPerspective()
        //{

        //    return this.adventureDataControl.getPerspective();
        //}

        //public void setPerspective(Perspective perspective)
        //{

        //    this.adventureDataControl.setPerspective(perspective);
        //}

        //public DragBehaviour getDragBehaviour()
        //{

        //    return this.adventureDataControl.getDragBehaviour();
        //}

        //public void setDragBehaviour(DragBehaviour dragBehaviour)
        //{

        //    this.adventureDataControl.setDragBehaviour(dragBehaviour);
        //}

        //public int getMainWindowWidth()
        //{
        //    return mainWindow.getWidth();
        //}

        //public int getMainWindowHeight()
        //{
        //    return mainWindow.getHeight();
        //}

        //public int getMainWindowX()
        //{
        //    return mainWindow.getX();
        //}

        //public int getMainWindowY()
        //{
        //    return mainWindow.getY();
        //}

        ///**
        // * Change all animation old formats (name_01) for new formats (.eaa)
        // */
        //public void changeAllAnimationFormats()
        //{

        //    //Get all cutsecene data controls 
        //    List<DataControlWithResources> dataControlList = new ArrayList<DataControlWithResources>();
        //    dataControlList.addAll(chaptersController.getSelectedChapterDataControl().getCutscenesList().getAllCutsceneDataControls());
        //    // change formats seting the option "slides animation" in the new .eaa created animations
        //    changeFormats(true, dataControlList);
        //    dataControlList.clear();

        //    //Get all NPC and Player data controls 
        //    dataControlList.addAll(chaptersController.getSelectedChapterDataControl().getNPCsList().getAllNPCDataControls());
        //    dataControlList.add(chaptersController.getSelectedChapterDataControl().getPlayer());
        //    // change formats seting the option "slides animation" in the new .eaa created animations
        //    changeFormats(false, dataControlList);

        //    loadingScreen.setMessage(TC.get("Operation.ExportProject.AsLO"));
        //    loadingScreen.setVisible(true);

        //} // end changeAllAnimationFormats

        //private void changeFormats(bool isCutScene, List<DataControlWithResources> dataControlList)
        //{

        //    HashMap<string, string> cache = new HashMap<string, string>();

        //    // Take the project folder to check if the .eaa animation has been previously created
        //    File projectFolder = new File(Controller.getInstance().getProjectFolder());
        //    for (DataControlWithResources dc : dataControlList)
        //    {
        //        // iterate the whole list of resourceDataControls looking for all animations
        //        List<ResourcesDataControl> resourcesDataControl = dc.getResources();
        //        for (ResourcesDataControl rdc : resourcesDataControl)
        //        {
        //            for (int i = 0; i < rdc.getAssetCount(); i++)
        //            {
        //                if (rdc.getAssetCategory(i) == AssetsConstants.CATEGORY_ANIMATION)
        //                {
        //                    string assetPath = rdc.getAssetPath(i);
        //                    if ((assetPath == null || assetPath.Equals("")) /*&&  !assetPath.Equals( SpecialAssetPaths.ASSET_EMPTY_ANIMATION )*/)
        //                    {
        //                        assetPath = SpecialAssetPaths.ASSET_EMPTY_ANIMATION + ".eaa";
        //                    }

        //                    if (!assetPath.toLowerCase().endsWith(".eaa"))
        //                    {
        //                        string originalAssetPath = assetPath;
        //                        string[] temp = assetPath.split("/");
        //                        string animationName = temp[temp.length - 1];
        //                        File animationFile = null;
        //                        if (cache.ContainsKey(originalAssetPath))
        //                        {
        //                            assetPath = cache.get(originalAssetPath);
        //                            animationFile = new File(projectFolder, originalAssetPath + ".eaa");
        //                        }
        //                        else {
        //                            // string path;
        //                            animationFile = new File(projectFolder, assetPath + ".eaa");
        //                            //In win there are no differences between files with the same name but with different
        //                            // case characteres. the first if check if there are exactly the same name file.
        //                            //if (! animationFile.exists( )){
        //                            Random r = new Random();
        //                            while (animationFile.exists())
        //                            {
        //                                assetPath += Integer.tostring(r.nextInt(10));
        //                                animationFile = new File(projectFolder, assetPath + ".eaa");
        //                            }
        //                            cache.put(originalAssetPath, assetPath);
        //                        }
        //                        /*if(! animationFile.existsSameFile( )) {
        //                            //For win, if there are a file with the same name but different case characters, delete it
        //                            File deleteFile = animationFile.existsIgnoreCase();
        //                            if (deleteFile!=null)
        //                                deleteFile.delete( );*/

        //                        Animation animation = new Animation(animationName, new EditorImageLoader());
        //                        // set the animation to cutsecene mode when was necessary
        //                        animation.setSlides(isCutScene);
        //                        animation.setDocumentation(rdc.getAssetDescription(i));
        //                        // add the images of the old animation
        //                        ResourcesDataControl.framesFromImages(animation, originalAssetPath, true);
        //                        AnimationWriter.writeAnimation(animationFile.getAbsolutePath(), animation);
        //                        // CAUTION!! adding resources without using tool
        //                        if (rdc.getAssetPath(rdc.getAssetName(i)) != null)
        //                        {
        //                            rdc.deleteAssetReferences(rdc.getAssetName(i));
        //                        }
        //                        rdc.addAsset(rdc.getAssetName(i), assetPath + ".eaa");
        //                        /*}
        //                        else
        //                            rdc.changeAssetPath( i, assetPath + ".eaa" );*/
        //                    }
        //                    else {
        //                        // if the eaa animation for this old animation was previously created, change only the path (without using Tools, cause this operation
        //                        // ask for user confirmation if animation path previously exist)
        //                        rdc.changeAssetPath(i, assetPath);
        //                    }

        //                }
        //            }
        //        }// end resources  for
        //    }// end main for
        //}

        ///**
        // * Used to determine location of dialogs and frames on the screen.
        // * @return
        // */
        //public bool isMainWindowBuilt()
        //{
        //    return mainWindow != null;
        //}

        ///**
        // * Builds the settings to run the game. It takes debug options from ConfigData and also
        // * calculates the graphic device where the game must be run, and the preferredBounds, if any.
        // * @param debug Running mode. False if normal, True if debug
        // * @return
        // */
        //private RunAndDebugSettings buildRunAndDebugSettings(bool debug)
        //{
        //    RunAndDebugSettings settings = null;

        //    //Calculate bounding rectangle, if defined
        //    Rectangle prefBounds = null;
        //    if (debug && ConfigData.getDebugWindowHeight() != Integer.MAX_VALUE &&
        //                ConfigData.getDebugWindowWidth() != Integer.MAX_VALUE &&
        //                ConfigData.getDebugWindowX() != Integer.MAX_VALUE &&
        //                ConfigData.getDebugWindowY() != Integer.MAX_VALUE)
        //    {
        //        prefBounds = new Rectangle(ConfigData.getDebugWindowX(), ConfigData.getDebugWindowY(),
        //                ConfigData.getDebugWindowWidth(), ConfigData.getDebugWindowHeight());
        //    }
        //    else if (!debug && ConfigData.getEngineWindowHeight() != Integer.MAX_VALUE &&
        //              ConfigData.getEngineWindowWidth() != Integer.MAX_VALUE &&
        //              ConfigData.getEngineWindowX() != Integer.MAX_VALUE &&
        //              ConfigData.getEngineWindowY() != Integer.MAX_VALUE)
        //    {
        //        prefBounds = new Rectangle(ConfigData.getEngineWindowX(), ConfigData.getEngineWindowY(),
        //                ConfigData.getEngineWindowWidth(), ConfigData.getEngineWindowHeight());
        //    };

        //    // Calculate GraphicsDevice
        //    GraphicsEnvironment environment = GraphicsEnvironment.getLocalGraphicsEnvironment();
        //    GraphicsDevice device = null;
        //    if (prefBounds != null)
        //    {
        //        // Try to determine which device these bounds belong to
        //        // If there are more than one device, pick one that is not the "default", as this one may be occupied with Editor's main window
        //        device = MultiscreenTools.getDeviceContainer(prefBounds, true);
        //    }

        //    // IF device could not be determined using preferred Bounds, then use other device that is not occupied by mainWindow.
        //    // Namely, the device selected is the one which interesction with the main window bounds has minimum area
        //    if (device == null)
        //    {
        //        Rectangle mainWindowBounds = mainWindow.getBounds();
        //        device = MultiscreenTools.getDeviceWithMinimumIntersection(mainWindowBounds);
        //    }

        //    // If device could not be calculated, then use default one
        //    if (device == null)
        //    {
        //        device = environment.getDefaultScreenDevice();
        //    }

        //    // Finally, make the object
        //    if (debug)
        //    {
        //        DebugSettings dSettings = ConfigData.getUserDefinedDebugSettings();
        //        settings = new RunAndDebugSettings(dSettings.isPaintGrid(), dSettings.isPaintHotSpots(), dSettings.isPaintBoundingAreas(),
        //                device, prefBounds, new GameWindowBoundsListenerImpl(debug));
        //    }
        //    else {
        //        settings = new RunAndDebugSettings(device, prefBounds, new GameWindowBoundsListenerImpl(debug));
        //    }

        //    return settings;
        //}


        //private static class GameWindowBoundsListenerImpl implements GameWindowBoundsListener
        //{

        //        private bool debug;
        //private Rectangle initialBounds;

        //public GameWindowBoundsListenerImpl(bool debug)
        //{
        //    this.debug = debug;
        //}

        //public void componentResized(ComponentEvent e)
        //{
        //    update(e.getComponent().getBounds());
        //}

        //public void componentMoved(ComponentEvent e)
        //{
        //    update(e.getComponent().getBounds());
        //}

        //public void componentShown(ComponentEvent e)
        //{
        //}

        //public void componentHidden(ComponentEvent e)
        //{
        //}

        //private void update(Rectangle newBounds)
        //{
        //    if (initialBounds != null && newBounds.Equals(initialBounds))
        //        return;
        //    if (!debug)
        //    {
        //        ConfigData.setEngineWindowX(newBounds.x);
        //        ConfigData.setEngineWindowY(newBounds.y);
        //        ConfigData.setEngineWindowWidth(newBounds.width);
        //        ConfigData.setEngineWindowHeight(newBounds.height);
        //    }
        //    else {
        //        ConfigData.setDebugWindowX(newBounds.x);
        //        ConfigData.setDebugWindowY(newBounds.y);
        //        ConfigData.setDebugWindowWidth(newBounds.width);
        //        ConfigData.setDebugWindowHeight(newBounds.height);
        //    }
        //}

        //public void setInitialBounds(Rectangle bounds)
        //{
        //    this.initialBounds = bounds;
        //}

        //    }

        //    public bool isCutscene(string cutsceneID)
        //{

        //    return this.chaptersController.getChapters().
        //        get(this.getSelectedChapter()).getCutscenesList().existsCutscene(cutsceneID);



        //}

        public void RefreshView()
        {
            EditorWindowBase.RefreshChapter();
        }
    }
}
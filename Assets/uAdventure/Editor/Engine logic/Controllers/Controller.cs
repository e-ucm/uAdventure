using System; 
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using uAdventure.Core;

using Animation = uAdventure.Core.Animation;
using System.Text.RegularExpressions;
using System.Globalization;
using uAdventure.Runner;
using UnityEditor.Callbacks;
using System.Linq;
using System.Xml.Serialization;
using IMS.CP.v1p2;
using IMS.MD.v1p2;
using System.Xml;
using uAdventure.Core.Metadata;
using SimvaPlugin;
using Simva.Api;
using UnityFx.Async.Promises;
using UnityEngine.Networking;
using System.IO.Compression;
using Simva;
using UniRx;

namespace uAdventure.Editor
{
    /**
     * This class is the main controller of the application. It holds the main
     * operations and data to control the editor.
     */

    [InitializeOnLoad]
    public class Controller
    {
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
         * Identifiers for differents fast exporting types
         */
        public const int EXPORT_WINDOWS = 0;

        public const int EXPORT_MACOSX = 1;

        public const int EXPORT_LINUX = 2;

        public const int EXPORT_STANDALONE = 3;

        public const int EXPORT_ANDROID = 4;

        public const int EXPORT_IOS = 5;

        public const int EXPORT_WEBGL = 6;

        public const int EXPORT_MOBILE = 7;

        public const int EXPORT_ALL = 8;

        private const string UADVENTURE_RESOURCE = "res_uAdventure";

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
         * The name of the file that is being currently edited. Used only to display
         * info.
         */
        private string currentZipName;

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

        private static ChapterListDataControl chaptersController = new ChapterListDataControl();

        static Controller()
        {
            chaptersController = new ChapterListDataControl();
            if (!File.Exists(ReleaseFolders.configFileEditorRelativePath()))
            {
                var protoConfig = Path.Combine(AssetsController.EditorResourcesPath + AssetsController.EADVETURE_CONTENT_FOLDER, ReleaseFolders.CONFIG_FILE_PATH_EDITOR);
                File.Copy(protoConfig, ReleaseFolders.configFileEditorRelativePath());
                UnityEditor.AssetDatabase.ImportAsset(ReleaseFolders.configFileEditorRelativePath());
            }

            ConfigData.LoadFromXML(ReleaseFolders.configFileEditorRelativePath());
            if (string.IsNullOrEmpty(ConfigData.GetExtraProperties().GetProperty("shownWelcome")))
            {
                OpenWelcomeWindow();
                ConfigData.GetExtraProperties().SetProperty("shownWelcome", "yes");
                ConfigData.StoreToXML();
            }
        }

        // ABSOLUTE?
        public string getCurrentLoadFolder()
        {
            return ReleaseFolders.PROJECTS_FOLDER;
        }

        /**
         * Returns the instance of the controller.
         * 
         * @return The instance of the controller
         */

        public static Controller Instance
        {
            get
            {
                if (controllerInstance == null)
                {
                    controllerInstance = new Controller();
                    controllerInstance.Init();
                }

                return controllerInstance;
            }
        }

        public class Startup
        {
            static Startup()
            {
            }
        }


        public static void ResetInstance()
        {
            controllerInstance = null;
            try
            {
                EditorApplication.playModeStateChanged -= controllerInstance.OnPlay;
            }
            catch
            {
                Debug.Log("Failed to unsubscribe to onPlay");
            }

            GameRources.GetInstance().Reset();
        }

        public int PlayerMode
        {
            get
            {
                return adventureDataControl.getPlayerMode();
            }
        }

        private bool CheckLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Layer for Not Targetable Elements
            SerializedProperty layers = tagManager.FindProperty("layers");
            string layerName = "UINotTargetable";
            int layerPos = 8;
            if (layers == null)
            {
                Debug.LogWarning("uAdventure was not able to check layers configuation, please make sure there is a "+ layerName + " layer" +
                    " and is properly configured at layer " + layerPos + " for elements that cannot be selected as targets.");
            }
            else
            {
                bool hasUINotTargetableLayer = false;
                int wronglyFoundAt = -1;
                var layer = layers.GetArrayElementAtIndex(layerPos);
                if (layer.stringValue != layerName)
                {
                    if (!string.IsNullOrEmpty(layer.stringValue))
                    {
                        Debug.LogWarning("uAdventure requires layer " + layerPos + " for not targetable elements. Please free the layer and restart uAdventure.");
                    }
                    else
                    {
                        for (int i = 0; i < layers.arraySize; i++)
                        {
                            if (i == layerPos)
                                continue;

                            if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                            {
                                wronglyFoundAt = i;
                                break;
                            }
                        }
                    }
                } else
                    hasUINotTargetableLayer = true;

                if(wronglyFoundAt != -1 && EditorUtility.DisplayDialog("Layer wrongly located", "uAdventure requires " + layerName + " at layer " + layerPos +
                    ". Do you want it to move it to position 9 automatically?", "Yes", "No"))
                {
                    layer.stringValue = layerName;
                    layers.GetArrayElementAtIndex(wronglyFoundAt).stringValue = string.Empty;
                }

                if (!hasUINotTargetableLayer && EditorUtility.DisplayDialog("Layer not found", "uAdventure requires layer " + layerPos +
                    " for not targetable elements. Do you want to try to configure it automatically?", "Yes", "No"))
                {
                    layer.stringValue = layerName;
                }
            }

            // Sorting Layer For Buttons
            string sortingLayerName = "Buttons";
            int buttonsLayerId = 115252387;
            SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");
            if(sortingLayers == null)
            {
                Debug.LogWarning("uAdventure was not able to check sorting layers configuation, please make sure there is a " + sortingLayerName + 
                    " sorting layer and is properly configured in in-game buttons to guarantee they're always on top of the UI");
            }
            else
            {
                bool hasButtonsSortingLayer = false;
                for (int i = 0; i < sortingLayers.arraySize; i++)
                {
                    if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID").intValue == buttonsLayerId)
                    {
                        hasButtonsSortingLayer = true;
                        break;
                    }
                }

                if (!hasButtonsSortingLayer && EditorUtility.DisplayDialog("Sorting layer not found", "uAdventure requires a sorting layer to " +
                    "correctly display interaction buttons. Do you want to try to create it automatically?", "Yes", "No"))
                {
                    sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
                    var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
                    newLayer.FindPropertyRelative("name").stringValue = "Buttons";
                    newLayer.FindPropertyRelative("uniqueID").intValue = buttonsLayerId;
                }
            }

            tagManager.ApplyModifiedPropertiesWithoutUndo();

            return true;
        }

        /**
        * Initializing function.
        */
        public void Init(string loadProjectPath = null)
        {
            HasError = false;
            Error = "";

            Debug.Log("Controller init"); 
            ConfigData.LoadFromXML(ReleaseFolders.configFileEditorRelativePath());

            // On play callback
            EditorApplication.playModeStateChanged += OnPlay;

            CheckLayers();
            // Load the configuration
            //ProjectConfigData.init();
            //SCORMConfigData.init();


            // Create necessary folders if no created before
            DirectoryInfo projectsFolder = new DirectoryInfo(ReleaseFolders.PROJECTS_FOLDER);
            if (!projectsFolder.Exists)
            {
                projectsFolder.Create();
            }
            DirectoryInfo tempFolder = new DirectoryInfo(ReleaseFolders.WEB_TEMP_FOLDER);
            if (!tempFolder.Exists)
            {
                tempFolder.Create();
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
            currentZipName = null;

            dataModified_F = false;

            Initialized = true;

            var currentGamePath = "Assets/uAdventure/Resources/CurrentGame";
            var currentGameDir = new DirectoryInfo(currentGamePath);
            var currentGameProjectFile = new FileInfo(currentGameDir + ".eap");
            var projectFile = loadProjectPath != null ? new FileInfo(loadProjectPath) : null;

            if (projectFile == null || projectFile.FullName == currentGameProjectFile.FullName || projectFile.FullName == currentGameDir.FullName)
            {
                if (!currentGameDir.Exists || currentGameDir.GetFiles().Length == 0)
                {
                    Debug.Log("No current game found, creating a 1st person view game...");
                    NewAdventure(DescriptorData.MODE_PLAYER_1STPERSON);
                }
                else
                {
                    Loaded = LoadFile();
                }
            }
            else if (projectFile.Exists)
            {
                if (projectFile.FullName.ToLower().EndsWith(".eap"))
                {
                    string absolutePath = projectFile.FullName;
                    Loaded = LoadFile(absolutePath.Substring(0, absolutePath.Length - 4));
                }
                else
                {
                    Loaded = LoadFile(projectFile.FullName);
                }
            }
        }

        public bool Initialized { get; private set; }

        public bool Loaded { get; private set; }

        /**
         * Returns the complete path to the currently open Project.
         * 
         * @return The complete path to the ZIP file, null if none is open
         */

        public string ProjectFolder
        {
            get
            {
                return currentZipFile;
            }
        }

        /**
         * Returns the File object representing the currently open Project.
         * 
         * @return The complete path to the ZIP file, null if none is open
         */

        public FileInfo ProjectFolderFile
        {
            get
            {
                return new FileInfo(currentZipFile);
            }
        }
        public AdventureDataControl AdventureData
        {
            get
            {
                return adventureDataControl;
            }
        }

        /**
         * Returns the selected chapter data controller.
         * 
         * @return The selected chapter data controller
         */

        public ChapterDataControl SelectedChapterDataControl
        {
            get
            {
                return chaptersController.getSelectedChapterDataControl();
            }
        }

        /**
         * Returns the identifier summary.
         * 
         * @return The identifier summary
         */
        public IdentifierSummary IdentifierSummary
        {
            get
            {
                return chaptersController.getIdentifierSummary();
            }
        }

        /**
         * Returns the varFlag summary.
         * 
         * @return The varFlag summary
         */
        public VarFlagSummary VarFlagSummary
        {
            get
            {
                return chaptersController.getVarFlagSummary();
            }
        }

        public ChapterListDataControl ChapterList
        {
            get
            {
                return chaptersController;
            }
        }

        /**
         * Returns whether the data has been modified since the last save.
         * 
         * @return True if the data has been modified, false otherwise
         */
        public bool IsDataModified
        {
            get
            {
                return dataModified_F;
            }
        }

        /**
         * Called when the data has been modified, it sets the value to true.
         */
        public void DataModified()
        {

            // If the data were not modified, change the value and set the new title of the window
            if (!dataModified_F)
            {
                dataModified_F = true;
                // mainWindow.updateTitle();
            }
        }

        public bool PlayTransparent
        {
            get
            {
                if (adventureDataControl == null)
                {
                    return false;
                }
                return adventureDataControl.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON;

            }
        }

        // TODO enable this
        //    public void swapPlayerMode(bool showConfirmation)
        //    {

        //        addTool(new SwapPlayerModeTool(showConfirmation, adventureDataControl, chaptersController));
        //    }

        // Functions that perform usual application actions

        /**
         * This method creates a new file with it's respective data.
         * 
         * @return True if the new data was created successfully, false otherwise
         */



        public bool newFile(string gamePath, int fileType)
        {

            ConfigData.LoadFromXML(ReleaseFolders.configFileEditorRelativePath());

            bool fileCreated = false;

            if (fileType == Controller.FILE_ADVENTURE_1STPERSON_PLAYER ||
                fileType == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER)
                fileCreated = newAdventureFile(gamePath, fileType);
            else if (fileType == Controller.FILE_ASSESSMENT)
            {
                //fileCreated = newAssessmentFile();
            }
            else if (fileType == Controller.FILE_ADAPTATION)
            {
                //fileCreated = newAdaptationFile();
            }

            if (fileCreated)
                AssetsController.ResetCache();

            return fileCreated;
        }


        private bool newAdventureFile(string fileName, int fileType)
        {
            bool fileCreated = false;
            bool create = false;
            DirectoryInfo selectedDir = null;
            FileInfo selectedFile = null;
            
            DirectoryInfo selectedFolder = new DirectoryInfo(fileName);
            selectedFile = new FileInfo(fileName);
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
                // Folder can be created/used
                // Does the folder exist?
                if (selectedFolder.Exists)
                {
                    //Is the folder empty?
                    if (selectedFolder.GetFiles().Length > 0)
                    {
                        // Delete content?
                        if (this.ShowStrictConfirmDialog(TC.get("Operation.NewProject.FolderNotEmptyTitle"),
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
                    if (this.ShowStrictConfirmDialog(TC.get("Operation.NewProject.FolderNotCreatedTitle"),
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
                            this.ShowStrictConfirmDialog(TC.get("Error.Title"), TC.get("Error.CreatingFolder"));
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

            // Create the new project?
            //LoadingScreen loadingScreen = new LoadingScreen(TextConstants.getText( "Operation.CreateProject" ), getLoadingImage( ), mainWindow);

            if (create)
            {
                // Set the new file, path and create the new adventure
                currentZipFile = selectedDir.FullName;
                currentZipName = selectedDir.Name;

                int playerMode = -1;
                if (fileType == FILE_ADVENTURE_3RDPERSON_PLAYER)
                    playerMode = DescriptorData.MODE_PLAYER_3RDPERSON;
                else if (fileType == FILE_ADVENTURE_1STPERSON_PLAYER)
                    playerMode = DescriptorData.MODE_PLAYER_1STPERSON;


                adventureDataControl = new AdventureDataControl(TC.get("DefaultValue.AdventureTitle"), "ChapterTitle", TC.get("DefaultValue.SceneId"), playerMode);
                // Clear the list of data controllers and refill it
                chaptersController = new ChapterListDataControl(adventureDataControl.getChapters());

                // Init project properties (empty)
                ProjectConfigData.init();

                AssetsController.CreateFolderStructure();
                AssetsController.addSpecialAssets();
                AssetsController.copyAssets(currentZipFile, new DirectoryInfo("Assets/uAdventure/Resources").FullName);

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
                    catch (IOException)
                    {
                    }

                    // The file was saved
                    fileCreated = true;

                }
                else
                    fileCreated = false;

                if (fileCreated)
                {
                    uAdventureWindowMain.Instance.RefreshWindows();
                }
            }

            return fileCreated;

        }

        public void Save()
        {
            Writer.writeData("Assets/uAdventure/Resources/CurrentGame", adventureDataControl, true);
            ProjectConfigData.storeToXML();
            UnityEditor.AssetDatabase.Refresh();
            dataModified_F = false;
        }

        public void DeleteDirectory(string targetDir)
        {
            if (!Directory.Exists(targetDir))
                return;

            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public bool NewAdventure(int fileType)
        {

            ResourceManager = ResourceManagerFactory.CreateLocal(); 

            string currentGamePath = "Assets/uAdventure/Resources/CurrentGame";
            bool fileCreated = false;
            bool create = false;

            FileInfo selectedFile = new FileInfo(currentGamePath + ".eap");
            ConfigData.LoadFromXML(ReleaseFolders.configFileEditorRelativePath());

            // Check the parent folder is not forbidden
            if (isValidTargetProject(selectedFile))
            {
                // Folder can be created/used
                // Does the folder exist?
                if (Directory.Exists(currentGamePath))
                {
                    DeleteDirectory(currentGamePath);
                }
                Directory.CreateDirectory(currentGamePath);
                create = true; 
            }
            else
            {
                // Show error: The target dir cannot be contained 
                Debug.LogError(TC.get("Operation.NewProject.ForbiddenParent.Title") + " \n " +
                                TC.get("Operation.NewProject.ForbiddenParent.Message"));
                create = false;
            }

            if (create)
            {
                EditorUtility.DisplayProgressBar("Creating new Adventure", "Starting", 0);

                DirectoryInfo selectedDir = new DirectoryInfo(currentGamePath);
                // Set the new file, path and create the new adventure
                currentZipFile = currentGamePath;
                currentZipName = selectedDir.Name;

                int playerMode = -1;
                if (fileType == FILE_ADVENTURE_3RDPERSON_PLAYER)
                    playerMode = DescriptorData.MODE_PLAYER_3RDPERSON;
                else if (fileType == FILE_ADVENTURE_1STPERSON_PLAYER) 
                    playerMode = DescriptorData.MODE_PLAYER_1STPERSON;


                EditorUtility.DisplayProgressBar("Creating new Adventure", "Creating model", 0.1f);
                adventureDataControl = new AdventureDataControl(TC.get("DefaultValue.AdventureTitle"), "ChapterTitle", TC.get("DefaultValue.SceneId"), playerMode);
                // Clear the list of data controllers and refill it
                chaptersController = new ChapterListDataControl(adventureDataControl.getChapters());

                // Init project properties (empty)
                ProjectConfigData.init();

                // Create the folders and add the assets
                EditorUtility.DisplayProgressBar("Creating new Adventure", "Creating folder structure", 0.4f);
                AssetsController.CreateFolderStructure();
                EditorUtility.DisplayProgressBar("Creating new Adventure", "Adding special assets", 0.6f);
                AssetsController.addSpecialAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                // Check the consistency of the chapters
                bool valid = chaptersController.isValid(null, null);

                EditorUtility.DisplayProgressBar("Creating new Adventure", "Persisting model", 0.9f);
                // Save the data
                if (Writer.writeData(currentZipFile, adventureDataControl, valid))
                {
                    // Set modified to false and update the window title
                    dataModified_F = false;

                    Thread.Sleep(1);

                    if (selectedFile != null && !selectedFile.Exists)
                        selectedFile.Create().Close();
                    
                    AssetsController.ResetCache();
                    // The file was saved
                    fileCreated = true;
                    Loaded = true;
                    EditorUtility.DisplayProgressBar("Creating new Adventure", "Done!", 1f);
                }
                else
                    fileCreated = false;

                Save();
                EditorUtility.ClearProgressBar();
                
            }
            Loaded = fileCreated;
            return fileCreated;
        }

        [UnityEditor.MenuItem("uAdventure/Configure Layout", priority = 0)]
        public static void ConfigureWindowLayout()
        {
            ConfigData.LoadFromXML(ReleaseFolders.configFileEditorRelativePath());
            if (string.IsNullOrEmpty(ConfigData.GetExtraProperties().GetProperty("layoutSet")))
            {
                ConfigData.GetExtraProperties().SetProperty("layoutSet", "yes");
                ConfigData.StoreToXML();
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/uAdventure/Editor/Layouts/uAdventure.wlt");
            EditorUtility.LoadWindowLayout(path); 
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


        public static ResourceManager ResourceManager { get; private set; }
        public bool HasError { get; internal set; }
        public string Error { get; internal set; }

        /**
         * Called when the user wants to load data from a file.
         * 
         * @return True if a file was loaded successfully, false otherwise
         */
        public bool LoadFile(string path = null)
        {
			Debug.Log ("Loading file");
            bool fileLoaded = false; 
            bool localLoaded = path == null;
                
            AdventureData loadedAdventureData = null;
            DirectoryInfo directory = null;

            List<Incidence> incidences = new List<Incidence>();

            // LOCAL FILE PATH
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets/uAdventure/Resources/CurrentGame";
                localLoaded = true;
                ResourceManager = ResourceManagerFactory.CreateLocal();
            }
            else 
            {
                path = path.RemoveFromEnd(".eap");
                ResourceManager = ResourceManagerFactory.CreateExternal(path + "/");
            }

            // VOY A CARGAR
            // Create a file to extract the name and path
            directory = new DirectoryInfo(path);

            if (directory.Exists)
            {
                // Load the data from the file, and update the info
                loadedAdventureData = Loader.LoadAdventureData(ResourceManager, incidences);
            }

            // SI LO CARGO HAGO COSAS
            if (loadedAdventureData != null && incidences.Count == 0)
            {
                // Update the values of the controller
                currentZipFile = path;
                currentZipName = directory.Name;

                // Check the project folder structure and dtds
                AssetsController.CreateFolderStructure();

                System.IO.File.WriteAllText("Assets/uAdventure/Resources/CurrentGame.eap", path);
                loadedAdventureData.setProjectName(currentZipName);

                if (!localLoaded)
                {
                    // Recreate the resource manager
                    ResourceManager = ResourceManagerFactory.CreateLocal();
                    directory = new DirectoryInfo("Assets/uAdventure/Resources/CurrentGame");
                    currentZipFile = "Assets/uAdventure/Resources/CurrentGame";
                    currentZipName = directory.Name;

                    // Import the proyect
                    EditorUtility.DisplayProgressBar("Importing project", "Creating folder structure", 0);
                    AssetsController.CreateFolderStructure();
                    EditorUtility.DisplayProgressBar("Importing project", "Adding special assets...", 0.25f);
                    AssetsController.addSpecialAssets();
                    AssetsController.copyAllFiles(new DirectoryInfo(path).FullName, currentZipFile);
                    AssetsController.checkAssetFilesConsistency(incidences);
                    Incidence.sortIncidences(incidences);

                    // Reload the adventure as probably the animations have been replaced
                    EditorUtility.DisplayProgressBar("Importing project", "Finished! Reloading project...", 100);
                    ResourceManager = null;
                    Resources.UnloadUnusedAssets();

                    if (incidences.Count == 0)
                    {
                        ResourceManager = ResourceManagerFactory.CreateLocal();
                        loadedAdventureData = Loader.LoadAdventureData(ResourceManager, incidences);
                        loadedAdventureData.setProjectName(currentZipName);
                    }
                    EditorUtility.ClearProgressBar();
                }
                    
                // PARSING
                if (incidences.Count == 0)
                {
                    try
                    {
                        adventureDataControl = new AdventureDataControl(loadedAdventureData);
                        chaptersController = new ChapterListDataControl(adventureDataControl.getChapters());
                        ProjectConfigData.LoadFromXML();

                        dataModified_F = false;
                        fileLoaded = true;
                        if (!EditorApplication.isPlayingOrWillChangePlaymode)
                        {
                            Game.Instance.ResourceManager = ResourceManager;
                        }
                    }
                    catch (Exception ex)
                    {
                        HasError = true;
                        Error = ex.ToString();
                    }
                }

            }

            if (incidences.Count > 0)
            {

                EditorUtility.DisplayDialog(TC.get("Error.LoadAborted.Title"),
                    TC.get("Error.LoadAborted.Message"), TC.get("GeneralText.OK"));
                HasError = true;
                Error = incidences.Select(i =>
                    i.getImportance() + " | " + i.getType() + " | " + i.getAffectedArea() + " | " + i.getMessage() +
                    " | " + i.getAffectedResource() + " | " + i.getException().ToString()).Aggregate((s1,s2) => s1 + "\n" + s2);
                /*bool abort = fixIncidences(incidences);
                if (abort)
                {
                    mainWindow.showInformationDialog(TC.get("Error.LoadAborted.Title"), TC.get("Error.LoadAborted.Message"));
                    hasIncedence = true;
                }*/
            }

            if (fileLoaded)
            {
                AssetsController.ResetCache();
                if (!localLoaded)
                {
                    // Save to store the upgrades
                    Save();
                }

                //startAutoSave(15);

                //// Feedback
                ////loadingScreen.close( );
                //if (!hasIncedence)
                //    mainWindow.showInformationDialog(TC.get("Operation.FileLoadedTitle"), TC.get("Operation.FileLoadedMessage"));
                //else
                //    mainWindow.showInformationDialog(TC.get("Operation.FileLoadedWithErrorTitle"), TC.get("Operation.FileLoadedWithErrorMessage"));

            }
            else 
            {
                // Feedback
                ShowErrorDialog(TC.get("Operation.FileNotLoadedTitle"), TC.get("Operation.FileNotLoadedMessage"));
            }

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
                        if (!newFile.Exists)
                            System.IO.File.Create(newFile.FullName).Close();

                        // If this is a "Save as" operation, copy the assets from the old file to the new one
                        if (saveAs)
                        {
                            //loadingScreen.setMessage(TC.get("Operation.SaveProjectAs"));
                            //loadingScreen.setVisible(true);
                            AssetsController.copyAssets(new DirectoryInfo("Assets/uAdventure/Resources").FullName, currentZipFile);
                            AssetsController.copyAssets(currentZipFile, newFolder.FullName);
                        }

                        // Set the new file and path
                        AssetsController.CreateFolderStructure();
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

            // If the data must be saved
            if (saveFile)
            {
                //TODO: implement
                ConfigData.StoreToXML();
                ProjectConfigData.storeToXML();
                
                // Check the consistency of the chapters
                var incidences = new List<string>();
                bool valid = chaptersController.isValid(null, incidences);

                // If the data is not valid, show an error message
                if (!valid)
                {
                    Debug.LogError(TC.get("Operation.AdventureConsistencyTitle") + "\n" +
                                   TC.get("Operation.AdventurInconsistentWarning"));
                    foreach (var incidence in incidences)
                    {
                        Debug.LogError(incidence);
                    }
                }

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
                Debug.Log(currentZipFile);
                ProjectConfigData.storeToXML();

                AssetsController.ResetCache();
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
        public bool BuildGame()
        {
            return BuildGame(String.Empty, -1);
        }

        public bool BuildGame(int mode)
        {
            return BuildGame("Games/" + currentZipName, mode);
        }
        

        public bool BuildGame(string targetFilePath, int mode)
        {

            bool buildGame = true;
            bool exported = false;
            //buildGame = saveFile(false);

            if (buildGame)
            {
                string selectedPath = targetFilePath;
                
                if (mode == -1)
                {
                    var exportWindow = BuildWindow.CreateBuildWindow();
                    exportWindow.OnConfigSelected += BuildWindow_OnConfigSelected;
                }
                else if (string.IsNullOrEmpty(selectedPath))
                {
                    // Get filename.
                    var path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
                    var exportConfig = BuildConfigs.Instanciate();
                    exportConfig.path = path;
                    DoBuild(exportConfig);
                }
            }

            return exported;
        }

        private void BuildWindow_OnConfigSelected(object sender, EventArgs e)
        {
            var args = e as ExportConfigSelectedEventArgs;
            Debug.Log("Config selected.");
            DoBuild(args.exportConfig);
        }

        public static class BuildConfigs
        {
            public static bool BuildWindows,
                BuildLinux,
                BuildMacOsX,
                BuildAndroid,
                BuildIOS,
                BuildWebGL;
            public static string path;

            /// <summary>
            /// Creates a instance of an export config with the current ExportConfigs values and returns it.
            /// </summary>
            /// <returns>A instance of ExportConfig with current ExportConfigs values.</returns>
            public static BuildConfig Instanciate()
            {
                var exportConfig = new BuildConfig();

                exportConfig.path = BuildConfigs.path;

                exportConfig.BuildWindows = BuildConfigs.BuildWindows;
                exportConfig.BuildMacOsX = BuildConfigs.BuildMacOsX;
                exportConfig.BuildLinux = BuildConfigs.BuildLinux;
                exportConfig.BuildAndroid = BuildConfigs.BuildAndroid;
                exportConfig.BuildIOS = BuildConfigs.BuildIOS;
                exportConfig.BuildWebGL = BuildConfigs.BuildWebGL;

                exportConfig.fileName = PlayerSettings.productName;
                exportConfig.author = PlayerSettings.companyName;
                exportConfig.version = PlayerSettings.Android.bundleVersionCode;
                exportConfig.packageName = PlayerSettings.applicationIdentifier;

                return exportConfig;
            }
        }

        /// <summary>
        /// Class to transfer the export configs to the simplified exporter.
        /// </summary>
        public class BuildConfig
        {
            /// <summary>
            /// Check if the builder has to export into Windows x86 and x64.
            /// </summary>
            public bool BuildWindows;
            /// <summary>
            /// Check if the builder has to export into Linux universal.
            /// </summary>
            public bool BuildLinux;
            /// <summary>
            /// Check if the builder has to export into Mac Os X universal.
            /// </summary>
            public bool BuildMacOsX;
            /// <summary>
            /// Check if the builder has to export into Android.
            /// </summary>
            public bool BuildAndroid;
            /// <summary>
            /// Check if the builder has to export into iOS
            /// </summary>
            public bool BuildIOS;
            /// <summary>
            /// Check if the builder has to export into WebGL.
            /// </summary>
            public bool BuildWebGL;
            /// <summary>
            /// Version to tag the current build.
            /// </summary>
            public int version;
            /// <summary>
            /// Author/Company of the game.
            /// </summary>
            public string author;
            /// <summary>
            /// Package name in the reverse domain name format (i.e. "com.company.game").
            /// </summary>
            public string packageName;
            /// <summary>
            /// Name of the game and built file.
            /// </summary>
            public string fileName;
            /// <summary>
            /// Path to write down the files.
            /// </summary>
            public string path;
            /// <summary>
            /// Icon for the game.
            /// </summary>
            public string icon;
        }

        private BuildPlayerOptions createBasic(string[] scenes, string path, BuildTarget target, BuildOptions options)
        {
            return new BuildPlayerOptions()
            {
                scenes = scenes,
                locationPathName = path,
                target = target,
                options = options
            };
        }

        private string Namify(string name)
        {
            return name.Split(' ')[0];
        }

        private void DoBuild(BuildConfig config)
        {
            var activeTarget = EditorUserBuildSettings.activeBuildTarget;
            var activeGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            string[] scenes = new string[] { "Assets/uAdventure/Scenes/_Scene1.unity" };

            // Build player.
            List<BuildPlayerOptions> builds = new List<BuildPlayerOptions>();

            PlayerSettings.companyName = config.author;
            PlayerSettings.productName = config.fileName;

            var name = Namify(config.fileName);

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, config.packageName);

            if (config.BuildWindows)
            {
                /*var b = createBasic(scenes, config.path + "/Windows/" + name + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);
                builds.Add(b);*/

                var b64 = createBasic(scenes, config.path + "/Windows64/" + name + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
                builds.Add(b64);
            }

            if (config.BuildLinux)
            {
                var b = createBasic(scenes, config.path + "/Linux/" + name, BuildTarget.StandaloneLinux64, BuildOptions.None);
                builds.Add(b);
            }

            if (config.BuildMacOsX)
            {
                var b = createBasic(scenes, config.path + "/MacOsX/" + name, BuildTarget.StandaloneOSX, BuildOptions.None);
                builds.Add(b);
            }

            if (config.BuildAndroid)
            {
                var b = createBasic(scenes, config.path + "/Android/" + name + ".apk", BuildTarget.Android, BuildOptions.None);
                builds.Add(b);

                PlayerSettings.Android.androidIsGame = true;
                PlayerSettings.Android.bundleVersionCode = config.version;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, config.packageName);
            }

            if (config.BuildIOS)
            {
                var b = createBasic(scenes, config.path + "/iOS(XCode)/", BuildTarget.iOS, BuildOptions.None);
                builds.Add(b);

                PlayerSettings.iOS.buildNumber = config.version.ToString();
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, config.packageName);
            }

            if (config.BuildWebGL)
            {
                var b = createBasic(scenes, config.path + "/WebGL/", BuildTarget.WebGL, BuildOptions.None);
                builds.Add(b);
            }

            for(int build = 0; build < builds.Count; build++)
            {
                EditorUtility.DisplayProgressBar("Building...", "Building for: " + builds[build].target.ToString(), build / ((float)builds.Count));
                BuildPipeline.BuildPlayer(builds[build]);
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayCancelableProgressBar("Building...", "Done!", 1f);
            EditorUtility.ClearProgressBar();

            if (activeTarget != EditorUserBuildSettings.activeBuildTarget || activeGroup != EditorUserBuildSettings.selectedBuildTargetGroup)
                EditorUserBuildSettings.SwitchActiveBuildTarget(activeGroup, activeTarget);
        }

        private static readonly string WINDOWS64_FFMPEG_URL = "https://github.com/e-ucm/uAdventure-FFMPEG/releases/download/1/ffmpeg-3.4.2-win64.zip";
        private static readonly string WINDOWS32_FFMPEG_URL = "https://github.com/e-ucm/uAdventure-FFMPEG/releases/download/1/ffmpeg-3.4.2-win32.zip";
        private static readonly string MACOSX64_FFMPEG_URL  = "https://github.com/e-ucm/uAdventure-FFMPEG/releases/download/1/ffmpeg-3.4.2-mac64.zip";

        public static void DownloadDependencyZip(string name, string folderName, string url, System.Action<bool> ready)
        {
            var projectPath = Directory.GetCurrentDirectory().Replace("\\", "/");
            var downloadPath = projectPath + folderName;

            using (var uwr = UnityWebRequest.Get(url))
            {
                uwr.SendWebRequest();
                while (!uwr.isDone)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Downloading", "Downloading "+ name + "...", uwr.downloadProgress))
                    {
                        EditorUtility.ClearProgressBar();
                        ready(false);
                        return;
                    }
                }
                EditorUtility.ClearProgressBar();

                if (!string.IsNullOrEmpty(uwr.error))
                {
                    EditorUtility.DisplayDialog("Error!", "Download failed! Check your connection and try again. " +
                        "If the problem persist download it manually and put it in the " + folderName +" folder at the root of the project. (" + uwr.error + ")", "Ok");
                    ready(false);
                    return;
                }

                if (uwr.downloadProgress != 1f)
                {
                    ready(false);
                    return;
                }
                // Write the zip file
                var downloadFileName = name.Replace(" ", "").Trim() + ".zip";

                if (!Directory.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }

                File.WriteAllBytes(downloadPath + "/" + downloadFileName, uwr.downloadHandler.data);
                // Unzip it
                EditorUtility.DisplayProgressBar("Extracting...", "Extracting " + name + " to " + downloadPath, 0f);

                var buffer = new byte[1024];
                using (FileStream zipToOpen = new FileStream(downloadPath + "/" + downloadFileName, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        foreach(var entry in archive.Entries)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(downloadPath + "/" + entry.FullName));
                            using (var file = File.OpenWrite(downloadPath + "/" + entry.FullName))
                            using (var s = entry.Open())
                            {
                                while(s.CanRead)
                                {
                                    var read = s.Read(buffer, 0, 1024);
                                    file.Write(buffer, 0, read);
                                }
                            }
                        }
                    }
                }
                EditorUtility.DisplayProgressBar("Extracting...", "Extracting " + name + " to " + downloadPath, 1f);
                EditorUtility.ClearProgressBar();
                // Delete the zip
                File.Delete(downloadPath + "/" + downloadFileName);
                // Continue
                ready(true);
            }
        }
        
        [PostProcessBuild(1)]
        public static void PostProcess(BuildTarget target, string pathToBuiltProject)
        {

            if (target == BuildTarget.WebGL)
            {
                Debug.Log("Adding videos to WebGL StreamingAssets folder");
                var projectPath = Directory.GetCurrentDirectory().Replace("\\", "/");
                var ffmpegPath = projectPath + "/FFMPEG";
                if (!Directory.Exists(ffmpegPath))
                    Directory.CreateDirectory(ffmpegPath);

                System.Action<bool> convert = (downloaded) =>
                {
                    if (downloaded)
                    {
                        ConvertVideos(pathToBuiltProject);
                    }
                };

                switch (SystemInfo.operatingSystemFamily)
                {
                    case OperatingSystemFamily.Windows:
                        if (!File.Exists(ffmpegPath + "/ffmpeg.exe"))
                        {
                            if(EditorUtility.DisplayDialog("Video conversion", "FFMPEG was not found, do you want to download it? Videos for WebGL must be in Webm format, this software will be used to covert them automatically.", "Yes", "No"))
                            {

                                if (SystemInfo.operatingSystem.Contains("64bit"))
                                    DownloadDependencyZip("FFMPEG", "/FFMPEG", WINDOWS64_FFMPEG_URL, convert);
                                else
                                    DownloadDependencyZip("FFMPEG", "/FFMPEG", WINDOWS32_FFMPEG_URL, convert);
                            }
                        }                            
                        else convert(true);
                        break;

                    case OperatingSystemFamily.MacOSX:
                        if (!File.Exists(ffmpegPath + "/ffmpeg"))
                        {
                            if (EditorUtility.DisplayDialog("Video conversion", "FFMPEG was not found, do you want to download it? Videos for WebGL must be in Webm format, this software will be used to covert them automatically.", "Yes", "No"))
                            {
                                DownloadDependencyZip("FFMPEG", "/FFMPEG", MACOSX64_FFMPEG_URL, convert);
                            }
                        }
                        else convert(true);
                        break;

                    default:
                        Debug.LogWarning("Video conversion is not supported in the current OS! Copying the videos to streamingassets.");
                        CopyAll(new DirectoryInfo(UnityEngine.Application.dataPath + "/uAdventure/Resources/CurrentGame/assets/video"), new DirectoryInfo(pathToBuiltProject + "/StreamingAssets"));
                        break;
                }


                Debug.Log("Done!");
            }
        }

        public void ExportLearningObject()
        {
            if (!IsPlatformAvailable(BuildTarget.WebGL))
            {
                ShowErrorDialog("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.WebGLUnavailable".Traslate());
                return;
            }

            var projectPath = "./Builds/WebGL/";
            if (Directory.Exists(projectPath) && File.Exists(projectPath + "index.html"))
            {
                if(!ShowStrictConfirmDialog("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.BuildDetected".Traslate()))
                {
                    projectPath = null;
                }
            }

            var outputFile = EditorUtility.SaveFilePanel("Operation.ExportProject.AsLO".Traslate(), projectPath, PlayerSettings.productName + ".zip", ".zip");
            if(string.IsNullOrEmpty(outputFile))
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.Starting".Traslate(), 0f);

            if (projectPath == null)
            {
                EditorUtility.DisplayProgressBar("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.BuildingWebGL".Traslate(), 0.05f);
                DoBuild(new BuildConfig
                {
                    BuildWebGL = true,
                    fileName = PlayerSettings.productName,
                    author = PlayerSettings.companyName,
                    version = PlayerSettings.Android.bundleVersionCode,
                    packageName = PlayerSettings.applicationIdentifier
                });
                projectPath = "./Builds/WebGL/"; // Default build path
            }


            EditorUtility.DisplayProgressBar("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.CreatingManifest".Traslate(), 0.75f);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ManifestType));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ManifestType manifest = GetImsManifest();

            XmlDocument doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                serializer.Serialize(writer, manifest, ns);
            }

            XmlDocument finalDoc = new XmlDocument();
            var finalElement = MetadataUtility.CleanXMLGarbage(finalDoc, doc.DocumentElement);

            finalDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            finalDoc.AppendChild(finalElement);
            finalDoc.DocumentElement.SetAttribute("xmlns:imsmd", "http://www.imsglobal.org/xsd/imsmd_v1p2");
            finalDoc.DocumentElement.SetAttribute("xsi:schemaLocation", "http://www.imsglobal.org/xsd/imscp_v1p1 ../xsds/imscp_v1p2.xsd http://www.imsglobal.org/xsd/imsmd_v1p2 http://www.imsglobal.org/xsd/imsmd_v1p2p4.xsd ");

            using (var fw = File.Open(projectPath+"imsmanifest.xml", FileMode.Create))
            using (var xmlWr = XmlWriter.Create(fw, new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                Indent = true
            }))
            {
                finalDoc.WriteTo(xmlWr);
                xmlWr.Flush();
                fw.Flush();

                Debug.Log(fw.ToString());
            }

            EditorUtility.DisplayProgressBar("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.GatheringFiles".Traslate(), 0.80f);
            var files = Directory.GetFiles(projectPath, "*", SearchOption.AllDirectories);
            /*var zipFile = new Ionic.Zip.ZipFile(PlayerSettings.productName + ".zip", System.Text.Encoding.UTF8);
            var prefix = new DirectoryInfo(projectPath).FullName;
            foreach (var file in files.Select(f => new FileInfo(f)))
            {
                zipFile.AddFile(file.FullName, file.FullName.Remove(0, prefix.Length).RemoveFromEnd(file.Name));
            }
            EditorUtility.DisplayProgressBar("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.Compressing".Traslate(), 0.9f);
            zipFile.Save(outputFile);

            EditorUtility.DisplayProgressBar("Operation.ExportProject.AsLO".Traslate(), "ExportLearningObject.Done".Traslate(), 1f);
            EditorUtility.ClearProgressBar();
            EditorUtility.RevealInFinder(outputFile);*/
        }

        private static ManifestType GetImsManifest()
        {
            return new ManifestType
            {
                version = "IMS CP 1.2",
                identifier = MetadataUtility.GenerateManifestIdentifier(),
                metadata = new ManifestMetadataType
                {
                    schema = "IMS Content",
                    schemaversion = "1.2",
                    Any = new XmlElement[]
                    {
                            MetadataUtility.SerializeToXmlElement(new lomType
                            {
                                general = new generalType
                                {
                                    title = new langType
                                    {
                                        langstring = new langstringType[]
                                        {
                                            new langstringType
                                            {
                                                lang = "es-ES",
                                                Value = Controller.Instance.AdventureData.getTitle()
                                            }
                                        }
                                    }
                                }
                            })
                    }
                },
                organizations = new OrganizationsType
                {
                    @default = "uAdventure",
                    organization = new OrganizationType[]
                    {
                            new OrganizationType
                            {
                                title = "uAdventure course",
                                item = new ItemType[]
                                {
                                    new ItemType
                                    {
                                        identifier = "itm_uAdventure",
                                        identifierref = UADVENTURE_RESOURCE,
                                        isvisible = true,
                                        title = Controller.Instance.AdventureData.getTitle()
                                    }
                                }
                            }
                    }
                },
                resources = new ResourcesType
                {
                    resource = new ResourceType[]
                    {
                            new ResourceType
                            {
                                href = "index.html",
                                identifier = UADVENTURE_RESOURCE,
                                type = "webcontent",
                                metadata = new MetadataType
                                {
                                    schema = "IMS Content",
                                    schemaversion = "1.2",
                                    Any = new XmlElement[] 
                                    { 
                                        MetadataUtility.SerializeToXmlElement(controllerInstance.AdventureData.getImsCPMetadata()) 
                                    }
                                },
                                file = new IMS.CP.v1p2.FileType[]
                                {
                                    new IMS.CP.v1p2.FileType
                                    {
                                        href = "index.html"
                                    }
                                }
                            }
                    }
                }
            };
        }

        private static bool IsPlatformAvailable(BuildTarget target)
        {
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target }) });
        }

        public static int GetFrameCount(FileInfo file)
        {
            var projectPath = Directory.GetCurrentDirectory().Replace("\\", "/");
            var ffmpegPath = projectPath + "/FFMPEG";

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                Arguments = " -i \"" + file.FullName + "\" -map 0:v:0 -c copy -f null -stats -v quiet -",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows: startInfo.FileName = ffmpegPath + "/ffmpeg.exe"; break;
                case OperatingSystemFamily.MacOSX: startInfo.FileName = ffmpegPath + "/ffmpeg"; break;
            }

            System.Diagnostics.Process processTemp = new System.Diagnostics.Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            int frames = 1;
            try
            {
                processTemp.Start();
                while (!processTemp.HasExited) { }
                var error = processTemp.StandardError.ReadToEnd();
                if (processTemp.ExitCode != 0 && !string.IsNullOrEmpty(error))
                    EditorUtility.DisplayDialog("Error", error, "continue...");
                else if(error.Length > 7)
                {
                    var framesText = error.Substring(6, 6).Trim();
                    frames = ExParsers.ParseDefault(framesText, -1);
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return frames;
        }

        private static IEnumerator<int> ConvertVideo(FileInfo fi, string outFile)
        {
            var projectPath = Directory.GetCurrentDirectory().Replace("\\", "/");
            var ffmpegPath = projectPath + "/FFMPEG";

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                Arguments = " -y -i \"" + fi.FullName + "\" -f webm -c:v libvpx -b:v 1M -vf \"scale = 'min(1280,iw)':-1\" -acodec libvorbis " + outFile + " -hide_banner -stats -v quiet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows: startInfo.FileName = ffmpegPath + "/ffmpeg.exe"; break;
                case OperatingSystemFamily.MacOSX:  startInfo.FileName = ffmpegPath + "/ffmpeg";     break;
            }

            System.Diagnostics.Process processTemp = new System.Diagnostics.Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            if (processTemp.Start())
            { 
                while (!processTemp.HasExited)
                {
                    var output = processTemp.StandardError.ReadLine();
                    // Return the current frame
                    if(!string.IsNullOrEmpty(output))
                        yield return ExParsers.ParseDefault(output.Substring(6, 6).Trim(), -1);
                }
                if (processTemp.ExitCode != 0)
                {
                    EditorUtility.DisplayDialog("Error", processTemp.StandardError.ReadToEnd(), "continue...");
                }
            }
            else Debug.Log("Couldn't start the process");

        }

        public static void ConvertVideos(string pathToBuiltProject)
        {
            var outFolder = pathToBuiltProject + "/StreamingAssets/CurrentGame/assets/video/";
            var source = new DirectoryInfo(UnityEngine.Application.dataPath + "/uAdventure/Resources/CurrentGame/assets/video");

            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);
            var toConvert = source.GetFiles();
            var i = 0;
            EditorUtility.DisplayProgressBar("Converting", "Converting videos...", 0f);
            // Copy each file into it's new directory.
            foreach (FileInfo fi in toConvert)
            {
                ++i;
                if (fi.Extension == ".meta")
                    continue;
                EditorUtility.DisplayProgressBar("Converting", "Converting videos...", i / (float) toConvert.Length);
                var outName = outFolder + Path.GetFileNameWithoutExtension(fi.Name) + ".webm";
                var count = GetFrameCount(fi);

                var convertVideo = ConvertVideo(fi, outName);
                while (convertVideo.MoveNext())
                {
                    EditorUtility.DisplayProgressBar("Encoding", "Encoding video " + fi.Name, convertVideo.Current / (float) count);
                }
                EditorUtility.ClearProgressBar();
            }
            EditorUtility.ClearProgressBar();
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }




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
        

        ///**
        // * Shows the adventure data dialog editor.
        // */
        //public void showAdventureDataDialog()
        //{

        //    new AdventureDataDialog();
        //}
        

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
            AddTool(new AddChapterTool(chaptersController, val));
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

            AddTool(new DeleteChapterTool(chaptersController));
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
            return isElementIdValid(elementId, false);
        }

        public string makeElementValid(string elementId)
        {
            if (elementId == null) elementId = "new";
            var clean = elementId.Replace(" ", "").Replace("'"," ");
            while (clean != "" && !char.IsLetter(clean[0]) && clean.Length > 0)
            {
                clean = clean.Remove(0, 1);
            }
            if (string.IsNullOrEmpty(clean)) clean = "new";

            if (clean.Equals(Player.IDENTIFIER) || clean.Equals(TC.get("ConversationLine.PlayerName")))
                clean = "new";
            
            while (IdentifierSummary.existsId(clean))
            {
                int lastN = 0;
                Match match = Regex.Match(clean, @"\d+$");

                for (; match.Success; match = match.NextMatch())
                {
                    lastN = int.Parse(match.Value, NumberFormatInfo.InvariantInfo); // do something with it
                }
                if (lastN == 0)
                {
                    clean += "0";
                }
                clean = clean.Substring(0, clean.Length - ("" + lastN).Length);
                lastN++;
                clean += lastN;
            }

            return clean;
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
                if (!IdentifierSummary.existsId(elementId))
                {

                    //If the identifier is not a reserved identifier
                    if (!elementId.Equals(Player.IDENTIFIER) && !elementId.Equals(TC.get("ConversationLine.PlayerName")))
                    {

                        //If the first character is a letter
                        if (elementId.Length > 0 && char.IsLetter(elementId[0]))
                        {
                            elementIdValid = isCharacterValid(elementId);
							/*if (!elementIdValid && showError)
                                mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorCharacter"));*/
                        }
                        //Show non-letter first character error
                        /*else if (showError)
                            mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorFirstCharacter"));*/
                    }

                    //Show invalid identifier error
                   /* else if (showError)
                        mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorReservedIdentifier", elementId));*/
                }

                // Show repeated identifier error
                /*else if (showError)
                    mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorAlreadyUsed"));*/
            }

            //Show blank spaces error
            /*else if (showError)
                mainWindow.showErrorDialog(TC.get("Operation.IdErrorTitle"), TC.get("Operation.IdErrorBlankSpaces"));*/

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
            foreach (SceneDataControl scene in SelectedChapterDataControl.getScenesList().getScenes())
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
            foreach (SceneDataControl scene in SelectedChapterDataControl.getScenesList().getScenes())
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

            if (SelectedChapterDataControl!= null)
                return SelectedChapterDataControl.getPlayer().getPreviewImage();
            else
                return null;
        }

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
            foreach (ItemDataControl item in SelectedChapterDataControl.getItemsList().getItems())
                if (elementId.Equals(item.getId()))
                    elementImage = item.getPreviewImage();

            // Search for the image in the characters, comparing the identifiers
            foreach (NPCDataControl npc in SelectedChapterDataControl.getNPCsList().getNPCs())
                if (elementId.Equals(npc.getId()))
                    elementImage = npc.getPreviewImage();
            // Search for the image in the items, comparing the identifiers
            foreach (AtrezzoDataControl atrezzo in SelectedChapterDataControl.getAtrezzoList().getAtrezzoList())
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
            var incidences = new List<Incidence>();
            Animation animation = Loader.LoadAnimation(eaaFilePath, ResourceManager, incidences);
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
            var incidences = new List<Incidence>();
            Animation animation = Loader.LoadAnimation(eaaFilePath, ResourceManager, incidences);

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

        /**
         * Deletes a given asset from the script, removing all occurrences.
         * 
         * @param assetPath
         *            Path of the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         */
        public void deleteAssetReferences(string assetPath)
        {

            adventureDataControl.deleteAssetReferences(assetPath);
            chaptersController.deleteAssetReferences(assetPath);

            // Add references in images and sounds in eaa files
            List<string> assetPaths = new List<string>();
            List<int> assetTypes = new List<int>();
            getAssetReferences(assetPaths, assetTypes);

            /*for (int i = 0; i < assetPath.Length; i++)
            {
                if (assetPaths[i].ToLower().EndsWith(".eaa"))
                {
                    deleteAssetReferencesInEAA(assetPaths[i], assetPath);
                }
            }*/

        }
        


        ///**
        // * Counts all the references to a given identifier in the entire script.
        // * 
        // * @param id
        // *            Identifier to which the references must be found
        // * @return Number of references to the given identifier
        // */
        public int countIdentifierReferences(string id)
        {

            return SelectedChapterDataControl.countIdentifierReferences(id);
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
            SelectedChapterDataControl.replaceIdentifierReferences(oldId, newId);
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
        public bool ShowStrictConfirmDialog(string title, string message)
        {
            return EditorUtility.DisplayDialog(title, message, TC.get("GeneralText.Yes"), TC.get("GeneralText.No"));
        }

        public void ShowInputDialog(string title, string message, InputReceiver.HandleInputCallback onInput, InputReceiver.CancelInputCallback onCancel = null)
        {
            ShowInputDialog(title, message, "", onInput, onCancel);
        }

        public void ShowInputDialog(string title, string message, object token, DialogReceiverInterface receiver)
        {
            ShowInputDialog(title, message, "", token, receiver);
        }

        public void ShowInputDialog(string title, string message, string defaultValue, InputReceiver.HandleInputCallback onInput, InputReceiver.CancelInputCallback onCancel = null)
        {
            var receiver = new InputReceiver(this, onInput, onCancel);
            ShowInputDialog(title, message, defaultValue, null, receiver);
        }
        public void ShowInputDialog(string title, string message, string defaultValue, object token, DialogReceiverInterface receiver)
        {
            var inputDialog = ScriptableObject.CreateInstance<InputDialog>();
            inputDialog.Init(receiver, token, title, message, defaultValue);
        }

        public void ShowInputDialog(string title, string message, object[] selectionValues, InputReceiver.HandleInputCallback onInput, InputReceiver.CancelInputCallback onCancel = null)
        {
            var receiver = new InputReceiver(this, onInput, onCancel);
            ShowInputDialog(title, message, selectionValues, null, receiver);
        }

        public void ShowInputDialog(string title, string message, object[] selectionValues, object token, DialogReceiverInterface receiver)
        {
            var inputDialog = ScriptableObject.CreateInstance<ChooseObjectDialog>();
            string[] values;
            if(selectionValues is string[])
            {
                values = selectionValues as string[];
            }
            else
            {
                values = new string[selectionValues.Length];
                for (int i = 0; i < selectionValues.Length; ++i)
                    values[i] = selectionValues[i].ToString();
            }

            inputDialog.Init(receiver, token, title, message, values);
        }

        public void ShowInputIdDialog(string title, string message, string defaultValue, InputReceiver.HandleInputCallback onInput, InputReceiver.CancelInputCallback onCancel = null)
        {
            var receiver = new InputReceiver(this, onInput, onCancel);
            var inputDialog = ScriptableObject.CreateInstance<InputDialog>();
            inputDialog.Init(receiver, null, title, message, defaultValue);
            inputDialog.Validation(value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    return "Validation.Id.IsEmpty";
                }
                if (value.Contains(" "))
                {
                    return "Validation.Id.ContainsSpaces";
                }
                Regex alphanumeric = new Regex("^[a-zA-Z0-9\\.\\:_-]*$");
                if (!alphanumeric.IsMatch(value))
                {
                    return "Validation.Id.NotValidCharacter";
                }
                if (!char.IsLetter(value[0]))
                {
                    return "Validation.Id.NotStartsWithLetter";
                }
                if (value.ToLower().Equals(Player.IDENTIFIER.ToLower()) || value.ToLower().Equals(TC.get("ConversationLine.PlayerName").ToLower()))
                {
                    return "Validation.Id.IsReservedPlayerValue";
                }
                if (IdentifierSummary.existsId(value))
                {
                    return "Validation.Id.IsAlreadyUsed";
                }
                
                return null;
            });
        }

        ///**
        // * Uses the GUI to show an error dialog.
        // * 
        // * @param title
        // *            Title of the dialog
        // * @param message
        // *            Message of the dialog
        // */
        public void ShowErrorDialog(string title, string message)
        {
            EditorUtility.DisplayDialog(title, message, "Ok");
            Debug.LogError(title + "\n" + message);
        }

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
         * The method will Reload the main window always
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
         * The method will Reload the main window if reloadData is true
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
        }
        

        //// METHODS TO MANAGE UNDO/REDO

        public bool AddTool(Tool tool)
        {
            return chaptersController.addTool(tool);
        }

        public void UndoTool()
        {
            chaptersController.undoTool();
        }

        public void RedoTool()
        {
            chaptersController.redoTool();
        }

        private void OnPlay(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.ExitingEditMode:
                    if (IsDataModified)
                    {
                        if (EditorUtility.DisplayDialog(TC.get("Operation.SaveChangesTitle"),
                            TC.get("Run.CanBeRun.Text"), TC.get("GeneralText.Yes"), TC.get("GeneralText.No")))
                        {
                            Save();
                        }
                    }
                    break;
            }
        }


        public void SelectElement(Searchable element)
        {
            var path = SelectedChapterDataControl.getPathToDataControl(element);

            uAdventureWindowMain.Instance.SelectElement(path);
        }

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

        public AdvancedFeaturesDataControl getAdvancedFeaturesController()
        {

            return chaptersController.getSelectedChapterDataControl().getAdvancedFeaturesController();
        }

        public void RefreshView()
        {
            uAdventureWindowMain.Instance.RefreshChapter();
        }

        public class InputReceiver : DialogReceiverInterface
        {
            public delegate void HandleInputCallback(object sender, string input);
            public delegate void CancelInputCallback(object sender);
            
            private event HandleInputCallback HandleInput;
            private event CancelInputCallback CancelInput;

            public InputReceiver(object sender, HandleInputCallback handleInput, CancelInputCallback cancelInput = null)
            {
                this.HandleInput = handleInput;
                this.CancelInput = cancelInput;
            }

            public void OnDialogCanceled(object workingObject = null)
            {
                if(CancelInput != null)
                {
                    CancelInput(workingObject);
                }
            }

            public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
            {
                if(HandleInput != null)
                {
                    HandleInput(workingObject, message);
                }
            }

        }

#region Windows
        [UnityEditor.MenuItem("uAdventure/Welcome screen", priority = -2)]
        public static void OpenWelcomeWindow()
        {
            if (!Language.Initialized)
            {
                Language.Initialize();
            }

            var window = EditorWindow.GetWindow(typeof(WelcomeWindow));
            window.Show();
        }

        [UnityEditor.MenuItem("uAdventure/Experimental/Simva user UnComplete", priority = 1)]
        public static void UnComplete()
        {
            if(EditorUtility.DisplayDialog("Uncomplete?", "Do you want to un-complete a Simva token?", "Yes", "Cancel"))
            {
                Controller.Instance.ShowInputDialog("Uncomplete", "Insert token: ", (s, token) =>
                {
                    SimvaApi<StudentsApi>.LoginWithToken(token).Then(api =>
                    {
                        System.Action onConfigReady = () =>
                        {
                            api.Api.GetSchedule(SimvaConf.Local.Study)
                            .Then(schedule =>
                            {
                                var act = schedule.Activities.First(a => a.Value.Name == "Gameplay");
                                api.Api.SetCompletion(act.Key, token, false);
                            });
                        };
                        Debug.Log("[SIMVA] Starting...");
                        if (SimvaConf.Local == null)
                        {
                            SimvaConf.Local = new SimvaConf();
                            Observable.FromCoroutine(SimvaConf.Local.LoadAsync).Subscribe(_ => onConfigReady());
                            Debug.Log("[SIMVA] Conf Loaded...");
                        }

                        onConfigReady();
                    });
                });
            }

            
        }
        [UnityEditor.MenuItem("uAdventure/Experimental/Screenshot", priority = 1)]
        public static void Screenshot()
        {
            ScreenCapture.CaptureScreenshot("Capture");

        }


        [UnityEditor.MenuItem("uAdventure/Editor", priority = -1)]
        public static void OpenEditorWindow()
        {
            if (!Language.Initialized)
            {
                Language.Initialize();
            }


            ConfigData.LoadFromXML(ReleaseFolders.configFileEditorRelativePath());
            if (string.IsNullOrEmpty(ConfigData.GetExtraProperties().GetProperty("layoutSet")))
            {
                ConfigData.GetExtraProperties().SetProperty("layoutSet", "yes");
                ConfigData.StoreToXML();
                if (Controller.Instance.ShowStrictConfirmDialog("Set up the layout?", "Do you want to set up the Unity " +
                    "layout to uAdventure's layout (Recommended for new users)?"))
                {
                    Controller.ConfigureWindowLayout();
                }
            }

            var window = EditorWindow.GetWindow(typeof(uAdventureWindowMain));
            window.Show();
        }  

#endregion
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.IO;

using uAdventure.Core;
using System.Security;

namespace uAdventure.Editor
{
    public class ConfigData
    {

        private bool showStartDialog_F;

        private static ConfigData instance;
        private static Properties extraProperties;

        private string configFile;

        /**
         * Stores the file that contains the GUI strings.
         */
        private string languageFile;

        /**
         * Stores the file that contains the about document.
         */
        private string aboutFile;

        /**
         * Stores the file that contains the loading screen.
         */
        private string loadingImage;

        /**
         * Stores whether the item references must be displayed by default.
         */
        private bool showItemReferences_F;

        /**
         * Stores whether the atrezzo item references must be displayed by default.
         */
        private bool showAtrezzoReferences_F;

        /**
         * Stores whether the character references must be displayed by default.
         */
        private bool showNPCReferences_F;

        private string exportsPath;

        private string reportsPath;

        private string projectsPath;

        private int effectSelectorTab;


        /////// Added to control preferences on support display options for debugging
        private DebugSettings debugOptions;
        //////

        /**
         * @since V1.5
         * Stores the last X position of the upper-left vertex of the main window
         */
        private int editorWindowX = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last Y position of the upper-left vertex of the main window
         */
        private int editorWindowY = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last width of the main window
         */
        private int editorWindowW = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last height of the main window
         */
        private int editorWindowH = int.MaxValue;

        // Engine - Run

        /**
         * @since V1.5
         * Stores the last X position of the upper-left vertex of the game engine
         */
        private int engineWindowX = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last Y position of the upper-left vertex of the game engine
         */
        private int engineWindowY = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last width of the game engine
         */
        private int engineWindowW = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last height of the game engine
         */
        private int engineWindowH = int.MaxValue;

        // Engine - Debug

        /**
         * @since V1.5
         * Stores the last X position of the upper-left vertex of the game debug
         */
        private int debugWindowX = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last Y position of the upper-left vertex of the game debug
         */
        private int debugWindowY = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last width of the game debug
         */
        private int debugWindowW = int.MaxValue;

        /**
         * @since V1.5
         * Stores the last height of the game debug
         */
        private int debugWindowH = int.MaxValue;

        public static Properties GetExtraProperties()
        {
            return extraProperties;
        }

        public static bool showNPCReferences()
        {

            return instance.showNPCReferences_F;
        }

        public static bool showItemReferences()
        {

            return instance.showItemReferences_F;
        }

        public static bool showAtrezzoReferences()
        {

            return instance.showAtrezzoReferences_F;
        }

        public static string getLanguangeFile()
        {
            return instance.languageFile;
        }

        public static string getAboutFile()
        {

            return instance.aboutFile;
        }

        public static string getLoadingImage()
        {

            return instance.loadingImage;
        }

        public static DebugSettings getUserDefinedDebugSettings()
        {
            return instance.debugOptions;
        }

        public static bool showStartDialog()
        {

            return instance.showStartDialog_F;
        }

        public static void setShowNPCReferences(bool b)
        {

            instance.showNPCReferences_F = b;
        }

        public static void setShowItemReferences(bool b)
        {

            instance.showItemReferences_F = b;
        }

        public static void setShowAtrezzoReferences(bool b)
        {

            instance.showAtrezzoReferences_F = b;
        }

        public static void setLanguangeFile(string language, string about, string loadingImage)
        {

            instance.languageFile = language;
            instance.aboutFile = about;
            instance.loadingImage = loadingImage;
        }

        public static void setAboutFile(string s)
        {

            instance.aboutFile = s;
        }

        public static void setLoadingImage(string s)
        {

            instance.loadingImage = s;
        }

        public static void setShowStartDialog(bool b)
        {

            instance.showStartDialog_F = b;
        }

        // Editor
        public static void setEditorWindowX(int x)
        {
            instance.editorWindowX = x;
        }

        public static void setEditorWindowY(int y)
        {
            instance.editorWindowY = y;
        }

        public static void setEditorWindowWidth(int w)
        {
            instance.editorWindowW = w;
        }

        public static void setEditorWindowHeight(int h)
        {
            instance.editorWindowH = h;
        }

        public static int getEditorWindowX()
        {
            return instance.editorWindowX;
        }

        public static int getEditorWindowY()
        {
            return instance.editorWindowY;
        }

        public static int getEditorWindowWidth()
        {
            return instance.editorWindowW;
        }

        public static int getEditorWindowHeight()
        {
            return instance.editorWindowH;
        }


        //Engine - normal run
        public static void setEngineWindowX(int x)
        {
            instance.engineWindowX = x;
        }

        public static void setEngineWindowY(int y)
        {
            instance.engineWindowY = y;
        }

        public static void setEngineWindowWidth(int w)
        {
            instance.engineWindowW = w;
        }

        public static void setEngineWindowHeight(int h)
        {
            instance.engineWindowH = h;
        }

        public static int getEngineWindowX()
        {
            return instance.engineWindowX;
        }

        public static int getEngineWindowY()
        {
            return instance.engineWindowY;
        }

        public static int getEngineWindowWidth()
        {
            return instance.engineWindowW;
        }

        public static int getEngineWindowHeight()
        {
            return instance.engineWindowH;
        }


        //Engine - debug run
        public static void setDebugWindowX(int x)
        {
            instance.debugWindowX = x;
        }

        public static void setDebugWindowY(int y)
        {
            instance.debugWindowY = y;
        }

        public static void setDebugWindowWidth(int w)
        {
            instance.debugWindowW = w;
        }

        public static void setDebugWindowHeight(int h)
        {
            instance.debugWindowH = h;
        }

        public static int getDebugWindowX()
        {
            return instance.debugWindowX;
        }

        public static int getDebugWindowY()
        {
            return instance.debugWindowY;
        }

        public static int getDebugWindowWidth()
        {
            return instance.debugWindowW;
        }

        public static int getDebugWindowHeight()
        {
            return instance.debugWindowH;
        }


        public static void LoadFromXML(string configFile)
        {
            instance = new ConfigData(configFile);
        }

        public static void StoreToXML()
        {
            // Load the current configuration
            Properties configuration = new Properties(instance.configFile);

            configuration.SetProperty("EditorWindowX", instance.editorWindowX.ToString());
            configuration.SetProperty("EditorWindowY", instance.editorWindowY.ToString());
            configuration.SetProperty("EditorWindowWidth", instance.editorWindowW.ToString());
            configuration.SetProperty("EditorWindowHeight", instance.editorWindowH.ToString());

            configuration.SetProperty("EngineWindowX", instance.engineWindowX.ToString());
            configuration.SetProperty("EngineWindowY", instance.engineWindowY.ToString());
            configuration.SetProperty("EngineWindowWidth", instance.engineWindowW.ToString());
            configuration.SetProperty("EngineWindowHeight", instance.engineWindowH.ToString());

            configuration.SetProperty("DebugWindowX", instance.debugWindowX.ToString());
            configuration.SetProperty("DebugWindowY", instance.debugWindowY.ToString());
            configuration.SetProperty("DebugWindowWidth", instance.debugWindowW.ToString());
            configuration.SetProperty("DebugWindowHeight", instance.debugWindowH.ToString());

            configuration.SetProperty("PaintGrid", instance.debugOptions.isPaintGrid().ToString());
            configuration.SetProperty("PaintHotSpots", instance.debugOptions.isPaintHotSpots().ToString());
            configuration.SetProperty("PaintBoundingAreas", instance.debugOptions.isPaintBoundingAreas().ToString());

            configuration.SetProperty("ShowItemReferences", instance.showItemReferences_F.ToString());
            configuration.SetProperty("ShowNPCReferences", instance.showNPCReferences_F.ToString());
            configuration.SetProperty("ShowAtrezzoReferences", instance.showAtrezzoReferences_F.ToString());
            configuration.SetProperty("ShowStartDialog", instance.showStartDialog_F.ToString());
            configuration.SetProperty("LanguageFile", instance.languageFile);
            configuration.SetProperty("AboutFile", instance.aboutFile);
            configuration.SetProperty("LoadingImage", instance.loadingImage);
            
            if (instance.exportsPath != null)
                configuration.SetProperty("ExportsDirectory", instance.exportsPath);
            if (instance.reportsPath != null)
                configuration.SetProperty("ReportsDirectory", instance.reportsPath);
            if (instance.projectsPath != null)
                configuration.SetProperty("ProjectsDirectory", instance.projectsPath);
            if (instance.projectsPath != null)
                configuration.SetProperty("EffectSelectorTab", instance.effectSelectorTab.ToString());

            if (extraProperties != null)
            {
                configuration.SetProperty("ExtraProperties", SecurityElement.Escape(extraProperties.ToString()));
            }

            // Store the configuration into a file
            try
            {
                configuration.Save();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.StackTrace);
            }

        }

        private ConfigData(string fileName)
        {
            Debug.Log("Loading config data at: " + fileName);
            this.configFile = fileName;
            Properties configuration = new Properties(fileName);
            try
            {
                configuration.Reload(fileName);
                languageFile = configuration.GetProperty("LanguageFile");
                aboutFile = configuration.GetProperty("AboutFile");
                loadingImage = configuration.GetProperty("LoadingImage");

                // Editor
                editorWindowX = ExParsers.ParseDefault(configuration.GetProperty("EditorWindowX"), int.MaxValue);
                editorWindowY = ExParsers.ParseDefault(configuration.GetProperty("EditorWindowY"), int.MaxValue);
                editorWindowW = ExParsers.ParseDefault(configuration.GetProperty("EditorWindowWidth"), int.MaxValue);
                editorWindowH = ExParsers.ParseDefault(configuration.GetProperty("EditorWindowHeight"), int.MaxValue);
                engineWindowX = ExParsers.ParseDefault(configuration.GetProperty("EngineWindowX"), int.MaxValue);
                engineWindowY = ExParsers.ParseDefault(configuration.GetProperty("EngineWindowY"), int.MaxValue);
                engineWindowW = ExParsers.ParseDefault(configuration.GetProperty("EngineWindowWidth"), int.MaxValue);
                engineWindowH = ExParsers.ParseDefault(configuration.GetProperty("EngineWindowHeight"), int.MaxValue);
                debugWindowX = ExParsers.ParseDefault(configuration.GetProperty("DebugWindowX"), int.MaxValue);
                debugWindowY = ExParsers.ParseDefault(configuration.GetProperty("DebugWindowY"), int.MaxValue);
                debugWindowW = ExParsers.ParseDefault(configuration.GetProperty("DebugWindowWidth"), int.MaxValue);
                debugWindowH = ExParsers.ParseDefault(configuration.GetProperty("DebugWindowHeight"), int.MaxValue);


                showItemReferences_F = bool.Parse(configuration.GetProperty("ShowItemReferences"));
                showNPCReferences_F = bool.Parse(configuration.GetProperty("ShowNPCReferences"));
                showStartDialog_F = bool.Parse(configuration.GetProperty("ShowStartDialog"));

                debugOptions = new DebugSettings();
                if (configuration.GetProperty("PaintBoundingAreas") != null)
                    debugOptions.setPaintBoundingAreas(bool.Parse(configuration.GetProperty("PaintBoundingAreas")));
                if (configuration.GetProperty("PaintGrid") != null)
                    debugOptions.setPaintGrid(bool.Parse(configuration.GetProperty("PaintGrid")));
                if (configuration.GetProperty("PaintHotSpots") != null)
                    debugOptions.setPaintHotSpots(bool.Parse(configuration.GetProperty("PaintHotSpots")));

                exportsPath = configuration.GetProperty("ExportsDirectory");
                if (exportsPath != null)
                    ReleaseFolders.setExportsPath(exportsPath);
                reportsPath = configuration.GetProperty("ReportsDirectory");
                if (reportsPath != null)
                    ReleaseFolders.setReportsPath(reportsPath);
                projectsPath = configuration.GetProperty("ProjectsDirectory");
                if (projectsPath != null)
                    ReleaseFolders.setProjectsPath(projectsPath);

                effectSelectorTab = ExParsers.ParseDefault(configuration.GetProperty("EffectSelectorTab"), 0);
                extraProperties = new Properties();
                if (!string.IsNullOrEmpty(configuration.GetProperty("ExtraProperties")))
                {
                    extraProperties.ReloadFromString(configuration.GetProperty("ExtraProperties"));
                }
            }
            catch (FileNotFoundException)
            {
                checkConsistency();
            }
            catch (IOException)
            {
                checkConsistency();
            }
            catch (Exception)
            {
                checkConsistency();
            }

        }

        private void checkConsistency()
        {

            if (debugOptions == null)
                debugOptions = new DebugSettings();

            if (languageFile == null)
            {
                languageFile = ReleaseFolders.getLanguageFilePath(ReleaseFolders.LANGUAGE_ENGLISH);
            }
            if (aboutFile == null)
            {
                aboutFile = "Assets/uAdventure/Resources/" + ReleaseFolders.LANGUAGE_DIR_EDITOR + "/" + ReleaseFolders.getDefaultAboutFilePath();
            }
            if (loadingImage == null)
            {
                loadingImage = "Assets/uAdventure/Resources/" + ReleaseFolders.IMAGE_LOADING_DIR + " / " + Controller.Instance.getDefaultLanguage() + "/Editor2D-Loading.png";
            }
        }

        /**
         * @return the effectSelectorTab
         */
        public static int getEffectSelectorTab()
        {

            return instance.effectSelectorTab;
        }

        /**
         * @param effectSelectorTab
         *            the effectSelectorTab to set
         */
        public static void setEffectSelectorTab(int effectSelectorTab)
        {

            instance.effectSelectorTab = effectSelectorTab;
        }
    }
}
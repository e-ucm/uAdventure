using System;
using UnityEngine;
using System.Collections;
using System.IO;

public class ConfigData {

    private bool showStartDialog_F;

    private RecentFiles recentFiles;

    private static ConfigData instance;

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


    public static void loadFromXML(string configFile)
    {
        instance = new ConfigData(configFile);
    }

    public static void storeToXML()
    {

        // Load the current configuration
        Properties configuration = new Properties(instance.configFile);

        configuration.setProperty("EditorWindowX", instance.editorWindowX.ToString());
        configuration.setProperty("EditorWindowY", instance.editorWindowY.ToString());
        configuration.setProperty("EditorWindowWidth", instance.editorWindowW.ToString());
        configuration.setProperty("EditorWindowHeight", instance.editorWindowH.ToString());

        configuration.setProperty("EngineWindowX", instance.engineWindowX.ToString());
        configuration.setProperty("EngineWindowY", instance.engineWindowY.ToString());
        configuration.setProperty("EngineWindowWidth", instance.engineWindowW.ToString());
        configuration.setProperty("EngineWindowHeight", instance.engineWindowH.ToString());

        configuration.setProperty("DebugWindowX", instance.debugWindowX.ToString());
        configuration.setProperty("DebugWindowY", instance.debugWindowY.ToString());
        configuration.setProperty("DebugWindowWidth", instance.debugWindowW.ToString());
        configuration.setProperty("DebugWindowHeight", instance.debugWindowH.ToString());

        configuration.setProperty("PaintGrid", instance.debugOptions.isPaintGrid().ToString());
        configuration.setProperty("PaintHotSpots", instance.debugOptions.isPaintHotSpots().ToString());
        configuration.setProperty("PaintBoundingAreas", instance.debugOptions.isPaintBoundingAreas().ToString());

        configuration.setProperty("ShowItemReferences", instance.showItemReferences_F.ToString());
        configuration.setProperty("ShowNPCReferences", instance.showNPCReferences_F.ToString());
        configuration.setProperty("ShowAtrezzoReferences", instance.showAtrezzoReferences_F.ToString());
        configuration.setProperty("ShowStartDialog", instance.showStartDialog_F.ToString());
        configuration.setProperty("LanguageFile", instance.languageFile);
        configuration.setProperty("AboutFile", instance.aboutFile);
        configuration.setProperty("LoadingImage", instance.loadingImage);
        if (instance.exportsPath != null)
            configuration.setProperty("ExportsDirectory", instance.exportsPath);
        if (instance.reportsPath != null)
            configuration.setProperty("ReportsDirectory", instance.reportsPath);
        if (instance.projectsPath != null)
            configuration.setProperty("ProjectsDirectory", instance.projectsPath);
        if (instance.projectsPath != null)
            configuration.setProperty("EffectSelectorTab", instance.effectSelectorTab.ToString());

        instance.recentFiles.fillProperties(configuration);

        // Store the configuration into a file
        try
        {
            configuration.Save();
        }
        catch (FileNotFoundException e)
        {
        }
        catch (IOException e)
        {
        }

    }

    private ConfigData(string fileName)
    {
        this.configFile = fileName;
        Properties configuration = new Properties(fileName);
        try
        {
            configuration.reload(fileName);
            languageFile = configuration.getProperty("LanguageFile");
            aboutFile = configuration.getProperty("AboutFile");
            loadingImage = configuration.getProperty("LoadingImage");

            // Editor
            try
            {
                editorWindowX = int.Parse(configuration.getProperty("EditorWindowX"));
                /*if (editorWindowX<0 || editorWindowX>size.width){
                    editorWindowX=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                editorWindowX = int.MaxValue;
            }
            try
            {
                editorWindowY = int.Parse(configuration.getProperty("EditorWindowY"));
                /*if (editorWindowY<0 || editorWindowY>size.height){
                    editorWindowY=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                editorWindowY = int.MaxValue;
            }
            try
            {
                editorWindowW = int.Parse(configuration.getProperty("EditorWindowWidth"));
                /*if (editorWindowW<0 || editorWindowW>size.width){
                    editorWindowW=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                editorWindowW = int.MaxValue;
            }
            try
            {
                editorWindowH = int.Parse(configuration.getProperty("EditorWindowHeight"));
                /*if (editorWindowH<0 || editorWindowH>size.height){
                    editorWindowH=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                editorWindowH = int.MaxValue;
            }

            // Engine
            try
            {
                engineWindowX = int.Parse(configuration.getProperty("EngineWindowX"));
                /*if (engineWindowX<0 || engineWindowX>size.width){
                    engineWindowX=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                engineWindowX = int.MaxValue;
            }
            try
            {
                engineWindowY = int.Parse(configuration.getProperty("EngineWindowY"));
                /*if (engineWindowY<0 || engineWindowY>size.height){
                    engineWindowY=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                engineWindowY = int.MaxValue;
            }
            try
            {
                engineWindowW = int.Parse(configuration.getProperty("EngineWindowWidth"));
                /*if (engineWindowW<0 || engineWindowW>size.width){
                    engineWindowW=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                engineWindowW = int.MaxValue;
            }
            try
            {
                engineWindowH = int.Parse(configuration.getProperty("EngineWindowHeight"));
                /*if (engineWindowH<0 || engineWindowH>size.height){
                    engineWindowH=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                engineWindowH = int.MaxValue;
            }

            // Debug
            try
            {
                debugWindowX = int.Parse(configuration.getProperty("DebugWindowX"));
                /*if (debugWindowX<0 || debugWindowX>size.width){
                    debugWindowX=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                debugWindowX = int.MaxValue;
            }
            try
            {
                debugWindowY = int.Parse(configuration.getProperty("DebugWindowY"));
                /*if (debugWindowY<0 || debugWindowY>size.height){
                    debugWindowY=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                debugWindowY = int.MaxValue;
            }
            try
            {
                debugWindowW = int.Parse(configuration.getProperty("DebugWindowWidth"));
                /*if (debugWindowW<0 || debugWindowW>size.width){
                    debugWindowW=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                debugWindowW = int.MaxValue;
            }
            try
            {
                debugWindowH = int.Parse(configuration.getProperty("DebugWindowHeight"));
                /*if (debugWindowH<0 || debugWindowH>size.height){
                    debugWindowH=int.MaxValue;
                }*/
            }
            catch (FormatException ne)
            {
                debugWindowH = int.MaxValue;
            }


            showItemReferences_F = bool.Parse(configuration.getProperty("ShowItemReferences"));
            showNPCReferences_F = bool.Parse(configuration.getProperty("ShowNPCReferences"));
            showStartDialog_F = bool.Parse(configuration.getProperty("ShowStartDialog"));

            debugOptions = new DebugSettings();
            if (configuration.getProperty("PaintBoundingAreas") != null)
                debugOptions.setPaintBoundingAreas(bool.Parse(configuration.getProperty("PaintBoundingAreas")));
            if (configuration.getProperty("PaintGrid") != null)
                debugOptions.setPaintGrid(bool.Parse(configuration.getProperty("PaintGrid")));
            if (configuration.getProperty("PaintHotSpots") != null)
                debugOptions.setPaintHotSpots(bool.Parse(configuration.getProperty("PaintHotSpots")));

            exportsPath = configuration.getProperty("ExportsDirectory");
            if (exportsPath != null)
                ReleaseFolders.setExportsPath(exportsPath);
            reportsPath = configuration.getProperty("ReportsDirectory");
            if (reportsPath != null)
                ReleaseFolders.setReportsPath(reportsPath);
            projectsPath = configuration.getProperty("ProjectsDirectory");
            if (projectsPath != null)
                ReleaseFolders.setProjectsPath(projectsPath);
            try
            {
                effectSelectorTab = int.Parse(configuration.getProperty("EffectSelectorTab"));
            }
            catch (Exception e)
            {
                effectSelectorTab = 0;
            }

            recentFiles = new RecentFiles(configuration);
        }
        catch (FileNotFoundException e)
        {
            checkConsistency();
        }
        catch (IOException e)
        {
            checkConsistency();
        }
        catch (Exception e)
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
            aboutFile = "Assets/Resources/" + ReleaseFolders.LANGUAGE_DIR_EDITOR + "/" + ReleaseFolders.getDefaultAboutFilePath();
        }
        if (loadingImage == null)
        {
            loadingImage = "Assets/Resources/" + ReleaseFolders.IMAGE_LOADING_DIR + " / " + Controller.getInstance().getDefaultLanguage() + "/Editor2D-Loading.png";
        }
        if (exportsPath == null)
        {

        }
        if (projectsPath == null)
        {

        }
        if (reportsPath == null)
        {

        }
        if (recentFiles == null)
        {
            recentFiles = new RecentFiles(new Properties(instance.configFile));
        }

    }

    public static void fileLoaded(string file)
    {

        instance.recentFiles.fileLoaded(file);
    }

    public static string[][] getRecentFilesInfo(int r)
    {

        return instance.recentFiles.getRecentFilesInfo(r);
    }

    public static string[][] getRecentFilesInfo(int l, int r)
    {

        return instance.recentFiles.getRecentFilesInfo(l, r);
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

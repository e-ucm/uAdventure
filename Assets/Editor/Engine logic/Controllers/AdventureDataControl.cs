using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class holds all the information of the adventure, including the chapters
 * and the configuration of the HUD.
 */
public class AdventureDataControl {


    /**
     * The whole data of the adventure
     */
    private AdventureData adventureData;

    ///**
    // * Controller for LOM data (only required when exporting games to LOM)
    // */
    //private LOMDataControl lomController;

    ///**
    // * Controller for IMS data (only required when exporting games to SCORM)
    // */
    //private IMSDataControl imsController;

    ///**
    // * Controller for LOM-ES data (require to export games as ODE)
    // */
    //private LOMESDataControl lomesController;

    /**
     * Assessment file data controller
     */
    //private AssessmentProfilesDataControl assessmentProfilesDataControl;
    /**
     * Adaptation file data controller
     */
    //private AdaptationProfilesDataControl adaptationProfilesDataControl;
    /**
     * Constructs the data control with the adventureData
     */
    public AdventureDataControl(AdventureData data):this()
    {
        adventureData = data;
        checkContextualButtons();
    }

    /**
     * Empty constructor. Sets all values to null.
     */
    public AdventureDataControl()
    {
        adventureData = new AdventureData();
    }

    /**
     * Constructor which creates an adventure data with default title and
     * description, traditional GUI and one empty chapter (with a scene).
     * 
     * @param adventureTitle
     *            Default title for the adventure
     * @param chapterTitle
     *            Default title for the chapter
     * @param sceneId
     *            Default identifier for the scene
     */
    public AdventureDataControl(string adventureTitle, string chapterTitle, string sceneId, int playerMode)
    {

        adventureData = new AdventureData();
        adventureData.setTitle(adventureTitle);
        adventureData.setDescription("");
        adventureData.setGUIType(DescriptorData.GUI_CONTEXTUAL);
        adventureData.setPlayerMode(playerMode);
        adventureData.addChapter(new Chapter(chapterTitle, sceneId));
    }

    public AdventureDataControl(string adventureTitle, string chapterTitle, string sceneId) :
        this(adventureTitle, chapterTitle, sceneId, DescriptorData.MODE_PLAYER_3RDPERSON)
    {

    }

    /**
     * Constructor gith given parameters.
     * 
     * @param title
     *            Title of the adventure
     * @param description
     *            Description of the adventure
     * @param guiType
     *            Type of the GUI
     * @param chapters
     *            Chapters of the adventure
     */
    public AdventureDataControl(string title, string description, List<Chapter> chapters)
    {

        adventureData = new AdventureData();
        adventureData.setTitle(title);
        adventureData.setDescription(description);
        adventureData.setGUIType(DescriptorData.GUI_TRADITIONAL);
        adventureData.setChapters(chapters);
        adventureData.setGraphicConfig(DescriptorData.GRAPHICS_WINDOWED);
        adventureData.setPlayerMode(DescriptorData.MODE_PLAYER_3RDPERSON);
    }

    public bool isCursorTypeAllowed(string type)
    {

        return isCursorTypeAllowed(DescriptorData.getCursorTypeIndex(type));
    }

    public bool isCursorTypeAllowed(int type)
    {

        return DescriptorData.typeAllowed[adventureData.getGUIType()][type];
    }

    /**
     * Returns the title of the adventure
     * 
     * @return Adventure's title
     */
    public string getTitle()
    {

        return adventureData.getTitle();
    }

    /**
     * Returns the description of the adventure.
     * 
     * @return Adventure's description
     */
    public string getDescription()
    {

        return adventureData.getDescription();
    }

    /**
     * Returns the GUI type of the adventure.
     * 
     * @return Adventure's GUI type
     */
    public int getGUIType()
    {

        return adventureData.getGUIType();
    }

    /**
     * Returns the list of chapters of the adventure.
     * 
     * @return Adventure's chapters list
     */
    public List<Chapter> getChapters()
    {

        return adventureData.getChapters();
    }

    /**
     * Returns settings for keyboard navigation
     * @return true if keyboard navigation (using arrows) is enabled, false if it is disabled (default behaviour)
     */
    public bool isKeyboardNavigationEnabled()
    {
        return adventureData.isKeyboardNavigationEnabled();
    }

    ///**
    // * Enables/Disables keyboard navigation - by default is set to false
    // * @param enabled
    // */
    //public bool setKeyboardNavigation(bool enabled)
    //{
    //    return Controller.getInstance().addTool(new ChangeBooleanValueTool(adventureData, enabled, "isKeyboardNavigationEnabled", "setKeyboardNavigation"));
    //}

    /**
     * Sets the title of the adventure.
     * 
     * @param title
     *            New title for the adventure
     */
    public void setTitle(string title)
    {

        Tool tool = new ChangeTitleTool(adventureData, title);
        Controller.getInstance().addTool(tool);
    }

    /**
     * Sets the description of the adventure.
     * 
     * @param description
     *            New description for the adventure
     */
    public void setDescription(string description)
    {

        Tool tool = new ChangeDescriptionTool(adventureData, description);
        Controller.getInstance().addTool(tool);
    }

    /**
     * Shows the GUI style selection dialog.
     */
    public void showGUIStylesDialog()
    {

        // Show the dialog
       // new GUIStylesDialog(adventureData.getGUIType());
    }

    /**
     * @return the playerMode
     */
    public int getPlayerMode()
    {

        return adventureData.getPlayerMode();
    }

    /**
     * @param playerMode
     *            the playerMode to set
     */
    public void setPlayerMode(int playerMode)
    {

        Tool tool = new ChangeIntegerValueTool(adventureData, playerMode, "getPlayerMode", "setPlayerMode");
        Controller.getInstance().addTool(tool);
    }

    public List<CustomCursor> getCursors()
    {

        return adventureData.getCursors();
    }

    public List<CustomButton> getButtons()
    {

        return adventureData.getButtons();
    }

    public List<CustomArrow> getArrows()
    {

        return adventureData.getArrows();
    }

    public string getCursorPath(string type)
    {

        foreach (CustomCursor cursor in adventureData.getCursors())
        {
            if (cursor.getType().Equals(type))
            {
                return cursor.getPath();
            }
        }
        return null;
    }

    public string getCursorPath(int type)
    {

        return getCursorPath(DescriptorData.getCursorTypeString(type));
    }

    public void deleteCursor(int type)
    {

        string typeS = DescriptorData.getCursorTypeString(type);
        int position = -1;
        for (int i = 0; i < adventureData.getCursors().Count; i++)
        {
            if (adventureData.getCursors()[i].getType().Equals(typeS))
            {
                position = i;
                break;
            }
        }
        if (position >= 0)
        {
            Controller.getInstance().addTool(new DeleteCursorTool(adventureData, position));
        }
    }

    public void editCursorPath(int t)
    {

        //try
        //{
            Controller.getInstance().addTool(new SelectCursorPathTool(adventureData, t));
        //}
        //catch (CloneNotSupportedException e)
        //{
        //    ReportDialog.GenerateErrorReport(e, false, "Could not clone cursor-adventureData");
        //}
    }

    public string getArrowPath(string type)
    {

        foreach (CustomArrow arrow in adventureData.getArrows())
        {
            if (arrow.getType().Equals(type))
            {
                return arrow.getPath();
            }
        }
        return null;
    }

    public void deleteArrow(string type)
    {

        int position = -1;
        for (int i = 0; i < adventureData.getArrows().Count; i++)
        {
            if (adventureData.getArrows()[i].getType().Equals(type))
            {
                position = i;
                break;
            }
        }
        if (position >= 0)
        {
            Controller.getInstance().addTool(new DeleteArrowTool(adventureData, position));
        }
    }

    ///**
    // * @return the lomController
    // */
    //public LOMDataControl getLomController()
    //{

    //    return lomController;
    //}

    ///**
    // * @param lomController
    // *            the lomController to set
    // */
    //public void setLomController(LOMDataControl lomController)
    //{

    //    this.lomController = lomController;
    //}

    ///**
    // * @return the imsController
    // */
    //public IMSDataControl getImsController()
    //{

    //    return imsController;
    //}

    ///**
    // * @return the lomesController
    // */
    //public LOMESDataControl getLOMESController()
    //{

    //    return lomesController;
    //}

    ///**
    // * @param imsController
    // *            the imsController to set
    // */
    //public void setImsController(IMSDataControl imsController)
    //{

    //    this.imsController = imsController;
    //}

    /**
     * @return the assessmentRulesListDataControl
     */
    /*public AssessmentProfilesDataControl getAssessmentRulesListDataControl( ) {
    	return assessmentProfilesDataControl;
    }*/

    /**
     * @return the adaptationRulesListDataControl
     */
    /*public AdaptationProfilesDataControl getAdaptationRulesListDataControl( ) {
    	return adaptationProfilesDataControl;
    }*/

    public bool isCommentaries()
    {

        return adventureData.isCommentaries();
    }

    public void setCommentaries(bool commentaries)
    {

        Tool tool = new ChangeBooleanValueTool(adventureData, commentaries, "isCommentaries", "setCommentaries");
        Controller.getInstance().addTool(tool);
    }

    public bool isKeepShowing()
    {

        return adventureData.isKeepShowing();
    }

    public DescriptorData.DefaultClickAction getDefaultClickAction()
    {
        return adventureData.getDefaultClickAction();
    }

    public DescriptorData.Perspective getPerspective()
    {
        return adventureData.getPerspective();
    }

    public void setKeepShowing(bool keepShowing)
    {

        Tool tool = new ChangeBooleanValueTool(adventureData, keepShowing, "isKeepShowing", "setKeepShowing");
        Controller.getInstance().addTool(tool);
    }

    public int getGraphicConfig()
    {

        return adventureData.getGraphicConfig();
    }

    public void setGraphicConfig(int graphicConfig)
    {

        Tool tool = new ChangeIntegerValueTool(adventureData, graphicConfig, "getGraphicConfig", "setGraphicConfig");
        Controller.getInstance().addTool(tool);
    }

    /**
     * @return the adventureData
     */
    public AdventureData getAdventureData()
    {

        return adventureData;
    }

    public string getButtonPath(string action, string type)
    {

        foreach (CustomButton cb in adventureData.getButtons())
        {
            if (cb.getType().Equals(type) && cb.getAction().Equals(action))
                return cb.getPath();
        }
        return null;
    }

    public void deleteButton(string action, string type)
    {

        Controller.getInstance().addTool(new DeleteButtonTool(adventureData, action, type));
    }

    public void editButtonPath(string action, string type)
    {

        //try
        //{
            Controller.getInstance().addTool(new SelectButtonTool(adventureData, action, type));
        //}
        //catch (CloneNotSupportedException e)
        //{
        //    ReportDialog.GenerateErrorReport(e, false, "Could not clone resources: buttons");
        //}
    }

    public void editArrowPath(string type)
    {
        //try
        //{
            Controller.getInstance().addTool(new SelectArrowTool(adventureData, type));
        //}
        //catch (CloneNotSupportedException e)
        //{
        //    ReportDialog.GenerateErrorReport(e, false, "Could not clone resources: arrows");
        //}
    }

    public int getInventoryPosition()
    {

        return adventureData.getInventoryPosition();
    }

    public void setInventoryPosition(int inventoryPosition)
    {

        Controller.getInstance().addTool(new ChangeIntegerValueTool(adventureData, inventoryPosition, "getInventoryPosition", "setInventoryPosition"));
    }

    public int countAssetReferences(string assetPath)
    {

        return adventureData.countAssetReferences(assetPath);
    }

    public void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        adventureData.getAssetReferences(assetPaths, assetTypes);
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

        adventureData.deleteAssetReferences(assetPath);
    }

    public void setGUIStyleDialog(int optionSelected)
    {

        Tool tool = new ChangeIntegerValueTool(adventureData, optionSelected, "getGUIType", "setGUIType");
        Controller.getInstance().addTool(tool);

    }

    public void setDefaultClickAction(DescriptorData.DefaultClickAction defaultClickAction)
    {

        adventureData.setDeafultClickAction(defaultClickAction);

    }

    public void setPerspective(DescriptorData.Perspective perspective)
    {
        adventureData.setPerspective(perspective);
    }

    public void setDragBehaviour(DescriptorData.DragBehaviour dragBehaviour)
    {
        adventureData.setDragBehaviour(dragBehaviour);
    }

    public DescriptorData.DragBehaviour getDragBehaviour()
    {
        return adventureData.getDragBehaviour();
    }

    /**
     * Checks if the user-grab button was defined, which is deprecated. In this case, buttons "use", "use-with", "grab", and "give-to" are set
     * using the same configuration.
     * 
     * This method should be invoked when the AdventureDataControl is set with a given adventureData
     * 
     * This method was added in version 1.4
     */
    private void checkContextualButtons()
    {
        checkContextualButtons(DescriptorData.USE_BUTTON);
        checkContextualButtons(DescriptorData.GRAB_BUTTON);
        checkContextualButtons(DescriptorData.GIVETO_BUTTON);
        checkContextualButtons(DescriptorData.USEWITH_BUTTON);
        string useGrabPath = adventureData.getButtonPathFromEditor(DescriptorData.USE_GRAB_BUTTON, DescriptorData.NORMAL_BUTTON);
        if (useGrabPath != null && !useGrabPath.Equals(""))
        {
            foreach (CustomButton button in adventureData.getButtons())
            {
                if (button.getAction().Equals(DescriptorData.USE_GRAB_BUTTON) &&
                        button.getType().Equals(DescriptorData.NORMAL_BUTTON))
                {
                    adventureData.getButtons().Remove(button);
                    break;
                }
            }
        }

        useGrabPath = adventureData.getButtonPathFromEditor(DescriptorData.USE_GRAB_BUTTON, DescriptorData.HIGHLIGHTED_BUTTON);
        if (useGrabPath != null && !useGrabPath.Equals(""))
        {
            foreach (CustomButton button in adventureData.getButtons())
            {
                if (button.getAction().Equals(DescriptorData.USE_GRAB_BUTTON) &&
                        button.getType().Equals(DescriptorData.HIGHLIGHTED_BUTTON))
                {
                    adventureData.getButtons().Remove(button);
                    break;
                }
            }
        }

    }

    private void checkContextualButtons(string action)
    {
        checkContextualButtons(action, DescriptorData.NORMAL_BUTTON);
        checkContextualButtons(action, DescriptorData.HIGHLIGHTED_BUTTON);
    }

    private void checkContextualButtons(string action, string type)
    {
        string useGrabPath = adventureData.getButtonPathFromEditor(DescriptorData.USE_GRAB_BUTTON, type);
        if (useGrabPath != null && !useGrabPath.Equals(""))
        {
            if (adventureData.getButtonPathFromEditor(action, type) == null || adventureData.getButtonPathFromEditor(action, type).Equals(""))
            {
                adventureData.addButton(action, type, useGrabPath);
            }
        }
    }
}

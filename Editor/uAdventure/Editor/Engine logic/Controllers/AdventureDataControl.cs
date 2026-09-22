using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    /**
     * This class holds all the information of the adventure, including the chapters
     * and the configuration of the HUD.
     */
    public class AdventureDataControl
    {
        
        /**
         * The whole data of the adventure
         */
        private readonly AdventureData adventureData;

        public AdventureDataControl(AdventureData data) : this()
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
            var chapter = new Chapter(chapterTitle, sceneId);
            chapter.getObjects<Scene>().Add(new Scene(sceneId));
            adventureData.addChapter(chapter);
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

        public string getApplicationIdentifier()
        {
            return adventureData.getApplicationIdentifier();
        }
        public void setApplicationIdentifier(string applicationIdentifier)
        {
            Controller.Instance.AddTool(new ChangeStringValueTool(adventureData, applicationIdentifier, "getApplicationIdentifier", "setApplicationIdentifier"));
        }

        public bool isCursorTypeAllowed(string type)
        {
            return isCursorTypeAllowed(DescriptorData.getCursorTypeIndex(type));
        }

        public bool isCursorTypeAllowed(int type)
        {
            return DescriptorData.typeAllowed[adventureData.getGUIType()][type];
        }

        public bool isShowSaveLoad()
        {
            return adventureData.isShowSaveLoad();
        }

        public void setShowSaveLoad(bool showSaveLoad)
        {
            Controller.Instance.AddTool(new ChangeBooleanValueTool(adventureData, showSaveLoad, "isShowSaveLoad", "setShowSaveLoad"));
        }

        public bool isShowReset()
        {
            return adventureData.isShowReset();
        }

        public void setShowReset(bool showReset)
        {
            Controller.Instance.AddTool(new ChangeBooleanValueTool(adventureData, showReset, "isShowReset", "setShowReset"));
        }

        public bool isAutoSave()
        {
            return adventureData.isAutoSave();
        }

        public void setAutoSave(bool autoSave)
        {
            Controller.Instance.AddTool(new ChangeBooleanValueTool(adventureData, autoSave, "isAutoSave", "setAutoSave"));
        }

        public bool isSaveOnSuspend()
        {
            return adventureData.isSaveOnSuspend();
        }

        public void setSaveOnSuspend(bool autoSave)
        {
            Controller.Instance.AddTool(new ChangeBooleanValueTool(adventureData, autoSave, "isSaveOnSuspend", "setSaveOnSuspend"));
        }

        public bool isRestoreAfterOpen()
        {
            return adventureData.isRestoreAfterOpen();
        }

        public void setRestoreAfterOpen(bool autoSave)
        {
            Controller.Instance.AddTool(new ChangeBooleanValueTool(adventureData, autoSave, "isRestoreAfterOpen", "setRestoreAfterOpen"));
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

        /**
         * Enables/Disables keyboard navigation - by default is set to false
         * @param enabled
         */
        public bool setKeyboardNavigation(bool enabled)
        {
            return Controller.Instance.AddTool(new ChangeBooleanValueTool(adventureData, enabled, "isKeyboardNavigationEnabled", "setKeyboardNavigation"));
        }

        /**
         * Sets the title of the adventure.
         * 
         * @param title
         *            New title for the adventure
         */
        public void setTitle(string title)
        {
            Tool tool = new ChangeTitleTool(adventureData, title);
            Controller.Instance.AddTool(tool);
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
            Controller.Instance.AddTool(tool);
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
            Tool tool = new ChangeValueTool<AdventureData, int>(adventureData, playerMode, "getPlayerMode", "setPlayerMode");
            Controller.Instance.AddTool(tool);
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
                Controller.Instance.AddTool(new DeleteCursorTool(adventureData, position));
            }
        }

        public void editCursorPath(string t, string newPath)
        {
            var cursor = getCursors().Find(c => c.getType() == t);
            if(cursor == null)
            {
                cursor = new CustomCursor(t, getCursorPath(t));
                getCursors().Add(cursor);
            }

            Controller.Instance.AddTool(new ChangeValueTool<CustomCursor, string>(cursor, newPath, "getPath", "setPath"));
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
                Controller.Instance.AddTool(new DeleteArrowTool(adventureData, position));
            }
        }

        public bool isCommentaries()
        {

            return adventureData.isCommentaries();
        }

        public void setCommentaries(bool commentaries)
        {

            Tool tool = new ChangeBooleanValueTool(adventureData, commentaries, "isCommentaries", "setCommentaries");
            Controller.Instance.AddTool(tool);
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
            Controller.Instance.AddTool(tool);
        }

        public int getGraphicConfig()
        {

            return adventureData.getGraphicConfig();
        }

        public void setGraphicConfig(int graphicConfig)
        {

            Tool tool = new ChangeValueTool<AdventureData, int>(adventureData, graphicConfig, "getGraphicConfig", "setGraphicConfig");
            Controller.Instance.AddTool(tool);
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

            Controller.Instance.AddTool(new DeleteButtonTool(adventureData, action, type));
        }

        public void editButtonPath(string action, string type, string newPath)
        {
            var button = getButtons().Find(c => c.getAction() == action && c.getType() == type);
            if (button == null)
            {
                button = new CustomButton(action, type, adventureData.getDefaultButtonPath(action, type));
                getButtons().Add(button);
            }

            Controller.Instance.AddTool(new ChangeValueTool<CustomButton, string>(button, newPath, "getPath", "setPath"));
        }

        public void editArrowPath(string type)
        {
            Controller.Instance.AddTool(new SelectArrowTool(adventureData, type));
        }

        public int getInventoryPosition()
        {

            return adventureData.getInventoryPosition();
        }

        public void setInventoryPosition(int inventoryPosition)
        {

            Controller.Instance.AddTool(new ChangeValueTool<AdventureData, int>(adventureData, inventoryPosition, "getInventoryPosition", "setInventoryPosition"));
        }

        public Vector2 getInventoryCoords()
        {

            return adventureData.getInventoryCoords();
        }

        public float getInventoryScale()
        {

            return adventureData.getInventoryScale();
        }

        public void setInventoryScale(float inventoryScale)
        {

            Controller.Instance.AddTool(new ChangeValueTool<AdventureData, float>(adventureData, inventoryScale, "getInventoryScale", "setInventoryScale"));
        }

        public void setInventoryImage(string inventoryImage)
        {

            Controller.Instance.AddTool(new ChangeValueTool<AdventureData, string>(adventureData, inventoryImage, "getInventoryImage", "setInventoryImage"));
        }

        public string getInventoryImage()
        {

            return adventureData.getInventoryImage();
        }

        public void setInventoryCoords(Vector2 inventoryCoords)
        {

            Controller.Instance.AddTool(new ChangeValueTool<AdventureData, Vector2>(adventureData, inventoryCoords, "getInventoryCoords", "setInventoryCoords"));
        }

        public IMS.MD.v1p2.lomType getImsCPMetadata()
        {

            return adventureData.getImsCPMetadata();
        }

        public void setImsCPMetadata(IMS.MD.v1p2.lomType metadata)
        {

            Controller.Instance.AddTool(new ChangeValueTool<AdventureData, IMS.MD.v1p2.lomType>(adventureData, metadata, "getImsCPMetadata", "setImsCPMetadata"));
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

            Tool tool = new ChangeValueTool<AdventureData, int>(adventureData, optionSelected, "getGUIType", "setGUIType");
            Controller.Instance.AddTool(tool);

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

        public string getVersionNumber()
        {
            return adventureData.getVersionNumber();
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
            var hasUseGrabPath = !string.IsNullOrEmpty(useGrabPath);
            var hasCustomButtonForUseGrabPath = hasUseGrabPath && string.IsNullOrEmpty(adventureData.getButtonPathFromEditor(action, type));
            if (hasUseGrabPath && hasCustomButtonForUseGrabPath)
            {
                adventureData.addButton(action, type, useGrabPath);
            }
        }
    }
}
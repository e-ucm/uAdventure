using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IMS.MD.v1p2;

namespace uAdventure.Core
{
    /**
     * Stores the description of the eAdventure file
     */
    public class DescriptorData : Described, Titled, ICloneable
    {

        public const string DEFAULT_CURSOR = "default";

        public const string USE_CURSOR = "use";

        public const string LOOK_CURSOR = "look";

        public const string EXAMINE_CURSOR = "examine";

        public const string TALK_CURSOR = "talk";

        public const string GRAB_CURSOR = "grab";

        public const string GIVE_CURSOR = "give";

        public const string EXIT_CURSOR = "exit";

        public const string CURSOR_OVER = "over";

        public const string CURSOR_ACTION = "action";

        public const string TALK_BUTTON = "talk";

        public const string USE_GRAB_BUTTON = "use-grab";

        // Specific buttons for grab, use, use-with and give-to were added in eAd1.4
        // Before that, a use-grab generic button was used. It is kept for backwards compatibility
        public const string GRAB_BUTTON = "grab";

        public const string USE_BUTTON = "use";

        public const string USEWITH_BUTTON = "use-with";

        public const string GIVETO_BUTTON = "give-to";

        public const string DRAGTO_BUTTON = "drag-to";
        // 

        public const string EXAMINE_BUTTON = "examine";

        public const string HIGHLIGHTED_BUTTON = "highlighted";
        //ONLY USE IT IN editor.ButtonsPanel
        public const string SOUND_PATH = "sound";
        public const string SOUND_PATH_ARROW_RIGHT = "sound-right";
        public const string SOUND_PATH_ARROW_LEFT = "sound-left";

        public const string NORMAL_BUTTON = "normal";

        public const string PRESSED_BUTTON = "pressed";

        public const string NORMAL_ARROW_RIGHT = "normalright";

        public const string NORMAL_ARROW_LEFT = "normalleft";

        public const string HIGHLIGHTED_ARROW_RIGHT = "highlightedright";

        public const string HIGHLIGHTED_ARROW_LEFT = "highlightedleft";

        public const int INVENTORY_NONE = 0;

        public const int INVENTORY_TOP_BOTTOM = 1;

        public const int INVENTORY_TOP = 2;

        public const int INVENTORY_BOTTOM = 3;

        public const int INVENTORY_FIXED_TOP = 4;

        public const int INVENTORY_FIXED_BOTTOM = 5;

        public const int INVENTORY_ICON_FREEPOS = 6;


        public enum DefaultClickAction
        {
            SHOW_DETAILS, SHOW_ACTIONS
        }

        public enum Perspective
        {
            REGULAR, ISOMETRIC
        }

        public enum DragBehaviour
        {
            IGNORE_NON_TARGETS, CONSIDER_NON_TARGETS
        }

        public static string getCursorTypeString(int index)
        {

            switch (index)
            {
                case 0:
                    return DEFAULT_CURSOR;
                case 1:
                    return CURSOR_OVER;
                case 2:
                    return CURSOR_ACTION;
                case 3:
                    return EXIT_CURSOR;
                case 4:
                    return USE_CURSOR;
                case 5:
                    return LOOK_CURSOR;
                case 6:
                    return EXAMINE_CURSOR;
                case 7:
                    return TALK_CURSOR;
                case 8:
                    return GRAB_CURSOR;
                case 9:
                    return GIVE_CURSOR;
                default:
                    return null;
            }
        }

        public static int getCursorTypeIndex(string type)
        {

            if (type.Equals(DEFAULT_CURSOR))
            {
                return 0;
            }
            else if (type.Equals(USE_CURSOR))
            {
                return 4;
            }
            else if (type.Equals(LOOK_CURSOR))
            {
                return 5;
            }
            else if (type.Equals(EXAMINE_CURSOR))
            {
                return 6;
            }
            else if (type.Equals(TALK_CURSOR))
            {
                return 7;
            }
            else if (type.Equals(GRAB_CURSOR))
            {
                return 8;
            }
            else if (type.Equals(GIVE_CURSOR))
            {
                return 9;
            }
            else if (type.Equals(EXIT_CURSOR))
            {
                return 3;
            }
            else if (type.Equals(CURSOR_OVER))
            {
                return 1;
            }
            else if (type.Equals(CURSOR_ACTION))
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        // the deleted cursors are related with the traditional hud, which has been removed 
        private static string[] cursorTypes = { DEFAULT_CURSOR, CURSOR_OVER, CURSOR_ACTION, EXIT_CURSOR/*, USE_CURSOR, LOOK_CURSOR, EXAMINE_CURSOR, TALK_CURSOR, GRAB_CURSOR, GIVE_CURSOR */
  };

        public static string[] getCursorTypes()
        {

            return cursorTypes;
        }

        /*
         * Action types, buttonTypes & arrowTypes are only used in the editor, to create the gui customization dialog 
         * automatically. So: order matters!
         */
        private static readonly string[] actionTypes = { EXAMINE_BUTTON, TALK_BUTTON, /*USE_GRAB_BUTTON, */USE_BUTTON, GRAB_BUTTON, USEWITH_BUTTON, GIVETO_BUTTON, DRAGTO_BUTTON };

        public static string[] getActionTypes()
        {

            return actionTypes;
        }

        private static readonly string[] buttonTypes = { NORMAL_BUTTON, HIGHLIGHTED_BUTTON, SOUND_PATH/*, PRESSED_BUTTON */};

        public static string[] getButtonTypes()
        {

            return buttonTypes;
        }

        private static readonly string[] arrowTypes = { NORMAL_ARROW_RIGHT, NORMAL_ARROW_LEFT, HIGHLIGHTED_ARROW_RIGHT, HIGHLIGHTED_ARROW_LEFT, SOUND_PATH_ARROW_RIGHT, SOUND_PATH_ARROW_LEFT };

        public static string[] getArrowTypes()
        {

            return arrowTypes;
        }

        private static readonly int[] inventoryTypes = { INVENTORY_NONE, INVENTORY_TOP_BOTTOM, INVENTORY_TOP, INVENTORY_BOTTOM, INVENTORY_FIXED_TOP, INVENTORY_FIXED_BOTTOM, INVENTORY_ICON_FREEPOS};

        public static int[] getInventoryTypes()
        {

            return inventoryTypes;
        }

        public static readonly bool[][]
            typeAllowed = new bool[][]{
        //TRADITIONAL GUI
        new bool[]{ true, false, false, true, true, true, true, true, true, true },
        //CONTEXTUAL GUI
        new bool[]{ true, true, true, true, false, false, false, false, false, false } };

        /**
         * Constant for traditional GUI.
         */
        public const int GUI_TRADITIONAL = 0;

        /**
         * Constant for contextual GUI.
         */
        public const int GUI_CONTEXTUAL = 1;

        public const int GRAPHICS_WINDOWED = 0;

        public const int GRAPHICS_BLACKBKG = 1;

        public const int GRAPHICS_FULLSCREEN = 2;

        /**
         * Constant for 1st person adventure mode
         */
        public const int MODE_PLAYER_1STPERSON = 0;

        /**
         * Constant for 3rd person adventure mode
         */
        public const int MODE_PLAYER_3RDPERSON = 1;

        /**
         * Title of the adventure.
         */
        protected string title;

        /**
         * Description of the adventure.
         */
        protected string description;

        /**
         * Description of the adventure.
         */
        protected string applicationIdentifier;

        /**
         * Type of the GUI (Traditional or contextual)
         */
        protected int guiType;

        /**
         * Default graphic configuration (fullscreen, windowed, etc)
         */
        private int graphicConfig;

        /**
         * Adventure mode (1st person/3rd person)
         */
        protected int playerMode;

        /**
         * Stores if the GUI's graphics are customized
         */
        protected bool guiCustomized;

        /**
         * List of contents of the game
         */
        protected List<ChapterSummary> contents;

        /**
         * List of custom cursors
         */
        protected List<CustomCursor> cursors;

        /**
         * List of custom buttons
         */
        protected List<CustomButton> buttons;

        protected List<CustomArrow> arrows;

        /**
         * This flag tells if the adventure should show automatic commentaries.
         */
        protected bool commentaries = false;

        /**
         * This flag tell if the conversations in this adventure will stop the conversation lines until user skip them
         */
        protected bool keepShowing = false;

        /**
         * The name of the player, only used when reports are send by e-mail.
         */
        protected string playerName = "";

        protected int inventoryPosition = INVENTORY_TOP_BOTTOM;

        protected Vector2 inventoryCoords = new Vector2(675, 550);

        protected float inventoryScale = 0.2f;

        protected string inventoryImage;

        /**
         * The version number of the current game/proyect
         */
        protected string versionNumber;

        /**
         * This is not store in physical descriptor xml file.
         * 
         * Is used to only allow to load a saved game in the same project or .ead
         * game
         */
        protected string projectName;

        protected DefaultClickAction defaultClickAction;

        protected Perspective perspective;

        protected DragBehaviour dragBehaviour;

        protected bool keyboardNavigationEnabled;

        protected bool showSaveLoad = true;

        protected bool showReset = true;

        protected bool autoSave;

        protected bool saveOnSuspend;

        protected bool restoreAfterOpen;

        private IMS.MD.v1p2.lomType imsCPMetadata;

        /**
         * Constant for identify when a game is executed from engine
         */
        public const string ENGINE_EXECUTION = "engine";

        /**
         * Constructor
         */
        public DescriptorData()
        {

            contents = new List<ChapterSummary>();
            cursors = new List<CustomCursor>();
            buttons = new List<CustomButton>();
            arrows = new List<CustomArrow>();
            title = null;
            description = null;
            guiType = -1;
            defaultClickAction = DefaultClickAction.SHOW_DETAILS;
            perspective = Perspective.REGULAR;
            dragBehaviour = DragBehaviour.CONSIDER_NON_TARGETS;
            playerMode = MODE_PLAYER_1STPERSON;
            graphicConfig = GRAPHICS_WINDOWED;
            projectName = ENGINE_EXECUTION;
            versionNumber = "0";
            keyboardNavigationEnabled = false;
        }

        /**
         * Returns the title of the adventure
         * 
         * @return Adventure's title
         */
        public string getTitle()
        {

            return title;
        }

        /**
         * Returns the description of the adventure.
         * 
         * @return Adventure's description
         */
        public string getDescription()
        {

            return description;
        }

        /**
         * Returns the GUI type of the adventure.
         * 
         * @return Adventure's GUI type
         */
        public int getGUIType()
        {

            return guiType;
        }

        /**
         * Sets the title of the adventure.
         * 
         * @param title
         *            New title for the adventure
         */
        public void setTitle(string title)
        {

            this.title = title;
        }

        /**
         * Sets the description of the adventure.
         * 
         * @param description
         *            New description for the adventure
         */
        public void setDescription(string description)
        {

            this.description = description;
        }

        /**
         * Sets the GUI type of the adventure.
         * 
         * @param guiType
         *            New GUI type for the adventure
         */
        public void setGUIType(int guiType)
        {

            this.guiType = guiType;
        }

        /**
         * @return the playerMode
         */
        public int getPlayerMode()
        {

            return playerMode;
        }

        /**
         * @param playerMode
         *            the playerMode to set
         */
        public void setPlayerMode(int playerMode)
        {

            this.playerMode = playerMode;
        }

        /**
         * Returns whether the GUI is customized
         * 
         * @return True if the GUI is customized, false otherwise
         */
        public bool isGUICustomized()
        {

            return guiCustomized;
        }

        /**
         * Sets the parameters of the GUI
         * 
         * @param guiType
         *            Type of the GUI
         * @param guiCustomized
         *            False if the GUI should be customized, false otherwise
         */
        public void setGUI(int guiType, bool guiCustomized)
        {

            this.guiType = guiType;
            this.guiCustomized = guiCustomized;
        }

        /**
         * Returns the list of chapters of the game
         * 
         * @return List of chapters of the game
         */
        public virtual List<ChapterSummary> getChapterSummaries()
        {

            return contents;
        }

        /**
         * Adds a new chapter to the list
         * 
         * @param chapter
         *            Chapter to be added
         */
        public virtual void addChapterSummary(ChapterSummary chapter)
        {

            contents.Add(chapter);
        }

        public void addCursor(CustomCursor cursor)
        {

            cursors.Add(cursor);
            this.guiCustomized = true;
        }

        public List<CustomCursor> getCursors()
        {

            return cursors;
        }

        public void addCursor(string type, string path)
        {

            addCursor(new CustomCursor(type, path));
        }

        public string getCursorPath(string type)
        {

            foreach (CustomCursor cursor in cursors)
            {
                if (cursor.getType().Equals(type))
                {
                    return cursor.getPath();
                }
            }
            return null;
        }

        public void addButton(CustomButton button)
        {

            buttons.Add(button);
            this.guiCustomized = true;
        }

        public List<CustomButton> getButtons()
        {

            return buttons;
        }

        public void addButton(string action, string type, string path)
        {
            CustomButton cb = new CustomButton(action, type, path);
            addButton(cb);
        }

        public string getDefaultButtonPath(string action, string type)
        {
            if (getActionTypes().Contains(action) && getButtonTypes().Contains(type))
            {

                var camelCaseAction = action.Split('-').Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)).Aggregate((w1, w2) => w1 + w2);
                var talktoFix = camelCaseAction == "Talk" ? "TalkTo" : camelCaseAction;
                if (type == HIGHLIGHTED_BUTTON)
                {
                    talktoFix += "Highlighted";
                }

                return "gui/hud/contextual/btn" + talktoFix + ".png";
            }
            return null;
        }
        public string getDefaultCursorPath(string cursor)
        {
            if (getCursorTypes().Contains(cursor))
            {
                return "gui/cursors/" + cursor + ".png";
            }
            return null;
        }

        public string getButtonPathFromEditor(string action, string type)
        {
            return getButtonPath(action, type, false);
        }

        public string getButtonPathFromEngine(string action, string type)
        {
            return getButtonPath(action, type, true);
        }

        private string getButtonPath(string action, string type, bool checkLegacyButtons)
        {

            foreach (CustomButton button in buttons)
            {
                if (button.getType().Equals(type) && button.getAction().Equals(action))
                {
                    return button.getPath();
                }
            }

            // ADDED IN eAd1.4: different types of buttons for use, use-with, grab and give-to. If the specific button is not found, a generic "use-grab" button is
            // searched
            if (checkLegacyButtons && action != null && (action.Equals(USEWITH_BUTTON) || action.Equals(USE_BUTTON) || action.Equals(GRAB_BUTTON) || action.Equals(GIVETO_BUTTON)))
            {
                return getButtonPath(USE_GRAB_BUTTON, type, false);
            }

            return null;
        }

        public void addArrow(CustomArrow arrow)
        {

            arrows.Add(arrow);
        }

        public void addArrow(string type, string path)
        {
            CustomArrow ca = new CustomArrow(type, path);
            arrows.Add(ca);
        }

        public List<CustomArrow> getArrows()
        {

            return arrows;
        }

        public string getArrowPath(string type)
        {

            foreach (CustomArrow arrow in arrows)
            {
                if (arrow.getType().Equals(type))
                {
                    return arrow.getPath();
                }
            }
            return null;
        }

        public bool isCommentaries()
        {

            return commentaries;
        }

        public void setCommentaries(bool commentaries)
        {

            this.commentaries = commentaries;
        }

        public int getGraphicConfig()
        {

            return this.graphicConfig;
        }

        public void setGraphicConfig(int graphicConfig)
        {

            this.graphicConfig = graphicConfig;
        }

        /**
         * @return the playerName
         */
        public string getPlayerName()
        {

            return playerName;
        }

        /**
         * @param playerName
         *            the playerName to set
         */
        public void setPlayerName(string playerName)
        {

            this.playerName = playerName;
        }

        public int getInventoryPosition()
        {

            return inventoryPosition;
        }

        public void setInventoryPosition(int inventoryPosition)
        {

            this.inventoryPosition = inventoryPosition;
        }

        public Vector2 getInventoryCoords()
        {

            return inventoryCoords;
        }

        public void setInventoryCoords(Vector2 inventoryCoords)
        {

            this.inventoryCoords = inventoryCoords;
        }

        public float getInventoryScale()
        {

            return inventoryScale;
        }

        public void setInventoryScale(float inventoryScale)
        {

            this.inventoryScale = inventoryScale;
        }

        public string getInventoryImage()
        {

            return inventoryImage;
        }

        public void setInventoryImage(string inventoryImage)
        {

            this.inventoryImage = inventoryImage;
        }

        public int countAssetReferences(string path)
        {

            int count = 0;
            foreach (CustomButton cb in buttons)
            {
                if (cb.getPath().Equals(path))
                    count++;
                if (cb.getSoundPath().Equals(path))
                    count++;
            }
            foreach (CustomArrow a in arrows)
            {
                if (a.getPath().Equals(path))
                    count++;
                if (a.getSoundPath().Equals(path))
                    count++;
            }
            foreach (CustomCursor cc in cursors)
            {
                if (cc.getPath().Equals(path))
                    count++;
            }

            return count;
        }

        public void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            if (assetPaths != null && assetTypes != null)
            {
                // Firstly iterate arrows
                foreach (CustomArrow arrow in arrows)
                {
                    int assetType = AssetsConstants.CATEGORY_BUTTON;
                    string assetPath = arrow.getPath();
                    getAssetReferencesForOneAsset(assetPaths, assetTypes, assetPath, assetType);
                    if (arrow.getSoundPath() != null && !arrow.getSoundPath().Equals(""))
                    {
                        assetPath = arrow.getSoundPath();
                        assetType = AssetsConstants.CATEGORY_AUDIO;
                        getAssetReferencesForOneAsset(assetPaths, assetTypes, assetPath, assetType);
                    }
                }
                // Secondly iterate buttons
                foreach (CustomButton button in buttons)
                {
                    int assetType = AssetsConstants.CATEGORY_BUTTON;
                    string assetPath = button.getPath();
                    getAssetReferencesForOneAsset(assetPaths, assetTypes, assetPath, assetType);
                    if (button.getSoundPath() != null && !button.getSoundPath().Equals(""))
                    {
                        assetPath = button.getSoundPath();
                        assetType = AssetsConstants.CATEGORY_AUDIO;
                        getAssetReferencesForOneAsset(assetPaths, assetTypes, assetPath, assetType);
                    }
                }
                // Finally iterate cursors
                foreach (CustomCursor cursor in cursors)
                {
                    int assetType = AssetsConstants.CATEGORY_CURSOR;
                    string assetPath = cursor.getPath();
                    getAssetReferencesForOneAsset(assetPaths, assetTypes, assetPath, assetType);
                }

            }
        }

        private void getAssetReferencesForOneAsset(List<string> assetPaths, List<int> assetTypes, string assetPath, int assetType)
        {

            if (assetPath == null)
                return;

            bool found = false;
            foreach (string path in assetPaths)
            {
                if (path.ToLower().Equals(assetPath.ToLower()))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                int last = assetPaths.Count;
                assetPaths.Insert(last, assetPath);
                assetTypes.Insert(last, assetType);
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

            if (assetPath != null)
            {
                // Firstly iterate arrows
                for (int i = 0; i < arrows.Count; i++)
                {
                    CustomArrow arrow = arrows[i];
                    if (arrow.getPath() != null && arrow.getPath().Equals(assetPath))
                    {
                        arrow.setPath(null);
                    }

                    if (arrow.getSoundPath() != null && arrow.getSoundPath().Equals(assetPath))
                    {
                        arrow.setSoundPath(null);
                    }

                    if (arrow.getPath() == null && arrow.getSoundPath() == null)
                    {
                        arrows.RemoveAt(i);
                        i--;
                    }
                }
                // Secondly iterate buttons
                for (int i = 0; i < buttons.Count; i++)
                {
                    CustomButton button = buttons[i];
                    if (button.getPath() != null && button.getPath().Equals(assetPath))
                    {
                        button.setPath(null);
                    }

                    if (button.getSoundPath() != null && button.getSoundPath().Equals(assetPath))
                    {
                        button.setSoundPath(null);
                    }

                    if (button.getPath() == null && button.getSoundPath() == null)
                    {
                        arrows.RemoveAt(i);
                        i--;
                    }
                }
                // Finally iterate cursors
                for (int i = 0; i < cursors.Count; i++)
                {
                    CustomCursor cursor = cursors[i];
                    if (cursor.getPath() != null && cursor.getPath().Equals(assetPath))
                    {
                        cursors.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /*
        @Override
            public Object clone() throws CloneNotSupportedException
        {
            //TODO the keepShowing is now no included due to decide the final situation
            DescriptorData dd = (DescriptorData) super.clone( );
                if( buttons != null ) {
                dd.buttons = new List<CustomButton>();
                for (CustomButton cb : buttons)
                    dd.buttons.add((CustomButton)cb.clone());
            }
            dd.commentaries = commentaries;
                if( contents != null ) {
                dd.contents = new List<ChapterSummary>();
                for (ChapterSummary cs : contents)
                    dd.contents.add((ChapterSummary)cs.clone());
            }
                if( cursors != null ) {
                dd.cursors = new List<CustomCursor>();
                for (CustomCursor cc : cursors)
                    dd.cursors.add((CustomCursor)cc.clone());
            }
                if( arrows != null ) {
                dd.arrows = new List<CustomArrow>();
                for (CustomArrow ca : arrows)
                    dd.arrows.add((CustomArrow)ca.clone());
            }
            dd.description = ( description != null ? new string(description ) : null );
                dd.graphicConfig = graphicConfig;
                dd.guiCustomized = guiCustomized;
                dd.guiType = guiType;
                dd.playerMode = playerMode;
                dd.playerName = ( playerName != null ? new string(playerName ) : null );
                dd.title = ( title != null ? new string(title ) : null );
                dd.inventoryPosition = new int(inventoryPosition );
                return dd;
            }
            */
        /**
         * @return the versionNumber
         */
        public string getVersionNumber()
        {

            return versionNumber;
        }

        /**
         * @param versionNumber
         *            the versionNumber to set
         */
        public void setVersionNumber(string versionNumber)
        {

            this.versionNumber = versionNumber;
        }

        /**
         * @return the projectName
         */
        public string getProjectName()
        {

            return projectName;
        }

        /**
         * @param projectName
         *            the projectName to set
         */
        public void setProjectName(string projectName)
        {

            this.projectName = projectName;
        }


        public bool isKeepShowing()
        {

            return keepShowing;
        }


        public void setKeepShowing(bool keepShowing)
        {

            this.keepShowing = keepShowing;
        }

        public void setDeafultClickAction(DefaultClickAction clickAction)
        {
            this.defaultClickAction = clickAction;
        }

        public DefaultClickAction getDefaultClickAction()
        {
            return defaultClickAction;
        }

        public void setPerspective(Perspective perspective)
        {
            this.perspective = perspective;
        }

        public Perspective getPerspective()
        {
            return perspective;
        }


        public DragBehaviour getDragBehaviour()
        {

            return dragBehaviour;
        }


        public void setDragBehaviour(DragBehaviour dragBehaviour)
        {

            this.dragBehaviour = dragBehaviour;
        }


        public bool isKeyboardNavigationEnabled()
        {

            return keyboardNavigationEnabled;
        }


        public void setKeyboardNavigation(bool keyboardNavigation)
        {

            this.keyboardNavigationEnabled = keyboardNavigation;
        }

        public bool isShowSaveLoad()
        {
            return showSaveLoad;
        }

        public void setShowSaveLoad(bool showSaveLoad)
        {
            this.showSaveLoad = showSaveLoad;
        }

        public bool isShowReset()
        {
            return showReset;
        }

        public void setShowReset(bool showReset)
        {
            this.showReset = showReset;
        }

        public bool isAutoSave()
        {
            return autoSave;
        }

        public void setAutoSave(bool autoSave)
        {
            this.autoSave = autoSave;
        }

        public bool isSaveOnSuspend()
        {
            return saveOnSuspend;
        }

        public void setSaveOnSuspend(bool saveOnSuspend)
        {
            this.saveOnSuspend = saveOnSuspend;
        }

        public bool isRestoreAfterOpen()
        {
            return restoreAfterOpen;
        }

        public void setRestoreAfterOpen(bool restoreAfterOpen)
        {
            this.restoreAfterOpen = restoreAfterOpen;
        }


        public string getApplicationIdentifier()
        {
            return applicationIdentifier;
        }

        public void setApplicationIdentifier(string applicationIdentifier)
        {
            this.applicationIdentifier = applicationIdentifier;
        }

        public lomType getImsCPMetadata()
        {
            return imsCPMetadata;
        }

        public void setImsCPMetadata(lomType value)
        {
            imsCPMetadata = value;
        }

        public virtual object Clone()
        {

            //TODO the keepShowing is now no included due to decide the final situation
            DescriptorData dd = (DescriptorData)this.MemberwiseClone();
            if (buttons != null)
            {
                dd.buttons = new List<CustomButton>();
                foreach (CustomButton cb in buttons)
                    dd.buttons.Add((CustomButton)cb.Clone());
            }
            dd.commentaries = commentaries;
            if (contents != null)
            {
                dd.contents = new List<ChapterSummary>();
                foreach (ChapterSummary cs in contents)
                    dd.contents.Add((ChapterSummary)cs.Clone());
            }
            if (cursors != null)
            {
                dd.cursors = new List<CustomCursor>();
                foreach (CustomCursor cc in cursors)
                    dd.cursors.Add((CustomCursor)cc.Clone());
            }
            if (arrows != null)
            {
                dd.arrows = new List<CustomArrow>();
                foreach (CustomArrow ca in arrows)
                    dd.arrows.Add((CustomArrow)ca.Clone());
            }
            dd.description = (description != null ? description : null);
            dd.graphicConfig = graphicConfig;
            dd.guiCustomized = guiCustomized;
            dd.guiType = guiType;
            dd.playerMode = playerMode;
            dd.playerName = (playerName != null ? playerName : null);
            dd.title = (title != null ? title : null);
            dd.inventoryPosition = inventoryPosition;
            return dd;
        }
    }
}
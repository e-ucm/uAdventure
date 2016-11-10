using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class is the handler to parse the e-Adventure descriptor file.
 */
public class DescriptorHandler : XMLHandler
{

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading a chapter
     */
    private const int READING_CHAPTER = 1;

    /**
     * string to store the current string in the XML file
     */
    private string currentstring;

    /**
     * Stores the game descriptor being read
     */
    private DescriptorData gameDescriptor;

    /**
     * Stores the element which is being read
     */
    private int reading = READING_NONE;

    /**
     * Chapter being currently read
     */
    private ChapterSummary currentChapter;

    /**
     * InputStreamCreator used in resolveEntity to find dtds (only required in
     * Applet mode)
     */
    private InputStreamCreator isCreator;

    /**
     * Constructor
     */
    public DescriptorHandler(InputStreamCreator isCreator)
    {

        currentstring = string.Empty;
        gameDescriptor = new DescriptorData();
        this.isCreator = isCreator;
    }

    /**
     * Returns the game descriptor read
     * 
     * @return Game descriptor
     */
    public DescriptorData getGameDescriptor()
    {

        return gameDescriptor;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#startElement(java.lang.string, java.lang.string, java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        if (qName.Equals("game-descriptor"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("versionNumber"))
                {
                    gameDescriptor.setVersionNumber(entry.Value.ToString());
                }
        }

        if (qName.Equals("configuration"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("keepShowing"))
                    gameDescriptor.setKeepShowing(entry.Value.ToString().Equals("yes"));
                if (entry.Key.Equals("keyboard-navigation"))
                    gameDescriptor.setKeyboardNavigation(entry.Value.ToString().Equals("enabled"));

                if (entry.Key.Equals("defaultClickAction"))
                {
                    if (entry.Value.ToString().Equals("showDetails"))
                        gameDescriptor.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_DETAILS);
                    if (entry.Value.ToString().Equals("showActions"))
                        gameDescriptor.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_ACTIONS);
                }
                if (entry.Key.Equals("perspective"))
                {
                    if (entry.Value.ToString().Equals("regular"))
                        gameDescriptor.setPerspective( DescriptorData.Perspective.REGULAR);
                    if (entry.Value.ToString().Equals("isometric"))
                        gameDescriptor.setPerspective(DescriptorData.Perspective.ISOMETRIC);
                }
                if (entry.Key.Equals("dragBehaviour"))
                {
                    if (entry.Value.ToString().Equals("considerNonTargets"))
                        gameDescriptor.setDragBehaviour(DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS);
                    if (entry.Value.ToString().Equals("ignoreNonTargets"))
                        gameDescriptor.setDragBehaviour(DescriptorData.DragBehaviour.IGNORE_NON_TARGETS);
                }
            }
        }

        // If the element is the GUI configuration, store the values
        if (qName.Equals("gui"))
        {
            int guiType = DescriptorData.GUI_TRADITIONAL;
            bool guiCustomized = false;
            int inventoryPosition = DescriptorData.INVENTORY_TOP_BOTTOM;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                // Type of the GUI
                if (entry.Key.Equals("type"))
                {
                    if (entry.Value.ToString().Equals("traditional"))
                        guiType = DescriptorData.GUI_TRADITIONAL;
                    else if (entry.Value.ToString().Equals("contextual"))
                        guiType = DescriptorData.GUI_CONTEXTUAL;
                }

                // Customized GUI
                else if (entry.Key.Equals("customized"))
                {
                    guiCustomized = entry.Value.ToString().Equals("yes");
                }
                if (entry.Key.Equals("inventoryPosition"))
                {
                    if (entry.Value.ToString().Equals("none"))
                        inventoryPosition = DescriptorData.INVENTORY_NONE;
                    else if (entry.Value.ToString().Equals("top_bottom"))
                        inventoryPosition = DescriptorData.INVENTORY_TOP_BOTTOM;
                    else if (entry.Value.ToString().Equals("top"))
                        inventoryPosition = DescriptorData.INVENTORY_TOP;
                    else if (entry.Value.ToString().Equals("bottom"))
                        inventoryPosition = DescriptorData.INVENTORY_BOTTOM;
                    else if (entry.Value.ToString().Equals("fixed_top"))
                        inventoryPosition = DescriptorData.INVENTORY_FIXED_TOP;
                    else if (entry.Value.ToString().Equals("fixed_bottom"))
                        inventoryPosition = DescriptorData.INVENTORY_FIXED_BOTTOM;
                }

            }

            // Set the values
            gameDescriptor.setGUI(guiType, guiCustomized);
            gameDescriptor.setInventoryPosition(inventoryPosition);
        }

        //Cursor
        if (qName.Equals("cursor"))
        {
            string type = "";
            string uri = "";
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("type"))
                {
                    type = entry.Value.ToString();
                }
                else if (entry.Key.Equals("uri"))
                {
                    uri = entry.Value.ToString();
                }
            }
            gameDescriptor.addCursor(type, uri);
        }

        //Button
        if (qName.Equals("button"))
        {
            string type = "";
            string uri = "";
            string action = "";
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("type"))
                {
                    type = entry.Value.ToString();
                }
                else if (entry.Key.Equals("uri"))
                {
                    uri = entry.Value.ToString();
                }
                else if (entry.Key.Equals("action"))
                {
                    action = entry.Value.ToString();
                }
            }
            gameDescriptor.addButton(action, type, uri);
        }

        if (qName.Equals("arrow"))
        {
            string type = "";
            string uri = "";
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("type"))
                {
                    type = entry.Value.ToString();
                }
                else if (entry.Key.Equals("uri"))
                {
                    uri = entry.Value.ToString();
                }
            }
            gameDescriptor.addArrow(type, uri);
        }

        if (qName.EndsWith("automatic-commentaries"))
        {
            gameDescriptor.setCommentaries(true);
        }

        //If the element is the player mode, store value
        if (qName.Equals("mode"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("playerTransparent"))
                {
                    if (entry.Value.ToString().Equals("yes"))
                    {
                        gameDescriptor.setPlayerMode(DescriptorData.MODE_PLAYER_1STPERSON);
                    }
                    else if (entry.Value.ToString().Equals("no"))
                    {
                        gameDescriptor.setPlayerMode(DescriptorData.MODE_PLAYER_3RDPERSON);
                    }
                }
            }
        }

        if (qName.Equals("graphics"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("mode"))
                {
                    if (entry.Value.ToString().Equals("windowed"))
                    {
                        gameDescriptor.setGraphicConfig(DescriptorData.GRAPHICS_WINDOWED);
                    }
                    else if (entry.Value.ToString().Equals("fullscreen"))
                    {
                        gameDescriptor.setGraphicConfig(DescriptorData.GRAPHICS_FULLSCREEN);
                    }
                    else if (entry.Value.ToString().Equals("blackbkg"))
                    {
                        gameDescriptor.setGraphicConfig(DescriptorData.GRAPHICS_BLACKBKG);
                    }
                }
            }
        }

        // If it is a chapter, create it and store the path
        else if (qName.Equals("chapter"))
        {
            currentChapter = new ChapterSummary();

            // Store the path of the chapter
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("path"))
                    currentChapter.setChapterPath(entry.Value.ToString());

            // Change the state
            reading = READING_CHAPTER;
        }

        // If it is an adaptation file, store the path
        // With last profile modifications, only old games includes that information in its descriptor file.
        // For that reason, the next "path" info is the name of the profile, and it is necessary to eliminate the path's characteristic
        // such as / and .xml

        else if (qName.Equals("adaptation-configuration"))
        {
            // Store the path of the adaptation file
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("path"))
                {
                    string adaptationName = entry.Value.ToString();
                    // delete the path's characteristics
                    // adaptationName = adaptationName.substring(adaptationName.indexOf("/")+1);
                    // adaptationName = adaptationName.substring(0,adaptationName.indexOf("."));
                    currentChapter.setAdaptationName(adaptationName);
                }

        }

        // If it is an assessment file, store the path
        // With last profile modifications, only old games includes that information in its descriptor file.
        // For that reason, the next "path" info is the name of the profile, and it is necessary to eliminate the path's characteristic
        // such as / and .xml
        else if (qName.Equals("assessment-configuration"))
        {
            // Store the path of the assessment file
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("path"))
                {
                    string assessmentName = entry.Value.ToString();
                    // delete the path's characteristics
                    // assessmentName = assessmentName.substring(assessmentName.indexOf("/")+1);
                    // assessmentName = assessmentName.substring(0,assessmentName.indexOf("."));
                    currentChapter.setAssessmentName(assessmentName);
                }
        }
    }

    /*  
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#endElement(java.lang.string, java.lang.string, java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // Stores the title
        if (qName.Equals("title"))
        {
            if (reading == READING_NONE)
                gameDescriptor.setTitle(currentstring.ToString().Trim());
            else if (reading == READING_CHAPTER)
                currentChapter.setTitle(currentstring.ToString().Trim());
        }

        // Stores the description
        else if (qName.Equals("description"))
        {
            if (reading == READING_NONE)
                gameDescriptor.setDescription(currentstring.ToString().Trim());
            else if (reading == READING_CHAPTER)
                currentChapter.setDescription(currentstring.ToString().Trim());
        }

        // Change the state if ends reading a chapter
        else if (qName.Equals("chapter"))
        {
            // Add the new chapter and change the state
            gameDescriptor.addChapterSummary(currentChapter);
            reading = READING_NONE;
        }

        // Reset the current string
        currentstring = string.Empty;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {

        // Append the new characters
        currentstring += new string(buf, offset, len);
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ErrorHandler#error(org.xml.sax.SAXParseException)
     */
    //    @Override
    //    public void error(SAXParseException exception) throws SAXParseException
    //{

    //    // On validation, propagate exception
    //    exception.printStackTrace( );
    //        throw exception;
    //}

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.EntityResolver#resolveEntity(java.lang.string, java.lang.string)
     */
    //@Override
    //    public InputSource resolveEntity(string publicId, string systemId)
    //{

    //    int startFilename = systemId.lastIndexOf("/") + 1;
    //    string filename = systemId.substring(startFilename, systemId.length());
    //    InputStream inputStream = isCreator.buildInputStream(filename);
    //    return new InputSource(inputStream);

    //}
}

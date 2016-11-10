using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * This class is the handler to parse the e-Adventure descriptor file.
 *
 * @author Bruno Torijano Bueno
 */
public class AdventureHandler : XMLHandler
{

    /**
     * Constant with the assessment folder path
     */
    private const string assessmentFolderPath = "assessment";

    /**
     * Constant with the adaptation folder path
     */
    private const string adaptationFolderPath = "adaptation";


    /**
	 * Constant for reading nothing.
	 */
    private const int READING_NONE = 0;

    /**
	 * Constant for reading a chapter.
	 */
    private const int READING_CHAPTER = 1;

    /**
	 * Stores the current element being read.
	 */
    private int reading = READING_NONE;

    /**
	 * Adventure data being read.
	 */
    private AdventureData adventureData;

    /**
	 * List of incidences
	 */
    private List<Incidence> incidences;

    /**
	 * List of chapters of the adventure.
	 */
    private List<Chapter> chapters;

    /**
	 * Assessment controller: to be filled with the assessment data
	 */
    //private List<AssessmentProfile> assessmentController;

    /**
	 * Adaptation controller: to be filled with the adaptation data
	 */
    //private List<AdaptationProfile> adaptationController;

    /**
	 * Chapter being currently read.
	 */
    private Chapter currentChapter;

    /**
	 * string to store the current string in the XML file
	 */
    protected string currentstring;

    private InputStreamCreator isCreator;

    /**
	 * The paths of assessments files
	 */
    private List<string> assessmentPaths;

    /**
	 * The paths of adaptation files
	 */
    private List<string> adaptationPaths;

    private static void getXMLFilePaths(InputStreamCreator isCreator, List<string> assessmentPaths, List<string> adaptationPaths)
    {

        // Assessment
        foreach (string child in isCreator.listNames(assessmentFolderPath))
        {
            if (child.ToLower().EndsWith(".xml"))
            {
                assessmentPaths.Add(assessmentFolderPath + "/" + child);
            }
        }

        // Adaptation

        foreach (string child in isCreator.listNames(adaptationFolderPath))
        {
            if (child.ToLower().EndsWith(".xml"))
            {
                adaptationPaths.Add(adaptationFolderPath + "/" + child);
            }
        }
    }

    /**
	 * Constructor.
	 * 
	 * @param zipFile
	 *            Path to the zip file which helds the chapter files
	 */
    public AdventureHandler(InputStreamCreator isCreator, List<Incidence> incidences)
    {
        this.isCreator = isCreator;
        assessmentPaths = new List<string>();
        adaptationPaths = new List<string>();
        getXMLFilePaths(isCreator, assessmentPaths, adaptationPaths);

        adventureData = new AdventureData();
        this.incidences = incidences;
        chapters = new List<Chapter>();
    }

    /**
	 * Load the assessment and adaptation profiles from xml.
	 * 
	 */
    //This method must be called after all chapter data is parse, because is a past functionality, and must be preserved in order
    // to bring the possibility to load game of past versions. Now the adaptation and assessment profiles are into chapter.xml, and not 
    // in separate files.
    public void loadProfiles()
    {

        //check if in chapter.xml there was any assessment or adaptation data
        if (!adventureData.hasAdapOrAssesData())
        {

            // Load all the assessment files in each chapter
            foreach (string assessmentPath in assessmentPaths)
            {
                bool added = false;
                AssessmentProfile assessProfile = Loader.loadAssessmentProfile(isCreator, assessmentPath, incidences);
                if (assessProfile != null)
                {
                    foreach (Chapter chapter in adventureData.getChapters())
                    {
                        if (chapter.getAssessmentName().Equals(assessProfile.getName()))
                        {
                            chapter.addAssessmentProfile(assessProfile);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        foreach (Chapter chapter in adventureData.getChapters())
                        {
                            chapter.addAssessmentProfile(assessProfile);
                        }
                    }


                }
            }

            // Load all the adaptation files in each chapter
            foreach (string adaptationPath in adaptationPaths)
            {
                bool added = false;
                AdaptationProfile adaptProfile = Loader.loadAdaptationProfile(isCreator, adaptationPath, incidences);
                if (adaptProfile != null)
                {
                    foreach (Chapter chapter in adventureData.getChapters())
                    {
                        if (chapter.getAdaptationName().Equals(adaptProfile.getName()))
                        {
                            chapter.addAdaptationProfile(adaptProfile);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        foreach (Chapter chapter in adventureData.getChapters())
                        {
                            chapter.addAdaptationProfile(adaptProfile);
                        }
                    }
                }
            }
        }

    }

    /**
	 * Returns the adventure data read
	 * 
	 * @return The adventure data from the XML descriptor
	 */
    public AdventureData getAdventureData()
    {
        return adventureData;
    }

    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        if (qName.Equals("game-descriptor"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("versionNumber"))
                {
                    adventureData.setVersionNumber(entry.Value.ToString());
                }
        }

        if (qName.Equals("configuration"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {

                if (entry.Key.Equals("keepShowing"))
                    adventureData.setKeepShowing(entry.Value.ToString().Equals("yes"));
                if (entry.Key.Equals("keyboard-navigation"))
                    adventureData.setKeyboardNavigation(entry.Value.ToString().Equals("enabled"));

                if (entry.Key.Equals("defaultClickAction"))
                {
                    if (entry.Value.ToString().Equals("showDetails"))
                        adventureData.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_DETAILS);
                    if (entry.Value.ToString().Equals("showActions"))
                        adventureData.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_ACTIONS);
                }
                if (entry.Key.Equals("perspective"))
                {
                    if (entry.Value.ToString().Equals("regular"))
                        adventureData.setPerspective(DescriptorData.Perspective.REGULAR);
                    if (entry.Value.ToString().Equals("isometric"))
                        adventureData.setPerspective(DescriptorData.Perspective.ISOMETRIC);
                }
                if (entry.Key.Equals("dragBehaviour"))
                {
                    if (entry.Value.ToString().Equals("considerNonTargets"))
                        adventureData.setDragBehaviour(DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS);
                    if (entry.Value.ToString().Equals("ignoreNonTargets"))
                        adventureData.setDragBehaviour(DescriptorData.DragBehaviour.IGNORE_NON_TARGETS);
                }

            }
        }


        // If reading a title, empty the current string
        if (qName.Equals("title") || qName.Equals("description"))
        {
            currentstring = string.Empty;
        }

        if (qName.EndsWith("automatic-commentaries"))
        {
            adventureData.setCommentaries(true);
        }

        // If reading the GUI tag, store the settings
        if (qName.Equals("gui"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("type"))
                {
                    if (entry.Value.ToString().Equals("traditional"))
                        adventureData.setGUIType(DescriptorData.GUI_TRADITIONAL);
                    else if (attrs["type"].Equals("contextual"))
                        adventureData.setGUIType(DescriptorData.GUI_CONTEXTUAL);
                }
                if (entry.Key.Equals("customized"))
                {
                    if (entry.Value.ToString().Equals("yes"))
                        adventureData.setGUI(adventureData.getGUIType(), true);
                    else
                        adventureData.setGUI(adventureData.getGUIType(), false);
                }
                if (entry.Key.Equals("inventoryPosition"))
                {
                    if (entry.Value.ToString().Equals("none"))
                        adventureData.setInventoryPosition(DescriptorData.INVENTORY_NONE);
                    else if (entry.Value.ToString().Equals("top_bottom"))
                        adventureData.setInventoryPosition(DescriptorData.INVENTORY_TOP_BOTTOM);
                    else if (entry.Value.ToString().Equals("top"))
                        adventureData.setInventoryPosition(DescriptorData.INVENTORY_TOP);
                    else if (entry.Value.ToString().Equals("bottom"))
                        adventureData.setInventoryPosition(DescriptorData.INVENTORY_BOTTOM);
                    else if (entry.Value.ToString().Equals("fixed_top"))
                        adventureData.setInventoryPosition(DescriptorData.INVENTORY_FIXED_TOP);
                    else if (entry.Value.ToString().Equals("fixed_bottom"))
                        adventureData.setInventoryPosition(DescriptorData.INVENTORY_FIXED_BOTTOM);
                }
            }
        }

        //Cursor
        if (qName.Equals("cursor"))
        {
            string type = ""; string uri = "";
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
            adventureData.addCursor(type, uri);
        }

        //Button
        if (qName.Equals("button"))
        {
            string type = ""; string uri = ""; string action = "";
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
            adventureData.addButton(action, type, uri);
        }

        if (qName.Equals("arrow"))
        {
            string type = ""; string uri = "";
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
            adventureData.addArrow(type, uri);
        }

        // If reading the mode tag:
        if (qName.Equals("mode"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("playerTransparent"))
                    if (entry.Value.ToString().Equals("yes"))
                        adventureData.setPlayerMode(DescriptorData.MODE_PLAYER_1STPERSON);
                    else if (entry.Value.ToString().Equals("no"))
                        adventureData.setPlayerMode(DescriptorData.MODE_PLAYER_3RDPERSON);
        }

        if (qName.Equals("graphics"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("mode"))
                {
                    if (entry.Value.ToString().Equals("windowed"))
                    {
                        adventureData.setGraphicConfig(DescriptorData.GRAPHICS_WINDOWED);
                    }
                    else if (entry.Value.ToString().Equals("fullscreen"))
                    {
                        adventureData.setGraphicConfig(DescriptorData.GRAPHICS_FULLSCREEN);
                    }
                    else if (entry.Value.ToString().Equals("blackbkg"))
                    {
                        adventureData.setGraphicConfig(DescriptorData.GRAPHICS_BLACKBKG);
                    }
                }
            }
        }


        // If reading the contents tag, switch to the chapters mode
        else if (qName.Equals("contents"))
        {
            reading = READING_CHAPTER;
        }

        // If reading the contents of a chapter, create a new one to store the data
        else if (qName.Equals("chapter"))
        {
            // Create the chapter
            currentChapter = new Chapter();

            // Search and store the path of the file
            string chapterPath = null;
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("path"))
                    chapterPath = entry.Value.ToString();

            if (chapterPath != null)
            {
                currentChapter.setChapterPath(chapterPath);
            }
            else
                currentChapter.setChapterPath("");

            // Open the file and load the data
            try
            {
                // Set the chapter handler
                // ChapterHandler chapterParser = new ChapterHandler(isCreator, currentChapter);
                ChapterHandler_ chapterParser = new ChapterHandler_(currentChapter);
                //// Create a new factory
                //SAXParserFactory factory = SAXParserFactory.newInstance();
                ////factory.setValidating( validate );
                //factory.setValidating(false);
                //SAXParser saxParser = factory.newSAXParser();

                //// Set the input stream with the file
                //InputStream chapterIS = isCreator.buildInputStream(chapterPath);

                //// Parse the data and close the data
                //saxParser.parse(chapterIS, chapterParser);
                //chapterIS.close();
                string chapterIS = isCreator.buildInputStream(chapterPath);
                chapterParser.Parse(chapterIS);

            }
            catch (Exception e) { Debug.LogError(e); }
            //catch (ParserConfigurationException e)
            //{
            //    incidences.add(Incidence.createChapterIncidence(Language.GetText("Error.LoadData.SAX"), chapterPath, e));
            //}
            //catch (SAXException e)
            //{
            //    incidences.add(Incidence.createChapterIncidence(Language.GetText("Error.LoadData.SAX"), chapterPath, e));
            //}
            //catch (IOException e)
            //{
            //    incidences.add(Incidence.createChapterIncidence(Language.GetText("Error.LoadData.IO"), chapterPath, e));
            //}

        }
        // If reading the adaptation configuration, store it
        // With last profile modifications, only old games includes that information in its descriptor file.
        // For that reason, the next "path" info is the name of the profile, and it is necessary to eliminate the path's characteristic
        // such as / and .xml
        else if (qName.Equals("adaptation-configuration"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("path"))
                {
                    string adaptationName = entry.Value.ToString();
                    // delete the path's characteristics
                    adaptationName = adaptationName.Substring(adaptationName.IndexOf("/") + 1);
                    adaptationName = adaptationName.Substring(0, adaptationName.IndexOf("."));
                    currentChapter.setAdaptationName(adaptationName);
                    // Search in incidences. If an adaptation incidence was related to this profile, the error is more relevant
                    for (int j = 0; j < incidences.Count; j++)
                    {
                        Incidence current = incidences[j];
                        if (current.getAffectedArea() == Incidence.ADAPTATION_INCIDENCE && current.getAffectedResource().Equals(adaptationName))
                        {
                            string message = current.getMessage();
                            incidences.RemoveAt(j);
                            incidences.Insert(j, Incidence.createAdaptationIncidence(true, message + "Error.LoadAdaptation.Referenced", adaptationName, null));
                        }
                    }
                }
        }
        // If reading the assessment configuration, store it
        // With last profile modifications, only old games includes that information in its descriptor file.
        // For that reason, the next "path" info is the name of the profile, and it is necessary to eliminate the path's characteristic
        // such as / and .xml
        else if (qName.Equals("assessment-configuration"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
                if (entry.Key.Equals("path"))
                {
                    string assessmentName = entry.Value.ToString();
                    // delete the path's characteristics
                    assessmentName = assessmentName.Substring(assessmentName.IndexOf("/") + 1);
                    assessmentName = assessmentName.Substring(0, assessmentName.IndexOf("."));
                    currentChapter.setAssessmentName(assessmentName);
                    // Search in incidences. If an adaptation incidence was related to this profile, the error is more relevant
                    for (int j = 0; j < incidences.Count; j++)
                    {
                        Incidence current = incidences[j];
                        if (current.getAffectedArea() == Incidence.ASSESSMENT_INCIDENCE && current.getAffectedResource().Equals(assessmentName))
                        {
                            string message = current.getMessage();
                            incidences.RemoveAt(j);
                            incidences.Insert(j, Incidence.createAssessmentIncidence(true, message + "Error.LoadAssessment.Referenced", assessmentName, null));
                        }
                    }

                }
        }
    }

    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If the title is complete, store it
        if (qName.Equals("title"))
        {
            // Store it in the adventure data
            if (reading == READING_NONE)
                adventureData.setTitle(currentstring.ToString().Trim());

            // Or in the chapter
            else if (reading == READING_CHAPTER)
                currentChapter.setTitle(currentstring.ToString().Trim());
        }

        // If the description is complete, store it
        else if (qName.Equals("description"))
        {
            // Store it in the adventure data
            if (reading == READING_NONE)
                adventureData.setDescription(currentstring.ToString().Trim());

            // Or in the chapter
            else if (reading == READING_CHAPTER)
                currentChapter.setDescription(currentstring.ToString().Trim());
        }

        // If the list of chapters is closing, store it
        else if (qName.Equals("contents"))
        {
            adventureData.setChapters(chapters);
        }

        // If a chapter is closing, store it in the list
        else if (qName.Equals("chapter"))
        {
            chapters.Add(currentChapter);
        }
    }

    public override void characters(char[] buf, int offset, int len)
    {
        // Append the new characters
        currentstring += new string(buf, offset, len);
    }

    //@Override
    //    public void error(SAXParseExceptsion exception) throws SAXParseException
    //{
    //		throw exception;
    //}

    /*	@Override
        public InputSource resolveEntity( string publicId, string systemId ) throws FileNotFoundException {
            // Take the name of the file SAX is looking for
            int startFilename = systemId.lastIndexOf( "/" ) + 1;
            string filename = systemId.substring( startFilename, systemId.length( ) );

            // Create the input source to return
            InputSource inputSource = null;

            try {
                // If the file is descriptor.dtd, use the one in the editor's folder
                if( filename.toLowerCase( ).Equals( "descriptor.dtd" ) )
                    inputSource = new InputSource( new FileInputStream( filename ) );

                // If it is any other file, use the super's method
                else
                    inputSource = super.resolveEntity( publicId, systemId );
            } catch( IOException e ) {
                e.printStackTrace( );
            } catch( SAXException e ) {
                e.printStackTrace( );
            }

            return inputSource;
        }*/

    /**
     * @return the incidences
     */
    public List<Incidence> getIncidences()
    {
        return incidences;
    }

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
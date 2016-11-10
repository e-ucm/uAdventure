using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class is the handler to parse the e-Adventure XML file
 */
public class ChapterHandler : XMLHandler
{

    /* Attributes */

    /**
     * Constant for subparsing nothing
     */
    private const int NONE = 0;

    /**
     * Constant for subparsing scene tag
     */
    private const int SCENE = 1;

    /**
     * Constant for subparsing slidescene tag
     */
    private const int CUTSCENE = 2;

    /**
     * Constant for subparsing book tag
     */
    private const int BOOK = 3;

    /**
     * Constant for subparsing object tag
     */
    private const int OBJECT = 4;

    /**
     * Constant for subparsing player tag
     */
    private const int PLAYER = 5;

    /**
     * Constant for subparsing character tag
     */
    private const int CHARACTER = 6;

    /**
     * Constant for subparsing conversation tag
     */
    private const int CONVERSATION = 7;

    /**
     * Constant for subparsing timer tag
     */
    private const int TIMER = 8;

    /**
     * Constant for subparsing global-state tag
     */
    private const int GLOBAL_STATE = 9;

    /**
     * Constant for subparsing macro tag
     */
    private const int MACRO = 10;

    /**
     * Constant for subparsing atrezzo object tag
     */
    private const int ATREZZO = 11;

    /**
     * Constant for subparsing assessment tag
     */
    private const int ASSESSMENT = 12;

    /**
     * Constant for subparsing adaptation tag
     */
    private const int ADAPTATION = 13;

    /**
     * Stores the current element being parsed
     */
    private int subParsing = NONE;

    /**
     * Current subparser being used
     */
    private SubParser subParser;

    /**
     * Chapter data
     */
    private Chapter chapter;

    /**
     * InputStreamCreator used in resolveEntity to find dtds (only required in
     * Applet mode)
     */
    private InputStreamCreator isCreator;

    /**
     * Current global state being subparsed
     */
    private GlobalState currentGlobalState;

    /**
     * Current macro being subparsed
     */
    private Macro currentMacro;

    /**
     * Buffer for globalstate docs
     */
    private string currentString;

    /* Methods */

    /**
     * Default constructor.
     * 
     * @param chapter
     *            Chapter in which the data will be stored
     */
    public ChapterHandler(InputStreamCreator isCreator, Chapter chapter)
    {

        this.chapter = chapter;
        this.isCreator = isCreator;
        currentString = string.Empty;
    }

    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed, check if we must subparse something
        if (subParsing == NONE)
        {

            //Parse eAdventure attributes
            if (qName.Equals("eAdventure"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("adaptProfile"))
                    {
                        chapter.setAdaptationName(entry.Value.ToString());
                    }
                    if (entry.Key.Equals("assessProfile"))
                    {
                        chapter.setAssessmentName(entry.Value.ToString());
                    }
                }
            }
            // Subparse scene
            else if (qName.Equals("scene"))
            {
                subParser = new SceneSubParser(chapter);
                subParsing = SCENE;
            }

            // Subparse slidescene
            else if (qName.Equals("slidescene") || qName.Equals("videoscene"))
            {
                subParser = new CutsceneSubParser(chapter);
                subParsing = CUTSCENE;
            }

            // Subparse book
            else if (qName.Equals("book"))
            {
                subParser = new BookSubParser(chapter);
                subParsing = BOOK;
            }

            // Subparse object
            else if (qName.Equals("object"))
            {
                subParser = new ItemSubParser(chapter);
                subParsing = OBJECT;
            }

            // Subparse player
            else if (qName.Equals("player"))
            {
                subParser = new PlayerSubParser(chapter);
                subParsing = PLAYER;
            }

            // Subparse character
            else if (qName.Equals("character"))
            {
                subParser = new CharacterSubParser(chapter);
                subParsing = CHARACTER;
            }

            // Subparse conversacion (tree conversation)
            else if (qName.Equals("tree-conversation"))
            {
                subParser = new TreeConversationSubParser(chapter);
                subParsing = CONVERSATION;
            }

            // Subparse conversation (graph conversation)
            else if (qName.Equals("graph-conversation"))
            {
                subParser = new GraphConversationSubParser(chapter);
                subParsing = CONVERSATION;
            }

            // Subparse timer
            else if (qName.Equals("timer"))
            {
                subParser = new TimerSubParser(chapter);
                subParsing = TIMER;
            }

            // Subparse global-state
            else if (qName.Equals("global-state"))
            {
                string id = null;
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        id = entry.Value.ToString();
                }
                currentGlobalState = new GlobalState(id);
                currentString = string.Empty;
                chapter.addGlobalState(currentGlobalState);
                subParser = new ConditionSubParser(currentGlobalState, chapter);
                subParsing = GLOBAL_STATE;
            }

            // Subparse macro
            else if (qName.Equals("macro"))
            {
                string id = null;
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        id = entry.Value.ToString();
                }
                currentMacro = new Macro(id);
                currentString = string.Empty;
                chapter.addMacro(currentMacro);
                subParser = new EffectSubParser(currentMacro, chapter);
                subParsing = MACRO;
            }
            // Subparse atrezzo object
            else if (qName.Equals("atrezzoobject"))
            {
                subParser = new AtrezzoSubParser(chapter);
                subParsing = ATREZZO;
            }// Subparse assessment profile
            else if (qName.Equals("assessment"))
            {
                subParser = new AssessmentSubParser(chapter);
                subParsing = ASSESSMENT;
            }// Subparse adaptation profile
            else if (qName.Equals("adaptation"))
            {
                subParser = new AdaptationSubParser(chapter);
                subParsing = ADAPTATION;
            }

        }

        // If an element is being subparsed, spread the call
        if (subParsing != NONE)
        {
            //try {
            subParser.startElement(namespaceURI, sName, qName, attrs);
            //} catch (Exception e) { Debug.LogError(e); }{
            //	System.out.println("Marihuanhell es muy malo pero hemos capturado la excepción");
            //e.printStackTrace();
            //}

        }
    }

    public override void endElement(string namespaceURI, string sName, string qName)
    {

        if (qName.Equals("documentation") && subParsing == GLOBAL_STATE)
        {
            currentGlobalState.setDocumentation(currentString.ToString().Trim());
        }
        else if (qName.Equals("documentation") && subParsing == MACRO)
        {
            currentMacro.setDocumentation(currentString.ToString().Trim());
        }

        currentString = string.Empty;

        // If an element is being subparsed
        if (subParsing != NONE)
        {

            // Spread the end element call
            subParser.endElement(namespaceURI, sName, qName);
           // Debug.Log(subParsing + " " + sName + " " + qName);
            // If the element is not being subparsed anymore, return to normal state
            if (qName.Equals("scene") && subParsing == SCENE || (qName.Equals("slidescene") || qName.Equals("videoscene")) && subParsing == CUTSCENE || qName.Equals("book") && subParsing == BOOK || qName.Equals("object") && subParsing == OBJECT || qName.Equals("player") && subParsing == PLAYER || qName.Equals("character") && subParsing == CHARACTER || qName.Equals("tree-conversation") && subParsing == CONVERSATION || qName.Equals("graph-conversation") && subParsing == CONVERSATION || qName.Equals("timer") && subParsing == TIMER || qName.Equals("global-state") && subParsing == GLOBAL_STATE || qName.Equals("macro") && subParsing == MACRO || qName.Equals("atrezzoobject") && subParsing == ATREZZO || qName.Equals("assessment") && subParsing == ASSESSMENT || qName.Equals("adaptation") && subParsing == ADAPTATION)
            {
                subParsing = NONE;
            }

        }
    }

    public void endDocument()
    {

        // In the end of the document, if the chapter has no initial scene
        if (chapter.getTargetId() == null)
        {
            // Set it to the first scene
            if (chapter.getScenes().Count > 0)
                chapter.setTargetId(chapter.getScenes()[0].getId());

            // Or to the first cutscene
            else if (chapter.getCutscenes().Count > 0)
                chapter.setTargetId(chapter.getCutscenes()[0].getId());
        }
    }

    public override void characters(char[] buf, int offset, int len)
    {

        // If the SAX handler is reading an element, just spread the call to the parser
        currentString += new string(buf, offset, len);
        if (subParsing != NONE)
        {
            subParser.characters(buf, offset, len);
        }
    }

    //    @Override
    //    public void error(SAXParseException exception) throws SAXParseException
    //{

    //    // On validation, propagate exception
    //    exception.printStackTrace( );
    //        throw exception;
    //}

    /*	@Override
        public InputSource resolveEntity( string publicId, string systemId ) {
            // Take the name of the file SAX is looking for
            int startFilename = systemId.lastIndexOf( "/" ) + 1;
            string filename = systemId.substring( startFilename, systemId.length( ) );

            // Create the input source to return
            InputSource inputSource = null;

            try {
                // If the file is eadventure.dtd, use the one in the editor's folder
                if( filename.toLowerCase( ).Equals( "eadventure.dtd" ) )
                    inputSource = new InputSource( new FileInputStream( filename ) );

                // If it is any other file, use the super's method
                else
                    inputSource = super.resolveEntity( publicId, systemId );
            } catch( FileNotFoundException e ) {
                e.printStackTrace( );
            } catch( IOException e ) {
                e.printStackTrace( );
            } catch( SAXException e ) {
                e.printStackTrace( );
            }

            return inputSource;
        }*/

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.EntityResolver#resolveEntity(java.lang.String, java.lang.String)
     */
    //@Override
    //    public InputSource resolveEntity(string publicId, string systemId)
    //{
    //    int startFilename = systemId.lastIndexOf("/") + 1;
    //    string filename = systemId.substring(startFilename, systemId.length());
    //    InputStream inputStream = isCreator.buildInputStream(filename);
    //    return new InputSource(inputStream);
    //}

    //@Override
    //    public void fatalError(SAXParseException e) throws SAXException
    //{

    //    //throw e;
    //}
}
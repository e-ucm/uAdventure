using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

namespace uAdventure.Core
{
    /**
     * This class loads the e-Adventure data from a XML file
     */
    public class Loader
    {
        /**
            * AdventureData structure which has been previously read. (For Debug
            * execution)
            */
        private static AdventureData adventureData;

        /**
         * Cache the SaxParserFactory
         */
        //private static SAXParserFactory factory = SAXParserFactory.newInstance();

        /**
         * Private constructor
         */
        private Loader()
        {

        }

        /**
         * Loads the adventure data from the given ZIP file.
         * 
         * @param zipFile
         *            Path to the zip file which holds the adventure
         * @return The adventure data, null if there was an error
         */
        public static AdventureData loadAdventureData(InputStreamCreator isCreator, List<Incidence> incidences)
        {

            AdventureData adventureDataTemp = null;
            try
            {
                // Set the adventure handler
                AdventureHandler adventureParser = new AdventureHandler(isCreator, incidences);
                //factory.setValidating(false);
                //SAXParser saxParser = factory.newSAXParser();

                // Read and close the input stream
                string descriptorIS = isCreator.buildInputStream("descriptor.xml");
                adventureParser.Parse(descriptorIS);
                //descriptorIS.close();

                // Load the assessment and adaptation profiles. It must be after parse
                // the adventure data because the profile's load from xml inserts each profile
                // in each chapter.
                adventureParser.loadProfiles();
                // Store the adventure data
                adventureDataTemp = adventureParser.getAdventureData();

            }
            catch (Exception e) { Debug.LogError(e); }
            //catch (ParserConfigurationException e)
            //{
            //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.SAX"), e));
            //}
            //catch (SAXException e)
            //{
            //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.SAX"), e));
            //}
            //catch (IOException e)
            //{
            //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.IO"), e));
            //}
            //catch (IllegalArgumentException e)
            //{
            //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.NoDescriptor"), e));
            //}

            return adventureDataTemp;
        }

        public static AdventureData loadAdventureData(string path, List<Incidence> incidences)
        {
            AdventureData adventureDataTemp = new AdventureData();
            try
            {
                AdventureHandler_ adventureParser = new AdventureHandler_(adventureDataTemp);

                // Read and close the input stream
                adventureParser.Parse(path + "\\descriptor.xml");
                //descriptorIS.close();

                adventureDataTemp = adventureParser.getAdventureData();

            }
            catch (Exception e) { Debug.LogError(e); }

            return adventureDataTemp;
        }

        /**
         * Loads the descriptor of the current ZIP adventure loaded
         * 
         * @return The descriptor data of the game
         */
        public static DescriptorData loadDescriptorData(InputStreamCreator isCreator, List<Incidence> incidences)
        {

            DescriptorData descriptorData = null;

            if (Loader.adventureData != null)
            {
                descriptorData = Loader.adventureData;
            }
            else
            {

                try
                {
                    // Set the adventure handler
                    DescriptorHandler descriptorParser = new DescriptorHandler(isCreator);

                    //factory.setValidating(false);
                    //SAXParser saxParser = factory.newSAXParser();

                    // Read and close the inputstrea
                    string descriptorIS = isCreator.buildInputStream("descriptor.xml");
                    //saxParser.parse(descriptorIS, descriptorParser);
                    //descriptorIS.close();
                    descriptorParser.Parse(descriptorIS);

                    // Store the adventure data
                    descriptorData = descriptorParser.getGameDescriptor();

                }
                catch (Exception e) { Debug.LogError(e); }

                //catch (ParserConfigurationException e)
                //{
                //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.SAX"), e));
                //}
                //catch (SAXException e)
                //{
                //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.SAX"), e));
                //}
                //catch (IOException e)
                //{
                //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.IO"), e));
                //}
                //catch (IllegalArgumentException e)
                //{
                //    incidences.add(Incidence.createDescriptorIncidence(Language.GetText("Error.LoadDescriptor.NoDescriptor"), e));
                //}
            }
            return descriptorData;

        }

        /**
         * Loads the script data from the given XML file
         * 
         * @param filename
         *            Name of the XML file containing the script
         * @param validate
         *            distinguish between if the load is made in editor or engine
         * @return The script stored as game data
         */
        public static Chapter loadChapterData(InputStreamCreator isCreator, string fileName, List<Incidence> incidences)
        {
            // Create the chapter
            Chapter currentChapter = new Chapter();
            bool chapterFound = false;
            if (Loader.adventureData != null)
            {
                foreach (Chapter chapter in adventureData.getChapters())
                {
                    if (chapter != null && chapter.getChapterPath() != null && chapter.getChapterPath().Equals(fileName))
                    {
                        currentChapter = chapter;
                        chapterFound = true;
                        break;

                    }
                    else if (chapter != null && chapter.getChapterPath() == null)
                    {

                        currentChapter = chapter;
                        chapterFound = true;
                        currentChapter.setChapterPath("chapter1.xml");
                        break;

                    }
                }

            }
            if (!chapterFound)
            {

                string chapterIS = null;

                //if (zipFile!=null){
                chapterIS = isCreator.buildInputStream(fileName);
                currentChapter.setChapterPath(fileName);

                //} else{
                // Then fileName is an absolutePath
                //string chapterPath = fileName.substring( Math.max (fileName.lastIndexOf( '\\' ), fileName.lastIndexOf( '/' ) ), fileName.length( ));
                //currentChapter.setName( chapterPath );
                //try {
                //	chapterIS = new FileInputStream( fileName );
                //} catch (FileNotFoundException e) {
                //e.printStackTrace();
                //	incidences.add( Incidence.createChapterIncidence( TextConstants.getText( "Error.LoadData.IO" ), fileName ) );
                //}
                //}

                // Open the file and load the data
                try
                {
                    if (chapterIS != null)
                    {
                        // Set the chapter handler
                        ChapterHandler chapterParser = new ChapterHandler(isCreator, currentChapter);

                        //factory.setValidating(false);

                        //SAXParser saxParser = factory.newSAXParser();

                        //// Parse the data and close the data
                        //saxParser.parse(chapterIS, chapterParser);
                        //chapterIS.close();
                        chapterParser.Parse(chapterIS);
                    }

                }
                catch (Exception e) { Debug.LogError(e); }
                //catch (ParserConfigurationException e)
                //{
                //    incidences.add(Incidence.createChapterIncidence(Language.GetText("Error.LoadData.SAX"), fileName, e));
                //}
                //catch (SAXException e)
                //{
                //    incidences.add(Incidence.createChapterIncidence(Language.GetText("Error.LoadData.SAX"), fileName, e));
                //}
                //catch (IOException e)
                //{
                //    incidences.add(Incidence.createChapterIncidence(Language.GetText("Error.LoadData.IO"), fileName, e));
                //}
            }
            //Debug.Log(currentChapter);
            return currentChapter;
        }

        /**
         * Loads the assessment profile (set of assessment rules) stored in file
         * with path xmlFile in zipFile
         * 
         * @param zipFile
         * @param xmlFile
         * @param incidences
         * @return
         */
        public static AssessmentProfile loadAssessmentProfile(InputStreamCreator isCreator, string xmlFile, List<Incidence> incidences)
        {

            AssessmentProfile newProfile = null;
            if (Loader.adventureData != null)
            {
                foreach (Chapter chapter in Loader.adventureData.getChapters())
                {
                    if (chapter.getAssessmentProfiles().Count != 0)
                    {
                        foreach (AssessmentProfile profile in chapter.getAssessmentProfiles())
                        {
                            if (profile.getName().Equals(xmlFile))
                            {
                                //try
                                //{
                                newProfile = (AssessmentProfile)profile;
                                //}
                                //catch (CloneNotSupportedException e)
                                //{
                                //    e.printStackTrace();
                                //}
                                break;
                            }
                        }
                    }
                }
            }
            else
            {

                // Open the file and load the data
                try
                {
                    // Set the chapter handler
                    AssessmentProfile profile = new AssessmentProfile();

                    string name = xmlFile;
                    name = name.Substring(name.IndexOf("/") + 1);
                    if (name.IndexOf(".") != -1)
                        name = name.Substring(0, name.IndexOf("."));
                    profile.setName(name);
                    AssessmentHandler assParser = new AssessmentHandler(isCreator, profile);

                    //factory.setValidating(true);
                    //SAXParser saxParser = factory.newSAXParser();

                    //// Parse the data and close the data
                    string assessmentIS = isCreator.buildInputStream(xmlFile);

                    //saxParser.parse(assessmentIS, assParser);
                    //assessmentIS.close();
                    assParser.Parse(assessmentIS);
                    // Finally add the new controller to the list
                    // Create the new profile

                    // Fill flags & vars
                    newProfile = profile;

                }
                catch (Exception e) { Debug.LogError(e); }

                //catch (ParserConfigurationException e)
                //{
                //    incidences.add(Incidence.createAssessmentIncidence(false, Language.GetText("Error.LoadAssessmentData.SAX"), xmlFile, e));
                //}
                //catch (SAXException e)
                //{
                //    incidences.add(Incidence.createAssessmentIncidence(false, Language.GetText("Error.LoadAssessmentData.SAX"), xmlFile, e));
                //}
                //catch (IOException e)
                //{
                //    incidences.add(Incidence.createAssessmentIncidence(false, Language.GetText("Error.LoadAssessmentData.IO"), xmlFile, e));
                //}
            }
            return newProfile;
        }

        /**
         * Loads the adaptation profile (set of adaptation rules + initial state)
         * stored in file with path xmlFile in zipFile
         * 
         * @param zipFile
         * @param xmlFile
         * @param incidences
         * @return
         */
        public static AdaptationProfile loadAdaptationProfile(InputStreamCreator isCreator, string xmlFile, List<Incidence> incidences)
        {

            AdaptationProfile newProfile = null;
            if (Loader.adventureData != null)
            {
                foreach (Chapter chapter in Loader.adventureData.getChapters())
                {
                    if (chapter.getAssessmentProfiles().Count != 0)
                    {
                        foreach (AdaptationProfile profile in chapter.getAdaptationProfiles())

                            if (profile.getName().Equals(xmlFile))
                            {
                                newProfile = profile;
                                break;
                            }
                    }
                }

            }
            else
            {

                // Open the file and load the data
                try
                {
                    // Set the chapter handler
                    List<AdaptationRule> rules = new List<AdaptationRule>();
                    AdaptedState initialState = new AdaptedState();
                    AdaptationHandler adpParser = new AdaptationHandler(isCreator, rules, initialState);

                    //factory.setValidating(true);
                    //SAXParser saxParser = factory.newSAXParser();

                    // Parse the data and close the data
                    string adaptationIS = isCreator.buildInputStream(xmlFile);

                    //saxParser.parse(adaptationIS, adpParser);
                    //adaptationIS.close();

                    adpParser.Parse(adaptationIS);

                    // Finally add the new controller to the list
                    // Create the new profile
                    string name = xmlFile;
                    name = name.Substring(name.IndexOf("/") + 1);
                    name = name.Substring(0, name.IndexOf("."));
                    newProfile = new AdaptationProfile(adpParser.getAdaptationRules(), adpParser.getInitialState(), name, adpParser.isScorm12(), adpParser.isScorm2004());

                    newProfile.setFlags(adpParser.getFlags());
                    newProfile.setVars(adpParser.getVars());

                }
                catch (Exception e)
                { Debug.LogError(e); }
                //catch (ParserConfigurationException e)
                //{
                //    incidences.add(Incidence.createAdaptationIncidence(false, Language.GetText("Error.LoadAdaptationData.SAX"), xmlFile, e));
                //}
                //catch (SAXException e)
                //{
                //    incidences.add(Incidence.createAdaptationIncidence(false, Language.GetText("Error.LoadAdaptationData.SAX"), xmlFile, e));
                //}
                //catch (IOException e)
                //{
                //    incidences.add(Incidence.createAdaptationIncidence(false, Language.GetText("Error.LoadAdaptationData.IO"), xmlFile, e));
                //}
            }
            return newProfile;
        }

        /**
         * @param adventureData
         *            the adventureData to set
         */
        public static void setAdventureData(AdventureData adventureData)
        {

            Loader.adventureData = adventureData;
        }

        /**
         * Loads an animation from a filename
         * 
         * @param filename
         *            The xml descriptor for the animation
         * @return the loaded Animation
         */
        public static Animation loadAnimation(InputStreamCreator isCreator, string filename, ImageLoaderFactory imageloader)
        {

            AnimationHandler_ animationHandler = new AnimationHandler_(isCreator, imageloader);

            // Create a new factory
            //factory.setValidating(false);
            //SAXParser saxParser;
            try
            {
                //saxParser = factory.newSAXParser();

                // Read and close the input stream
                //File file = new File(filename);
                string descriptorIS = null;
                /*try {
                    System.out.println("FILENAME="+filename);
                    descriptorIS = ResourceHandler.getInstance( ).buildInputStream(filename);
                    System.out.println("descriptorIS==null?"+(descriptorIS==null));

                    //descriptorIS = new InputStream(ResourceHandler.getInstance().getResourceAsURLFromZip(filename));
                } catch (Exception e) { Debug.LogError(e); } {
                    e.printStackTrace();
                }
                if (descriptorIS == null) {
                    descriptorIS = AssetsController.getInputStream(filename);
                }*/
                descriptorIS = isCreator.buildInputStream("Assets/Resources/CurrentGame/" + filename);
                if (!descriptorIS.EndsWith(".eaa"))
                    descriptorIS += ".eaa";
                animationHandler.Parse(descriptorIS);
                //saxParser.parse(descriptorIS, animationHandler);
                //descriptorIS.close();

            }
            catch (Exception e) { Debug.LogError(e); }
            //catch (ParserConfigurationException e)
            //{
            //    e.printStackTrace();
            //    System.err.println(filename);
            //}
            //catch (SAXException e)
            //{
            //    e.printStackTrace();
            //    System.err.println(filename);
            //}
            //catch (FileNotFoundException e)
            //{
            //    e.printStackTrace();
            //    System.err.println(filename);
            //}
            //catch (IOException e)
            //{
            //    e.printStackTrace();
            //    System.err.println(filename);
            //}

            if (animationHandler.getAnimation() != null)
                return animationHandler.getAnimation();
            else
                return new Animation("anim" + (new System.Random()).Next(1000), imageloader);
        }

        /**
         * Returns true if the given file contains an eAdventure game from a newer
         * version. Essentially, it looks for the "ead.properties" file in the new
         * eAdventure games. If it's found, then returns true
         * 
         * @param f
         *            the file to check
         * @return if the game requires a newer version
         */
        //public static bool requiresNewVersion(java.io.File f)
        //{
        //    bool isOldProject = true;
        //    FileInputStream in = null;
        //    ZipInputStream zipIn = null;
        //    try
        //    {
        //        in = new FileInputStream(f);
        //        zipIn = new ZipInputStream( in );
        //        ZipEntry zipEntry = null;
        //        while ((zipEntry = zipIn.getNextEntry()) != null)
        //        {
        //            if (zipEntry.getName().endsWith("ead.properties"))
        //            {
        //                isOldProject = false;
        //            }
        //        }
        //        zipIn.close();
        //    }
        //    catch (IOException e)
        //    {

        //    }
        //    finally
        //    {
        //        if ( in != null ) {
        //            try
        //            {
        //                in.close();
        //            }
        //            catch (IOException e)
        //            {

        //            }
        //        }

        //        if (zipIn != null)
        //        {
        //            try
        //            {
        //                zipIn.close();
        //            }
        //            catch (IOException e)
        //            {
        //            }
        //        }
        //    }
        //    return !isOldProject;
        //}
    }
}
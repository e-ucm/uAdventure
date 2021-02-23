using System.Xml;
using System.Collections.Generic;

using uAdventure.Core;
using System.Xml.Serialization;
using uAdventure.Core.Metadata;

namespace uAdventure.Editor
{
    /**
     * Static class, containing the main functions to write an adventure into an XML
     * file.
     */

    public class Writer
    {
        /**
            * Text Constants for LOM Exportation
            */
        private const string RESOURCE_IDENTIFIER = "res_eAdventure";

        private const string ITEM_IDENTIFIER = "itm_eAdventure";

        //private const string ITEM_TITLE="The eAdventure game";

        private const string ORGANIZATION_IDENTIFIER = "eAdventure";

        private const string ORGANIZATION_TITLE = "eAdventure course";

        private const string ORGANIZATION_STRUCTURE = "hierarchical";

        private static XmlDocument doc;

        public static XmlDocument GetDoc()
        {
            return doc;
        }

        /**
         * Private constructor.
         */

        private Writer()
        {

        }

        /**
         * Writes the daventure data into the given file.
         * 
         * @param folderName
         *            Folder where to write the data
         * @param adventureData
         *            Adventure data to write in the file
         * @param valid
         *            True if the adventure is valid (can be executed in the
         *            engine), false otherwise
         * @return True if the operation was succesfully completed, false otherwise
         */

        public static bool writeData(string folderName, AdventureDataControl adventureData, bool valid)
        {
            bool dataSaved = false;

            // Create the necessary elements for building the DOM
            doc = new XmlDocument();

            // Delete the previous XML files in the root of the project dir
            //DirectoryInfo projectFolder = new DirectoryInfo(folderName);
            //if (projectFolder.Exists)
            //{
            //    foreach (FileInfo file in projectFolder.GetFiles())
            //    {
            //        file.Delete();
            //    }
            //    foreach (DirectoryInfo dir in projectFolder.GetDirectories())
            //    {
            //        dir.Delete(true);
            //    }
            //}

            // Add the special asset files
            // TODO AssetsController.addSpecialAssets();

            /** ******* START WRITING THE DESCRIPTOR ********* */
            // Pick the main node for the descriptor
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");
            //XmlDocumentType typeDescriptor = doc.CreateDocumentType("game-descriptor", "SYSTEM", "descriptor.dtd", null);
            doc.AppendChild(declaration);
            //doc.AppendChild(typeDescriptor);

            if (!valid)
                DOMWriterUtility.DOMWrite(doc, adventureData, new DescriptorDOMWriter.InvalidAdventureDataControlParam());
            else
                DOMWriterUtility.DOMWrite(doc, adventureData);

            doc.Save(folderName + "/descriptor.xml");
            /** ******** END WRITING THE DESCRIPTOR ********** */

            /** ******* START WRITING THE CHAPTERS ********* */
            // Write every chapter
            //XmlDocumentType typeChapter;

            int chapterIndex = 1;
            foreach (Chapter chapter in adventureData.getChapters())
            {

                doc = new XmlDocument();
                declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");
                //typeChapter = doc.CreateDocumentType("eAdventure", "SYSTEM", "eadventure.dtd", null);
                doc.AppendChild(declaration);
                //doc.AppendChild(typeChapter);

                DOMWriterUtility.DOMWrite(doc, chapter);

                doc.Save(folderName + "/chapter" + chapterIndex++ + ".xml");
            }
            /** ******** END WRITING THE CHAPTERS ********** */

            /** ******* START WRITING THE METADATA ********* */

            // Pick the main node for the descriptor
            var metadata = adventureData.getImsCPMetadata();
            if(metadata != null)
            {
                var xmlMetadata = SerializeToXmlElement(new XmlDocument(), metadata);
                doc = new XmlDocument();
                xmlMetadata = MetadataUtility.CleanXMLGarbage(doc, xmlMetadata);
                doc.AppendChild(xmlMetadata);
                doc.Save(folderName + "/imscpmetadata.xml"); 
            }
            /** ******** END WRITING THE CHAPTERS ********** */
            dataSaved = true;
            return dataSaved;
        }

        public static XmlElement SerializeToXmlElement(XmlDocument doc, object o)
        {
            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                var serializer = new XmlSerializer(o.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("imsmd", "http://www.imsglobal.org/xsd/imsmd_v1p2");
                serializer.Serialize(writer, o, ns);
            }

            return doc.DocumentElement;
        }

        public static XmlElement writeAssessmentData( /* string zipFilename,*/
            AssessmentProfile profile, bool valid, XmlDocument doc)
        {

            /**
             * ******************* STORE ASSESSMENT DATA WHEN PRESENT
             * ******************
             */
            XmlElement assNode = null;
            if (profile.getName() != null && !profile.getName().Equals(""))
            {
                assNode = AssessmentDOMWriter.buildDOM(profile, doc);
            }
            return assNode;
        }

        public static XmlElement writeAdaptationData(AdaptationProfile profile, bool valid, XmlDocument doc)
        {

            /**
             * ******************* STORE ASSESSMENT DATA WHEN PRESENT
             * ******************
             */

            XmlElement adpNode = null;
            // take the name of the profile
            string name = profile.getName();
            if (name != null && !profile.getName().Equals("") /*&& new File(zipFilename, controller.getPath( )).exists()*/)
            {

                List<AdaptationRule> rules = profile.getRules();
                AdaptedState initialState = profile.getInitialState();

                // check if it is an scorm profile
                bool scorm2004 = profile.isScorm2004();
                bool scorm12 = profile.isScorm12();

                adpNode = AdaptationDOMWriter.buildDOM(rules, initialState, scorm12, scorm2004, name, doc);
            }
            return adpNode;
        }

        /**
         * Indent the given DOM node recursively with the given depth.
         * 
         * @param nodeDOM
         *            DOM node to be indented
         * @param depth
         *            Depth of the current node
         */

        private static void indentDOM(XmlNode nodeDOM, int depth)
        {

            // First of all, extract the document of the node, and the list of children
            XmlDocument document = nodeDOM.OwnerDocument;
            XmlNodeList children = nodeDOM.ChildNodes;

            // Flag for knowing if the current node is empty of element nodes
            bool isEmptyOfElements = true;

            int i = 0;
            // For each children node
            while (i < children.Count)
            {
                XmlNode currentChild = children.Item(i);

                // If the current child is an element node
                if (currentChild.NodeType == XmlNodeType.Element)
                {
                    // Insert a indention before it, and call the recursive function with the child (and a higher depth)
                    nodeDOM.InsertBefore(document.CreateTextNode("\n" + getTab(depth + 1)), currentChild);
                    indentDOM(currentChild, depth + 1);

                    // Set empty of elements to false, and increase i (the new child moves all children)
                    isEmptyOfElements = false;
                    i++;
                }

                // Go to next child
                i++;
            }

            // If this node has some element, add the indention for the closing tag
            if (!isEmptyOfElements)
                nodeDOM.AppendChild(document.CreateTextNode("\n" + getTab(depth)));
        }

        private static bool writeWebPage(string tempDir, string loName, bool windowed, string mainClass, bool debugerScorm)
        {
            return writeWebPage(tempDir, loName, windowed, mainClass, null, debugerScorm);
        }

        private static bool writeWebPage(string tempDir, string loName, bool windowed, string mainClass)
        {
            return writeWebPage(tempDir, loName, windowed, mainClass, null, false);
        }

        private static bool writeWebPage(string tempDir, string loName, bool windowed, string mainClass,
            Dictionary<string, string> additionalParams)
        {

            return writeWebPage(tempDir, loName, windowed, mainClass, additionalParams, false);
        }


        private static bool writeWebPage(string tempDir, string loName, bool windowed, string mainClass,
            Dictionary<string, string> additionalParams, bool debugScorm)
        {


            bool dataSaved = true;
            //try
            //{
            //    string jscript = "";

            //    //TODO cambiar
            //    bool lams = true;
            //    if (mainClass.Equals("es.eucm.eadventure.engine.EAdventureAppletLAMS"))
            //    {
            //        lams = true;
            //    }

            //    // THE NEXT CODE WAS ADDED ON 18th Dec 2009. This will be used to remove the loading message dynamically
            //    jscript += "\t\t<script type='text/javascript' language='JavaScript'>\n";
            //    jscript += "\t\t\t<!--\n";
            //    jscript += "\t\t\tfunction hideText(){\n";
            //    jscript += "\t\t\t\tmsg = document.getElementById('loadingMessage');\n";
            //    jscript += "\t\t\t\tmsg.style.display = 'none';\n";
            //    jscript += "\t\t\t}\n";
            //    if (lams)
            //    {
            //        // THE NEXT CODE WAS ADDED ON Feb 2010. This will be used to communicate with LAMS
            //        jscript += "\t\t\tfunction getParams(){\n";
            //        jscript += "\t\t\t\tparent.setParams();\n";
            //        jscript += "\t\t\t}\n";
            //        // END ADDED CODE
            //        // THE NEXT CODE WAS ADDED ON Feb 2010. This will be used to communicate with LAMS
            //        jscript += "\t\t\tfunction showButton(){\n";
            //        jscript += "\t\t\t\ttop.contentFrame.frames[0].showButton();\n";
            //        jscript += "\t\t\t}\n";
            //        // END ADDED CODE
            //    }

            //    jscript += "\t\t//-->\n";
            //    jscript += "\t\t</script>\n";
            //    // END ADDED CODE

            //    if (mainClass.Equals("es.eucm.eadventure.engine.EAdventureAppletScorm"))
            //    {
            //        jscript += "\n\t\t<script type='text/javascript' src='eadventure.js'></script>\n";
            //    }

            //    // adding the js which contains the managing debug console functions,FORM tags and css
            //    string stringForm = "";
            //    string css = "";
            //    if (debugScorm)
            //    {
            //        jscript += "\n\t\t<script type='text/javascript' src='debugscorm.js'></script>\n";

            //        FileInfo form = new File("web/debugform.txt");

            //        char buffer[] = new char[(int)(form.length() * 1.5)];
            //        FileInputStream fis = new FileInputStream(form);
            //        BufferedReader br = new BufferedReader(
            //              new InputStreamReader(fis));
            //        br.read(buffer, 0, (int)form.length());
            //        br.close();
            //        stringForm = new string(buffer, 0, (int)form.length());

            //        css = "\n\t\t<link rel='stylesheet' type='text/css' href='debugger.css'/>\n";



            //    }

            //    string webPage = "<html>\n" + "\t<head>\n" + css +

            //    //"\t\t<script type='text/javascript' src='commapi.js'></script>\n"+
            //    //"\t\t<script type='text/javascript' src='ajax-wrapper.js'></script>\n"+
            //    //"\t\t<script type='text/javascript' src='egame.js'></script>\n"+

            //    //"\t\t<param name=\"USER_ID\" value=\"567\"/>\n" + "\t\t<param name=\"RUN_ID\" value=\"5540\"/>\n" +
            //    //The game is initating.. please be patient while the digital sign is verified

            //    jscript + "\t</head>\n" + stringForm + "\t<body>\n" + "\t\t<applet code=\"" + mainClass + "\" archive=\"./" + loName + ".jar\" name=\"eadventure\" id=\"eadventure\" " + (windowed ? "width=\"200\" height=\"150\"" : "width=\"800\" height=\"600\"") + " MAYSCRIPT>\n" + "\t\t<param name=\"WINDOWED\" value=\"" + (windowed ? "yes" : "no") + "\"/>\n" + "\t\t<param name=\"java_arguments\" value=\"-Xms256m -Xmx512m\"/>\n" + "\t\t<param name=\"image\" value=\"splashScreen.gif\"/>\n";

            //    // Add additional params
            //    if (additionalParams != null)
            //    {
            //        for (string param: additionalParams.keySet())
            //        {
            //            if (param != null && additionalParams.get(param) != null)
            //            {
            //                string value = additionalParams.get(param);
            //                webPage += "\t\t<param name=\"" + param + "\" value=\"" + value + "\"/>\n";
            //            }
            //        }
            //    }
            //    webPage += "\t\t</applet>\n" + "<div id=\"loadingMessage\"><p><b>" + TC.get("Applet.LoadingMessage") + "</b></p></div>\n" + (debugScorm ? "\t\t<div id=\"report\"></div>\n" : "") + "\t</body>\n" + "</html>\n";

            //    File pageFile = new File(tempDir + "/" + loName + ".html");
            //    pageFile.createNewFile();
            //    OutputStream is = new FileOutputStream(pageFile);
            //    is.write(webPage.getBytes());
            //    is.close();

            //    dataSaved = true;

            //}
            //catch (FileNotFoundException e)
            //{
            //    ReportDialog.GenerateErrorReport(e, true, "UNKNOWNERROR");
            //    dataSaved = false;
            //}
            //catch (IOException e)
            //{
            //    ReportDialog.GenerateErrorReport(e, true, "UNKNOWNERROR");
            //    dataSaved = false;
            //}
            return dataSaved;
        }

        /**
         * Returns the text of a simple manifest file with the main class as
         * specified by argument
         * 
         * @param destinyFile
         * @param mainClass
         */

        public static string defaultManifestFile(string mainClass)
        {

            string manifest = "Manifest-Version: 1.0\n" + "Ant-Version: Apache Ant 1.7.0\n" +
                              "Created-By: 1.6.0_02-b06 (Sun Microsystems Inc.)\n" + "Main-Class: " + mainClass + "\n";

            return manifest;
        }

        /**
         * Exports the game as a .ead file
         * 
         * @param projectDirectory
         * @param destinyEADPath
         * @return
         */

        public static bool export(string projectDirectory, string destinyEADPath)
        {

            bool exported = false;

            System.IO.File.Create(destinyEADPath).Close();
            exported = true;
            return exported;
            //return true;
        }

        /**
         * Copies all localized images of the GUI (which are under gui/options/language_ID/*.png) for the current
         * editor language to a temp folder. This makes merging these files with the project folder and other libraries
         * much easier.
         *
         * @return The temp folder that contains the whole gui/options/language_ID/*.png structure that must be copied
         * ( this folder is parent of "gui").
         */
        //private static File addLocalizedGUIImages()
        //{

        //    File parentTempDir = new File("web/temp/configPanel/");
        //    parentTempDir.deleteAll();
        //    File destinyDir = new File("web/temp/configPanel/gui/options/" + Controller.getInstance().getLanguage() + "/");
        //    destinyDir.mkdirs();
        //    File sourceDir = new File("gui/options/" + Controller.getInstance().getLanguage() + "/");
        //    File.copyAllTo(sourceDir, destinyDir);
        //    return parentTempDir;
        //}

        //public static void addNeededLibrariesToJar(ZipOutputStream os, Controller controller)
        //{

        //    File.addJarContentsToZip("jars/tritonus_share.jar", os);
        //    // File.addJarContentsToZip("jars/plugin.jar", os);
        //    File.addJarContentsToZip("jars/mp3spi1.9.4.jar", os);
        //    File.addJarContentsToZip("jars/jl1.0.jar", os);
        //    File.addJarContentsToZip("jars/jmf.jar", os);
        //    //mails libraries are always added because they are needed to show report error dialog
        //    // TODO mirar de donde salen las referencias a este jar, supuestamente para el report dialog no hace falta
        //    File.addJarContentsToZip("jars/mailapi.jar", os);
        //    File.addJarContentsToZip("jars/smtp.jar", os);
        //    File.addJarContentsToZip("jars/activation.jar", os);
        //    //XXX Uncomment these three lines to activate the game log
        //    //File.addJarContentsToZip( "jars/httpclient-4.1.3.jar", os );
        //    //File.addJarContentsToZip( "jars/httpcore-4.1.4.jar", os );
        //    //File.addJarContentsToZip( "jars/commons-logging-1.1.1.jar", os );

        //    bool needsFreeTts = false;

        //    bool needsJFFMpeg = false;



        //    for (ChapterDataControl chapter :controller.getCharapterList().getChapters())
        //    {
        //        for (CutsceneDataControl cutscene : chapter.getCutscenesList().getCutscenes())
        //        {
        //            if (cutscene.getType() == Controller.CUTSCENE_VIDEO)
        //                needsJFFMpeg = true;
        //        }
        //        for (NPCDataControl npc: chapter.getNPCsList().getNPCs())
        //        {
        //            if (npc.isAlwaysSynthesizer())
        //                needsFreeTts = true;
        //        }
        //        if (chapter.getPlayer().isAlwaysSynthesizer())
        //            needsFreeTts = true;
        //        for (ConversationDataControl conversation: chapter.getConversationsList().getConversations())
        //        {
        //            for (ConversationNodeView cnv : conversation.getAllNodes())
        //            {
        //                for (int i = 0; i < cnv.getLineCount(); i++)
        //                    if (cnv.getConversationLine(i).getSynthesizerVoice())
        //                        needsFreeTts = true;
        //            }
        //        }
        //    }

        //    if (needsFreeTts)
        //    {
        //        File.addJarContentsToZip("jars/cmu_time_awb.jar", os);
        //        File.addJarContentsToZip("jars/cmu_us_kal.jar", os);
        //        File.addJarContentsToZip("jars/cmudict04.jar", os);
        //        File.addJarContentsToZip("jars/cmulex.jar", os);
        //        File.addJarContentsToZip("jars/cmutimelex.jar", os);

        //        File.addJarContentsToZip("jars/freetts.jar", os);
        //        File.addJarContentsToZip("jars/en_us.jar", os);

        //    }
        //    if (needsJFFMpeg)
        //    {
        //        File.addJarContentsToZip("jars/jffmpeg-1.1.0.jar", os);
        //    }


        //    File.addFileToZip(new File(ReleaseFolders.getLanguageFilePath4Engine(Controller.getInstance().getLanguage())), "i18n/engine/en_EN.xml", os);
        //}

        /**
         * Exports the game as a jar file
         * 
         * @param projectDirectory
         * @param destinyJARPath
         * @param controller 
         * @return
         */

        public static bool exportStandalone(string projectDirectory, string destinyJARPath)
        {

            bool exported = true;
            //TODO: implementation
            //try
            //{
            //    // Destiny file
            //    File destinyJarFile = new File(destinyJARPath);

            //    // Create output stream		
            //    FileOutputStream mergedFile = new FileOutputStream(destinyJarFile);

            //    // Create zipoutput stream
            //    ZipOutputStream os = new ZipOutputStream(mergedFile);

            //    // Create and copy the manifest into the output stream
            //    string manifest = Writer.defaultManifestFile("es.eucm.eadventure.engine.EAdventureStandalone");
            //    ZipEntry manifestEntry = new ZipEntry("META-INF/MANIFEST.MF");
            //    os.putNextEntry(manifestEntry);
            //    os.write(manifest.getBytes());
            //    os.closeEntry();
            //    os.flush();

            //    // Merge projectDirectory and web/eAdventure_temp.jar into output stream
            //    File tempFile = addLocalizedGUIImages();
            //    File.mergeZipAndDirToJar(os, "web/eAdventure_temp.jar", projectDirectory, tempFile.getAbsolutePath());
            //    addNeededLibrariesToJar(os, Controller.getInstance());


            //    os.close();
            //}
            //catch (FileNotFoundException e)
            //{
            //    exported = false;
            //    ReportDialog.GenerateErrorReport(e, true, "UNKNOWNERROR");
            //}
            //catch (IOException e)
            //{
            //    exported = false;
            //    ReportDialog.GenerateErrorReport(e, true, "UNKNOWNERROR");
            //}

            return exported;

        }

        private static List<AdaptationRule> getAdaptationRules(List<Chapter> cs)
        {
            List<AdaptationRule> adapt = new List<AdaptationRule>();

            foreach (Chapter chapter in cs)
            {
                foreach (AdaptationProfile profile in chapter.getAdaptationProfiles())
                {
                    adapt.AddRange(profile.getRules());
                }
            }

            return adapt;
        }

        private static List<AssessmentRule> getAssessmemtRules(List<Chapter> cs)
        {
            List<AssessmentRule> assess = new List<AssessmentRule>();

            foreach (Chapter chapter in cs)
            {
                foreach (AssessmentProfile profile in chapter.getAssessmentProfiles())
                {
                    assess.AddRange(profile.getRules());
                }
            }
            return assess;
        }

        /**
         * Returns a set of tabulations, equivalent to the given number.
         * 
         * @param tabulations
         *            Number of tabulations
         */

        private static string getTab(int tabulations)
        {

            string tab = "";
            for (int i = 0; i < tabulations; i++)
                tab += "\t";
            return tab;
        }
    }
}
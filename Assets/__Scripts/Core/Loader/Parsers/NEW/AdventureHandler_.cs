using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;


public class AdventureHandler_
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
     * Adventure data being read.
     */
    private AdventureData adventureData;

    private List<Chapter> chapters;

    /**
     * Constructor.
     * 
     * @param zipFile
     *            Path to the zip file which helds the chapter files
     */
    public AdventureHandler_(AdventureData adventuredata)
    {
        adventureData = adventuredata;
        chapters = new List<Chapter>();
    }

    private string getXmlContent(string path)
    {
        string xml = "";
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) {
                xml = System.IO.File.ReadAllText(path);

            FileInfo fi = new FileInfo(path);
            directory = fi.DirectoryName + "\\";
        }else
#endif
            switch (ResourceManager.Instance.getLoadingType())
            {
                case ResourceManager.LoadingType.RESOURCES_LOAD:
                    directory = path.Split('/')[0] + "/";
                    if (path.Contains(".xml"))
                    {
                        path = path.Replace(".xml", "");
                    }
                    TextAsset ta = Resources.Load(path) as TextAsset;
                    if (ta == null)
                    {
                        Debug.Log("Can't load Descriptor file: " + path);
                        return "";
                    }
                    else
                        xml = ta.text;
                    break;
                case ResourceManager.LoadingType.SYSTEM_IO:
                    xml = System.IO.File.ReadAllText(path);

                    directory = "";
                    string[] parts = path.Split('/');

                    for (int i = 0; i < parts.Length - 1; i++)
                        directory += parts[i] + "/";

                    break;
            }

        return xml;
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

	string directory = "";
    string tmpString = "";
    public void Parse(string path)
    {
		XmlDocument xmld = new XmlDocument ();

        string xml = getXmlContent(path);
        if (string.IsNullOrEmpty(xml))
            return;

		xmld.LoadXml(xml);

        XmlElement element = xmld.DocumentElement
            ,descriptor     = (XmlElement) element.SelectSingleNode ("/game-descriptor")
            ,title          = (XmlElement) descriptor.SelectSingleNode ("title")
            ,description    = (XmlElement) descriptor.SelectSingleNode ("description")
            ,configuration  = (XmlElement) descriptor.SelectSingleNode ("configuration")
            ,contents       = (XmlElement) descriptor.SelectSingleNode ("contents");



        if (descriptor != null) {
            tmpString = descriptor.GetAttribute ("versionNumber");

            if (!string.IsNullOrEmpty(tmpString))
            {
                adventureData.setVersionNumber (tmpString);
            }
        }

        if (title != null) {
            tmpString = title.InnerText;

            if (!string.IsNullOrEmpty(tmpString))
            {
                adventureData.setTitle (tmpString);
            }
        }

        if (description != null) {
            tmpString = description.InnerText;

            if (!string.IsNullOrEmpty(tmpString))
            {
                adventureData.setDescription (tmpString);
            }
        }

        if (configuration != null)
        {
            tmpString = configuration.GetAttribute ("keepShowing");
            if (!string.IsNullOrEmpty(tmpString))
            {
                adventureData.setKeepShowing (tmpString.Equals("yes"));
            }

            tmpString = configuration.GetAttribute ("keyboard-navigation");
            if (!string.IsNullOrEmpty(tmpString))
            {
                adventureData.setKeyboardNavigation (tmpString.Equals("enabled"));
            }

            tmpString = configuration.GetAttribute ("defaultClickAction");
            if (!string.IsNullOrEmpty(tmpString))
            {
                if(tmpString.Equals("showDetails"))
                    adventureData.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_DETAILS);
                else if(tmpString.Equals("showDetails"))
                    adventureData.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_ACTIONS);
            }

            tmpString = configuration.GetAttribute ("perspective");
            if (!string.IsNullOrEmpty(tmpString))
            {
                if(tmpString.Equals("regular"))
                    adventureData.setPerspective(DescriptorData.Perspective.REGULAR);
                else if(tmpString.Equals("isometric"))
                    adventureData.setPerspective(DescriptorData.Perspective.ISOMETRIC);
            }

            tmpString = configuration.GetAttribute ("dragBehaviour");
            if (!string.IsNullOrEmpty(tmpString))
            {
                if(tmpString.Equals("considerNonTargets"))
                    adventureData.setDragBehaviour(DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS);
                else if(tmpString.Equals("ignoreNonTargets"))
                    adventureData.setDragBehaviour(DescriptorData.DragBehaviour.IGNORE_NON_TARGETS);
            }


            XmlElement gui = (XmlElement) configuration.SelectSingleNode ("gui");
            if (gui != null)
            {
                tmpString = gui.GetAttribute ("type");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    if(tmpString.Equals("traditional"))
                        adventureData.setGUIType(DescriptorData.GUI_TRADITIONAL);
                    else if(tmpString.Equals("contextual"))
                        adventureData.setGUIType(DescriptorData.GUI_CONTEXTUAL);
                }

                tmpString = gui.GetAttribute ("customized");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    adventureData.setGUI(adventureData.getGUIType(), tmpString.Equals("yes"));
                }

                tmpString = gui.GetAttribute ("inventoryPosition");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    switch (tmpString) {
                    case "none":            adventureData.setInventoryPosition(DescriptorData.INVENTORY_NONE); break;
                    case "top_bottom":      adventureData.setInventoryPosition(DescriptorData.INVENTORY_TOP_BOTTOM); break;
                    case "top":             adventureData.setInventoryPosition(DescriptorData.INVENTORY_TOP); break;
                    case "bottom":          adventureData.setInventoryPosition(DescriptorData.INVENTORY_BOTTOM); break;
                    case "fixed_top":       adventureData.setInventoryPosition(DescriptorData.INVENTORY_FIXED_TOP); break;
                    case "fixed_bottom":    adventureData.setInventoryPosition(DescriptorData.INVENTORY_FIXED_BOTTOM); break;
                    }
                }

                XmlNodeList cursors = gui.SelectNodes ("cursors/cursor");
                foreach (XmlElement cursor in cursors) {
                    string type = ""; string uri = "";

                    tmpString = cursor.GetAttribute ("type");
                    if (!string.IsNullOrEmpty(tmpString)){
                        type = tmpString;
                    }

                    tmpString = cursor.GetAttribute ("uri");
                    if (!string.IsNullOrEmpty(tmpString)){
                        uri = tmpString;
                    }

                    adventureData.addCursor(type, uri);
                }

                XmlNodeList buttons = gui.SelectNodes ("buttons/button");
                foreach (XmlElement button in buttons) {
                    string type = ""; string uri = ""; string action = "";

                    tmpString = button.GetAttribute ("type");
                    if (!string.IsNullOrEmpty(tmpString)){
                        type = tmpString;
                    }

                    tmpString = button.GetAttribute ("uri");
                    if (!string.IsNullOrEmpty(tmpString)){
                        uri = tmpString;
                    }

                    tmpString = button.GetAttribute ("action");
                    if (!string.IsNullOrEmpty(tmpString)){
                        action = tmpString;
                    }

                    adventureData.addButton(action, type, uri);
                }

                XmlNodeList arrows = gui.SelectNodes ("cursors/cursor");
                foreach (XmlElement arrow in arrows) {
                    string type = ""; string uri = "";

                    tmpString = arrow.GetAttribute ("type");
                    if (!string.IsNullOrEmpty(tmpString)){
                        type = tmpString;
                    }

                    tmpString = arrow.GetAttribute ("uri");
                    if (!string.IsNullOrEmpty(tmpString)){
                        uri = tmpString;
                    }

                    adventureData.addArrow(type, uri);
                }
            }

            XmlElement mode = (XmlElement) configuration.SelectSingleNode ("mode");
            if (mode != null)
            {
                tmpString = mode.GetAttribute ("playerTransparent");
                if (!string.IsNullOrEmpty(tmpString)){
                    adventureData.setPlayerMode (tmpString.Equals ("yes") ? DescriptorData.MODE_PLAYER_1STPERSON : DescriptorData.MODE_PLAYER_3RDPERSON);
                }
            }

            XmlElement graphics = (XmlElement) configuration.SelectSingleNode ("graphics");
            if (graphics != null)
            {
                tmpString = graphics.GetAttribute ("playerTransparent");
                if (!string.IsNullOrEmpty(tmpString)){
                    switch (tmpString) {
                    case "windowed":    adventureData.setGraphicConfig(DescriptorData.GRAPHICS_WINDOWED); break;
                    case "fullscreen":  adventureData.setGraphicConfig(DescriptorData.GRAPHICS_FULLSCREEN); break;
                    case "blackbkg":    adventureData.setGraphicConfig(DescriptorData.GRAPHICS_BLACKBKG); break;
                    }
                }
            }
        }

        if (contents != null) {
            Chapter currentChapter;

            XmlNodeList chapters = contents.SelectNodes ("chapter");
            foreach (XmlElement chapter in chapters) {
                currentChapter = new Chapter ();

                string chapterPath = "";
                tmpString = chapter.GetAttribute ("path");
                if (!string.IsNullOrEmpty(tmpString)){
                    chapterPath = tmpString;
                }
                currentChapter.setChapterPath(directory + chapterPath);

                ChapterHandler_ chapterParser = new ChapterHandler_(currentChapter);
				chapterParser.Parse (directory + chapterPath);

                title = (XmlElement) chapter.SelectSingleNode ("title");
                if (title != null) {
                    tmpString = title.InnerText;

                    if (!string.IsNullOrEmpty(tmpString))
                    {
                        currentChapter.setTitle (tmpString);
                    }
                }

                description = (XmlElement) chapter.SelectSingleNode ("description");
                if (description != null) {
                    tmpString = title.InnerText;

                    if (!string.IsNullOrEmpty(tmpString))
                    {
                        currentChapter.setDescription (tmpString);
                    }
                }


                XmlElement adaptation = (XmlElement) chapter.SelectSingleNode ("adaptation-configuration");
                if (adaptation != null) {
                    tmpString = adaptation.GetAttribute ("path");
                    if (!string.IsNullOrEmpty (tmpString)) {
                        string adaptationName = tmpString;
                        // delete the path's characteristics
                        adaptationName = adaptationName.Substring (adaptationName.IndexOf ("/") + 1);
                        adaptationName = adaptationName.Substring (0, adaptationName.IndexOf ("."));
                        currentChapter.setAdaptationName (adaptationName);
                    }
                }

                XmlElement assestment = (XmlElement) chapter.SelectSingleNode ("assessment-configuration");
                if (adaptation != null) {
                    tmpString = adaptation.GetAttribute ("path");
                    if (!string.IsNullOrEmpty (tmpString)) {
                        string assessmentName = tmpString;
                        // delete the path's characteristics
                        assessmentName = assessmentName.Substring(assessmentName.IndexOf("/") + 1);
                        assessmentName = assessmentName.Substring(0, assessmentName.IndexOf("."));
                        currentChapter.setAssessmentName(assessmentName);
                    }
                }

                adventureData.addChapter (currentChapter);
            }
        }

        /*if (qName.EndsWith("automatic-commentaries"))
        {
            adventureData.setCommentaries(true);
        }*/
    }
}
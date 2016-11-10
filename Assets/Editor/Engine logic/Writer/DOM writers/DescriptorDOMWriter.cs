using UnityEngine;
using System.Collections;
using System.Xml;

public class DescriptorDOMWriter
{
    /**
        * Private constructor.
        */

    private DescriptorDOMWriter()
    {

    }

    /**
     * Returns the DOM element for the descriptor of the adventure.
     * 
     * @param adventureData
     *            Adventure from which the descriptor will be taken
     * @param valid
     *            True if the adventure is valid (can be executed in the
     *            engine), false otherwise
     * @return DOM element with the descriptor data
     */

    public static XmlNode buildDOM(AdventureDataControl adventureData, bool valid)
    {

        XmlNode descriptorNode = null;

        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        descriptorNode = doc.CreateElement("game-descriptor");
        ((XmlElement) descriptorNode).SetAttribute("versionNumber", adventureData.getAdventureData().getVersionNumber());

        // Create and append the title
        XmlNode adventureTitleNode = doc.CreateElement("title");
        adventureTitleNode.AppendChild(doc.CreateTextNode(adventureData.getTitle()));
        descriptorNode.AppendChild(adventureTitleNode);

        // Create and append the description
        XmlNode adventureDescriptionNode = doc.CreateElement("description");
        adventureDescriptionNode.AppendChild(doc.CreateTextNode(adventureData.getDescription()));
        descriptorNode.AppendChild(adventureDescriptionNode);

        // Create and append the "invalid" tag (if necessary)
        if (!valid)
            descriptorNode.AppendChild(doc.CreateElement("invalid"));
        if (adventureData.isCommentaries())
        {
            descriptorNode.AppendChild(doc.CreateElement("automatic-commentaries"));
        }

        // Create and append the configuration
        XmlNode configurationNode = doc.CreateElement("configuration");
        if (adventureData.isKeepShowing())
            ((XmlElement) configurationNode).SetAttribute("keepShowing", "yes");
        else
            ((XmlElement) configurationNode).SetAttribute("keepShowing", "no");

        if (adventureData.isKeyboardNavigationEnabled())
            ((XmlElement) configurationNode).SetAttribute("keyboard-navigation", "enabled");
        else
            ((XmlElement) configurationNode).SetAttribute("keyboard-navigation", "disabled");


        switch (adventureData.getDefaultClickAction())
        {
            case DescriptorData.DefaultClickAction.SHOW_DETAILS:
                ((XmlElement) configurationNode).SetAttribute("defaultClickAction", "showDetails");
                break;
            case DescriptorData.DefaultClickAction.SHOW_ACTIONS:
                ((XmlElement) configurationNode).SetAttribute("defaultClickAction", "showActions");
                break;
        }

        switch (adventureData.getPerspective())
        {
            case DescriptorData.Perspective.REGULAR:
                ((XmlElement) configurationNode).SetAttribute("perspective", "regular");
                break;
            case DescriptorData.Perspective.ISOMETRIC:
                ((XmlElement) configurationNode).SetAttribute("perspective", "isometric");
                break;
        }

        switch (adventureData.getDragBehaviour())
        {
            case DescriptorData.DragBehaviour.IGNORE_NON_TARGETS:
                ((XmlElement) configurationNode).SetAttribute("dragBehaviour", "ignoreNonTargets");
                break;
            case DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS:
                ((XmlElement) configurationNode).SetAttribute("dragBehaviour", "considerNonTargets");
                break;
        }

        //GUI Element
        XmlElement guiElement = doc.CreateElement("gui");
        if (adventureData.getGUIType() == DescriptorData.GUI_TRADITIONAL)
            guiElement.SetAttribute("type", "traditional");
        else if (adventureData.getGUIType() == DescriptorData.GUI_CONTEXTUAL)
            guiElement.SetAttribute("type", "contextual");

        switch (adventureData.getInventoryPosition())
        {
            case DescriptorData.INVENTORY_NONE:
                guiElement.SetAttribute("inventoryPosition", "none");
                break;
            case DescriptorData.INVENTORY_TOP_BOTTOM:
                guiElement.SetAttribute("inventoryPosition", "top_bottom");
                break;
            case DescriptorData.INVENTORY_TOP:
                guiElement.SetAttribute("inventoryPosition", "top");
                break;
            case DescriptorData.INVENTORY_BOTTOM:
                guiElement.SetAttribute("inventoryPosition", "bottom");
                break;
            case DescriptorData.INVENTORY_FIXED_TOP:
                guiElement.SetAttribute("inventoryPosition", "fixed_top");
                break;
            case DescriptorData.INVENTORY_FIXED_BOTTOM:
                guiElement.SetAttribute("inventoryPosition", "fixed_bottom");
                break;

        }

        if (adventureData.getCursors().Count > 0)
        {
            XmlNode cursorsNode = doc.CreateElement("cursors");
            foreach (CustomCursor cursor in adventureData.getCursors())
            {
                XmlElement currentCursor = doc.CreateElement("cursor");
                currentCursor.SetAttribute("type", cursor.getType());
                currentCursor.SetAttribute("uri", cursor.getPath());
                cursorsNode.AppendChild(currentCursor);
            }
            guiElement.AppendChild(cursorsNode);
        }

        if (adventureData.getButtons().Count > 0)
        {
            XmlNode buttonsNode = doc.CreateElement("buttons");

            foreach (CustomButton button in adventureData.getButtons())
            {
                XmlElement currentButton = doc.CreateElement("button");
                currentButton.SetAttribute("action", button.getAction());
                currentButton.SetAttribute("type", button.getType());
                currentButton.SetAttribute("uri", button.getPath());
                buttonsNode.AppendChild(currentButton);
            }
            guiElement.AppendChild(buttonsNode);
        }

        if (adventureData.getArrows().Count > 0)
        {
            XmlNode arrowNode = doc.CreateElement("arrows");

            foreach (CustomArrow arrow in adventureData.getArrows())
            {
                XmlElement currentArrow = doc.CreateElement("arrow");
                currentArrow.SetAttribute("type", arrow.getType());
                currentArrow.SetAttribute("uri", arrow.getPath());
                arrowNode.AppendChild(currentArrow);
            }
            guiElement.AppendChild(arrowNode);
        }

        configurationNode.AppendChild(guiElement);

        //Player mode element
        XmlElement playerModeElement = doc.CreateElement("mode");
        if (adventureData.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON)
            playerModeElement.SetAttribute("playerTransparent", "yes");
        else if (adventureData.getPlayerMode() == DescriptorData.MODE_PLAYER_3RDPERSON)
            playerModeElement.SetAttribute("playerTransparent", "no");
        configurationNode.AppendChild(playerModeElement);

        //Graphic config element
        XmlElement graphicConfigElement = doc.CreateElement("graphics");
        if (adventureData.getGraphicConfig() == DescriptorData.GRAPHICS_WINDOWED)
            graphicConfigElement.SetAttribute("mode", "windowed");
        else if (adventureData.getGraphicConfig() == DescriptorData.GRAPHICS_FULLSCREEN)
            graphicConfigElement.SetAttribute("mode", "fullscreen");
        else if (adventureData.getGraphicConfig() == DescriptorData.GRAPHICS_BLACKBKG)
            graphicConfigElement.SetAttribute("mode", "blackbkg");
        configurationNode.AppendChild(graphicConfigElement);

        //Append configurationNode
        descriptorNode.AppendChild(configurationNode);

        // Create and add the contents with the chapters
        XmlNode contentsNode = doc.CreateElement("contents");
        int chapterIndex = 1;
        foreach (Chapter chapter in adventureData.getChapters())
        {
            // Create the chapter and add the path to it
            XmlElement chapterElement = doc.CreateElement("chapter");
            chapterElement.SetAttribute("path", "chapter" + chapterIndex++ + ".xml");

            // Create and append the title
            XmlNode chapterTitleNode = doc.CreateElement("title");
            chapterTitleNode.AppendChild(doc.CreateTextNode(chapter.getTitle()));
            chapterElement.AppendChild(chapterTitleNode);

            // Create and append the description
            XmlNode chapterDescriptionNode = doc.CreateElement("description");
            chapterDescriptionNode.AppendChild(doc.CreateTextNode(chapter.getDescription()));
            chapterElement.AppendChild(chapterDescriptionNode);

            // Create and append the adaptation configuration
            /*if( !chapter.getAdaptationPath( ).equals( "" ) ) {
                	Element adaptationPathElement = doc.CreateElement( "adaptation-configuration" );
                	adaptationPathElement.SetAttribute( "path", chapter.getAdaptationPath( ) );
                	chapterElement.AppendChild( adaptationPathElement );
                }

                // Create and append the assessment configuration
                if( !chapter.getAssessmentPath( ).equals( "" ) ) {
                	Element assessmentPathElement = doc.CreateElement( "assessment-configuration" );
                	assessmentPathElement.SetAttribute( "path", chapter.getAssessmentPath( ) );
                	chapterElement.AppendChild( assessmentPathElement );
                }*/

            // Store the node
            contentsNode.AppendChild(chapterElement);
        }
        // Store the chapters in the descriptor
        descriptorNode.AppendChild(contentsNode);


        return descriptorNode;
    }
}
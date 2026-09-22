using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System;
using System.Linq;
using System.Globalization;
using UnityEditor;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(AdventureDataControl))]
    public class DescriptorDOMWriter : ParametrizedDOMWriter
    {
        public DescriptorDOMWriter()
        {

        }
        

        protected override string GetElementNameFor(object target)
        {
            return "game-descriptor";
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
         public class InvalidAdventureDataControlParam : IDOMWriterParam {

         }

        /// <summary>
        /// Fills the descriptor node with the addventure data control data
        /// </summary>
        /// <param name="node"></param>
        /// <param name="target"></param>
        /// <param name="options">InvalidAdventureDataControlParam is accepted</param>
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var adventureData = target as AdventureDataControl;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            ((XmlElement)node).SetAttribute("versionNumber", adventureData.getAdventureData().getVersionNumber());

            // Create and append the title
            XmlNode adventureTitleNode = doc.CreateElement("title");
            adventureTitleNode.AppendChild(doc.CreateTextNode(adventureData.getTitle()));
            node.AppendChild(adventureTitleNode);

            // Create and append the description
            XmlNode adventureDescriptionNode = doc.CreateElement("description");
            adventureDescriptionNode.AppendChild(doc.CreateTextNode(adventureData.getDescription()));
            node.AppendChild(adventureDescriptionNode);

            // Create and append the description
            XmlNode applicationIdentifierNode = doc.CreateElement("application-identifier");
            applicationIdentifierNode.AppendChild(doc.CreateTextNode(PlayerSettings.applicationIdentifier));
            node.AppendChild(applicationIdentifierNode);

            // Create and append the "invalid" tag (if necessary)
            var invalid = options.Any(option => option is InvalidAdventureDataControlParam);
            if (invalid)
            {
                node.AppendChild(doc.CreateElement("invalid"));
            }

            // Automatic commentaries
            if (adventureData.isCommentaries())
            {
                node.AppendChild(doc.CreateElement("automatic-commentaries"));
            }

            // Create and append the configuration
            XmlElement configurationNode = doc.CreateElement("configuration");

            // Keep Showing
            var keepShowingValue = adventureData.isKeepShowing() ? "yes" : "no";
            configurationNode.SetAttribute("keepShowing", keepShowingValue);

            // Keyboard Navigation
            var keyboardNavigationValue = adventureData.isKeepShowing() ? "enabled" : "disabled";
            configurationNode.SetAttribute("keyboard-navigation", keyboardNavigationValue);

            // Default click action
            switch (adventureData.getDefaultClickAction())
            {
                case DescriptorData.DefaultClickAction.SHOW_DETAILS:
                    configurationNode.SetAttribute("defaultClickAction", "showDetails");
                    break;
                case DescriptorData.DefaultClickAction.SHOW_ACTIONS:
                    configurationNode.SetAttribute("defaultClickAction", "showActions");
                    break;
            }

            // Perspective
            switch (adventureData.getPerspective())
            {
                case DescriptorData.Perspective.REGULAR:
                    configurationNode.SetAttribute("perspective", "regular");
                    break;
                case DescriptorData.Perspective.ISOMETRIC:
                    configurationNode.SetAttribute("perspective", "isometric");
                    break;
            }

            // Drag Behaviour
            switch (adventureData.getDragBehaviour())
            {
                case DescriptorData.DragBehaviour.IGNORE_NON_TARGETS:
                    configurationNode.SetAttribute("dragBehaviour", "ignoreNonTargets");
                    break;
                default: // CONSIDER_NON_TARGETS
                    configurationNode.SetAttribute("dragBehaviour", "considerNonTargets");
                    break;
            }

            // GUI Element
            XmlElement guiElement = doc.CreateElement("gui");
            switch (adventureData.getGUIType())
            {
                case DescriptorData.GUI_TRADITIONAL:
                    guiElement.SetAttribute("type", "ignoreNonTargets");
                    break;
                default: // GUI CONTEXTUAL
                    guiElement.SetAttribute("type", "contextual");
                    break;
            }

            // Inventory Position
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
                case DescriptorData.INVENTORY_ICON_FREEPOS:
                    guiElement.SetAttribute("inventoryPosition", "icon");
                    guiElement.SetAttribute("inventoryImage", adventureData.getInventoryImage());
                    guiElement.SetAttribute("inventoryX", adventureData.getInventoryCoords().x.ToString(CultureInfo.InvariantCulture));
                    guiElement.SetAttribute("inventoryY", adventureData.getInventoryCoords().y.ToString(CultureInfo.InvariantCulture));
                    guiElement.SetAttribute("inventoryScale", adventureData.getInventoryScale().ToString(CultureInfo.InvariantCulture));
                    break;

            }

            // Cursors
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

            // Buttons
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

            // Arrows
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
            var isPlayerTransparent = adventureData.getPlayerMode() == DescriptorData.MODE_PLAYER_1STPERSON;
            playerModeElement.SetAttribute("playerTransparent", isPlayerTransparent ? "yes" : "no");
            configurationNode.AppendChild(playerModeElement);

            //Graphic config element
            XmlElement graphicConfigElement = doc.CreateElement("graphics");
            var graphicsMode = "fullscreen"; // GRAPHICS_FULLSCREEN
            switch (adventureData.getGraphicConfig())
            {
                case DescriptorData.GRAPHICS_WINDOWED: graphicsMode = "windowed"; break;
                case DescriptorData.GRAPHICS_BLACKBKG: graphicsMode = "blackbkg"; break;
            }
            graphicConfigElement.SetAttribute("mode", graphicsMode);

            configurationNode.AppendChild(graphicConfigElement);

            // Show Save Load
            var showSaveLoad = adventureData.isShowSaveLoad() ? "yes" : "no";
            configurationNode.SetAttribute("show-save-load", showSaveLoad);

            // Show Reset
            var showReset = adventureData.isShowReset() ? "yes" : "no";
            configurationNode.SetAttribute("show-reset", showReset);

            // Autosave
            var autoSave = adventureData.isAutoSave() ? "yes" : "no";
            configurationNode.SetAttribute("autosave", autoSave);

            // Save on suspend
            var saveOnSuspend = adventureData.isSaveOnSuspend() ? "yes" : "no";
            configurationNode.SetAttribute("save-on-suspend", saveOnSuspend);

            // Restore after open
            var restoreAfterOpen = adventureData.isRestoreAfterOpen() ? "yes" : "no";
            configurationNode.SetAttribute("restore-after-open", restoreAfterOpen);

            //Append configurationNode
            node.AppendChild(configurationNode);

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

                // Store the node
                contentsNode.AppendChild(chapterElement);
            }


            // Extension Objects
            XmlNode extensionObjects = doc.CreateElement("extension-objects");
            foreach (var type in adventureData.getAdventureData().getObjectTypes())
            {
                foreach (var tosave in adventureData.getAdventureData().getObjects(type))
                {
                    DOMWriterUtility.DOMWrite(extensionObjects, tosave, options);
                }
            }
            node.AppendChild(extensionObjects);

            // Store the chapters in the descriptor
            node.AppendChild(contentsNode);
        }
    }
}
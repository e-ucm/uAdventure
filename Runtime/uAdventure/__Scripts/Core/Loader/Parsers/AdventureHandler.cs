using System.Xml;
using System.Collections.Generic;

using uAdventure.Runner;
using System.Globalization;

namespace uAdventure.Core
{
    public class AdventureHandler : XmlHandler<AdventureData>
    {
        public AdventureHandler(AdventureData adventureData, ResourceManager resourceManager, List<Incidence> incidences) 
            : base(adventureData, resourceManager, incidences)
        {
        }

        protected override AdventureData CreateObject()
        {
            return new AdventureData();
        }

        protected override AdventureData ParseXml(XmlDocument doc)
        {
            XmlElement element = doc.DocumentElement,
                descriptor     = (XmlElement)element.SelectSingleNode("/game-descriptor"),
                title          = (XmlElement)descriptor.SelectSingleNode("title"),
                description    = (XmlElement)descriptor.SelectSingleNode("description"),
                application_identifier = (XmlElement)descriptor.SelectSingleNode("application-identifier");

            // Basic attributes
            content.setVersionNumber(ExString.Default(descriptor.GetAttribute("versionNumber"), "0"));
            content.setTitle(title.InnerText ?? "");
            content.setDescription(description.InnerText ?? "");
            content.setApplicationIdentifier(application_identifier?.InnerText);

            // Sub nodes
            XmlElement configuration    = (XmlElement)descriptor.SelectSingleNode("configuration"),
                       contents         = (XmlElement)descriptor.SelectSingleNode("contents"),
                       extensionObjects = (XmlElement)descriptor.SelectSingleNode("extension-objects");

            ParseConfiguration(content, configuration, incidences);
            ParseContents(content, contents, resourceManager, incidences);

            if (extensionObjects != null)
            {
                foreach (var el in extensionObjects.ChildNodes)
                {
                    object parsed = DOMParserUtility.DOMParse(el as XmlElement);
                    if (parsed != null)
                    {
                        var t = GroupableTypeAttribute.GetGroupType(parsed.GetType());
                        content.getObjects(t).Add(parsed);
                    }
                }
            }

            ParseImsCPMetadata(content, resourceManager, incidences);

            return base.content;
        }

        private static void ParseContents(AdventureData adventureData, XmlElement contents, ResourceManager resourceManager, List<Incidence> incidences)
        {
            if (contents == null)
            {
                return;
            }

            XmlNodeList chapters = contents.SelectNodes("chapter");
            foreach (XmlElement chapter in chapters)
            {
                string chapterPath = chapter.GetAttribute("path");
                if (!string.IsNullOrEmpty(chapterPath))
                {
                    adventureData.addChapter(Loader.LoadChapter(chapterPath, resourceManager, incidences));
                }
            }
        }

        private static void ParseImsCPMetadata(AdventureData adventureData, ResourceManager resourceManager, List<Incidence> incidences)
        {
            var imsCPMetadataPath = "imscpmetadata.xml";

            if (!string.IsNullOrEmpty(resourceManager.getText(imsCPMetadataPath))) 
            {
                adventureData.setImsCPMetadata(Loader.LoadImsCPMetadata(imsCPMetadataPath, resourceManager, incidences));
            }
        }

        private static void ParseConfiguration(AdventureData adventureData, XmlElement configuration, List<Incidence> incidences)
        {
            if (configuration == null)
            {
                return;
            }

            // Keep showing text until user clicks
            var isKeepShowing = ExString.EqualsDefault(configuration.GetAttribute("keepShowing"), "yes", false);
            adventureData.setKeepShowing(isKeepShowing);

            // Use keys to navigate in the scene
            var isKeyboardNavigation = ExString.EqualsDefault(configuration.GetAttribute("keyboard-navigation"), "enabled", false);
            adventureData.setKeyboardNavigation(isKeyboardNavigation);

            // Default click action for scene elements
            var defaultClickAction = ExString.Default(configuration.GetAttribute("defaultClickAction"), "showDetails");
            switch (defaultClickAction)
            {
                case "showDetails": adventureData.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_DETAILS); break;
                case "showActions": adventureData.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_ACTIONS); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown defaultClickAction type: " + defaultClickAction, null)); break;
            }

            // Perspective for rendering
            var perspective = ExString.Default(configuration.GetAttribute("perspective"), "regular");
            switch (perspective)
            {
                case "regular": adventureData.setPerspective(DescriptorData.Perspective.REGULAR); break;
                case "isometric": adventureData.setPerspective(DescriptorData.Perspective.ISOMETRIC); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown perspective type: " + perspective, null)); break;
            }

            // Drag behaviour configuration
            var dragBehaviour = ExString.Default(configuration.GetAttribute("dragBehaviour"), "considerNonTargets");
            switch (dragBehaviour)
            {
                case "considerNonTargets": adventureData.setDragBehaviour(DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS); break;
                case "ignoreNonTargets": adventureData.setDragBehaviour(DescriptorData.DragBehaviour.IGNORE_NON_TARGETS); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown dragBehaviour type: " + dragBehaviour, null)); break;
            }

            // Show save/load
            var showSaveLoad = ExString.EqualsDefault(configuration.GetAttribute("show-save-load"), "yes", true);
            adventureData.setShowSaveLoad(showSaveLoad);

            // Show reset
            var showReset = ExString.EqualsDefault(configuration.GetAttribute("show-reset"), "yes", true);
            adventureData.setShowReset(showReset);

            // Auto Save
            var isAutoSave = ExString.EqualsDefault(configuration.GetAttribute("autosave"), "yes", false);
            adventureData.setAutoSave(isAutoSave);

            // Save on suspend
            var isSaveOnSuspend = ExString.EqualsDefault(configuration.GetAttribute("save-on-suspend"), "yes", false);
            adventureData.setSaveOnSuspend(isSaveOnSuspend);

            // Restore after open
            var restoreAfterOpen = ExString.EqualsDefault(configuration.GetAttribute("restore-after-open"), "yes", false);
            adventureData.setRestoreAfterOpen(restoreAfterOpen);

            // Sub nodes
            XmlElement gui          = (XmlElement)configuration.SelectSingleNode("gui"),
                mode                = (XmlElement)configuration.SelectSingleNode("mode"),
                graphics            = (XmlElement)configuration.SelectSingleNode("graphics");

            ParseGui(adventureData, gui, incidences);
            ParseMode(adventureData, mode);
            ParseGraphics(adventureData, graphics, incidences);
        }

        private static void ParseGraphics(AdventureData adventureData, XmlElement graphics, List<Incidence> incidences)
        {
            if (graphics == null)
            {
                return;
            }

            var graphicsMode = ExString.Default(graphics.GetAttribute("mode"), "fullscreen");
            switch (graphicsMode)
            {
                case "windowed": adventureData.setGraphicConfig(DescriptorData.GRAPHICS_WINDOWED); break;
                case "fullscreen": adventureData.setGraphicConfig(DescriptorData.GRAPHICS_FULLSCREEN); break;
                case "blackbkg": adventureData.setGraphicConfig(DescriptorData.GRAPHICS_BLACKBKG); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown graphicsMode type: " + graphicsMode, null)); break;
            }
        }

        private static void ParseMode(AdventureData adventureData, XmlElement mode)
        {
            if (mode == null)
            {
                return;
            }
            
            var isPlayerTransparent = ExString.EqualsDefault(mode.GetAttribute("playerTransparent"), "yes", true);
            adventureData.setPlayerMode(isPlayerTransparent ? DescriptorData.MODE_PLAYER_1STPERSON : DescriptorData.MODE_PLAYER_3RDPERSON);
        }

        private static void ParseGui(AdventureData adventureData, XmlElement gui, List<Incidence> incidences)
        {
            if (gui == null)
            {
                return;
            }

            var guiType = ExString.Default(gui.GetAttribute("type"), "contextual");
            switch (guiType)
            {
                case "traditional": adventureData.setGUIType(DescriptorData.GUI_TRADITIONAL); break;
                case "contextual":  adventureData.setGUIType(DescriptorData.GUI_CONTEXTUAL);  break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown GUIType type: " + guiType, null)); break;
            }
            
            var isCustomized = ExString.EqualsDefault(gui.GetAttribute("customized"), "yes", false);
            adventureData.setGUI(adventureData.getGUIType(), isCustomized);

            var inventoryPosition = ExString.Default(gui.GetAttribute("inventoryPosition"), "top_bottom");
            switch (inventoryPosition)
            {
                case "none": adventureData.setInventoryPosition(DescriptorData.INVENTORY_NONE); break;
                case "top_bottom": adventureData.setInventoryPosition(DescriptorData.INVENTORY_TOP_BOTTOM); break;
                case "top": adventureData.setInventoryPosition(DescriptorData.INVENTORY_TOP); break;
                case "bottom": adventureData.setInventoryPosition(DescriptorData.INVENTORY_BOTTOM); break;
                case "fixed_top": adventureData.setInventoryPosition(DescriptorData.INVENTORY_FIXED_TOP); break;
                case "fixed_bottom": adventureData.setInventoryPosition(DescriptorData.INVENTORY_FIXED_BOTTOM); break;
                case "icon":
                    adventureData.setInventoryPosition(DescriptorData.INVENTORY_ICON_FREEPOS);
                    var scale = gui.GetAttribute("inventoryScale");
                    adventureData.setInventoryScale(ExParsers.ParseDefault(gui.GetAttribute("inventoryScale"), CultureInfo.InvariantCulture, 0.2f));
                    var xCoord = ExParsers.ParseDefault(gui.GetAttribute("inventoryX"), CultureInfo.InvariantCulture, 675f);
                    var yCoord = ExParsers.ParseDefault(gui.GetAttribute("inventoryY"), CultureInfo.InvariantCulture, 550f);
                    adventureData.setInventoryCoords(new UnityEngine.Vector2(xCoord, yCoord));
                    adventureData.setInventoryImage(ExString.Default(gui.GetAttribute("inventoryImage"), SpecialAssetPaths.ASSET_DEFAULT_INVENTORY));
                    break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown inventoryPosition type: " + inventoryPosition, null)); break;
            }

            XmlNodeList cursors = gui.SelectNodes("cursors/cursor");
            foreach (XmlElement cursor in cursors)
            {
                string type = ExString.Default(cursor.GetAttribute("type"), ""),
                    uri     = ExString.Default(cursor.GetAttribute("uri"), "");

                adventureData.addCursor(type, uri);
            }

            XmlNodeList buttons = gui.SelectNodes("buttons/button");
            foreach (XmlElement button in buttons)
            {
                string type = ExString.Default(button.GetAttribute("type"), ""),
                    uri     = ExString.Default(button.GetAttribute("uri"), ""),
                    action  = ExString.Default(button.GetAttribute("action"), "");

                adventureData.addButton(action, type, uri);
            }

            XmlNodeList arrows = gui.SelectNodes("cursors/cursor");
            foreach (XmlElement arrow in arrows)
            {
                string type = ExString.Default(arrow.GetAttribute("type"), ""),
                    uri     = ExString.Default(arrow.GetAttribute("uri"), "");

                adventureData.addArrow(type, uri);
            }
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }
    }
}
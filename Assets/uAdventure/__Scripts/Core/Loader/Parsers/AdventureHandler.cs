using System.Xml;
using System.Collections.Generic;

using uAdventure.Runner;

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
                description    = (XmlElement)descriptor.SelectSingleNode("description");

            // Basic attributes
            content.setVersionNumber(ExParsers.ParseDefault(descriptor.GetAttribute("versionNumber"), "0"));
            content.setTitle(title.InnerText ?? "");
            content.setDescription(description.InnerText ?? "");

            // Sub nodes
            XmlElement configuration = (XmlElement)descriptor.SelectSingleNode("configuration"),
                       contents      = (XmlElement)descriptor.SelectSingleNode("contents");

            ParseConfiguration(configuration);
            ParseContents(contents);

            return base.content;
        }

        private void ParseContents(XmlElement contents)
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
                    content.addChapter(Loader.LoadChapter(chapterPath, resourceManager, incidences));
                }
            }
        }

        private void ParseConfiguration(XmlElement configuration)
        {
            if (configuration == null)
            {
                return;
            }

            // Keep showing text until user clicks
            var keepShowing = ExParsers.ParseDefault(configuration.GetAttribute("keepShowing"), "no");
            var isKeepShowing = "yes".Equals(keepShowing);
            content.setKeepShowing(isKeepShowing);

            // Use keys to navigate in the scene
            var keyboardNavigation = ExParsers.ParseDefault(configuration.GetAttribute("keyboard-navigation"), "disabled");
            var isKeyboardNavigation = "enabled".Equals(keyboardNavigation);
            content.setKeyboardNavigation(isKeyboardNavigation);

            // Default click action for scene elements
            var defaultClickAction = ExParsers.ParseDefault(configuration.GetAttribute("defaultClickAction"), "showDetails");
            switch (defaultClickAction)
            {
                case "showDetails": content.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_DETAILS); break;
                case "showActions": content.setDeafultClickAction(DescriptorData.DefaultClickAction.SHOW_ACTIONS); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown defaultClickAction type: " + defaultClickAction, null)); break;
            }

            // Perspective for rendering
            var perspective = ExParsers.ParseDefault(configuration.GetAttribute("perspective"), "regular");
            switch (perspective)
            {
                case "regular": content.setPerspective(DescriptorData.Perspective.REGULAR); break;
                case "isometric": content.setPerspective(DescriptorData.Perspective.ISOMETRIC); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown perspective type: " + perspective, null)); break;
            }

            // Drag behaviour configuration
            var dragBehaviour = ExParsers.ParseDefault(configuration.GetAttribute("dragBehaviour"), "considerNonTargets");
            switch (dragBehaviour)
            {
                case "considerNonTargets": content.setDragBehaviour(DescriptorData.DragBehaviour.CONSIDER_NON_TARGETS); break;
                case "ignoreNonTargets": content.setDragBehaviour(DescriptorData.DragBehaviour.IGNORE_NON_TARGETS); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown dragBehaviour type: " + dragBehaviour, null)); break;
            }

            // Auto Save
            var autosave = ExParsers.ParseDefault(configuration.GetAttribute("autosave"), "yes");
            var isAutoSave = "yes".Equals(autosave);
            content.setAutoSave(isAutoSave);

            // Save on suspend
            var saveOnSuspend = ExParsers.ParseDefault(configuration.GetAttribute("save-on-suspend"), "yes");
            var isSaveOnSuspend = "yes".Equals(saveOnSuspend);
            content.setSaveOnSuspend(isSaveOnSuspend);

            // Sub nodes
            XmlElement gui = (XmlElement)configuration.SelectSingleNode("gui"),
                mode       = (XmlElement)configuration.SelectSingleNode("mode"),
                graphics   = (XmlElement)configuration.SelectSingleNode("graphics");

            ParseGui(gui);
            ParseMode(mode);
            ParseGraphics(graphics);
        }

        private void ParseGraphics(XmlElement graphics)
        {
            if (graphics == null)
            {
                return;
            }

            var mode = ExParsers.ParseDefault(graphics.GetAttribute("mode"), "fullscreen");
            switch (mode)
            {
                case "windowed": content.setGraphicConfig(DescriptorData.GRAPHICS_WINDOWED); break;
                case "fullscreen": content.setGraphicConfig(DescriptorData.GRAPHICS_FULLSCREEN); break;
                case "blackbkg": content.setGraphicConfig(DescriptorData.GRAPHICS_BLACKBKG); break;
            }
        }

        private void ParseMode(XmlElement mode)
        {
            if (mode == null)
            {
                return;
            }

            var playerTransparent = ExParsers.ParseDefault(mode.GetAttribute("playerTransparent"), "yes");
            var isPlayerTransparent = "yes".Equals(playerTransparent);
            content.setPlayerMode(isPlayerTransparent ? DescriptorData.MODE_PLAYER_1STPERSON : DescriptorData.MODE_PLAYER_3RDPERSON);
        }

        private void ParseGui(XmlElement gui)
        {
            if (gui == null)
            {
                return;
            }

            var guiType = ExParsers.ParseDefault(gui.GetAttribute("type"), "contextual");
            switch (guiType)
            {
                case "traditional": content.setGUIType(DescriptorData.GUI_TRADITIONAL); break;
                case "contextual":  content.setGUIType(DescriptorData.GUI_CONTEXTUAL);  break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown GUIType type: " + guiType, null)); break;
            }

            var customized = ExParsers.ParseDefault(gui.GetAttribute("customized"), "no");
            var isCustomized = "yes".Equals(customized);
            content.setGUI(content.getGUIType(), isCustomized);

            var inventoryPosition = ExParsers.ParseDefault(gui.GetAttribute("inventoryPosition"), "top_bottom");
            switch (inventoryPosition)
            {
                case "none": content.setInventoryPosition(DescriptorData.INVENTORY_NONE); break;
                case "top_bottom": content.setInventoryPosition(DescriptorData.INVENTORY_TOP_BOTTOM); break;
                case "top": content.setInventoryPosition(DescriptorData.INVENTORY_TOP); break;
                case "bottom": content.setInventoryPosition(DescriptorData.INVENTORY_BOTTOM); break;
                case "fixed_top": content.setInventoryPosition(DescriptorData.INVENTORY_FIXED_TOP); break;
                case "fixed_bottom": content.setInventoryPosition(DescriptorData.INVENTORY_FIXED_BOTTOM); break;
                default: incidences.Add(Incidence.createDescriptorIncidence("Unknown inventoryPosition type: " + inventoryPosition, null)); break;
            }

            XmlNodeList cursors = gui.SelectNodes("cursors/cursor");
            foreach (XmlElement cursor in cursors)
            {
                string type = ExParsers.ParseDefault(cursor.GetAttribute("type"), ""),
                    uri     = ExParsers.ParseDefault(cursor.GetAttribute("uri"), "");

                content.addCursor(type, uri);
            }

            XmlNodeList buttons = gui.SelectNodes("buttons/button");
            foreach (XmlElement button in buttons)
            {
                string type = ExParsers.ParseDefault(button.GetAttribute("type"), ""),
                    uri     = ExParsers.ParseDefault(button.GetAttribute("uri"), ""),
                    action  = ExParsers.ParseDefault(button.GetAttribute("action"), "");

                content.addButton(action, type, uri);
            }

            XmlNodeList arrows = gui.SelectNodes("cursors/cursor");
            foreach (XmlElement arrow in arrows)
            {
                string type = ExParsers.ParseDefault(arrow.GetAttribute("type"), ""),
                    uri     = ExParsers.ParseDefault(arrow.GetAttribute("uri"), "");

                content.addArrow(type, uri);
            }
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ResourcesDOMWriter
    {

        public const int RESOURCES_NONE = 0;

        public const int RESOURCES_ITEM = 1;

        public const int RESOURCES_SCENE = 2;

        public const int RESOURCES_CUTSCENE = 3;

        public const int RESOURCES_CHARACTER = 4;

        public const int RESOURCES_ANIMATION = 5;

        public const int RESOURCES_CUSTOM_ACTION = 6;

        public const int RESOURCES_BOOK = 7;

        public const int RESOURCES_CONVERSATION_LINE = 8;

        /**
         * Private constructor.
         */

        private ResourcesDOMWriter()
        {

        }

        public static XmlNode buildDOM(ResourcesUni resources)
        {

            return buildDOM(resources, RESOURCES_NONE);
        }

        public static XmlNode buildDOM(ResourcesUni resources, int resourcesType)
        {

            XmlElement resourcesNode = null;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node
            resourcesNode = doc.CreateElement("resources");
            resourcesNode.SetAttribute("name", resources.getName());

            // Append the conditions block (if there is one)
            if (!resources.getConditions().IsEmpty())
            {
                DOMWriterUtility.DOMWrite(resourcesNode, resources.getConditions());
            }

            // Take the array of types and values of the assets
            string[] assetTypes = resources.getAssetTypes();
            string[] assetValues = resources.getAssetValues();
            for (int i = 0; i < resources.getAssetCount(); i++)
            {
                XmlElement assetElement = doc.CreateElement("asset");
                assetElement.SetAttribute("type", assetTypes[i]);
                assetElement.SetAttribute("uri", assetValues[i]);
                resourcesNode.AppendChild(assetElement);
            }

            // If the owner is an item
            if (resourcesType == RESOURCES_ITEM)
            {
                // If the item has no image, add the default one
                if (resources.getAssetPath("image") == null)
                {
                    XmlElement assetElement = doc.CreateElement("asset");
                    assetElement.SetAttribute("type", "image");
                    assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_IMAGE);
                    resourcesNode.AppendChild(assetElement);
                }

                // If the item has no icon, add the default one
                if (resources.getAssetPath("icon") == null)
                {
                    XmlElement assetElement = doc.CreateElement("asset");
                    assetElement.SetAttribute("type", "icon");
                    assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_ICON);
                    resourcesNode.AppendChild(assetElement);
                }
            }

            // If the owner is a scene
            if (resourcesType == RESOURCES_SCENE)
            {
                // If the item has no image, add the default one
                if (resources.getAssetPath("background") == null)
                {
                    XmlElement assetElement = doc.CreateElement("asset");
                    assetElement.SetAttribute("type", "background");
                    assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);
                    resourcesNode.AppendChild(assetElement);
                }
            }

            // If the owner is a scene
            if (resourcesType == RESOURCES_CUTSCENE)
            {
                // If the item has no image, add the default one
                if (resources.getAssetPath("slides") == null)
                {
                    XmlElement assetElement = doc.CreateElement("asset");
                    assetElement.SetAttribute("type", "slides");
                    assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_ANIMATION);
                    resourcesNode.AppendChild(assetElement);
                }
            }

            // If the owner is a character
            else if (resourcesType == RESOURCES_CHARACTER)
            {
                // For each asset, if it has not been declared attach the empty animation
                string[] assets = new string[]
                {
                "standup", "standdown", "standright", "standleft", "speakup", "speakdown", "speakright", "speakleft",
                "useright", "useleft", "walkup", "walkdown", "walkright", "walkleft"
                };
                foreach (string asset in assets)
                {
                    if (resources.getAssetPath(asset) == null)
                    {
                        XmlElement assetElement = doc.CreateElement("asset");
                        assetElement.SetAttribute("type", asset);
                        assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_ANIMATION);
                        resourcesNode.AppendChild(assetElement);
                    }
                }
            }

            // If the owner is a character
            else if (resourcesType == RESOURCES_CUSTOM_ACTION)
            {
                // For each asset, if it has not been declared attach the empty animation
                string[] assets = new string[] { "buttonNormal", "buttonOver", "buttonPressed" };
                foreach (string asset in assets)
                {
                    if (resources.getAssetPath(asset) == null)
                    {
                        XmlElement assetElement = doc.CreateElement("asset");
                        assetElement.SetAttribute("type", asset);
                        assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_ICON);
                        resourcesNode.AppendChild(assetElement);
                    }
                }
            }

            // if the owner is a book
            else if (resourcesType == RESOURCES_BOOK)
            {
                // For each asset, if it has not been declared attach the empty animation
                string[] assets = new string[]
                {"background" /*, "arrowLeftNormal", "arrowRightNormal", "arrowLeftOver", "arrowRightOver" */};
                foreach (string asset in assets)
                {
                    if (resources.getAssetPath(asset) == null)
                    {
                        XmlElement assetElement = doc.CreateElement("asset");
                        assetElement.SetAttribute("type", asset);
                        assetElement.SetAttribute("uri", SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);
                        resourcesNode.AppendChild(assetElement);
                    }
                }
            }

            return resourcesNode;
        }
    }
}
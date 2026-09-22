using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SelectArrowTool : SelectResourceTool
    {
        protected AdventureData adventureData;

        protected string type;

        protected bool removed;

        protected static AssetInformation[] createAssetInfoArray(string type)
        {

            AssetInformation[] array = new AssetInformation[1];
            if (type.StartsWith(DescriptorData.SOUND_PATH))
            {
                array[0] = new AssetInformation("", type, true, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_NONE);
            }
            else
            {
                array[0] = new AssetInformation("", type, true, AssetsConstants.CATEGORY_BUTTON, AssetsController.FILTER_NONE);
            }
            return array;
        }

        protected static ResourcesUni createResources(AdventureData adventureData, string type)
        {

            ResourcesUni resources = new ResourcesUni();
            bool introduced = false;
            for (int i = 0; i < adventureData.getArrows().Count; i++)
            {
                CustomArrow customArrow = adventureData.getArrows()[i];
                if (customArrow.getType().Equals(type))
                {
                    resources.addAsset(type, customArrow.getPath());
                    introduced = true;
                    break;
                }
            }

            if (!introduced)
            {
                resources.addAsset(type, null);
            }

            return resources;
        }

        public SelectArrowTool(AdventureData adventureData, string type) : base(createResources(adventureData, type), createAssetInfoArray(type), Controller.RESOURCES, 0)
        {
            this.adventureData = adventureData;
            this.type = type;
        }

        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getArrows().Count; i++)
                {
                    if (adventureData.getArrows()[i].getType().Equals(type))
                    {
                        if (removed)
                            adventureData.getArrows().RemoveAt(i);
                        else
                        {
                            adventureData.getArrows()[i].setPath(resources.getAssetPath(type));
                        }
                        break;

                    }
                }
                controller.updatePanel();
                controller.DataModified();
                return true;
            }

        }

        public override bool redoTool()
        {

            if (removed)
                adventureData.addArrow(type, "");
            bool done = base.redoTool();
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getArrows().Count; i++)
                {
                    if (adventureData.getArrows()[i].getType().Equals(type))
                    {
                        adventureData.getArrows()[i].setPath(resources.getAssetPath(type));
                    }
                }
                controller.updatePanel();
                return true;
            }
        }

        public override bool doTool()
        {

            //  if( resources.getAssetPath( type ).Equals( "NULL" ) ) {
            if (resources.getAssetPath(type) == null)
            {
                removed = false;
            }
            else
            {
                for (int i = 0; i < adventureData.getArrows().Count; i++)
                {
                    CustomArrow arrow = adventureData.getArrows()[i];
                    if (arrow.getType().Equals(type))
                    {
                        adventureData.getArrows().Remove(arrow);
                        break;
                    }
                }
                removed = true;
            }
            bool done = base.doTool();
            if (!done)
                return false;
            else
            {
                setArrow(type, resources.getAssetPath(type));
                return true;
            }
        }

        public void setArrow(string type, string path)
        {

            CustomArrow arrow = new CustomArrow(type, path);
            CustomArrow temp = null;
            foreach (CustomArrow cb in adventureData.getArrows())
            {
                if (cb.Equals(arrow))
                    temp = cb;
            }
            if (temp != null)
                adventureData.getArrows().Remove(temp);
            adventureData.addArrow(arrow);
        }
    }
}
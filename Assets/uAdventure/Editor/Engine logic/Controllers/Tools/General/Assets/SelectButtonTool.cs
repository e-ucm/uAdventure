using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SelectButtonTool : SelectResourceTool
    {
        protected AdventureData adventureData;

        protected string type;

        protected string action;

        protected bool removed;

        protected static AssetInformation[] createAssetInfoArray(string action, string type)
        {

            AssetInformation[] array = new AssetInformation[1];
            if (type.Equals(DescriptorData.SOUND_PATH))
            {
                array[0] = new AssetInformation("", action + "#" + type, true, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_NONE);
            }
            else
            {
                array[0] = new AssetInformation("", action + "#" + type, true, AssetsConstants.CATEGORY_BUTTON, AssetsController.FILTER_NONE);
            }
            return array;
        }

        protected static ResourcesUni createResources(AdventureData adventureData, string action, string type)
        {

            ResourcesUni resources = new ResourcesUni();
            bool introduced = false;
            for (int i = 0; i < adventureData.getButtons().Count; i++)
            {
                CustomButton customButton = adventureData.getButtons()[i];
                if (customButton.getType().Equals(type) && customButton.getAction().Equals(action))
                {
                    resources.addAsset(action + "#" + type, customButton.getPath());
                    introduced = true;
                    break;
                }
            }

            if (!introduced)
            {
                resources.addAsset(action + "#" + type, /*"NULL"*/ null);
            }

            return resources;
        }

        public SelectButtonTool(AdventureData adventureData, string action, string type) : base(createResources(adventureData, action, type), createAssetInfoArray(action, type), Controller.ACTION_CUSTOM, 0)
        {
            this.adventureData = adventureData;
            this.type = type;
            this.action = action;
        }

        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getButtons().Count; i++)
                {
                    if (adventureData.getButtons()[i].getType().Equals(type) && adventureData.getButtons()[i].getAction().Equals(action))
                    {
                        if (removed)
                        {
                            adventureData.getButtons().RemoveAt(i);
                            setButton(action, type, resources.getAssetPath(action + "#" + type));
                        }
                        else
                        {
                            adventureData.getButtons()[i].setPath(resources.getAssetPath(action + "#" + type));
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
                adventureData.addButton(action, type, "");
            bool done = base.redoTool();
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getButtons().Count; i++)
                {
                    if (adventureData.getButtons()[i].getType().Equals(type) && adventureData.getButtons()[i].getAction().Equals(action))
                    {
                        adventureData.getButtons()[i].setPath(resources.getAssetPath(action + "#" + type));
                    }
                }
                controller.updatePanel();
                return true;
            }
        }

        public override bool doTool()
        {

            //   if( resources.getAssetPath( action + "#" + type ).Equals( "NULL" ) ) {
            if (resources.getAssetPath(action + "#" + type) == null)
            {
                removed = false;
            }
            else
            {
                for (int i = 0; i < adventureData.getButtons().Count; i++)
                {
                    CustomButton button = adventureData.getButtons()[i];
                    if (button.getAction().Equals(action) && button.getType().Equals(type))
                    {
                        adventureData.getButtons().Remove(button);
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
                setButton(action, type, resources.getAssetPath(action + "#" + type));
                return true;
            }
        }

        public void setButton(string action, string type, string path)
        {

            CustomButton button = new CustomButton(action, type, path);
            CustomButton temp = null;
            foreach (CustomButton cb in adventureData.getButtons())
            {
                if (cb.Equals(button))
                    temp = cb;
            }
            if (temp != null)
                adventureData.getButtons().Remove(temp);
            adventureData.addButton(button);
        }
    }
}
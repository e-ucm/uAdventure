using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SelectCursorPathTool : SelectResourceTool
    {

        protected AdventureData adventureData;

        protected int t;

        protected string type;

        protected bool added;

        protected static AssetInformation[] createAssetInfoArray(int t)
        {

            string type = DescriptorData.getCursorTypeString(t);
            AssetInformation[] array = new AssetInformation[1];
            array[0] = new AssetInformation("", type, true, AssetsConstants.CATEGORY_CURSOR, AssetsController.FILTER_NONE);
            return array;
        }

        protected static ResourcesUni createResources(AdventureData adventureData, int t)
        {

            string type = DescriptorData.getCursorTypeString(t);
            ResourcesUni resources = new ResourcesUni();
            bool introduced = false;
            for (int i = 0; i < adventureData.getCursors().Count; i++)
            {
                if (adventureData.getCursors()[i].getType().Equals(type) && adventureData.getCursors()[i].getPath() != null)
                {
                    resources.addAsset(type, adventureData.getCursors()[i].getPath());
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

        public SelectCursorPathTool(AdventureData adventureData, int t)
            : base(createResources(adventureData, t), createAssetInfoArray(t), Controller.ACTION_CUSTOM, 0)
        {
            this.adventureData = adventureData;
            this.t = t;
            this.type = DescriptorData.getCursorTypeString(t);
        }

        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getCursors().Count; i++)
                {
                    if (adventureData.getCursors()[i].getType().Equals(type))
                    {
                        if (added)
                        {
                            adventureData.getCursors().RemoveAt(i);
                            //adventureData.addCursor( type, "" );
                        }
                        else
                            adventureData.getCursors()[i].setPath(resources.getAssetPath(type));
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


            bool done = base.redoTool();
            if (added)
                adventureData.addCursor(type, "");
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getCursors().Count; i++)
                {
                    if (adventureData.getCursors()[i].getType().Equals(type))
                    {
                        adventureData.getCursors()[i].setPath(resources.getAssetPath(type));
                    }
                }
                controller.updatePanel();
                return true;
            }
        }

        public override bool doTool()
        {

            // if( resources.getAssetPath( type ).Equals( "NULL" ) ) {
            if (resources.getAssetPath(type) == null)
            {
                adventureData.addCursor(type, "");
                added = true;
            }
            else
            {
                added = false;
            }
            bool done = base.doTool();
            if (!done)
                return false;
            else
            {
                for (int i = 0; i < adventureData.getCursors().Count; i++)
                {
                    if (adventureData.getCursors()[i].getType().Equals(type))
                    {
                        adventureData.getCursors()[i].setPath(resources.getAssetPath(type));
                    }
                }
                return true;
            }
        }
    }
}
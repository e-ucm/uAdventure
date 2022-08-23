using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SelectSimpleSoundTool : SelectResourceTool
    {
        protected const string AUDIO_STR = "audio";

        private HasSound objectWithSound;

        //  private string oldName;

        protected static AssetInformation[] createAssetInfoArray()
        {

            AssetInformation[] array = new AssetInformation[1];
            array[0] = new AssetInformation("", AUDIO_STR, true, AssetsConstants.CATEGORY_AUDIO,
                AssetsController.FILTER_NONE);
            return array;
        }

        protected static ResourcesUni createResources(HasSound objectWithSound)
        {

            ResourcesUni resources = new ResourcesUni();

            string soundPath = objectWithSound.getSoundPath();
            if (soundPath != null)
            {
                resources.addAsset(AUDIO_STR, soundPath);
            }
            return resources;
        }

        public SelectSimpleSoundTool(HasSound sound) : base(createResources(sound), createAssetInfoArray(), 0, 0)
        {
            this.objectWithSound = sound;
        }




        public override bool doTool()
        {

            //string selectedName;

            bool done = base.doTool();
            if (!done)
                return false;
            else
            {
                objectWithSound.setSoundPath(resources.getAssetPath(AUDIO_STR));
                controller.updatePanel();
                return true;

            } // end else


        }


        public override bool redoTool()
        {

            bool done = base.redoTool();
            if (!done)
                return false;
            else
            {
                objectWithSound.setSoundPath(resources.getAssetPath(AUDIO_STR));
                controller.updatePanel();
                return true;
            } // end else

        }


        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (!done)
                return false;
            else
            {
                objectWithSound.setSoundPath(resources.getAssetPath(AUDIO_STR));
                controller.updatePanel();
                return true;
            } // end else

        }
    }
}
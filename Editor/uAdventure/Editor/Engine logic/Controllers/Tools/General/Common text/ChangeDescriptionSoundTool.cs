using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeDescriptionSoundTool : SelectResourceTool
    {

        protected const string AUDIO_STR = "audio";

        private HasDescriptionSound descriptionSound;

        //  private string oldName;

        private HasDescriptionSoundEnum type;

        protected static AssetInformation[] createAssetInfoArray()
        {

            AssetInformation[] array = new AssetInformation[1];
            array[0] = new AssetInformation("", AUDIO_STR, true, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_NONE);
            return array;
        }

        protected static ResourcesUni createResources(HasDescriptionSound descriptionSound, int type)
        {

            ResourcesUni resources = new ResourcesUni();

            string selectedName;

            switch ((HasDescriptionSoundEnum)type)
            {
                case HasDescriptionSoundEnum.NAME_PATH:
                    selectedName = descriptionSound.getNameSoundPath();
                    if (selectedName != null)
                    {
                        resources.addAsset(AUDIO_STR, selectedName);
                    }
                    return resources;
                case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                    selectedName = descriptionSound.getDescriptionSoundPath();
                    if (selectedName != null)
                    {
                        resources.addAsset(AUDIO_STR, selectedName);
                    }
                    return resources;
                case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                    selectedName = descriptionSound.getDetailedDescriptionSoundPath();
                    if (selectedName != null)
                    {
                        resources.addAsset(AUDIO_STR, selectedName);
                    }
                    return resources;
                default:
                    return resources;
            }

        }

        public ChangeDescriptionSoundTool(HasDescriptionSound descrSound, int type) : base(createResources(descrSound, type), createAssetInfoArray(), 0, 0)
        {
            this.descriptionSound = descrSound;
            this.type = (HasDescriptionSoundEnum)type;

        }




        public override bool doTool()
        {

            //string selectedName;

            bool done = base.doTool();
            if (!done)
                return false;
            else
            {

                switch (type)
                {
                    case HasDescriptionSoundEnum.NAME_PATH:
                        descriptionSound.setNameSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                        descriptionSound.setDescriptionSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                        descriptionSound.setDetailedDescriptionSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    default:
                        return false;
                }

            }// end else


        }


        public override bool redoTool()
        {

            bool done = base.redoTool();
            if (!done)
                return false;
            else
            {

                switch (type)
                {
                    case HasDescriptionSoundEnum.NAME_PATH:
                        descriptionSound.setNameSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                        descriptionSound.setDescriptionSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;
                    case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                        descriptionSound.setDetailedDescriptionSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    default:
                        return false;
                }

            }// end else

        }


        public override bool undoTool()
        {

            bool done = base.undoTool();
            if (!done)
                return false;
            else
            {

                switch (type)
                {
                    case HasDescriptionSoundEnum.NAME_PATH:
                        descriptionSound.setNameSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                        descriptionSound.setDescriptionSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                        descriptionSound.setDetailedDescriptionSoundPath(resources.getAssetPath(AUDIO_STR));
                        controller.updatePanel();
                        return true;

                    default:
                        return false;
                }

            }// end else

        }
    }
}
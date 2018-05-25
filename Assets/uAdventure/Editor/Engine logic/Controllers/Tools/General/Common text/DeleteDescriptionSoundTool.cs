using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteDescriptionSoundTool : Tool
    {

        private HasDescriptionSound hds;

        private HasDescriptionSoundEnum type;

        private string oldPath;

        public DeleteDescriptionSoundTool(HasDescriptionSound hds, int type)
        {
            this.hds = hds;
            this.type = (HasDescriptionSoundEnum)type;

            switch ((HasDescriptionSoundEnum)type)
            {
                case HasDescriptionSoundEnum.NAME_PATH:
                    oldPath = hds.getNameSoundPath();
                    break;

                case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                    oldPath = hds.getDescriptionSoundPath();
                    break;

                case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                    oldPath = hds.getDetailedDescriptionSoundPath();
                    break;

            }
        }





        public override bool doTool()
        {

            switch (type)
            {
                case HasDescriptionSoundEnum.NAME_PATH:
                    hds.setNameSoundPath(null);
                    break;

                case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                    hds.setDescriptionSoundPath(null);
                    break;

                case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                    hds.setDetailedDescriptionSoundPath(null);
                    break;

            }

            Controller.Instance.updatePanel();

            return true;
        }


        public override bool redoTool()
        {

            switch (type)
            {
                case HasDescriptionSoundEnum.NAME_PATH:
                    hds.setNameSoundPath(null);
                    break;

                case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                    hds.setDescriptionSoundPath(null);
                    break;

                case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                    hds.setDetailedDescriptionSoundPath(null);
                    break;

            }

            Controller.Instance.updatePanel();


            return true;
        }


        public override bool undoTool()
        {

            switch (type)
            {
                case HasDescriptionSoundEnum.NAME_PATH:
                    hds.setNameSoundPath(oldPath);
                    break;

                case HasDescriptionSoundEnum.DESCRIPTION_PATH:
                    hds.setDescriptionSoundPath(oldPath);
                    break;

                case HasDescriptionSoundEnum.DETAILED_DESCRIPTION_PATH:
                    hds.setDetailedDescriptionSoundPath(oldPath);
                    break;

            }

            Controller.Instance.updatePanel();

            return true;
        }


        public override bool canRedo()
        {
            return true;
        }


        public override bool canUndo()
        {

            return true;
        }


        public override bool combine(Tool other)
        {

            return true;
        }
    }
}
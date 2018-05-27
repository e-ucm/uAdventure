using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteSimpleSoundTool : Tool
    {

        private HasSound objectWithSound;

        private string oldPath;

        public DeleteSimpleSoundTool(HasSound objectWithSound)
        {
            this.objectWithSound = objectWithSound;
            oldPath = objectWithSound.getSoundPath();
        }


        public override bool doTool()
        {
            objectWithSound.setSoundPath(null);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool redoTool()
        {
            objectWithSound.setSoundPath(null);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {
            objectWithSound.setSoundPath(oldPath);
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
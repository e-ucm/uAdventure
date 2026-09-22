using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeTitleTool : Tool
    {

        private Titled titled;

        private string title;

        private string oldTitle;

        private Controller controller;

        public ChangeTitleTool(Titled titled, string title)
        {

            this.titled = titled;
            this.title = title;
            controller = Controller.Instance;
        }

        public override bool canRedo()
        {

            return true;
        }

        public override bool canUndo()
        {

            return true;
        }

        public override bool doTool()
        {

            if (!title.Equals(titled.getTitle()))
            {
                oldTitle = titled.getTitle();
                titled.setTitle(title);
                controller.updateStructure();
                controller.updateChapterMenu();
                return true;
            }
            return false;
        }

        public override string getToolName()
        {

            return "Change Chapter Title";
        }

        public override bool redoTool()
        {
            titled.setTitle(title);
            controller.updateStructure();
            controller.updateChapterMenu();
            controller.updatePanel();
            return true;
        }

        public override bool undoTool()
        {

            titled.setTitle(oldTitle);
            controller.updateStructure();
            controller.updateChapterMenu();
            controller.updatePanel();
            return true;
        }

        public override bool combine(Tool other)
        {

            if (other is ChangeTitleTool)
            {
                ChangeTitleTool ctt = (ChangeTitleTool)other;
                if (ctt.titled == titled && ctt.oldTitle == title)
                {
                    title = ctt.title;
                    timeStamp = ctt.timeStamp;
                    return true;
                }
            }
            return false;
        }
    }
}
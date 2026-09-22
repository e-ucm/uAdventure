using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteChapterTool : Tool
    {
        private Controller controller;

        private ChapterListDataControl chaptersController;

        // Removed data
        private Chapter chapterRemoved;

        private int index;

        private string chapterTitle;

        public DeleteChapterTool(ChapterListDataControl chaptersController)
        {
            this.chaptersController = chaptersController;
            controller = Controller.Instance;
        }

        public override bool canRedo()
        {

            return false;
        }

        public override bool canUndo()
        {

            return false;
        }

        public override bool combine(Tool other)
        {

            return false;
        }

        public override bool doTool()
        {
            // Delete the chapter and the controller
            index = chaptersController.getSelectedChapter();
            chapterRemoved = ((Chapter)chaptersController.removeChapterDataControl().getContent());

            return true;
        }

        public override bool redoTool()
        {
            chaptersController.removeChapterDataControl(index);
            //controller.reloadData();
            return true;
        }

        public override bool undoTool()
        {
            chaptersController.addChapterDataControl(index, chapterRemoved);

            //controller.reloadData();
            return true;
        }
    }
}
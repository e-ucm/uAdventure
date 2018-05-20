using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AddChapterTool : Tool
    {
        private Controller controller;

        private ChapterListDataControl chaptersController;

        private Chapter newChapter;

        private int index;

        private string chapterTitle;

        public AddChapterTool(ChapterListDataControl chaptersController, string chapterName)
        {
            this.chapterTitle = chapterName;
            this.chaptersController = chaptersController;
            this.controller = Controller.Instance;
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

            return false;
        }

        public override bool doTool()
        {
            // If some value was typed
            if (chapterTitle != null)
            {
                if (!chaptersController.exitsChapter(chapterTitle))
                {
                    // Create the new chapter, and the controller
                    newChapter = new Chapter(chapterTitle, TC.get("DefaultValue.SceneId"));
                    chaptersController.addChapterDataControl(newChapter);
                    index = chaptersController.getSelectedChapter();

                    Debug.Log("ADD index: " + index);
                    //controller.reloadData();
                    return true;
                }
                else
                {
                    controller.ShowErrorDialog(TC.get("Operation.CreateAdaptationFile.FileName.ExistValue.Title"), TC.get("Operation.NewChapter.ExistingName"));
                    return false;
                }
            }
            return false;

        }

        public override bool redoTool()
        {

            // Create the new chapter, and the controller
            newChapter = new Chapter(chapterTitle, TC.get("DefaultValue.SceneId"));
            chaptersController.addChapterDataControl(newChapter);
            index = chaptersController.getSelectedChapter();

            //controller.reloadData();
            return true;
        }

        public override bool undoTool()
        {

            bool done = (chaptersController.removeChapterDataControl(index)) != null;
            //controller.reloadData();
            return done;
        }
    }
}
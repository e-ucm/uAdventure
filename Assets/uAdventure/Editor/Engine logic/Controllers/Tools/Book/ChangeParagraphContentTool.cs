using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeParagraphContentTool : Tool
    {


        private BookParagraph bookParagraph;

        private string newContent;

        private string oldContent;

        public ChangeParagraphContentTool(BookParagraph bookParagraph, string content)
        {

            this.bookParagraph = bookParagraph;
            this.newContent = content;
            this.oldContent = bookParagraph.getContent();
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

            if (other is ChangeParagraphContentTool)
            {
                ChangeParagraphContentTool tool = (ChangeParagraphContentTool)other;
                if (tool.bookParagraph == bookParagraph)
                {
                    newContent = tool.newContent;
                    timeStamp = tool.timeStamp;
                    return true;
                }
            }
            return false;
        }


        public override bool doTool()
        {

            if (oldContent != null && oldContent.Equals(newContent))
                return false;
            bookParagraph.setContent(newContent);
            return true;
        }


        public override bool redoTool()
        {

            bookParagraph.setContent(newContent);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            bookParagraph.setContent(oldContent);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}
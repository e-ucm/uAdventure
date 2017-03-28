using UnityEngine;

namespace uAdventure.Editor
{
    public class VideoFileOpenDialog : BaseFileOpenDialog
    {
        public virtual void Init(DialogReceiverInterface e, FileType fType)
        {
            fileFilter = "video files (*.mpg)|*.mpg";
            base.Init(e, fType);
        }

        protected override void ChoosedCorrectFile()
        {
            CopySelectedAssset();
            reference.OnDialogOk(returnPath, fileType);
        }

        protected override void FileSelectionNotPerfromed()
        {
            Debug.Log("NIc nie wybrałeś");
            reference.OnDialogCanceled();
        }
    }
}
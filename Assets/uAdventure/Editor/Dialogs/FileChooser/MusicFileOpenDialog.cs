using UnityEngine;

namespace uAdventure.Editor
{
    public class MusicFileOpenDialog : BaseFileOpenDialog
    {
        public virtual void Init(DialogReceiverInterface e, FileType fType)
        {
            fileFilter = "mp3";
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
using UnityEngine;

namespace uAdventure.Editor
{
    public class VideoFileOpenDialog : BaseFileOpenDialog
    {
        public override void Init(DialogReceiverInterface e, FileType fType)
        {
            fileFilter = "asf,avi,dv,m4v,mov,mp4,mpg,mpeg,ogv,vp8,webm,wmv";
            base.Init(e, fType);
        }

        protected override void ChoosedCorrectFile()
        {
            CopySelectedAssset();
            reference.OnDialogOk(returnPath, fileType);
        }

        protected override void FileSelectionNotPerfromed()
        {
            reference.OnDialogCanceled();
        }
    }
}
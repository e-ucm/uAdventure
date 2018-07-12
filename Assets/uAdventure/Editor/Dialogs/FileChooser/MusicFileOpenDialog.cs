namespace uAdventure.Editor
{
    public class MusicFileOpenDialog : BaseFileOpenDialog
    {
        public override void Init(DialogReceiverInterface e, FileType fType)
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
            reference.OnDialogCanceled();
        }
    }
}
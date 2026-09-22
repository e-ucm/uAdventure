using System;
using UnityEngine;

namespace uAdventure.Editor
{
    public class ImageFileOpenDialog : BaseFileOpenDialog
    {
        public override void Init(DialogReceiverInterface e, FileType fType)
        {
            fileFilter = "jpg,png,bmp";
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

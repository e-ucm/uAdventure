using System;
using UnityEngine;

namespace uAdventure.Editor
{
    public class PathFileOpenDialog : BaseFileOpenDialog
    {
        protected override void ChoosedCorrectFile()
        {
            reference.OnDialogOk(selectedAssetPath, fileType);
        }

        protected override void FileSelectionNotPerfromed()
        {
            reference.OnDialogCanceled();
        }
    }
}

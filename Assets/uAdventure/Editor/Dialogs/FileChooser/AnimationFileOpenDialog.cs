using UnityEngine;

namespace uAdventure.Editor
{
    public class AnimationFileOpenDialog : BaseFileOpenDialog
    {

        override public void Init(DialogReceiverInterface e, FileType fType)
        {
            basePath = "Assets/uAdventure/Resources/CurrentGame/assets/animation/";
            fileFilter = "eaa.xml";
            base.Init(e, fType);
        }

        protected override void ChoosedCorrectFile()
        {
            CopySelectedAssset();
            reference.OnDialogOk(returnPath, fileType);
        }

        protected override void FileSelectionNotPerfromed()
        {
            Debug.Log("No animation selected");
            reference.OnDialogCanceled();
        }
    }
}
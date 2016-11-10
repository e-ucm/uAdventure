using UnityEngine;
using System.Collections;

public class AnimationFileOpenDialog : BaseFileOpenDialog
{
    public virtual void Init(DialogReceiverInterface e, FileType fType)
    {
        fileFilter = "JPG set of slides (*.eaa)|*.eaa";
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

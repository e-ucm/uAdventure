using UnityEngine;
using System.Collections;
using System.IO;

public class ImageFileOpenDialog : BaseFileOpenDialog
{
    public virtual void Init(DialogReceiverInterface e, FileType fType)
    {
        fileFilter = "image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp"; 
        base.Init(e, fType);
    }

    protected override void ChoosedCorrectFile()
    {
        CopySelectedAssset();
        reference.OnDialogOk(returnPath, fileType);
    }

    protected override void FileSelectionNotPerfromed()
    {
        Debug.Log("NIc nie wybrałeś" );
        reference.OnDialogCanceled();
    }
}

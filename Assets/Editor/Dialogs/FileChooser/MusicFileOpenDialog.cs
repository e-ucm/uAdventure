using UnityEngine;
using System.Collections;
using System.IO;

public class MusicFileOpenDialog : BaseFileOpenDialog
{
    public virtual void Init(DialogReceiverInterface e, FileType fType)
    {
        fileFilter = "music files (*.mp3)|*.mp3";
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

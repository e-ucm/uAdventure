using UnityEngine;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class NewProjectMenuItem : IMenuItem
    {
        public NewProjectMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class LoadProjectMenuItem : IMenuItem
    {
        public LoadProjectMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class SaveProjectMenuItem : IMenuItem
    {
        public SaveProjectMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.Save();
            Debug.Log("Saved!");
        }
    }

    public class SaveProjectAsMenuItem : IMenuItem
    {
        public SaveProjectAsMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Debug.Log("Saved!");
        }
    }

    public class LOMMetadataEditorMenuItem : IMenuItem
    {
        public LOMMetadataEditorMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            uAdventureWindowMetaData.OpenMetaDataWindow();
        }
    }

    public class LearningObjectPropertiesMenuItem : IMenuItem
    {
        public LearningObjectPropertiesMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class LearningObjectPropertiesSCORMMenuItem : IMenuItem
    {
        public LearningObjectPropertiesSCORMMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class ExportProjectMenuItem : IMenuItem
    {
        public ExportProjectMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class BuildProjectEadMenuItem : IMenuItem
    {
        public BuildProjectEadMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame();
        }
    }
    
    public class BuildProjectWindows : IMenuItem
    {
        public BuildProjectWindows(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_WINDOWS);
        }
    }
    public class BuildProjectLinux : IMenuItem
    {
        public BuildProjectLinux(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_LINUX);
        }
    }
    public class BuildProjectMacOsX : IMenuItem
    {
        public BuildProjectMacOsX(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_MACOSX);
        }
    }
    public class BuildProjectStandalone : IMenuItem
    {
        public BuildProjectStandalone(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_STANDALONE);
        }
    }
    public class BuildProjectAndroid : IMenuItem
    {
        public BuildProjectAndroid(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_ANDROID);
        }
    }
    public class BuildProjectIOS : IMenuItem
    {
        public BuildProjectIOS(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_IOS);
        }
    }
    public class BuildProjectWebGL : IMenuItem
    {
        public BuildProjectWebGL(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_WEBGL);
        }
    }
    public class BuildProjectMobile : IMenuItem
    {
        public BuildProjectMobile(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_MOBILE);
        }
    }
    public class BuildProjectAll : IMenuItem
    {
        public BuildProjectAll(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.BuildGame(Controller.EXPORT_ALL);
        }
    }

    public class ExportLearningObject : IMenuItem
    {
        public ExportLearningObject(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.ExportLearningObject();
        }
    }

    public class UndoMenuItem : IMenuItem
    {
        public UndoMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.UndoTool();
        }
    }

    public class RedoMenuItem : IMenuItem
    {
        public RedoMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.RedoTool();
        }
    }

    public class SearchMenuItem : IMenuItem
    {
        public SearchMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class CheckAdventureConsistencyMenuItem : IMenuItem
    {
        public CheckAdventureConsistencyMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class EditAdventureDataMenuItem : IMenuItem
    {
        public EditAdventureDataMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            uAdventureWindowSettings.OpenAdventureWindow();
        }
    }


    public class VisualisationMenuItem : IMenuItem
    {
        public VisualisationMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class ConvertToMenuItem : IMenuItem
    {
        public ConvertToMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class DeleteUnusedDataMenuItem : IMenuItem
    {
        public DeleteUnusedDataMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class AddChapterMenuItem : IMenuItem, DialogReceiverInterface
    {
        public AddChapterMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance.ShowInputDialog("", TC.get("Operation.AddChapterMessage"), null, this);
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            Controller.Instance.addChapter(message);
        }

        public void OnDialogCanceled(object workingObject = null)
        {
        }
    }

    public class DeleteChapterMenuItem : IMenuItem, DialogReceiverInterface
    {
        public DeleteChapterMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            ConfirmationDialog window = (ConfirmationDialog)ScriptableObject.CreateInstance(typeof(ConfirmationDialog));
            window.Init(this, "Delete chapter: " + Controller.Instance.ChapterList.getChapterTitles()[Controller.Instance.ChapterList.getSelectedChapter()]);
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            if (workingObject is ConfirmationDialog)
            {
                Controller.Instance.deleteChapter();
                Controller.Instance.RefreshView();
                ChaptersMenu.getInstance().RefreshMenuItems();
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
        }
    }

    public class MoveUpChapterMenuItem : IMenuItem
    {
        public MoveUpChapterMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance                .ChapterList                .moveChapterUp(Controller.Instance.ChapterList.getSelectedChapter());
            ChaptersMenu.getInstance().RefreshMenuItems();
        }
    }

    public class MoveDownChapterMenuItem : IMenuItem
    {
        public MoveDownChapterMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Controller.Instance                .ChapterList                .moveChapterDown(Controller.Instance.ChapterList.getSelectedChapter());
            ChaptersMenu.getInstance().RefreshMenuItems();
        }
    }

    public class ImportChapterMenuItem : IMenuItem
    {
        public ImportChapterMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class EditFlagsVariablesMenuItem : IMenuItem, DialogReceiverInterface
    {
        public EditFlagsVariablesMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
			ChapterVarAndFlagsEditor.Init ();
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {

        }

        public void OnDialogCanceled(object workingObject = null)
        {

        }
    }


    public class RunMenuItem : IMenuItem
    {
        public RunMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class RunNormalMenuItem : IMenuItem
    {
        public RunNormalMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class RunDebugMenuItem : IMenuItem
    {
        public RunDebugMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {

        }
    }

    public class SetLanguageEnglishMenuItem : IMenuItem
    {
        public SetLanguageEnglishMenuItem()
        {
            this.Label = "English";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_ENGLISH));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageDeutschMenuItem : IMenuItem
    {
        public SetLanguageDeutschMenuItem()
        {
            this.Label = "Deutsch";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_DEUTSCH));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageSpanishMenuItem : IMenuItem
    {
        public SetLanguageSpanishMenuItem()
        {
            this.Label = "Español";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_SPANISH));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageGalegoMenuItem : IMenuItem
    {
        public SetLanguageGalegoMenuItem()
        {
            this.Label = "Galego";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_GALEGO));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageItalianoMenuItem : IMenuItem
    {
        public SetLanguageItalianoMenuItem()
        {
            this.Label = "Italiano";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_ITALIANO));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguagePortugeseMenuItem : IMenuItem
    {
        public SetLanguagePortugeseMenuItem()
        {
            this.Label = "Português";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_PORTUGESE));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguagePortugeseBrazilMenuItem : IMenuItem
    {
        public SetLanguagePortugeseBrazilMenuItem()
        {
            this.Label = "Português-Brasil";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_PORTUGESE_BRAZIL));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageRomaniaMenuItem : IMenuItem
    {
        public SetLanguageRomaniaMenuItem()
        {
            this.Label = "Language.Name";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_ROMANIA));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageRussiaMenuItem : IMenuItem
    {
        public SetLanguageRussiaMenuItem()
        {
            this.Label = "русский язык";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_RUSSIAN));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class SetLanguageChinaMenuItem : IMenuItem
    {
        public SetLanguageChinaMenuItem()
        {
            this.Label = "中文";
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            TC.loadstrings(ReleaseFolders.getLanguageFilePath4Editor(true, ReleaseFolders.LANGUAGE_CHINA));
            EditorWindowBase.LanguageChanged();
        }
    }

    public class AboutEAMenuItem : IMenuItem
    {
        public AboutEAMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Application.OpenURL("https://www.e-ucm.es/uadventure/");
        }
    }

    public class AboutEASendMenuItem : IMenuItem
    {
        public AboutEASendMenuItem(string name_)
        {
            this.Label = name_;
        }

        public string Label
        {
            get; set;
        }

        public void OnCliked()
        {
            Application.OpenURL("https://github.com/e-ucm/uAdventure/issues/new");
        }
    }
}
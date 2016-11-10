using UnityEditor;
using UnityEngine;

public class FileMenu : WindowMenuContainer
{
   // private NewProjectMenuItem newProject;
    //private LoadProjectMenuItem loadProject;
    private SaveProjectMenuItem saveProject;
    //private SaveProjectAsMenuItem saveProjectAs;
    //private LOMMetadataEditorMenuItem lom;
    //private LearningObjectPropertiesMenuItem learningObjects;
    //private LearningObjectPropertiesSCORMMenuItem scorm;
    //private ExportProjectMenuItem exportProject;
    private ExportProjectEadMenuItem exportEadProject;

    public FileMenu()
    {
        SetMenuItems();
    }

    protected override void Callback(object obj)
    {
        //if ((obj as NewProjectMenuItem) != null)
        //    newProject.OnCliked();
        //else if ((obj as LoadProjectMenuItem) != null)
        //    loadProject.OnCliked();
        //else
        if ((obj as SaveProjectMenuItem) != null)
            saveProject.OnCliked();
        //else if ((obj as SaveProjectAsMenuItem) != null)
        //    saveProjectAs.OnCliked();
        //else if ((obj as LOMMetadataEditorMenuItem) != null)
        //    lom.OnCliked();
        //else if ((obj as LearningObjectPropertiesMenuItem) != null)
        //    learningObjects.OnCliked();
        //else if ((obj as LearningObjectPropertiesSCORMMenuItem) != null)
        //    scorm.OnCliked();
        //else if ((obj as ExportProjectMenuItem) != null)
        //    exportProject.OnCliked();
        else if ((obj as ExportProjectEadMenuItem) != null)
            exportEadProject.OnCliked();
    }

    protected override void SetMenuItems()
    {
        menu = new GenericMenu();

        //newProject = new NewProjectMenuItem("NEW_PROJECT");
        //loadProject = new LoadProjectMenuItem("LOAD_PROJECT");
        saveProject = new SaveProjectMenuItem("MenuFile.Save");
        //saveProjectAs = new SaveProjectAsMenuItem("SAVE_PROJECT_AS");
        //lom = new LOMMetadataEditorMenuItem("LOM_METADATA_EDITOR");
        //learningObjects = new LearningObjectPropertiesMenuItem("LEARNING_OBJECTS_PROPERTIES");
        //scorm = new LearningObjectPropertiesSCORMMenuItem("LEARNING_OBJECTS_PROPERTIES_SCORM");
        //exportProject = new ExportProjectMenuItem("EXPORT_PROJECT");
        exportEadProject = new ExportProjectEadMenuItem("MenuFile.Export");

        //menu.AddItem(new GUIContent(TC.get(newProject.Label)), false, Callback, newProject);
        //menu.AddItem(new GUIContent(TC.get(loadProject.Label)), false, Callback, loadProject);
        //menu.AddSeparator("");
        menu.AddItem(new GUIContent(TC.get(saveProject.Label)), false, Callback, saveProject);
       // menu.AddItem(new GUIContent(TC.get(saveProjectAs.Label)), false, Callback, saveProjectAs);
       // menu.AddSeparator("");
        //menu.AddItem(new GUIContent(TC.get(lom.Label) + "/" + TC.get(learningObjects.Label)), false, Callback, learningObjects);
        //menu.AddItem(new GUIContent(TC.get(lom.Label) + "/" + TC.get(scorm.Label)), false, Callback, scorm);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent(TC.get(exportEadProject.Label) + "/" +TC.get(exportEadProject.Label)), false, Callback, exportEadProject);
    }
}
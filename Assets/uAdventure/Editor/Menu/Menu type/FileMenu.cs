using UnityEditor;
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
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
        private BuildProjectEadMenuItem buildProject;
        private BuildProjectWindows buildProjectWindows;
        private BuildProjectMacOsX buildProjectMacOsX;
        private BuildProjectLinux buildProjectLinux;
        private BuildProjectStandalone buildProjectStandalone;
        private BuildProjectAndroid buildProjectAndroid;
        private BuildProjectIOS buildProjectIOS;
        private BuildProjectWebGL buildProjectWebGL;
        private BuildProjectMobile buildProjectMobile;
        private BuildProjectAll buildProjectAll;
        private ExportLearningObject exportLearningObject;

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
            else if ((obj as BuildProjectEadMenuItem) != null) buildProject.OnCliked();
            else if ((obj as BuildProjectWindows) != null) buildProjectWindows.OnCliked();
            else if ((obj as BuildProjectMacOsX) != null) buildProjectMacOsX.OnCliked();
            else if ((obj as BuildProjectLinux) != null) buildProjectLinux.OnCliked();
            else if ((obj as BuildProjectStandalone) != null) buildProjectStandalone.OnCliked();
            else if ((obj as BuildProjectAndroid) != null) buildProjectAndroid.OnCliked();
            else if ((obj as BuildProjectIOS) != null) buildProjectIOS.OnCliked();
            else if ((obj as BuildProjectWebGL) != null) buildProjectWebGL.OnCliked();
            else if ((obj as BuildProjectMobile) != null) buildProjectMobile.OnCliked();
            else if ((obj as BuildProjectAll) != null) buildProjectAll.OnCliked();
            else if ((obj as ExportLearningObject) != null) exportLearningObject.OnCliked();
        }

        protected override void SetMenuItems()
        {
            menu = new GenericMenu();

            //newProject = new NewProjectMenuItem("NEW_PROJECT");
            //loadProject = new LoadProjectMenuItem("LOAD_PROJECT");
            saveProject = new SaveProjectMenuItem("MenuFile.Save");
            buildProject = new BuildProjectEadMenuItem("MenuFile.Build");
            buildProjectWindows = new BuildProjectWindows("MenuFile.BuildWindows");
            buildProjectMacOsX = new BuildProjectMacOsX("MenuFile.BuildMacOsX");
            buildProjectLinux = new BuildProjectLinux("MenuFile.BuildLinux");
            buildProjectStandalone = new BuildProjectStandalone("MenuFile.BuildStandalone");
            buildProjectAndroid = new BuildProjectAndroid("MenuFile.BuildAndroid");
            buildProjectIOS = new BuildProjectIOS("MenuFile.BuildIOS");
            buildProjectWebGL = new BuildProjectWebGL("MenuFile.BuildWebGL");
            buildProjectMobile = new BuildProjectMobile("MenuFile.BuildMobile");
            buildProjectAll = new BuildProjectAll("MenuFile.BuildAll");
            exportLearningObject = new ExportLearningObject("MenuFile.ExportLOM");

            menu.AddItem(new GUIContent(TC.get(saveProject.Label)), false, Callback, saveProject);

            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProject.Label)), false, Callback, buildProject);
            menu.AddSeparator(TC.get(buildProject.Label) + "/");
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectWindows.Label)), false, Callback, buildProjectWindows);
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectMacOsX.Label)), false, Callback, buildProjectMacOsX);
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectLinux.Label)), false, Callback, buildProjectLinux);
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectStandalone.Label)), false, Callback, buildProjectStandalone);
            menu.AddSeparator(TC.get(buildProject.Label) + "/");
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectAndroid.Label)), false, Callback, buildProjectAndroid);
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectIOS.Label)), false, Callback, buildProjectIOS);
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectWebGL.Label)), false, Callback, buildProjectWebGL);
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectMobile.Label)), false, Callback, buildProjectMobile);
            menu.AddSeparator(TC.get(buildProject.Label) + "/");
            menu.AddItem(new GUIContent(TC.get(buildProject.Label) + "/" + TC.get(buildProjectAll.Label)), false, Callback, buildProjectAll);

            menu.AddItem(new GUIContent(TC.get(exportLearningObject.Label)), false, Callback, exportLearningObject);
        }
    }
}
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{

    public class ScenesWindowDocumentation : LayoutWindow
    {
        private string descriptionOfScene, nameOfScene, descriptionOfSceneLast, nameOfSceneLast, sceneclass = "", sceneclasslast, scenetype = "", scenetypelast;
        private Scene current;

        public ScenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            string doc = "";
            string name = "";
            string sclass = "";
            string stype = "";

            if (GameRources.GetInstance().selectedSceneIndex >= 0)
            {
                current = Controller.Instance.ChapterList.getSelectedChapterData().getScenes()[GameRources.GetInstance().selectedSceneIndex];

                doc = current.getDocumentation();
                name = current.getName();
                sclass = current.getXApiClass();
                stype = current.getXApiType();
            }

            doc = (doc == null ? "" : doc);
            name = (name == null ? "" : name);
            sclass = (sclass == null ? "" : sclass);
            stype = (stype == null ? "" : stype);

            descriptionOfScene = descriptionOfSceneLast = doc;
            nameOfScene = nameOfSceneLast = name;
            sceneclass = sceneclasslast = sclass;
            scenetype = scenetypelast = stype;
            
        }

        public override void Draw(int aID)
        {
            sceneclass = EditorGUILayout.TextField(new GUIContent("xAPI Class"), sceneclass);
            if (!sceneclass.Equals(sceneclasslast)) ChangeClass(sceneclass);

            scenetype = EditorGUILayout.TextField(new GUIContent("xAPI Type"), scenetype);
            if (!scenetype.Equals(scenetypelast)) ChangeType(scenetype);

            GUILayout.Label(TC.get("Scene.Documentation"));
            descriptionOfScene = GUILayout.TextArea(descriptionOfScene, GUILayout.MinHeight(0.4f * m_Rect.height));
            if (!descriptionOfScene.Equals(descriptionOfSceneLast))
                ChangeDocumentation(descriptionOfScene);

            GUILayout.Space(30);

            GUILayout.Label(TC.get("Scene.Name"));
            nameOfScene = GUILayout.TextField(nameOfScene);
            if (!nameOfScene.Equals(nameOfSceneLast))
                ChangeName(nameOfScene);
        }

        private void ChangeClass(string s)
        {
            Controller.Instance.ChapterList.getSelectedChapterData().getScenes()[GameRources.GetInstance().selectedSceneIndex].setXApiClass(s);
            sceneclasslast = s;
        }

        private void ChangeType(string s)
        {
            Controller.Instance.ChapterList.getSelectedChapterData().getScenes()[GameRources.GetInstance().selectedSceneIndex].setXApiType(s);
            scenetypelast = s;
        }

        private void ChangeName(string s)
        {
            Controller.Instance.ChapterList.getSelectedChapterData().getScenes()[GameRources.GetInstance().selectedSceneIndex].setName(s);
            nameOfSceneLast = s;
        }

        private void ChangeDocumentation(string s)
        {
            Controller.Instance.ChapterList.getSelectedChapterData().getScenes()[GameRources.GetInstance().selectedSceneIndex].setDocumentation(s);
            descriptionOfSceneLast = s;
        }
    }
}
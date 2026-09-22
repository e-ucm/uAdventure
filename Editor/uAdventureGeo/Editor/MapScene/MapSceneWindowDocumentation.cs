using uAdventure.Core;
using uAdventure.Editor;
using uAdventure.Geo;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class MapSceneWindowDocumentation : LayoutWindow
    {

        public MapSceneWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

        }

        public override void Draw(int aID)
        {
            var workingMapScene = GeoController.Instance.MapScenes[GeoController.Instance.SelectedMapScene];

            var xAPIConfigurable = workingMapScene as IxAPIConfigurable;

            //XApi class
            EditorGUI.BeginChangeCheck();
            var classes = xAPIConfigurable.getxAPIValidClasses();
            if (!classes.Contains(xAPIConfigurable.getxAPIClass()))
            {
                xAPIConfigurable.setxAPIClass(classes[0]);
            }

            var newClass =
                classes[
                    EditorGUILayout.Popup("xAPI Class", classes.IndexOf(xAPIConfigurable.getxAPIClass()),
                        classes.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                xAPIConfigurable.setxAPIClass(newClass);
            }

            // Xapi Type
            EditorGUI.BeginChangeCheck();
            var types = xAPIConfigurable.getxAPIValidTypes(xAPIConfigurable.getxAPIClass());
            if (!types.Contains(xAPIConfigurable.getxAPIType()))
            {
                xAPIConfigurable.setxAPIType(types[0]);
            }

            var newType =
                types[
                    EditorGUILayout.Popup("xAPI type", types.IndexOf(xAPIConfigurable.getxAPIType()), types.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                xAPIConfigurable.setxAPIType(newType);
            }

            // Name
            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.TextField(TC.get("Scene.Name"), workingMapScene.Name);
            if (EditorGUI.EndChangeCheck())
            {
                workingMapScene.Name = newName;
            }

            // Documentation
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(TC.get("Scene.Documentation"));
            var newDescription = EditorGUILayout.TextArea(workingMapScene.Documentation, GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingMapScene.Documentation = newDescription;
            }
        }
    }
}
using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Geo
{
    [EditorComponent(typeof(GeoElementDataControl), Name = "Geo.GeoElementWindow.Description.Title", Order = 30)]
    public class GeoElementWindowDescription : AbstractEditorComponent
    {
        private GeoElementDataControl workingGeoElement;

        /*
        * SETTINGS fields
        */
        private readonly DescriptionsEditor descriptionsEditor;

        public GeoElementWindowDescription(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            descriptionsEditor = ScriptableObject.CreateInstance<DescriptionsEditor>();
        }


        public override void Draw(int aID)
        {

            workingGeoElement = Target as GeoElementDataControl ?? GeoController.Instance.GeoElements.DataControls[GeoController.Instance.SelectedGeoElement];


            // -------------
            // Documentation
            // -------------

            GUILayout.Label(TC.get("Item.Documentation"));
            EditorGUI.BeginChangeCheck();
            var documentation = GUILayout.TextArea(workingGeoElement.Documentation?? string.Empty);
            if (EditorGUI.EndChangeCheck())
            {
                workingGeoElement.Documentation = documentation;
            }


            // -------------
            // Descriptions
            // -------------
            descriptionsEditor.Descriptions = workingGeoElement.DescriptionController;
            descriptionsEditor.OnInspectorGUI();
            GUILayout.Space(20);
        }
    }
}

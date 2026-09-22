using UnityEditor;
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AdventureMenu : WindowMenuContainer
    {
        private CheckAdventureConsistencyMenuItem consistency;
        private EditAdventureDataMenuItem editAdventureData;
        private LOMMetadataEditorMenuItem editMetaData;
        private VisualisationMenuItem visualisation;
        private ConvertToMenuItem convertTo;
        private DeleteUnusedDataMenuItem deleteUnused;

        public AdventureMenu()
        {
            SetMenuItems();
        }

        protected override void Callback(object obj)
        {
            if ((obj as CheckAdventureConsistencyMenuItem) != null)
                consistency.OnCliked();
            else if ((obj as EditAdventureDataMenuItem) != null)
                editAdventureData.OnCliked();
            else if ((obj as LOMMetadataEditorMenuItem) != null)
                editMetaData.OnCliked();
            else if ((obj as VisualisationMenuItem) != null)
                visualisation.OnCliked();
            else if ((obj as ConvertToMenuItem) != null)
                convertTo.OnCliked();
            else if ((obj as DeleteUnusedDataMenuItem) != null)
                deleteUnused.OnCliked();
        }

        protected override void SetMenuItems()
        {
            menu = new GenericMenu();

            consistency = new CheckAdventureConsistencyMenuItem("MenuAdventure.CheckConsistency");
            editAdventureData = new EditAdventureDataMenuItem("MenuAdventure.AdventureData");
            editMetaData = new LOMMetadataEditorMenuItem("MenuFile.AllLOMProperties");
            visualisation = new VisualisationMenuItem("MenuAdventure.Visualization");
            convertTo = new ConvertToMenuItem("MenuAdventure.ChangeToModePlayerVisible");
            deleteUnused = new DeleteUnusedDataMenuItem("MenuAdventure.DeleteUnusedAssets");

            // menu.AddItem(new GUIContent(TC.get(consistency.Label)), false, Callback, consistency);
            menu.AddItem(new GUIContent(TC.get(editAdventureData.Label)), false, Callback, editAdventureData);
            menu.AddItem(new GUIContent(TC.get(editMetaData.Label)), false, Callback, editMetaData);

            // menu.AddItem(new GUIContent(TC.get(visualisation.Label)), false, Callback, visualisation);
            // menu.AddItem(new GUIContent(TC.get(convertTo.Label)), false, Callback, convertTo);
            // menu.AddItem(new GUIContent(TC.get(deleteUnused.Label)), false, Callback, deleteUnused);
        }
    }
}
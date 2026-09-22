using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ScenesListDataControl : DataControl
    {

        /**
         * List of scenes.
         */
        private List<Scene> scenesList;

        /**
         * List of scene controllers.
         */
        private List<SceneDataControl> scenesDataControlList;

        /**
         * Constructor.
         * 
         * @param scenesList
         *            List of scenes
         */
        public ScenesListDataControl(List<Scene> scenesList, string playerImagePath)
        {

            this.scenesList = scenesList;

            // Create subcontrollers
            scenesDataControlList = new List<SceneDataControl>();
            foreach (Scene scene in scenesList)
                scenesDataControlList.Add(new SceneDataControl(scene, playerImagePath));
        }

        /**
         * Returns the list of scene controllers.
         * 
         * @return List of scene controllers.
         */
        public List<SceneDataControl> getScenes()
        {

            return scenesDataControlList;
        }

        /**
         * Returns the last scene controller of the list.
         * 
         * @return Last scene controller
         */
        public SceneDataControl getLastScene()
        {

            return scenesDataControlList[scenesDataControlList.Count - 1];
        }

        /**
         * Returns the info of the scenes contained in the list.
         * 
         * @return Array with the information of the scenes. It contains the
         *         identifier of each scene, and the number of exits, items and
         *         characters
         */
        public string[][] getScenesInfo()
        {

            string[][] scenesInfo = null;

            // Create the list for the scenes
            scenesInfo = new string[scenesList.Count][];
            // Create the jagged array
            for (int i = 0; i < scenesInfo.Length; i++)
            {
                scenesInfo[i] = new string[4];
            }

            // Fill the array with the info
            for (int i = 0; i < scenesList.Count; i++)
            {
                Scene scene = scenesList[i];
                scenesInfo[i][0] = scene.getId();
                scenesInfo[i][1] = TC.get("ScenesList.ExitsNumber", scene.getExits().Count.ToString());
                scenesInfo[i][2] = TC.get("ScenesList.ItemsNumber", scene.getItemReferences().Count.ToString());
                scenesInfo[i][3] = TC.get("ScenesList.NPCsNumber", scene.getCharacterReferences().Count.ToString());
            }

            return scenesInfo;
        }

        public string[] getScenesIDs()
        {
            string[] scenesInfo = null;
            scenesInfo = new string[scenesList.Count];
            for (int i = 0; i < scenesList.Count; i++)
            {
                scenesInfo[i] = scenesList[i].getId();
            }

            return scenesInfo;
        }

        public int getSceneIndexByID(string id)
        {
            for (int i = 0; i < scenesList.Count; i++)
            {
                if (scenesList[i].getId().Equals(id))
                    return i;
            }
            return -1;
        }

        public override System.Object getContent()
        {

            return scenesList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.SCENE };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new scenes
            return type == Controller.SCENE;
        }


        public override bool canBeDeleted()
        {

            return false;
        }


        public override bool canBeMoved()
        {

            return false;
        }


        public override bool canBeRenamed()
        {

            return false;
        }


        public override bool addElement(int type, string sceneId)
        {

            bool elementAdded = false;

            if (type == Controller.SCENE)
            {

                // Show a dialog asking for the scene id
                if (string.IsNullOrEmpty(sceneId))
                    controller.ShowInputIdDialog(TC.get("Operation.AddSceneTitle"), TC.get("Operation.AddSceneMessage"), 
                        Controller.Instance.makeElementValid(TC.get("Operation.AddSceneDefaultValue")), performAddElement);
                else
                {
                    performAddElement(null, sceneId);
                    elementAdded = true;
                }
                
            }

            return elementAdded;
        }

        private void performAddElement(object sender, string id)
        {

            // If some value was typed and the identifier is valid
            if (!controller.isElementIdValid(id))
                id = controller.makeElementValid(id);

            // Add thew new scene
            Scene newScene = new Scene(id);
            scenesList.Add(newScene);
            var sceneDataControl = new SceneDataControl(newScene, controller.getPlayerImagePath());
            scenesDataControlList.Add(sceneDataControl);
            if (Controller.Instance.PlayerMode == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER && sceneDataControl.isPlayerAtDefaultPosition())
            {
                sceneDataControl.setPlayerScale(sceneDataControl.getPlayerAppropiateScale());
            }

            controller.IdentifierSummary.addId<Scene>(id);
            controller.DataModified();
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is SceneDataControl))
                return false;


			Scene newElement = (Scene)(((Scene)(dataControl.getContent())).Clone());

			string id = newElement.getId ();
			if (!controller.isElementIdValid(id))
				id = controller.makeElementValid(id);
			
            newElement.setId(id);
            scenesList.Add(newElement);
            scenesDataControlList.Add(new SceneDataControl(newElement, controller.getPlayerImagePath()));
            controller.IdentifierSummary.addId<Scene>(id);
            return true;

        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddSceneDefaultValue");
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            bool elementDeleted = false;

            // Take the number of general scenes in the chapter
			int generalScenesCount = controller.IdentifierSummary.groupIds<IChapterTarget> ().Length;

            // If there are at least two scenes, this one can be deleted
            if (generalScenesCount > 1)
            {
                string sceneId = ((SceneDataControl)dataControl).getId();
                string references = controller.countIdentifierReferences(sceneId).ToString();

                // Ask for confirmation
                if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { sceneId, references })))
                {
                    if (scenesDataControlList.Remove((SceneDataControl)dataControl))
                    {
                        scenesList.Remove((Scene)dataControl.getContent());
                        controller.deleteIdentifierReferences(sceneId);
                        controller.IdentifierSummary.deleteId<Scene>(sceneId);
                        controller.updateVarFlagSummary();
                        controller.DataModified();
                        elementDeleted = true;
                    }
                }
            }

            // If this is the last scene, it can't be deleted
            else
                controller.ShowErrorDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.ErrorLastScene"));

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = scenesList.IndexOf((Scene)dataControl.getContent());

            if (elementIndex > 0)
            {
                Scene s = scenesList[elementIndex];
                SceneDataControl c = scenesDataControlList[elementIndex];
                scenesList.RemoveAt(elementIndex);
                scenesDataControlList.RemoveAt(elementIndex);
                scenesList.Insert(elementIndex - 1, s);
                scenesDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = scenesList.IndexOf((Scene)dataControl.getContent());

            if (elementIndex < scenesList.Count - 1)
            {
                Scene s = scenesList[elementIndex];
                SceneDataControl c = scenesDataControlList[elementIndex];
                scenesList.RemoveAt(elementIndex);
                scenesDataControlList.RemoveAt(elementIndex);
                scenesList.Insert(elementIndex + 1, s);
                scenesDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Iterate through each scene
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.SCENES_LIST);

            // Iterate through the scenes
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
            {
                string scenePath = currentPath + " >> " + sceneDataControl.getId();
                valid &= sceneDataControl.isValid(scenePath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each scene
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                count += sceneDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each scnee
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each scene
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each scene
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
            {
                if (sceneDataControl.getName().Equals(id))
                    count++;
                count += sceneDataControl.countIdentifierReferences(id);
            }

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each scene
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every scene
            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            return false;
        }

        /**
         * Adds a player reference in all scenes
         */
        public void addPlayerToAllScenes()
        {

            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.addPlayerInReferenceList();
        }

        /**
         * Delete player reference in all scenes
         */
        public void deletePlayerToAllScenes()
        {

            foreach (SceneDataControl sceneDataControl in scenesDataControlList)
                sceneDataControl.deletePlayerInReferenceList();
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.scenesDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return getPathFromChild(dataControl, scenesDataControlList.Cast<System.Object>().ToList());
        }
    }
}
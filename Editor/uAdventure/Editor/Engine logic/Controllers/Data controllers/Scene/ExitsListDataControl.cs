using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ExitsListDataControl : DataControl
    {

        /**
         * Scene controller that contains this element reference.
         */
        private SceneDataControl sceneDataControl;

        /**
         * List of exits.
         */
        private List<Exit> exitsList;

        /**
         * List of exits controllers.
         */
        private List<ExitDataControl> exitsDataControlList;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Link to the parent scene controller
         * @param exitsList
         *            List of exits
         */
        public ExitsListDataControl(SceneDataControl sceneDataControl, List<Exit> exitsList)
        {

            this.sceneDataControl = sceneDataControl;
            this.exitsList = exitsList;

            // Create subcontrollers
            exitsDataControlList = new List<ExitDataControl>();
            foreach (Exit exit in exitsList)
                exitsDataControlList.Add(new ExitDataControl(sceneDataControl, exit));
        }

        /**
         * Returns the list of exits controllers.
         * 
         * @return List of exits controllers
         */
        public List<ExitDataControl> getExits()
        {

            return exitsDataControlList;
        }

        /**
         * Returns the last exit controller from the list.
         * 
         * @return Last exit controller
         */
        public ExitDataControl getLastExit()
        {

            return exitsDataControlList[exitsDataControlList.Count - 1];
        }

        /**
         * Returns the id of the scene that contains this exits list.
         * 
         * @return Parent scene id
         */
        public string getParentSceneId()
        {

            return sceneDataControl.getId();
        }


        public override System.Object getContent()
        {

            return exitsList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.EXIT };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new exit
            return type == Controller.EXIT;
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


        public override bool addElement(int type, string id)
        {

            bool elementAdded = false;

            if (type == Controller.EXIT)
            {
                if (string.IsNullOrEmpty(id))
                {
                    string[] generalScenes = controller.IdentifierSummary.groupIds<IChapterTarget>();
                    if (generalScenes.Length != 0)
                        controller.ShowInputDialog(TC.get("ExitsList.AddExit"), TC.get("Exit.TargetScene"), generalScenes, performAddExit);
                    else
                        controller.ShowErrorDialog(TC.get("ExitsList.AddExit"), TC.get("ExitsList.NoScenesError"));
                }
                else
                {
                    // We check if the proposed id is a valid IChapterTarget
                    var destinationType = controller.IdentifierSummary.getType(id);
                    if (destinationType != null && typeof(IChapterTarget).IsAssignableFrom(destinationType))
                    {
                        // Is valid target
                        performAddExit(null, id);
                        elementAdded = true;
                    }
                }
            }

            return elementAdded;
        }

        private void performAddExit(object sender, string target)
        {
            Exit newExit = new Exit(true, 240, 240, 100, 100);
            newExit.setDestinyScale(1f);
            newExit.setNextSceneId(target);
            ExitDataControl newExitDataControl = new ExitDataControl(sceneDataControl, newExit);

            exitsList.Add(newExit);
            exitsDataControlList.Add(newExitDataControl);
            Controller.Instance.DataModified();
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is ExitDataControl))
                return false;


            Exit newElement = (Exit)(((Exit)(dataControl.getContent())).Clone());
            exitsList.Add(newElement);
            exitsDataControlList.Add(new ExitDataControl(sceneDataControl, newElement));
            Controller.Instance.updateVarFlagSummary();
            Controller.Instance.DataModified();
            return true;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;

            if (exitsList.Remove((Exit)dataControl.getContent()))
            {
                exitsDataControlList.Remove((ExitDataControl)dataControl);
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.DataModified();
                elementDeleted = true;
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = exitsList.IndexOf((Exit)dataControl.getContent());

            if (elementIndex > 0)
            {
                Exit o = exitsList[elementIndex];
                ExitDataControl c = exitsDataControlList[elementIndex];
                exitsList.RemoveAt(elementIndex);
                exitsDataControlList.RemoveAt(elementIndex);
                exitsList.Insert(elementIndex - 1, o);
                exitsDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = exitsList.IndexOf((Exit)dataControl.getContent());

            if (elementIndex < exitsList.Count - 1)
            {
                Exit o = exitsList[elementIndex];
                ExitDataControl c = exitsDataControlList[elementIndex];
                exitsList.RemoveAt(elementIndex);
                exitsDataControlList.RemoveAt(elementIndex);
                exitsList.Insert(elementIndex + 1, o);
                exitsDataControlList.Insert(elementIndex + 1, c);
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

            // Iterate through each exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                exitDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Iterate through the exits
            for (int i = 0; i < exitsDataControlList.Count; i++)
            {
                string exitPath = currentPath + " >> " + TC.getElement(Controller.EXIT) + " #" + (i + 1);
                valid &= exitsDataControlList[i].isValid(exitPath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                count += exitDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                exitDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                exitDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                count += exitDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                exitDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every exit
            foreach (ExitDataControl exitDataControl in exitsDataControlList)
                exitDataControl.deleteIdentifierReferences(id);

            int i = 0;
            while (i < exitsList.Count)
            {
                if (exitsList[i].getNextSceneId() == null)
                {
                    exitsList.RemoveAt(i);
                    exitsDataControlList.RemoveAt(i);
                }
                else
                    i++;
            }
        }


        public override bool canBeDuplicated()
        {

            return false;
        }

        public List<BarrierDataControl> getParentSceneBarriers()
        {

            return sceneDataControl.getBarriersList().getBarriers();
        }

        public List<ActiveAreaDataControl> getParentSceneActiveAreas()
        {

            return sceneDataControl.getActiveAreasList().getActiveAreas();
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.exitsDataControlList)
                dc.recursiveSearch();
        }

        public SceneDataControl getSceneDataControl()
        {

            return sceneDataControl;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, exitsDataControlList.Cast<System.Object>().ToList());
        }

        public List<Exit> getExitsList()
        {

            return this.exitsList;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BarriersListDataControl : DataControl
    {
        /**
            * Scene controller that contains this element reference.
            */
        private SceneDataControl sceneDataControl;

        /**
         * List of barriers.
         */
        private List<Barrier> barriersList;

        /**
         * List of barriers controllers.
         */
        private List<BarrierDataControl> barriersDataControlList;

        /**
         * Id of the next active area
         */
        private int id = 0;

        /**
         * Constructor.
         * 
         * @param sceneDataControl
         *            Link to the parent scene controller
         * @param barriersList
         *            List of activeAreas
         */
        public BarriersListDataControl(SceneDataControl sceneDataControl, List<Barrier> barriersList)
        {

            this.sceneDataControl = sceneDataControl;
            this.barriersList = barriersList;

            // Create subcontrollers
            barriersDataControlList = new List<BarrierDataControl>();
            foreach (Barrier barrier in barriersList)
                barriersDataControlList.Add(new BarrierDataControl(sceneDataControl, barrier));

            id = barriersList.Count + 1;
        }

        /**
         * Returns the list of barriers controllers.
         * 
         * @return List of barriers controllers
         */
        public List<BarrierDataControl> getBarriers()
        {

            return barriersDataControlList;
        }

        /**
         * Returns the last barrier controller from the list.
         * 
         * @return Last barrier controller
         */
        public BarrierDataControl getLastBarrier()
        {

            return barriersDataControlList[barriersDataControlList.Count - 1];
        }

        /**
         * Returns the id of the scene that contains this activeAreas list.
         * 
         * @return Parent scene id
         */
        public string getParentSceneId()
        {

            return sceneDataControl.getId();
        }


        public override System.Object getContent()
        {

            return barriersList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.BARRIER };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new barrier
            return type == Controller.BARRIER;
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


        public override bool addElement(int type, string barrierId)
        {

            bool elementAdded = false;

            if (type == Controller.BARRIER)
            {
                Barrier newBarrier = new Barrier(barrierId, 200, 200, 100, 100);
                id++;
                BarrierDataControl newBarrierDataControl = new BarrierDataControl(sceneDataControl, newBarrier);
                barriersList.Add(newBarrier);
                barriersDataControlList.Add(newBarrierDataControl);
                elementAdded = true;
            }

            return elementAdded;
        }

        public string getDefaultId()
        {

            return id.ToString();
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is BarrierDataControl))
                return false;

            Barrier newElement = (Barrier)(((Barrier)(dataControl.getContent())).Clone());

            newElement.setId(id.ToString());
            id++;
            barriersList.Add(newElement);
            barriersDataControlList.Add(new BarrierDataControl(sceneDataControl, newElement));
            controller.updateVarFlagSummary();
            controller.DataModified();
            return true;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;

            if (barriersList.Remove((Barrier)dataControl.getContent()))
            {
                barriersDataControlList.Remove((BarrierDataControl)dataControl);
                controller.updateVarFlagSummary();
                controller.DataModified();
                elementDeleted = true;
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = barriersList.IndexOf((Barrier)dataControl.getContent());

            if (elementIndex > 0)
            {
                Barrier o = barriersList[elementIndex];
                BarrierDataControl c = barriersDataControlList[elementIndex];
                barriersList.RemoveAt(elementIndex);
                barriersDataControlList.RemoveAt(elementIndex);
                barriersList.Insert(elementIndex - 1, o);
                barriersDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = barriersList.IndexOf((Barrier)dataControl.getContent());

            if (elementIndex < barriersList.Count - 1)
            {
                Barrier o = barriersList[elementIndex];
                BarrierDataControl c = barriersDataControlList[elementIndex];
                barriersList.RemoveAt(elementIndex);
                barriersDataControlList.RemoveAt(elementIndex);
                barriersList.Insert(elementIndex + 1, o);
                barriersDataControlList.Insert(elementIndex + 1, c);
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

            // Iterate through each activeArea
            foreach (BarrierDataControl barrierDataControl in barriersDataControlList)
                barrierDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            // no sense to spread isValid method calling when there aren't any possibility where
            // barriers are not valid
            return true;

            /* bool valid = true;

             // Iterate through the barriers
             for( int i = 0; i < barriersDataControlList.size( ); i++ ) {
                 string activeAreaPath = currentPath + " >> " + TC.getElement( Controller.BARRIER ) + " #" + ( i + 1 );
                 valid &= barriersDataControlList.get( i ).isValid( activeAreaPath, incidences );
             }

             return valid;*/
        }


        public override int countAssetReferences(string assetPath)
        {

            // no sense to spread countAssetsRefernces method calling when there aren't any possibility where
            // barriers have assets
            return 0;

            /*  int count = 0;

            // Iterate through each activeArea
            for( BarrierDataControl barrierDataControl : barriersDataControlList )
                count += barrierDataControl.countAssetReferences( assetPath );

            return count;*/
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // no sense to spread getAssetReferences method calling when there aren't any possibility where
            // barriers have assets
            /*        for( BarrierDataControl barrierDataControl : barriersDataControlList )
                        barrierDataControl.getAssetReferences( assetPaths, assetTypes );*/

        }


        public override void deleteAssetReferences(string assetPath)
        {
            // no sense to spread getAssetReferences method calling when there aren't any possibility where
            // barriers have assets
            /*        for( BarrierDataControl barrierDataControl : barriersDataControlList )
                        barrierDataControl.deleteAssetReferences( assetPath );*/
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each activeArea
            foreach (BarrierDataControl barrierDataControl in barriersDataControlList)
                count += barrierDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each activeArea
            foreach (BarrierDataControl barrierDataControl in barriersDataControlList)
                barrierDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every activeArea
            foreach (BarrierDataControl barrierDataControl in barriersDataControlList)
                barrierDataControl.deleteIdentifierReferences(id);

        }


        public override bool canBeDuplicated()
        {

            return false;
        }

        public TrajectoryDataControl getTrajectoryDataControl()
        {

            return sceneDataControl.getTrajectory();
        }

        public List<ExitDataControl> getParentSceneExits()
        {

            return sceneDataControl.getExitsList().getExits();
        }

        public List<ActiveAreaDataControl> getParentSceneActiveAreas()
        {

            return sceneDataControl.getActiveAreasList().getActiveAreas();
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.barriersDataControlList)
                dc.recursiveSearch();
        }

        public TrajectoryDataControl getParentSceneTrajectory()
        {

            return sceneDataControl.getTrajectory();
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, barriersDataControlList.Cast<System.Object>().ToList());
        }

        public List<Barrier> getBarriersList()
        {

            return this.barriersList;
        }
    }
}
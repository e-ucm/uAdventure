using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AdvancedFeaturesDataControl : DataControl
    {

        private TimersListDataControl timersListDataControl;

        private GlobalStateListDataControl globalStatesListDataControl;

        private MacroListDataControl macrosListDataControl;

        /**
         * Constructor.
         * 
         * @param scene
         *            Contained scene data
         */
        public AdvancedFeaturesDataControl()
        {

        }


        public override System.Object getContent()
        {

            return null;
        }


        public override int[] getAddableElements()
        {

            return new int[] { /*Controller.RESOURCES*/};
        }


        public override bool canAddElement(int type)
        {

            return false;
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

            return false;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            timersListDataControl.updateVarFlagSummary(varFlagSummary);
            globalStatesListDataControl.updateVarFlagSummary(varFlagSummary);
            macrosListDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            valid &= timersListDataControl.isValid(currentPath, incidences);
            valid &= globalStatesListDataControl.isValid(currentPath, incidences);
            valid &= macrosListDataControl.isValid(currentPath, incidences);

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            count += timersListDataControl.countAssetReferences(assetPath);
            count += globalStatesListDataControl.countAssetReferences(assetPath);
            count += macrosListDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            timersListDataControl.getAssetReferences(assetPaths, assetTypes);
            globalStatesListDataControl.getAssetReferences(assetPaths, assetTypes);
            macrosListDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            timersListDataControl.deleteAssetReferences(assetPath);
            globalStatesListDataControl.deleteAssetReferences(assetPath);
            macrosListDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            count += timersListDataControl.countIdentifierReferences(id);
            count += globalStatesListDataControl.countIdentifierReferences(id);
            count += macrosListDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            timersListDataControl.replaceIdentifierReferences(oldId, newId);
            globalStatesListDataControl.replaceIdentifierReferences(oldId, newId);
            macrosListDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            timersListDataControl.deleteIdentifierReferences(id);
            globalStatesListDataControl.deleteIdentifierReferences(id);
            macrosListDataControl.deleteIdentifierReferences(id);

        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            this.getMacrosListDataControl().recursiveSearch();
            this.getGlobalStatesListDataControl().recursiveSearch();
            this.getTimersList().recursiveSearch();
        }

        public void setTimerListDataControl(TimersListDataControl timersList)
        {

            this.timersListDataControl = timersList;
        }

        public void setGlobalStatesListDataContorl(GlobalStateListDataControl globalStatesListDataControl)
        {

            this.globalStatesListDataControl = globalStatesListDataControl;
        }

        public void setMacrosListDataControl(MacroListDataControl macrosListDataControl)
        {

            this.macrosListDataControl = macrosListDataControl;
        }

        public TimersListDataControl getTimersList()
        {

            return timersListDataControl;
        }

        public GlobalStateListDataControl getGlobalStatesListDataControl()
        {

            return globalStatesListDataControl;
        }

        public MacroListDataControl getMacrosListDataControl()
        {

            return macrosListDataControl;
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            List<Searchable> path = getPathFromChild(dataControl, globalStatesListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, timersListDataControl);
            if (path != null)
                return path;
            return getPathFromChild(dataControl, macrosListDataControl);
        }

    }
}
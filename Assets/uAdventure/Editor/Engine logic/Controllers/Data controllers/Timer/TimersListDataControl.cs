using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TimersListDataControl : DataControl
    {
        /**
       * List of timers.
       */
        private List<Timer> timersList;

        /**
         * List of timer controllers.
         */
        private List<TimerDataControl> timersDataControlList;

        /**
         * Constructor.
         * 
         * @param timersList
         *            List of timers
         */

        public TimersListDataControl(List<Timer> timersList)
        {

            this.timersList = timersList;

            // Create subcontrollers
            timersDataControlList = new List<TimerDataControl>();
            foreach (Timer timer in timersList)
                timersDataControlList.Add(new TimerDataControl(timer));
        }

        /**
         * Returns the list of cutscene controllers.
         * 
         * @return Cutscene controllers
         */

        public List<TimerDataControl> getTimers()
        {

            return timersDataControlList;
        }

        /**
         * Returns the last timer controller of the list.
         * 
         * @return Last timer controller
         */

        public TimerDataControl getLastTimer()
        {

            return timersDataControlList[timersDataControlList.Count - 1];
        }

        /**
         * Returns the info of the timers contained in the list.
         * 
         * @return Array with the information of the timers. It contains the index
         *         of each timer, its time, and if it has init conditions, end
         *         conditions, effects or post-effects
         */

        public string[][] getTimersInfo()
        {

            string[][] timersInfo = null;

            // Create the list for the timers info
            timersInfo = new string[timersList.Count][];
            for (int i = 0; i < timersList.Count; i++)
                timersInfo[i] = new string[6];

            // Fill the array with the info
            for (int i = 0; i < timersList.Count; i++)
            {
                Timer timer = timersList[i];
                timersInfo[i][0] = i.ToString();
                timersInfo[i][1] = timersDataControlList[i].getTimeHhMmSs();

                if (!timer.getInitCond().IsEmpty())
                {
                    timersInfo[i][2] = TC.get("GeneralText.Yes");
                }
                else
                {
                    timersInfo[i][2] = TC.get("GeneralText.No");
                }

                if (!timer.getEndCond().IsEmpty())
                {
                    timersInfo[i][3] = TC.get("GeneralText.Yes");
                }
                else
                {
                    timersInfo[i][3] = TC.get("GeneralText.No");
                }

                if (!timer.getEffects().IsEmpty())
                {
                    timersInfo[i][4] = TC.get("GeneralText.Yes");
                }
                else
                {
                    timersInfo[i][4] = TC.get("GeneralText.No");
                }

                if (!timer.getPostEffects().IsEmpty())
                {
                    timersInfo[i][5] = TC.get("GeneralText.Yes");
                }
                else
                {
                    timersInfo[i][5] = TC.get("GeneralText.No");
                }

            }

            return timersInfo;
        }


        public override System.Object getContent()
        {

            return timersList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.TIMER };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new cutscenes
            return type == Controller.TIMER;
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

            if (type == Controller.TIMER)
            {
                // Create the new timer with default time
                Timer newTimer = new Timer();
                newTimer.setRunsInLoop(false);
                newTimer.setMultipleStarts(false);
                newTimer.setUsesEndCondition(false);

                // Add the new timer
                timersList.Add(newTimer);
                timersDataControlList.Add(new TimerDataControl(newTimer));
                controller.DataModified();
                elementAdded = true;
            }

            return elementAdded;
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is TimerDataControl))
                return false;


            Timer newElement = (Timer)(((Timer)(dataControl.getContent())).Clone());
            timersList.Add(newElement);
            timersDataControlList.Add(new TimerDataControl(newElement));
            controller.updateVarFlagSummary();
            controller.DataModified();
            return true;

        }

        private int findDataControlIndex(DataControl dataControl)
        {

            int index = -1;
            for (int i = 0; i < this.timersDataControlList.Count; i++)
            {
                if (timersDataControlList[i] == dataControl)
                {
                    index = i;
                    break;
                }
            }
            return index;

        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;

            int index = findDataControlIndex(dataControl);

            // Ask for confirmation
            if (!askConfirmation ||
                controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"),
                    TC.get("Operation.DeleteElementWarning",
                        new string[] { TC.getElement(Controller.TIMER) + " #" + index, "0" })))
            {
                if (timersList.Remove((Timer)dataControl.getContent()))
                {
                    timersDataControlList.Remove((TimerDataControl)dataControl);
                    controller.updateVarFlagSummary();
                    controller.DataModified();
                    elementDeleted = true;
                }
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = timersList.IndexOf((Timer)dataControl.getContent());

            if (elementIndex > 0)
            {
                Timer e = timersList[elementIndex];
                TimerDataControl c = timersDataControlList[elementIndex];
                timersList.RemoveAt(elementIndex);
                timersDataControlList.RemoveAt(elementIndex);
                timersList.Insert(elementIndex - 1, e);
                timersDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = timersList.IndexOf((Timer)dataControl.getContent());

            if (elementIndex < timersList.Count - 1)
            {
                Timer e = timersList[elementIndex];
                TimerDataControl c = timersDataControlList[elementIndex];
                timersList.RemoveAt(elementIndex);
                timersDataControlList.RemoveAt(elementIndex);
                timersList.Insert(elementIndex + 1, e);
                timersDataControlList.Insert(elementIndex + 1, c);
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

            // Iterate through each timer
            foreach (TimerDataControl timerDataControl in timersDataControlList)
                timerDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.TIMERS_LIST);

            // Iterate through the timers
            for (int i = 0; i < timersDataControlList.Count; i++)
            {
                string cutscenePath = currentPath + " >> " + TC.getElement(Controller.TIMER) + " #" + i;
                valid &= timersDataControlList[i].isValid(cutscenePath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each timer
            foreach (TimerDataControl timerDataControl in timersDataControlList)
                count += timerDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            foreach (TimerDataControl timerDataControl in timersDataControlList)
            {
                timerDataControl.getAssetReferences(assetPaths, assetTypes);
            }
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each timer
            foreach (TimerDataControl timerDataControl in timersDataControlList)
                timerDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each timer
            foreach (TimerDataControl cutsceneDataControl in timersDataControlList)
                count += cutsceneDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each timer
            foreach (TimerDataControl timerDataControl in timersDataControlList)
                timerDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every timer
            foreach (TimerDataControl timerDataControl in timersDataControlList)
                timerDataControl.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            return false;
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.timersDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, timersDataControlList.Cast<Searchable>().ToList());
        }

        public List<Timer> getTimersList()
        {

            return timersList;
        }
    }
}
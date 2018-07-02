using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TimerDataControl : DataControl
    {

        /**
             * Contained timer structure.
             */
        private Timer timer;

        /**
         * Conditions controller (init).
         */
        private ConditionsController initConditionsController;

        /**
         * Conditions controller (end).
         */
        private ConditionsController endConditionsController;

        /**
         * Effects controller
         */
        private EffectsController effectsController;

        /**
         * Post effects controller.
         */
        private EffectsController postEffectsController;

        /**
         * Contructor.
         * 
         * @param timer
         *            Timer structure
         */

        public TimerDataControl(Timer timer)
        {

            this.timer = timer;

            // Create subcontrollers
            Dictionary<string, ConditionsController.ConditionContextProperty> context1 =
                new Dictionary<string, ConditionsController.ConditionContextProperty>();
            ConditionsController.ConditionOwner owner = new ConditionsController.ConditionOwner(Controller.TIMER,
                timer.getDisplayName());
            context1.Add(ConditionsController.CONDITION_OWNER, owner);
            ConditionsController.ConditionCustomMessage cMessage1 =
                new ConditionsController.ConditionCustomMessage(TC.get("Conditions.Context.1A.44"),
                    TC.get("Conditions.Context.2A.44"));
            context1.Add(ConditionsController.CONDITION_CUSTOM_MESSAGE, cMessage1);

            Dictionary<string, ConditionsController.ConditionContextProperty> context2 =
                new Dictionary<string, ConditionsController.ConditionContextProperty>();
            context2.Add(ConditionsController.CONDITION_OWNER, owner);
            ConditionsController.ConditionCustomMessage cMessage2 =
                new ConditionsController.ConditionCustomMessage(TC.get("Conditions.Context.1B.44"),
                    TC.get("Conditions.Context.2B.44"));
            context2.Add(ConditionsController.CONDITION_CUSTOM_MESSAGE, cMessage2);

            initConditionsController = new ConditionsController(timer.getInitCond(), context1);
            endConditionsController = new ConditionsController(timer.getEndCond(), context2);
            effectsController = new EffectsController(timer.getEffects());
            postEffectsController = new EffectsController(timer.getPostEffects());

        }

        /**
         * Returns the init conditions of the timer.
         * 
         * @return Init Conditions of the timer
         */

        public ConditionsController getInitConditions()
        {

            return initConditionsController;
        }

        /**
         * Returns the end conditions of the timer.
         * 
         * @return end Conditions of the timer
         */

        public ConditionsController getEndConditions()
        {

            return endConditionsController;
        }

        /**
         * Returns the effects of the next scene.
         * 
         * @return Effects of the next scene
         */

        public EffectsController getEffects()
        {

            return effectsController;
        }

        /**
         * Returns the post-effects of the next scene.
         * 
         * @return Post-effects of the next scene
         */

        public EffectsController getPostEffects()
        {

            return postEffectsController;
        }


        public override System.Object getContent()
        {

            return timer;
        }


        public override int[] getAddableElements()
        {

            return new int[] { };
        }


        public override bool canAddElement(int type)
        {

            return false;
        }


        public override bool canBeDeleted()
        {

            return true;
        }


        public override bool canBeMoved()
        {

            return true;
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

            // Update the summary with conditions and both blocks of effects
            ConditionsController.updateVarFlagSummary(varFlagSummary, timer.getInitCond());
            ConditionsController.updateVarFlagSummary(varFlagSummary, timer.getEndCond());
            EffectsController.updateVarFlagSummary(varFlagSummary, timer.getEffects());
            EffectsController.updateVarFlagSummary(varFlagSummary, timer.getPostEffects());
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Valid if the effects and the post effects are valid
            valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.Effects"), incidences,
                timer.getEffects());
            valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.PostEffects"), incidences,
                timer.getPostEffects());

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Add to the counter the values of the effects and posteffects
            count += EffectsController.countAssetReferences(assetPath, timer.getEffects());
            count += EffectsController.countAssetReferences(assetPath, timer.getPostEffects());

            return count;
        }


        public override void deleteAssetReferences(string assetPath)
        {

            EffectsController.deleteAssetReferences(assetPath, timer.getEffects());
            EffectsController.deleteAssetReferences(assetPath, timer.getPostEffects());
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Add to the counter the values of the effects and posteffects
            count += EffectsController.countIdentifierReferences(id, timer.getEffects());
            count += EffectsController.countIdentifierReferences(id, timer.getPostEffects());
            count += initConditionsController.countIdentifierReferences(id);
            count += endConditionsController.countIdentifierReferences(id);
            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            EffectsController.replaceIdentifierReferences(oldId, newId, timer.getEffects());
            EffectsController.replaceIdentifierReferences(oldId, newId, timer.getPostEffects());
            initConditionsController.replaceIdentifierReferences(oldId, newId);
            endConditionsController.replaceIdentifierReferences(oldId, newId);

        }


        public override void deleteIdentifierReferences(string id)
        {

            EffectsController.deleteIdentifierReferences(id, timer.getEffects());
            EffectsController.deleteIdentifierReferences(id, timer.getPostEffects());
            initConditionsController.deleteIdentifierReferences(id);
            endConditionsController.deleteIdentifierReferences(id);
        }

        /**
         * Returns the time of the timer represented as hours:minutes:seconds. The
         * string returned will look like: HHh:MMm:SSs
         * 
         * @return The time as HHh:MMm:SSs
         */

        public string getTimeHhMmSs()
        {

            string time = "";
            long seconds = timer.getTime();

            // Less than 60 seconds
            if (seconds < 60 && seconds >= 0)
            {
                time = seconds + "s";
            }

            // Between 1 minute and 60 minutes
            else if (seconds < 3600 && seconds >= 60)
            {
                long minutes = seconds / 60;
                long lastSeconds = seconds % 60;
                time = minutes + "m:" + lastSeconds + "s";
            }

            // One hour or more
            else if (seconds >= 3600)
            {
                long hours = seconds / 3600;
                long minutes = (seconds % 3600) / 60;
                long lastSeconds = (seconds % 3600) % 60;
                time = hours + "h:" + minutes + "m:" + lastSeconds + "s";
            }

            return time;
        }

        /**
         * Returns the time in total seconds
         * 
         * @return The time in seconds
         */

        public long getTime()
        {

            return timer.getTime();
        }

        public string getDocumentation()
        {

            return timer.getDocumentation();
        }

        public void setDocumentation(string newDoc)
        {

            controller.AddTool(new ChangeDocumentationTool(timer, newDoc));
        }

        public void setTime(long newTime)
        {

            controller.AddTool(new ChangeLongValueTool(timer, newTime, "getTime", "setTime"));
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            EffectsController.getAssetReferences(assetPaths, assetTypes, timer.getEffects());
            EffectsController.getAssetReferences(assetPaths, assetTypes, timer.getPostEffects());
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            check(this.getDocumentation(), TC.get("Search.Documentation"));
            check(this.getEndConditions(), TC.get("Search.EndConditions"));
            check(this.getInitConditions(), TC.get("Search.InitConditions"));
            for (int i = 0; i < this.getEffects().getEffectCount(); i++)
            {
                check(this.getEffects().getEffectInfo(i), TC.get("Search.Effect"));
            }
            for (int i = 0; i < this.getPostEffects().getEffectCount(); i++)
            {
                check(this.getPostEffects().getEffectInfo(i), TC.get("Search.PostEffect"));
            }
        }

        public bool isUsesEndCondition()
        {

            return timer.isUsesEndCondition();
        }

        public void setUsesEndCondition(bool selected)
        {

            controller.AddTool(new ChangeBooleanValueTool(timer, selected, "isUsesEndCondition", "setUsesEndCondition"));
        }

        public bool isMultipleStarts()
        {

            return timer.isMultipleStarts();
        }

        public void setMultipleStarts(bool multipleStarts)
        {

            controller.AddTool(new ChangeBooleanValueTool(timer, multipleStarts, "isMultipleStarts", "setMultipleStarts"));
        }

        public bool isRunsInLoop()
        {

            return timer.isRunsInLoop();
        }

        public void setRunsInLoop(bool runsInLoop)
        {

            controller.AddTool(new ChangeBooleanValueTool(timer, runsInLoop, "isRunsInLoop", "setRunsInLoop"));
        }

        public bool isShowTime()
        {

            return timer.isShowTime();
        }

        public void setShowTime(bool selected)
        {

            controller.AddTool(new ChangeBooleanValueTool(timer, selected, "isShowTime", "setShowTime"));
        }

        public bool isCountDown()
        {

            return timer.isCountDown();
        }

        public void setCountDown(bool countDown)
        {

            controller.AddTool(new ChangeBooleanValueTool(timer, countDown, "isCountDown", "setCountDown"));
        }

        public bool isShowWhenStopped()
        {

            return timer.isShowWhenStopped();
        }

        public void setShowWhenStopped(bool showWhenStopped)
        {

            controller.AddTool(new ChangeBooleanValueTool(timer, showWhenStopped, "isShowWhenStopped", "setShowWhenStopped"));
        }

        public string getDisplayName()
        {

            return timer.getDisplayName();
        }

        public void setDisplayName(string displayName)
        {

            controller.AddTool(new ChangeStringValueTool(timer, displayName, "getDisplayName", "setDisplayName"));
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MilestoneDataControl : DataControl
    {
        private Completable.Milestone milestone;
        private ConditionsController conditionsController;

        public MilestoneDataControl(Completable.Milestone milestone)
        {
            this.milestone = milestone;
            this.conditionsController = new ConditionsController(milestone.getConditions());
        }

        public override bool addElement(int type, string id) { return false; }
        public override bool canAddElement(int type) { return false; }
        public override bool canBeDeleted() { return true; }
        public override bool canBeDuplicated() { return true; }
        public override bool canBeMoved() { return true; }
        public override bool canBeRenamed() { return false; }
        public override int countAssetReferences(string assetPath) { return 0; }
        public override int countIdentifierReferences(string id)
        {
            int occurrences = 0;
            if (id.Equals(milestone.getId(), StringComparison.InvariantCultureIgnoreCase))
                ++occurrences;
            return conditionsController.countIdentifierReferences(id) + occurrences;
        }
        public override void deleteAssetReferences(string assetPath) { }
        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }
        public override void deleteIdentifierReferences(string id)
        {
            if (id.Equals(milestone.getId(), StringComparison.InvariantCultureIgnoreCase))
                milestone.setId("");
            conditionsController.deleteIdentifierReferences(id);
        }
        public override int[] getAddableElements() { return new int[0]; }
        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes) { }
        public override object getContent() { return milestone; }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            throw new NotImplementedException();
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            throw new NotImplementedException();
        }

        public override bool moveElementDown(DataControl dataControl) { return false; }
        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override void recursiveSearch()
        {
            throw new NotImplementedException();
        }

        public override string renameElement(string newName) { return null; }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (oldId.Equals(milestone.getId(), StringComparison.InvariantCultureIgnoreCase))
                milestone.setId(newId);
            conditionsController.replaceIdentifierReferences(oldId, newId);
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            // Update the flag summary with the exits, item and character references
            ConditionsController.updateVarFlagSummary(varFlagSummary, milestone.getConditions());
        }

        public Completable.Milestone.MilestoneType getType()
        {
            return milestone.getType();
        }

        public void setType(Completable.Milestone.MilestoneType type)
        {
            Controller.Instance.addTool(new ChangeEnumValueTool(milestone, type, "getType", "setType"));
        }

        public string getId()
        {
            return milestone.getId();
        }

        public void setId(string newId)
        {
            Controller.Instance.addTool(new ChangeIdTool(milestone, newId));
        }

        public float getProgress()
        {
            return milestone.getProgress();
        }

        public void setProgress(float progress)
        {
            Controller.Instance.addTool(new ChangeFloatValueTool(milestone, progress, "getProgress", "setProgress"));
        }

        public ConditionsController getConditions()
        {
            return conditionsController;
        }

        public void setConditions(ConditionsController conditions)
        {
            milestone.setConditions(conditions.Conditions);
            this.conditionsController = conditions;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProgressDataControl : DataControl
    {

        private Completable.Progress progress;
        private List<MilestoneDataControl> milestoneDataControls;

        public ProgressDataControl(Completable.Progress progress)
        {
            this.progress = progress;
            milestoneDataControls = progress.getMilestones().ConvertAll(m => new MilestoneDataControl(m));
        }

        public override bool addElement(int type, string id)
        {
            if (!canAddElement(type))
                return false;

            var milestone = new Completable.Milestone();
            progress.addMilestone(milestone);
            milestoneDataControls.Add(new MilestoneDataControl(milestone));

            return true;
        }

        public override bool canAddElement(int type) { return type == Controller.MILESTONE; }
        public override bool canBeDeleted() { return true; }
        public override bool canBeDuplicated() { return true; }
        public override bool canBeMoved() { return true; }
        public override bool canBeRenamed() { return false; }
        public override int countAssetReferences(string assetPath) { return 0; }
        public override int countIdentifierReferences(string id)
        {
            return milestoneDataControls.Select(m => m.countIdentifierReferences(id)).Sum();
        }
        public override void deleteAssetReferences(string assetPath) { }
        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            if (!(dataControl is MilestoneDataControl))
                return false;

            var milestone = dataControl as MilestoneDataControl;
            if (milestoneDataControls.Contains(milestone))
            {
                milestoneDataControls.Remove(milestone);
                var milestones = progress.getMilestones();
                milestones.Remove(milestone.getContent() as Completable.Milestone);
                progress.setMilestones(milestones);
                return true;
            }

            return false;
        }
        public override void deleteIdentifierReferences(string id)
        {
            milestoneDataControls.ForEach(m => m.deleteIdentifierReferences(id));
        }
        public override int[] getAddableElements() { return new int[1] { Controller.MILESTONE }; }
        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            foreach(var m in milestoneDataControls)
                m.getAssetReferences(assetPaths, assetTypes);
        }
        public override object getContent() { return progress; }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            throw new NotImplementedException();
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            throw new NotImplementedException();
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            // Continue only if dataControl is a Milestone
            if (!(dataControl is MilestoneDataControl))
                return false;

            // Continue only if the dataControl belongs to my set
            var milestone = dataControl as MilestoneDataControl;
            if (!milestoneDataControls.Contains(milestone))
                return false;

            // Continue only if its not the last one
            var index = milestoneDataControls.IndexOf(milestone);
            if (index == milestoneDataControls.Count - 1)
                return false;

            milestoneDataControls.RemoveAt(index);
            milestoneDataControls.Insert(index + 1, milestone);

            var milestones = progress.getMilestones();
            milestones.RemoveAt(index);
            milestones.Insert(index + 1, milestone.getContent() as Completable.Milestone);
            progress.setMilestones(milestones);
            return true;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            // Continue only if dataControl is a Milestone
            if (!(dataControl is MilestoneDataControl))
                return false;

            // Continue only if the dataControl belongs to my set
            var milestone = dataControl as MilestoneDataControl;
            if (!milestoneDataControls.Contains(milestone))
                return false;

            // Continue only if its not the first one
            var index = milestoneDataControls.IndexOf(milestone);
            if (index == 0)
                return false;

            milestoneDataControls.RemoveAt(index);
            milestoneDataControls.Insert(index - 1, milestone);

            var milestones = progress.getMilestones();
            milestones.RemoveAt(index);
            milestones.Insert(index - 1, milestone.getContent() as Completable.Milestone);
            progress.setMilestones(milestones);
            return true;
        }

        public override void recursiveSearch()
        {
            throw new NotImplementedException();
        }

        public override string renameElement(string newName) { return null; }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            foreach(var m in milestoneDataControls)
                m.replaceIdentifierReferences(oldId, newId);
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            foreach (var m in milestoneDataControls)
                m.updateVarFlagSummary(varFlagSummary);
        }

        public List<MilestoneDataControl> getMilestones()
        {
            return milestoneDataControls;
        }

        public float getProgress()
        {
            return progress.getProgress();
        } 

        public void setProgress(float value)
        {
            Controller.Instance.addTool(new ChangeFloatValueTool(progress, value, "getProgress", "setProgress"));
        }

        public Completable.Progress.ProgressType getType()
        {
            return progress.getType();
        }

        public void setType(Completable.Progress.ProgressType type)
        {
            Controller.Instance.addTool(new ChangeEnumValueTool(progress, type, "getType", "setType"));
        }
    }

    public class CompletableDataControl : DataControl
    {
        private Completable completable;

        private MilestoneDataControl startDataControl;
        private MilestoneDataControl endDataControl;
        private ProgressDataControl progressDataControl;
        private ScoreDataControl scoreDataControl;

        public CompletableDataControl(Completable completable)
        {
            this.completable = completable;
            this.startDataControl = new MilestoneDataControl(completable.getStart());
            this.endDataControl = new MilestoneDataControl(completable.getEnd());
            this.progressDataControl = new ProgressDataControl(completable.getProgress());
            this.scoreDataControl = new ScoreDataControl(completable.getScore());

            Controller.Instance.IdentifierSummary.addId<Completable>(completable.getId());
        }

        public override bool addElement(int type, string id) { return false; }
        public override bool canAddElement(int type) { return false; }
        public override bool canBeDeleted() { return true; }
        public override bool canBeDuplicated() { return true; }
        public override bool canBeMoved() { return true; }
        public override bool canBeRenamed() { return false; }
        public override int countAssetReferences(string assetPath) { return 0; }
        public override int countIdentifierReferences(string id)
        {
            int occurrences = 0;
            if (id.Equals(completable.getId(), StringComparison.InvariantCultureIgnoreCase))
                ++occurrences;
            return startDataControl.countIdentifierReferences(id) + endDataControl.countIdentifierReferences(id) + progressDataControl.countIdentifierReferences(id) + occurrences;
        }
        public override void deleteAssetReferences(string assetPath) { }
        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }
        public override void deleteIdentifierReferences(string id)
        {
            if (id.Equals(completable.getId(), StringComparison.InvariantCultureIgnoreCase))
                completable.setId("");

            startDataControl.deleteIdentifierReferences(id);
            endDataControl.deleteIdentifierReferences(id);
            progressDataControl.deleteIdentifierReferences(id);
        }
        public override int[] getAddableElements() { return new int[0]; }
        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            startDataControl.getAssetReferences(assetPaths, assetTypes);
            endDataControl.getAssetReferences(assetPaths, assetTypes);
            progressDataControl.getAssetReferences(assetPaths, assetTypes);
        }
        public override object getContent() { return completable; }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            throw new NotImplementedException();
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            throw new NotImplementedException();
        }

        public override bool moveElementDown(DataControl dataControl) { return false; }
        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override void recursiveSearch()
        {
            throw new NotImplementedException();
        }

        public override string renameElement(string newName)
        {
            string oldSceneId = completable.getId();
            if (oldSceneId == newName)
                return newName;

            string references = controller.countIdentifierReferences(oldSceneId).ToString();

            // Ask for confirmation
            if (newName != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameSceneTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldSceneId, references })))
            {

                // Show a dialog asking for the new scene id
                if (newName == null)
                    controller.ShowInputDialog(TC.get("Operation.RenameSceneTitle"), TC.get("Operation.RenameSceneMessage"), oldSceneId, (o, s) => performRenameElement<Completable>(s));
                else
                    return performRenameElement<Completable>(newName);
            }

            return null;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (oldId.Equals(completable.getId(), StringComparison.InvariantCultureIgnoreCase))
                completable.setId(newId);

            startDataControl.replaceIdentifierReferences(oldId, newId);
            endDataControl.replaceIdentifierReferences(oldId, newId);
            progressDataControl.replaceIdentifierReferences(oldId, newId);
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            startDataControl.updateVarFlagSummary(varFlagSummary);
            endDataControl.updateVarFlagSummary(varFlagSummary);
            progressDataControl.updateVarFlagSummary(varFlagSummary);
        }

        public string getId()
        {
            return completable.getId();
        }

        public MilestoneDataControl getStart()
        {
            return startDataControl;
        }
        public MilestoneDataControl getEnd()
        {
            return endDataControl;
        }
        public ProgressDataControl getProgress()
        {
            return progressDataControl;
        }

        public ScoreDataControl getScore()
        {
            return scoreDataControl;
        }
    }
}


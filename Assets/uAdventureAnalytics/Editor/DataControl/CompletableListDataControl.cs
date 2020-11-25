using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Analytics
{
    public class CompletableListDataControl : DataControl
    {
        private readonly List<CompletableDataControl> completableDataControls;
        private readonly List<Completable> completables;

        public CompletableListDataControl(List<Completable> completables)
        {
            this.completables = completables;
            completableDataControls = completables.ConvertAll(c => new CompletableDataControl(c));
        }


        public override bool addElement(int type, string id)
        {
            var elementAdded = false;

            if (type == AnalyticsController.COMPLETABLE)
            {
                // Show a dialog asking for the scene id
                if (string.IsNullOrEmpty(id))
                {
                    controller.ShowInputIdDialog(TC.get("Operation.AddSceneTitle"), TC.get("Operation.AddSceneMessage"),
                        Controller.Instance.makeElementValid(TC.get("Operation.AddSceneDefaultValue")), performAddElement);
                }
                else
                {
                    performAddElement(null, id);
                    elementAdded = true;
                }

            }

            return elementAdded;
        }

        private void performAddElement(object sender, string id)
        {
            // If some value was typed and the identifier is valid
            if (!controller.isElementIdValid(id))
            {
                id = controller.makeElementValid(id);
            }

            // Add thew new scene
            var newCompletable = new Completable();
            newCompletable.setId(id);
            var score = new Completable.Score();
            score.setMethod(Completable.Score.ScoreMethod.SINGLE);
            score.setType(Completable.Score.ScoreType.VARIABLE);
            newCompletable.setScore(score);
            completables.Add(newCompletable);
            completableDataControls.Add(new CompletableDataControl(newCompletable));
            controller.IdentifierSummary.addId<Completable>(id);
            controller.DataModified();
        }

        public override bool canAddElement(int type) { return type == AnalyticsController.COMPLETABLE; }
        public override bool canBeDeleted() { return true; }
        public override bool canBeDuplicated() { return false; }
        public override bool canBeMoved() { return true; }
        public override bool canBeRenamed() { return false; }
        public override int countAssetReferences(string assetPath) { return 0; }
        public override int countIdentifierReferences(string id)
        {
            return completableDataControls.Select(c => c.countIdentifierReferences(id)).Sum();
        }
        public override void deleteAssetReferences(string assetPath) { }
        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            var completable = dataControl as CompletableDataControl;
            if (completable == null)
            {
                return false;
            }

            if (!completableDataControls.Contains(completable))
            {
                return false;
            }

            completableDataControls.Remove(completable);
            completables.Remove(completable.getContent() as Completable);
            controller.IdentifierSummary.deleteId<Completable>(completable.getId());
            controller.updateVarFlagSummary();
            controller.DataModified();

            return true;
        }

        public override void deleteIdentifierReferences(string id)
        {
            completableDataControls.ForEach(c => c.deleteIdentifierReferences(id));
        }
        public override int[] getAddableElements() { return new int[1] { AnalyticsController.COMPLETABLE }; }
        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            foreach (var c in completableDataControls)
            {
                c.getAssetReferences(assetPaths, assetTypes);
            }
        }
        public override object getContent() { return completables; }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            throw new NotImplementedException();
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return completableDataControls.All(c => c.isValid(currentPath, incidences));
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            var completable = dataControl as CompletableDataControl;
            // It it is not a completable OR is not in the set OR is already the last one
            var index = -1;
            if (completable == null || (index = completableDataControls.IndexOf(completable)) == -1 || index == completableDataControls.Count - 1)
            {
                return false;
            }

            completableDataControls.RemoveAt(index);
            completableDataControls.Insert(index + 1, completable);
            completables.RemoveAt(index);
            completables.Insert(index + 1, completable.getContent() as Completable);
            controller.DataModified();
            return true;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            var completable = dataControl as CompletableDataControl;

            // It it is not a completable OR is not in the set OR is already the first one
            var index = -1;
            if (completable == null || (index = completableDataControls.IndexOf(completable)) == -1 || index == 0)
            {
                return false;
            }

            completableDataControls.RemoveAt(index);
            completableDataControls.Insert(index - 1, completable);
            completables.RemoveAt(index);
            completables.Insert(index - 1, completable.getContent() as Completable);
            controller.DataModified();
            return true;
        }

        public override void recursiveSearch()
        {
            throw new NotImplementedException();
        }

        public override string renameElement(string newName) { return null; }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            foreach (var c in completableDataControls)
            {
                c.replaceIdentifierReferences(oldId, newId);
            }
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            foreach (var c in completableDataControls)
            {
                c.updateVarFlagSummary(varFlagSummary);
            }
        }

        public List<CompletableDataControl> getCompletables()
        {
            return completableDataControls;
        }
    }

    public class ScoreDataControl : DataControl
    {
        private readonly Completable.Score score;
        private readonly List<ScoreDataControl> scoreDataControls;

        public ScoreDataControl(Completable.Score score)
        {
            this.score = score;
            this.scoreDataControls = score.getSubScores().ConvertAll(s => new ScoreDataControl(s));
        }

        public override bool addElement(int type, string id)
        {
            var elementAdded = false;

            if (type == AnalyticsController.SCORE)
            {
                var newSubScore = new Completable.Score();
                newSubScore.setMethod(Completable.Score.ScoreMethod.SINGLE);
                string target = null;
                var vars = controller.VarFlagSummary.getVars();
                if (vars != null && vars.Length > 0)
                {
                    target = vars[0];
                    controller.VarFlagSummary.addVarReference(vars[0]);
                    newSubScore.setType(Completable.Score.ScoreType.VARIABLE);
                }
                else
                {
                    var completables = controller.IdentifierSummary.getIds<Completable>();
                    if (completables != null && completables.Length > 0)
                    {
                        target = completables[0];
                        newSubScore.setType(Completable.Score.ScoreType.COMPLETABLE);
                    }
                }

                if (!string.IsNullOrEmpty(target))
                {
                    newSubScore.setId(target);
                    score.addSubScore(newSubScore);
                    scoreDataControls.Add(new ScoreDataControl(newSubScore));
                }
            }

            return elementAdded;
        }

        public override bool canAddElement(int type) { return score.getMethod() != Completable.Score.ScoreMethod.SINGLE && type == AnalyticsController.SCORE; }
        public override bool canBeDeleted() { return true; }
        public override bool canBeDuplicated() { return true; }
        public override bool canBeMoved() { return true; }
        public override bool canBeRenamed() { return false; }
        public override int countAssetReferences(string assetPath) { return 0; }
        public override int countIdentifierReferences(string id)
        {
            return (id.Equals(score.getId()) ? 1 : 0) + scoreDataControls.Select(s => s.countIdentifierReferences(id)).Sum();
        }
        public override void deleteAssetReferences(string assetPath) { }
        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            var scoreDataControl = dataControl as ScoreDataControl;
            if (scoreDataControl == null)
            {
                return false;
            }

            if (!scoreDataControls.Contains(scoreDataControl))
            {
                return false;
            }

            scoreDataControls.Remove(scoreDataControl);
            this.score.getSubScores().Remove(scoreDataControl.getContent() as Completable.Score);
            controller.updateVarFlagSummary();
            controller.DataModified();
            return true;

        }
        public override void deleteIdentifierReferences(string id)
        {
            if (id.Equals(score.getId()))
            {
                score.setId("");
            }
            scoreDataControls.ForEach(s => s.deleteIdentifierReferences(id));
        }
        public override int[] getAddableElements() { return score.getMethod() != Completable.Score.ScoreMethod.SINGLE ? new int[1] { AnalyticsController.SCORE } : new int[0]; }
        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            foreach (var s in scoreDataControls)
            {
                s.getAssetReferences(assetPaths, assetTypes);
            }
        }
        public override object getContent() { return score; }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            throw new NotImplementedException();
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            var valid = score != null;

            if (!valid)
            {
                return false;
            }

            if (score.getMethod() == Completable.Score.ScoreMethod.SINGLE)
            {
                valid &= scoreDataControls.Count == 0;
                if (score.getType() == Completable.Score.ScoreType.COMPLETABLE)
                {
                    valid &= controller.IdentifierSummary.getIds<Completable>().Contains(score.getId());
                }

            }
            else
            {
                valid &= scoreDataControls.Count > 0;
            }

            valid &= scoreDataControls.All(s => s.isValid(currentPath, incidences));

            return valid;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            var scoreDataControl = dataControl as ScoreDataControl;
            // Continue only if dataControl is a Milestone
            if (scoreDataControl == null)
            {
                return false;
            }

            // Continue only if the dataControl belongs to my set
            if (!scoreDataControls.Contains(scoreDataControl))
            {
                return false;
            }

            // Continue only if its not the last one
            var index = scoreDataControls.IndexOf(scoreDataControl);
            if (index == 0)
            {
                return false;
            }

            scoreDataControls.RemoveAt(index);
            scoreDataControls.Insert(index + 1, scoreDataControl);
            this.score.getSubScores().RemoveAt(index);
            this.score.getSubScores().Insert(index + 1, scoreDataControl.getContent() as Completable.Score);
            controller.DataModified();
            return true;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            var scoreDataControl = dataControl as ScoreDataControl;
            // Continue only if dataControl is a Milestone
            if (scoreDataControl == null)
            {
                return false;
            }

            // Continue only if the dataControl belongs to my set
            if (!scoreDataControls.Contains(scoreDataControl))
            {
                return false;
            }

            // Continue only if its not the last one
            var index = scoreDataControls.IndexOf(scoreDataControl);
            if (index == 0)
            {
                return false;
            }

            scoreDataControls.RemoveAt(index);
            scoreDataControls.Insert(index - 1, scoreDataControl);
            this.score.getSubScores().RemoveAt(index);
            this.score.getSubScores().Insert(index - 1, scoreDataControl.getContent() as Completable.Score);
            controller.DataModified();
            return true;
        }

        public override void recursiveSearch()
        {
            throw new NotImplementedException();
        }

        public override string renameElement(string newName) { return null; }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (oldId.Equals(score.getId()))
            {
                score.setId(newId);
            }

            foreach (var s in scoreDataControls)
            {
                s.replaceIdentifierReferences(oldId, newId);
            }
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            if (score.getMethod() == Completable.Score.ScoreMethod.SINGLE)
            {
                if (score.getType() == Completable.Score.ScoreType.VARIABLE)
                {
                    varFlagSummary.addVarReference(score.getId());
                }
            }
            else
            {
                foreach (var s in scoreDataControls)
                {
                    s.updateVarFlagSummary(varFlagSummary);
                }
            }
        }

        public List<ScoreDataControl> getScores()
        {
            return scoreDataControls;
        }

        public void setMethod(Completable.Score.ScoreMethod scoreMethod)
        {
            if (getMethod() == scoreMethod)
            {
                return;
            }

            if (scoreMethod != Completable.Score.ScoreMethod.SINGLE)
            {
                score.setId("");
            }
            else
            {
                score.setSubScores(new List<Completable.Score>());
                scoreDataControls.Clear();
            }

            Controller.Instance.AddTool(ChangeEnumValueTool.Create(score, scoreMethod, "getMethod", "setMethod"));
        }

        public Completable.Score.ScoreMethod getMethod()
        {
            return score.getMethod();
        }

        public void setType(Completable.Score.ScoreType scoreType)
        {
            score.setId("");
            Controller.Instance.AddTool(ChangeEnumValueTool.Create(score, scoreType, "getType", "setType"));
        }

        public Completable.Score.ScoreType getType()
        {
            return score.getType();
        }

        public string getId()
        {
            return score.getId();
        }

        public void setId(string id)
        {
            if (!id.Equals(score.getId()))
            {
                Controller.Instance.AddTool(new ChangeStringValueTool(score, id, "getId", "setId"));
            }
        }
    }
}


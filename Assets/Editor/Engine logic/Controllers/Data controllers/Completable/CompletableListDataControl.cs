using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Editor{

    public class CompletableListDataControl : DataControl
    {
        private List<CompletableDataControl> completableDataControls;
        private Chapter chapter;

        public CompletableListDataControl(Chapter chapter)
        {
            this.chapter = chapter;
            completableDataControls = chapter.getCompletables().ConvertAll(c => new CompletableDataControl(c));
        }


        public override bool addElement(int type, string id)
        {

            bool elementAdded = false;

            if (type == Controller.COMPLETABLE)
            {

                // Show a dialog asking for the scene id
                if (string.IsNullOrEmpty(id))
                    controller.ShowInputDialog(TC.get("Operation.AddSceneTitle"), TC.get("Operation.AddSceneMessage"), TC.get("Operation.AddSceneDefaultValue"), performAddElement);
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
                id = controller.makeElementValid(id);

            // Add thew new scene
            Completable newCompletable = new Completable();
            newCompletable.setId(id);
            var completables = chapter.getCompletables();
            completables.Add(newCompletable);
            chapter.setCompletables(completables);
            completableDataControls.Add(new CompletableDataControl(newCompletable));
            controller.IdentifierSummary.addId<Completable>(id);
            //controller.dataModified( );
        }

        public override bool canAddElement(int type) { return type == Controller.MILESTONE; }
        public override bool canBeDeleted() { return true; }
        public override bool canBeDuplicated() { return true; }
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
            if (!(dataControl is CompletableDataControl))
                return false;

            var completable = dataControl as CompletableDataControl;
            if (completableDataControls.Contains(completable))
            {
                completableDataControls.Remove(completable);
                
                var completables = chapter.getCompletables();
                completables.Remove(completable.getContent() as Completable);
                chapter.setCompletables(completables);
                controller.IdentifierSummary.deleteId<Completable>(completable.getId());
                return true;
            }

            return false;
        }
        public override void deleteIdentifierReferences(string id)
        {
            completableDataControls.ForEach(c => c.deleteIdentifierReferences(id));
        }
        public override int[] getAddableElements() { return new int[1] { Controller.MILESTONE }; }
        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            foreach (var c in completableDataControls)
                c.getAssetReferences(assetPaths, assetTypes);
        }
        public override object getContent() { return chapter.getCompletables(); }

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
            if (!(dataControl is CompletableDataControl))
                return false;

            // Continue only if the dataControl belongs to my set
            var completable = dataControl as CompletableDataControl;
            if (!completableDataControls.Contains(completable))
                return false;

            // Continue only if its not the last one
            var index = completableDataControls.IndexOf(completable);
            if (index == completableDataControls.Count - 1)
                return false;

            completableDataControls.RemoveAt(index);
            completableDataControls.Insert(index + 1, completable);

            var completables = chapter.getCompletables();
            completables.RemoveAt(index);
            completables.Insert(index + 1, completable.getContent() as Completable);
            chapter.setCompletables(completables);
            return true;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            // Continue only if dataControl is a Milestone
            if (!(dataControl is CompletableDataControl))
                return false;

            // Continue only if the dataControl belongs to my set
            var completable = dataControl as CompletableDataControl;
            if (!completableDataControls.Contains(completable))
                return false;

            // Continue only if its not the last one
            var index = completableDataControls.IndexOf(completable);
            if (index == 0)
                return false;

            completableDataControls.RemoveAt(index);
            completableDataControls.Insert(index - 1, completable);

            var completables = chapter.getCompletables();
            completables.RemoveAt(index);
            completables.Insert(index - 1, completable.getContent() as Completable);
            chapter.setCompletables(completables);
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
                c.replaceIdentifierReferences(oldId, newId);
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            foreach (var c in completableDataControls)
                c.updateVarFlagSummary(varFlagSummary);
        }
    }
}


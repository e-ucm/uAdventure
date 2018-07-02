using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DescriptionsController : DataControl
    {
        private List<Description> descriptions;

        private List<DescriptionController> descriptionsController;

        private int selectedDescription = 0;

        public DescriptionsController(List<Description> descriptions)
        {
            this.descriptions = descriptions;

            descriptionsController = new List<DescriptionController>();
            this.descriptions = descriptions;

            if (this.descriptions == null || this.descriptions.Count == 0)
            {

                Description d = new Description();
                this.descriptions.Add(d);
                descriptionsController.Add(new DescriptionController(d));

            }
            else
            {
                foreach (Description d in descriptions)
                {
                    descriptionsController.Add(new DescriptionController(d));
                }
            }

        }

        public List<DescriptionController> getDescriptions()
        {
            return descriptionsController;
        }

        public int getDescriptionCount()
        {
            return descriptionsController.Count;
        }

        public string getName(int index)
        {
            return descriptionsController[index].getName();

        }

        public int getNumberOfDescriptions()
        {
            return descriptionsController.Count;
        }

        public Description getSelectedDescription()
        {
            return descriptions[selectedDescription];
        }

        public DescriptionController getSelectedDescriptionController()
        {
            return descriptionsController[selectedDescription];
        }

        public int getSelectedDescriptionNumber()
        {
            return selectedDescription;
        }

        public string getSelectedName()
        {
            return descriptions[selectedDescription].getName();
        }


        public void setSelectedDescription(int selectedDescription)
        {

            this.selectedDescription = selectedDescription;
        }

        public void addDescription(Description description)
        {
            this.descriptions.Add(description);
        }

        public void addDescriptionController(DescriptionController desController)
        {
            this.descriptionsController.Add(desController);
        }

        public void addDescription(Description description, int index)
        {
            this.descriptions.Insert(index, description);
        }

        public void addDescriptionController(DescriptionController desController, int index)
        {
            this.descriptionsController.Insert(index, desController);
        }

        public bool removeDescription(Description description)
        {
            return this.descriptions.Remove(description);
        }

        public bool removeDescriptionController(DescriptionController desController)
        {
            return this.descriptionsController.Remove(desController);
        }

        /**
         * Remove the selected description and its controller
         * 
         * @return
         *      The deleted description controller
         */
        public DescriptionController removeSelectedDescription()
        {

            descriptions.RemoveAt(this.selectedDescription);
            DescriptionController c = descriptionsController[selectedDescription];
            descriptionsController.RemoveAt(this.selectedDescription);
            return c;
        }



        public DescriptionController getDescriptionController(int index)
        {
            return descriptionsController[index];
        }

        public override bool duplicateElement(DataControl dataControl)
        {

            return Controller.Instance.AddTool(new DuplicateDescriptionTool(this));
        }



        public override System.Object getContent()
        {
            return this.descriptions;
        }



        public override int[] getAddableElements()
        {
            return new int[] { 999 };
        }



        public override bool canAddElement(int type)
        {
            return type == 999;
        }



        public override bool canBeDeleted()
        {
            return false;
        }



        public override bool canBeDuplicated()
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
            if(type == 999)
            {
                controller.AddTool(new AddDescriptionTool(this));
                return true;
            }

            return false;
        }



        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            if (this.getDescriptions().Contains(dataControl as DescriptionController))
            {
                controller.AddTool(new RemoveDescriptionTool(this));
                return true;
            }
            
            return false;
        }




        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = descriptions.IndexOf((Description)dataControl.getContent());

            if (elementIndex > 0)
            {
                Description e = descriptions[elementIndex];
                DescriptionController c = descriptionsController[elementIndex];
                descriptions.RemoveAt(elementIndex);
                descriptionsController.RemoveAt(elementIndex);
                descriptions.Insert(elementIndex - 1, e);
                descriptionsController.Insert(elementIndex - 1, c);
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = descriptions.IndexOf((Description)dataControl.getContent());

            if (elementIndex < descriptions.Count - 1)
            {
                Description e = descriptions[elementIndex];
                DescriptionController c = descriptionsController[elementIndex];
                descriptions.RemoveAt(elementIndex);
                descriptionsController.RemoveAt(elementIndex);
                descriptions.Insert(elementIndex + 1, e);
                descriptionsController.Insert(elementIndex + 1, c);
                elementMoved = true;
            }

            return elementMoved;
        }



        public override string renameElement(string newName)
        {
            return null;
        }



        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            foreach (Description d in descriptions)
            {
                ConditionsController.updateVarFlagSummary(varFlagSummary, d.getConditions());
            }
        }



        public override bool isValid(string currentPath, List<string> incidences)
        {
            return true;
        }



        public override int countAssetReferences(string assetPath)
        {
            int refs = 0;
            foreach (Description d in descriptions)
            {
                if (d.getDescriptionSoundPath() != null && d.getDescriptionSoundPath().Equals(assetPath))
                {
                    refs++;
                }
                if (d.getNameSoundPath() != null && d.getNameSoundPath().Equals(assetPath))
                {
                    refs++;
                }
                if (d.getDetailedDescriptionSoundPath() != null && d.getDetailedDescriptionSoundPath().Equals(assetPath))
                {
                    refs++;
                }
            }
            return refs;
        }



        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            foreach (Description d in descriptions)
            {
                if (d.getDescriptionSoundPath() != null && d.getDescriptionSoundPath().Length > 0)
                {
                    assetPaths.Add(d.getDescriptionSoundPath());
                    assetTypes.Add(AssetsConstants.CATEGORY_AUDIO);
                }
                if (d.getNameSoundPath() != null && d.getNameSoundPath().Length > 0)
                {
                    assetPaths.Add(d.getNameSoundPath());
                    assetTypes.Add(AssetsConstants.CATEGORY_AUDIO);
                }
                if (d.getDetailedDescriptionSoundPath() != null && d.getDetailedDescriptionSoundPath().Length > 0)
                {
                    assetPaths.Add(d.getDetailedDescriptionSoundPath());
                    assetTypes.Add(AssetsConstants.CATEGORY_AUDIO);
                }
            }
        }



        public override void deleteAssetReferences(string assetPath)
        {
            foreach (Description d in descriptions)
            {
                if (d.getDescriptionSoundPath() != null && d.getDescriptionSoundPath().Equals(assetPath))
                {
                    d.setDescriptionSoundPath(null);
                }
                if (d.getNameSoundPath() != null && d.getNameSoundPath().Equals(assetPath))
                {
                    d.setNameSoundPath(null);
                }
                if (d.getDetailedDescriptionSoundPath() != null && d.getDetailedDescriptionSoundPath().Equals(assetPath))
                {
                    d.setDetailedDescriptionSoundPath(null);
                }
            }
        }



        public override int countIdentifierReferences(string id)
        {
            int refs = 0;
            foreach (DescriptionController d in descriptionsController)
            {
                refs += d.getConditionsController().countIdentifierReferences(id);
            }
            return refs;
        }



        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            foreach (DescriptionController d in descriptionsController)
            {
                d.getConditionsController().replaceIdentifierReferences(oldId, newId);
            }
        }



        public override void deleteIdentifierReferences(string id)
        {
            foreach (DescriptionController d in descriptionsController)
            {
                d.getConditionsController().deleteIdentifierReferences(id);
            }
        }



        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }



        public override void recursiveSearch()
        {
            foreach (DescriptionController d in descriptionsController)
            {
                check(d.getConditionsController(), TC.get("Search.Conditions"));
                check(d.getBriefDescription(), TC.get("Search.BriefDescription"));
                check(d.getDescriptionSoundPath(), TC.get("Search.BriefDescription"));
                check(d.getDetailedDescription(), TC.get("Search.DetailedDescription"));
                check(d.getDetailedDescriptionSoundPath(), TC.get("Search.DetailedDescription"));
                check(d.getName(), TC.get("Search.Name"));
                check(d.getNameSoundPath(), TC.get("Search.Name"));
            }
        }
    }
}
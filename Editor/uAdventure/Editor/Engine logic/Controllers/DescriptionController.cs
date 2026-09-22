using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class DescriptionController : DataControl
    {

        private Description description;

        private ConditionsController conditionsController;

        public DescriptionController(Description description)
        {
            this.description = description;

            if (description.getConditions() == null)
            {
                description.setConditions(new Conditions());
            }

            conditionsController = new ConditionsController(description.getConditions());

        }

        public ConditionsController getConditionsController()
        {
            return conditionsController;
        }

        public string getName()
        {
            return description.getName();

        }

        public string getBriefDescription()
        {
            return description.getDescription();

        }

        public string getDetailedDescription()
        {
            return description.getDetailedDescription();

        }

        public string getNameSoundPath()
        {
            return description.getNameSoundPath();

        }

        public string getDescriptionSoundPath()
        {
            return description.getDescriptionSoundPath();

        }

        public string getDetailedDescriptionSoundPath()
        {
            return description.getDetailedDescriptionSoundPath();

        }


        public void setDescriptionData(Description description)
        {
            this.description = description;
        }


        public Description getDescriptionData()
        {
            return description;
        }

        public override object getContent()
        {
            return description;
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override bool canAddElement(int type)
        {
            return false;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
        {
            return true;
        }

        public override bool canBeMoved()
        {
            return true;
        }

        public override bool canBeRenamed()
        {
            return true;
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

        public override string renameElement(string newName)
        {
            this.description.setName(newName);
            return newName;
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return true;
        }

        public override int countAssetReferences(string assetPath)
        {
            return 0;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
        }

        public override void deleteAssetReferences(string assetPath)
        {
        }

        public override int countIdentifierReferences(string id)
        {
            return 0;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
        }

        public override void deleteIdentifierReferences(string id)
        {
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return new List<Searchable>();
        }

        public override void recursiveSearch()
        {
        }
    }
}
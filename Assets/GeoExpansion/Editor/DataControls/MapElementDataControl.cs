using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEngine;

using uAdventure.Editor;

namespace uAdventure.Geo
{
	public abstract class MapElementDataControl : DataControl, IElementReference, HasTargetId
    {
        private readonly ConditionsController conditions;
        private readonly MapElement mapElement;

        protected MapElementDataControl(MapElement mapElement)
        {
            this.mapElement = mapElement;
            conditions = new ConditionsController(mapElement.Conditions);
        }

        public ConditionsController Conditions
        {
            get { return conditions; }
        }

        public abstract DataControl ReferencedDataControl { get; }

        public virtual string ReferencedId
        {
            get { return mapElement.getTargetId(); }
        }

        public Orientation Orientation
        {
            get { return mapElement.Orientation; }
            set
            {
                controller.AddTool(ChangeEnumValueTool.Create(mapElement, value, "Orientation"));
                
            }
        }

        public float Scale
        {
            get { return mapElement.Scale; }
            set
            {
                controller.AddTool(new ChangeValueTool<MapElement, float>(mapElement, value, "Scale", Changed));
            }
        }

        public abstract bool UsesOrientation { get; }

        public override object getContent()
        {
            return mapElement;
        }

        public override int[] getAddableElements() { return null; }

        public override bool canAddElement(int type) { return false; }

        public override bool canBeDeleted() { return true; }

        public override bool canBeDuplicated() { return true; }

        public override bool canBeMoved() { return true; }

        public override bool canBeRenamed() { return false; }

        public override bool addElement(int type, string id) { return false; }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }

        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override bool moveElementDown(DataControl dataControl) { return false; }

        public override string renameElement(string newName) { return null; }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            ConditionsController.updateVarFlagSummary(varFlagSummary, conditions.Conditions);
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
            return mapElement.getTargetId().Equals(id, StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (mapElement.getTargetId().Equals(oldId, StringComparison.InvariantCultureIgnoreCase))
            {
                mapElement.setTargetId(newId);
            }
        }

        public override void deleteIdentifierReferences(string id)
        {
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }

        public override void recursiveSearch()
        {
            throw new System.NotImplementedException();
        }

        public string getTargetId()
        {
            return mapElement.getTargetId();
        }

        public abstract void setTargetId(string id);
    }
}

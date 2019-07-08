using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using uAdventure.Editor;
using System;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Geo
{
    public class GeoActionDataControl : DataControl
    {
        private readonly GeoAction geoAction;
        private readonly string Type;
        private readonly ConditionsController conditionsController;
        private readonly EffectsController effectsController;

        public GeoActionDataControl(GeoAction geoAction)
        {
            this.geoAction = geoAction;
            this.Type = geoAction.Name;
            this.conditionsController = new ConditionsController(geoAction.Conditions);
            this.effectsController = new EffectsController(geoAction.Effects);
        }

        public string getType()
        {
            return Type;
        }

        public ConditionsController Conditions
        {
            get { return conditionsController; }
        }

        public EffectsController Effects
        {
            get { return effectsController; }
        }

        public string[] Parameters
        {
            get { return geoAction.Parameters; }
        }

        public object this[string parameter]
        {
            get { return geoAction[parameter]; }
            set { controller.AddTool(new ChangeParameterTool(geoAction, parameter, value)); }
        }

        private class ChangeParameterTool : Tool
        {
            private readonly GeoAction geoAction;
            private readonly object originalValue;
            private object newValue;
            private readonly string parameter;
            private readonly bool ok;

            public ChangeParameterTool(GeoAction geoAction, string parameter, object value)
            {
                this.parameter = parameter;
                this.geoAction = geoAction;
                this.newValue = value;
                this.originalValue = geoAction[parameter];
                this.ok = geoAction.Parameters.Contains(parameter);
            }

            public override bool canRedo()
            {
                return ok;
            }

            public override bool canUndo()
            {
                return ok;
            }

            public override bool combine(Tool other)
            {
                var otherChange = other as ChangeParameterTool;

                if (ok && otherChange.ok && parameter.Equals(otherChange.parameter, StringComparison.InvariantCulture))
                {
                    this.newValue = otherChange.newValue;
                    return true;
                }

                return false;
            }

            public override bool doTool()
            {
                if (ok)
                {
                    geoAction[parameter] = newValue;
                }

                return ok;
            }

            public override bool redoTool()
            {
                return doTool();
            }

            public override bool undoTool()
            {
                if (ok)
                {
                    geoAction[parameter] = originalValue;
                }

                return ok;
            }
        }

        public override bool addElement(int type, string id) { return false; }

        public override bool canAddElement(int type) { return false; }

        public override bool canBeDeleted() { return true; }

        public override bool canBeDuplicated() { return true; }

        public override bool canBeMoved() { return true; }

        public override bool canBeRenamed() { return false; }

        public override int countAssetReferences(string assetPath)
        {
            return EffectsController.countAssetReferences(assetPath, effectsController.getEffectsDirectly());
        }

        public override int countIdentifierReferences(string id)
        {
            return EffectsController.countIdentifierReferences(id, effectsController.getEffectsDirectly());
        }

        public override void deleteAssetReferences(string assetPath)
        {
            EffectsController.deleteAssetReferences(assetPath, effectsController.getEffectsDirectly());
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }

        public override void deleteIdentifierReferences(string id)
        {
            EffectsController.deleteIdentifierReferences(id, effectsController.getEffectsDirectly());
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            EffectsController.getAssetReferences(assetPaths, assetTypes, effectsController.getEffectsDirectly());
        }

        public override object getContent()
        {
            return geoAction;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            var valid = false;
            return valid & EffectsController.isValid(currentPath, incidences, effectsController.getEffectsDirectly());
        }

        public override bool moveElementDown(DataControl dataControl) { return false; }

        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override void recursiveSearch() { }

        public override string renameElement(string newName) { return null; }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            EffectsController.replaceIdentifierReferences(oldId, newId, effectsController.getEffectsDirectly());
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            EffectsController.updateVarFlagSummary(varFlagSummary, effectsController.getEffectsDirectly());
        }
    }
}

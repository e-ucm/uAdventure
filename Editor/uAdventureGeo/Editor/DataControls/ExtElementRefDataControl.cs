using System.Collections.Generic;
using uAdventure.Core;

using uAdventure.Editor;
using System.Linq;
using MoreLinq;
using System;

using TD = uAdventure.Editor.DefaultElementFactory<uAdventure.Geo.GeoActionDataControl>.ElementCreator.TypeDescriptor;

namespace uAdventure.Geo
{
    public class TransformManagerDataControl : DataControl, IObserver<DataControl>
    {
        private readonly ExtElementRefDataControl extElemReferencedataControl;
        private readonly ExtElemReference extElemReference;
        private IGuiMapPositionManager positionManager;
        private bool registerChanges;

        public TransformManagerDataControl(ExtElementRefDataControl extElemReferencedataControl)
        {
            this.extElemReferencedataControl = extElemReferencedataControl;
            this.extElemReference = extElemReferencedataControl.getContent() as ExtElemReference;
            registerChanges = false;
            this.positionManager = GuiMapPositionManagerFactory.Instance.CreateInstance(extElemReference.TransformManagerDescriptor, this);
            registerChanges = true;
            extElemReferencedataControl.Subscribe(this);
        }

        public Dictionary<string, ParameterDescription> ParameterDescription
        {
            get { return extElemReference.TransformManagerDescriptor.ParameterDescription; }
        }

        public IGuiMapPositionManager GUIMapPositionManager
        {
            get { return positionManager; }
        }

        public string[] ValidPositionManagers
        {
            get { return TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.Select(kv => kv.Value).ToArray(); }
        }

        public float Scale
        {
            get { return extElemReferencedataControl.Scale; }
            set { extElemReferencedataControl.Scale = value; }
        }

        public Orientation Orientation
        {
            get { return extElemReference.Orientation; }
            set { extElemReference.Orientation = value; }
        }

        public string PositionManagerName
        {
            get { return extElemReference.TransformManagerDescriptor.Name; }
            set
            {
                var descriptors =
                    TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.Select(kv =>
                        new {kv.Value, kv.Key});

                var validDescriptor = descriptors.Where(d => d.Value == value).Select(d => d.Key);
                if (validDescriptor.Any())
                {
                    var descriptor =
                        TransformManagerDescriptorFactory.Instance.CreateDescriptor(validDescriptor.First());

                    extElemReference.TransformManagerDescriptor = descriptor;
                    positionManager =
                        GuiMapPositionManagerFactory.Instance.CreateInstance(
                            extElemReference.TransformManagerDescriptor, this);
                    Changed();
                }
            }
        }

        public bool ContainsParameter(string parameter)
        {
            return extElemReference.TransformManagerParameters.ContainsKey(parameter);
        }

        public object this[string parameter]
        {
            get { return extElemReference.TransformManagerParameters[parameter]; }
            set
            {
                if (extElemReference.TransformManagerParameters.ContainsKey(parameter) &&
                    extElemReference.TransformManagerParameters[parameter].Equals(value))
                {
                    return;
                }

                if (!registerChanges)
                {
                    extElemReference.TransformManagerParameters[parameter] = value;
                }
                else
                {
                    controller.AddTool(new ChangeParameterTool(this, new KeyValuePair<string, object>(parameter, value)));
                }
            }
        }

        private class ChangeParameterTool : Tool
        {
            private readonly TransformManagerDataControl transformManagerDataControl;
            private readonly Dictionary<string, object> oldValues;
            private KeyValuePair<string, object>[] newValues;

            public ChangeParameterTool(TransformManagerDataControl transformManagerDataControl, params KeyValuePair<string, object>[] newValues)
            {
                this.transformManagerDataControl = transformManagerDataControl;
                this.newValues = newValues;

                this.oldValues = new Dictionary<string, object>();
                foreach (var kv in newValues)
                {
                    oldValues[kv.Key] = transformManagerDataControl.ContainsParameter(kv.Key) ? transformManagerDataControl[kv.Key] : null;
                }
            }

            public override bool canRedo() { return true; }

            public override bool canUndo() { return true; }

            public override bool combine(Tool other)
            {
                var otherChange = other as ChangeParameterTool;
                if (otherChange == null || newValues.Length != otherChange.newValues.Length)
                {
                    return false;
                }

                var paramsA = newValues.Select(kv => kv.Key);
                var paramsB = otherChange.newValues.Select(kv => kv.Key);

                if (paramsA.Except(paramsB).Any())
                {
                    return false;
                }

                newValues = otherChange.newValues;

                return true;
            }

            public override bool doTool()
            {
                SetValues(newValues);
                transformManagerDataControl.Changed();
                return true;
            }

            public override bool redoTool()
            {
                return doTool();
            }

            public override bool undoTool()
            {
                SetValues(oldValues);
                transformManagerDataControl.Changed();
                return true;
            }

            private void SetValues(IEnumerable<KeyValuePair<string, object>> values)
            {
                foreach (var kv in values)
                {
                    transformManagerDataControl.extElemReference.TransformManagerParameters[kv.Key] = kv.Value;
                }
            }
        }

        public override object getContent()
        {
            return extElemReference.TransformManagerParameters;
        }

        public override int[] getAddableElements() { return null; }
        public override bool canAddElement(int type) { return false; }
        public override bool canBeDeleted() { return false; }
        public override bool canBeDuplicated() { return false; }
        public override bool canBeMoved() { return false; }
        public override bool canBeRenamed() { return false; }
        public override bool addElement(int type, string id) { return false; }
        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }
        public override bool moveElementUp(DataControl dataControl) { return false; }
        public override bool moveElementDown(DataControl dataControl) { return false; }
        public override string renameElement(string newName) { return null; }
        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary) {}

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return extElemReference.TransformManagerDescriptor.ParameterDescription.All(parameter =>
            {
                var description = parameter.Value;
                object value;
                extElemReference.TransformManagerParameters.TryGetValue(parameter.Key, out value);
                var c = value as IComparable;

                return value != null && value.GetType() == description.Type && 
                   (description.AllowedValues == null || description.AllowedValues.Contains(value)) &&
                   (c == null || description.MaxValue == null || c.CompareTo(description.MaxValue) <= 0) &&
                   (c == null || description.MinValue == null || c.CompareTo(description.MinValue) >= 0);
            });
        }

        public override int countAssetReferences(string assetPath) {return 0;}

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes) {}

        public override void deleteAssetReferences(string assetPath){}

        public override int countIdentifierReferences(string id)
        {
            return extElemReference.TransformManagerParameters.Count(kv =>
                kv.Value != null && kv.Value is string &&
                ((string) kv.Value).Equals(id, StringComparison.InvariantCulture));
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            foreach (var k in extElemReference.TransformManagerParameters.Keys.ToList())
            {
                var s = extElemReference.TransformManagerParameters[k] as string;
                if (s != null && s.Equals(oldId, StringComparison.InvariantCulture))
                {
                    extElemReference.TransformManagerParameters[k] = newId;
                }
            }
        }

        public override void deleteIdentifierReferences(string id)
        {
            var toUnset = new List<string>();
            foreach (var kv in extElemReference.TransformManagerParameters)
            {
                var s = kv.Value as string;
                if (s != null && s.Equals(id, StringComparison.InvariantCulture))
                {
                    toUnset.Add(kv.Key);
                }
            }

            toUnset.ForEach(k => extElemReference.TransformManagerParameters.Remove(k));
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl){return null;}

        public override void recursiveSearch()
        {
            extElemReference.TransformManagerDescriptor.ParameterDescription.ForEach(parameter =>
            {
                if (parameter.Value.Type == typeof(string))
                {
                    check(extElemReference.TransformManagerParameters[parameter.Key] as string, parameter.Key);
                }
            });
        }

        public void OnCompleted()
        {
            Changed();
        }

        public void OnError(Exception error)
        {
            Changed();
        }

        public void OnNext(DataControl value)
        {
            Changed();
        }
    }

    public class ExtElementRefDataControl : MapElementDataControl
    {
        private readonly ExtElemReference extElemReference;
        private readonly TransformManagerDataControl transformManagerDataControl;
        private readonly ListDataControl<ExtElementRefDataControl, GeoActionDataControl> geoActionDataControls;

        private int type;

        public override DataControl ReferencedDataControl
        {
            get { return getReferencedElementDataControl(); }
        }
        public ListDataControl<ExtElementRefDataControl, GeoActionDataControl> GeoActions
        {
            get { return geoActionDataControls; }
        }

        public ExtElementRefDataControl(ExtElemReference extElemReference) : base(extElemReference)
        {
            this.extElemReference = extElemReference;
            type = getReferenceType(extElemReference.getTargetId());
            this.transformManagerDataControl = new TransformManagerDataControl(this); 

            geoActionDataControls = new ListDataControl<ExtElementRefDataControl, GeoActionDataControl>(this, extElemReference.Actions, new []
            {
                new ListDataControl<ExtElementRefDataControl, GeoActionDataControl>.ElementFactoryView
                {
                    Titles =
                    {
                        { GeoElementDataControl.ENTER_ACTION   , "Geo.Create.Title.EnterAction"},
                        { GeoElementDataControl.EXIT_ACTION    , "Geo.Create.Title.ExitAction"},
                        { GeoElementDataControl.LOOK_TO_ACTION , "Geo.Create.Title.LookToAction"}
                    },
                    Messages =
                    {
                        { GeoElementDataControl.ENTER_ACTION   , "Geo.Create.Message.EnterAction"},
                        { GeoElementDataControl.EXIT_ACTION    , "Geo.Create.Message.ExitAction"},
                        { GeoElementDataControl.LOOK_TO_ACTION , "Geo.Create.Message.LookToAction"}
                    },
                    Errors =
                    {
                        { GeoElementDataControl.ENTER_ACTION   , "Geo.Create.Error.EnterAction"},
                        { GeoElementDataControl.EXIT_ACTION    , "Geo.Create.Error.ExitAction"},
                        { GeoElementDataControl.LOOK_TO_ACTION , "Geo.Create.Error.LookToAction"}
                    },
                    ElementFactory = new DefaultElementFactory<GeoActionDataControl>(new DefaultElementFactory<GeoActionDataControl>.ElementCreator()
                    {
                        TypeDescriptors = new []
                        {
                            new TD
                            {
                                Type = GeoElementDataControl.ENTER_ACTION,
                                ContentType = typeof(EnterAction)
                            },
                            new TD
                            {
                                Type = GeoElementDataControl.EXIT_ACTION,
                                ContentType = typeof(ExitAction)
                            },
                            new TD
                            {
                                Type = GeoElementDataControl.LOOK_TO_ACTION,
                                ContentType = typeof(InspectAction)
                            }
                        },
                        CreateDataControl = action => new GeoActionDataControl(action as GeoAction),
                        CreateElement = (type, id, _) =>
                        {
                            switch (type)
                            {
                                case GeoElementDataControl.ENTER_ACTION:   return new GeoActionDataControl(new EnterAction());
                                case GeoElementDataControl.EXIT_ACTION:    return new GeoActionDataControl(new ExitAction());
                                case GeoElementDataControl.LOOK_TO_ACTION: return new GeoActionDataControl(new LookToAction());
                                default: return null;
                            }
                        }
                    })
                }
            });
        }

        private static int getReferenceType(string id)
        {
            var referenceType = -1;
            if (Controller.Instance.IdentifierSummary.isType<Item>(id))
            {
                referenceType = Controller.ITEM_REFERENCE;
            }
            else if (Controller.Instance.IdentifierSummary.isType<Atrezzo>(id))
            {
                referenceType = Controller.ATREZZO_REFERENCE;
            }
            else if (Controller.Instance.IdentifierSummary.isType<NPC>(id))
            {
                referenceType = Controller.NPC_REFERENCE;
            }

            return referenceType;
        }

        public TransformManagerDataControl TransformManager
        {
            get { return transformManagerDataControl; }
        }

        public override bool UsesOrientation
        {
            get
            {
                return type == Controller.NPC_REFERENCE || type == Controller.PLAYER;
            }
        }

        public override void setTargetId(string id)
        {
            if (!Controller.Instance.IdentifierSummary.isType<NPC>(id)
                || !Controller.Instance.IdentifierSummary.isType<Atrezzo>(id)
                || !Controller.Instance.IdentifierSummary.isType<Item>(id))
            {
                return;
            }

            controller.AddTool(new ChangeTargetIdToolWithType(this, id));
        }

        public DataControl getReferencedElementDataControl()
        {

            switch (type)
            {
                case Controller.ATREZZO_REFERENCE:
                    AtrezzoListDataControl aldc = Controller.Instance.SelectedChapterDataControl.getAtrezzoList();
                    foreach (AtrezzoDataControl adc in aldc.getAtrezzoList())
                    {
                        if (adc.getId().Equals(this.getTargetId()))
                        {
                            return adc;
                        }
                    }
                    break;
                case Controller.NPC_REFERENCE:
                    NPCsListDataControl nldc = Controller.Instance.SelectedChapterDataControl.getNPCsList();
                    foreach (NPCDataControl ndc in nldc.getNPCs())
                    {
                        if (ndc.getId().Equals(this.getTargetId()))
                        {
                            return ndc;
                        }
                    }
                    break;
                case Controller.ITEM_REFERENCE:
                    ItemsListDataControl ildc = Controller.Instance.SelectedChapterDataControl.getItemsList();
                    foreach (ItemDataControl idc in ildc.getItems())
                    {
                        if (idc.getId().Equals(this.getTargetId()))
                        {
                            return idc;
                        }
                    }
                    break;
            }
            return null;

        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            if (type == -1 || getReferenceType(this.getTargetId()) != type)
            {
                incidences.Add("The target id is not valid for the external element in the map scene.");
                return false;
            }

            return true;
        }

        private class ChangeTargetIdToolWithType : ChangeTargetIdTool
        {
            private readonly ExtElementRefDataControl data;
            private int newReferenceType;
            private readonly int oldReferenceType;

            public ChangeTargetIdToolWithType(ExtElementRefDataControl data, string targetId) : base(data.extElemReference, targetId)
            {
                this.data = data;
                this.newReferenceType = getReferenceType(targetId);
                this.oldReferenceType = data.type;
            }

            public override bool doTool()
            {
                if (newReferenceType != -1 && base.doTool())
                {
                    data.type = newReferenceType;
                    return true;
                }

                return false;
            }

            public override bool combine(Tool other)
            {
                var otherChangeTargetId = other as ChangeTargetIdToolWithType;
                if (otherChangeTargetId != null && base.combine(other))
                {
                    newReferenceType = otherChangeTargetId.newReferenceType;
                    return true;
                }

                return false;
            }

            public override bool undoTool()
            {
                if (oldReferenceType != -1 && base.undoTool())
                {
                    data.type = oldReferenceType;
                    return true;
                }

                return false;
            }
        }
    }
}

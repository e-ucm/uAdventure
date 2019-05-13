using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEngine;

using uAdventure.Editor;

namespace uAdventure.Geo
{
	public class MapSceneDataControl : DataControl, IxAPIConfigurable
    {

        public const int GEO_REFERENCE = 981285;


        private readonly MapScene mapScene;

        private readonly ListDataControl<MapSceneDataControl, MapElementDataControl> mapElementListDataControl;

        private readonly Dictionary<string, List<string>> xApiOptions;

        public MapSceneDataControl(MapScene mapScene)
        {
            this.mapScene = mapScene;
            this.mapElementListDataControl = new ListDataControl<MapSceneDataControl, MapElementDataControl>(this, mapScene.Elements, new ListDataControl<MapSceneDataControl, MapElementDataControl>.ElementFactoryView()
            {
                Titles   = {{ GEO_REFERENCE, "Operation.AddGeoReferenceTitle" }},
                Messages = {{ GEO_REFERENCE, "Operation.AddGeoReferenceMessage" }},
                Errors   = {{ GEO_REFERENCE, "Operation.AddGeoReferenceErrorNoItems" }},
                ElementFactory = new DefaultElementFactory<MapElementDataControl>(new DefaultElementFactory<MapElementDataControl>.ElementCreator()
                {
                    CreateDataControl = o => new GeoElementRefDataControl(o as GeoReference),
                    CreateElement = (type, targetId) => new GeoElementRefDataControl(new GeoReference(targetId)),
                    TypeDescriptors = new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor[]
                    {
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = GEO_REFERENCE,
                            ContentType = typeof(GeoReference),
                            ValidReferenceTypes = new[] { typeof(GeoElement) },
                            ReferencesId = true
                        }
                    }
                })
            }, new ListDataControl<MapSceneDataControl, MapElementDataControl>.ElementFactoryView()
            {
                Titles = {
                    { Controller.ITEM_REFERENCE   , "Operation.AddItemReferenceTitle" },
                    { Controller.ATREZZO_REFERENCE, "Operation.AddAtrezzoReferenceTitle" },
                    { Controller.NPC_REFERENCE    , "Operation.AddNPCReferenceTitle" }
                },
                Messages = {
                    { Controller.ITEM_REFERENCE   , "Operation.AddItemReferenceMessage" },
                    { Controller.ATREZZO_REFERENCE, "Operation.AddAtrezzoReferenceMessage" },
                    { Controller.NPC_REFERENCE    , "Operation.AddNPCReferenceMessage" }
                },
                Errors = {
                    { Controller.ITEM_REFERENCE   , "Operation.AddItemReferenceErrorNoItems" },
                    { Controller.ATREZZO_REFERENCE, "Operation.AddReferenceErrorNoAtrezzo" },
                    { Controller.NPC_REFERENCE    , "Operation.AddReferenceErrorNoNPC" }
                },
                ElementFactory = new DefaultElementFactory<MapElementDataControl>(new DefaultElementFactory<MapElementDataControl>.ElementCreator()
                {
                    CreateDataControl = o => new ExtElementRefDataControl(o as ExtElemReference),
                    CreateElement = (type, targetId) => new ExtElementRefDataControl(new ExtElemReference(targetId)),
                    TypeDescriptors = new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor[]
                    {
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = Controller.ITEM_REFERENCE,
                            ContentType = typeof(ExtElemReference),
                            ValidReferenceTypes = new[] { typeof(Item) },
                            ReferencesId = true
                        },
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = Controller.ATREZZO_REFERENCE,
                            ContentType = typeof(ExtElemReference),
                            ValidReferenceTypes = new[] { typeof(Atrezzo) },
                            ReferencesId = true
                        },
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = Controller.NPC_REFERENCE,
                            ContentType = typeof(ExtElemReference),
                            ValidReferenceTypes = new[] { typeof(NPC) },
                            ReferencesId = true
                        }
                    }
                })
            });

            xApiOptions = new Dictionary<string, List<string>>();

            var accessibleOptions = Enum.GetValues(typeof(AccessibleTracker.Accessible))
                .Cast<AccessibleTracker.Accessible>()
                .Select(v => v.ToString().ToLower())
                .ToList();

            xApiOptions.Add("accesible", accessibleOptions);

            var alternativeOptions = Enum.GetValues(typeof(AlternativeTracker.Alternative))
                .Cast<AlternativeTracker.Alternative>()
                .Select(v => v.ToString().ToLower())
                .ToList();

            xApiOptions.Add("alternative", alternativeOptions);
        }


        public override object getContent()
        {
            return mapScene;
        }

        public override int[] getAddableElements() { return null; }

        public override bool canAddElement(int type) { return false; }

        public override bool canBeDeleted() { return true; }

        public override bool canBeDuplicated() { return true; }

        public override bool canBeMoved() { return true; }

        public override bool canBeRenamed() { return true; }

        public override bool addElement(int type, string id) { return false; }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }

        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override bool moveElementDown(DataControl dataControl) { return false; }

        public override string renameElement(string newName) { return null; }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            mapElementListDataControl.updateVarFlagSummary(varFlagSummary);
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
            return mapElementListDataControl.countIdentifierReferences(id);
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            mapElementListDataControl.replaceIdentifierReferences(oldId, newId);
        }

        public override void deleteIdentifierReferences(string id)
        {
            mapElementListDataControl.deleteAssetReferences(id);
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return getPathFromChild(dataControl, mapElementListDataControl.DataControls.Cast<System.Object>().ToList());
        }


        public override void recursiveSearch()
        {
            this.Elements.recursiveSearch();
            check(this.Documentation, TC.get("Search.Documentation"));
            check(this.Id, "ID");
        }

        public ListDataControl<MapSceneDataControl, MapElementDataControl> Elements
        {
            get { return mapElementListDataControl; }
        }

        public string Id
        {
            get { return mapScene.getId(); }
        }

        public string Documentation
        {
            get { return mapScene.getDocumentation(); }
            set
            {
                if (!string.Equals(value, mapScene.Documentation, StringComparison.InvariantCulture))
                {
                    controller.AddTool(new ChangeDocumentationTool(mapScene, value));
                }
            }
        }

        public CameraType CameraType
        {
            get { return mapScene.CameraType; }
            set
            {
                if(!CameraType.Equals(mapScene.CameraType, value))
                {
                    controller.AddTool(ChangeEnumValueTool.Create(mapScene, value, "CameraType"));
                }
            }
        }

        public List<string> getxAPIValidTypes(string @class)
        {
            return xApiOptions[@class];
        }

        public List<string> getxAPIValidClasses()
        {
            return xApiOptions.Keys.ToList();
        }

        public string getxAPIType()
        {
            return mapScene.getXApiType();
        }

        public string getxAPIClass()
        {
            return mapScene.getXApiClass();
        }

        public void setxAPIType(string type)
        {
            if (!xApiOptions.ContainsKey(getxAPIClass()) || !xApiOptions[getxAPIClass()].Contains(type))
            {
                return;
            }

            controller.AddTool(new ChangeStringValueTool(mapScene, type, "getXApiType", "setXApiType"));
        }

        public void setxAPIClass(string @class)
        {
            if (!xApiOptions.ContainsKey(@class))
            {
                return;
            }

            controller.AddTool(new ChangeStringValueTool(mapScene, @class, "getXApiClass", "setXApiClass"));
        }
    }
}

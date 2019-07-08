using System;
using System.Collections.Generic;
using System.Linq;
using MapzenGo.Helpers;
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

        private readonly GameplayAreaDataControl gameplaAreaDataControl;

        private readonly Dictionary<string, List<string>> xApiOptions;

        public MapSceneDataControl(MapScene mapScene)
        {
            this.mapScene = mapScene;
            this.gameplaAreaDataControl = new GameplayAreaDataControl(this);
            var mapParametersGatherer = new Action<Action<object>>[]
            {
                callback => callback(LatLon),
                callback => callback(Zoom)
            };

            this.mapElementListDataControl = new ListDataControl<MapSceneDataControl, MapElementDataControl>(this, mapScene.Elements, new ListDataControl<MapSceneDataControl, MapElementDataControl>.ElementFactoryView()
            {
                Titles   = {{ GEO_REFERENCE, "Operation.AddGeoReferenceTitle" }},
                Messages = {{ GEO_REFERENCE, "Operation.AddGeoReferenceMessage" }},
                Errors   = {{ GEO_REFERENCE, "Operation.AddGeoReferenceErrorNoItems" }},
                ElementFactory = new DefaultElementFactory<MapElementDataControl>(new DefaultElementFactory<MapElementDataControl>.ElementCreator()
                {
                    CreateDataControl = o => new GeoElementRefDataControl(o as GeoReference),
                    CreateElement = (type, targetId, _) => new GeoElementRefDataControl(new GeoReference(targetId)),
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
                    CreateElement = (type, targetId, extra) =>
                    {
                        var extElementRef = new ExtElemReference(targetId);
                        extElementRef.TransformManagerParameters["Position"] = (Vector2d)extra[0];
                        var zoom = (int) extra[1];
                        var pixelScale = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(1, 1), zoom), 19);
                        extElementRef.TransformManagerParameters["Scale"] = new Vector3((float)pixelScale.x, (float)pixelScale.y, 1);
                        return new ExtElementRefDataControl(extElementRef);
                    },
                    TypeDescriptors = new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor[]
                    {
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = Controller.ITEM_REFERENCE,
                            ContentType = typeof(ExtElemReference),
                            ValidReferenceTypes = new[] { typeof(Item) },
                            ExtraParameters = mapParametersGatherer,
                            ReferencesId = true
                        },
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = Controller.ATREZZO_REFERENCE,
                            ContentType = typeof(ExtElemReference),
                            ValidReferenceTypes = new[] { typeof(Atrezzo) },
                            ExtraParameters = mapParametersGatherer,
                            ReferencesId = true
                        },
                        new DefaultElementFactory<MapElementDataControl>.ElementCreator.TypeDescriptor()
                        {
                            Type = Controller.NPC_REFERENCE,
                            ContentType = typeof(ExtElemReference),
                            ValidReferenceTypes = new[] { typeof(NPC) },
                            ExtraParameters = mapParametersGatherer,
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

        public CameraType CameraType
        {
            get { return mapScene.CameraType; }
            set { controller.AddTool(ChangeEnumValueTool.Create(mapScene, value, "CameraType")); }
        }

        public RenderStyle RenderStyle
        {
            get { return mapScene.RenderStyle; }
            set { controller.AddTool(ChangeEnumValueTool.Create(mapScene, value, "RenderStyle")); }
        }

        public string Name
        {
            get { return mapScene.Name; }
            set { controller.AddTool(new ChangeNameTool(mapScene, value)); }
        }

        public GameplayAreaDataControl GameplayArea
        {
            get { return gameplaAreaDataControl; }
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
            set { controller.AddTool(new ChangeIdTool(mapScene, value)); }
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

        public Vector2d LatLon
        {
            get { return mapScene.LatLon; }
            set { controller.AddTool(new ChangeValueTool<MapScene, Vector2d>(mapScene, value, "LatLon")); }
        }

        public int Zoom
        {
            get { return mapScene.Zoom; }
            set { controller.AddTool(new ChangeValueTool<MapScene, int>(mapScene, value, "Zoom")); }
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

        public void setxAPIClass(string value)
        {
            if (!xApiOptions.ContainsKey(value))
            {
                return;
            }

            controller.AddTool(new ChangeStringValueTool(mapScene, value, "getXApiClass", "setXApiClass"));
        }
    }
}

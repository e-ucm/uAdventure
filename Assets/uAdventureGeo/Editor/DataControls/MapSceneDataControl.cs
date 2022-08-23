using System;
using System.Collections.Generic;
using System.Linq;
using MapzenGo.Helpers;
using uAdventure.Core;
using UnityEngine;

using uAdventure.Editor;
using Xasu.HighLevel;

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
                        extElementRef.Scale = (float)pixelScale.x;
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

            var accessibleOptions = Enum.GetValues(typeof(AccessibleTracker.AccessibleType))
                .Cast<AccessibleTracker.AccessibleType>()
                .Select(v => v.ToString().ToLower())
                .ToList();

            xApiOptions.Add("accesible", accessibleOptions);

            var alternativeOptions = Enum.GetValues(typeof(AlternativeTracker.AlternativeType))
                .Cast<AlternativeTracker.AlternativeType>()
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


        public override string renameElement(string newName)
        {
            string oldMapSceneId = mapScene.getId();
            string references = controller.countIdentifierReferences(oldMapSceneId).ToString();

            // Ask for confirmation 
            if (newName != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameSceneTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldMapSceneId, references })))
            {
                // Show a dialog asking for the new atrezzo item id
                if (newName == null)
                {
                    controller.ShowInputDialog(TC.get("Operation.RenameSceneTitle"), TC.get("Operation.RenameSceneMessage"), oldMapSceneId, (o, s) => performRenameElement(s));
                }
                else
                {
                    return performRenameElement(newName);
                }
            }

            return null;
        }

        private string performRenameElement(string newMapSceneId)
        {
            string oldMapSceneId = mapScene.getId();

            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newMapSceneId))
            {
                newMapSceneId = controller.makeElementValid(newMapSceneId);
            }

            mapScene.setId(newMapSceneId);
            controller.replaceIdentifierReferences(oldMapSceneId, newMapSceneId);
            controller.IdentifierSummary.deleteId<MapScene>(oldMapSceneId);
            controller.IdentifierSummary.addId<MapScene>(newMapSceneId);

            return newMapSceneId;
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            mapElementListDataControl.updateVarFlagSummary(varFlagSummary);
            gameplaAreaDataControl.updateVarFlagSummary(varFlagSummary);
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            var valid = true;
            valid &= mapElementListDataControl.isValid(currentPath, incidences);
            valid &= gameplaAreaDataControl.isValid(currentPath, incidences);
            return valid;
        }

        public override int countAssetReferences(string assetPath)
        {
            var count = 0;
            count &= mapElementListDataControl.countAssetReferences(assetPath);
            count &= gameplaAreaDataControl.countAssetReferences(assetPath);
            return count;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            mapElementListDataControl.getAssetReferences(assetPaths, assetTypes);
            gameplaAreaDataControl.getAssetReferences(assetPaths, assetTypes);
        }

        public override void deleteAssetReferences(string assetPath)
        {
            mapElementListDataControl.deleteAssetReferences(assetPath);
            gameplaAreaDataControl.deleteAssetReferences(assetPath);
        }

        public override int countIdentifierReferences(string id)
        {
            var count = 0;
            count &= mapElementListDataControl.countIdentifierReferences(id);
            count &= gameplaAreaDataControl.countIdentifierReferences(id);
            return count;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            mapElementListDataControl.replaceIdentifierReferences(oldId, newId);
            gameplaAreaDataControl.replaceIdentifierReferences(oldId, newId);
        }

        public override void deleteIdentifierReferences(string id)
        {
            mapElementListDataControl.deleteIdentifierReferences(id);
            gameplaAreaDataControl.deleteIdentifierReferences(id);
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            List<Searchable> path;
            path = getPathFromChild(dataControl, mapElementListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, gameplaAreaDataControl);
            if (path != null)
                return path;

            return null;
        }


        public override void recursiveSearch()
        {
            this.mapElementListDataControl.recursiveSearch();
            this.gameplaAreaDataControl.recursiveSearch();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using uAdventure.Editor;
using System;
using System.Linq;
using uAdventure.Core;

using TD = uAdventure.Editor.DefaultElementFactory<uAdventure.Geo.GeoActionDataControl>.ElementCreator.TypeDescriptor;

namespace uAdventure.Geo
{
    public class GeoElementDataControl : DataControlWithResources
    {
        private readonly GeoElement geoElement;
        private readonly DescriptionsController descriptionController;
        private readonly ListDataControl<GeoElementDataControl, GeoActionDataControl> geoActionDataControls;
        private readonly ListDataControl<GeoElementDataControl, GMLGeometryDataControl> geometryDataControls;
        private int selectedGeometry;

        public const int GEO_ELEMENT    = 62345459;
        public const int ENTER_ACTION   = 87234678;
        public const int EXIT_ACTION    = 78234568;
        public const int LOOK_TO_ACTION = 23848923;
        public const int INSPECT_ACTION = 67213469;
        public const int GEOMETRY       = 24312342;

        public DescriptionsController DescriptionController
        {
            get { return descriptionController; }
        }

        public ListDataControl<GeoElementDataControl, GeoActionDataControl> GeoActions
        {
            get { return geoActionDataControls; }
        }

        public GeoElementDataControl(GeoElement geoElement)
        {
            this.geoElement = geoElement;

            this.resourcesList = geoElement.Resources;

            selectedResources = 0;

            // Add a new resource if the list is empty
            if (resourcesList.Count == 0)
            {
                resourcesList.Add(new ResourcesUni());
            }

            // Create the subcontrollers
            resourcesDataControlList = new List<ResourcesDataControl>();
            foreach (ResourcesUni resources in resourcesList)
            {
                resourcesDataControlList.Add(new ResourcesDataControl(resources, GEO_ELEMENT));
            }

            descriptionController = new DescriptionsController(geoElement.Descriptions);

            geoActionDataControls = new ListDataControl<GeoElementDataControl, GeoActionDataControl>(this, geoElement.Actions, new[]
            {
                new ListDataControl<GeoElementDataControl, GeoActionDataControl>.ElementFactoryView
                {
                    Titles =
                    {
                        { ENTER_ACTION   , "Geo.Create.Title.EnterAction"},
                        { EXIT_ACTION    , "Geo.Create.Title.ExitAction"},
                        { LOOK_TO_ACTION , "Geo.Create.Title.LookToAction"},
                        { INSPECT_ACTION , "Geo.Create.Title.InspectAction"}
                    },
                    Messages =
                    {
                        { ENTER_ACTION   , "Geo.Create.Message.EnterAction"},
                        { EXIT_ACTION    , "Geo.Create.Message.ExitAction"},
                        { LOOK_TO_ACTION , "Geo.Create.Message.LookToAction"},
                        { INSPECT_ACTION , "Geo.Create.Message.InspectAction"}
                    },
                    Errors =
                    {
                        { ENTER_ACTION   , "Geo.Create.Error.EnterAction"},
                        { EXIT_ACTION    , "Geo.Create.Error.ExitAction"},
                        { LOOK_TO_ACTION , "Geo.Create.Error.LookToAction"},
                        { INSPECT_ACTION , "Geo.Create.Error.InspectAction"}
                    },
                    ElementFactory = new DefaultElementFactory<GeoActionDataControl>(new DefaultElementFactory<GeoActionDataControl>.ElementCreator()
                    {
                        TypeDescriptors = new []
                        {
                            new TD
                            {
                                Type = ENTER_ACTION,
                                ContentType = typeof(EnterAction)
                            },
                            new TD
                            {
                                Type = EXIT_ACTION,
                                ContentType = typeof(ExitAction)
                            },
                            new TD
                            {
                                Type = LOOK_TO_ACTION,
                                ContentType = typeof(InspectAction)
                            },
                            new TD
                            {
                                Type = INSPECT_ACTION,
                                ContentType = typeof(LookToAction)
                            }
                        },
                        CreateDataControl = action => new GeoActionDataControl(action as GeoAction),
                        CreateElement = (type, id, _) =>
                        {
                            switch (type)
                            {
                                case ENTER_ACTION:   return new GeoActionDataControl(new EnterAction());
                                case EXIT_ACTION:    return new GeoActionDataControl(new ExitAction());
                                case LOOK_TO_ACTION: return new GeoActionDataControl(new LookToAction());
                                case INSPECT_ACTION: return new GeoActionDataControl(new InspectAction());
                                default: return null;
                            }
                        }
                    })
                }
            });

            geometryDataControls = new ListDataControl<GeoElementDataControl, GMLGeometryDataControl>(this, geoElement.Geometries, new[]
            {
                new ListDataControl<GeoElementDataControl, GMLGeometryDataControl>.ElementFactoryView()
                {
                    Titles      = { { GEOMETRY , "Geo.Create.Title.Geometry" } },
                    Messages    = { { GEOMETRY , "Geo.Create.Message.Geometry" } },
                    Errors      = { { GEOMETRY , "Geo.Create.Error.Geometry" } },
                    ElementFactory = new DefaultElementFactory<GMLGeometryDataControl>(new DefaultElementFactory<GMLGeometryDataControl>.ElementCreator
                    {
                        CreateDataControl = geometry => new GMLGeometryDataControl(geometry as GMLGeometry),
                        CreateElement = (type, id, _) => new GMLGeometryDataControl(new GMLGeometry()),
                        TypeDescriptors = new []
                        {
                            new DefaultElementFactory<GMLGeometryDataControl>.ElementCreator.TypeDescriptor
                            {
                                ContentType = typeof(GMLGeometry),
                                Type = GEOMETRY
                            }
                        }
                    })
                }
            })
            {
                CanDeleteLastElement = false
            };
        }

        public ListDataControl<GeoElementDataControl, GMLGeometryDataControl> GMLGeometries
        {
            get { return geometryDataControls; }
        }

        public int SelectedGeometry
        {
            get { return selectedGeometry; }
            set { this.selectedGeometry = value; }
        }

        public string Documentation
        {
            get
            {
                return geoElement.getDocumentation();
            }

            set
            {
                controller.AddTool(new ChangeDocumentationTool(geoElement, value));
            }
        }

        public override bool addElement(int type, string id) { return false; }

        public override bool canAddElement(int type) { return false;}

        public override bool canBeDeleted() { return true; }

        public override bool canBeDuplicated() { return true; }

        public override bool canBeMoved() { return true; }

        public override bool canBeRenamed() { return true; }

        public override int countAssetReferences(string assetPath)
        {
            var count = 0;
            count += descriptionController.countAssetReferences(assetPath);
            count += geoActionDataControls.countAssetReferences(assetPath);
            count += geometryDataControls.countAssetReferences(assetPath);
            count += resourcesDataControlList.Sum(r => r.countAssetReferences(assetPath));
            return count;
        }

        public override int countIdentifierReferences(string id)
        {
            var count = 0;
            count += descriptionController.countIdentifierReferences(id);
            count += geoActionDataControls.countIdentifierReferences(id);
            count += geometryDataControls.countIdentifierReferences(id);
            count += resourcesDataControlList.Sum(r => r.countIdentifierReferences(id));
            return count;
        }

        public override void deleteAssetReferences(string assetPath)
        {
            descriptionController.deleteAssetReferences(assetPath);
            geoActionDataControls.deleteAssetReferences(assetPath);
            geometryDataControls.deleteAssetReferences(assetPath);
            resourcesDataControlList.ForEach(r => r.deleteAssetReferences(assetPath));
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }

        public override void deleteIdentifierReferences(string id)
        {
            descriptionController.deleteIdentifierReferences(id);
            geoActionDataControls.deleteIdentifierReferences(id);
            geometryDataControls.deleteIdentifierReferences(id);
            resourcesDataControlList.ForEach(r => r.deleteIdentifierReferences(id));
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            descriptionController.getAssetReferences(assetPaths, assetTypes);
            geoActionDataControls.getAssetReferences(assetPaths, assetTypes);
            geometryDataControls.getAssetReferences(assetPaths, assetTypes);
            resourcesDataControlList.ForEach(r => r.getAssetReferences(assetPaths, assetTypes));
        }

        public override object getContent()
        {
            return geoElement;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            List<Searchable> path;
            path = getPathFromChild(dataControl, descriptionController);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, geoActionDataControls);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, geometryDataControls);
            if (path != null)
                return path;
            foreach (var r in resourcesDataControlList)
            {
                path = getPathFromChild(dataControl, r);
                if (path != null)
                    return path;
            }
            return null;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            var valid = true;
            valid &= descriptionController.isValid(currentPath, incidences);
            valid &= geoActionDataControls.isValid(currentPath, incidences);
            valid &= geometryDataControls.isValid(currentPath, incidences);
            valid &= resourcesDataControlList.All(r => r.isValid(currentPath, incidences));
            return valid;
        }

        public override bool moveElementDown(DataControl dataControl) { return false; }

        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override void recursiveSearch()
        {
            descriptionController.recursiveSearch();
            geoActionDataControls.recursiveSearch();
            geometryDataControls.recursiveSearch();
            resourcesDataControlList.ForEach(r => r.recursiveSearch());
            check(geoElement.getDocumentation(), TC.get("Search.Documentation"));
            check(geoElement.getName(), TC.get("Search.Name"));
            check(geoElement.getId(), "ID");
        }


        public override string renameElement(string newName)
        {
            string oldGeoElementId = geoElement.getId();
            string references = controller.countIdentifierReferences(oldGeoElementId).ToString();

            // Ask for confirmation 
            if (newName != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameElementTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldGeoElementId, references })))
            {
                // Show a dialog asking for the new atrezzo item id
                if (newName == null)
                {
                    controller.ShowInputDialog(TC.get("Operation.RenameElementTitle"), TC.get("Operation.RenameElementMessage"), oldGeoElementId, (o, s) => performRenameElement(s));
                }
                else
                {
                    return performRenameElement(newName);
                }
            }

            return null;
        }

        private string performRenameElement(string newGeoElementId)
        {
            string oldGeoElementId = geoElement.getId();

            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newGeoElementId))
            {
                newGeoElementId = controller.makeElementValid(newGeoElementId);
            }

            geoElement.setId(newGeoElementId);
            controller.replaceIdentifierReferences(oldGeoElementId, newGeoElementId);
            controller.IdentifierSummary.deleteId<GeoElement>(oldGeoElementId);
            controller.IdentifierSummary.addId<GeoElement>(newGeoElementId);

            return newGeoElementId;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            descriptionController.replaceIdentifierReferences(oldId, newId);
            geoActionDataControls.replaceIdentifierReferences(oldId, newId);
            geometryDataControls.replaceIdentifierReferences(oldId, newId);
            resourcesDataControlList.ForEach(r => r.replaceIdentifierReferences(oldId, newId));
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            descriptionController.updateVarFlagSummary(varFlagSummary);
            geoActionDataControls.updateVarFlagSummary(varFlagSummary);
            geometryDataControls.updateVarFlagSummary(varFlagSummary);
            resourcesDataControlList.ForEach(r => r.updateVarFlagSummary(varFlagSummary));
        }
    }
}

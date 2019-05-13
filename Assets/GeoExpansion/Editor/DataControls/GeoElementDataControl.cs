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

        private const int GEO_ELEMENT    = 62345459;
        private const int ENTER_ACTION   = 87234678;
        private const int EXIT_ACTION    = 78234568;
        private const int LOOK_TO_ACTION = 23848923;
        private const int INSPECT_ACTION = 67213469;

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

            geoActionDataControls = new ListDataControl<GeoElementDataControl, GeoActionDataControl>(this, geoElement.Actions, new []
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
                        CreateElement = (type, id) =>
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
        }

        public override bool addElement(int type, string id) { return false; }

        public override bool canAddElement(int type) { return false;}

        public override bool canBeDeleted() { return true; }

        public override bool canBeDuplicated() { return true; }

        public override bool canBeMoved() { return true; }

        public override bool canBeRenamed() { return true; }

        public override int countAssetReferences(string assetPath)
        {
            return geoActionDataControls.countAssetReferences(assetPath);
        }

        public override int countIdentifierReferences(string id)
        {
            return geoActionDataControls.countIdentifierReferences(id);
        }

        public override void deleteAssetReferences(string assetPath)
        {
            geoActionDataControls.deleteAssetReferences(assetPath);
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation) { return false; }

        public override void deleteIdentifierReferences(string id)
        {
            geoActionDataControls.deleteIdentifierReferences(id);
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            geoActionDataControls.getAssetReferences(assetPaths, assetTypes);
        }

        public override object getContent()
        {
            return geoElement;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return getPathFromChild(dataControl, geoActionDataControls.DataControls.Cast<object>().ToList());
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            var valid = descriptionController.isValid(currentPath, incidences);
            resourcesDataControlList.ForEach(r => valid &= r.isValid(currentPath, incidences));
            valid &= geoActionDataControls.isValid(currentPath, incidences);
            valid &= geoElement.Geometries.Count > 0 && geoElement.Geometries.All(g => g.Points.Count > 0);
            return valid;
        }

        public override bool moveElementDown(DataControl dataControl) { return false; }

        public override bool moveElementUp(DataControl dataControl) { return false; }

        public override void recursiveSearch()
        {
            descriptionController.recursiveSearch();
            resourcesDataControlList.ForEach(r => r.recursiveSearch());
            geoActionDataControls.recursiveSearch();
            check(geoElement.getDocumentation(), TC.get("Search.Documentation"));
            check(geoElement.getName(), TC.get("Search.Name"));
            check(geoElement.getId(), "ID");
        }

        public override string renameElement(string newName)
        {
            if (!controller.isElementIdValid(newName))
            {
                newName = controller.makeElementValid(newName);
            }

            return controller.AddTool(new ChangeIdTool(geoElement, newName)) ? newName : geoElement.getId();
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            geoActionDataControls.replaceIdentifierReferences(oldId, newId);
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            geoActionDataControls.updateVarFlagSummary(varFlagSummary);
        }
    }
}

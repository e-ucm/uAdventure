using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using System;

namespace uAdventure.Geo
{

    public class GeoElement : Documented, Named, HasId, ICloneable
    {

        private string image;
        private string documentation;
        private GeoElementDrawer drawer;
        private List<Description> descriptions;
        private List<GeoAction> geoActions;
        private List<GMLGeometry> geometries;
        private List<ResourcesUni> resources;

        public GeoElement(string id)
        {
            Id = id;
            resources = new List<ResourcesUni> { new ResourcesUni() };
            descriptions = new List<Description> { new Description() };
            geometries = new List<GMLGeometry> { new GMLGeometry { Influence = 0 }};
            geoActions = new List<GeoAction>();
        }

        public string Id { get; set; }
        public List<Description> Descriptions { get { return descriptions; } set { descriptions = value; } }
        public List<GMLGeometry> Geometries { get { return geometries; } set { geometries = value; } }
        public GeoElementDrawer Drawer { get { return drawer; } set { drawer = value; } }
        public string Image { get { return image; } set { image = value; } }
        public List<GeoAction> Actions { get { return geoActions; } set { geoActions = value; } }
        public List<ResourcesUni> Resources { get { return resources; } set { resources = value; } }

        public string getDocumentation()
        {
            return documentation;
        }

        public string getId()
        {
            return Id;
        }

        public void setId(string id)
        {
            Id = id;
        }

        public void setDocumentation(string documentation)
        {
            this.documentation = documentation;
        }

        public void setName(string name)
        {
            descriptions[0].setName(name);
        }

        public string getName()
        {
            return descriptions[0].getName();
        }

        public object Clone()
        {
            var clone = this.MemberwiseClone();
            Resources = Resources.ConvertAll(r => r.Clone() as ResourcesUni);
            geoActions = geoActions.ConvertAll(a => a.Clone() as GeoAction);
            geometries = geometries.ConvertAll(g => g.Clone() as GMLGeometry);
            descriptions = descriptions.ConvertAll(g => g.Clone() as Description);
            return clone;
        }
    }

    public interface GeoElementDrawer
    {
        void Init(GeoElement element);
        void Update();
    }

}
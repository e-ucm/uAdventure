using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Geo
{

    public class GeoElement
    {

        public GeoElement(string id)
        {
            Id = id;
            Geometry = new GMLGeometry();
            Name = "";
            DetailedDescription = "";
            FullDescription = "";
            BriefDescription = "";
            Influence = 0f;
            Image = "";
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string DetailedDescription { get; set; }
        public string FullDescription { get; set; }
        public string BriefDescription { get; set; }
        public GMLGeometry Geometry { get; set; }

        public GeoElementDrawer Drawer { get; set; }
        public float Influence { get; set; }
        public string Image { get; set; }

        public List<GeoAction> Actions { get; set; }
    }

    public interface GeoElementDrawer
    {
        void Init(GeoElement element);
        void Update();
    }

}
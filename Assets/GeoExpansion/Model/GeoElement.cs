using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using System;

namespace uAdventure.Geo
{

    public class GeoElement : HasId
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
            Actions = new List<GeoAction>();
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

        public string getId()
        {
            return Id;
        }

        public void setId(string id)
        {
            Id = id;
        }
    }

    public interface GeoElementDrawer
    {
        void Init(GeoElement element);
        void Update();
    }

}
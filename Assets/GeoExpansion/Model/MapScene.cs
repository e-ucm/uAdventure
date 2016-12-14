using UnityEngine;
using System.Collections.Generic;


namespace uAdventure.Geo
{
    public enum CameraType
    {
        Aerial2D, Ortographic3D, Perspective3D
    };

    public class MapScene
    {
        public CameraType CameraType { get; set; }

        List<MapElement> mapElements;

        public MapScene(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public void setId(string id)
        {
            Id = id;
        }

        public string getId()
        {
            return Id;
        }

        public override string ToString()
        {
            return Id;
        }
    }

}


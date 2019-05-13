using UnityEngine;
using System.Collections.Generic;
using uAdventure.Core;
using System;

namespace uAdventure.Geo
{
    public enum CameraType
    {
        Aerial2D, Ortographic3D, Perspective3D
    };

    public class MapScene : IChapterTarget, Documented, HasId
    {
        /**
         * xApi Class
         */
        protected string xapiClass = "accesible";

        /**
         * xApi Class type
         */
        protected string xapiType = "area";

        //-------------
        // Properties
        //-------------
        public CameraType CameraType { get; set; }
        public string Id { get; set; }
        public string Documentation { get; set; }
        public List<MapElement> Elements { get; set; }
        public Vector2d LatLon { get; set; }

        /// <summary>
        /// Creates a mapScene using the id. Initializing its elements to an empty list.
        /// </summary>
        /// <param name="id">Id to use</param>
        public MapScene(string id)
        {
            Id = id;
            Elements = new List<MapElement>();
        }

        /// <summary>
        /// ToString writes down the ID
        /// </summary>
        /// <returns>string with the id</returns>
        public override string ToString()
        {
            return Id;
        }

        public string getId()
        {
            return Id;
        }

        public void setId(string id)
        {
            this.Id = id;
        }

        public string getXApiClass()
        {
            return xapiClass;
        }

        public string getXApiType()
        {
            return xapiType;
        }

        public void setDocumentation(string documentation)
        {
            Documentation = documentation;
        }

        public string getDocumentation()
        {
            return Documentation;
        }
    }

}


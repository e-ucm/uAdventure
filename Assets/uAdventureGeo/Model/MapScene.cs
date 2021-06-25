using UnityEngine;
using System.Collections.Generic;
using uAdventure.Core;

namespace uAdventure.Geo
{
    public enum CameraType
    {
        Aerial2D, Ortographic3D, Perspective3D
    };

    public enum RenderStyle
    {
        Tile,
        Vector,
        Hybrid
    };

    public class MapScene : IChapterTarget, Named, Documented, HasId
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
        public string Name { get; set; }
        public string Documentation { get; set; }
        public List<MapElement> Elements { get; set; }
        public Vector2d LatLon { get; set; }
        public int Zoom { get; set; }
        public string TileMetaIdentifier { get; set; }
        public RenderStyle RenderStyle { get; set; }
        public bool UsesGameplayArea { get; set; }
        public RectD GameplayArea { get; set; }

        /// <summary>
        /// Creates a mapScene using the id. Initializing its elements to an empty list.
        /// </summary>
        /// <param name="id">Id to use</param>
        public MapScene(string id)
        {
            Id = id;
            Elements = new List<MapElement>();
            GameplayArea = new RectD(Vector2d.zero, Vector2d.zero);
            TileMetaIdentifier = "OSMTile";
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

        public void setName(string name)
        {
            Name = name;
        }

        public string getName()
        {
            return Name;
        }

        public bool allowsSavingGame()
        {
            return true;
        }
    }

}


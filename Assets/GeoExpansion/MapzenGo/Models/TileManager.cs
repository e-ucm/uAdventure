using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using MapzenGo.Helpers;
using MapzenGo.Models.Factories;
using MapzenGo.Models.Plugins;
using UniRx;
using UnityEngine;

namespace MapzenGo.Models
{
    public class TileManager : MonoBehaviour
    {

        [SerializeField]
        public float Latitude = 39.921864f;
        [SerializeField]
        public float Longitude = 32.818442f;
        [SerializeField]
        public int Range = 3;
        [SerializeField]
        public int Zoom = 16;
        [SerializeField]
        public float TileSize = 100;

        [SerializeField]
        protected Material MapMaterial;

        [SerializeField]
        private Rect _centerCollider;
        [SerializeField]
        private Transform _player;
        [SerializeField]
        private int _removeAfter;
        [SerializeField]
        private bool _keepCentralized;

        protected Transform TileHost;

        protected Dictionary<Vector2d, Tile> Tiles; //will use this later on
        protected Vector2d CenterTms; //tms tile coordinate
        protected Vector2d CenterInMercator; //this is like distance (meters) in mercator 

        private List<Plugin> _plugins;

        private void InitPlugins()
        {
            _plugins = new List<Plugin>();
            foreach (var plugin in GetComponentsInChildren<Plugin>())
            {
                _plugins.Add(plugin);
            }
        }

        public virtual void Start()
        {
            InitPlugins();
            
            _removeAfter = Math.Max(_removeAfter, Range * 2 + 1);
            var centerrect = new Vector2(TileSize, TileSize);
            _centerCollider = new Rect(Vector2.zero - centerrect / 2, centerrect);

            var v2 = GM.LatLonToMeters(Latitude, Longitude);
            var tile = GM.MetersToTile(v2, Zoom);

            TileHost = new GameObject("Tiles").transform;
            TileHost.SetParent(transform, false);

            Tiles = new Dictionary<Vector2d, Tile>();
            CenterTms = tile;
            CenterInMercator = GM.TileBounds(CenterTms, Zoom).Center;

            LoadTiles(CenterTms, CenterInMercator);

            var rect = GM.TileBounds(CenterTms, Zoom);
            transform.localScale = Vector3.one * (float)(TileSize / rect.Width);
            if (MapMaterial == null)
                MapMaterial = Resources.Load<Material>("Ground");
        }

        public virtual void Update()
        {
            UpdateTiles();
        }

        private void UpdateTiles()
        {
            if (!_centerCollider.Contains(_player.transform.position.ToVector2xz(), true))
            {
                //player movement in TMS tiles
                var tileDif = GetMovementVector();
                //Debug.Log(tileDif);
                //move locals
                Centralize(tileDif);
                //create new tiles
                LoadTiles(CenterTms, CenterInMercator);
                UnloadTiles(CenterTms);
            }
        }


        protected void LoadTiles(Vector2d tms, Vector2d center)
        {
            for (int i = -Range; i <= Range; i++)
            {
                for (int j = -Range; j <= Range; j++)
                {
                    var v = new Vector2d(tms.x + i, tms.y + j);
                    if (Tiles.ContainsKey(v))
                        continue;
                    StartCoroutine(CreateTile(v, center));
                }
            }
        }

        protected virtual IEnumerator CreateTile(Vector2d tileTms, Vector2d centerInMercator)
        {
            var rect = GM.TileBounds(tileTms, Zoom);
            var tile = new GameObject("tile " + tileTms.x + "-" + tileTms.y).AddComponent<Tile>();

            tile.Zoom = Zoom;
            tile.TileTms = tileTms;
            tile.TileCenter = rect.Center;
            tile.Material = MapMaterial;
            tile.Rect = GM.TileBounds(tileTms, Zoom);

            Tiles.Add(tileTms, tile);
            tile.transform.position = (rect.Center - centerInMercator).ToVector3();
            tile.transform.SetParent(TileHost, false);
            ExecutePlugins((plugin, onFinish) => plugin.Create(tile, onFinish));
            
            yield return null;
        }

        private void Centralize(Vector2 tileDif)
        {
            //move everything to keep current tile at 0,0
            CenterTms += tileDif.ToVector2d();
            if (_keepCentralized)
            {
                foreach (var tile in Tiles.Values)
                {
                    tile.transform.position -= new Vector3((float)(tileDif.x * TileSize), 0, (float)(-tileDif.y * TileSize));
                }

                CenterInMercator = GM.TileBounds(CenterTms, Zoom).Center;
                var difInUnity = new Vector3((float)(tileDif.x * TileSize), 0, (float)(-tileDif.y * TileSize));
                _player.position -= difInUnity;
                Camera.main.transform.position -= difInUnity;
            }
            else
            {
                var difInUnity = new Vector2(tileDif.x * TileSize, -tileDif.y * TileSize);
                _centerCollider.position += difInUnity;
            }
        }


        private void UnloadTiles(Vector2d currentTms)
        {
            var rem = new List<Vector2d>();
            foreach (var key in Tiles.Keys.Where(x => x.ManhattanTo(currentTms) > _removeAfter))
            {
                rem.Add(key);
                ExecutePlugins((plugin, onFinish) => plugin.UnLoad(Tiles[key], onFinish));
                Destroy(Tiles[key].gameObject);
            }
            foreach (var v in rem)
            {
                Tiles.Remove(v);
            }
        }

        protected void ExecutePlugins(Action<Plugin, Action<Plugin, bool>> whatToDo)
        {
            List<Plugin> todo = new List<Plugin>(_plugins);
            List<Plugin> doing = new List<Plugin>();

            ContinuePlugins(todo, doing, whatToDo);
        }

        private void ContinuePlugins(List<Plugin> todo, List<Plugin> doing, Action<Plugin, Action<Plugin, bool>> whatToDo)
        {
            var pluginsToStart = new List<Plugin>();
            foreach (var plugin in todo)
            {
                if (doing.Contains(plugin)) continue;
                // Check dependencies
                if (plugin.Dependencies.All(dependencie => !todo.Contains(dependencie)))
                {
                    pluginsToStart.Add(plugin);
                }
            }

            foreach(var toStart in pluginsToStart)
            {
                doing.Add(toStart);
                whatToDo(toStart, (pluginDone, success) =>
                {
                    todo.Remove(pluginDone);
                    doing.Remove(pluginDone);

                    if (success)
                        ContinuePlugins(todo, doing, whatToDo);
                });
            }
        }

        private Vector2 GetMovementVector()
        {
            var dif = _player.transform.position.ToVector2xz();
            var tileDif = Vector2.zero;
            if (dif.x < Math.Min(_centerCollider.xMin, _centerCollider.xMax))
                tileDif.x = -1;
            else if (dif.x > Math.Max(_centerCollider.xMin, _centerCollider.xMax))
                tileDif.x = 1;

            if (dif.y < Math.Min(_centerCollider.yMin, _centerCollider.yMax))
                tileDif.y = 1;
            else if (dif.y > Math.Max(_centerCollider.yMin, _centerCollider.yMax))
                tileDif.y = -1; //invert axis  TMS vs unity
            return tileDif;
        }
    }
}

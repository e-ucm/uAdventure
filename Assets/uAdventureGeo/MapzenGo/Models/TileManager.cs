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
        public int Range = 4;
        [SerializeField]
        public int Zoom = 16;
        [SerializeField]
        public float TileSize = 100;
        [SerializeField]
        public bool RealisticTileSize = true;

        [SerializeField]
        protected Material MapMaterial;

        [SerializeField]
        private Rect _centerCollider;
        [SerializeField]
        private Transform _player;
        [SerializeField]
        private Transform _ground;
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
            if (!RealisticTileSize)
            {
                var centerrect = new Vector2(TileSize, TileSize);
                centerrect.Scale(transform.localScale.ToVector2xz());
                _centerCollider = new Rect((Vector2.zero - centerrect / 2), centerrect);
            }
            else
            {
                var realisticTileSize = GM.TileBounds(Vector2d.zero, Zoom);
                var centerrect = realisticTileSize.Size.ToVector2();
                centerrect = new Vector2(Mathf.Abs(centerrect.x), Mathf.Abs(centerrect.y));
                _centerCollider = new Rect((Vector2.zero - centerrect / 2),centerrect);
            }

            var v2 = GM.LatLonToMeters(Latitude, Longitude);
            var tile = GM.MetersToTile(v2, Zoom);

            TileHost = new GameObject("Tiles").transform;
            TileHost.SetParent(transform, false);

            Tiles = new Dictionary<Vector2d, Tile>();
            CenterTms = tile;
            var centerold = GM.TileBounds(CenterTms, Zoom).Center; 
            CenterInMercator = GM.LatLonToMeters(Latitude, Longitude);//

            LoadTiles(CenterTms, CenterInMercator);

            var rect = GM.TileBounds(CenterTms, Zoom);
            if(!RealisticTileSize)
                transform.localScale = Vector3.one * (float)(TileSize / rect.Width);
            if (MapMaterial == null)
                MapMaterial = Resources.Load<Material>("Ground");
        }

        public virtual void ReloadPlugins<T>() where T : Plugin
        {
            foreach(var t in Tiles)
            {
                ExecutePlugins<T>((plugin, onFinish) => plugin.UnLoad(t.Value, onFinish));
            }

            foreach (var t in Tiles)
            {
                ExecutePlugins<T>((plugin, onFinish) => plugin.Create(t.Value, onFinish));
            }
        }

        public virtual void Update()
        {
            UpdateTiles();
            // Move the ground
            _ground.transform.position = _player.transform.position;

            var visualGround = _ground.transform.GetChild(0);
            visualGround.position = new Vector3(_ground.transform.position.x, transform.position.y - .5f, _ground.transform.position.z);
        }

        private void UpdateTiles()
        {
            if (!_centerCollider.Contains(_player.transform.localPosition.ToVector2xz(), true))
            {
                //player movement in TMS tiles
                var tileDif = GetMovementVector();
                //Debug.Log(tileDif);
                //move locals
                Centralize(tileDif);
                //create new tiles
                UnloadTiles(CenterTms);
                LoadTiles(CenterTms, CenterInMercator);
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
            tile.Rect = rect;

            Tiles.Add(tileTms, tile);
            tile.transform.localPosition = (rect.Center - centerInMercator).ToVector3();
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
                if (RealisticTileSize)
                {
                    foreach (var tile in Tiles.Values)
                    {
                        tile.transform.localPosition -= new Vector3((float)(tileDif.x * _centerCollider.width), 0, (float)(-tileDif.y * _centerCollider.height));
                    }
                }
                else
                {
                    foreach (var tile in Tiles.Values)
                    {
                        tile.transform.localPosition -= new Vector3((float)(tileDif.x * TileSize), 0, (float)(-tileDif.y * TileSize));
                    }
                }                

                CenterInMercator = GM.TileBounds(CenterTms, Zoom).Center;
                Vector3 difInUnity;
                if (RealisticTileSize)
                {
                    difInUnity = new Vector3(tileDif.x * _centerCollider.width, 0, -tileDif.y * _centerCollider.height);
                }
                else
                {
                    difInUnity = new Vector3((float)(tileDif.x * TileSize), 0, (float)(-tileDif.y * TileSize));
                }

                var realisticTileSize = GM.TileBounds(Vector2d.zero, Zoom);
                var newLatLon = GM.MetersToLatLon(GM.LatLonToMeters(
                    new Vector2d(Latitude, Longitude)) + new Vector2d(realisticTileSize.Size.x * tileDif.x, realisticTileSize.Size.y * tileDif.y));
                Latitude = (float) newLatLon.y;
                Longitude = (float) newLatLon.x;

                _player.localPosition -= difInUnity;
                if(RealisticTileSize)
                    difInUnity.Scale(new Vector3(1/transform.lossyScale.x, 1/transform.lossyScale.y, 1/transform.lossyScale.z));
                Camera.main.transform.position -= difInUnity;
            }
            else
            {
                if (RealisticTileSize)
                {
                    var difInUnity = new Vector2(tileDif.x * _centerCollider.width, -tileDif.y * _centerCollider.height);
                    _centerCollider.position += difInUnity;
                }
                else
                {
                    var difInUnity = new Vector2(tileDif.x * TileSize, -tileDif.y * TileSize);
                    _centerCollider.position += difInUnity;
                }
            }
        }


        private void UnloadTiles(Vector2d currentTms)
        {
            var rem = new List<Vector2d>();
            foreach (var key in Tiles.Keys.Where(x => x.ManhattanTo(currentTms) > _removeAfter))
            {
                rem.Add(key);
                ExecutePlugins((plugin, onFinish) => plugin.UnLoad(Tiles[key], onFinish));
                DestroyImmediate(Tiles[key].gameObject);
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

        protected void ExecutePlugins<T>(Action<T, Action<Plugin, bool>> whatToDo) where T : Plugin
        {
            List<Plugin> todo = _plugins.FindAll(p => p is T);
            List<Plugin> doing = new List<Plugin>();

            ContinuePlugins<T>(todo, doing, whatToDo);
        }


        private void ContinuePlugins<T>(List<Plugin> todo, List<Plugin> doing, Action<T, Action<Plugin, bool>> whatToDo) where T : Plugin
        {
            var pluginsToStart = new List<T>();
            foreach (var plugin in todo)
            {
                if (doing.Contains(plugin)) continue;
                // Check dependencies
                if (plugin.Dependencies.All(dependencie => !todo.Contains(dependencie)))
                {
                    pluginsToStart.Add(plugin as T);
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
            var dif = _player.transform.localPosition.ToVector2xz();
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

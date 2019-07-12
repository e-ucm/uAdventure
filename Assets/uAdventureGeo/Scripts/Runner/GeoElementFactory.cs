using UnityEngine;
using System.Collections;
using MapzenGo.Models.Factories;
using MapzenGo.Models;
using System.Collections.Generic;

using uAdventure.Runner;

using ClipperLib;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    public class GeoElementFactory : MapElementFactory
    {
        private Dictionary<GeoReference, GeoElement> cache;
        public GameObject geoElementPrefab;

        private Clipper clipper;

        public override void Awake()
        {
            base.Awake();
            cache = new Dictionary<GeoReference, GeoElement>();
            clipper = new Clipper();
            _createdCache = new Dictionary<Tile, List<GeoElementMB>>();
            Query = (elem, tile) => elem is GeoReference;// && Intersects(tile, elem as GeoReference);
        }

        private GeoElement FindGeoElement(GeoReference reference)
        {
            if (!cache.ContainsKey(reference))
            {
                var elem = Game.Instance.GameState.FindElement<GeoElement>(reference.getTargetId());
                cache.Add(reference, elem);
            }

            return cache.ContainsKey(reference) ? cache[reference] : null;
        }

        private Dictionary<Tile, List<GeoElementMB>> _createdCache;

        protected override IEnumerable<MonoBehaviour> Create(Tile tile, MapElement mapElement)
        {
            var geoRef = mapElement as GeoReference;
            var geoEle = FindGeoElement(geoRef);

            // The geometry is inside of the ti
            var geoElementMB = Instantiate(geoElementPrefab).GetComponent<GeoElementMB>();
            geoElementMB.Reference = geoRef;    
            geoElementMB.Element = geoEle;
            geoElementMB.Tile = tile;

            if (!_createdCache.ContainsKey(tile))
                _createdCache.Add(tile, new List<GeoElementMB>());

            _createdCache[tile].Add(geoElementMB);

            yield return geoElementMB;
                
        }

        protected override IEnumerable<MapElement> Destroy(Tile tile, MapElement mapElement)
        {
            if (_createdCache.ContainsKey(tile))
            {
                var geoElementMB = _createdCache[tile].Find(mb => mb.Element.Id == mapElement.getTargetId());
                if (geoElementMB != null)
                {
                    DestroyImmediate(geoElementMB.gameObject);
                    yield return mapElement;
                }
            }

            yield return null;
        }

        private List<IntPoint> AdaptToClipper(List<Vector2d> geoPoints)
        {
            return geoPoints.ConvertAll(gp => GM.MetersToPixels(GM.LatLonToMeters(gp), 19)).ConvertAll(p => new IntPoint(p.x, p.y));
        }

        /*private bool Intersects(Tile tile, GeoReference elem)
        {
            var element = FindGeoElement(elem);

            // do the clipping
            var tb = GM.TileBounds(tile.TileTms, tile.Zoom);
            var tilePoints = AdaptToClipper(new List<Vector2d> {
                    tb.Min,
                    tb.Min + new Vector2d(tb.Width, 0),
                    tb.Min + new Vector2d(tb.Width, tb.Height),
                    tb.Min + new Vector2d(0, tb.Height)
                }.ConvertAll(p => GM.MetersToLatLon(p)));
            var geoPoints = AdaptToClipper(element.Geometry.Points);

            var solution = new List<List<IntPoint>>();

            clipper.Clear();
            clipper.AddPath(tilePoints, PolyType.ptSubject, true);
            clipper.AddPath(geoPoints, PolyType.ptSubject, true);

            return clipper.Execute(ClipType.ctIntersection, solution) && solution.Count > 0;
        }*/
    }

}

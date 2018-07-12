using UnityEngine;
using uAdventure.Core;
using uAdventure.Runner;
using System.Collections.Generic;
using MapzenGo.Models;

namespace uAdventure.Geo
{
    public class ExtElemFactory : MapElementFactory
    {

        private Dictionary<ExtElemReference, Element> cache;
        public GameObject extElementPrefab;

        public override void Awake()
        {
            base.Awake();
            cache = new Dictionary<ExtElemReference, Element>();
            _createdCache = new Dictionary<Tile, List<GeoWrapper>>();
            Query = (elem, tile) => elem is ExtElemReference;
        }

        private Element FindElement(ExtElemReference reference)
        {
            if (!cache.ContainsKey(reference))
            {
                var elem = Game.Instance.GameState.FindElement<Element>(reference.getTargetId());
                cache.Add(reference, elem);
            }

            return cache.ContainsKey(reference) ? cache[reference] : null;
        }

        private Dictionary<Tile, List<GeoWrapper>> _createdCache;

        protected override IEnumerable<MonoBehaviour> Create(Tile tile, MapElement mapElement)
        {
            var extRef = mapElement as ExtElemReference;
            var element = FindElement(extRef);

            // The geometry is inside of the ti
            var geoWrapper = Instantiate(extElementPrefab).GetComponent<GeoWrapper>();

            geoWrapper.Reference = extRef;
            geoWrapper.Element = element;
            geoWrapper.Tile = tile;

            if (!_createdCache.ContainsKey(tile))
            {
                _createdCache.Add(tile, new List<GeoWrapper>());
            }

            _createdCache[tile].Add(geoWrapper);

            yield return geoWrapper;

        }

        protected override IEnumerable<MapElement> Destroy(Tile tile, MapElement mapElement)
        {
            if (_createdCache.ContainsKey(tile))
            {
                var geoWrapper = _createdCache[tile].Find(mb => mb.Element.getId() == mapElement.getTargetId());
                if (geoWrapper != null)
                {
                    DestroyImmediate(geoWrapper.gameObject);
                    _createdCache[tile].Remove(geoWrapper);
                    yield return mapElement;
                }
            }

            yield return null;
        }
    }

}
using UnityEngine;
using uAdventure.Core;
using uAdventure.Runner;
using System.Collections.Generic;
using MapzenGo.Models;

namespace uAdventure.Geo
{
    public class ExtElemFactory : MapElementFactory
    {
        public GameObject Character_Prefab;
        public GameObject Atrezzo_Prefab;
        public GameObject Object_Prefab;

        private Dictionary<ExtElemReference, Element> cache;

        private Dictionary<Tile, List<GeoPositioner>> _createdCache;

        public override void Awake()
        {
            base.Awake();
            cache = new Dictionary<ExtElemReference, Element>();
            _createdCache = new Dictionary<Tile, List<GeoPositioner>>();
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

        protected override IEnumerable<MonoBehaviour> Create(Tile tile, MapElement mapElement)
        {

            var extRef = mapElement as ExtElemReference;
            var element = FindElement(extRef);


            GameObject base_prefab = null;
            if (element is Atrezzo)
            {
                base_prefab = Atrezzo_Prefab;
            }
            else if (element is NPC)
            {
                base_prefab = Character_Prefab;
            }
            else if(element is Item)
            {
                base_prefab = Object_Prefab;
            }
            else
            {
                yield return null;
            }

            GameObject ret = Instantiate(base_prefab);
            var representable = ret.GetComponent<Representable>();
            representable.Context = extRef;
            representable.Element = element;

            // The geometry is inside of the ti
            var geoWrapper = ret.AddComponent<GeoPositioner>();
            geoWrapper.Tile = tile;
            geoWrapper.Context = extRef;
            geoWrapper.Element = element;
            geoWrapper.Representable = representable;

            if (!_createdCache.ContainsKey(tile))
            {
                _createdCache.Add(tile, new List<GeoPositioner>());
            }

            _createdCache[tile].Add(geoWrapper);
            representable.Adaptate();

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
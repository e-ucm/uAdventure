using UnityEngine;
using System.Collections;
using uAdventure.Geo;
using uAdventure.Core;
using uAdventure.Runner;
using System.Collections.Generic;
using MapzenGo.Models;
using ClipperLib;

public class ExtElemFactory : MapElementFactory {
    
    private Dictionary<ExtElemReference, Element> cache;
    public GameObject extElementPrefab;

    private Clipper clipper;

    public override void Awake()
    {
        base.Awake();
        cache = new Dictionary<ExtElemReference, Element>();
        clipper = new Clipper();
        Query = (elem,tile) => elem is ExtElemReference;// && Intersects(tile, elem as GeoReference);
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

        // The geometry is inside of the ti
        var geoWrapper = Instantiate(extElementPrefab).GetComponent<GeoWrapper>();
        
        geoWrapper.Reference = extRef;
        geoWrapper.Element = element;
        geoWrapper.Tile = tile;

        yield return geoWrapper;

    }
}

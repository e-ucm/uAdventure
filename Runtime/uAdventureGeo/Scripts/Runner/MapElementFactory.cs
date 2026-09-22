using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MapzenGo.Models.Plugins;
using MapzenGo.Models;

namespace uAdventure.Geo
{
    public class MapElementFactory : Plugin
    {
        public override List<Plugin> Dependencies
        {
            get
            {
                return new List<Plugin>() { uAdventurePlugin };
            }
        }

        public float Order = 1;
        public virtual Func<MapElement, Tile, bool> Query { get; set; }
        
        protected uAdventurePlugin uAdventurePlugin;

        public virtual void Awake()
        {
            uAdventurePlugin = FindObjectOfType<uAdventurePlugin>();
            Query = (mapElement, tile) => true;
        }

        protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {
            

            foreach(var elem in new List<MapElement>(uAdventurePlugin.OrphanElements))
            {
                if (uAdventurePlugin.OrphanElements.Contains(elem) && Query(elem, tile))
                {
                    foreach(var createdElem in Create(tile, elem))
                    {
                        if(createdElem != null)
                        {
                            createdElem.transform.SetParent(tile.transform, false);
                            uAdventurePlugin.AdoptElement(elem);

                            var geoPositioner = createdElem.gameObject.GetComponent<GeoPositioner>();
                            if (geoPositioner)
                            {
                                uAdventurePlugin.MapSceneMB.geoPositioners.Add(geoPositioner);
                            }
                            else
                            {
                                var geoElement = createdElem.gameObject.GetComponent<GeoElementMB>();
                                if (geoElement)
                                {
                                    uAdventurePlugin.MapSceneMB.geoElements.Add(geoElement);
                                }
                            }
                        }
                    }
                }
            }

            /*foreach (var entity in uAdventurePlugin.OrphanElements.Where(x => Query(x, tile)).SelectMany(elem => Create(tile, elem)))
            {
                if (entity != null)
                {
                    entity.transform.SetParent(tile.transform, false);
                }
            }*/

            finished(true);

            yield return null;
        }

        protected override IEnumerator UnLoadRoutine(Tile tile, Action<bool> finished)
        {
            foreach (var elem in new List<MapElement>(uAdventurePlugin.AdoptedElements))
            {
                if (uAdventurePlugin.AdoptedElements.Contains(elem) && Query(elem, tile))
                {
                    foreach (var destroyed in Destroy(tile, elem))
                    {
                        if (destroyed != null)
                        {
                            uAdventurePlugin.ReleaseElement(destroyed);
                        }
                    }
                }
            }

            finished(true);

            yield return null;
        }

        protected virtual IEnumerable<MonoBehaviour> Create(Tile tile, MapElement mapElement)
        {
            return null;
        }

        protected virtual IEnumerable<MapElement> Destroy(Tile tile, MapElement mapElement)
        {
            return null;
        }
    }
}
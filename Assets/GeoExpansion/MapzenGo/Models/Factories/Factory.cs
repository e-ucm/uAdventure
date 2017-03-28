using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapzenGo.Models.Plugins;
using UnityEngine;

namespace MapzenGo.Models.Factories
{
    public class Factory : Plugin
    {
        public bool MergeMeshes;
        public bool JustDrawEverythingFam = false;
        public float Order = 1;
        public virtual string XmlTag {get { return ""; } }
        public virtual Func<JSONObject, bool> Query { get; set; }
        

        public virtual void Start()
        {
            Query = (geo) => true;
        }

        protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {

            if (MergeMeshes)
            {
                if (!tile.Data.HasField(XmlTag))
                {
                    finished(false);
                    yield break;
                }

                var b = CreateLayer(tile, tile.Data[XmlTag]["features"].list);
                if (b) //getting a weird error without this, no idea really
                    b.transform.SetParent(tile.transform, false);
            }
            else
            {
                if (!(tile.Data.HasField(XmlTag) && tile.Data[XmlTag].HasField("features")))
                {
                    finished(false);
                    yield break;
                }

                foreach (var entity in tile.Data[XmlTag]["features"].list.Where(x => Query(x)).SelectMany(geo => Create(tile, geo)))
                {
                    if (entity != null)
                    {
                        entity.transform.SetParent(tile.transform, false);
                    }
                }
            }

            finished(true);
        }

        protected virtual IEnumerable<MonoBehaviour> Create(Tile tile, JSONObject geo)
        {
            return null;
        }

        protected virtual GameObject CreateLayer(Tile tile, List<JSONObject> toList)
        {
            return null;
        }

        
    }
}

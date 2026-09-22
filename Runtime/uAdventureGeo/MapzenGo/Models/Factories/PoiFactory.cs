using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MapzenGo.Helpers;
using MapzenGo.Models.Factories;
using MapzenGo.Models.Settings;
using UnityEngine;
using UnityEngine.UI;
using System;
using MapzenGo.Models.Plugins;

namespace MapzenGo.Models
{
    public class PoiFactory : Factory
    {
        [SerializeField] private GameObject _labelPrefab;
        [SerializeField] private GameObject _container;
        public override string XmlTag { get { return "pois"; } }
        [SerializeField] protected PoiFactorySettings FactorySettings;

        public override List<Plugin> Dependencies
        {
            get
            {
                return new List<Plugin> { FindObjectOfType<VectorDataPlugin>() };
            }
        }

        public override void Start()
        {
            base.Start();
            Query = (geo) => geo["geometry"]["type"].str == "Point" && geo["properties"].HasField("name");
        }

        protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {
            if (!(tile.Data.HasField(XmlTag) && tile.Data[XmlTag].HasField("features")))
                yield break;

            foreach (var entity in tile.Data[XmlTag]["features"].list.Where(x => Query(x)).SelectMany(geo => Create(tile, geo)))
            {
                if (entity != null)
                {
                    entity.transform.SetParent(_container.transform, true);
                    //entity.transform.localScale = Vector3.one * 3/tile.transform.lossyScale.x;
                }
            }

            finished(true);
        }

        protected override IEnumerable<MonoBehaviour> Create(Tile tile, JSONObject geo)
        {
            var kind = geo["properties"]["kind"].str.ConvertToPoiType();

            if (!FactorySettings.HasSettingsFor(kind))
                yield break;

            var typeSettings = FactorySettings.GetSettingsFor<PoiSettings>(kind);

            var go = Instantiate(_labelPrefab);
            var poi = go.AddComponent<Poi>();
            poi.transform.SetParent(_container.transform, true);
            poi.GetComponentInChildren<Image>().sprite = typeSettings.Sprite;
            //if (geo["properties"].HasField("name"))
            //    go.GetComponentInChildren<TextMesh>().text = geo["properties"]["name"].str;

            var c = geo["geometry"]["coordinates"];
            var dotMerc = GM.LatLonToMeters(c[1].f, c[0].f);
            var localMercPos = dotMerc - tile.Rect.Center;
            go.transform.position = new Vector3((float) localMercPos.x, (float) localMercPos.y);
            var target = new GameObject("poiTarget");
            target.transform.position = localMercPos.ToVector3();
            target.transform.SetParent(tile.transform, false);
            poi.Stick(target.transform);

            SetProperties(geo, poi, typeSettings);

            yield return poi;
        }

        private static void SetProperties(JSONObject geo, Poi poi, PoiSettings typeSettings)
        {
            poi.Id = geo["properties"]["id"].ToString();
            if (geo["properties"].HasField("name"))
                poi.Name = geo["properties"]["name"].str;
            poi.Type = geo["type"].str;
            poi.Kind = geo["properties"]["kind"].str;
            poi.name = "poi";
        }
    }
}

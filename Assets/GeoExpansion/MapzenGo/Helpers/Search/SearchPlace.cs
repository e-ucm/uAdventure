using System.Collections.Generic;
using MapzenGo.Models;
using UniRx;
using UnityEngine;
using System;

namespace MapzenGo.Helpers.Search
{
    [ExecuteInEditMode]
    [AddComponentMenu("Mapzen/SearchPlace")]
    public class SearchPlace : MonoBehaviour
    {
        const string seachUrl = "http://nominatim.openstreetmap.org/search/{0}?format=json";
        public string namePlace = "";
        public string namePlaceСache = "";
        public StructSearchData DataStructure;

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
        }
        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }

#endif
        }

        public void SearchInOSM()
        {
            if (namePlace != string.Empty && namePlaceСache != namePlace)
            {
                namePlaceСache = namePlace;
                ObservableWWW.Get(String.Format(seachUrl, System.Uri.EscapeDataString(namePlaceСache))).Subscribe(
                    success =>
                    {
                        DataProcessingOSM(success);
                    },
                    error =>
                    {
                        Debug.Log(error);
                    });
            }
        }

        public void SetupToTileManager(float Latitude, float Longitude)
        {
            TileManager tm = GetComponent<TileManager>();
            tm.Latitude = Latitude;
            tm.Longitude = Longitude;
        }

        public void DataProcessingOSM(string success)
        {
            JSONObject obj = new JSONObject(success);
            DataStructure.dataChache = new List<SearchData>();
            foreach (JSONObject jsonObject in obj.list)
            {
                DataStructure.dataChache.Add(new SearchData()
                {
                    coordinates = new Vector2(float.Parse(jsonObject["lon"].str), float.Parse(jsonObject["lat"].str)),
                    label = jsonObject["display_name"].str
                });
            }
        }

        public void DataProcessingMapzen(string success)
        {
            JSONObject obj = new JSONObject(success);
            DataStructure.dataChache = new List<SearchData>();
            foreach (JSONObject jsonObject in obj["features"].list)
            {
                DataStructure.dataChache.Add(new SearchData()
                {
                    coordinates = new Vector2(jsonObject["geometry"]["coordinates"][0].f, jsonObject["geometry"]["coordinates"][1].f),
                    label = jsonObject["properties"]["label"].str
                });
            }
        }
    }
}
